using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace System.Collections.Generic
{
    public class PriorityObservable<OBSERVER_TYPE, PRIO_TYPE> : IPriorityObservable<OBSERVER_TYPE>
        where OBSERVER_TYPE : IPriorityObserver<PRIO_TYPE>
        where PRIO_TYPE : IComparable<PRIO_TYPE>
    {
        protected LinkedListEx<OBSERVER_TYPE> iObservers = new LinkedListEx<OBSERVER_TYPE>();

        public PriorityObservable()
        {
            iObservers.SortComparer = (OBSERVER_TYPE value1, OBSERVER_TYPE value2) => { return value2.Priority.CompareTo(value1.Priority); };
        }

        public bool AddObserver(OBSERVER_TYPE observer)
        {
            if (iObservers.Contains(observer)) return false;

            iObservers.AddSorted(observer);
            return true;
        }

        public bool RemoveObserver(OBSERVER_TYPE observer)
        {
            return iObservers.Remove(observer);
        }

        public LinkedListEx<OBSERVER_TYPE> ObserverList
        {
            get => iObservers;
        }
    }
}
