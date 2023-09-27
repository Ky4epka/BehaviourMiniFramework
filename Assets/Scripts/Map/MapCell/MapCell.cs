using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using System.IO;
using Main.Events;


namespace Main
{
    namespace Aggregator.Events.MapCell
    {
        public sealed class IndexesProperty : SharedPropertyEvent<Vector2Int>
        {
            public IndexesProperty() : base() { }
        }
        public sealed class OwnerProperty : SharedPropertyEvent<Main.Map>
        {
            public OwnerProperty() : base() { }
        }

        public sealed class CollisionFlagsProperty : SharedPropertyEvent<Aggregator.Enum.MapCell.CollisionFlags>
        {
        }

        public sealed class ModificatorsProperty : SharedPropertyEvent<Map_Cell_Modificators.Enum>
        {
            public ModificatorsProperty() : base()
            {
            }
        }

        public sealed class SpriteNameProperty : SharedPropertyEvent<string>
        {
        }

        public sealed class SpriteProperty : SharedPropertyEvent<Sprite>
        {
            public SpriteProperty() : base()
            {
            }
        }

        public sealed class InvalidatePositionEvent : EventDataBase
        {
            public InvalidatePositionEvent() : base()
            {
            }
        }

    }

	namespace Aggregator.Properties.MapCell
    {

        [System.Serializable]
        public sealed class OwnerProperty : SharedPropertyReference<Main.Map, Events.MapCell.OwnerProperty>
        {
            public override string SharedName => "Owner";
            public override string GroupTag => "Common";

        }

        [System.Serializable]
        public sealed class IndexesProperty : SharedProperty<Vector2Int, Events.MapCell.IndexesProperty>
        {
            public override string SharedName => "Map indexes";
            public override string GroupTag => "Common";
		}

		public sealed class CollisionFlagsProperty : SharedEnumProperty<Aggregator.Enum.MapCell.CollisionFlags, Events.MapCell.CollisionFlagsProperty>
        {
            public override string SharedName => "Collision flags";

            public override string GroupTag => "Common";

		}

        public sealed class ModificatorsProperty : SharedEnumProperty<Map_Cell_Modificators.Enum, Events.MapCell.ModificatorsProperty>
        {
            public override string SharedName => "Modificators";
            public override string GroupTag => "Common";


        }

        public sealed class PathCost : SharedProperty<int, Events.MapCell.PathCostProperty>
        {
            public override string SharedName => "Path cost";
            public override string GroupTag => "Common";

        }

        public sealed class SpriteNameProperty : SharedPropertyReference<string, Aggregator.Events.MapCell.SpriteNameProperty>
        {
            public override string SharedName => "Sprite name";
            public override string GroupTag => "Common";

        }

        public sealed class SpriteProperty : SharedPropertyReference<Sprite, Aggregator.Events.MapCell.SpriteProperty>
        {
            public override string GroupTag => "Common";

            public override string SharedName => "Sprite";
        }

    }


    [System.Serializable]
    public class MapCell: BehaviourContainer
    {
        public void UpdateCellData()
        {
        }

        public void Alloc()
        {
        }

        public void Release()
        {
        }

        public void Assign(MapCell source)
        {
            AssignSharedProperties(source);
        }
    }
}