/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;

namespace VARP.Scheme.Tokenizing.Test
{
    public class TokenizerTest
    {

        string[] tests = new string[]
        {
        // List
        "()","(:OpenBracket ):CloseBracket",
        "(()())","(:OpenBracket (:OpenBracket ):CloseBracket (:OpenBracket ):CloseBracket ):CloseBracket", // Nested List
        // Numbers
        "1 1.0","1:Integer 1.0:Floating",           
        // Strings    
        "\"foo\" \"bar\"","foo:String bar:String",  
        // Symbols
        "foo bar","foo:Symbol bar:Symbol",          
        // Boolean
        "#t #f","#t:Boolean #f:Boolean",            
        // Characters
        "#\\A #\\space","#\\A:Character #\\space:Character",
        "#(1 2)","(:OpenVector 1:Integer 2:Integer ):CloseBracket",
        // Dot syntax
        "(1 . 2)","(:OpenBracket 1:Integer .:Dot 2:Integer ):CloseBracket",
        // Quotes
        "'(,() `() ,@())","':Quote (:OpenBracket ,:Unquote (:OpenBracket ):CloseBracket `:QuasiQuote (:OpenBracket ):CloseBracket ,(:UnquoteSplicing (:OpenBracket ):CloseBracket ):CloseBracket",
        };

        [Test]
        public void TokenizerTestRun()
        {
            for (int i = 0; i < tests.Length; i += 2)
                Test(tests[i], tests[i + 1]);
        }

        void Test(string source, string expectedResult)
        {
            // Just tokenized it
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Tokenizer lexer = new Tokenizer(new StringReader(source), "TokenizerTest");
            Token token = lexer.ReadToken();

            bool addSpace = false;
            while (token != null)
            {
                if (addSpace) sb.Append(" "); else addSpace = true;
                sb.Append(token.value + ":" + token.type.ToString());
                token = lexer.ReadToken();
            }

            string result = sb.ToString();
            Assert.AreEqual(expectedResult, result);
        }

    }
}
