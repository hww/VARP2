using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.Scheme.Data
{
    public static class SpecialForm
    {
        public static bool IsSpecialForm ( Name name )
        {
                return name.index == (int)EName.Quote ||
                       name.index == (int)EName.Quasiquote ||
                       name.index == (int)EName.Unquote ||
                       name.index == (int)EName.Unquotesplice;
        }

        public static string ToSpecialFormString ( Name name )
        {
            switch ( (EName)name.index )
            {
                case EName.Quote:
                    return "'";
                case EName.Quasiquote:
                    return "`";
                case EName.Unquote:
                    return ",";
                case EName.Unquotesplice:
                    return ",@";
                default:
                    return name.ToString();
            }
        }
    }
}
