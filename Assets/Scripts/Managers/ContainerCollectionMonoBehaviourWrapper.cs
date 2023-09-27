using System.Collections;
using System.Collections.Generic;
using Main.Objects;
using Main.Events;
using System;

namespace Main.Managers
{
    public class ContainerCollectionMonoBehaviourWrapper: CachedMonoBehaviour, IContainerCollection
    {
        public IBehaviourContainer this[int index] => ObjectCollection[index];

        public IContainerCollection ObjectCollection { get; set; }

        public int Count => ObjectCollection.Count;
        public bool IsReadOnly => ObjectCollection.IsReadOnly;
        public bool Disposed => ObjectCollection.Disposed;

        public IBehaviourContainer First => ObjectCollection.First;

        public IBehaviourContainer Last => ObjectCollection.Last;

        public virtual void Add(IBehaviourContainer item) => ObjectCollection.Add(item);
        public bool AddEventListener(IEventProcessor listener) => ObjectCollection.AddEventListener(listener);
        public void AddEventListener(Type key, Delegate listener) => ObjectCollection.AddEventListener(key, listener);
        public void AddEventListener<T>(Action<T> listener) where T : IEventData => ObjectCollection.AddEventListener<T>(listener);
        public bool BindMutuallyListener(IEventProcessor listener) => ObjectCollection.BindMutuallyListener(listener);
        public virtual void Clear() => ObjectCollection.Clear();
        public void ClearAllEventListeners() => ObjectCollection.ClearAllEventListeners();
        public bool Contains(IBehaviourContainer item) => ObjectCollection.Contains(item);
        public void CopyTo(IBehaviourContainer[] array, int arrayIndex) => ObjectCollection.CopyTo(array, arrayIndex);
        public void Dispose() => ObjectCollection.Dispose();
        public IEventData Event(Type key, IEventProcessor sender) => ObjectCollection.Event(key, sender);
        public T Event<T>(IEventProcessor sender) where T : IEventData => ObjectCollection.Event<T>(sender);
        public IEnumerator<IBehaviourContainer> GetEnumerator() => ObjectCollection.GetEnumerator();
        public void InvokeEvent(Type key, IEventData eventData) => ObjectCollection.InvokeEvent(key, eventData);
        public void InvokeEvent<T>(T eventData) where T : IEventData => ObjectCollection.InvokeEvent<T>(eventData);
        public virtual bool Remove(IBehaviourContainer item) => ObjectCollection.Remove(item);
        public bool RemoveEventListener(IEventProcessor listener) => ObjectCollection.RemoveEventListener(listener);
        public void RemoveEventListener(Type key, Delegate listener) => ObjectCollection.RemoveEventListener(key, listener);
        public void RemoveEventListener<T>(Action<T> listener) where T : IEventData => ObjectCollection.RemoveEventListener<T>(listener);
        public bool UnbindMutuallyListener(IEventProcessor listener) => ObjectCollection.UnbindMutuallyListener(listener);
        IEnumerator IEnumerable.GetEnumerator() => ObjectCollection.GetEnumerator();
    }
}
