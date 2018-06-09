/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Diagnostics;

namespace VARP.Scheme.AST
{
    using VM;
    using DataStructures;
    using Data;
    using STX;

    public abstract class AstBinding : Binding
    {
        public readonly Name Identifier;     //< the variable name definition
        public int VarIdx;                   //< the variable indexs

        /// <summary>
        /// Create global binding
        /// </summary>
        /// <param name="variable"></param>
        protected AstBinding(Syntax variable)
        {
            Debug.Assert(variable != null);
            Identifier = ((SyntaxName)variable).asName;
        }

        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<binding {0}>", Identifier); }

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format ( "#<binding {0}>", Identifier );
        }
    }

    public sealed class PrimitiveBinding : AstBinding
    {
        public delegate AST CompilerPrimitive(Syntax expression, Environment context);

        public CompilerPrimitive Primitive;     

        public PrimitiveBinding(Syntax identifier, CompilerPrimitive primitive) 
            : base (identifier)
        {
            Debug.Assert(primitive != null);
            Primitive = primitive;
        }

        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<primitive {0}>", Identifier); }
        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format ( "#<primitive {0}>", Identifier );
        }
    }
    
    public sealed class LocalBinding: AstBinding
    {
        public LocalBinding(Syntax identifier) : base(identifier) { }

        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<local-binding {0}>", Identifier); }
    }
    
    public sealed class GlobalBinding : AstBinding
    {
        public GlobalBinding(Syntax identifier) : base(identifier) { }

        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<global-binding {0}>", Identifier); }
    }

    public sealed class UpBinding : AstBinding
    {
        public int UpEnvIdx;
        public int UpVarIdx;

        public UpBinding ( Syntax identifier, int envIdx, int varIdx )
            : base ( identifier )
        {
            UpEnvIdx = envIdx;
            UpVarIdx = varIdx;
        }

        public override bool AsBool ( ) { return true; }
        public override string ToString ( ) { return string.Format ( "#<up-binding {0}>", Identifier ); }
        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
                return string.Format ( "[{0},{1}] {1}>", UpEnvIdx, UpVarIdx, Identifier );
        }
    }

    public sealed class ArgumentBinding : AstBinding
    {
        public enum Type
        {
            Required,       // (lambda (x y z) ...)
            Optionals,      // (lambda (x y #!optional z) ...)
            Key,            // (lambda (x y #!key z) ...)
            Rest,           // (lambda (x y #!rest z) ...)
            Body,           // (lambda (x y #!body z) ...)
            Define,
            End             // after #!res value
        }

        public Type ArgType;
        public AstBinding Refrence;
        public AST Initializer;

        public ArgumentBinding(Syntax identifier, Type type, AST initializer) : base(identifier)
        {
            ArgType = type;
            Initializer = initializer;
        }

        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<arg-binding {0}>", Identifier); }
    }

}
