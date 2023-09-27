using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;

namespace Main.Aggregator.Enum.Behaviours.Movable
{
    public enum MotionType
    {
        Unknown = 0,
        Ground,
        Air,
        Water
    }
}

namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class MotionTypeProperty : SharedPropertyEvent<Aggregator.Enum.Behaviours.Movable.MotionType>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class MotionTypeProperty : SharedEnumProperty<Aggregator.Enum.Behaviours.Movable.MotionType, Main.Aggregator.Events.Behaviours.Movable.MotionTypeProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "MotionType";
    }
}



namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class LockPositioningProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class LockPositioningProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Movable.LockPositioningProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "LockPositioning";
    }
}

namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class PositionProperty : SharedPropertyEvent<Vector2>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Movable
{
    /// <summary>
    /// Map position property is positioned in local map coordinates
    /// </summary>
    public class PositionProperty : SharedProperty<Vector2, Main.Aggregator.Events.Behaviours.Movable.PositionProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "Position";
    }
}



namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class MapPositionProperty : SharedPropertyEvent<Vector2Int>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class MapPositionProperty : SharedProperty<Vector2Int, Main.Aggregator.Events.Behaviours.Movable.MapPositionProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "MapPosition";
    }
}




namespace Main.Objects.Behaviours.Movable
{

    [RequireComponent(typeof(Transform))]
    [Unique]
    [DisallowMultipleComponent]
    public class MapStaticMotionBehaviour : ObjectBehavioursBase, IMotionBehaviour
    {
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.PositionProperty PositionProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty MapProperty { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.LockPositioningProperty LockPositioningProperty { get; protected set; }


        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.MapPositionProperty MapPosition { get; protected set; }

        protected bool iLoopProtectionFlag = false;
        protected bool iFromPositionFlag = false;
        protected bool iFromMapIndexesFlag = false;
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.MotionTypeProperty MotionTypeProperty { get; protected set; }


        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.MapPositionProperty))]
        public void MapPositionPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.MapPositionProperty eventData)
        {
            //return;

            if (iFromPositionFlag)
                return;

            if (!MapProperty.Value)
            {
                GLog.Log("Can not change object '"+gameObject+"' map position because the map value is null");
                return;
            }

            try
            {
                iFromMapIndexesFlag = true;
                PositionProperty.Value = MapProperty.Value.Common.MapIndexesToLocalCellCenter(eventData.PropertyValue);
            }
            finally
            {
                iFromMapIndexesFlag = false;
            }
        }


        [SharedPropertyHandler(typeof(Main.Aggregator.Properties.Behaviours.Movable.PositionProperty))]
        public bool PositionPropertyHandler(ISharedProperty property, Vector2 oldValue, ref Vector2 newValue)
        {
            return !LockPositioningProperty.Value;
        }


        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.PositionProperty))]
        public void PositionPropertyPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.PositionProperty eventData)
        {
            Vector2 position;

            if (MapProperty.Value != null)
                position = MapProperty.Value.Common.LocalToWorld(eventData.PropertyValue);
            else
                position = eventData.PropertyValue;

            ApplyPosition(position);

            if (!iFromMapIndexesFlag && MapProperty.Value)
            {
                try
                {
                    iFromPositionFlag = true;
                    MapPosition.Value = MapProperty.Value.Common.LocalToMapIndexes(position);
                }
                finally
                {
                    iFromPositionFlag = false;
                }
            }
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty))]
        public void MapPropertyPropertyViewer(Main.Aggregator.Events.Behaviours.Common.MapManagedBehaviour.Map.MapProperty eventData)
        {
            if (eventData.PrevValue != null)
                eventData.PropertyValue.RemoveEventListener<Aggregator.Events.Map.CacheRecalcNotifyEvent>(MapCacheRecalcEvent);

            if (eventData.PropertyValue == null)
            {
                Debug.LogWarning($"Could not enable behaviour '{GetType().FullName}' of object '{gameObject}' for a reason: MapProperty.Value is null");
                enabled = false;
                return;
            }

            eventData.PropertyValue.AddEventListener<Aggregator.Events.Map.CacheRecalcNotifyEvent>(MapCacheRecalcEvent);
            PositionProperty.DirtyValue();
        }

        protected void MapCacheRecalcEvent(Aggregator.Events.Map.CacheRecalcNotifyEvent eventData)
        {
            PositionProperty.DirtyValue();
        }

        public bool MapCellIsPassable(MapCell cell)
        {
            if (MapProperty.Value == null)
                return true;

            if (cell == null)
                return false;

            Map_Cell_CollisionBehaviour cellColBeh = cell.GetComponent<Map_Cell_CollisionBehaviour>();
            bool cellHasSameMotionTypeObject = false;

            for (int i = 0; i < cellColBeh.GetCollisionCount(); i++)
                if (!cellColBeh.GetCollisionByIndex(i).Equals(Container) &&
                    cellColBeh.GetCollisionByIndex(i).SharedProperty<Aggregator.Properties.Behaviours.Movable.MotionTypeProperty>().Value == MotionTypeProperty.Value)
                {
                    cellHasSameMotionTypeObject = true;
                    break;
                }

            return Configuration.Instance.CellTypeToMotionTypeCollisionMapProperty.Value[cellColBeh.CollisionFlags.Value, MotionTypeProperty.Value] &&
                   !cellHasSameMotionTypeObject;
        }

        public bool MapCellIsPassable(Vector2Int cellIndexes)
        {
            MapCell cell = MapProperty.Value.Common.GetCellByMapIndexes(cellIndexes);
            return MapCellIsPassable(cell);
        }

        protected virtual void ApplyPosition(Vector2 position)
        {
            transform.position = position;
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            if (MapProperty.Value == null)
            {
                return false;
            }

            return true;
        }
    }
}