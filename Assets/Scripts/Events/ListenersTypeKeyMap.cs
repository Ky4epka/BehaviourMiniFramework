using System;

namespace Main.Events
{
    public abstract class ListenersTypeKeyMap<DelegateType, ListenersType> : ListenersMap<Type, DelegateType, ListenersType>, IListenersTypeKeyMap<DelegateType, ListenersType>
        where DelegateType : System.Delegate
        where ListenersType : IEventListeners<DelegateType>
    {

        public ListenersTypeKeyMap() : base()
        {
        }

        public ListenersTypeKeyMap(int capacity) : base (capacity)
        {
        }

        public void AddListener<T>(DelegateType listener) 
        {
            Listeners<T>().AddListener(listener);
        }

        public void RemoveListener<T>(DelegateType listener)
        {
            Listeners<T>().RemoveListener(listener);
        }

        protected virtual ListenersType Listeners<T>() 
        {
            return Listeners(typeof(T));
        }

        public abstract ListenersType ListenersCreator<T>();

        public virtual ListenersType DoAddListenersInstance<T>()
        {
            return DoAddListenersInstance(typeof(T));
        }

        public void ClearTypedEvents()
        {
            ClearEvents();
        }
    }
}