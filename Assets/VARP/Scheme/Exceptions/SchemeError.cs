/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Exceptions
{
    using DataStructures;
    using Data;
    using STX;
    using System.Text;

    public class SchemeError : BaseSchemeError
    {
        public SchemeError ( ) : base ( )
        {
        }
        public SchemeError ( string message ) : base ( message )
        {
        }
        public SchemeError ( string message, System.Exception innerException ) : base ( message, innerException )
        {
        }



        // -- Inspector ----------------------------------------------------------------------------------

        /// <summary>
        /// Syntax error message
        /// </summary>
        /// <param name="name">name of method where happen error</param>
        /// <param name="message">error message</param>
        /// <param name="expression">expression where happen error</param>
        /// <param name="subexpression">exact token or syntax where happen error</param>
        /// <returns></returns>
        public static string SyntaxErrorMessage ( string name, string message, object expression, object subexpression = null )
        {
            if ( subexpression == null )
            {
                var expStr = Inspect ( expression );
                var expLoc = GetLocationString ( expression );
                return string.Format ( "{0}: {1}: {2} in: {3}", expLoc, name, message, expStr );
            }
            else
            {
                var expStr = Inspect ( expression );
                var subLoc = GetLocationString ( subexpression );
                var subStr = Inspect ( expression );
                return string.Format ( "{0}: {1}: {2} in: {3}\n error syntax: {4}", subLoc, name, message, expStr, subStr );
            }
        }

        /// <summary>
        /// Create new syntax error exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <param name="expression"></param>
        /// <param name="subexpression"></param>
        /// <returns></returns>
        public static SchemeError SyntaxError ( string name, string message, object expression, object subexpression = null )
        {
            return new SchemeError ( SyntaxErrorMessage ( name, message, expression, subexpression ) );
        }

        // -- Range errors -------------------------------------------------------------------------------

        /// <summary>
        /// Range error message
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeDescription"></param>
        /// <param name="indexPrefix"></param>
        /// <param name="index"></param>
        /// <param name="inValue"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static string RangeErrorMessage ( string name,       //< "vector-ref" | "array-ref"
                                        string typeDescription,   //< "vector" | "array"
                                        string indexPrefix,       //< "start"
                                        int index,                //< current index
                                        object inValue,           //< [1,2,3,4,5,6]
                                        int lowerBound,           //< minimum index
                                        int upperBound )           //< maximum index
        {
            var sb = new StringBuilder ( );
            sb.Append ( name );
            sb.Append ( ": " );
            sb.Append ( indexPrefix );

            var msg = string.Empty;
            if ( index < lowerBound )
                sb.Append ( " index is out of range\n" );
            else if ( index > upperBound )
                sb.Append ( " index is out of range\n" );
            else
                throw new SchemeError ( "Bad arguments" );

            sb.Append ( string.Format ( "  {0} index: {1}\n", indexPrefix, index ) );
            sb.Append ( string.Format ( " valid-range: [{0},{1}]\n", lowerBound, upperBound ) );
            sb.Append ( string.Format ( " {0}: \n  ", typeDescription ) );
            sb.Append ( Inspect ( inValue ) );

            return sb.ToString ( );
        }

        /// <summary>
        /// Range exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeDescription"></param>
        /// <param name="indexPrefix"></param>
        /// <param name="index"></param>
        /// <param name="inValue"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static SchemeError RangeError (
            string name,                            // "vector-ref" | "array-ref"
            string typeDescription,                 // "vector" | "array"
            string indexPrefix,                     // "start"
            int index,                              // current index
            object inValue,                         // [1,2,3,4,5,6]
            int lowerBound,                         // minimum index
            int upperBound )                         // maximum index
        {
            return new SchemeError ( RangeErrorMessage ( name, typeDescription, indexPrefix, index, inValue, lowerBound, upperBound ) );
        }

        
    }

}
