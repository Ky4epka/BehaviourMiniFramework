namespace Main.Events
{
    public interface IListenersTypeKeyMap<DelegateType, ListenersType>
        where DelegateType : System.Delegate
        where ListenersType : IEventListeners<DelegateType>
    {
        void AddListener<T>(DelegateType listener);
        void RemoveListener<T>(DelegateType listener);

        void ClearTypedEvents();
    }
}