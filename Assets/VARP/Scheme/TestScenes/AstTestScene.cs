/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;
using System.IO;
using VARP.Scheme.Tokenizing;
using VARP.Scheme.STX;
using VARP.Scheme.AST;
using VARP.Scheme.Data;
using VARP.Scheme.Exceptions;

[ ExecuteInEditMode]
public class AstTestScene : MonoBehaviour
{
    private Tokenizer lexer;

    [TextArea(5, 100)]
    public string testString;
    [TextArea(5, 100)]
    public string tokensString;
    [TextArea(5, 100)]
    public string syntaxString;
    [TextArea(10, 100)]
    public string astString;
    [TextArea(5, 100)]
    public string envString;

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        Name.Init ( );
        SystemEnvironment.Init ( );
        System.Text.StringBuilder sb;
        try
        {
            // ------------------------------------------------------------------
            // Just tokenized it
            // ------------------------------------------------------------------
            sb = new System.Text.StringBuilder();
            lexer = new Tokenizer(new StringReader(testString), "AstTest");
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
            lexer = new Tokenizer(new StringReader(testString), "AstTest" );
            sb = new System.Text.StringBuilder();
            do
            {
                SObject result = SyntaxParser.Parse(lexer);
                if (result == null) break;
                sb.AppendLine(Inspector.InspectObject(result));
            } while (lexer.LastToken != null);
            syntaxString = sb.ToString();

            // ------------------------------------------------------------------
            // Parse scheme
            // ------------------------------------------------------------------

            lexer = new Tokenizer(new StringReader(testString), "AstTest" );

            sb = new System.Text.StringBuilder();
            do
            {
                var result = SyntaxParser.Parse(lexer);
                if (result == null) break;
                var ast = AstBuilder.Expand(result, SystemEnvironment.Top);
                sb.AppendLine( Inspector.InspectObject ( ast ) );
            } while (lexer.LastToken != null);
            astString = sb.ToString();
            envString = SystemEnvironment.Top.Inspect();
        }
        catch (SchemeError ex)
        {
            astString = ex.Message;
            throw ex;
        }

        Name.DeInit ( );

    }


}
