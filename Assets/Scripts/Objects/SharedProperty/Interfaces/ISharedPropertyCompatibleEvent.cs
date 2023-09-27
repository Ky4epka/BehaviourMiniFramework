namespace Main.Objects
{
    using Main.Events;

    public interface ISharedPropertyCompatibleEvent<T>: IEventData
    {
        ISharedProperty Property { get; }
        T PropertyValue { get; }
        T PrevValue { get; }

        void Invoke(ISharedProperty property, T newValue, T prevValue);
        void InvokeFor(ISharedProperty property, T newValue, T prevValue, System.Delegate eventListener);
    }
}
