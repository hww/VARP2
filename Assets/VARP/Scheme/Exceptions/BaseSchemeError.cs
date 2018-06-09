/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Exceptions
{
    using Data;
    using Tokenizing;
    using STX;
    using DataStructures;

    /// <summary>
    /// General exception class
    /// </summary>
    public class BaseSchemeError : System.ApplicationException
    {
        public BaseSchemeError ( ) : base ( )
        {
        }
        public BaseSchemeError ( string message ) : base ( message )
        {
        }
        public BaseSchemeError ( string message, System.Exception innerException ) : base ( message, innerException )
        {
        }

        // -- Arity errors ----------------------------------------------------------------------------------

        /// <summary>
        /// Arity error message
        /// </summary>
        /// <param name="name">function name wehre happens error</param>
        /// <param name="message">the error message</param>
        /// <param name="expected">expected arguments quantity</param>
        /// <param name="given">given arguments quantity</param>
        /// <param name="argv">arguments</param>
        /// <param name="expression">the expression whenre happens error</param>
        /// <returns></returns>
        public static string ArityErrorMessage ( string name, string message, int expected, int given, LinkedList<Variant> argv, Syntax expression )
        {
            var sb = new System.Text.StringBuilder ( );
            sb.Append ( GetLocationString ( expression ) );
            sb.Append ( "arity mismatch;\n" );
            sb.Append ( "  the expected number of arguments does not match the given number\n" );
            sb.Append ( string.Format ( "  expected: {0}\n", expected ) );
            sb.Append ( string.Format ( "  given: {0}\n", given ) );
            sb.Append ( "  arguments...:\n" );
            foreach ( var arg in argv )
                sb.AppendLine ( "  " + Inspect ( arg ) );
            return sb.ToString ( );
        }

        /// <summary>
        /// Arity exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <param name="expected"></param>
        /// <param name="given"></param>
        /// <param name="argv"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SchemeError ArityError ( string name, string message, int expected, int given, LinkedList<Variant> argv, Syntax expression )
        {
            return new SchemeError ( ArityErrorMessage ( name, message, expected, given, argv, expression ) );
        }

        // -- Expand location string from object -----------------------------------------------------------

        protected static string GetLocationString ( object x )
        {
            if ( x is Location )
                return ( x as Location ).GetLocationString ( );
            if ( x is Token )
                return ( x as Token ).GetLocation ( ).GetLocationString ( );
            if ( x is Syntax )
                return ( x as Syntax ).getLocation ( ).GetLocationString ( );
            return string.Empty;
        }

        // -- Inspector --------------------------------------------------------------------------------------

        /// <summary>
        /// Inspect object for error message
        /// Standart REPL inspector uses o.Inspect() method
        /// this version will use AsString() method
        /// </summary>
        /// <param name="o"></param>
        protected static string Inspect ( object o )
        {
            if ( o == null )
                return "()";
            return o.ToString ( );
        }

    }
}
