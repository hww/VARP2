/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Globalization;
using System.Text;
using UnityEngine;
using VARP.DataStructures;

namespace VARP.Tokenizing
{
    /// <summary>
    /// Syntax error methods
    /// </summary>
    public partial class TokenizerError : System.ApplicationException
    {
        public TokenizerError ( ) : base()
        {
        }

        public TokenizerError ( string message ) : base(message)
        {
        }

        public TokenizerError ( string message, System.Exception innerException ) : base(message, innerException)
        {
        }

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
        public static TokenizerError SyntaxError ( string name, string message, object expression, object subexpression = null )
        {
            return new TokenizerError ( SyntaxErrorMessage ( name, message, expression, subexpression ) );
        }

        // ================================================================================
        // Expand location string from object
        // ================================================================================

        protected static string GetLocationString ( object x )
        {
            if ( x is Location )
                return GetLocationStringIntern ( x as Location );
            if ( x is Token )
                return GetLocationStringIntern ( ( x as Token ).location );
            return string.Empty;
        }


        protected static string GetLocationStringIntern ( Location x )
        {
            return x == null ? string.Empty : string.Format ( "{0}({1},{2}): ", x.File, x.LineNumber, x.ColNumber );
        }

        // ================================================================================
        // Inspector
        // ================================================================================

        /// <summary>
        /// Inspect object for error message
        /// Standart REPL inspector uses o.Inspect() method
        /// this version will use AsString() method
        /// </summary>
        /// <param name="o"></param>
        protected static string Inspect ( object o )
        {
            if ( o == null )
                return "null";
            if ( o is Variant )
                return ( (Variant)o ).ToString ( );
            return o.ToString ( );
        }

        /// <summary>
        /// Create simple message without any formatting and inspect objects
        /// </summary>
        /// <param name="message">the mesage</param>
        /// <param name="fields">inspected objects</param>
        public static string ErrorMessage ( string message, params object[] fields )
        {
            var sb = new StringBuilder ( );
            sb.Append ( message );
            sb.Append ( ": " );
            foreach ( var v in fields )
            {
                sb.Append ( " " );
                sb.Append ( Inspect ( v ) );
            }
            return sb.ToString ( );
        }

        // ================================================================================
        // Simple error messages
        // ================================================================================

        public static TokenizerError Error ( string message, params object[] fields )
        {
            return new TokenizerError ( ErrorMessage ( message, fields ) );
        }

        public static string ErrorMessageWithName ( string name, string message, params object[] fields )
        {
            return ErrorMessage ( string.Format ( "{0}: {1}", name, message ), fields );
        }

        public static TokenizerError ErrorWithName ( string name, string message, params object[] fields )
        {
            return new TokenizerError ( ErrorMessageWithName ( name, message, fields ) );
        }
    }
}