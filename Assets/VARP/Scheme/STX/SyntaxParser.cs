/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Collections.Generic;

namespace VARP.Scheme.STX
{
    using Tokenizing;
    using Data;
    using DataStructures;
    using Exceptions;

    /// <summary>
    /// Class that reads symbols from a Tokenizer and turns them into an object
    /// </summary>
    public sealed class SyntaxParser 
    {

        /// <summary>
        /// Parses a scheme expression in the default manner
        /// </summary>
        /// <returns>A scheme object</returns>
        /// <remarks>It is an error to pass scheme to this method with 'extraneous' tokens, such as trailing closing brackets</remarks>
        public static Syntax Parse(string scheme, string filepath)
        {
            var reader = new Tokenizer(new System.IO.StringReader(scheme), filepath);

            var res = Parse(reader);
            var token = reader.ReadToken();
            if (token != null) throw ParserError.SyntaxError("parser", "found extra tokens after the end of a scheme expression", token);

            return res;
        }

        /// <summary>
        /// Turns the contents of a Tokenizer into an object
        /// </summary>
        /// <remarks>
        /// Recursive-descent via ParseToken. There may be tokens left in the reader.
        /// </remarks>
        public static Syntax Parse(Tokenizer reader)
        {
            var firstToken = reader.ReadToken();
            if (firstToken == null) return null;
            return ParseToken(firstToken, reader);
        }

        /// <summary>
        /// Turns thisToken into an object, using moreTokens to get further tokens if required
        /// </summary>
        /// <returns></returns>
        private static Syntax ParseToken(Token thisToken, Tokenizer moreTokens)
        {
            if (thisToken == null)
                throw ParserError.SyntaxError("parser", "unexpectedly reached the end of the input", moreTokens.LastToken);

            switch (thisToken.type)
            {
                //case TokenType.Dot:
                //    return thisToken; //TODO maybe exception or symbol .

                case TokenType.Character:
                    return Syntax.Create( thisToken.GetCharacter(), thisToken.location );

                case TokenType.Boolean:
                    return Syntax.Create ( thisToken.GetBool(), thisToken.location );

                case TokenType.String:
                    return Syntax.Create ( thisToken.GetString(), thisToken.location );

                case TokenType.Symbol:
                    return Syntax.Create ( thisToken.GetName(), thisToken.location );

                case TokenType.Heximal:
                case TokenType.Integer:
                    return Syntax.Create ( thisToken.GetInteger(), thisToken.location );

                case TokenType.Floating:
                    return Syntax.Create ( thisToken.GetFloat(), thisToken.location );

                case TokenType.OpenBracket:
                    return ParseList(thisToken, moreTokens);

                case TokenType.OpenVector:
                    return ParseVector(thisToken, moreTokens);

                case TokenType.Quote:
                case TokenType.Unquote:
                case TokenType.UnquoteSplicing:
                case TokenType.QuasiQuote:
                    return ParseQuoted(thisToken, moreTokens);

                case TokenType.BadNumber:
                    throw ParserError.SyntaxError("parser", "looks like it should be a number, but it contains a syntax error", thisToken);

                default:
                    // Unknown token type
                    throw ParserError.SyntaxError("parser", "the element is being used in a context where it is not understood", thisToken);
            }
        }

        public static Syntax ParseQuoted(Token thisToken, Tokenizer moreTokens)
        {
            EName quote = EName.None;

            // First symbol is quote, unquote, quasiquote depending on what the token was
            switch (thisToken.type)
            {
                case TokenType.Quote: quote = EName.Quote; break;
                case TokenType.Unquote: quote = EName.Unquote; break;
                case TokenType.QuasiQuote: quote = EName.Quasiquote; break;
                case TokenType.UnquoteSplicing: quote = EName.UnquoteSplicing; break;
            }
            var quote_stx = Syntax.Create((Name)quote, thisToken.location);
            var nextToken = moreTokens.ReadToken();
            var quoted = ParseToken(nextToken, moreTokens);
            var list = new LinkedList<Syntax>();
            list.AddLast(quote_stx);
            list.AddLast(quoted);
            return Syntax.Create( list, thisToken.location);
        }

        private static Syntax ParseDot(Token thisToken, Tokenizer moreTokens)
        {
            return null;
        }

        private static Syntax ParseList(Token thisToken, Tokenizer moreTokens)
        {
            // Is a list/vector
            var listContents = new LinkedList<Syntax>();
            Token dotToken = null;

            var nextToken = moreTokens.ReadToken();
            while (nextToken != null && nextToken.type != TokenType.CloseBracket)
            {
                // Parse this token
                listContents.AddLast(ParseToken(nextToken, moreTokens));

                // Fetch the next token
                nextToken = moreTokens.ReadToken();
                if (nextToken == null)
                    throw ParserError.SyntaxError("parser", "Improperly formed list.", dotToken);

                if (nextToken.type == TokenType.Dot)
                {
                    if (dotToken != null || thisToken.type != TokenType.OpenBracket)
                        throw ParserError.SyntaxError("parser", "Improperly formed dotted list", nextToken);
                    dotToken = nextToken;
                    nextToken = moreTokens.ReadToken();
                    if (nextToken == null)
                        throw ParserError.SyntaxError("parser", "Improperly formed dotted list", dotToken);
                    if (nextToken.type == TokenType.CloseBracket)
                        throw ParserError.SyntaxError("parser", "Improperly formed dotted list", dotToken);
                    listContents.AddLast(ParseToken(nextToken, moreTokens));
                    nextToken = moreTokens.ReadToken();
                    if (nextToken.type != TokenType.CloseBracket)
                        throw ParserError.SyntaxError("parser", "Improperly formed dotted list", dotToken);
                    break;
                }
            }

            if (nextToken == null)
            {
                // Missing ')'
                throw ParserError.SyntaxError("parser", "missing close parenthesis", thisToken);
            }

            if (dotToken != null)
            {
                if (listContents.Count == 2)
                    return Syntax.Create(new Pair(listContents[0], listContents[1]), thisToken.location);
                else
                    throw ParserError.SyntaxError("parser", "improper dot syntax", thisToken);
            }
            else
            {
                if (listContents.Count == 0)
                    return Syntax.Create (null as LinkedList<Syntax>, thisToken.location );
                else
                    return Syntax.Create ( listContents, thisToken.location);
            }

        }

        private static Syntax ParseVector(Token thisToken, Tokenizer moreTokens)
        {
            var listContents = new List<Syntax>();
            Token dotToken = null;

            var nextToken = moreTokens.ReadToken();
            while (nextToken != null && nextToken.type != TokenType.CloseBracket)
            {
                // Parse this token
                listContents.Add(ParseToken(nextToken, moreTokens));

                // Fetch the next token
                nextToken = moreTokens.ReadToken();
                if (nextToken == null)
                    throw ParserError.SyntaxError("parser", "Improperly formed list.", dotToken);

                //if (!improper && nextToken.Type == TokenType.Symbol && dotSymbol.Equals(nextToken.Value) && thisToken.Type == TokenType.OpenBracket)
                if (nextToken.type == TokenType.Dot)
                    throw ParserError.SyntaxError("parser", "Improperly formed dotted list", nextToken);
            }

            if (nextToken == null) // Missing ')'
                throw ParserError.SyntaxError("parser", "Missing close parenthesis", thisToken);

            return Syntax.Create(listContents, thisToken.location );
        }

        /// <summary>
        /// Works out how many brackets are missing for the expression given by the Tokenizer
        /// </summary>
        /// <param name="reader">The reader to read expressions from</param>
        /// <returns>The number of closing parenthesizes that are required to complete the expression (-1 if there are too many)</returns>
        public static int RemainingBrackets(Tokenizer reader)
        {
            var bracketCount = 0;
            Token thisToken;

            try
            {
                thisToken = reader.ReadToken();
            }
            catch ( ParserError )
            {
                thisToken = new Token(TokenType.BadSyntax, "", null);
            }
            catch (ArithmeticException)
            {
                thisToken = new Token(TokenType.BadNumber, "", null);
            }

            while (thisToken != null)
            {
                switch (thisToken.type)
                {
                    case TokenType.OpenBracket:
                    case TokenType.OpenVector:
                        // If this begins a list or a vector, increase the bracket count
                        bracketCount++;
                        break;

                    case TokenType.CloseBracket:
                        // Close brackets indicate the end of a list or vector
                        bracketCount--;
                        break;
                }

                // Get the next token
                thisToken = reader.ReadToken();
            }

            // Set the count to -1 if there were too many brackets
            if (bracketCount < 0) bracketCount = -1;

            return bracketCount;
        }
    }
}

