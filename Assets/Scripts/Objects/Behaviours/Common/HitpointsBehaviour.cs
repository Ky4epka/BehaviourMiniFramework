using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Aggregator.Enum.Behaviours.Common.Hitpoints;

namespace Main.Aggregator.Enum.Behaviours.Common.Hitpoints
{
    public enum DamageType
    {
        Regular = 0,
        God
    }

    public enum ArmourType
    {
        Regular = 0,
        Light,
        Medium,
        Heavy,
        God
    }
}

namespace Main.Aggregator.Helpers.Behaviours.Common.Hitpoints
{
    [System.Serializable]
    public class ArmourToDamageMap : System.Collections.Generic.ValueMap.AddressableValueMap<Aggregator.Enum.Behaviours.Common.Hitpoints.DamageType, Aggregator.Enum.Behaviours.Common.Hitpoints.ArmourType, float>
    {
        public override float AllocCell()
        {
            return 1f;
        }

        public override void AssignCell(float source, ref float destination)
        {
            destination = source;
        }

        public override void CellAddressChanged(ref float cell, DamageType x, ArmourType y)
        {
        }

        public override void InitCell(float cell)
        {
        }

        public override void ReleaseCell(ref float cell)
        {
        }
    }
}


namespace Main.Aggregator.Events.Behaviours.Common.Hitpoints
{
    public class HitpointsProperty : SharedPropertyEvent<float>
    {
    }

    public class HitpointsMinValueProperty : SharedPropertyEvent<float>
    {
    }

    public class HitpointsMaxValueProperty : SharedPropertyEvent<float>
    {
    }

    public class HitpointsMinBorderEvent : SharedPropertyEvent<float>
    {
    }

    public class HitpointsMaxBorderEvent : SharedPropertyEvent<float>
    {
    }

    public class DoReviveEvent : EventDataBase
    {

    }

    public class DoKillEvent: EventDataBase
    {

    }

    public class BaseDamageEvent : EventDataBase
    {

        public float Damage { get; private set; }
        public Enum.Behaviours.Common.Hitpoints.DamageType DamageType { get; private set; }

        public virtual void Invoke(float value, Enum.Behaviours.Common.Hitpoints.DamageType damageType)
        {
            Damage = value;
            DamageType = damageType;
            base.Invoke();
        }
    }

    public class DoDamageEvent : BaseDamageEvent
    {

    }

    public class OnDamageEvent : BaseDamageEvent
    {
        public float DealedDamage { get; private set; }

        public virtual void Invoke(float inputDamage, float dealedDamage, Enum.Behaviours.Common.Hitpoints.DamageType damageType)
        {
            DealedDamage = dealedDamage;
            base.Invoke(Damage, DamageType);
        }

    }
}

namespace Main.Aggregator.Properties.Behaviours.Common.Hitpoints
{
    public class HitpointsMinValueProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Common.Hitpoints.HitpointsMinValueProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "HitpointsMin";
    }

    public class HitpointsMaxValueProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Common.Hitpoints.HitpointsMaxValueProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "HitpointsMax";
    }

    public class HitpointsProperty : LimitedValueSharedProperty<
        float,
        Main.Aggregator.Events.Behaviours.Common.Hitpoints.HitpointsProperty,
        Main.Aggregator.Events.Behaviours.Common.Hitpoints.HitpointsMinBorderEvent,
        Main.Aggregator.Events.Behaviours.Common.Hitpoints.HitpointsMaxBorderEvent,
        Main.Aggregator.Properties.Behaviours.Common.Hitpoints.HitpointsMinValueProperty,
        Main.Aggregator.Properties.Behaviours.Common.Hitpoints.HitpointsMaxValueProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "Hitpoints";
    }
}


namespace Main.Aggregator.Events.Behaviours.Common.Hitpoints
{
    public class ArmourTypeProperty : SharedPropertyEvent<Main.Aggregator.Enum.Behaviours.Common.Hitpoints.ArmourType>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.Hitpoints
{
    public class ArmourTypeProperty : SharedEnumProperty<Main.Aggregator.Enum.Behaviours.Common.Hitpoints.ArmourType, Main.Aggregator.Events.Behaviours.Common.Hitpoints.ArmourTypeProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "ArmourType";
    }
}

namespace Main.Objects.Behaviours.Common
{
    [Unique]
    [RequireComponent(typeof(AliveBehaviour))]
    public class HitpointsBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.AliveBehaviour.IsAliveProperty AliveProperty { get; protected set; }


        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.Hitpoints.HitpointsProperty Hitpoints { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.Hitpoints.HitpointsMinValueProperty HitpointsMin { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.Hitpoints.HitpointsMaxValueProperty HitpointsMax { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.Hitpoints.ArmourTypeProperty ArmourType { get; protected set; }


        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Common.Hitpoints.HitpointsProperty))]
        public void HitpointsPropertyViewer(Main.Aggregator.Events.Behaviours.Common.Hitpoints.HitpointsProperty eventData)
        {
            bool isAlive = eventData.PropertyValue > HitpointsMin.Value;

            if (isAlive != AliveProperty.Value)
                AliveProperty.Value = isAlive;
        }

        [EnabledStateEvent]
        public void DoReviveEvent(Aggregator.Events.Behaviours.Common.Hitpoints.DoReviveEvent eventData)
        {
            Hitpoints.Value = HitpointsMax.Value;
            Hitpoints.DirtyValue();
        }

        [EnabledStateEvent]
        public void DoKillEvent(Aggregator.Events.Behaviours.Common.Hitpoints.DoKillEvent eventData)
        {
            Hitpoints.Value = HitpointsMin.Value;
            Hitpoints.DirtyValue();
        }

        [EnabledStateEvent]
        public void DoDamageEvent(Aggregator.Events.Behaviours.Common.Hitpoints.DoDamageEvent eventData)
        {
            if (!AliveProperty.Value)
                return;

            float dealedDamage = eventData.Damage * Configuration.Instance.ArmourTypeMap.Value.GetCell(eventData.DamageType, ArmourType.Value);

            if (Hitpoints.Value - dealedDamage <= HitpointsMin.Value)
                dealedDamage = HitpointsMin.Value;

            Hitpoints.Value = dealedDamage;
            Event<Aggregator.Events.Behaviours.Common.Hitpoints.OnDamageEvent>(Container).Invoke(eventData.Damage, dealedDamage, eventData.DamageType);
        }

    }
}