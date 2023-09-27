using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Collections.Generic
{
    public class QueueEx<T> : ICollection
    {
        protected ListEx<T> iList = null;

        public QueueEx()
        {
            iList = new ListEx<T>();
        }

        public QueueEx(bool isUniqueItems)
        {
            iList = new ListEx<T>();
            iList.UniqueItems = isUniqueItems;
        }

        public QueueEx(IEnumerable<T> collection)
        {
            iList = new ListEx<T>(collection);
        }
        public QueueEx(int capacity)
        {
            iList = new ListEx<T>(capacity);
        }

        public int Count { get => iList.Count; }

        public bool IsSynchronized { get; } = false;

        public object SyncRoot 
        { 
            get => (iSyncRoot != null) ? iSyncRoot : iSyncRoot = new object();
        }

        protected object iSyncRoot = null;

        public void Clear()
        {
            iList.Clear();
        }

        public bool Contains(T item)
        {
            return iList.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            iList.CopyTo(array, arrayIndex);
        }
        public T Dequeue()
        {
            T temp = Peek();
            iList.RemoveAt(0);
            return temp;
        }
        public void Enqueue(T item)
        {
            iList.Add(item);
        }

        public T Peek()
        {
            return iList[0];
        }

        public T Bottom()
        {
            return iList[iList.Count - 1];
        }

        public T[] ToArray()
        {
            return iList.ToArray();
        }
        public void TrimExcess()
        {
            iList.TrimExcess();
        }

        public void CopyTo(Array array, int index)
        {
            for (int i=index; i<Count; i++)
            {
                array.SetValue(iList[i - index], i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return iList.GetEnumerator();
        }

        public bool IsEmpty => Count > 0;

        public void PourTo(QueueEx<T> recipient)
        {
            while (!IsEmpty)
            {
                recipient.Enqueue(Dequeue());
            }
        }

        public void PourTo(ICollection<T> recipient)
        {
            PourTo(recipient, null);
        }

        public void PourTo(ICollection<T> recipient, Predicate<T> addCondition)
        {
            T item;
            while (!IsEmpty)
            {
                item = Dequeue();
                if ((addCondition == null) ||
                    addCondition(item))
                    recipient.Add(item);
            }
        }

        public bool Remove(T item)
        {
            return iList.Remove(item);
        }
    }

}