/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;
using System.IO;
using VARP.Scheme.Tokenizing;
using VARP.Scheme.STX;
using VARP.Scheme.Data;

[ExecuteInEditMode]
public class TokenizerTestScene : MonoBehaviour {
    private Tokenizer lexer;

    [TextArea(20,100)]
    public string testString;
    [TextArea(10, 100)]
    public string tokensString;

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate ( )
    {
        Name.Init ( );
        System.Text.StringBuilder sb;

        // ------------------------------------------------------------------
        // Just tokenized it
        // ------------------------------------------------------------------
        sb = new System.Text.StringBuilder ( );
        lexer = new Tokenizer ( new StringReader ( testString ), "TokenizerTest" );
        var token = lexer.ReadToken ( );
        while ( token != null )
        {
            sb.Append ( Inspector.InspectObject ( token ) + " " );
            token = lexer.ReadToken ( );
        }
        tokensString = sb.ToString ( );

    }
}
