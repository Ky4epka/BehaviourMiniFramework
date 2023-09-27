using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Tools;
using Main.Objects.Behaviours.Attributes;

namespace Main.Aggregator.Events.MapCell
{
    public sealed class SelectedPrefabProperty : SharedPropertyEvent<GameObject>
    {
        public SelectedPrefabProperty() : base()
        {
        }
    }

    public sealed class SelectedProperty : SharedPropertyEvent<bool>
    {
        public SelectedProperty() : base()
        {
        }
    }
}

namespace Main.Aggregator.Properties.MapCell
{
    public sealed class SelectedPrefabProperty : SharedPropertyReference<GameObject, Aggregator.Events.Tools.PrefabInjector.PrefabProperty>
    {
        public override string GroupTag => "EditorStates";

        public override string SharedName => "SelectedPrefab";
    }


    public sealed class SelectedProperty : SharedProperty<bool, Aggregator.Events.Tools.PrefabInjector.PrefabVisibleStateProperty>
    {

        public override bool IsSerializable => false;
        public override string GroupTag => "EditorStates";

        public override string SharedName => "Selected";
    }
}

namespace Main
{

    public class MapCell_SelectedBehaviour : PrefabInjectorBaseBehaviour
    {
        [SharedProperty(typeof(Aggregator.Properties.MapCell.SelectedPrefabProperty))]
        public override ISharedPropertyReference<GameObject> Prefab { get; protected set; }
        [SharedProperty(typeof(Aggregator.Properties.MapCell.SelectedProperty))]
        public override ISharedProperty<bool> PrefabVisibleState { get; protected set; }
    }

}