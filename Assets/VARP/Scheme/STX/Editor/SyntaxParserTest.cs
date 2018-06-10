/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;
using NUnit.Framework;
using System.IO;
using VARP.Scheme.Tokenizing;
using VARP.Scheme.STX;
using VARP.Scheme.Data;
using VARP.Scheme.Exceptions;

namespace VARP.Scheme.Syntax.Test
{
    public class SyntaxParserTest
    {

        string[] tests = new string[]
        {
            /** Source code, Expected result */ 
            // List
            "()","#<syntax:1:1 ()>",
            "'()","#<syntax:1:1 (quote ())>",
            "(()())","#<syntax:1:1 (() ())>", // Nested List
            // Numbers
            "1 1.1 #xFF","#<syntax:1:1 1> #<syntax:1:3 1.1> #<syntax:1:7 255>",           
            // Strings    
            "\"foo\" \"bar\"","#<syntax:1:1 \"foo\"> #<syntax:1:7 \"bar\">",  
            // Symbols
            "foo bar","#<syntax:1:1 foo> #<syntax:1:5 bar>",          
            // Boolean
            "#t #f","#<syntax:1:1 #t> #<syntax:1:4 #f>",            
            // Characters
            "#\\A #\\space","#<syntax:1:1 #\\A> #<syntax:1:5 #\\space>",
            // Array
            "#(1 2)","#<syntax:1:1 #(1 2)>",
            // Dot syntax
            "(1 . 2)","#<syntax:1:1 (1 . 2)>",
            // Quotes
            "'(,() `() ,@())","#<syntax:1:1 (quote ((unquote ()) (quasiquote ()) (unquote-splicing ())))>",
        };

        [Test]
        public void ParserTestRun()
        {
            Name.Init ( );
            for (int i = 0; i < tests.Length; i += 2)
                Test(tests[i], tests[i + 1]);
            Name.DeInit ( );
        }

        void Test(string source, string expectedResult)
        {
            try
            {
                Tokenizer lexer = new Tokenizer(new StringReader(source), "TokenizerTest");
                SyntaxParser parser = new SyntaxParser();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                bool addSpace = false;
                do
                {
                    SObject result = SyntaxParser.Parse(lexer);
                    if (result == null) break;
                    if (addSpace) sb.Append(" "); else addSpace = true; 
                    sb.Append( Inspector.InspectObject(result));
                    
                } while (lexer.LastToken != null);
                string sresult = sb.ToString();
                Assert.AreEqual(expectedResult, sresult);
            }
            catch (BaseSchemeError ex)
            {
                Debug.LogError(string.Format("Source: {0}\n{1}\n{2}", source, ex.Message, ex.StackTrace));
            }
        }

        string FoundAndExpected(string found, string expected)
        {
            return string.Format(" EXPECTED:\n{0}\n FOUND:\n{1}", expected, found);
        }
    }
}