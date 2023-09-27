using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Main.Events;
using Main.Managers;
using Main.Objects;
using System.Reflection;
using Main.Other;
using Main.Objects.Behaviours.Attributes;

namespace Main.Events
{

    public delegate bool BroadcastEventMapFilter<ENUM_TYPE>(Type event_id,
                                                            IEventData eventData,
                                                            ENUM_TYPE enum_unit,
                                                            object param);




    ///// <summary>
    ///// Generic types:
    ///// 1 (Type) - Collected type;
    ///// 2 (Type) - Attribute type;
    ///// 3 (DelegateInfo) - Delegate info
    ///// </summary>
    //protected Dictionary<Type, Dictionary<Type, List<DelegateInfo>>> iAttributeDelegates = new Dictionary<Type, Dictionary<Type, List<DelegateInfo>>>();

    //protected class DelegateInfo
    //{
    //    public Delegate Delegate;
    //    public Attribute Attribute;

    //    public DelegateInfo(Delegate _delegate, Attribute _attribute)
    //    {
    //        Delegate = _delegate;
    //        Attribute = _attribute;
    //    }
    //}

    public class EventProcessor : EventManager, IEventProcessor
    {
        public bool LogEvents { get; set; } = false;


        protected ListEx<IEventProcessor> iListeners = new ListEx<IEventProcessor>() { UniqueItems = true };

        public EventProcessor() : base()
        {
        }


        public virtual bool AddEventListener(IEventProcessor listener)
        {
            return iListeners.AddItem(listener);
        }

        public virtual bool RemoveEventListener(IEventProcessor listener)
        {
            return iListeners.Remove(listener);
        }

        public virtual bool BindMutuallyListener(IEventProcessor listener)
        {
            return
                iListeners.AddItem(listener) &&
                listener.AddEventListener(this);
        }

        public virtual bool UnbindMutuallyListener(IEventProcessor listener)
        {
            return
                iListeners.Remove(listener) &&
                listener.RemoveEventListener(this);
        }

        public override void Invoke(Type eventId, IEventData eventData)
        {
            if (LogEvents)
            {
                string eventString = eventData.ToString();

                GLog.LogFormat(LogType.Log, "EVENT '{0}' receiver '{1}' Data: {2}",
                    new object[3] { nameof(eventData), null, eventString });
            }

            for (int i = 0; i < iListeners.Count; i++)
            {
                try
                {
                    iListeners[i].InvokeEvent(eventData.thisType, eventData);
                }
                catch (Exception e)
                {
                    GLog.Log(e);
                }
            }

            base.Invoke(eventId, eventData);
        }

        protected override IEventListeners DoAddListenersInstance(Type eventType)
        {
            IEventListeners result = base.DoAddListenersInstance(eventType);
            return result;
        }


        public IEventData Event(Type key, IEventProcessor sender)
        {
            return EventDataBase.InstanceConstructor(key, sender, this);
        }

        public T Event<T>(IEventProcessor sender) where T : IEventData
        {
            return (T)Event(typeof(T), sender); 
        }

        public void AddEventListener(Type key, Delegate listener) => AddListener(key, listener);
        public void AddEventListener<T>(Action<T> listener) where T : IEventData => AddListener<T>(listener);
        public void ClearAllEventListeners() => ClearEvents();
        public void RemoveEventListener(Type key, Delegate listener) => RemoveListener(key, listener);
        public void RemoveEventListener<T>(Action<T> listener) where T : IEventData => RemoveListener<T>(listener);
        public void InvokeEvent(Type key, IEventData eventData) => Invoke(key, eventData);
        public void InvokeEvent<T>(T eventData) where T : IEventData => Invoke<T>(eventData);
    }
}