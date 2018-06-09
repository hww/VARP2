/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Globalization;
using UnityEngine;

namespace VARP.Tokenizing
{
    using DataStructures;

    public sealed class Token
    { 
        public TokenType Type;
        public string Value;
        public Location location;

        public Token()
        {
        }

        public Token(TokenType type, string value, Location location = null) 
        {
            this.Type = type;
            this.Value = value;
            this.location = location;
        }

        public Token(Token token) 
        {
            this.Type = token.Type;
            this.Value = token.Value;
            this.location = token.location;
        }

        public override string ToString() { return Value.ToString(); }

        public bool GetBool()
        {
            Debug.Assert(Type == TokenType.Boolean);

            if (Value == "#t")
                return true;
            else if (Value == "#f")
                return false;
            else
                throw TokenizerError.SyntaxError("get-bool", "improperly formed bool value", this);
        }

        public int GetInteger()
        {
            try
            {
                switch (Type)
                {
                    case TokenType.Integer:
                        return StringParser.GetInteger(Value);

                    case TokenType.Heximal:
                        return StringParser.GetHexadecimal (Value);

                    default:
                        throw TokenizerError.SyntaxError("get-integer", "wrong token type", this);
                }
            }
            catch (System.Exception ex)
            {
                throw TokenizerError.SyntaxError("get-integer", "improperly formed int value", this);
            }
        }

        public double GetFloat()
        {
            Debug.Assert(Type == TokenType.Floating);
            return StringParser.GetFloat(Value);
        }

        public string GetString()
        {
            Debug.Assert(Type == TokenType.String);
            return Value;
        }

        public Name GetName()
        {
            Debug.Assert(Type == TokenType.Symbol);
            return new Name(Value, FindName.Add);
        }
    }


}