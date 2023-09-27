using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;


namespace Main.Aggregator.Events.Behaviours.Visual.AliveAnimationBehaviour
{
    public class BirthAnimationProperty : SharedPropertyEvent<Main.Aggregator.Helpers.Animation.AnimationEventInfo>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Visual.AliveAnimationBehaviour
{
    public class BirthAnimationProperty : SharedProperty<Main.Aggregator.Helpers.Animation.AnimationEventInfo, Main.Aggregator.Events.Behaviours.Visual.AliveAnimationBehaviour.BirthAnimationProperty>
    {
        public override string GroupTag => "Animation";
        public override string SharedName => "BirthAnimation";
    }
}


namespace Main.Aggregator.Events.Behaviours.Visual.AliveAnimationBehaviour
{
    public class DeathAnimationProperty : SharedPropertyEvent<Main.Aggregator.Helpers.Animation.AnimationEventInfo>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Visual.AliveAnimationBehaviour
{
    public class DeathAnimationProperty : SharedProperty<Main.Aggregator.Helpers.Animation.AnimationEventInfo, Main.Aggregator.Events.Behaviours.Visual.AliveAnimationBehaviour.DeathAnimationProperty>
    {
        public override string GroupTag => "Animation";
        public override string SharedName => "DeathAnimation";
    }
}

namespace Main.Objects.Behaviours.Visual
{

    [Unique]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Main.Objects.Behaviours.Tools.AnimatorBehaviourWrapper))]
    public class AliveAnimationControllerBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.AliveBehaviour.IsAliveProperty IsAliveProperty { get; private set; }

        [SharedProperty]
        public Aggregator.Properties.Behaviours.Visual.AliveAnimationBehaviour.BirthAnimationProperty BirhtAnimationInfo { get; private set; }

        [SharedProperty]
        public Aggregator.Properties.Behaviours.Visual.AliveAnimationBehaviour.DeathAnimationProperty DeathAnimationInfo { get; private set; }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Behaviours.Common.AliveBehaviour.IsAliveProperty))]
        public void IsAlivePropertyViewer(Aggregator.Events.Behaviours.Common.AliveBehaviour.IsAliveProperty eventData)
        {
            if (eventData.PropertyValue)
                Event<Aggregator.Events.Tools.AnimatorWrapper.PlayEvent>(Container).Invoke(BirhtAnimationInfo.Value);
            else
                Event<Aggregator.Events.Tools.AnimatorWrapper.PlayEvent>(Container).Invoke(DeathAnimationInfo.Value);
        }
    }

}
