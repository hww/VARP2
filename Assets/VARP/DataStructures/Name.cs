/* 
 * Copyright (c) 2016 Valery Alex P.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Collections.Generic;

namespace VARP.DataStructures
{
    public enum FindName
    {
        Find,
        Add
    }

    public class NameEntry
    {
        public int nextHash;
        public string name;

        public NameEntry (string name, int nextHash)
        {
            this.name = name;
            this.nextHash = nextHash;
        }
    }

    public struct Name
    {
        // -- Constructors ------------------------------------------------------------------------

        public Name ( int n ) 
        {
            index = n;
        }

        public Name ( EName name )
        {
            index = (int)name;
        }

        public Name ( Name other )
        {
            index = other.index;
        }

        public Name (string name, FindName findType = FindName.Add)
        {
            UnityEngine.Debug.Assert ( name != null );
            UnityEngine.Debug.Assert ( Initialized );
            index = 0;
	        if ( name != NULL_NAME )
	        {
		        //	Search for existing entry.
		        int hashIndex = GetStrigHash ( name ) & HASH_TABLE_INDEX_MASK;
                int tempHash = NamesHash[ hashIndex ];
                while ( tempHash != 0 )
                {
                    if ( name == Names[ tempHash ].name ) 
                    {
                        index = tempHash;
                        break;
                    }
                    tempHash = Names[ tempHash ].nextHash;
                }
                if (tempHash == 0 && findType == FindName.Add)
		        {
                    //	Add a new entry.
                    index = Names.Count;
                    Names.Add(new NameEntry ( name, NamesHash[ hashIndex ] ));
			        NamesHash[ hashIndex ] = index;
			        MemorySizeForNames += name.Length;
                }
                UnityEngine.Debug.Assert ( index <= 0xffff );
            }
        }

        // -- Constructors ------------------------------------------------------------------------

        public bool IsValid ( )
        {
            return index >= 0 && index < Names.Count && Names[ index ] != null;
        }

        // -- Cast to other type  --------------------------------------------------------------

        public override string ToString()
        {
            if (Initialized)
                return index < Names.Count ? Names[ index ].name : "NotValid";
            else
                return "NotInitialized";
        }

        // explicit conversion (Variant)true
        public static explicit operator string ( Name name )
        {
            return name.ToString();
        }

        // -- Comparison ------------------------------------------------------------------------

        public override bool Equals ( object obj )
        {
            if ( obj == null || GetType ( ) != obj.GetType ( ) )
                return false;

            return index == ((Name)obj).index ;
        }

        public override int GetHashCode ( )
        {
            return index;
        }

        // -- Fields -----------------------------------------------------------------------------

        public int index;

        // -- Static methods ---------------------------------------------------------------------

        public static void Init ( )
        {
            Clear ( );
            foreach ( EName name in System.Enum.GetValues ( typeof ( EName ) ) )
            {
                var namestring = name.ToString ( );
                var nameindex = (int)name;
                int hashIndex = GetStrigHash ( name.ToString ( ) ) & HASH_TABLE_INDEX_MASK;
                Names.Add ( new NameEntry ( namestring, NamesHash[ hashIndex ] ) );
                NamesHash[ hashIndex ] = nameindex;
                MemorySizeForNames += namestring.Length;
            }
            Initialized = true;
        }

        private static void Clear ( )
        {
            for ( var i = 0 ; i < NamesHash.Length ; i++ )
                NamesHash[ i ] = 0;
            Names.Clear ( );
            MemorySizeForNames = 0;
        }

        public static void DeInit ( )
        {
            Clear ( );
            Initialized = false;
        }

        public static int GetNamesCount ( )
        {
            return Names.Count;
        }

        public static NameEntry GetEntry ( int i )
        {
            return Names[ i ];
        }

        public static bool GetInitialized ( )
        {
            return Initialized;
        }

        public static int GetStrigHash ( string str )
        {
            var result = 0;
            int sh = 0;
            for ( var i = 0 ; i < str.Length ; i++ )
            {
                result ^= ( str[ i ] & 0xff ) << sh;
                sh = ( sh + 1 ) & 3;
            }
            return result;
        }

        // -- Static factory ---------------------------------------------------------------------

        public static Name Intern ( string name )
        {
            return new Name ( name, FindName.Add );
        }

        // -- Static fields  ---------------------------------------------------------------------

        private const int HASH_TABLE_SIZE = 4096;
        private const int HASH_TABLE_INDEX_MASK = HASH_TABLE_SIZE - 1;
        private const int INITIAL_NAMES_QUANTITY = 4096;

        /* 
          * The first element reserved for NullName 
          *
          * Array: nameHash[]                         Array: names[]
          * -----------------------------      0      ------------------------------
          * | [0]      0 (means first)  |------------>| name:     "None"           |    
          * -----------------------------             | hashNext: 1                |---+
          * | [1]      2                |-----+       ------------------------------   |
          * -----------------------------     |       | name:     "bar"            |<--+
          * | ...     ...               |     |       | hashNext: 0 (means last)   |
          * -----------------------------     |       ------------------------------
          * | [N]      0                |     +------>| name:     "baz"            |
          * -----------------------------             | hashNext: 0 (means last)   |
          *                                          ------------------------------
          */

        private static readonly string NULL_NAME = EName.None.ToString();
        private static List<NameEntry> Names = new List<NameEntry> ( INITIAL_NAMES_QUANTITY );
        private static int[] NamesHash = new int[ HASH_TABLE_SIZE ];
        private static bool Initialized;
        private static int MemorySizeForNames;
    }
}
