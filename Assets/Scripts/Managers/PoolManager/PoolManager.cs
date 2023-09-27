using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Player;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using System;

namespace Main.Aggregator.Events.Managers.PoolManager
{

    public sealed class DoRentElementEvent: EventDataBase
    {
        public IBehaviourContainer ResultReceiver { get; private set; }

        public Type ElementType { get; private set; }

        public void Invoke(IBehaviourContainer resultReceiver, Type elementType)
        {
            ResultReceiver = resultReceiver;
            ElementType = elementType;
            base.Invoke();
        }
    }

    /// <summary>
    /// This event used as callback event for equiring pool element and sends direct to receiver (if is set)
    /// </summary>
    public sealed class OnReceiverRentElementEvent : EventDataBase
    {
        public IDataPool_Element ElementInstance { get; private set; }
        public bool IsSuccess { get; private set; }

        public void Invoke(IDataPool_Element elementInstance, bool isSuccess)
        {
            ElementInstance = elementInstance;
            IsSuccess = isSuccess;
            base.Invoke();
        }
    }

    public sealed class DoReturnElementEvent : EventDataBase
    {
        public IDataPool_Element ElementInstance { get; private set; }

        public void Invoke(IDataPool_Element elementInstance)
        {
            ElementInstance = elementInstance;
            base.Invoke();
        }
    }

    public sealed class DoCreatePool: EventDataBase
    {
        public Type ElementType { get; private set; }
        public IDataPoolElementFabric ElementFabric { get; private set; }
        public int InitialCapacity { get; private set; }
        public bool UseAutoGrow { get; private set; }

        public void Invoke(Type elementType, IDataPoolElementFabric elementFabric, int initialCapacity, bool useAutoGrow)
        {
            ElementType = elementType;
            ElementFabric = elementFabric;
            InitialCapacity = initialCapacity;
            UseAutoGrow = useAutoGrow;
            base.Invoke();
        }
    }

    public sealed class DoDeletePool : EventDataBase
    {
        public Type ElementType { get; private set; }

        public void Invoke(Type elementType)
        {
            ElementType = elementType;
            base.Invoke();
        }
    }
}

namespace Main.Managers
{
    public class CustomDataPool<T> : DataPool<T> where T : IDataPool_Element
    {
        public CustomDataPool(IDataPoolElementFabric elementFabric) : base(elementFabric)
        {
        }

        public CustomDataPool(IDataPoolElementFabric elementFabric, int capacity) : base(elementFabric, capacity)
        {
        }
    }

    [RequireComponent(typeof(PoolManagerContainer))]
    public class PoolManager : ObjectBehavioursBase, IDisposable
    {
        protected Dictionary<Type, IDataPool> iPools = new Dictionary<Type, IDataPool>();
        protected bool iDisposed = false;

        protected static PoolManager iInstance = null;

        public static PoolManager Instance
        {
            get
            {
                iInstance = (!iInstance) ? iInstance = FindObjectOfType<PoolManager>() : iInstance;

                if (!iInstance)
                {
                    GameObject container = new GameObject("PoolManager");
                    iInstance = container.AddComponent<PoolManager>();
                }

                return iInstance;
            }
        }

        [EnabledStateEvent]
        public void DoRentElementEvent(Aggregator.Events.Managers.PoolManager.DoRentElementEvent eventData)
        {
            IDataPool_Element element = RentElement(eventData.ElementType);

            eventData.ResultReceiver?.Event<Aggregator.Events.Managers.PoolManager.OnReceiverRentElementEvent>(Container).Invoke(element, element != null);
        }

        [EnabledStateEvent]
        public void DoReturnElementEvent(Aggregator.Events.Managers.PoolManager.DoReturnElementEvent eventData)
        {
            ReturnElement(eventData.ElementInstance);
        }

        [EnabledStateEvent]
        public void DoCreatePool(Aggregator.Events.Managers.PoolManager.DoCreatePool eventData)
        {
            CreatePool(eventData.ElementType, eventData.ElementFabric, eventData.InitialCapacity, eventData.UseAutoGrow);
        }

        [EnabledStateEvent]
        public void DoDeletePool(Aggregator.Events.Managers.PoolManager.DoDeletePool eventData)
        {
            DeletePool(eventData.ElementType);
        }

        public IDataPool CreatePool(Type pooledType, IDataPoolElementFabric elementFabric, int initialCapacity, bool useAutoGrow)             
        {
            if (HasPool(pooledType))
            {
                GLog.LogError(nameof(PoolManager), $"Pool with ident key '{pooledType}' already exists!");
                return null;
            }

            IDataPool pool = new CustomDataPool<IDataPool_Element>(elementFabric, initialCapacity);
            iPools.Add(pooledType, pool);
            return pool;
        }

        public bool DeletePool(Type pooledType)
        {
            if (!HasPool(pooledType))
            {
                return false;
            }

            iPools[pooledType].ClearPool();
            iPools.Remove(pooledType);
            return true;
        }

        public void ClearAll()
        {
            foreach (var keyval in iPools)
                keyval.Value.ClearPool();
        }

        public bool HasPool(Type pooledType)
        {
            return iPools.ContainsKey(pooledType);
        }

        public IDataPool_Element RentElement(Type elementType)
        {
            IDataPool pool;

            if (!iPools.TryGetValue(elementType, out pool))
            {
                GLog.LogError(nameof(PoolManager), $"Could not rent element of type '{elementType.FullName}'. Pool not found.");
                return default;
            }

            return pool.RentElement();
        }

        public T RentElement<T>() where T : IDataPool_Element
        {
            return (T)RentElement(typeof(T));
        }

        public void ReturnElement(IDataPool_Element element)
        {
            IDataPool pool;

            if (!iPools.TryGetValue(element?.GetType(), out pool))
            {
                GLog.LogError(nameof(PoolManager), $"Could not return element of type '{element?.GetType()?.FullName}'. Pool not found.");
                return;
            }

            pool.ReturnElement(element);
        }

        protected override void Start()
        {
            base.Start();

        }

        public void Dispose()
        {
            if (iPools.Count != 0)

            ClearAll();
            iDisposed = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Dispose();
        }
    }
}
