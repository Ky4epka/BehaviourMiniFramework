using System;
using System.Collections;

namespace System.Collections.Generic
{
    public class TreeNode<T> : ITreeNode<T>
    {
        public TreeNode()
        {

        }

        public TreeNode(ITreeNode<T> parent)
        {
            ChangeParent(parent);
        }

        public TreeNode(T value)
        {
            Value = value;
        }

        public TreeNode(ITreeNode<T> parent, T value)
        {
            Value = value;
            ChangeParent(parent);
        }

        public ITreeNode<T> Parent
        {
            get => iParent;
            set
            {
                if (Parent == value)
                    return;

                iParent = value;
                RecalcDepthLevel();
            }
        }


        public ITreeNode<T>[] Childs
        {
            get
            {
                ITreeNode<T>[] result = new ITreeNode<T>[Count];
                CopyTo(result, 0);

                return result;
            }
            set
            {
                Clear();

                foreach (ITreeNode<T> node in value)
                {
                    Add(node);
                }
            }
        }

        public ITreeNode<T> PrevSibling
        {
            get => iPrevSiblingNode;
            set
            {
                if (value == this)
                    throw new InvalidOperationException("Attempt looping a tree node ");

                iPrevSiblingNode = value;
            }
        }

        public ITreeNode<T> NextSibling
        {
            get => iNextSiblingNode;
            set
            {
                if (value == this)
                    throw new InvalidOperationException("Attempt looping a tree node ");

                iNextSiblingNode = value;
            }
        }

        protected ITreeNode<T> iParent = null;
        protected ITreeNode<T> iFirstNode = null;
        protected ITreeNode<T> iLastNode = null;
        protected ITreeNode<T> iPrevSiblingNode = null;
        protected ITreeNode<T> iNextSiblingNode = null;
        protected T iValue = default;
        protected int iCount = 0;
        protected int iDepthLevel = 0;

        public int Count
        {
            get => iCount;
        }

        public bool IsReadOnly => throw new System.NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public T[] ChildValues
        {
            get
            {
                T[] result = new T[Count];
                CopyTo(result, 0);

                return result;
            }
            set
            {
                Clear();

                foreach (T elem in value)
                {
                    Add(elem);
                }
            }
        }
        public T Value
        {
            get => iValue;
            set => iValue = value;
        }

        public ITreeNode<T> FirstChild
        {
            get => iFirstNode;
        }

        public ITreeNode<T> LastChild
        {
            get => iLastNode;
        }

        public int DepthLevel
        {
            get => iDepthLevel;
        }

        public virtual void Add(T item)
        {
            AddLast(item);
        }

        public void Clear()
        {
            foreach (ITreeNode<T> node in this)
            {
                (node as ICollection<ITreeNode<T>>).Clear();
                node.ChangeParent(null);
            }
        }

        void ICollection<T>.Clear() => Clear();

        public bool Contains(ITreeNode<T> node)
        {
            return Contains(node, false);
        }

        public bool Contains(T item)
        {
            return Contains(item, false);
        }

        public bool Contains(ITreeNode<T> node, bool recursive)
        {
            bool match = false;
            Foreach((ITreeNode<T> current) => { if (current == node) match = true; return true; }, recursive);

            return match;
        }

        public bool Contains(T item, bool recursive)
        {
            return Find(item, recursive) != null;
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (ITreeNode<T> node in this)
            {
                array[arrayIndex++] = node.Value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            foreach (ITreeNode<T> node in this)
            {
                array.SetValue(node.Value, index++);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public bool Remove(T item)
        {
            return Remove(item, false, false) > 0;
        }

        public virtual int Remove(T item, bool recurse, bool dont_release_data)
        {
            int count = 0;
            Foreach((ITreeNode<T> node) => { if (node.Value.Equals(item)) { Remove(node, dont_release_data); count++; } return true; }, recurse);

            return count;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public void Add(ITreeNode<T> item)
        {
            AddLast(item);
        }

        public void CopyTo(ITreeNode<T>[] array, int arrayIndex)
        {
            foreach (ITreeNode<T> node in this)
            {
                array[arrayIndex++] = node.Clone() as ITreeNode<T>;
            }
        }

        public bool Remove(ITreeNode<T> item)
        {
            return Remove(item, false);
        }

        public virtual bool Remove(ITreeNode<T> item, bool dont_release_data)
        {
            if (item.Parent != this)
                throw new InvalidOperationException("Tree node is not a child this parent");

            if (FirstChild == null) return false;

            item.Parent = null;

            if (FirstChild == LastChild)
            {
                iFirstNode = iLastNode = null;
            }
            else if (FirstChild == item)
            {
                iFirstNode = item.NextSibling;
                iFirstNode.PrevSibling = null;
            }
            else if (LastChild == item)
            {
                iLastNode = item.PrevSibling;
                iLastNode.NextSibling = null;
            }
            else
            {
                item.PrevSibling.NextSibling = item.NextSibling;
                item.NextSibling.PrevSibling = item.PrevSibling;
            }

            item.PrevSibling = null;
            item.NextSibling = null;
            iCount--;

            if (!dont_release_data)
                (item.Value as IDisposable)?.Dispose(); 

            return true;
        }

        IEnumerator<ITreeNode<T>> IEnumerable<ITreeNode<T>>.GetEnumerator()
        {
            return new NodeEnumerator(this);
        }

        public void RemoveFirst(bool dont_release_data)
        {
            Remove(FirstChild, dont_release_data);
        }

        public void RemoveLast(bool dont_release_data)
        {
            Remove(LastChild, dont_release_data);
        }

        public ITreeNode<T> AddAfter(ITreeNode<T> node, T value)
        {
            ITreeNode<T> result = new TreeNode<T>(value);
            AddAfter(node, result);
            return result;
        }

        public virtual void AddAfter(ITreeNode<T> node, ITreeNode<T> newNode)
        {
            if (newNode == null)
                throw new ArgumentNullException("newNode");

            if (newNode.Parent != null)
                throw new InvalidOperationException("Before adding a node to tree it need to delete from a previous parent");

            if (node == null)
                node = LastChild;

            if (LastChild == null)
            {
                iFirstNode = newNode;
                iLastNode = newNode;
            }
            else
            {
                newNode.PrevSibling = node;
                newNode.NextSibling = node.NextSibling;

                if (node.NextSibling != null)
                    node.NextSibling.PrevSibling = newNode;

                node.NextSibling = newNode;

                if (node == LastChild)
                    iLastNode = newNode;
            }

            newNode.Parent = this;
            iCount++;
        }

        public ITreeNode<T> AddBefore(ITreeNode<T> node, T value)
        {
            ITreeNode<T> result = new TreeNode<T>(value);
            AddBefore(node, result);
            return result;
        }

        public virtual void AddBefore(ITreeNode<T> node, ITreeNode<T> newNode)
        {
            if (newNode == null)
                throw new ArgumentNullException("newNode");

            if (newNode.Parent != null)
                throw new InvalidOperationException("Before adding a node to tree it need to delete from a previous parent");

            if (node == null)
                node = FirstChild;

            if (FirstChild == null)
            {
                iFirstNode = newNode;
                iLastNode = newNode;
            }
            else
            {
                newNode.NextSibling = node;
                newNode.PrevSibling = node.PrevSibling;

                if (node.PrevSibling != null)
                    node.PrevSibling.NextSibling = newNode;

                node.PrevSibling = newNode;

                if (node == FirstChild)
                    iFirstNode = newNode;
            }

            newNode.Parent = this;
            iCount++;
        }

        public ITreeNode<T> AddFirst(T value)
        {
            return AddBefore(FirstChild, value);
        }

        public void AddFirst(ITreeNode<T> node)
        {
            AddBefore(FirstChild, node);
        }

        public virtual ITreeNode<T> AddLast(T value)
        {
            return AddAfter(LastChild, value);
        }

        public virtual void AddLast(ITreeNode<T> node)
        {
            AddAfter(LastChild, node);
        }

        public ITreeNode<T> Find(T value)
        {
            return Find(value, false);
        }

        public ITreeNode<T> Find(T value, bool recursive)
        {
            ITreeNode<T> result = null;
            Foreach((ITreeNode<T> node) => { if (node.Value.Equals(value)) { result = node; return false; } return true; }, recursive);

            return result;
        }

        public ITreeNode<T> FindLast(T value)
        {
            return FindLast(value, false);
        }

        public ITreeNode<T> FindLast(T value, bool recursive)
        {
            ITreeNode<T> result = null;
            ForeachReverse((ITreeNode<T> node) => { if (node.Value.Equals(value)) { result = node; return false; } return true; }, recursive);

            return result;
        }

        public virtual object Clone()
        {
            TreeNode<T> result = new TreeNode<T>(Value);

            Foreach(
                (ITreeNode<T> node) =>
                {
                    result.Add(node.Clone() as ITreeNode<T>);
                    return true;
                },
                false
            );

            return result;
        }

        public bool Foreach(Predicate<ITreeNode<T>> action, bool recurse)
        {
            ITreeNode<T> node = FirstChild;

            while (node != null)
            {
                if (!action(node) || 
                    (recurse && !node.Foreach(action, recurse)))
                    return false;

                node = node.NextSibling;
            }

            return true;
        }

        public bool ForeachReverse(Predicate<ITreeNode<T>> action, bool recurse)
        {
            ITreeNode<T> node = LastChild;

            while (node != null)
            {
                if (!action(node) ||
                    (recurse && !node.Foreach(action, recurse)))
                    return false;

                node = node.PrevSibling;
            }

            return true;
        }

        public List<ITreeNode<T>> Filter(Predicate<ITreeNode<T>> filter, bool recurse)
        {
            List<ITreeNode<T>> result = new List<ITreeNode<T>>();

            Foreach(
                (ITreeNode<T> node) =>
                {
                    if (filter(node))
                        result.Add(node);

                    return true;
                },
                recurse
            );

            return result;
        }

        public List<ITreeNode<T>> FilterReverse(Predicate<ITreeNode<T>> filter, bool recurse)
        {
            List<ITreeNode<T>> result = new List<ITreeNode<T>>();

            ForeachReverse(
                (ITreeNode<T> node) =>
                {
                    if (filter(node))
                        result.Add(node);

                    return true;
                },
                recurse
            );

            return result;
        }

        public void ChangeParent(ITreeNode<T> newParent)
        {
            if (Parent != null)
            {
                Parent.Remove(this);
            }

            Parent = newParent;

            if (Parent != null)
            {
                Parent.Add(this);
            }
        }

        public int Remove(Predicate<ITreeNode<T>> remove_condition, bool recurse, bool dont_release_data)
        {
            int count_removed = 0;

            foreach (ITreeNode<T> node in Filter(remove_condition, recurse))
            {
                Remove(node, dont_release_data);
                count_removed++;
            }

            return count_removed;
        }

        public class NodeEnumerator : IEnumerator<ITreeNode<T>>
        {
            public ITreeNode<T> Current
            {
                get => iCurNode;
            }

            object IEnumerator.Current => Current;

            protected ITreeNode<T> iEnumNode = null;
            protected ITreeNode<T> iCurNode = null;

            public NodeEnumerator(ITreeNode<T> enum_node)
            {
                iEnumNode = enum_node;
            }

            public bool MoveNext()
            {
                iCurNode = iCurNode?.NextSibling;
                return iCurNode != null;
            }

            public void Reset()
            {
                iCurNode = iEnumNode.FirstChild;
            }

            public void Dispose()
            {
            }
        }

        public class Enumerator : IEnumerator<ITreeNode<T>>, IEnumerator<T>, IEnumerator
        {
            public T Current 
            {
                get => iCurNode.Value;
            }

            object IEnumerator.Current => Current;

            ITreeNode<T> IEnumerator<ITreeNode<T>>.Current
            {
                get => iCurNode;
            }

            protected ITreeNode<T> iEnumNode = null;
            protected ITreeNode<T> iCurNode = null;

            public Enumerator(ITreeNode<T> enum_node)
            {
                iEnumNode = enum_node;
            }

            public bool MoveNext()
            {
                iCurNode = iCurNode?.NextSibling;
                return iCurNode != null;
            }

            public void Reset()
            {
                iCurNode = iEnumNode.FirstChild;
            }

            public void Dispose()
            {
            }
        }

        protected void RecalcDepthLevel()
        {
            iDepthLevel = (Parent?.DepthLevel + 1) ?? 0;
        }

        /// <summary>
        /// Deleting node from a parent and clearing childs
        /// </summary>
        public void Dispose()
        {
            Clear();
            ChangeParent(null);
        }

        public void MoveBefore(ITreeNode<T> pivot, ITreeNode<T> node)
        {
            if ((pivot.Parent == this) && (node.Parent == this))
                throw new InvalidOperationException("Parent of moving node and pivot node must be a general.");

            Remove(node, true);
            AddBefore(pivot, node);
        }

        public void MoveAfter(ITreeNode<T> pivot, ITreeNode<T> node)
        {
            if (node.Parent != pivot.Parent)
                throw new InvalidOperationException("Parent of moving node and pivot node must be a general.");

            Remove(node, true);
            AddAfter(pivot, node);
        }

        public override string ToString()
        {
            return ToString(".");
        }

        public virtual string ToString(string separator)
        {
            string result = String.Join("", GetType().Name, " L", DepthLevel, "_", Value?.ToString() ?? "");

            if (Parent == null)
                return result;
            else
                return String.Join(separator, Parent.ToString(), result);

        }
    }

}