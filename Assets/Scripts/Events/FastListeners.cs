using System;
using System.Collections.Generic;

namespace Main.Events
{

    public class FastListeners<EventData> : IEventListeners<Action<EventData>>, IInvokable<EventData>
    {
        protected HashSet<Action<EventData>> iListeners = new HashSet<Action<EventData>>();
        protected HashSet<Action<EventData>> iAddQueue= new HashSet<Action<EventData>>();
        protected HashSet<Action<EventData>> iRemoveQueue= new HashSet<Action<EventData>>();
        protected bool iInEnumerate = false;
        protected bool iDoClear = false;

        public void AddListener(Action<EventData> listener)
        {
            if (iInEnumerate)
                iAddQueue.Add(listener);
            else
                iListeners.Add(listener);
        }

        public void RemoveListener(Action<EventData> listener)
        {
            if (iInEnumerate)
                iRemoveQueue.Add(listener);
            else
                iListeners.Remove(listener);
        }

        public void Clear()
        {
            if (iInEnumerate)
                iDoClear = true;
            else
                iListeners.Clear();
        }

        public virtual void Invoke(EventData param1)
        {
            try
            {
                iInEnumerate = true;

                foreach (Action<EventData> action in iListeners)
                {
                    action(param1);
                }
            }
            catch (Exception e)
            {
                GLog.LogException(e);
            }
            finally
            {
                iInEnumerate = false;

                if (iDoClear)
                    Clear();
                else
                {
                    foreach (Action<EventData> action in iRemoveQueue)
                        RemoveListener(action);

                    foreach (Action<EventData> action in iAddQueue)
                        AddListener(action);
                }
            }
        }
    }

}