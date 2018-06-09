/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.VM.Functions
{
    using VARP.DataStructures;
    using VARP.Scheme.Data;
    using VARP.Scheme.Exceptions;
    using VARP.Scheme.STX;

    public abstract class Function : SObject
    {

        /// <summary>
        /// Native functions will be called by VM
        /// wit three arguments: CALL A.B.C
        /// 
        /// Operand 'A' contains the index of result 
        /// Operand 'C' is quantity of results
        /// 0: NILL 0 no result
        /// 1: R(A) 
        /// 2: R(A..B)
        /// 3: R(A..C)
        /// Operand 'B' is quantity of arguments
        /// 0: () no arguments
        /// 1: R(A+1)
        /// 2: R(A+1..A+2)
        /// 3: R(A+1..A+3)
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>        
        public abstract void Call(Frame frame, int a, int b, int c);

        // --Assertions ----------------------------------------------------------------------------------

        private static SchemeError ArityError(string name, string message, int argidx, int given, int expected, Variant[] argv)
        {
            var arguments = new LinkedList<Variant> ();
            if (given > 0)
            {
                var lastidx = argidx + given;
                for (var i = argidx; i < lastidx; i++) arguments.AddLast(argv[i]);
            }
            return new SchemeError(SchemeError.ArityErrorMessage(name, message, expected, given, arguments, null));
        }

        protected static void AssertArgsMinimum(string name, string message, int argidx, int given, int expected, Frame frame)
        {
            if (given < expected)
                throw ArityError(name, message, argidx, given, expected, frame.Values);
        }

        protected static void AssertArgsMaximum(string name, string message, int argidx, int given, int expected, Frame frame)
        {
            if (given > expected)
                throw ArityError(name, message, argidx, given, expected, frame.Values);
        }

        protected static void AssertArgsEqual(string name, string message, int argidx, int given, int expected, Frame frame)
        {
            if (given != expected)
                throw ArityError(name, message, argidx, given, expected, frame.Values);
        }


        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<function {0}>", this.GetType().Name); }
        public override string Inspect( InspectOptions options = InspectOptions.Default ) { return ToString(); }

    }
}
