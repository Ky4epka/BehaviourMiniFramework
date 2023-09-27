using Main.Events;
using System;

public interface IEventProcessor
{
    bool AddEventListener(IEventProcessor listener);
    bool RemoveEventListener(IEventProcessor listener);
    bool BindMutuallyListener(IEventProcessor listener);
    bool UnbindMutuallyListener(IEventProcessor listener);

    void AddEventListener(Type key, Delegate listener);
    void AddEventListener<T>(Action<T> listener) where T : IEventData;
    void ClearAllEventListeners();
    void RemoveEventListener(Type key, Delegate listener);
    void RemoveEventListener<T>(Action<T> listener) where T : IEventData;

    IEventData Event(Type key, IEventProcessor sender);
    T Event<T>(IEventProcessor sender) where T : IEventData;

    void InvokeEvent(Type key, IEventData eventData);
    void InvokeEvent<T>(T eventData) where T : IEventData;
}
