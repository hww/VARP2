/*  Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Data
{
    using System.Diagnostics;
    using VARP.Scheme.REPL;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class SObject : Inspectable
    {
        public virtual bool AsBool() { return false; }
        public virtual string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return ToString();
        }

        // -- DebuggerDisplay ----------------------------------------------------------------------------

        public string DebuggerDisplay
        {
            get {
                try {
                    return Inspect ( );
                }
                catch (System.Exception ex) {
                    return string.Format("#<{0} ispect-error='{1}'>", GetType().Name, ex.Message);
                }
            }
        }

        // -- Inspect obct by static  --------------------------------------------------------------------

        public static string InspectObject ( object obj, InspectOptions options = InspectOptions.Default )
        {
            if ( obj == null )
                return "null";
            if ( obj is Inspectable )
                return ( (Inspectable)obj ).Inspect ( options );
            return obj.ToString ( );
        }

        public static bool ConvertToBool( object obj )
        {
            if ( obj == null )
                return false;
            if ( obj is SObject)
                return ( (SObject)obj ).AsBool ( );
            return false;
        }
    }

    public enum InspectOptions
    {
        Default,
        PrettyPrint
    }

}