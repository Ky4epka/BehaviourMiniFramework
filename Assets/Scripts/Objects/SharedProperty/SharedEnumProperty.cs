using Main.Events;
using System;
using UnityEngine;

namespace Main.Objects
{
    public abstract class SharedEnumProperty<T, EventType> : SharedPropertyBase<T, EventType>
        where T : Enum
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
			return System.Collections.Generic.EqualityComparer<T>.Default.Equals(Value, value);
        }

        public override int GetHashCode()
        {
            return	GetType().GetHashCode() ^ 
					System.Collections.Generic.EqualityComparer<T>.Default.GetHashCode(Value);
        }

    }
}