/*  Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Data
{
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class SObject 
    {
        public virtual bool AsBool() { return false; }

        // -- DebuggerDisplay ----------------------------------------------------------------------------

        public string DebuggerDisplay
        {
            get {
                try {
                    return Inspector.InspectObject ( this );
                }
                catch (System.Exception ex) {
                    return string.Format("#<{0} ispect-error='{1}'>", GetType().Name, ex.Message);
                }
            }
        }

        // -- Inspect obct by static  --------------------------------------------------------------------

        public static bool ObjectToBool ( object obj )
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