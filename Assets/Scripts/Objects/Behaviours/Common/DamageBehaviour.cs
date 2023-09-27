using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Aggregator.Enum.Behaviours.Common.Hitpoints;


namespace Main.Aggregator.Events.Behaviours.Common.DamageBehaviour
{
    public class DamageMinProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.DamageBehaviour
{
    public class DamageMinProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Common.DamageBehaviour.DamageMinProperty>
    {
        public override string GroupTag => "Damage";
        public override string SharedName => "DamageMin";
    }
}


namespace Main.Aggregator.Events.Behaviours.Common.DamageBehaviour
{
    public class DamageMaxProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.DamageBehaviour
{
    public class DamageMaxProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Common.DamageBehaviour.DamageMaxProperty>
    {
        public override string GroupTag => "Damage";
        public override string SharedName => "DamageMax";
    }
}

namespace Main.Aggregator.Events.Behaviours.Common.DamageBehaviour
{
    public class DoDamageEvent: EventDataBase
    {
        public IBehaviourContainer DamageSource { get; private set; }
        public IBehaviourContainer DamageTarget { get; private set; }

        public void Invoke(IBehaviourContainer damageSource, IBehaviourContainer damageTarget)
        {
            DamageSource = damageSource;
            DamageTarget = damageTarget;
            base.Invoke();
        }
    }
}


namespace Main.Aggregator.Events.Behaviours.Common.DamageBehaviour
{
    public class DamageTypeProperty : SharedPropertyEvent<DamageType>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.DamageBehaviour
{
    public class DamageTypeProperty : SharedEnumProperty<DamageType, Main.Aggregator.Events.Behaviours.Common.DamageBehaviour.DamageTypeProperty>
    {
        public override string GroupTag => "Damage";
        public override string SharedName => "DamageType";
    }
}

namespace Main.Aggregator.Events.Behaviours.Common.DamageBehaviour
{
    public class UseFriendlyFireProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.DamageBehaviour
{
    public class UseFriendlyFireProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Common.DamageBehaviour.UseFriendlyFireProperty>
    {
        public override string GroupTag => "Damage";
        public override string SharedName => "UseFirendlyFire";
    }
}

namespace Main.Objects.Behaviours.Common
{
    public class DamageBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.DamageBehaviour.DamageMinProperty DamageMin { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.DamageBehaviour.DamageMaxProperty DamageMax { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.DamageBehaviour.DamageTypeProperty DamageType { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.DamageBehaviour.UseFriendlyFireProperty UseFriendlyFire { get; private set; }


        [SharedPropertyHandler(typeof(Aggregator.Properties.Behaviours.Common.DamageBehaviour.DamageMinProperty), RehandleOnEnabled = true)]
        public bool DamageMinPropertyHandler(ISharedProperty property, float oldValue, ref float newValue)
        {
            if (newValue > DamageMax.Value)
                newValue = DamageMax.Value;

            return true;
        }

        [SharedPropertyHandler(typeof(Aggregator.Properties.Behaviours.Common.DamageBehaviour.DamageMaxProperty), RehandleOnEnabled = true)]
        public bool DamageMaxPropertyHandler(ISharedProperty property, float oldValue, ref float newValue)
        {
            if (newValue < DamageMin.Value)
                newValue = DamageMin.Value;

            return true;
        }

        [EnabledStateEvent]
        public void DoDamageEvent(Aggregator.Events.Behaviours.Common.DamageBehaviour.DoDamageEvent eventData)
        {
            if (eventData.DamageTarget == null)
            {
                Debug.LogError("Damage target argument is null", this);
                return;
            }

            if ((!UseFriendlyFire.Value) && 
                (eventData.
                DamageSource?.
                SharedProperty<
                    Aggregator.
                    Properties.
                    Behaviours.
                    Common.
                    PlayerOwnershipBehaviour.
                    OwningPlayerProperty>().
                        Value?.Data.IsAlliedPlayer(
                            eventData.
                            DamageTarget.
                            SharedProperty<
                                Aggregator.
                                Properties.
                                Behaviours.
                                Common.
                                PlayerOwnershipBehaviour.
                                OwningPlayerProperty>().Value) ?? false)) 
            {
                return;
            }

            eventData.DamageTarget.Event<Aggregator.Events.Behaviours.Common.Hitpoints.DoDamageEvent>(eventData.Sender).Invoke(CalcDamage(), DamageType.Value);
        }

        protected float CalcDamage()
        {
            return Random.Range(DamageMin.Value, DamageMax.Value);
        }
    }
}
