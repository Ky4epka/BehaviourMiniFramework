using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;

namespace Main.Aggregator.Enum.Behaviours.Rotation
{
    public enum RotationEndStatus
    {
        Success = 0,
        Cancel,
        Unreached
    }
}


namespace Main.Aggregator.Events.Behaviours.Rotation
{
    public class TargetAngleProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Rotation
{
    public class TargetAngleProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Rotation.TargetAngleProperty>
    {
        public override string GroupTag => "Rotation";
        public override string SharedName => "TargetAngle";
    }
}

namespace Main.Aggregator.Events.Behaviours.Rotation
{
    public class RotationSectorProperty : SharedPropertyEvent<Vector2>
    {
    }

    public class OnStartRotation: EventDataBase
    {
        public float RotationAngle { get; protected set; }
        public float CurrentAngle { get; protected set; }

        public void Invoke(float rotationAngle, float currentAngle)
        {
            RotationAngle = rotationAngle;
            CurrentAngle = currentAngle;
            base.Invoke();
        }
    }

    public class OnEndRotation : EventDataBase
    {
        public float RotationAngle { get; protected set; }
        public Enum.Behaviours.Rotation.RotationEndStatus EndStatus { get; protected set; }

        public void Invoke(float rotationAngle, Enum.Behaviours.Rotation.RotationEndStatus endStatus)
        {
            RotationAngle = rotationAngle;
            EndStatus = endStatus;
            base.Invoke();
        }
    }

    public class OnRotationSectorLeft: EventDataBase
    {
    }

    public class OnRotationSectorRight : EventDataBase
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Rotation
{
    public class RotationSectorProperty : SharedProperty<Vector2, Main.Aggregator.Events.Behaviours.Rotation.RotationSectorProperty>
    {
        public override string GroupTag => "Rotation";
        public override string SharedName => "RotationSector";
    }
}


namespace Main.Aggregator.Events.Behaviours.Rotation
{
    public class RotationSpeedProperty : SharedPropertyEvent<float>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Rotation
{
    public class RotationSpeedProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Rotation.RotationSpeedProperty>
    {
        public override string GroupTag => "Rotation";
        public override string SharedName => "RotationSpeed";
    }
}

namespace Main.Aggregator.Events.Behaviours.Rotation
{
    public class RotationAngleProperty : SharedPropertyEvent<float>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Rotation
{
    public class RotationAngleProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Rotation.RotationAngleProperty>
    {
        public override string GroupTag => "Rotation";
        public override string SharedName => "RotationAngle";
    }
}


namespace Main.Aggregator.Events.Behaviours.Rotation
{
    public class IsEqualsToTargetAngleProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Rotation
{
    public class IsEqualsToTargetAngleProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Rotation.IsEqualsToTargetAngleProperty>
    {
        public override string GroupTag => "Rotation";
        public override string SharedName => "IsEqualsToTargetAngle";
    }
}


namespace Main.Aggregator.Events.Behaviours.Rotation
{
    public class IsRotatingProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Rotation
{
    public class IsRotatingProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Rotation.IsRotatingProperty>
    {
        public override string GroupTag => "Rotation";
        public override string SharedName => "IsRotating";
    }
}

namespace Main.Objects.Behaviours.Rotation
{
    [Unique]
    public class RotationBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.RotationAngleProperty RotationAngle { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.RotationSectorProperty RotationSector { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.RotationSpeedProperty RotationSpeed { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.TargetAngleProperty TargetAngle { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.IsEqualsToTargetAngleProperty IsEqualsToTargetAngle { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.IsRotatingProperty IsRotating { get; protected set; }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Rotation.TargetAngleProperty))]
        public void TargetAnglePropertyViewer(Main.Aggregator.Events.Behaviours.Rotation.TargetAngleProperty eventData)
        {
            if (Mathf.Approximately(eventData.PropertyValue, RotationAngle.Value))
                return;

            if (Mathf.Approximately(eventData.PropertyValue, eventData.PrevValue))
                return;

            if (iIsRotating)
                DoEnd(eventData.PropertyValue, Aggregator.Enum.Behaviours.Rotation.RotationEndStatus.Cancel);

            iIsRotating = true;
            Event<Aggregator.Events.Behaviours.Rotation.OnStartRotation>(Container).Invoke(eventData.PropertyValue, RotationAngle.Value);
        }


        protected bool iIsRotating = false;
        protected float iTargetAngle = 0f;

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Rotation.RotationSectorProperty))]
        public void RotationSectorPropertyViewer(Main.Aggregator.Events.Behaviours.Rotation.RotationSectorProperty eventData)
        {
            RotationAngle.DirtyValue();
        }

        [SharedPropertyHandler(typeof(Main.Aggregator.Properties.Behaviours.Rotation.RotationSectorProperty))]
        public bool RotationSectorPropertyHandler(ISharedProperty property, Vector2 oldValue, ref Vector2 newValue)
        {
            if (newValue.x > newValue.y)
            {
                float temp = newValue.x;
                newValue.x = newValue.y;
                newValue.y = temp;
            }

            return true;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Rotation.RotationAngleProperty))]
        public void RotationgAnglePropertyViewer(Main.Aggregator.Events.Behaviours.Rotation.RotationAngleProperty eventData)
        {
            if (RotationSector.Value.sqrMagnitude > 0f)
            {
                if (Mathf.Approximately(eventData.PropertyValue, RotationSector.Value.x))
                {
                    Event<Aggregator.Events.Behaviours.Rotation.OnRotationSectorLeft>(Container).Invoke();
                }
                else if(Mathf.Approximately(eventData.PropertyValue, RotationSector.Value.y))
                {
                    Event<Aggregator.Events.Behaviours.Rotation.OnRotationSectorRight>(Container).Invoke();
                }
            }

//            IsEqualsToTargetAngle.Value = ;

            transform.localRotation = Quaternion.Euler(0, 0, eventData.PropertyValue);
        }

        [SharedPropertyHandler(typeof(Main.Aggregator.Properties.Behaviours.Rotation.RotationAngleProperty))]
        public bool RotatingAnglePropertyHandler(ISharedProperty property, float oldValue, ref float newValue)
        {
            if (!Mathf.Approximately(RotationSector.Value.sqrMagnitude, float.Epsilon))
                newValue = Mathf.Clamp(newValue, RotationSector.Value.x, RotationSector.Value.y);

            return true;
        }

        protected void DoEnd(float rotationAngle, Aggregator.Enum.Behaviours.Rotation.RotationEndStatus endStatus)
        {
            iIsRotating = false;
            Event<Aggregator.Events.Behaviours.Rotation.OnEndRotation>(Container).Invoke(rotationAngle, endStatus);
        }

        protected bool IsValidAngle(float angle)
        {
            return 
                Mathf.Approximately(RotationSector.Value.sqrMagnitude, 0f) ||
                ((angle >= RotationSector.Value.x) &&
                 (angle <= RotationSector.Value.y));
        }

        protected bool ValidateAngle(ref float angle)
        {
            if (IsValidAngle(angle))
                return true;
            else
            {
                angle = Mathf.Clamp(angle, RotationSector.Value.x, RotationSector.Value.y);
                return false;
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!iIsRotating)
                return;

            float angleSpeed = RotationSpeed.Value * Time.fixedDeltaTime;
            float deltaAngle = Mathf.DeltaAngle(RotationAngle.Value, TargetAngle.Value);

            if (Mathf.Abs(deltaAngle) <= angleSpeed * 2f)
            {
                RotationAngle.Value = TargetAngle.Value;
                DoEnd(RotationAngle.Value, Aggregator.Enum.Behaviours.Rotation.RotationEndStatus.Success);
            }
            else if (Mathf.Approximately(RotationSpeed.Value, 0))
            {
                RotationAngle.Value = TargetAngle.Value;
                DoEnd(RotationAngle.Value, Aggregator.Enum.Behaviours.Rotation.RotationEndStatus.Success);
            }
            else
            {
                float newAngle = RotationAngle.Value + angleSpeed * Mathf.Sign(deltaAngle);
                RotationAngle.Value = newAngle;

                if (!IsValidAngle(newAngle))
                    DoEnd(RotationAngle.Value, Aggregator.Enum.Behaviours.Rotation.RotationEndStatus.Unreached);
            }
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            IsRotating.ReadonlyValueProvider = () => { return iIsRotating; };
            IsEqualsToTargetAngle.ReadonlyValueProvider = () => { return Mathf.Abs(Mathf.DeltaAngle(RotationAngle.Value, TargetAngle.Value)) <= Configuration.ROTATION_EQUAL_ANGLE_THRESHOLD; };
            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoEnable())
                return false;

            IsRotating.ReadonlyValueProvider = null;
            IsEqualsToTargetAngle.ReadonlyValueProvider = null;
            return true;
        }

    }
}