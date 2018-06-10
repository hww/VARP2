/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;
using System.IO;
using VARP.Scheme.Tokenizing;
using VARP.Scheme.STX;
using VARP.Scheme.Data;

[ExecuteInEditMode]
public class SyntaxParserTestScene : MonoBehaviour {
    private Tokenizer lexer;

    [TextArea(20,100)]
    public string testString;
    [TextArea(10, 100)]
    public string tokensString;
    [TextArea(10, 100)]
    public string resultString;

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        Name.Init ( );
        System.Text.StringBuilder sb;

        // ------------------------------------------------------------------
        // Just tokenized it
        // ------------------------------------------------------------------
        sb = new System.Text.StringBuilder();
        lexer = new Tokenizer(new StringReader(testString), "SyntaxParserTest" );
        var token = lexer.ReadToken();
        while (token != null)
        {
            sb.Append(Inspector.InspectObject(token) + " ");
            token = lexer.ReadToken();
        }
        tokensString = sb.ToString();

        // ------------------------------------------------------------------
        // Parse scheme
        // ------------------------------------------------------------------
        lexer = new Tokenizer(new StringReader(testString), "SyntaxParserTest" );
        sb = new System.Text.StringBuilder();
        do
        {
            SObject result = SyntaxParser.Parse(lexer);
            if (result == null) break;
            sb.AppendLine( Inspector.InspectObject ( result ) );
        } while (lexer.LastToken != null);
        resultString = sb.ToString();
        Name.DeInit ( );
    }


}
