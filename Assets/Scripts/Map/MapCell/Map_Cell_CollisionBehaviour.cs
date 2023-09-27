using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;
using System.Collections.Generic.ValueMap;


namespace Main.Aggregator.Helpers.MapCell
{
	[System.Serializable]
	public class CellCollisionTypeToMotionTypeCollisionMap : AddressableValueMap<Enum.Behaviours.Movable.MotionType, Aggregator.Enum.MapCell.CollisionFlags, bool>
	{
		public override bool AllocCell()
		{
			return false;
		}

		public override void AssignCell(bool source, ref bool destination)
		{
			destination = source;
		}

		public override void CellAddressChanged(ref bool cell, Enum.Behaviours.Movable.MotionType x, Aggregator.Enum.MapCell.CollisionFlags y)
		{
		}

		public override void InitCell(bool cell)
		{
		}

		public override void ReleaseCell(ref bool cell)
		{
		}
	}
}

namespace Main
{
	[RequireComponent(typeof(MapCell))]
	[RequireComponent(typeof(BoxCollider2D))]
	[Unique]
	public class Map_Cell_CollisionBehaviour : ObjectBehavioursBase
	{
		[SharedProperty]
		public Aggregator.Properties.MapCell.OwnerProperty Owner { get; private set; }
		[SharedProperty]
		public Aggregator.Properties.MapCell.IndexesProperty MapIndexes { get; private set; }
		[SharedProperty]
		public Aggregator.Properties.MapCell.CollisionFlagsProperty CollisionFlags { get; private set; }

		protected BoxCollider2D iCollider = null;
		protected IBehaviourContainer[] iCollisions;
		protected int iCollisionCount;

		[EnabledStateEvent]
		public void MapIndexesView(Aggregator.Events.MapCell.InvalidatePositionEvent eventData)
        {
			UpdateCollider();
        }

		public void UpdateCollider()
        {
			iCollider.size = Owner?.Value?.Common?.CellWorldSize?.Value ?? new Vector2(1, 1);
		}

		public bool AddCollision(IBehaviourContainer col)
		{
			if (InCollisionWith(col))
				return false;

			if (iCollisionCount >= Configuration.MAP_CELL_COLLISION_POOL_SIZE)
				return false;

			iCollisions[iCollisionCount++] = col;
			return true;
		}

		public bool RemoveCollisionByIndex(int index)
		{
			if (index == Configuration.INVALID_INDEX)
				return false;

			iCollisionCount--;

			if (iCollisionCount > 0)
			{
				iCollisions[index] = iCollisions[iCollisionCount];
			}

			return true;
		}

		public IBehaviourContainer GetCollisionByIndex(int index)
		{
			return iCollisions[index];
		}

		public bool RemoveCollision(IBehaviourContainer col)
		{
			return RemoveCollisionByIndex(GetCollisionIndex(col));
		}

		public void ClearCollisions()
		{
			int i = iCollisionCount - 1;
			while (i >= 0)
			{
				RemoveCollisionByIndex(i);
				i--;
			}
		}

		public int GetCollisionCount()
		{
			return iCollisionCount;
		}

		public bool InCollisionWith(IBehaviourContainer col)
		{
			return GetCollisionIndex(col) != Configuration.INVALID_INDEX;
		}

		public int GetCollisionIndex(IBehaviourContainer col)
		{
			for (int i = 0; i < iCollisionCount; i++)
			{
				if (iCollisions[i].Equals(col))
				{
					return i;
				}
			}

			return Configuration.INVALID_INDEX;
		}


        protected override void Awake()
        {
			base.Awake();

			iCollisions = new IBehaviourContainer[Configuration.MAP_CELL_COLLISION_POOL_SIZE];
			iCollisionCount = 0;
			iCollider = GetComponent<BoxCollider2D>();
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();

			ClearCollisions();
			iCollisions = null;
		}

        protected override void Start()
        {
            base.Start();
			UpdateCollider();
        }

        protected override bool DoEnable()
		{
			if (!base.DoEnable())
				return false;

			iCollider.enabled = true;
			return true;
		}

		protected override bool DoDisable()
		{
			if (!base.DoDisable())
				return false;

			iCollider.enabled = false;
			return true;
		}
	}
}