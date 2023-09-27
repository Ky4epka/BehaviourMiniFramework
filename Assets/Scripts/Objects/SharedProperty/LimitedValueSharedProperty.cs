using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using System;

namespace Main.Aggregator.Commands
{
}

namespace Main.Aggregator.Events
{

}

namespace Main.Objects
{

    public abstract class LimitedValueSharedProperty<
			ValueType, 
			ValueEventType,
			MinBorderEventType,
			MaxBorderEventType,
			MinValueProperty,
			MaxValueProperty> : 
		SharedProperty<ValueType, ValueEventType>
		where ValueType : IEquatable<ValueType>, IComparable<ValueType>
		where ValueEventType: ISharedPropertyCompatibleEvent<ValueType>
		where MinBorderEventType: ISharedPropertyCompatibleEvent<ValueType>
		where MaxBorderEventType: ISharedPropertyCompatibleEvent<ValueType>
		where MinValueProperty : ISharedProperty<ValueType>
		where MaxValueProperty : ISharedProperty<ValueType>
	{
		public MinValueProperty MinValue
		{
			get;
			protected set;
		}

		public MaxValueProperty MaxValue
		{
			get;
			protected set;
		}

		public bool IsMinimum => Equals(MinValue);

		public bool IsMaximum => Equals(MaxValue);

		protected void MinValueEvent(ISharedPropertyCompatibleEvent<ValueType> eventData)
        {
			SetValue(Value);
        }

		protected void MaxValueEvent(ISharedPropertyCompatibleEvent<ValueType> eventData)
		{
			SetValue(Value);
		}

		protected override bool SetValue(ValueType value, bool checkValueChanges = true)
        {
			ValueType boundedValue = value;
			int minCompare = 0;
			int maxCompare = 0;

			if ((MinValue != null) &&
				(MaxValue != null))
			{
				minCompare = value.CompareTo(MinValue.Value);
				maxCompare = value.CompareTo(MaxValue.Value);

				if (minCompare <= 0)
				{
					boundedValue = MinValue.Value;
				}

				if (maxCompare >= 0)
				{
					boundedValue = MaxValue.Value;
				}
			}

			ValueType old = iValue;

			if (base.SetValue(boundedValue, checkValueChanges))
            {
				if (EmitEvents)
				{
					Container?.Event<ValueEventType>(Container).Invoke(this, value, old);
				}

				if (minCompare <= 0)
					Container?.Event<MinBorderEventType>(Container).Invoke(this, value, old);

				if (maxCompare >= 0)
					Container?.Event<MaxBorderEventType>(Container).Invoke(this, value, old);

				return true;
            }

			return false;
		}

        public override IBehaviourContainer Container 
		{ 
			get => base.Container;
			set
			{
				if (base.Container != null)
				{
					value.RemoveEventListener(MinValue.EventType, (Action<ISharedPropertyCompatibleEvent<ValueType>>)MinValueEvent);
					value.RemoveEventListener(MaxValue.EventType, (Action<ISharedPropertyCompatibleEvent<ValueType>>)MaxValueEvent);
				}

				base.Container = value;

				if (value != null)
				{
					MinValue = value.SharedProperty<MinValueProperty>();
					MaxValue = value.SharedProperty<MaxValueProperty>();
					value.AddEventListener(MinValue.EventType, (Action<ISharedPropertyCompatibleEvent<ValueType>>)MinValueEvent);
					value.AddEventListener(MaxValue.EventType, (Action<ISharedPropertyCompatibleEvent<ValueType>>)MaxValueEvent);
				}
			}
		}

    }

}