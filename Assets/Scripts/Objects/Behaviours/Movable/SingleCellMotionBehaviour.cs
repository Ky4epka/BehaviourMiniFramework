using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;

namespace Main.Aggregator.Enum.Behaviours.Movable.DirectionStepToCell
{
    [System.Serializable]
    public enum EndMotionStatus
    {
        Success = 0,
        Cancel,
        Unreached
    }
}



namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class RotateBodyOnMoveProperty : SharedPropertyEvent<bool>
    {
    }
    public class DirectionStepToCellProperty : SharedPropertyEvent<Vector2Int>
    {
    }
    public class DoCancelMotion : EventDataBase
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class RotateBodyOnMoveProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Movable.RotateBodyOnMoveProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "RotateBodyOnMove";
    }
}

namespace Main.Aggregator.Events.Behaviours.Movable.SingleCellMotionBehaviour
{

    public class OnStartMotionToCellPoint : EventDataBase
    {
        public Vector2Int StepDirection { get; private set; }
        public Vector2Int TargetMapIndexes { get; private set; }
        public Vector2 TargetLocalPoint { get; private set; }


        public void Invoke(Vector2Int stepDirection, Vector2Int targetMapIndexes, Vector2 targetLocalPoint)
        {
            StepDirection = stepDirection;
            TargetMapIndexes = targetMapIndexes;
            TargetLocalPoint = targetLocalPoint;
            base.Invoke();
        }
    }

    public class OnEndMotionToCellPoint : EventDataBase
    {
        public Vector2Int StepDirection { get; private set; }
        public Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus EndStatus { get; private set; }

        public void Invoke(Vector2Int stepDirection, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus endStatus)
        {
            StepDirection = stepDirection;
            EndStatus = endStatus;
            base.Invoke();
        }
    }

    public class OnFinishMotionToCellPoint : EventDataBase
    {
        public Vector2Int StepDirection { get; private set; }

        public void Invoke(Vector2Int stepDirection)
        {
            StepDirection = stepDirection;
            base.Invoke();
        }
    }

}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class DirectionStepToCellProperty : SharedProperty<Vector2Int, Main.Aggregator.Events.Behaviours.Movable.DirectionStepToCellProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "TargetCellMotion";
    }
}


namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class IsMovingProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class IsMovingProperty : SharedProperty<bool, Main.Aggregator.Events.Behaviours.Movable.IsMovingProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "IsMoving";
    }
}


namespace Main.Objects.Behaviours.Movable
{
    
    [Unique]
    [RequireComponent(typeof(DynamicMotionBehaviour), typeof(Rotation.RotationBehaviour))]
    [IncompatibleBehaviours(typeof(IMotionBehaviourController), includeInheritedBehaviours = true)]
    public class SingleCellMotionBehaviour : ObjectBehavioursBase, IMotionBehaviourController
    {
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.DirectionStepToCellProperty StepDirectionProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty MapProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.PositionProperty PositionProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.SpeedProperty SpeedProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.MotionTypeProperty MotionType { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.RotateBodyOnMoveProperty RotateBodyOnMoveProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.RotationAngleProperty RotationAngle { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.IsEqualsToTargetAngleProperty IsEqualsToTargetAngle { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.TargetAngleProperty TargetAngle { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Rotation.IsRotatingProperty IsRotating { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.IsMovingProperty IsMoving { get; protected set; }

        protected Vector2Int iTargetMapIndexes = new Vector2Int(-1, -1);
        protected Vector2 iTargetLocalPointMap = Vector2.zero;
        protected Vector2 iPrevPosition = Vector2.zero;
        protected int iCellCheckFrameCounter = 0;
        protected float iIdleElapsedTime = 0;
             
        protected bool iInMotion = false;

        [EnabledStateEvent]
        public void OnRotationEnd(Aggregator.Events.Behaviours.Rotation.OnEndRotation eventData)
        {
            if (!iInMotion)
                return;

            if (!eventData.EndStatus.HasFlag(Aggregator.Enum.Behaviours.Rotation.RotationEndStatus.Success))
                DoEnd(StepDirectionProperty.Value, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Unreached);
        }

        [EnabledStateEvent]
        public void OnDoCancelMotion(Aggregator.Events.Behaviours.Movable.DoCancelMotion eventData)
        {
            if (!iInMotion)
                return;

            DoEnd(StepDirectionProperty.Value, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Cancel);
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Common.MapManagedBehaviour.Map.MapProperty))]
        public virtual void MapPropertyViewer(Main.Aggregator.Events.Behaviours.Common.MapManagedBehaviour.Map.MapProperty eventData)
        {
            if (!MapProperty.Value && enabled)
                enabled = false;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.DirectionStepToCellProperty))]
        public void StepDirectionPropertyPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.DirectionStepToCellProperty eventData)
        {
            if (MapProperty.Value == null)
            {
                return;
            }

            if (((eventData.PrevValue != Vector2Int.zero) && eventData.PrevValue != eventData.PropertyValue) && iInMotion)
            {
                DoEnd(eventData.PrevValue, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Cancel);
            }

            if ((eventData.PropertyValue != Vector2.zero) && (eventData.PropertyValue != eventData.PrevValue))
            {
                iInMotion = true;
                iIdleElapsedTime = 0f;
                iCellCheckFrameCounter = 0;
                iPrevPosition = PositionProperty.Value;
                Vector2Int normalizedDirection = new Vector2Int(System.Math.Sign(eventData.PropertyValue.x), System.Math.Sign(eventData.PropertyValue.y));
                iTargetMapIndexes = MapProperty.Value.Common.LocalToMapIndexes(PositionProperty.Value) + normalizedDirection;
                iTargetLocalPointMap = MapProperty.Value.Common.MapIndexesToLocalCellCenter(iTargetMapIndexes);

                Event<Aggregator.Events.Behaviours.Movable.SingleCellMotionBehaviour.OnStartMotionToCellPoint>(Container).Invoke(eventData.PropertyValue, iTargetMapIndexes, iTargetLocalPointMap);
            }
        }

        protected virtual void DoEnd(Vector2Int direction, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus status)
        {
            iInMotion = false;
            StepDirectionProperty.Value = Vector2Int.zero;
            Event<Aggregator.Events.Behaviours.Movable.SingleCellMotionBehaviour.OnEndMotionToCellPoint>(Container).Invoke(direction, status);

            if (status == Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Success)
                Event<Aggregator.Events.Behaviours.Movable.SingleCellMotionBehaviour.OnFinishMotionToCellPoint>(Container).Invoke(direction);

        }

        protected virtual void FixedUpdate()
        {
            if (MapProperty.Value == null)
                return;


            if (!iInMotion)
                return;

            float speedDelta = SpeedProperty.Value * Time.fixedDeltaTime;

            if (Vector2.Distance(PositionProperty.Value, iTargetLocalPointMap) <= speedDelta * 2f)
            {
                PositionProperty.Value = iTargetLocalPointMap;
                DoEnd(StepDirectionProperty.Value, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Success);
            }
            else
            {
                Vector2 newPosition = Vector2.MoveTowards(PositionProperty.Value, iTargetLocalPointMap, speedDelta);
                MapCell cell = MapProperty.Value.Common.GetCellByLocal(iTargetMapIndexes);
                bool movable = iCellCheckFrameCounter != Configuration.MOTION_CELL_CHECK_FRAME_SKIP;

                if ((false) && (cell != null) && ((iCellCheckFrameCounter == 0)))
                {
                    DynamicMotionBehaviour motBeh = Container.GetComponent<DynamicMotionBehaviour>();
                    movable = motBeh.MapCellIsPassable(iTargetMapIndexes);
                }

                iCellCheckFrameCounter = iCellCheckFrameCounter++ % Configuration.MOTION_CELL_CHECK_FRAME_SKIP;

                if (movable)
                {
                    float calcAngle = Vector2.SignedAngle(Vector2.right, iTargetLocalPointMap - newPosition);

                    if (IsRotating.Value)
                    { }
                    else if (RotateBodyOnMoveProperty.Value && (Mathf.Abs(Mathf.DeltaAngle(calcAngle, RotationAngle.Value)) > Configuration.ROTATION_EQUAL_ANGLE_THRESHOLD))
                        TargetAngle.Value = calcAngle;
                    else if (Mathf.Approximately(SpeedProperty.Value, float.Epsilon))
                    {
                        PositionProperty.Value = iTargetLocalPointMap;
                        DoEnd(StepDirectionProperty.Value, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Success);
                    }
                    else
                        PositionProperty.Value = newPosition;
                }
                else
                    DoEnd(StepDirectionProperty.Value, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Unreached);

            }

            if ((Vector2.Distance(PositionProperty.Value, iPrevPosition) <= speedDelta * 2f) && 
                (!RotateBodyOnMoveProperty.Value || !IsRotating.Value))
            {
                iIdleElapsedTime += Time.fixedDeltaTime;

                if (iIdleElapsedTime >= Configuration.MOTION_IDLE_TIME)
                    DoEnd(StepDirectionProperty.Value, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Unreached);
            }
            else
                iIdleElapsedTime = 0;
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            if (MapProperty.Value != null)
            {
                IsMoving.ReadonlyValueProvider = () => { return iInMotion; };
                return true;
            }

            return false;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            if (iInMotion)
                DoEnd(StepDirectionProperty.Value, Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Cancel);

            IsMoving.ReadonlyValueProvider = null;

            return true;
        }

    }

}