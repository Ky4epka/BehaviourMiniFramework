using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;
using UnityEditor;

namespace Main
{

	[RequireComponent(typeof(MapCell))]
	public class Map_Cell_PositioningBehaviour: ObjectBehavioursBase
	{
		[SharedProperty]
		public Aggregator.Properties.MapCell.OwnerProperty Owner { get; private set; }
		[SharedProperty]
		public Aggregator.Properties.MapCell.IndexesProperty MapIndexes { get; private set; }

		[SharedPropertyViewer(typeof(Aggregator.Properties.MapCell.IndexesProperty))]
		public void MapIndexesView(Aggregator.Events.MapCell.IndexesProperty eventData)
		{
			DoInvalidatePosition();
		}

		[EnabledStateEvent]
		public void InvalidatePosition(Aggregator.Events.MapCell.InvalidatePositionEvent eventData)
        {
			DoInvalidatePosition();
        }

		protected void DoInvalidatePosition()
		{
			transform.localPosition = Owner.Value?.Common.MapIndexesToLocalCellCenter(MapIndexes.Value) ?? Vector3.zero;
		}

        protected override void Awake()
        {
            base.Awake();
			AddEventListener<Aggregator.Events.MapCell.InvalidatePositionEvent>(InvalidatePosition);
			InvokeEvent<Aggregator.Events.MapCell.InvalidatePositionEvent>(null);
		}
    }
}