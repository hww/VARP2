/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Globalization;
using UnityEngine;

namespace VARP.Scheme.Tokenizing
{
    using Data;
    using Exceptions;

    public sealed class Token : SObject
    { 
        public TokenType type;
        public string value;
        public Location location;

        public Token()
        {
        }
        public Token(TokenType type, string value, Location location = null ) 
        {
            this.type = type;
            this.value = value;
            this.location = location;
        }
        public Token(Token token) 
        {
            this.type = token.type;
            this.value = token.value;
            this.location = token.location;
        }

        // -- Parse value --------------------------------------------------------------------------------

        public bool GetBool()
        {
            Debug.Assert(type == TokenType.Boolean);

            if (value == "#t")
                return true;
            else if (value == "#f")
                return false;
            else
                throw TokenizerError.SyntaxError("get-bool", "improperly formed bool value", this);
        }
        public int GetInteger()
        {
            try
            {
                switch (type)
                {
                    case TokenType.Integer:
                        return StringParser.GetInteger (value);

                    case TokenType.Heximal:
                        return StringParser.GetHexadecimal (value);

                    default:
                        throw TokenizerError.SyntaxError("get-integer", "wrong token type", this);
                }
            }
            catch (System.Exception ex)
            {
                throw TokenizerError.SyntaxError("get-integer", "improperly formed int value", this);
            }
        }
        public float GetFloat()
        {
            Debug.Assert(type == TokenType.Floating);
            return StringParser.GetFloat(value);
        }
        public string GetString()
        {
            Debug.Assert(type == TokenType.String);
            return value;
        }
        public Name GetName()
        {
            Debug.Assert(type == TokenType.Symbol);
            return new Name(value, FindName.Add);
        }
        public char GetCharacter ( )
        {
            Debug.Assert ( type == TokenType.Character );
            if ( value.Length == 3 )
            {
                return System.Convert.ToChar ( value[ 2 ] );
            }
            else
            {
                var c = (char)0;
                if ( NamedCharacter.NameToCharacter ( value, out c ) )
                    return c;
                throw TokenizerError.SyntaxError ( "get-character", "improperly formed char value", this );
            }
        }
        // -- Conversion ---------------------------------------------------------------------------------

        public override string ToString ( ) {
            return value;
        }

        // -- Debugging ----------------------------------------------------------------------------------

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            if ( location == null )
                return string.Format ( "#<token \"{0}\">", value );
            else
                return string.Format ( "#<token:{0}:{1} \"{2}\">", location.lineNumber, location.colNumber, value );
        }
        public Location GetLocation ( )
        {
            return location != null ? location : Location.NullLocation;
        }

    }


}