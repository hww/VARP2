/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VARP.Utils.String
{

    public static partial class Humanizer
    {     
        /// <summary>
        /// Remove spaces
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string ParseOutSpacesAndSymbols(string me)
        {
            return Regex.Replace(me, "[^A-Za-z0-9]", string.Empty);
        }

        /// <summary>
        /// Alphabetize the characters in the string.
		/// It return all characters used in the string.
        /// </summary>
        public static string Alphabetize(string s)
        {
            // 1.
            // Convert to char array.
            var a = s.ToCharArray();

            // 2.
            // Sort letters.
            Array.Sort(a);

            // 3.
            // Return modified string.
            return new string(a);
        }

        public static string Humanize(string str, bool capitalize = true)
        {
            var output = string.Empty;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (c == '-')
                {
                    capitalize = true;
                    output += ' ';
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    if (i > 0)
                        output += ' ';
                    output += c;
                    capitalize = false;
                }
                else
                {
                    if (capitalize)
                    {
                        output += char.ToUpper(c);
                        capitalize = false;
                    }
                    else
                    {
                        output += c;
                    }
                }
            }
            return output;
        }

        public static string[] Humanize(string[] str, bool capitalize = true)
        {
            var res = new string[str.Length];
            for (var i = 0; i < str.Length; i++)
                res[i] = Humanize( str[ i ], capitalize );
            return res;
        }

        public static string Camelize(string str)
        {
            return Camelize(str, true);
        }

		/// Convert expression: 'foo-bar' to 'FooBar' or 'foobar'
        public static string Camelize(string str, bool capitalize)
        {
            var output = string.Empty;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (c == '-')
                {
                    capitalize = true;
                }
                else
                {
                    if (capitalize)
                    {
                        output += char.ToUpper(c);
                        capitalize = false;
                    }
                    else
                    {
                        output += c;
                    }
                }
            }
            return output;
        }

		/// Convert 'FooBar' to 'foo-bar'
        public static string Decamelize(string str)
        {
            var output = string.Empty;
            var small = false;
            var space = false;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsUpper(c))
                {
                    if (small)
                        output += '-';
                    output += char.ToLower(c);
                    small = false;
                    space = false;
                }
                else if (c == ' ')
                {
                    small = true; // make - if next capital
                    space = true; // make - if nex down
                }
                else
                {
                    if (space)
                        output += '-';
                    output += c;
                    small = true; // make - if next capital
                    space = false; // do not make - if next small
                }
            }
            return output;
        }

        public static string Pluralize(this string singularForm, int howMany)
        {
            return singularForm.Pluralize(howMany, singularForm + "s");
        }

        public static string Pluralize(this string singularForm, int howMany, string pluralForm)
        {
            return howMany == 1 ? singularForm : pluralForm;
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string WordWrap ( string text, int width )
        {
            int pos, next;
            StringBuilder sb = new StringBuilder ( );

            // Lucidity check
            if ( width < 1 )
                return text;

            // Parse each line of text
            for ( pos = 0 ; pos < text.Length ; pos = next )
            {
                // Find end of line
                int eol = text.IndexOf ( Environment.NewLine, pos );
                if ( eol == -1 )
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if ( eol > pos )
                {
                    do
                    {
                        int len = eol - pos;
                        if ( len > width )
                            len = BreakLine ( text, pos, width );
                        sb.Append ( text, pos, len );
                        sb.Append ( Environment.NewLine );

                        // Trim whitespace following break
                        pos += len;
                        while ( pos < eol && Char.IsWhiteSpace ( text[ pos ] ) )
                            pos++;
                    } while ( eol > pos );
                }
                else
                    sb.Append ( Environment.NewLine ); // Empty line
            }
            return sb.ToString ( );
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine ( string text, int pos, int max )
        {
            // Find last whitespace in line
            int i = max;
            while ( i >= 0 && !Char.IsWhiteSpace ( text[ pos + i ] ) )
                i--;

            // If no whitespace found, break at maximum length
            if ( i < 0 )
                return max;

            // Find start of whitespace
            while ( i >= 0 && Char.IsWhiteSpace ( text[ pos + i ] ) )
                i--;

            // Return length of text before whitespace
            return i + 1;
        }

    }
}