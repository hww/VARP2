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
        public static string DatumToString(object datum)
        {
            if ( datum == null )
                return "null";
            if ( datum is Variant )
                return ( (Variant)datum ).ToString ( );
            if ( datum is HasDatum )
                return ( (HasDatum)datum ).GetDatumString ( );
            if ( datum is bool )
                return ( (bool)datum ) ? "#t" : "#f";
            if ( datum is int )
                return ( (int)datum ).ToString ( );
            if ( datum is float )
                return ( (float)datum ).ToString ( );
            if ( datum is Name )
                return ( (Name)datum ).ToString ( );
            if ( datum is Name )
                return ( (Name)datum ).ToString ( );
            return datum.ToString ( );
        }

    }
}

