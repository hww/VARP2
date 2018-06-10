/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Data
{
    /* Enum of all embedded names */
    public enum EName
    {
        None,  //< Reserved to be first in list

        SchemeNames, //< All names after are designed for scheme and have to be downcased
        Lambda,
        True,
        False,
        Void,
        Quote,
        Unquote,
        Quasiquote,
        UnquoteSplicing
    }
}