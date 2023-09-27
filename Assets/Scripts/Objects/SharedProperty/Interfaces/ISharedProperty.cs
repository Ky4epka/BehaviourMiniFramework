using System;
using Main.Events;
using Main.Objects;

namespace Main.Objects
{
    public delegate bool SharedPropertyHandler<in SharedProperty, ValueType>
        (SharedProperty sender, ValueType oldValue, ref ValueType newValue) where SharedProperty : ISharedProperty;


    public interface IPropertyHandlers
    {
        void AddPropertyHandler(Delegate handler);
        void RemovePropertyHandler(Delegate handler);
        void ClearPropertyHandlers();
    }

    public interface IPropertyHandlers<SharedProperty, ValueType>
        where SharedProperty : ISharedProperty
    {
        void AddPropertyHandler(SharedPropertyHandler<SharedProperty, ValueType> handler);
        void RemovePropertyHandler(SharedPropertyHandler<SharedProperty, ValueType> handler);

        void ClearPropertyHandlers();
    }

    public interface ISharedPropertyStorage: ICloneable, System.Runtime.InteropServices.IAssignable
    {
        string SharedName { get; }
        string GroupTag { get; }

        object Value { get; set; }
        Type ValueType { get; }

        bool HasMathematicOp { get; }
        bool IsStorageMode { get; set; }
    }

    public interface ISharedPropertyStorage<T> : ISharedPropertyStorage
    {
        new T Value { get; set; }
    }

    public interface ISharedPropertyStorageAddictionOp<T>
    {
        T Addiction(T value);
    }

    public interface ISharedPropertyStorageSubtractionOp<T>
    {
        T Subtraction(T value);
    }

    public interface ISharedPropertyStorageMultiplicationOp<T>
    {
        T Multiplication(T value);
    }

    public interface ISharedPropertyStorageDivisionOp<T>
    {
        T Division(T value);
    }

    public interface ISharedPropertyStorageMathematic<T>: 
        ISharedPropertyStorageAddictionOp<T>, 
        ISharedPropertyStorageSubtractionOp<T>, 
        ISharedPropertyStorageMultiplicationOp<T>,
        ISharedPropertyStorageDivisionOp<T>
    {
    }

    public interface ISharedPropertyCompounded
    {
        bool IsCanHaveCompoundValue { get; }
    }

    [RoutableType]
    public interface ISharedProperty : ISharedPropertyStorage, ISharedPropertyCompounded, IDisposable, IPropertyHandlers
    {
        event Action OnDispose;
        bool IsValueInitialized { get; }
        bool EmitEvents { get; set; }
        bool IsDisposed { get; }

        bool IsReadOnly { get; }
        bool IsSerializable { get;  }
        bool IsEditableInEditor { get; }
        bool IsShowInEditor { get; }

        IBehaviourContainer Container { get; set; }

        Type EventType { get; }
        Type HandlerDelegateType { get; }

        /// <summary>
        /// Crutch for deserialization process when a only needs sets a value. Do not use this in any cases! Otherwise wherefore all is it?
        /// </summary>
        void BeginDisableEventsAndHandlers();
        void EndDisableEventsAndHanlders();

        Func<object> ReadonlyValueProvider { get; set; }

        /// <summary>
        /// Re-handle the value and re-inform listeners (usually used by behaviours for control self-properties)
        /// </summary>
        /// <param name="force">Ignore result of handlers</param>
        void DirtyValue();
        /// <summary>
        /// Re-handle value but if no changes after handling a value no inform listeners (example: used by handler attribute on enable behaviour)
        /// </summary>
        void RehandleValue();
        /// <summary>
        /// Update storage value by same value. Have meaning on readonly values in case of different result of getter and storage value
        /// </summary>
        void SyncReadonlyValue();

        /// <summary>
        /// Inform specific listener for a value (example: used by viewer attribute on enable behaviour)
        /// </summary>
        void EventValueFor(Delegate listener);

        /// <summary>
        /// Create instance of SharedPropertyStorage for this property and fill him data
        /// 
        /// Note: Unique for this type not guaranted. Dont use type of this instance for routing!
        /// </summary>
        /// <returns>Storage instance</returns>
        ISharedPropertyStorage ExtractStorage();
    }

    public interface ISharedPropertyReference<T>:
        ISharedProperty,
        ISharedPropertyStorage<T>,
        IEquatable<ISharedPropertyReference<T>>,
        IPropertyHandlers<ISharedProperty, T>
    {
        new T Value { get; set; }


        new Func<T> ReadonlyValueProvider { get; set; }
    }

    public interface ISharedProperty<T> :
            ISharedPropertyReference<T>,
            IEquatable<T>
        where T : IEquatable<T>
    {
    }
}