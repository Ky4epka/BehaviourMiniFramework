using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Player;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using System;

namespace Main.Aggregator.Events.Behaviours.Common.PlayerOwnershipBehaviour
{
    public sealed class OwningPlayerProperty: SharedPropertyEvent<PlayerBase>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Common.PlayerOwnershipBehaviour
{
    public sealed class OwningPlayerProperty : SharedPropertyReference<PlayerBase, Aggregator.Events.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "OwningPlayer";
    }
}

namespace Main.Objects.Behaviours.Common
{

    public class PlayerOwnershipBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty OwningPlayer { get; protected set; }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty))]
        public void OwningPlayerPropertyViewer(Aggregator.Events.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty eventData)
        {
            eventData.PrevValue?.Event<Aggregator.Events.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty>(Container).Invoke(OwningPlayer, eventData.PropertyValue, eventData.PrevValue);
            eventData.PropertyValue?.Event<Aggregator.Events.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty>(Container).Invoke(OwningPlayer, eventData.PropertyValue, eventData.PrevValue);
        }

    }
}