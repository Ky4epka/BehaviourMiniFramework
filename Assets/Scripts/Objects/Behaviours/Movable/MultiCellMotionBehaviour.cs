using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;
using System.Collections.Generic;


namespace Main.Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour
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
    public class CellsRangeProperty : SharedPropertyEvent<int>
    {
    }

    public class CellsRangeMinValueProperty : SharedPropertyEvent<int>
    {
    }

    public class CellsRangeMaxValueProperty : SharedPropertyEvent<int>
    {
    }

    public class CellsRangeMinBorderEvent : SharedPropertyEvent<int>
    {
    }

    public class CellsRangeMaxBorderEvent : SharedPropertyEvent<int>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class CellsRangeMinValueProperty : SharedProperty<int, Main.Aggregator.Events.Behaviours.Movable.CellsRangeMinValueProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "CellsRangeMin";
    }

    public class CellsRangeMaxValueProperty : SharedProperty<int, Main.Aggregator.Events.Behaviours.Movable.CellsRangeMaxValueProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "CellsRangeMax";
    }

    public class CellsRangeProperty : LimitedValueSharedProperty<
        int,
        Main.Aggregator.Events.Behaviours.Movable.CellsRangeProperty,
        Main.Aggregator.Events.Behaviours.Movable.CellsRangeMinBorderEvent,
        Main.Aggregator.Events.Behaviours.Movable.CellsRangeMaxBorderEvent,
        Main.Aggregator.Properties.Behaviours.Movable.CellsRangeMinValueProperty,
        Main.Aggregator.Properties.Behaviours.Movable.CellsRangeMaxValueProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "CellsRange";
    }
}


namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class StepsRangeProperty : SharedPropertyEvent<int>
    {
    }

    public class StepsRangeMinValueProperty : SharedPropertyEvent<int>
    {
    }

    public class StepsRangeMaxValueProperty : SharedPropertyEvent<int>
    {
    }

    public class StepsRangeMinBorderEvent : SharedPropertyEvent<int>
    {
    }

    public class StepsRangeMaxBorderEvent : SharedPropertyEvent<int>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class StepsRangeMinValueProperty : SharedProperty<int, Main.Aggregator.Events.Behaviours.Movable.StepsRangeMinValueProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "StepsRangeMin";
    }

    public class StepsRangeMaxValueProperty : SharedProperty<int, Main.Aggregator.Events.Behaviours.Movable.StepsRangeMaxValueProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "StepsRangeMax";
    }

    public class StepsRangeProperty : LimitedValueSharedProperty<
        int,
        Main.Aggregator.Events.Behaviours.Movable.StepsRangeProperty,
        Main.Aggregator.Events.Behaviours.Movable.StepsRangeMinBorderEvent,
        Main.Aggregator.Events.Behaviours.Movable.StepsRangeMaxBorderEvent,
        Main.Aggregator.Properties.Behaviours.Movable.StepsRangeMinValueProperty,
        Main.Aggregator.Properties.Behaviours.Movable.StepsRangeMaxValueProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "StepsRange";
    }
}

namespace Main.Aggregator.Events.Behaviours.Movable.MultiCellMotionBehaviour
{

    public class OnStartMotionToCellPoint : EventDataBase
    {
    }

    public class OnEndMotionToCellPoint : EventDataBase
    {
        public Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour.EndMotionStatus EndStatus { get; private set; }

        public void Invoke(Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour.EndMotionStatus endStatus)
        {
            EndStatus = endStatus;
            base.Invoke();
        }
    }

    public class OnFinishMotionToCellPoint : EventDataBase
    {
    }
}


namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class TargetCellsStepsQueryProperty : SharedPropertyEvent<Vector2Int[]>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class TargetCellsStepsQueryProperty : SharedPropertyReference<Vector2Int[], Main.Aggregator.Events.Behaviours.Movable.TargetCellsStepsQueryProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "TargetCellsQuery";

        public override bool Equals(Vector2Int[] value)
        {
            if ((iValue == null) || (value == null))
                return false;

            if (iValue.Length != value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
                if (iValue[i] != value[i])
                    return false;

            return true;
        }
    }
}

namespace Main.Objects.Behaviours.Movable
{

    public class MultiCellMotionBehaviour : SingleCellMotionBehaviour
    {

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.TargetCellsStepsQueryProperty TargetCellsSteps { get; protected set; }

        [SharedProperty]
        public Aggregator.Properties.Behaviours.Movable.CellsRangeMinValueProperty CellsRangeMinValue { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Movable.CellsRangeMaxValueProperty CellsRangeMaxValue { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Movable.StepsRangeMinValueProperty StepsRangeMin { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Movable.StepsRangeMaxValueProperty StepsRangeMax { get; protected set; }

        [EnabledStateEvent]
        public void OnCellStepEnd(Main.Aggregator.Events.Behaviours.Movable.SingleCellMotionBehaviour.OnEndMotionToCellPoint eventData)
        {
            switch (eventData.EndStatus)
            {
                case Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Success:
                    if (!DoNextStep())
                        DoEnd(Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour.EndMotionStatus.Success);

                    break;
                case Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Unreached:
                    DoEnd(Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour.EndMotionStatus.Unreached);
                    break;
                case Aggregator.Enum.Behaviours.Movable.DirectionStepToCell.EndMotionStatus.Cancel:

                    if (eventData.StepDirection != Vector2Int.zero)
                        DoEnd(Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour.EndMotionStatus.Cancel);

                    break;
            }
        }

        protected System.Collections.IEnumerator iStepsEnum;

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.TargetCellsStepsQueryProperty))]
        public void TargetCellsPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.TargetCellsStepsQueryProperty eventData)
        {
            if ((eventData.PropertyValue == null) ||
                (eventData.PropertyValue.Length == 0))
                return;

            Vector2Int prevStep = Vector2Int.zero;

            if (!IsMoving.Value)
            {
                iStepsEnum = TargetCellsSteps.Value.GetEnumerator();
                DoNextStep();
            }
        }

        public bool TargetCellsStepsHandler(ISharedProperty property, Vector2Int[] oldValue, ref Vector2Int[] newValue)
        {
            if (newValue == null)
            {
                newValue = new Vector2Int[0];
                return true;
            }

            int stepsMin = Mathf.Clamp(StepsRangeMin.Value, 0, newValue.Length - 1);
            int stepsMax = Mathf.Clamp(StepsRangeMax.Value, 0, newValue.Length - 1);

            int radiusMin = CellsRangeMinValue.Value;
            int radiusMax = CellsRangeMaxValue.Value;

            if (stepsMax == 0)
                stepsMax = newValue.Length - 1;
            else if (stepsMax < stepsMin)
                stepsMax = stepsMin;

            if (radiusMax == 0)
                radiusMax = MapProperty.Value.Common.Size.Value.sqrMagnitude;

            Vector2 thisPos = PositionProperty.Value;
            Vector2Int thisMapIndexes = MapProperty.Value.Common.LocalToMapIndexes(thisPos);
            float calculatedMagnitude;
            float minRadius = MapProperty.Value.Common.CellWorldSize.Value.sqrMagnitude * (float)radiusMin;
            float maxRadius = MapProperty.Value.Common.CellWorldSize.Value.sqrMagnitude * (float)radiusMax;

            for (int i = stepsMin; i <= stepsMax; i++)
            {
                calculatedMagnitude = (thisMapIndexes - newValue[i]).sqrMagnitude;

                if (calculatedMagnitude < minRadius)
                {
                    stepsMin++;
                }
                else if (calculatedMagnitude > maxRadius)
                    stepsMax--;
                else if ((calculatedMagnitude >= minRadius) && (calculatedMagnitude <= maxRadius))
                    break;
            }

            Vector2Int[] result = new Vector2Int[stepsMax-stepsMin+1];

            for (int i=0; i<=stepsMax; i++)
            {
                result[i] = newValue[i+stepsMin];
            }

            return true;
        }

        protected bool DoNextStep()
        {
            try
            {
                if (iStepsEnum?.MoveNext() ?? false)
                {
                    StepDirectionProperty.Value = (Vector2Int)iStepsEnum.Current;
                }
                else
                    return false;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            return true;
        }

        protected void ClearQueue()
        {
            TargetCellsSteps.Value = new Vector2Int[0];
        }

        protected virtual void DoEnd(Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour.EndMotionStatus status)
        {
            ClearQueue();
            Event<Aggregator.Events.Behaviours.Movable.MultiCellMotionBehaviour.OnEndMotionToCellPoint>(Container).Invoke();

            if (status.HasFlag(Aggregator.Enum.Behaviours.Movable.MultiCellMotionBehaviour.EndMotionStatus.Success))
                Event<Aggregator.Events.Behaviours.Movable.MultiCellMotionBehaviour.OnFinishMotionToCellPoint>(Container).Invoke();
        }

        
    }

}