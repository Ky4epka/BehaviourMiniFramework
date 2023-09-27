using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;

namespace Main.Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour
{
    [System.Serializable]
    public enum EndMotionStatus
    {
        Success = 0,
        Cancel,
        Unreached
    }
}

namespace Main.Aggregator.Events.Behaviours.Movable.LinearMovingBehaviour
{
    public class MovingDirectionProperty : SharedPropertyEvent<Vector2>
    {
    }

    public class OnStartMotionEvent: EventDataBase
    {
        public Vector2 Direction { get; private set; }

        public void Invoke(Vector2 direction)
        {
            Direction = direction;
            base.Invoke();
        }
    }

    public class OnEndMotionEvent : EventDataBase
    {
        public Vector2 Direction { get; private set; }
        public Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour.EndMotionStatus EndStatus { get; private set; }

        public void Invoke(Vector2 direction, Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour.EndMotionStatus endStatus)
        {
            Direction = direction;
            EndStatus = endStatus;
            base.Invoke();
        }
    }

    public class OnFinishMotionEvent : EventDataBase
    {
        public Vector2 Direction { get; private set; }

        public void Invoke(Vector2 direction)
        {
            Direction = direction;
            base.Invoke();
        }
    }


    public class DoCancelMotionEvent : EventDataBase
    {
        public Vector2 Direction { get; private set; }

        public void Invoke(Vector2 direction)
        {
            Direction = direction;
            base.Invoke();
        }
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable.LinearMovingBehaviour
{
    public class MovingDirectionProperty : SharedProperty<Vector2, Main.Aggregator.Events.Behaviours.Movable.LinearMovingBehaviour.MovingDirectionProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "MovingDirection";
    }
}


namespace Main.Objects.Behaviours.Movable
{
    [Unique]
    [RequireComponent(typeof(DynamicMotionBehaviour), typeof(Rotation.RotationBehaviour))]
    [IncompatibleBehaviours(typeof(IMotionBehaviourController), includeInheritedBehaviours = true)]
    [DisallowMultipleComponent]
    public class LinearMovingBehaviour : ObjectBehavioursBase, IMotionBehaviourController
    {

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.SpeedProperty SpeedProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.PositionProperty PositionProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.LinearMovingBehaviour.MovingDirectionProperty MovingDirection { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.IsMovingProperty IsMoving { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.RotationAngleProperty RotationAngle { get; protected set; }

        protected bool iInMotion = false;

        [EnabledStateEvent]
        public void DoCancelMotionEvent(Aggregator.Events.Behaviours.Movable.LinearMovingBehaviour.DoCancelMotionEvent eventData)
        {
            if (!iInMotion)
                return;

            DoEnd(MovingDirection.Value, Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour.EndMotionStatus.Cancel);
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.LinearMovingBehaviour.MovingDirectionProperty))]
        public void MovingDirectionPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.LinearMovingBehaviour.MovingDirectionProperty eventData)
        {
            if (!MathKit.Vectors2DEquals(eventData.PrevValue, Vector2.zero) && !MathKit.Vectors2DEquals(eventData.PrevValue, eventData.PropertyValue) && iInMotion)
            {
                DoEnd(eventData.PrevValue, Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour.EndMotionStatus.Cancel, true);
            }

            if (!MathKit.Vectors2DEquals(eventData.PropertyValue, Vector2.zero) &&
                !MathKit.Vectors2DEquals(eventData.PrevValue, eventData.PropertyValue))
            {
                iInMotion = true;
                IsMoving.DirtyValue();
                RotationAngle.Value = Mathf.Atan2(eventData.PropertyValue.y, eventData.PropertyValue.x) * Mathf.Rad2Deg - 90f;
                Event<Aggregator.Events.Behaviours.Movable.LinearMovingBehaviour.OnStartMotionEvent>(Container).Invoke(eventData.PropertyValue);
            }
        }

        protected virtual void DoEnd(Vector2 direction, Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour.EndMotionStatus status, bool fromViewer=false)
        {
            iInMotion = false;
            IsMoving.DirtyValue();

            if (!fromViewer)
                MovingDirection.Value = Vector2.zero;

            Event<Aggregator.Events.Behaviours.Movable.LinearMovingBehaviour.OnEndMotionEvent>(Container).Invoke(direction, status);

            if (status == Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour.EndMotionStatus.Success)
                Event<Aggregator.Events.Behaviours.Movable.LinearMovingBehaviour.OnFinishMotionEvent>(Container).Invoke(direction);
        }

        protected void FixedUpdate()
        {
            if (!iInMotion)
                return;

            float speedDelta = SpeedProperty.Value * Time.fixedDeltaTime;

            if (!MathKit.NumbersEquals(speedDelta, 0f))
            {
                PositionProperty.Value += MovingDirection.Value * speedDelta;
            }
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            IsMoving.ReadonlyValueProvider = () => { return iInMotion; };

            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            if (iInMotion)
                DoEnd(MovingDirection.Value, Aggregator.Enum.Behaviours.Movable.LinearMovingBehaviour.EndMotionStatus.Cancel);

            IsMoving.ReadonlyValueProvider = null;

            return true;
        }
    }
}