using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Tools;
using Main.Objects.Behaviours.Attributes;

namespace Main.Aggregator.Events.MapCell
{
    public sealed class HoveredPrefabProperty : SharedPropertyEvent<GameObject>
    {
        public HoveredPrefabProperty() : base()
        {
        }
    }

    public sealed class HoveredProperty : SharedPropertyEvent<bool>
    {
        public HoveredProperty() : base()
        {
        }
    }
}

namespace Main.Aggregator.Properties.MapCell
{
    public sealed class HoveredPrefabProperty : SharedPropertyReference<GameObject, Aggregator.Events.Tools.PrefabInjector.PrefabProperty>
    {
        public override string GroupTag => "EditorStates";

        public override string SharedName => "HoveredPrefab";
    }


    public sealed class HoveredProperty : SharedProperty<bool, Aggregator.Events.Tools.PrefabInjector.PrefabVisibleStateProperty>
    {
        public override bool IsSerializable => false;

        public override string GroupTag => "EditorStates";

        public override string SharedName => "Hovered";
    }
}

namespace Main
{

    public class MapCell_HoveredBehaviour : PrefabInjectorBaseBehaviour
    {
        [SharedProperty(typeof(Aggregator.Properties.MapCell.HoveredPrefabProperty))]
        public override ISharedPropertyReference<GameObject> Prefab { get; protected set; }
        [SharedProperty(typeof(Aggregator.Properties.MapCell.HoveredProperty))]
        public override ISharedProperty<bool> PrefabVisibleState { get; protected set; }
    }

}