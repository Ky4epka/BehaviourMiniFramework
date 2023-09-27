using Main.Events;
using Main.Objects;
using System;
using UnityEngine;

namespace Main.Objects
{
	/// <summary>
	/// Shared property addresses by finish type.
	/// Shared property type must be unique and sealed class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="CommandType"></typeparam>
	/// <typeparam name="EventType"></typeparam>
	[Serializable]
	public abstract class SharedProperty<T, EventType> : SharedPropertyBase<T, EventType>, ISharedProperty<T>
		where T : IEquatable<T>
		where EventType : IEventData, ISharedPropertyCompatibleEvent<T>
	{
		[SerializeField]
		protected T iValue = default;
		[SerializeField]
		protected T iPrevValue = default;

		protected override T StorageValue { get => iValue; set => iValue = value; }
		protected override T StoragePrevValue { get => iValue; set => iValue = value; }

		public override bool Equals(T value)
        {
            return Value?.Equals(value) ?? value == null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SharedProperty<T,  EventType> obj1, SharedProperty<T, EventType> obj2)
		{
			return obj1.Equals(obj2);
		}

		public static bool operator !=(SharedProperty<T, EventType> obj1, SharedProperty<T, EventType> obj2)
		{
			return !obj1.Equals(obj2);
		}

		protected override bool IsValidValue(T value)
        {
			if (IsDisposed)
				throw new ObjectDisposedException(this.GetType().Name);

			if (Equals(value))
				return false;

			return true;
		}

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

}