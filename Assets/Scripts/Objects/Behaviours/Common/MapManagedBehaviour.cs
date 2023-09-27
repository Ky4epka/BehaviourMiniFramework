using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;


namespace Main.Aggregator.Events.Behaviours.Common.MapManagedBehaviour.Map
{
    public class MapProperty : SharedPropertyEvent<Main.Map>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map
{
    public class MapProperty : SharedPropertyReference<Main.Map, Main.Aggregator.Events.Behaviours.Common.MapManagedBehaviour.Map.MapProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "Map";
    }
}

namespace Main.Objects.Behaviours.Common
{
    [Unique]
    public class MapManagedBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty MapProperty { get; protected set; }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty))]
        public void MapPropertyPropertyViewer(Main.Aggregator.Events.Behaviours.Common.MapManagedBehaviour.Map.MapProperty eventData)
        {
            if (eventData.PrevValue != null)
                eventData.PrevValue.RemoveEventListener<Aggregator.Events.Tools.OnBehaviourContainerDestroyedEvent>(MapContainerDestroyEvent);

            if (eventData.PropertyValue != null)
                eventData.PropertyValue.AddEventListener<Aggregator.Events.Tools.OnBehaviourContainerDestroyedEvent>(MapContainerDestroyEvent);
        }

        public void MapContainerDestroyEvent(Aggregator.Events.Tools.OnBehaviourContainerDestroyedEvent eventData)
        {
            MapProperty.Value = null;
        }

        protected override void OnDestroy()
        {
            MapProperty.Value = null;
            base.OnDestroy();
        }

    }

}