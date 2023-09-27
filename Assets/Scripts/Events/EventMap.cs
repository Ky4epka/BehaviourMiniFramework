using System;
using UnityEngine.Events;

namespace Main.Events
{


    public class EventMap : ListenersTypeKeyMap<Delegate, IEventListeners>, IEventMap
    {

        public EventMap()
        { 
        }

        public override IEventListeners ListenersCreator<T>()
        {
            return ListenersCreator(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Must implements constructor with IEventHandler param</param>
        /// <returns></returns>
        public override IEventListeners ListenersCreator(Type key)
        {
            try
            {
                return Activator.CreateInstance(typeof(EventListeners<>).MakeGenericType(key)) as IEventListeners;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log($"Exception: {e.Message}; typeKey: {key.FullName}");
                throw e;
            }
        }

        public IEventListeners EventListeners(Type key)
        {
            return Listeners(key);
        }

        public IEventListeners EventListeners<T>() where T : Type
        {
            return Listeners<T>();
        }

        public virtual void Invoke(Type key, IEventData eventData)
        {
            EventListeners(key).Invoke(eventData);
        }

        public void Invoke<T>(T eventData) where T : IEventData
        {
            Invoke(typeof(T), eventData);
        }

        public virtual void Invoke(IEventData eventData)
        {
            Invoke(eventData.thisType, eventData);
        }

        protected IEventProcessor iEventHandler = null;
    }


}