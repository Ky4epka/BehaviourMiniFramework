using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Diagnostics;

namespace Main.Events
{

    public interface IListenersMap<Key, DelegateType, ListenersType>
        where DelegateType : System.Delegate
        where ListenersType : IEventListeners<DelegateType>
    {
        void AddListener(Key key, DelegateType listener);
        void RemoveListener(Key key, DelegateType listener);

        void ClearEvents();
    }
}