/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;
using NUnit.Framework;
using System.IO;


namespace VARP.Scheme.Test
{
    using Tokenizing;
    using STX;
    using REPL;
    using VM;
    using AST;
    using Data;

    public class AstTest
    {

        string[] tests = new string[]
        {
        // List
        "()","nil",
        "'()","(quote nil)",
        "(()())","(nil nil)", // Nested List
        // Numbers
        "1 1.0 #xFF","1 1.0 255",           
        // Strings    
        "\"foo\" \"bar\"","\"foo\" \"bar\"",  
        // Symbols
        "+ -","+ -",
        // Boolean
        "#t #f","#t #f",            
        // Characters
        "#\\A #\\space","#\\A #\\space",
        // Array
        "#(1 2)","#(1 2)",
        // Dot syntax
        "'(1 . 2)","(quote (1 . 2))",
        // Quotes
        "'(,() `() ,@())","(quote ((unquote nil) (quasiquote nil) (unquote-splicing nil)))",
        // Lambda
        "(lambda (x y) (+ x y) (- x y))","(lambda (x y) (+ x y) (- x y))",
        "(lambda (x y &optional (o1 1) (o2 2) &key (k1 1) (k2 2) &rest r))","(lambda (x y &optional (o1 1) (o2 2) &key (k1 1) (k2 2) &rest r))",
        // Let
        "(let ((x 1) (y 2)) 3 4)","(let ((x 1) (y 2)) 3 4)",
        // Conditions
        "(if 1 2)","(if 1 2)",
        "(if 1 2 3)","(if 1 2 3)",
        "(if (1) (2) (3))","(if (1) (2) (3))",
        "(cond (1 2) (3 4) (else 5))","(cond (1 2) (3 4) (else 5))",
        "(cond (1 2) (3 4))","(cond (1 2) (3 4))",
        // Application
        "(display 1 2)","(display 1 2)",
        // Variable reference
        "+", "+",
        // Primitives
        "(not 1)","(not 1)",
        "(display 1 2 2 3)","(display 1 2 2 3)",
        "(and 1 2 2 3)","(and 1 2 2 3)"
        };

        [Test]
        public void AstTestRun()
        {
            for (int i = 0; i < tests.Length; i += 2)
                Test(tests[i], tests[i + 1]);
        }

        void Test(string source, string expectedResult)
        {
            try
            {
                Tokenizer lexer = new Tokenizer(new StringReader(source), "AstTest.cs/sample code");

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                bool addSpace = false;
                do
                {
                    Syntax result = SyntaxParser.Parse(lexer);

                    if (result == null) break;

                    var env = new Environment(SystemEnvironment.Top, EName.None, true);
                    var ast = AstBuilder.Expand(result, env);

                    if (addSpace) sb.Append(" "); else addSpace = true;
                    sb.Append(Inspector.Inspect(ast.GetDatum()));
                    
                } while (lexer.LastToken != null);
                string sresult = sb.ToString();
                Assert.AreEqual(expectedResult, sresult);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("{0}\n{1}\n{2}", source, ex.Message, ex.StackTrace));
            }
        }

    }
}