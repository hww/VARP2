/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.AST
{
    using Primitives;
    using VARP.Scheme.Data;
    using VARP.Scheme.VM;
    using VARP.Scheme.VM.Functions;

    public class SystemEnvironment : Environment
    {
        public static SystemEnvironment Top;

        public static void Init()
        {
            Top = new SystemEnvironment ( );
        }

        public static void DeInit ()
        {
            Top = null;
        }

        public SystemEnvironment()
            : base (null, Name.Intern("*SYSTEM-ENV*"), true, 1000)
        {
            /*
            DefinePrimitive("define", PrimitiveDefine.Expand);
            DefinePrimitive("set!", PrimitiveSet.Expand);
            DefinePrimitive("if", PrimitiveIf.Expand);
            DefinePrimitive("cond", PrimitiveCond.Expand);
            DefinePrimitive("lambda", PrimitiveLambda.Expand);
            DefinePrimitive("begin", PrimitiveBegin.Expand);
            DefinePrimitive("let", PrimitiveLet.Expand);

            DefinePrimitive("+", PrimitiveArgs2.Expand);
            DefinePrimitive("-", PrimitiveArgs2.Expand);
            DefinePrimitive("*", PrimitiveArgs2.Expand);
            DefinePrimitive("/", PrimitiveArgs2.Expand);
            DefinePrimitive("%", PrimitiveArgs2.Expand);

            DefinePrimitive("=", PrimitiveArgs2.Expand);
            DefinePrimitive("<", PrimitiveArgs2.Expand);
            DefinePrimitive(">", PrimitiveArgs2.Expand);

            DefinePrimitive("!=", PrimitiveArgs2.Expand);
            DefinePrimitive("<=", PrimitiveArgs2.Expand);
            DefinePrimitive(">=", PrimitiveArgs2.Expand);
            
            DefinePrimitive("pow", PrimitiveArgs2.Expand);

            DefinePrimitive("neg", PrimitiveArgs1.Expand);
            DefinePrimitive("not", PrimitiveArgs1.Expand);
            DefinePrimitive("len", PrimitiveArgs1.Expand);

            DefinePrimitive("or", PrimitiveArgsX.Expand);
            DefinePrimitive("and", PrimitiveArgsX.Expand);

            DefinePrimitive("display", PrimitiveArgsX.Expand);
            DefinePrimitive("concat", PrimitiveArgsX.Expand);

            DefinePrimitive("quote", QuotePrimitive.Expand);
            DefinePrimitive("quaziquote", QuaziquotePrimitive.Expand);

            DefinePrimitive("syntax", SyntaxTools.Syntax);

            DefineFunction("test", Test.Instance);
            */
        }
    }


}