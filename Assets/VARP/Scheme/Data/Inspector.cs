/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace VARP.Scheme.Data
{
    using DataStructures;
    using Data;
    using STX;
    using AST;

    public interface Inspectable
    {
        string Inspect(InspectOptions options = InspectOptions.Default );
    }

    /// <summary>
    /// Formats and prints objects
    /// </summary>
    public static class Inspector
    {
        public const int MAX_ARRAY_PRINT_LEN = 20;
        public const int MAX_CONS_PRINT_LEN = 20;

        public static object NamedChrater { get; private set; }

        // -- Convert to string any object  --------------------------------------------------------------

        public static string ObjectToString (object obj, InspectOptions options = InspectOptions.Default )
        {
            if ( obj == null )
                return "null";
            switch ( options )
            {
                case InspectOptions.Default:
                    return obj.ToString ( );
                case InspectOptions.PrettyPrint:
                    return obj.ToString ( );
                default:
                    return obj.ToString ( );
            }
        }

        // --Inspect any object  -------------------------------------------------------------------------

        public static string InspectObject ( object x, InspectOptions options = InspectOptions.Default )
        {
            if ( x == null )
                return "null";

            if ( x is Variant )
                return ((Variant)x).Inspect ();

            if ( x is SObject)
                return ( (SObject)x ).ToString ( );

            return InspectMonoObject ( x, options );
        }

        // --Inspect Mono Classes ------------------------------------------------------------------------

        private static string InspectMonoObject ( object x, InspectOptions options = InspectOptions.Default )
        {
            // -- Simple objects will be just printed as it is
            if ( x is float )
                return ( (float)x ).ToString ( "0.0###################" );

            if ( x is bool )
                return ( (bool)x ) ? "#t" : "#f";

            if ( x is string )
                return string.Format ( "\"{0}\"", x );

            if ( x is char )
                return NamedCharacter.CharacterToName( (char)x );

            if ( x is Inspectable)
                return ( x as Inspectable).Inspect(options);

            if ( x is LinkedList<object> )
                return InspectLinkedList ( x as LinkedList<object> );

            if ( x is List<object> )
                return InspectList ( x as List<object> );

            if ( x is Dictionary<object, object> )
                return InspectDictionary ( x as Dictionary<object, object> );

            if ( x is Array )
                return InspectArray ( (Array)x, options );

            if ( Type.GetTypeCode ( x.GetType ( ) ) == TypeCode.Object )
                return string.Format ( "#<{0}>", x.ToString ( ).Trim ( ) );

            return x.ToString ( ).Trim ( );
        }


        private static string InspectArray ( Array arr, InspectOptions options = InspectOptions.Default )
        {
            var sb = new StringBuilder ( );

            // For large arrays, don't try to print the elements
            if ( arr.Length > MAX_ARRAY_PRINT_LEN )
            {
                sb.Append ( "#<Array[" );
                for ( var ix = 0 ; ix < arr.Rank ; ++ix )
                {
                    if ( ix > 0 )
                        sb.Append ( " x " );
                    sb.Append ( arr.GetUpperBound ( ix ) - arr.GetLowerBound ( ix ) + 1 );
                }
                sb.Append ( "] " );
            }

            var ind = new int[ arr.Rank ];
            var lb = new int[ arr.Rank ];
            var ub = new int[ arr.Rank ];

            if ( arr.Length <= MAX_ARRAY_PRINT_LEN )
                sb.Append ( String.Format ( CultureInfo.CurrentCulture,
                                        arr.Rank > 1 ? "#{0}a" : "#", arr.Rank ) );

            for ( var ix = 0 ; ix < arr.Rank ; ++ix )
            {
                ind[ ix ] = lb[ ix ] = arr.GetLowerBound ( ix );
                ub[ ix ] = arr.GetUpperBound ( ix );
                sb.Append ( "(" );
            }

            var printedElts = 0;
            do
            {
                try
                {
                    sb.Append ( ObjectToString (arr.GetValue ( ind )) ) ;
                    ++printedElts;
                }
                catch ( IndexOutOfRangeException )
                {
                    // ignore - we just don't print out 0-length arrays
                }
            }
            while ( IncrementIndex ( arr.Rank - 1, ind, lb, ub, sb ) &&
                    printedElts < MAX_ARRAY_PRINT_LEN );

            if ( arr.Length > MAX_ARRAY_PRINT_LEN )
            {
                sb.Append ( " ... " );
                for ( var ix = 0 ; ix < arr.Rank ; ++ix )
                    sb.Append ( ')' );
                sb.Append ( ">" );
            }

            return sb.ToString ( );
        }

        internal static bool IncrementIndex ( int ix, int[] ind, int[] lb, int[] ub, StringBuilder sb )
        {
            var retval = false;
            if ( ix >= 0 )
            {
                if ( ind[ ix ] < ub[ ix ] )
                {
                    ++ind[ ix ];
                    sb.Append ( " " );
                    retval = true;
                }
                else
                {
                    sb.Append ( ")" );
                    ind[ ix ] = lb[ ix ];
                    retval = IncrementIndex ( ix - 1, ind, lb, ub, sb );
                    if ( retval )
                        sb.Append ( "(" );
                }
            }

            return retval;
        }

        private static string InspectLinkedList ( LinkedList<object> list, InspectOptions options = InspectOptions.Default, bool encloseList = true )
        {
            var sb = new StringBuilder ( );

            if ( encloseList )
                sb.Append ( "(" );

            var curent = list.First;
            var consLen = 0;
            while ( curent != null && ++consLen < MAX_CONS_PRINT_LEN )
            {
                sb.Append ( ObjectToString ( curent.Value, options ) );

                curent = curent.Next;
                if ( curent != null )
                    sb.Append ( " " );
            }

            if ( curent != null )
                sb.Append ( " ... " );

            if ( encloseList )
                sb.Append ( ")" );

            return sb.ToString ( );
        }

        private static string InspectList ( List<object> list, InspectOptions options = InspectOptions.Default )
        {
            var sb = new StringBuilder ( );
            var appendSpace = false;
            sb.Append ( "#(" );
            foreach ( var v in list )
            {
                if ( appendSpace )
                    sb.Append ( " " );
                sb.Append ( ObjectToString ( v, options ) );
                appendSpace |= true;
            }
            sb.Append ( ")" );
            return sb.ToString ( );
        }

        private static string InspectDictionary ( Dictionary<object, object> table, InspectOptions options = InspectOptions.Default )
        {
            var sb = new StringBuilder ( );
            var appendSpace = false;
            sb.Append ( "#hash(" );
            foreach ( var v in table )
            {
                if ( appendSpace )
                    sb.Append ( " " );
                sb.Append ( string.Format ( "#<pair {0} {1}>", ObjectToString ( v.Key, options ), ObjectToString ( v.Value, options ) ) );
                appendSpace |= true;
            }
            sb.Append ( ")" );
            return sb.ToString ( );
        }
    }
}
