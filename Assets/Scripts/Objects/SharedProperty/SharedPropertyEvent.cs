using Main.Events;
using System;

namespace Main.Objects
{
    public abstract class SharedPropertyEvent<ValueType> : EventDataBase, ISharedPropertyCompatibleEvent<ValueType>
	{
		public SharedPropertyEvent() : base()
        {
		}

		public ValueType PropertyValue { get; protected set; }
		public ValueType PrevValue { get; protected set; }
        public ISharedProperty Property { get; protected set; }

        public void Invoke(ISharedProperty property, ValueType newValue, ValueType prevValue)
		{
			Property = property;
			PropertyValue = newValue;
			PrevValue = prevValue;
			base.Invoke();
		}

        public void InvokeFor(ISharedProperty property, ValueType newValue, ValueType prevValue, Delegate eventListener)
		{
			Property = property;
			PropertyValue = newValue;
			PrevValue = prevValue;
			base.InvokeFor(eventListener);
		}
    }
}