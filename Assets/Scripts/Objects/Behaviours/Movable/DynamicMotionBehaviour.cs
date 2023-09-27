using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;


namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class SpeedProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class SpeedProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Movable.SpeedProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "Speed";
    }
}


namespace Main.Objects.Behaviours.Movable
{
    public class DynamicMotionBehaviour : MapStaticMotionBehaviour
    {

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.SpeedProperty SpeedProperty { get; protected set; }

    }
}
