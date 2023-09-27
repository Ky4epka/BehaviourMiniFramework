using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Collections.Generic
{

    public class DictionaryLists<ListKey, ListElem> : IDictionary<ListKey, List<ListElem>>
    {
        public List<ListElem> this[ListKey key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICollection<ListKey> Keys => throw new NotImplementedException();
        public ICollection<List<ListElem>> Values => throw new NotImplementedException();
        public int Count => throw new NotImplementedException();
        public bool IsReadOnly => throw new NotImplementedException();

        protected Dictionary<ListKey, List<ListElem>> iData = new Dictionary<ListKey, List<ListElem>>();

        public void Add(ListKey key, List<ListElem> value)
        {
            iData.Add(key, value);
        }

        public void Add(KeyValuePair<ListKey, List<ListElem>> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<ListKey, List<ListElem>> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(ListKey key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<ListKey, List<ListElem>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<ListKey, List<ListElem>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(ListKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<ListKey, List<ListElem>> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(ListKey key, out List<ListElem> value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}