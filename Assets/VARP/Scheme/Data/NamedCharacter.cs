/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Collections.Generic;

namespace VARP.Scheme.Data
{ 
    /// <summary>
    /// Represents a single character string.
    /// </summary>
    /// <remarks>
    /// These strings may contain nulls and are not null-terminated.
    /// </remarks>
    [Serializable]
    public static class NamedCharacter 
    {
        // TODO
        // Key Bucky bit prefix        Bucky bit
        // ---      ----------------    ---------
        // 
        // Meta     M- or Meta-                 1
        // Control  C- or Control-              2
        // Super    S- or Super-                4
        // Hyper    H- or Hyper-                8
        // Top      T- or Top-                 16
        // For example,
        // 
        // #\c-a                   ; Control-a
        // #\meta-b                ; Meta-b
        // #\c-s-m-h-a             ; Control-Meta-Super-Hyper-A

        private static void DeineCharacter(int character, string name)
        {
            stringToCharMap.Add(name, (char)character);
            charToStringMap[character] = name;
        }

        // -- Converttion --------------------------------------------------------------------------------

        public static bool NameToCharacter(string named, out char character)
        {
            return stringToCharMap.TryGetValue(named, out character);
        }

        public static string CharacterToName(char value)
        {
            if (value < charToStringMap.Length && charToStringMap[value] != null)
                return charToStringMap[value];

            return "#\\" + value.ToString();
        }

        // -- Static Constructor -------------------------------------------------------------------------

        static NamedCharacter ( )
        {
            stringToCharMap = new Dictionary<string, char> ( );
            DeineCharacter ( ' ', "#\\space" );
            DeineCharacter ( '\n', "#\\newline" );
            DeineCharacter ( 0, "#\\eof" );
        }

        // -- Static Fields ------------------------------------------------------------------------------

        private static Dictionary<string, char> stringToCharMap;
        private static string[] charToStringMap = new string[ 256 ];


    }
}
