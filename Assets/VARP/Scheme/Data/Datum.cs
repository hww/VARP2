using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.Scheme.Data
{
    public interface HasDatum
    {
        object GetDatum ( );
        string GetDatumString ( );
    }

    public static class Datum
    {
        public static string ObjectToString (object datum)
        {
            if ( datum == null )
                return "null";
            if ( datum is Variant )
                return ( (Variant)datum ).ToString ( );
            if ( datum is HasDatum )
                return ( (HasDatum)datum ).GetDatumString ( );
            if ( datum is bool )
                return ( (bool)datum ) ? "#t" : "#f";

            //  all another object will be converted by 
            return datum.ToString ( );
        }

    }
}

