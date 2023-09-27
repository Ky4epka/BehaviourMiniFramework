using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;

namespace Main
{
    [RequireComponent(typeof(MapCell))]
    [Unique]
    public class Map_Cell_PathBehaviour: ObjectBehavioursBase
    {
		[SharedProperty]
		public Aggregator.Properties.MapCell.PathCost PathCost { get; private set; }
    }
}