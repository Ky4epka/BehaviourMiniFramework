using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;

namespace Main.Aggregator.Events.Behaviours.Tanks.Bullets
{
    public sealed class DoFireEvent : EventDataBase
    {
        public IBehaviourContainer Owner { get; private set; }
        public Transform FirePoint { get; private set; }

        public void Invoke(IBehaviourContainer owner, Transform firePoint)
        {
            Owner = owner;
            FirePoint = firePoint;
            base.Invoke();
        }
    }
}



namespace Main.Aggregator.Events.Behaviours.Tanks.Bullets
{
    public class OwnerProperty : SharedPropertyEvent<IBehaviourContainer>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks.Bullets
{
    public class OwnerProperty : SharedPropertyReference<IBehaviourContainer, Main.Aggregator.Events.Behaviours.Tanks.Bullets.OwnerProperty>
    {
        public override string GroupTag => "Tanks.Bullets";
        public override string SharedName => "Owner";
    }
}


namespace Main.Objects.Behaviours.Tanks.Bullets
{

    [Unique]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Main.Objects.Behaviours.Common.AliveBehaviour))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Common.HitpointsBehaviour))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Common.DamageBehaviour))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Movable.RigidbodyMotionBehaviour))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Movable.LinearMovingBehaviour))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Tools.SpriteRendererBehaviourWrapper))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Tools.AnimatorBehaviourWrapper))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Visual.AliveAnimationControllerBehaviour))]
    [RequireComponent(typeof(Main.Objects.Behaviours.Common.Collision2DWrapperBehaviour))]
    public class BulletBaseBehaviour : Main.Objects.Behaviours.Tools.PoolableBehaviour
    {
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.LinearMovingBehaviour.MovingDirectionProperty MovingDirection { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.PositionProperty PositionProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.MapPositionProperty MapPositionProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty MapProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.AliveBehaviour.IsAliveProperty IsAliveProperty { get; private set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.Bullets.OwnerProperty Owner { get; protected set; }

        [EnabledStateEvent]
        public void DoFireEvent(Aggregator.Events.Behaviours.Tanks.Bullets.DoFireEvent eventData)
        {
            PositionProperty.Value = eventData.FirePoint.position;
            MovingDirection.Value = eventData.Owner.transform.up;
            Owner.Value = eventData.Owner;
            Event<Aggregator.Events.Behaviours.Common.Hitpoints.DoReviveEvent>(Container).Invoke();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Behaviours.Movable.MapPositionProperty))]
        public void MapPostionProeprtyViewer(Aggregator.Events.Behaviours.Movable.MapPositionProperty eventData)
        {
            if (!MapProperty.Value)
                return;

            if (!MapProperty.Value.Common.IsValidMapIndexes(eventData.PropertyValue))
            {
                Event<Aggregator.Events.Behaviours.Common.Hitpoints.DoKillEvent>(Container).Invoke();
                MovingDirection.Value = Vector2.zero;
            }
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Behaviours.Common.AliveBehaviour.IsAliveProperty))]
        public void IsAliveProeprtyViewer(Aggregator.Events.Behaviours.Common.AliveBehaviour.IsAliveProperty eventData)
        {
//            if (eventData.PrevValue == eventData.PropertyValue)
  //              return;

            if (eventData.PropertyValue)
            {
            }
            else
            {
                MovingDirection.Value = Vector2.zero;
                Event<Aggregator.Events.Behaviours.Tools.PoolableBehaviour.DoPoolReturnEvent>(Container).Invoke();
            }

            Event<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(Container).
                Invoke(
                    typeof(Movable.RigidbodyMotionBehaviour), 
                    eventData.PropertyValue);
            Event<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(Container).
                Invoke(
                    typeof(Movable.RigidbodyMotionBehaviour),
                    eventData.PropertyValue);
            Event<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(Container).
                Invoke(
                    typeof(Common.DamageBehaviour),
                    eventData.PropertyValue);
            Event<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(Container).
                Invoke(
                    typeof(Common.Collision2DWrapperBehaviour),
                    eventData.PropertyValue);
        }

        [EnabledStateEvent]
        public void OnCollision2DEnterEvent(Aggregator.Events.Behaviours.Common.Collision2DWrapper.OnTrigger2DEnterEvent eventData)
        {
            IBehaviourContainer contactObject = eventData.Collider.gameObject.GetComponent<IBehaviourContainer>();
            
            if ((contactObject?.Equals(Container) ?? false) ||
                (contactObject.SharedProperty<
                    Aggregator.
                    Properties.
                    Behaviours.
                    Common.
                    PlayerOwnershipBehaviour.
                    OwningPlayerProperty>().Value?.Data.IsAlliedObject(Owner.Value as BehaviourContainer) ?? false))
                return;

            Event<
                Aggregator.
                Events.
                Behaviours.
                Common.
                DamageBehaviour.
                DoDamageEvent>(Container).
                    Invoke(Owner.Value, contactObject);

            Event <Aggregator.Events.Behaviours.Common.Hitpoints.DoKillEvent>(Container).Invoke();
        }

    }

}