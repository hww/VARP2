/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Globalization;

namespace VARP.Scheme.Tokenizing
{
    using Exceptions;

    public static partial class StringParser
    {
        public static int GetInteger(string value)
        {
            try
            {
                var val = int.Parse(value, NumberStyles.AllowLeadingSign);
                return val;
            }
            catch (System.Exception ex)
            {
                throw TokenizerError.ErrorWithName ( "get-integer", "improperly formed int value", value);
            }
        }

        public static int GetHexadecimal(string value)
        {
            try
            {
                UnityEngine.Debug.Assert(value.Length > 2, "Error in hex literal");
                var hval = int.Parse(value.Substring(2), NumberStyles.AllowHexSpecifier);
                return hval;
            }
            catch (System.Exception ex)
            {
                throw TokenizerError.ErrorWithName("get-hexadecimal", "improperly formed int value", value);
            }
        }

        public static float GetFloat(string value)
        {
            float val = 0;

            if (float.TryParse(value, out val))
                return val;

            throw TokenizerError.ErrorWithName("get-float", "improperly formed float value", value);
        }

        public static double GetDouble(string value)
        {
            double val = 0;

            if (double.TryParse(value, out val))
                return val;

            throw TokenizerError.ErrorWithName("get-double", "improperly formed float value", value);
        }

        /// <summary>
        /// Return double value from any type of string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetNumerical(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw TokenizerError.ErrorWithName("get-double", "unexpected empty string", value);
            if (char.IsDigit(value[0]))
                return GetDouble(value);

            if (value[0] == '#')
                return GetHexadecimal(value);

            throw TokenizerError.ErrorWithName("get-numerical", "improperly formed numerical value", value);
        }

    }
}
