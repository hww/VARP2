/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Tokenizing
{
    public enum TokenType
    {
        Undefined,
        /* Parentheses */
        OpenBracket,                    // (
        OpenVector,                     // #(
        CloseBracket,                   // )
        /* Standard scheme types */
        Symbol,                         // foo
        Integer,                        // 12
        Heximal,                        // 0x12
        Floating,                       // 12.34
        String,                         // "Whatever"
        Boolean,                        // #t, #f
        Character,                      // #\A
        /* Scheme syntax elements */
        Quote,                          // '
        QuasiQuote,                     // `    (QuasiQuote)
        Unquote,                        // ,    (Unquote)
        UnquoteSplicing,                // ,@   (UnquoteSplicing)
        Dot,
        /* Lexical errors */
        BadHash,                        // # followed by an unrecognized sequence (ie, #X or something)
        BadNumber,						// #b222 or similar - a badly formatted number
        BadSyntax,                      // A syntax error of some kind (eg, missing ")
        /* Extended scheme types */
        Object,							// #[System.IO.Stream(45)]
    }
}