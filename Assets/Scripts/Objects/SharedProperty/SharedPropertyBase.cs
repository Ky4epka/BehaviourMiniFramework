using UnityEngine;
using Main.Events;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Main.Objects
{
    /// <summary>
    /// This class provides only the full implementation of sharedProperty, but not could using in direct. 
    /// Shared property type must be unique, because his type used for self-identification in container
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="commandType"></typeparam>
    /// <typeparam name="eventType"></typeparam>
    [Serializable]
	public abstract class SharedPropertyBase<T, eventType> : ISharedProperty, ISharedPropertyReference<T>
		where eventType : IEventData, ISharedPropertyCompatibleEvent<T>
	{
		public Guid uid = Guid.NewGuid();
		public abstract string GroupTag { get; }
		public abstract string SharedName { get; }

		public virtual T InitialValue => StorageValue;

		public virtual T Value
		{
			get => ((ReadonlyValueProvider != null) && !iSuppressEventsAndHandlers) ? 
					ReadonlyValueProvider() : 
					StorageValue;
			set
			{
				if (IsStorageMode)
                {
					StorageValue = value;
					StoragePrevValue = value;
					return;
                }

				if (IsValidValue(value))
					SetValue(value);
			}
		}



		/// <summary>
		/// If value has been initialized
		/// </summary>
		public virtual bool IsValueInitialized
		{
			get
			{
				return iValueInitialized || (ReadonlyValueProvider != null);
			}
		}

		public virtual IBehaviourContainer Container { get; set; }

		public virtual bool EmitEvents
		{
			get => iEmitEvents;
			set
			{
				if (IsDisposed)
					throw new ObjectDisposedException(this.GetType().Name);

				if (iEmitEvents == value)
					return;

				iEmitEvents = value;
			}
		}
		public virtual bool IsDisposed => iIsDisposed;
		object ISharedPropertyStorage.Value
		{
			get => (T)Value;
			// Condition with value != null to allow to avoid @NullReferenceException when value a non reference type takes abstract value (object) takes a "null" 
			set => Value = (value != null) ? (T)value : default(T);
		}

		protected abstract T StorageValue { get; set; }
		protected abstract T StoragePrevValue { get; set; }

		//[SerializeReference]
		//[SerializeField]
		//protected abstract T iValue;
		/// <summary>
		/// This field needs for a csorrect processing old value in cases of direct field 'iValue' changing. (Example: container property editor)
		/// </summary>
		//[SerializeField]
		//protected T iPrevValue = default;
		[SerializeField]
		protected bool iEmitEvents = false;
		[NonSerialized]
		protected bool iIsChanged = false;
		[NonSerialized]
		protected bool iValueInitialized = false;
		[NonSerialized]
		protected bool iIsDisposed = false;
		[NonSerialized]
		protected bool iSuppressEventsAndHandlers = false;
		[NonSerialized]
		protected List<SharedPropertyHandler<ISharedProperty, T>> iHandlers = null;
		protected List<SharedPropertyHandler<ISharedProperty, T>> Handlers
		{
			get => (iHandlers == null) ?
					iHandlers = new List<SharedPropertyHandler<ISharedProperty, T>>() :
					iHandlers;
		}

		[SerializeField]
		public Type EventType { get; } = typeof(eventType);
		[SerializeField]
		public Type ValueType { get; } = typeof(T);

		public static Type sHandlerDelegateType { get; } = typeof(SharedPropertyHandler<ISharedProperty, T>);

		[NonSerialized]
		protected Func<T> iValueProvider = null;
		[NonSerialized]
		protected bool iIsStorageMode = false;

		public event Action OnDispose = null;

		public virtual bool IsReadOnly => ReadonlyValueProvider != null;

		public virtual bool IsSerializable => true;
		Func<object> ISharedProperty.ReadonlyValueProvider { get; set; }
		public Func<T> ReadonlyValueProvider { get => iValueProvider; set => iValueProvider = value; }

		public virtual bool IsEditableInEditor => true;

		public virtual bool IsShowInEditor => true;

		public IEventProcessor Sender { get; set; }
		public IEventProcessor Receiver { get; set; }

		public Type HandlerDelegateType { get; } = sHandlerDelegateType;

		protected static bool iHasMathematicOpInitFlag = true;
		protected static bool iHasMathematicOp = false;


		public virtual bool IsCanHaveCompoundValue => true;

		public bool HasMathematicOp
		{
			get
			{
				if (iHasMathematicOpInitFlag)
				{
					iHasMathematicOp = typeof(ISharedPropertyStorageMathematic<T>).IsAssignableFrom(GetType());
					iHasMathematicOpInitFlag = false;
				}

				return iHasMathematicOp;
			}
		}

        public bool IsStorageMode 
		{
			get => iIsStorageMode && !IsReadOnly;
			set => iIsStorageMode = value; 
		}

        public override bool Equals(object obj)
		{
			if (obj is T)
				return this.Equals((T)obj);
			else if (obj is ISharedProperty)
				return this.Equals((ISharedProperty)obj);

			return false;
		}

		public virtual bool Equals(T value)
		{
			// In case of MainThread error of comparing a two UnityObjects, when both a null
			if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
			{
				return (value != null) && (Value != null) &&
					(value as UnityEngine.Object == Value as UnityEngine.Object);
			}

			return value?.Equals(Value) ?? value != null;
		}

		public bool Equals(ISharedProperty obj)
		{
			return this.Value.Equals(obj.Value);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public virtual void Dispose()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(this.GetType().Name);

			EmitEvents = false;
			iIsDisposed = true;
			Handlers.Clear();

			if (OnDispose != null)
				OnDispose();
		}

		/*
		public static bool operator == (SharedPropertyReference<T, eventType> obj1, SharedPropertyReference<T, eventType> obj2)
		{
			return Equals(obj1, obj2);
		}

		public static bool operator != (SharedPropertyReference<T, eventType> obj1, SharedPropertyReference<T, eventType> obj2)
		{
			return !Equals(obj1, obj2);
		}
		*/

		protected virtual bool SetValue(T value, bool checkValueChanges = true)
		{
			if (IsDisposed)
				throw new ObjectDisposedException(this.GetType().Name);

			if ((ReadonlyValueProvider != null) && !iSuppressEventsAndHandlers)
				value = ReadonlyValueProvider();
//			else if (!IsValueInitialized)
	//			value = InitialValue;

			T old = StoragePrevValue;

			if (!InvokeHandlers(old, ref value))
				return false;

			if (checkValueChanges && (old?.Equals(value) ?? value == null))
				return false;

			StorageValue = value;
			StoragePrevValue = value;
			ValueChanged(value, old);

			return true;
		}

		protected virtual void ValueChanged(T Value, T oldValue)
		{
			iIsChanged = true;
			iValueInitialized = true;
			if (EmitEvents && !iSuppressEventsAndHandlers)
			{
				if (Container == null)
					Debug.LogError($"Shared property '{GetType().FullName}' container is null;");

				ISharedPropertyCompatibleEvent<T> propEvent = Container.Event<eventType>(Container);
				propEvent.Invoke(this, Value, oldValue);
			}
		}

		public SharedPropertyBase()
		{

		}

		protected virtual bool IsValidValue(T value)
		{
			if (IsDisposed)
				throw new ObjectDisposedException(this.GetType().Name);

			if (Equals(value))
				return false;

			return true;
		}

		public void AddPropertyHandler(Delegate handler)
		{
			try
			{
				Handlers.Add((SharedPropertyHandler<ISharedProperty, T>)handler);
			}
			catch
			{
				GLog.LogException(new InvalidCastException(handler.GetType().FullName));
			}
		}

		public void RemovePropertyHandler(Delegate handler)
		{
			Handlers.Remove((SharedPropertyHandler<ISharedProperty, T>)handler);
		}

		public void ClearPropertyHandlers()
		{
			Handlers.Clear();
		}

		public void DirtyValue()
		{
			//if (IsReadOnly || (ValueProvider != null))
			//ValueChanged(Value, Value);
			//else
			SetValue(Value, false);
		}

		public virtual void RehandleValue()
		{
			SetValue(Value, true);
		}

		public virtual void SyncReadonlyValue()
        {
			if (IsReadOnly)
				SetValue(Value);
        }

		public void EventValueFor(Delegate listener)
		{
			Container.Event<eventType>(Container).InvokeFor(this, Value, Value, listener);
		}

		protected virtual bool InvokeHandlers(T oldValue, ref T newValue)
		{
			if (iSuppressEventsAndHandlers)
				return true;

			for (int i = 0; i < Handlers.Count; i++)
			{
				try
				{
					if (!Handlers[i](this, oldValue, ref newValue))
						return false;
				}
				catch (Exception e)
				{
					GLog.LogException(e);
				}
			}

			return true;
		}

		public bool Equals(ISharedPropertyReference<T> other)
		{
			return Value?.Equals(other.Value) ?? other.Value != null;
		}

		public void AddPropertyHandler(SharedPropertyHandler<ISharedProperty, T> handler)
		{
			Handlers.Add(handler);
		}

		public void RemovePropertyHandler(SharedPropertyHandler<ISharedProperty, T> handler)
		{
			Handlers.Remove(handler);
		}

        public object Clone()
        {
			return this.MemberwiseClone();
        }

        public void Assign(IAssignable source)
        {
			if (source == null)
				return;

			if (!GetType().IsAssignableFrom(source.GetType()))
				return;

			ISharedPropertyReference<T> sourceProp = source as ISharedPropertyReference<T>;

			Value = sourceProp.Value;
			EmitEvents = sourceProp.EmitEvents;
			ReadonlyValueProvider = sourceProp.ReadonlyValueProvider;
        }

        public void BeginDisableEventsAndHandlers()
        {
			iSuppressEventsAndHandlers = true;
		}

        public void EndDisableEventsAndHanlders()
        {
			iSuppressEventsAndHandlers = false;
		}

        public ISharedPropertyStorage ExtractStorage()
        {
			return new SharedPropertyStorage<T>(Value);
        }

        /*
        public void InvokeFor(Delegate listener)
        {
			EventDataBase.InvokeFor(this, listener);
        }

        public void Invoke()
        {
			EventDataBase.Invoke(this);
        }
		*/
    }
}