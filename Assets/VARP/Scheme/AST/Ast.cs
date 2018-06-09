/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.AST
{
    using STX;
    using DataStructures;
    using Data;
    using REPL;
    using VM;

    /// <summary>
    /// This structure have to be used as shortcuts to already existing type such as
    /// Syntax and Pair. Use the data in existing data structure and do not make copy
    /// Variants without reason. It will make more loaded GC but at same time simpify code.
    /// </summary>
    public abstract class AST : SObject
    {
        /// <summary>
        /// This is location of the expression
        /// For instance the expression (+ 1 2) will have
        /// position at first open bracket
        /// But for literal x the position of expression is
        /// position of this literal
        /// 
        /// for example: 
        ///   expression: (+ 1 2) 
        ///   will be: ast('(')
        /// </summary>
        protected Syntax Expression;   

        public AST(Syntax syntax)
        {
            Expression = syntax;
        }

        public object GetDatum() { return Expression == null ? null : Expression.getDatum(); }
        public Syntax GetSyntax() { return Expression; }
        protected Location GetLocation ( ) { return Expression == null ? Location.NullLocation : Expression.getLocation();  }

        // -- SObject Methods ----------------------------------------------------------------------------

        public override bool AsBool() { return true; }
        public override string ToString() { return GetDatum().ToString(); }
        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format ( "#<ast{0} {1}>", GetLocationString ( ), ToString ( ) );
        }
        protected string GetLocationString() { return GetLocation ( ).GetLocationString ( ); }


    }
    
    // ====================================================================================================
    // literal e.g. 99 or #f
    // ====================================================================================================

    public sealed class AstLiteral : AST
    {
        public readonly bool isSyntaxLiteral;

        public AstLiteral(Syntax stx, bool isSyntaxLiteral = false) : base(stx)
        {
            this.isSyntaxLiteral = isSyntaxLiteral;
        }
 
        public override string Inspect( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-literal{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // the variable type
    public enum AstReferenceType
    {
        Local,
        Global,
        UpValue
    }

    // variable reference  e.g. x
    public sealed class AstReference : AST
    {
        /// Type of this reference
        public AstReferenceType ReferenceType;

        /// index of the argument (local var index)
        public byte VarIdx;

        /// index of the referenced environment: 0 for local,  -1 for global
        public short UpEnvIdx = -1;

        /// FrameNum of variable in the referenced environment:  -1 for global
        public short UpVarIdx = -1;

        public Name Identifier;

        /// <summary>
        /// Create new reference
        /// </summary>
        /// <param name="syntax">reference's syntax</param>
        /// <param name="argIdx">index in local scope</param>
        /// <param name="upEnvIdx">index (relative offset) of environment</param>
        /// <param name="upVarIdx">index of variable inside referenced environment</param>
        public AstReference (Syntax syntax, AstReferenceType type, int argIdx) : base(syntax)
        {
            Identifier = ( (SyntaxName)syntax ).asName;
            ReferenceType = type;
            VarIdx = (byte)argIdx;
        }

        /// <summary>
        /// Create new reference
        /// </summary>
        /// <param name="syntax">reference's syntax</param>
        /// <param name="argIdx">index in local scope</param>
        /// <param name="upEnvIdx">index (relative offset) of environment</param>
        /// <param name="upVarIdx">index of variable inside referenced environment</param>
        public AstReference(Syntax syntax, AstReferenceType type, int argIdx, int upEnvIdx, int upVarIdx) : base(syntax)
        {
            Identifier = ( (SyntaxName)syntax ).asName;
            ReferenceType = type;
            VarIdx = (byte)argIdx;
            UpEnvIdx = (short)upEnvIdx;
            UpVarIdx = (short)upVarIdx;
        }

        public bool IsLocal { get { return ReferenceType == AstReferenceType.Local; } }
        public bool IsGlobal { get { return ReferenceType == AstReferenceType.Global; } }
        public bool IsUpVariant { get { return ReferenceType == AstReferenceType.UpValue; } }


        public override string Inspect( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-reference{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // variable assignment e.g. (set! x 99)
    public sealed class AstSet : AST
    {
        public Syntax Variable;             // x   
        public Name Identifier;
        public AST Value;                   // 99
        public int VarIdx;                  // index of variable
        public int UpEnvIdx;                // index of environment 
        public int UpVarIdx;                // index of variables

        public AstSet(Syntax syntax, Syntax variable, AST value, int varIdx, int refEnvIdx, int refVarIdx) : base(syntax)
        {
            Identifier = ( (SyntaxName)variable ).asName;
            VarIdx = (byte)varIdx;
            UpEnvIdx = (short)refEnvIdx;
            UpVarIdx = (short)refVarIdx;
            Variable = variable;
            Value = value;
        }
        public bool IsGlobal { get { return UpVarIdx < 0; } }
        public bool IsUpVariant { get { return UpEnvIdx > 0; } }

        public override string Inspect( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-set{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // conditional e.g. (if 1 2 3)
    public sealed class AstConditionIf : AST
    {
        private Syntax Keyword;
        public AST condExpression;        // 1
        public AST thenExperssion;        // 2
        public AST elseExpression;        // 3

        public AstConditionIf(Syntax syntax, Syntax keyword, AST cond, AST then, AST els) : base(syntax)
        {
            Keyword = keyword;
            condExpression = cond;
            thenExperssion = then;
            elseExpression = els;
        }

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-if{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // conditional e.g. (cond (() .. ) (() ...) (else ...))
    public sealed class AstCondition : AST
    {
        private Syntax Keyword;
        public LinkedList<AST> Conditions;     //< list of pairs
        public LinkedList<AST> ElseCase;       //< else condition

        public AstCondition(Syntax syntax, Syntax keyword, LinkedList<AST> conditions, LinkedList<AST> elseCase) : base(syntax)
        {
            Keyword = keyword;
            Conditions = conditions;
            ElseCase = elseCase;
        }

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-cond{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // primitive op e.g. (+ 1 2)
    public sealed class AstPrimitive : AST
    {
        public Syntax Identifier;
        public LinkedList<Variant> Arguments;
        public AstPrimitive(Syntax syntax, Syntax identifier, LinkedList<Variant> arguments) : base(syntax)
        {
            Identifier = identifier;
            Arguments = arguments;
        }

        public override string Inspect( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-prim{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // application e.g. (f 1 2)
    public sealed class AstApplication : AST
    {
        public LinkedList<AST> list;
        public AstApplication(Syntax syntax, LinkedList<AST> expression) : base(syntax)
        {
            list = expression;
        }

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-app{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // lambda expression   e.g. (lambda(x) x)
    public sealed class AstLambda : AST
    {
        private Syntax Keyword;                      // (<lambda> (...) ...)
        public AstBinding[] ArgList;                 // (lambda <(...)> ) TODO can be replace to reference to Environment!
        public LinkedList<Syntax> BodyExpression;    // (lambda (...) <...>)

        public AstLambda(Syntax syntax, Syntax keyword, Environment environment, LinkedList<Syntax> expression) : base(syntax)
        {
            ArgList = environment.ToAstArray();
            BodyExpression = expression;
            if (((Name)EName.Lambda).Equals(keyword.getDatum ( )))
                Keyword = keyword;
            else
                Keyword = Syntax.Create((Name)EName.Lambda, keyword.getLocation());
        }

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-lam{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

    // sequence e.g. (begin 1 2)
    public sealed class AstSequence : AST
    {
        private Syntax Keyword;
        public LinkedList<Syntax> BodyExpression;

        public AstSequence(Syntax syntax, Syntax keyword, LinkedList<Syntax> expression) : base(syntax)
        {
            Keyword = keyword;
            BodyExpression = expression;
        }

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format("#<ast-seq{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
    }

}