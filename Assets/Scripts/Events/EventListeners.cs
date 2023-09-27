using System;

namespace Main.Events
{
    public class EventListeners<Event> : FastListeners<Event>, IEventListeners
        where Event : IEventData
    {
        public virtual string Name { get => Type.FullName; }
        public Type Type
        {
            get
            {
                if (iThisType == null)
                    iThisType = GetType();

                return iThisType;
            }
        }

        public static Type DelegateType
        {
            get => typeof(Action<Event>);
        }

        public EventListeners() : base()
        {

        }

        protected static Type iThisType = null;

        public void AddListener(Delegate listener)
        {
            base.AddListener((Action<Event>)listener);
        }

        public void RemoveListener(Delegate listener)
        {
            base.RemoveListener((Action<Event>)listener);
        }

        public void Invoke(IEventData param1)
        {
            base.Invoke((Event)param1);
        }
    }

}