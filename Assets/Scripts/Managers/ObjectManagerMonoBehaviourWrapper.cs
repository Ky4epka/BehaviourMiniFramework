using System.Collections;
using System.Collections.Generic;
using Main.Objects;
using System;
using Main.Events;
using System.Reflection;
using Main.Managers.Attributes;

namespace Main.Managers
{
    public class ObjectManagerMonoBehaviourWrapper: CachedMonoBehaviour, IObjectManager
    {
        public Type thisType => (ithisType == null) ? (ithisType = GetType()) : ithisType;
        protected Type ithisType = null;

        public void BindSlaveManagers()
        {
            foreach (PropertyInfo propInfo in thisType.GetProperties())
            {
                if (typeof(IObjectManager).IsAssignableFrom(propInfo.PropertyType))
                {
                    BindManagerAsComponentAttribute attr = propInfo.GetCustomAttribute<BindManagerAsComponentAttribute>(true);

                    if (attr != null)
                    {
                        attr.InjectSlaveManagerProperty(this, propInfo);
                    }
                }
            }
        }
        public virtual IObjectManager ObjectManager { get; protected set; }

        public int Count => ObjectManager.Count;

        public bool IsReadOnly => ObjectManager.IsReadOnly;

        public bool Disposed => ObjectManager.Disposed;

        public IObjectManager MasterManager
        {
            get => ObjectManager.MasterManager;
            set => ObjectManager.MasterManager = value;
        }

        public IBehaviourContainer First => ObjectManager.First;

        public IBehaviourContainer Last => ObjectManager.Last;

        public IBehaviourContainer this[int index] => ObjectManager[index];

        public void Add(IBehaviourContainer item)
        {
            ObjectManager.Add(item);
        }

        public bool AddEventListener(IEventProcessor listener) => ObjectManager.AddEventListener(listener);

        public void AddEventListener(Type key, Delegate listener) => ObjectManager.AddEventListener(key, listener);

        public void AddEventListener<T>(Action<T> listener) where T : IEventData => ObjectManager.AddEventListener<T>(listener);

        public bool BindMutuallyListener(IEventProcessor listener) => ObjectManager.BindMutuallyListener(listener);

        public void Clear() => ObjectManager.Clear();

        public void ClearAllEventListeners() => ObjectManager.ClearAllEventListeners();

        public bool Contains(IBehaviourContainer item) => ObjectManager.Contains(item);

        public void CopyTo(IBehaviourContainer[] array, int arrayIndex) => ObjectManager.CopyTo(array, arrayIndex);

        public void Dispose() => ObjectManager.Dispose();

        public IEventData Event(Type key, IEventProcessor sender) => ObjectManager.Event(key, sender);

        public T Event<T>(IEventProcessor sender) where T : IEventData => ObjectManager.Event<T>(sender);

        public IEnumerator<IBehaviourContainer> GetEnumerator() => ObjectManager.GetEnumerator();

        public void InvokeEvent(Type key, IEventData eventData) => ObjectManager.InvokeEvent(key, eventData);

        public void InvokeEvent<T>(T eventData) where T : IEventData => ObjectManager.InvokeEvent<T>(eventData);

        public bool Remove(IBehaviourContainer item) => ObjectManager.Remove(item);

        public bool RemoveEventListener(IEventProcessor listener) => ObjectManager.RemoveEventListener(listener);

        public void RemoveEventListener(Type key, Delegate listener) => ObjectManager.RemoveEventListener(key, listener);

        public void RemoveEventListener<T>(Action<T> listener) where T : IEventData => ObjectManager.RemoveEventListener<T>(listener);

        public bool UnbindMutuallyListener(IEventProcessor listener) => ObjectManager.UnbindMutuallyListener(listener);

        IEnumerator IEnumerable.GetEnumerator() => ObjectManager.GetEnumerator();

        protected override void Awake()
        {
            base.Awake();
            BindSlaveManagers();
        }
    }
}
