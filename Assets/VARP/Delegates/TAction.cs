// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// This improved method of handling the various events used by TextMesh Pro was contributed by TowerOfBricks aka Aron Granberg.


using System.Collections.Generic;

namespace VARP.Delegates
{
    public class TAction
    {

        LinkedList<System.Action> Delegates = new LinkedList<System.Action>();

        Dictionary<System.Action, LinkedListNode<System.Action>> Lookup = new Dictionary<System.Action, LinkedListNode<System.Action>>();

        public void Add(System.Action function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Action function)
        {
            LinkedListNode<System.Action> node;
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
    }


    public class TAction<A>
    {

        LinkedList<System.Action<A>> Delegates = new LinkedList<System.Action<A>>();

        Dictionary<System.Action<A>, LinkedListNode<System.Action<A>>> Lookup = new Dictionary<System.Action<A>, LinkedListNode<System.Action<A>>>();

        public void Add(System.Action<A> function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Action<A> function)
        {
            LinkedListNode<System.Action<A>> node;
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
    }


    public class TAction<A, B>
    {

        LinkedList<System.Action<A, B>> Delegates = new LinkedList<System.Action<A, B>>();

        Dictionary<System.Action<A, B>, LinkedListNode<System.Action<A, B>>> Lookup = new Dictionary<System.Action<A, B>, LinkedListNode<System.Action<A, B>>>();

        public void Add(System.Action<A, B> function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Action<A, B> function)
        {
            LinkedListNode<System.Action<A, B>> node;
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
    }


    public class TAction<A, B, C>
    {

        LinkedList<System.Action<A, B, C>> Delegates = new LinkedList<System.Action<A, B, C>>();

        Dictionary<System.Action<A, B, C>, LinkedListNode<System.Action<A, B, C>>> Lookup = new Dictionary<System.Action<A, B, C>, LinkedListNode<System.Action<A, B, C>>>();

        public void Add(System.Action<A, B, C> function)
        {
            if (Lookup.ContainsKey(function)) return;

            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Action<A, B, C> function)
        {
            LinkedListNode<System.Action<A, B, C>> node;
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
    }

}


