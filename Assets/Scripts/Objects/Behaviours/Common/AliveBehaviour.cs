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


namespace Main.Objects.Behaviours.Attributes
{
    /// <summary>
    /// Binds event while object is alive
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AliveEventAttribute : ManagedDelegateOfEventAttribute
    {
    }
}

namespace Main.Aggregator.Events.Behaviours.Common.AliveBehaviour
{
    public sealed class IsAliveProperty: SharedPropertyEvent<bool>
    {

    }
}

namespace Main.Aggregator.Properties.Behaviours.Common.AliveBehaviour
{
    public sealed class IsAliveProperty : SharedProperty<bool, Events.Behaviours.Common.AliveBehaviour.IsAliveProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "IsAlive";
    }
}

namespace Main.Objects.Behaviours.Common
{
    [Unique]
    public class AliveBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Common.AliveBehaviour.IsAliveProperty IsAlive { get; protected set; }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Behaviours.Common.AliveBehaviour.IsAliveProperty))]
        public void IsAlivePropertyViewer(Aggregator.Events.Behaviours.Common.AliveBehaviour.IsAliveProperty eventData)
        {
            foreach (ObjectBehavioursBase behaviour in GetComponents<ObjectBehavioursBase>())
            {
                EnabledWhileAliveAttribute whileAliveAttr = behaviour.TypeReflector.GetCustomAttribute<EnabledWhileAliveAttribute>(true);
                EnableOnAliveAttribute onAliveAttr = behaviour.TypeReflector.GetCustomAttribute<EnableOnAliveAttribute>(true);
                DisableOnDieAttribute onDieAttr = behaviour.TypeReflector.GetCustomAttribute<DisableOnDieAttribute>(true);

                if (whileAliveAttr != null)
                {
                    whileAliveAttr.Handle(whileAliveAttr.GetType(), this, behaviour, 
                        (eventData.PropertyValue) ? BehaviourBinaryAttributeHandleDirection.One : BehaviourBinaryAttributeHandleDirection.Zero);
                }

                if (onAliveAttr != null)
                {
                    onAliveAttr.Handle(onAliveAttr.GetType(), this, behaviour,
                        (eventData.PropertyValue) ? BehaviourBinaryAttributeHandleDirection.One : BehaviourBinaryAttributeHandleDirection.Zero);
                }
                
                if (onDieAttr != null)
                {
                    onDieAttr.Handle(onDieAttr.GetType(), this, behaviour,
                        (!eventData.PropertyValue) ? BehaviourBinaryAttributeHandleDirection.One : BehaviourBinaryAttributeHandleDirection.Zero);
                }
            }

            if (IsAlive.Value)
                ActivateDelegateAttribute(typeof(AliveEventAttribute));
            else
                DeactivateDelegateAttribute(typeof(AliveEventAttribute));
        }

        protected override bool DoDisable()
        {
            bool prevIsAlive = IsAlive.Value;
            IsAlive.Value = false;

            if (!base.DoDisable())
            {
                IsAlive.Value = prevIsAlive;
                return false;
            }

            return true;
        }
    }
}