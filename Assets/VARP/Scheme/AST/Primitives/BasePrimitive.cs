/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.AST.Primitives
{
    using DataStructures;
    using Exceptions;
    using Data;
    using STX;

    public class BasePrimitive
    {

        protected static int GetArgsCount(LinkedList<Variant> o) { return o.Count - 1; }

        protected static void AssertArgsMinimum(string name, string message, int expected, int given, LinkedList<Variant> argv, Syntax expression)
        {
            if (given < expected)
                throw ParserError.ArityError(name, message, expected, given, argv, expression);
        }

        protected static void AssertArgsMaximum(string name, string message, int expected, int given, LinkedList<Variant> argv, Syntax expression)
        {
            if (given > expected)
                throw ParserError.ArityError(name, message, expected, given, argv, expression);
        }

        protected static void AssertArgsEqual(string name, string message, int expected, int given, LinkedList<Variant> argv, Syntax expression)
        {
            if (given != expected)
                throw ParserError.ArityError(name, message, expected, given, argv, expression);
        }

    }
}