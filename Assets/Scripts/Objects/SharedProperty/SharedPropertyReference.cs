using UnityEngine;
using Main.Events;

namespace Main.Objects
{
    public abstract class SharedPropertyReference<T, eventType> : SharedPropertyBase<T, eventType>
		where T: class
		where eventType : IEventData, ISharedPropertyCompatibleEvent<T>
	{
		[SerializeReference]
		protected T iValue = default;
		[SerializeReference]
		protected T iPrevValue = default;

		protected override T StorageValue { get => iValue; set => iValue = value; }
		protected override T StoragePrevValue { get => iValue; set => iValue = value; }
	}
}