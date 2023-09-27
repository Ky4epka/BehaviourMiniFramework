using UnityEngine.Events;
using System;

namespace Main.Events
{
    public interface IManagedEventsListeners: IEventListeners, IEventListeners<Delegate>, IInvokable
    {
        string Name { get; }
        /// <summary>
        /// Cached type of this event
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// param1 - event instance | 
        /// param2 - event data instance
        /// </summary>
        event System.Action<IManagedEventsListeners, IEventData> InvokeHooks;
    }

}