// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// This improved method of handling the various events used by TextMesh Pro was contributed by TowerOfBricks aka Aron Granberg.


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VARP.Delegates
{

    public class TFunction<TResult>
    {

        LinkedList<System.Func<TResult>> Delegates = new LinkedList<System.Func<TResult>>();

        Dictionary<System.Func<TResult>, LinkedListNode<System.Func<TResult>>> Lookup = new Dictionary<System.Func<TResult>, LinkedListNode<System.Func<TResult>>>();

        public void Add(System.Func<TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<TResult> function)
        {
            LinkedListNode<System.Func<TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call()
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value();
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<TResult>> GetDelegates()
        {
            return Delegates;
        }

    }


    public class TFunction<A, TResult>
    {

        LinkedList<System.Func<A, TResult>> Delegates = new LinkedList<System.Func<A, TResult>>();

        Dictionary<System.Func<A, TResult>, LinkedListNode<System.Func<A, TResult>>> Lookup = new Dictionary<System.Func<A, TResult>, LinkedListNode<System.Func<A, TResult>>>();

        public void Add(System.Func<A, TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<A, TResult> function)
        {
            LinkedListNode<System.Func<A, TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call(A a)
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value(a);
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<A, TResult>> GetDelegates()
        {
            return Delegates;
        }
    }


    public class TFunction<A, B, TResult>
    {

        LinkedList<System.Func<A, B, TResult>> Delegates = new LinkedList<System.Func<A, B, TResult>>();

        Dictionary<System.Func<A, B, TResult>, LinkedListNode<System.Func<A, B, TResult>>> Lookup = new Dictionary<System.Func<A, B, TResult>, LinkedListNode<System.Func<A, B, TResult>>>();

        public void Add(System.Func<A, B, TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<A, B, TResult> function)
        {
            LinkedListNode<System.Func<A, B, TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call(A a, B b)
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value(a, b);
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<A, B, TResult>> GetDelegates()
        {
            return Delegates;
        }
    }


    public class TFunction<A, B, C, TResult>
    {

        LinkedList<System.Func<A, B, C, TResult>> Delegates = new LinkedList<System.Func<A, B, C, TResult>>();

        Dictionary<System.Func<A, B, C, TResult>, LinkedListNode<System.Func<A, B, C, TResult>>> Lookup = new Dictionary<System.Func<A, B, C, TResult>, LinkedListNode<System.Func<A, B, C, TResult>>>();

        public void Add(System.Func<A, B, C, TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<A, B, C, TResult> function)
        {
            LinkedListNode<System.Func<A, B, C, TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call(A a, B b, C c)
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value(a, b, c);
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<A, B, C, TResult>> GetDelegates()
        {
            return Delegates;
        }

    }


}
