using System;

namespace Main.Events
{
    public interface IEventMap: IListenersMap<Type, Delegate, IEventListeners>, IListenersTypeKeyMap<Delegate, IEventListeners>, IInvokable
    {
        void Invoke(Type key, IEventData eventData);
        void Invoke<T>(T eventData) where T: IEventData;
    }

}