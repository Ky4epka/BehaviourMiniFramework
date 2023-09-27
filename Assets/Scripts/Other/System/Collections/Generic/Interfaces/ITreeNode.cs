using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Collections.Generic
{

    public interface ITreeNodeProperties<T>
    {        
        /// <summary>
             /// Only for data store!
             /// Implementation must no has logic
             /// </summary>
        ITreeNode<T> Parent { get; set; }
        ITreeNode<T>[] Childs { get; set; }
        T[] ChildValues { get; set; }
        /// <summary>
        /// Only for data store!
        /// Implementation must no has logic
        /// </summary>
        ITreeNode<T> PrevSibling { get; set; }
        /// <summary>
        /// Only for data store!
        /// Implementation must no has logic
        /// </summary>
        ITreeNode<T> NextSibling { get; set; }
        ITreeNode<T> FirstChild { get; }
        ITreeNode<T> LastChild { get; }
        int DepthLevel { get; }
        T Value { get; set; }
    }

    public interface ITreeNodeManipulation<T>: ITreeNodeProperties<T>
    {
        void ChangeParent(ITreeNode<T> newParent);
        ITreeNode<T> AddAfter(ITreeNode<T> node, T value);
        void AddAfter(ITreeNode<T> node, ITreeNode<T> newNode);
        ITreeNode<T> AddBefore(ITreeNode<T> node, T value);
        void AddBefore(ITreeNode<T> node, ITreeNode<T> newNode);
        ITreeNode<T> AddFirst(T value);
        void AddFirst(ITreeNode<T> node);
        ITreeNode<T> AddLast(T value);
        void AddLast(ITreeNode<T> node);
        void RemoveFirst(bool dont_release_data);
        void RemoveLast(bool dont_release_data);
        int Remove(T value, bool recurse, bool dont_release_data);
        /// <summary>
        /// Removing node from tree and trying dispose a value (if it implements IDispose)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="recurse"></param>
        /// <returns>True if removing successfull</returns>
        bool Remove(ITreeNode<T> value, bool dont_release_data);
        int Remove(Predicate<ITreeNode<T>> remove_condition, bool recurse, bool dont_release_data);

        void MoveBefore(ITreeNode<T> pivot, ITreeNode<T> node);
        void MoveAfter(ITreeNode<T> pivot, ITreeNode<T> node);
    }

    public interface ITreeNodeExplorer<T>: ITreeNodeProperties<T>
    {
        bool Contains(T value, bool recursive);
        bool Contains(ITreeNode<T> node, bool recursive);

        ITreeNode<T> Find(T value);
        ITreeNode<T> Find(T value, bool recursive);
        ITreeNode<T> FindLast(T value);
        ITreeNode<T> FindLast(T value, bool recursive);

        /// <summary>
        /// Direct enumeration of tree with possibility hot-change a collection
        /// </summary>
        /// <param name="action">Enumerate action with possibility break the enumeration loop (return false if need to break)</param>
        /// <param name="recurse"></param>
        /// <returns>True if loop worked a full collection (Path dependent at recurse param)</returns>
        bool Foreach(Predicate<ITreeNode<T>> action, bool recurse);
        /// <summary>
        /// Reverse enumeration of tree with possibility hot-change a collection
        /// </summary>
        /// <param name="action">Enumerate action with possibility break the enumeration loop (return false if need to break)</param>
        /// <param name="recurse"></param>
        /// <returns>True if loop worked a full collection (Path dependent at recurse param)</returns>
        bool ForeachReverse(Predicate<ITreeNode<T>> action, bool recurse);

        /// <summary>
        /// Direct collect a list of nodes by filtering conditions
        /// </summary>
        /// <param name="filter">Filtering condition (return true if need add an element)</param>
        /// <param name="recurse"></param>
        /// <returns>List of collected nodes</returns>
        List<ITreeNode<T>> Filter(Predicate<ITreeNode<T>> filter, bool recurse);
        /// <summary>
        /// Inverse collect a list of nodes by filtering conditions
        /// </summary>
        /// <param name="filter">Filtering condition (return true if need add an element)</param>
        /// <param name="recurse"></param>
        /// <returns>List of collected nodes</returns>
        List<ITreeNode<T>> FilterReverse(Predicate<ITreeNode<T>> filter, bool recurse);

    }

    public interface ITreeNode<T> : ITreeNodeProperties<T>, ITreeNodeManipulation<T>, ITreeNodeExplorer<T>, ICollection<ITreeNode<T>>, ICollection<T>, ICollection, IEnumerable<ITreeNode<T>>, IEnumerable, ICloneable, IDisposable
    { 

    }

}