using Main.Events;
using System;

namespace Main.Objects
{
    public abstract class MutatedSharedProperty<T, EventType, SourceType, SourceProperty> : SharedProperty<T, EventType>
        where T : IEquatable<T>
        where EventType : IEventData, ISharedPropertyCompatibleEvent<T>
		where SourceType : IEquatable<SourceType>
		where SourceProperty: class, ISharedProperty<SourceType>
	{
		protected SourceProperty iSourceProperty = default(SourceProperty);

		public SourceProperty Source
        {
			get => iSourceProperty;
			protected set
            {
				if (iSourceProperty == value)
					return;

				iSourceProperty = value;
            }
        }

        public override T Value 
		{ 
			get => Unbox(iSourceProperty.Value); 
			set => base.Value = value; 
		}	

        protected override bool SetValue(T value, bool checkValueChanges = true)
        {
			iSourceProperty.Value = Box(value);
			return true;
        }

        public override IBehaviourContainer Container
        {
			get => base.Container;
			set
            {
				base.Container = value;

				Source = (SourceProperty)Container?.SharedProperty(typeof(SourceProperty));
			}
        }
		/// <summary>
		/// Conversion from self type to source type
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public abstract SourceType Box(T value);
		/// <summary>
		/// Reverse conversion from source type to self type
		/// </summary>
		/// <param name="sourceValue"></param>
		/// <returns></returns>
		public abstract T Unbox(SourceType sourceValue);

    }
}