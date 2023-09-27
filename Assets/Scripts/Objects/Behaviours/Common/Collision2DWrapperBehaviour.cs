using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;
using System;
using System.Linq;

namespace Main.Other
{
    [Serializable]
    public class BehaviourContainerTypeWrapper: TypeWrapper
    {
        public override Type BaseType => typeof(Main.Objects.Behaviours.Common.Collision2DWrapperBehaviour);

        public BehaviourContainerTypeWrapper() : base()
        {
        }
    }

}


namespace Main.Aggregator.Events.Behaviours.Common.Collision2DWrapper
{
    public class OnCollision2DEnterEvent: EventDataBase
    {
        public Collision2D Collision { get; private set; }

        public void Invoke(Collision2D collision)
        {
            Collision = collision;
            base.Invoke();
        }
    }

    public class OnCollision2DExitEvent : EventDataBase
    {
        public Collision2D Collision { get; private set; }

        public void Invoke(Collision2D collision)
        {
            Collision = collision;
            base.Invoke();
        }
    }


    public class OnTrigger2DEnterEvent : EventDataBase
    {
        public Collider2D Collider { get; private set; }

        public void Invoke(Collider2D collider)
        {
            Collider = collider;
            base.Invoke();
        }
    }

    public class OnTrigger2DExitEvent : EventDataBase
    {
        public Collider2D Collider { get; private set; }

        public void Invoke(Collider2D collider)
        {
            Collider = collider;
            base.Invoke();
        }
    }
}



namespace Main.Aggregator.Events.Behaviours.Common.Collision2DWrapper
{
    public class WhiteListProperty : SharedPropertyEvent<Main.Other.BehaviourContainerTypeWrapper>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.Collision2DWrapper
{
    public class WhiteListProperty : SharedPropertyReference<Main.Other.BehaviourContainerTypeWrapper, Main.Aggregator.Events.Behaviours.Common.Collision2DWrapper.WhiteListProperty>
    {
        public override string GroupTag => "Collision";
        public override string SharedName => "ComponentsWhiteList";
    }
}

namespace Main.Aggregator.Events.Behaviours.Common.Collision2DWrapper
{
    public class UseWhiteListProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.Collision2DWrapper
{
    public class UseWhiteListProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Common.Collision2DWrapper.UseWhiteListProperty>
    {
        public override string GroupTag => "Collision";
        public override string SharedName => "UseWhiteList";
    }
}


namespace Main.Aggregator.Events.Behaviours.Common.Collision2DWrapper
{
    public class ReactOnlyOnObjectsWithThisBehaviourProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Common.Collision2DWrapper
{
    public class ReactOnlyOnObjectsWithThisBehaviourProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Common.Collision2DWrapper.ReactOnlyOnObjectsWithThisBehaviourProperty>
    {
        public override string GroupTag => "Collision";
        public override string SharedName => "ReactOnlyOnObjectsWithThisBehaviour";
    }
}

namespace Main.Objects.Behaviours.Common
{
    [Unique]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Collision2DWrapperBehaviour : ObjectBehavioursBase
    {
        [SharedProperty(DefaultConstructorIfDefaultReferenceValue = true)]
        public Aggregator.Properties.Behaviours.Common.Collision2DWrapper.ReactOnlyOnObjectsWithThisBehaviourProperty ReactOnlyOnObjectsWithThisBehaviour { get; protected set; }
        
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (CollisionFilter(collision.gameObject))
                Event<Aggregator.Events.Behaviours.Common.Collision2DWrapper.OnCollision2DEnterEvent>(Container).Invoke(collision);
        }

        protected void OnCollisionExit2D(Collision2D collision)
        {
            if (CollisionFilter(collision.gameObject))
                Event<Aggregator.Events.Behaviours.Common.Collision2DWrapper.OnCollision2DExitEvent>(Container).Invoke(collision);
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (CollisionFilter(collision.gameObject))
                Event<Aggregator.Events.Behaviours.Common.Collision2DWrapper.OnTrigger2DEnterEvent>(Container).Invoke(collision);
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (CollisionFilter(collision.gameObject))
                Event<Aggregator.Events.Behaviours.Common.Collision2DWrapper.OnTrigger2DExitEvent>(Container).Invoke(collision);
        }

        protected virtual bool CollisionFilter(GameObject collideWith)
        {
            if (ReactOnlyOnObjectsWithThisBehaviour.Value)
            {
                return collideWith.GetComponent<Collision2DWrapperBehaviour>();
            }

            return true;
        }

    }
}