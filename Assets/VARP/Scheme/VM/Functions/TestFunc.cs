/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.VM.Functions
{
    using Data;
    using UnityEngine;

    public class Test : Function
    {
        public static Function Instance = new Test();

        // (test @expected @expression)
        // (test @expected @expression @message) 
        public override void Call(Frame frame, int a, int b, int c)
        {
            if (b < 2) AssertArgsMinimum("test", "arity mismatch", a, b, 2, frame);
            if (b > 3) AssertArgsMaximum("test", "arity mismatch", a, b, 3, frame);

            var left = frame.Values[a + 1];
            var right = frame.Values[a + 2];
            if (left.Equals(right))
            {
                frame.Values[a] = (Variant)true;
            }
            else
            {
                if (b == 2)
                    Debug.LogErrorFormat("Expect: {0}\n  Found: {1}", Inspector.InspectObject(left), Inspector.InspectObject(right));
                else
                    Debug.LogErrorFormat("{0}\n  Expect: {1}\n  Found: {2}", frame.Values[a + 3], Inspector.InspectObject(left), Inspector.InspectObject(right));
                frame.Values[a] = (Variant)false;
            }
        }
    }
}