using System;
using System.Collections.Generic;

namespace Main.Events
{
    /// <summary>
    /// Prioritable execute list with executing order from last added high prio up to first added low prio listeners.
    /// NOTE: The delegate return if true then the breaks invoke listeners
    /// </summary>
    /// <typeparam name="EventData"></typeparam>
    public class PriorityListeners<EventData> : IEventListeners<Func<EventData, bool>>, IPriorityListeners<Func<EventData, bool>>, IInvokable<EventData>
    {
        protected class ExecuteItem
        {
            public Func<EventData, bool> Delegate;
            public ListenerPriority Priority;

            public ExecuteItem(Func<EventData, bool> del, ListenerPriority prio)
            {
                Delegate = del;
                Priority = prio;
            }
        }

        protected LinkedListEx<ExecuteItem> iListeners = new LinkedListEx<ExecuteItem>();

        public PriorityListeners()
        {
            iListeners.SimilarItemsToBegin = true;
            iListeners.SortComparer =
                (ExecuteItem cmp1, ExecuteItem cmp2) =>
                {
                    return cmp2.Priority - cmp1.Priority;
                };
        }

        public void AddListener(Func<EventData, bool> listener)
        {
            AddListener(listener, ListenerPriority.Normal);
        }

        public void AddListener(Func<EventData, bool> listener, ListenerPriority prio)
        {
            iListeners.AddSorted(new ExecuteItem(listener, prio));
        }

        public void RemoveListener(Func<EventData, bool> listener)
        {
            iListeners.RemoveWhere((ExecuteItem item) => { return item.Delegate.Equals(listener); });
        }

        public void Clear()
        {
            iListeners.Clear();
        }

        public virtual void Invoke(EventData param1)
        {
            LinkedListNode<ExecuteItem> node = iListeners.First;
            LinkedListNode<ExecuteItem> next;

            while (node != null)
            {
                next = node.Next;

                if (!node.Value.Delegate(param1))
                    break;

                node = next;
            }
        }
    }

}