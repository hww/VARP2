﻿/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#pragma warning disable CSE0001   
#pragma warning disable CSE0003

namespace VARP.DataStructures
{
    // =============================================================================
    // This LinkedListNode for a doubly-Linked circular list.
    // =============================================================================

    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class LinkedListNode<T> 
    {
        [NonSerialized]
        internal LinkedList<T> list;
        [NonSerialized]
        internal LinkedListNode<T> next;
        [NonSerialized]
        internal LinkedListNode<T> prev;
        internal T value;

        public LinkedListNode(T value)
        {
            this.value = value;
        }

        internal LinkedListNode(LinkedList<T> list, T value)
        {
            this.list = list;
            this.value = value;
        }

        public LinkedList<T> List
        {
            get { return list; }
        }

        public LinkedListNode<T> Next
        {
            get { return next == null || next == list.head ? null : next; }
        }

        public LinkedListNode<T> Previous
        {
            get { return prev == null || this == list.head ? null : prev; }
        }

        public T Value
        {
            get { return value; }
            set { this.value = value; }
        }

        internal void Invalidate()
        {
            list = null;
            next = null;
            prev = null;
        }


        // =============================================================================
        // List Methods
        // =============================================================================

        public void Remove()
        {
            if (list != null) list.Remove(this);
        }
        public void AddAfter(LinkedListNode<T> newNode)
        {
            Debug.Assert(list != null);
            list.AddAfter(this, newNode);
        }
        public void AddBefore(LinkedListNode<T> newNode)
        {
            Debug.Assert(list != null);
            list.AddBefore(this, newNode);
        }

        #region DebuggerDisplay 
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("#<LinkedListNode {0} {1}>", value, typeof(T));
            }
        }
        #endregion
    }

    // =============================================================================
    // This LinkedList is a doubly-Linked circular list.
    // =============================================================================

   // [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(System_CollectionDebugView<>))]
    [Serializable]
    public class LinkedList<T> : IEnumerable<T>
    {

        // -------------------------------------------------------------------------
        // Fields
        // -------------------------------------------------------------------------

        internal LinkedListNode<T> head;    //< pointer to the last and first
        internal int count;                 //< quantity

        // -------------------------------------------------------------------------
        // Constructors
        // -------------------------------------------------------------------------
        public LinkedList()
        {
        }

        public LinkedList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException( "collection" );

            foreach (var item in collection)
                AddLast(item);
        }

        // -------------------------------------------------------------------------
        // Methods
        // -------------------------------------------------------------------------
        public int Count
        {
            get { return count; }
        }

        public LinkedListNode<T> First
        {
            get { return head; }
        }

        public LinkedListNode<T> Last
        {
            get { return head == null ? null : head.prev; }
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            ValidateNode(node);
            var result = new LinkedListNode<T>(node.list, value);
            InternalInsertNodeBefore(node.next, result);
            return result;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node.next, newNode);
            newNode.list = this;
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            ValidateNode(node);
            var result = new LinkedListNode<T>(node.list, value);
            InternalInsertNodeBefore(node, result);
            if (node == head)
                head = result;
            return result;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node, newNode);
            newNode.list = this;
            if (node == head)
                head = newNode;
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            var result = new LinkedListNode<T>(this, value);
            if (head == null)
            {
                InternalInsertNodeToEmptyList(result);
            }
            else
            {
                InternalInsertNodeBefore(head, result);
                head = result;
            }
            return result;
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            ValidateNewNode(node);

            if (head == null)
            {
                InternalInsertNodeToEmptyList(node);
            }
            else
            {
                InternalInsertNodeBefore(head, node);
                head = node;
            }
            node.list = this;
        }

        public LinkedListNode<T> AddLast(T value)
        {
            var result = new LinkedListNode<T>(this, value);
            if (head == null)
            {
                InternalInsertNodeToEmptyList(result);
            }
            else
            {
                InternalInsertNodeBefore(head, result);
            }
            return result;
        }

        public void AddLast(LinkedListNode<T> node)
        {
            ValidateNewNode(node);

            if (head == null)
            {
                InternalInsertNodeToEmptyList(node);
            }
            else
            {
                InternalInsertNodeBefore(head, node);
            }
            node.list = this;
        }

        public void Clear()
        {
            var current = head;
            while (current != null)
            {
                var temp = current;
                current = current.Next;   // use Next the instead of "next", otherwise it will loop forever
                temp.Invalidate();
            }

            head = null;
            count = 0;
        }

        public bool Contains(T value)
        {
            return Find(value) != null;
        }

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException("index", string.Format("[{0}] Index out of range", index));

            if (array.Length - index < Count)
                throw new ArgumentException("Insufficient space");

            var node = head;
            if (node != null)
            {
                do
                {
                    array[index++] = node.value;
                    node = node.next;
                } while (node != head);
            }
        }

        public LinkedListNode<T> Find(T value)
        {
            var node = head;
            var c = EqualityComparer<T>.Default;
            if (node != null)
            {
                if (value != null)
                {
                    do
                    {
                        if (c.Equals(node.value, value))
                            return node;
                        node = node.next;
                    } while (node != head);
                }
                else
                {
                    do
                    {
                        if (node.value == null)
                            return node;
                        node = node.next;
                    } while (node != head);
                }
            }
            return null;
        }

        public LinkedListNode<T> FindLast(T value)
        {
            if (head == null) return null;

            var last = head.prev;
            var node = last;
            var c = EqualityComparer<T>.Default;
            if (node != null)
            {
                if (value != null)
                {
                    do
                    {
                        if (c.Equals(node.value, value))
                        {
                            return node;
                        }

                        node = node.prev;
                    } while (node != last);
                }
                else
                {
                    do
                    {
                        if (node.value == null)
                        {
                            return node;
                        }
                        node = node.prev;
                    } while (node != last);
                }
            }
            return null;
        }

        public bool Remove(T value)
        {
            var node = Find(value);
            if (node != null)
            {
                InternalRemoveNode(node);
                return true;
            }
            return false;
        }

        public void Remove(LinkedListNode<T> node)
        {
            ValidateNode(node);
            InternalRemoveNode(node);
        }

        public void RemoveFirst()
        {
            if (head == null) { throw new InvalidOperationException("LinkedList empty"); }
            InternalRemoveNode(head);
        }

        public void RemoveLast()
        {
            if (head == null) { throw new InvalidOperationException("LinkedList empty"); }
            InternalRemoveNode(head.prev);
        }
        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (array.Rank != 1)
                throw new ArgumentException("MultiRank");

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("Non zero lower bound");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", string.Format("Index out of range {0}", index));

            if (array.Length - index < Count)
                throw new ArgumentException("Insufficient space");

            var tArray = array as T[];
            if (tArray != null)
            {
                CopyTo(tArray, index);
            }
            else
            {
                //
                // Catch the obvious case assignment will fail.
                // We can found all possible problems by doing the check though.
                // For example, if the element type of the Array is derived from T,
                // we can't figure out if we can successfully copy the element beforehand.
                //
                var targetType = array.GetType().GetElementType();
                var sourceType = typeof(T);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                    throw new ArgumentException("Invalid array type");

                var objects = array as object[];
                if (objects == null)
                    throw new ArgumentException("Invalid array type");
                var node = head;
                try
                {
                    if (node != null)
                    {
                        do
                        {
                            objects[index++] = node.value;
                            node = node.next;
                        } while (node != head);
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type");
                }
            }
        }
        
        // -------------------------------------------------------------------------
        // Internals
        // -------------------------------------------------------------------------

        private void InternalInsertNodeBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev.next = newNode;
            node.prev = newNode;
            count++;
        }

        private void InternalInsertNodeToEmptyList(LinkedListNode<T> newNode)
        {
            Debug.Assert(head == null && count == 0, "LinkedList must be empty when this method is called!");
            newNode.next = newNode;
            newNode.prev = newNode;
            head = newNode;
            count++;
        }

        internal void InternalRemoveNode(LinkedListNode<T> node)
        {
            Debug.Assert(node.list == this, "Deleting the node from another list!");
            Debug.Assert(head != null, "This method shouldn't be called on empty list!");
            if (node.next == node)
            {
                Debug.Assert(count == 1 && head == node, "this should only be true for a list with only one node");
                head = null;
            }
            else
            {
                node.next.prev = node.prev;
                node.prev.next = node.next;
                if (head == node)
                {
                    head = node.next;
                }
            }
            node.Invalidate();
            count--;
        }

        internal void ValidateNewNode(LinkedListNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.list != null)
                throw new InvalidOperationException("External link node");
        }


        internal void ValidateNode(LinkedListNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.list != this)
                throw new InvalidOperationException("External link node");
        }


        #region Additional Enumerators

        public IEnumerator<LinkedListNode<T>> NodesFromLast()
        {
            var node = Last;
            while (node != null)
            {
                yield return node as LinkedListNode<T>;
                node = node.Previous;
            }
        }

        public IEnumerator<LinkedListNode<T>> NodesFromFirst()
        {
            var node = First;
            while (node != null)
            {
                yield return node as LinkedListNode<T>;
                node = node.Next;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            var node = First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Additional Converters

        public T[] ToArray()
        {
            var array = new T[Count];
            CopyTo(array,0);
            return array;
        }

        public List<T> ToList()
        {
            var list = new List<T>(Count);
            foreach(var v in this)
                list.Add(v);
            return list;
        }

        #endregion

        #region Array Interface
        public LinkedListNode<T> GetNodeAtIndex(int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < count);
            var current = First;
            while (index>0)
            {
                current = current.Next;
                index--;
            }
            return current;
        }

        public T this[int index]
        {
            get { return GetNodeAtIndex(index).Value; }
            set { GetNodeAtIndex(index).Value = value; }
        }

        #endregion

        #region Two Lists Operations Interface

        public void Append(LinkedList<T> other)
        {
            Debug.Assert(other != null);
            while (other.Count > 0)
            {
                var first = other.First;
                other.Remove(first);
                AddLast(first);
            }
        }

        public LinkedList<T> Duplicate()
        {
            var result = new LinkedList<T>();
            foreach (var v in this)
            {
                var n = new LinkedListNode<T>(v);
                result.AddLast(n);
            }
            return result;
        }

        /// <summary>
        /// Make sublist from give index, and up to size
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size">if -1 then up to end of list</param>
        /// <returns></returns>
        public LinkedList<T> Duplicate(int index, int size)
        {
            Debug.Assert(index >= 0);
            var result = new LinkedList<T>();
            if (index >= Count) return result;
            var current = GetNodeAtIndex(index);
            while (current != null && (size < 0 || size > 0))
            {
                var n = new LinkedListNode<T>(current.Value);
                result.AddLast(n);
                size--;
                current = current.Next;
            }
            return result;
        }

        /// <summary>
        /// Make sublist from give index, and up to size
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size">if -1 then up to end of list</param>
        /// <returns></returns>
        public LinkedList<T> DuplicateReverse(int index, int size)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < Count);
            Debug.Assert(index+size < Count);

            var result = new LinkedList<T>();
            var current = GetNodeAtIndex(index);

            while (current != null && (size < 0 || size > 0))
            {
                var n = new LinkedListNode<T>(current.Value);
                result.AddFirst(n);
                size--;
                current = current.Next;
            }
            return result;
        }

        public void Reverse()
        {
            if (count < 2) return;

            LinkedListNode<T> temp = null;
            var last = First; // this will be last
            var start = First; // this is iterator
            head = Last;
            while (start != null)
            {
                temp = start.next;
                start.next= start.prev;
                start.prev = temp;
                start = start.Previous;
            }

        }
        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");

            var curent = First;
            while (curent != null)
            {
                sb.Append(curent.Value.ToString());

                curent = curent.Next;
                if (curent != null) sb.Append(" ");
            }
            sb.Append(")");
            return sb.ToString();
        }

        #region DebuggerDisplay 
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("#<LinkedList Count={0}>", Count);
            }
        }
        #endregion
    }

    //
    // Custom debugger type proxy to display collections as arrays
    //
    internal sealed class System_CollectionDebugView<T>
    {
        private ICollection<T> collection;

        public System_CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var items = new T[collection.Count];
                collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}