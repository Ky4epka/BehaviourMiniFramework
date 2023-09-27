using Main.Objects.Behaviours.Attributes;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using System.Collections.Generic;
using Main.Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour;

namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class DoSetTargetForMotion: SharedPropertyEvent<object>
    {
        public object Target { get; private set; }

        public void Invoke(object target)
        {
            Target = target;
            base.Invoke();
        }
    }
}

namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class TargetPointForNearestPathProperty : SharedPropertyEvent<Vector2Int>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class TargetPointForNearestPathProperty : SharedProperty<Vector2Int, Main.Aggregator.Events.Behaviours.Movable.TargetPointForNearestPathProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "TargetPointForNearestPath";
    }
}


namespace Main.Objects.Behaviours.Movable
{
}

namespace Main.Objects.Behaviours.Movable
{
    public class TargetPointNearestPathMotionBehaviour : MultiCellMotionBehaviour
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty Map { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.TargetPointForNearestPathProperty TargetPointForNearestPath { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.MapPositionProperty MapPosition { get; protected set; }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.TargetPointForNearestPathProperty))]
        public void TargetPointForNearestPathPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.TargetPointForNearestPathProperty eventData)
        {
            if (iMapPathFindExt == null)
                return;

            Vector2Int targetPoint = Map.Value.Common.ValidateMapIndexes(eventData.PropertyValue);

            List<Vector2Int> path = new List<Vector2Int>();

            if (iMapPathFindExt.FindPathAlgorithmAStar(MapPosition.Value, targetPoint, path, (cell) => { return true; }, true))
                TargetCellsSteps.Value = path.ToArray();
        }

        [EnabledStateEvent]
        public virtual void OnDoSetTargetForMotion(Main.Aggregator.Events.Behaviours.Movable.DoSetTargetForMotion eventData)
        {
            if (eventData.PropertyValue is Vector2Int)
                TargetPointForNearestPath.Value = (Vector2Int)eventData.PropertyValue;
            else if (eventData.PropertyValue is Vector2)
                TargetPointForNearestPath.Value = Map.Value.Common.LocalToMapIndexes((Vector2)eventData.PropertyValue);
            else if (eventData.PropertyValue is Vector3)
                TargetPointForNearestPath.Value = Map.Value.Common.WorldToMapIndexes((Vector2)eventData.PropertyValue);
        }


        protected MapExt.Map_PathFind iMapPathFindExt = null;

        public override void MapPropertyViewer(Aggregator.Events.Behaviours.Common.MapManagedBehaviour.Map.MapProperty eventData)
        {
            if (eventData.PrevValue != null)
            {
                iMapPathFindExt = null;
                eventData.PropertyValue.RemoveEventListener<Aggregator.Events.Tools.OnBehaviourEnableStateEvent>(MapBehavioursEnabledState);
            }

            if (eventData.PropertyValue != null)
            {
                iMapPathFindExt = eventData.PropertyValue.GetComponent<MapExt.Map_PathFind>();

                if ((iMapPathFindExt == null) || (!iMapPathFindExt.enabled))
                {
                    enabled = false;
                    Debug.LogWarning($"For using {typeof(TargetPointNearestPathMotionBehaviour).Name} needs map path find extension");
                }

                eventData.PropertyValue.AddEventListener<Aggregator.Events.Tools.OnBehaviourEnableStateEvent>(MapBehavioursEnabledState);
            }
        }

        public void MapBehavioursEnabledState(Aggregator.Events.Tools.OnBehaviourEnableStateEvent eventData)
        {
            if (!eventData.State && (eventData.BehaviourType.Equals(typeof(MapExt.Map_PathFind))) && (this))
                enabled = false;
        }


        protected override void DoEnd(EndMotionStatus status)
        {
            if (status == EndMotionStatus.Unreached)
            {
                TargetPointForNearestPath.DirtyValue();
                return;
            }

            base.DoEnd(status);
        }

    }
}