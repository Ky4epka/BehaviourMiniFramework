using System.Collections.Generic;

namespace Main.Events
{
    public abstract class ListenersMap<Key, DelegateType, ListenersType> : IListenersMap<Key, DelegateType, ListenersType>
        where Key : System.Type
        where DelegateType : System.Delegate
        where ListenersType : IEventListeners<DelegateType>
    {

        protected Dictionary<Key, ListenersType> _EventBase { get; set; } = null;

        public ListenersMap()
        {
            _EventBase = new Dictionary<Key, ListenersType>();
        }

        public ListenersMap(int capacity)
        {
            _EventBase = new Dictionary<Key, ListenersType>(capacity);
        }

        public void ClearEvents()
        {
            foreach (KeyValuePair<Key, ListenersType> pair in _EventBase)
            {
                pair.Value.Clear();
            }

            _EventBase.Clear();
        }


        public void AddListener(Key key, DelegateType listener)
        {
            Listeners(key).AddListener(listener);
        }

        public void RemoveListener(Key key, DelegateType listener)
        {
            Listeners(key).RemoveListener(listener);
        }


        /// <summary>
        /// </summary>
        /// <param name="key">Must have non parametrized constructor for event fabric</param>
        /// <returns></returns>
        protected virtual ListenersType Listeners(Key key)
        {
            ListenersType ev = default;

            if (!_EventBase.TryGetValue(key, out ev))
            {
                ev = DoAddListenersInstance(key);
            }

            return ev;
        }

        public abstract ListenersType ListenersCreator(Key key);

        protected virtual ListenersType DoAddListenersInstance(Key key)
        {
            ListenersType result = ListenersCreator(key);
            _EventBase.Add(key, result);

            return result;
        }
    }
}