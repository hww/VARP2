/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;

namespace VARP.Scheme.AST
{
    using DataStructures;
    using Data;
    using Exceptions;
    using VM;
    using STX;

    public sealed class AstBuilder : SObject
    {
        [Flags]
        public enum Options
        {
            Default = 0,
            NoLambda = 1
        }

        #region Public Methods

        // Expand string @expression to abstract syntax tree, in given @env environment
        public static Ast Expand(string expression, string filepath, Environment env)
        {
            var syntax = SyntaxParser.Parse(expression, filepath);
            return Expand(syntax, env);
        }

        // Expand string @syntax to abstract syntax tree, in global environment
        public static Ast Expand(Syntax syntax, Environment env)
        {
            if (!env.IsLexical) throw new System.Exception("Expected Lexical environment");
            //if ((options & Options.NoLambda) == 0)
            {
                //   var list = new LinkedList<Value>();
                //   list.AddLast(new Value(Syntax.Lambda));
                //   list.AddLast(new Value(Syntax.Nil));
                //   list.AddLast(new Value(syntax));
                //
                //   var lambda = new Syntax(list, (Location)null);
                //
                //   return ExpandInternal(lambda, env);
            }
            //else
            {
                // expand expression
                //var lexicalEnv = new Environment(env, Symbol.Intern("Lexical"));
                return ExpandInternal(syntax, env);
            }
        }

        // Expand string @syntax to abstract syntax tree, in given @env environment
        public static Ast ExpandInternal(Syntax syntax, Environment env)
        {
            if (syntax == null)
                return null;
            else if (syntax.IsLiteral)
                return ExpandLiteral(syntax, env);
            else if (syntax.IsIdentifier)
                return ExpandIdentifier(syntax, env);
            else if (syntax.IsExpression)
                return ExpandExpression(syntax, env);
            else
                throw SchemeError.SyntaxError("ast-builder-expand", "expected literal, identifier or list expression", syntax);
        }

        #endregion

        #region Private Expand Methods

        // aka: 99
        public static Ast ExpandLiteral(Syntax syntax, Environment env)
        {
            // n.b. value '() is null it will be as literal
            return new AstLiteral(syntax);
        }

        // aka: x
        public static Ast ExpandIdentifier(Syntax syntax, Environment env)
        {
            if (!syntax.IsIdentifier) throw SchemeError.SyntaxError("ast-builder-expand-identifier", "expected identifier", syntax);

            var varname = (Name)syntax.GetDatum();

            // Check and expand some of literals
            if (varname == EName.None)
            {
                return new AstLiteral(syntax);
            }

            // Find the variable in ast environment
            int envIdx = 0;
            var binding = env.LookupAstRecursively(varname, ref envIdx);

            if (binding == null)
            {
                // If variable is not found designate it as global variable
                var localIdx = env.Define(varname, new GlobalBinding(syntax));
                return new AstReference(syntax, AstReferenceType.Global, localIdx);
            }
            else
            {
                if (envIdx == 0)
                {
                    // local variable reference
                    return new AstReference(syntax, AstReferenceType.Local, binding.VarIdx, 0, 0);
                }
                else
                {
                    // up-value reference
                    if (binding is GlobalBinding || !binding.environment.IsLexical)
                    {
                        // global variable
                        var localIdx = env.Define(varname, new GlobalBinding(syntax));
                        return new AstReference(syntax, AstReferenceType.Global, localIdx);
                    }
                    else if (binding is LocalBinding || binding is ArgumentBinding)
                    {
                        // up value to local variable
                        var localIdx = env.Define(varname, new UpBinding(syntax, envIdx, binding.VarIdx));
                        return new AstReference(syntax, AstReferenceType.UpValue, localIdx, envIdx, binding.VarIdx);
                    }
                    else if (binding is UpBinding)
                    {
                        // upValue to other upValue
                        var upBinding = binding as UpBinding;
                        var nEnvIdx = upBinding.UpEnvIdx + envIdx;
                        var nVarIdx = upBinding.UpVarIdx;
                        var localIdx = env.Define(varname, new UpBinding(syntax, nEnvIdx, nVarIdx));
                        return new AstReference(syntax, AstReferenceType.UpValue, localIdx, nEnvIdx, nVarIdx);
                    }
                    else
                    {
                        throw new SystemException();
                    }
                }
            }
        }

        // aka: (...)
        public static Ast ExpandExpression(Syntax syntax, Environment env)
        {
            var list = (syntax  as SyntaxLinkedList).ToLinkedList ( );
            if (list.Count == 0)
                return new AstApplication(syntax, null);
            var ident = list[0] as Syntax;
            if (ident.IsIdentifier)
            {
                var binding = env.LockupAstRecursively(((SyntaxName)ident).asName);
                if (binding != null)
                {
                    if (binding is PrimitiveBinding)
                        return (binding as PrimitiveBinding).Primitive(syntax, env);
                }
            }
            // we do not find primitive. expand all expression with keyword at firs element
            return new AstApplication(syntax, ExpandListElements(list, 0, env));
        }

        // Expand list of syntax objects as: (#<syntax> #<syntax> ...)
        // aka: (...)
        public static LinkedList<Ast> ExpandListElements(LinkedList<Syntax> list, int index, Environment env)
        {
            if (list == null) return null;

            var result = new LinkedList<Ast> ();

            foreach (var v in list)
            {
                if (index == 0)
                    result.AddLast(ExpandInternal(v as Syntax, env));
                else
                    index--;
            }

            return result;
        }

        #endregion
    }


}