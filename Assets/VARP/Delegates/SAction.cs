// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// This improved method of handling the various events used by TextMesh Pro was contributed by TowerOfBricks aka Aron Granberg.


using System.Collections.Generic;

namespace VARP.Delegates
{
    public class SAction
    {

        LinkedList<System.Action> Delegates = new LinkedList<System.Action>();

        public void Add(System.Action function)
        {
            if (Delegates.Contains(function)) return;

            Delegates.AddLast(function);
        }

        public void Remove(System.Action function)
        {
            Delegates.Remove(function);
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
            Delegates.Clear();
        }
    }


    public class SAction<A>
    {

        LinkedList<System.Action<A>> Delegates = new LinkedList<System.Action<A>>();

        public void Add(System.Action<A> function)
        {
            if (Delegates.Contains(function)) return;

            Delegates.AddLast(function);
        }

        public void Remove(System.Action<A> function)
        {
            Delegates.Remove(function);
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
            Delegates.Clear();
        }
    }


    public class SAction<A, B>
    {

        LinkedList<System.Action<A, B>> Delegates = new LinkedList<System.Action<A, B>>();

        public void Add(System.Action<A, B> function)
        {
            if (Delegates.Contains(function)) return;

            Delegates.AddLast(function);
        }

        public void Remove(System.Action<A, B> function)
        {
            Delegates.Remove(function);
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
            Delegates.Clear();
        }
    }


    public class SAction<A, B, C>
    {

        LinkedList<System.Action<A, B, C>> Delegates = new LinkedList<System.Action<A, B, C>>();

        public void Add(System.Action<A, B, C> function)
        {
            if (Delegates.Contains(function)) return;

            Delegates.AddLast(function);
        }

        public void Remove(System.Action<A, B, C> function)
        {
            Delegates.Remove(function);
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
            Delegates.Clear();
        }
    }

}


