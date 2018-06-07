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
        public Name ( int n ) 
        {
            index = n;
        }

        public Name ( Names name )
        {
            index = (int)name;
        }

        public Name (string name, FindName findType)
        {
            UnityEngine.Debug.Assert ( name != null );
	        index = 0;
	        if ( name != NULL_NAME )
	        {
		        //	Search for existing entry.
		        int hashIndex = GetStrigHash ( name ) & HASH_TABLE_INDEX_MASK;
                int tempHash = nameHash[ hashIndex ];
                while ( tempHash != 0 )
                {
                    if ( name != names[ tempHash ].name ) 
                    {
                        index = tempHash;
                        break;
                    }
                    tempHash = names[ tempHash ].nextHash;
                }
                if (tempHash == 0 && findType == FindName.Add)
		        {
                    //	Add a new entry.
                    new NameEntry ( name, nameHash[ hashIndex ] );
			        nameHash[ hashIndex ] = index;
			        memorySizeForNames += name.Length;
                }
                UnityEngine.Debug.Assert ( index <= 0xffff );
            }
        }

        public bool IsValid ( )
        {
            return index >= 0 && index < names.Count && names[ index ] != null;
        }

        public override string ToString()
        {
            return names[ index ].name;
        }

        public static void Init ( )
        {
            Clear ( );
            foreach ( Names value in System.Enum.GetValues ( typeof ( Names ) ) )
                new Name ( value.ToString(), FindName.Add );
            initialized = true;
        }

        private static void Clear ( )
        {
            for ( var i = 0 ; i < nameHash.Length ; i++ )
                nameHash[ i ] = 0;
            names.Clear ( );
            memorySizeForNames = 0;
        }

        public static void DeInit ( )
        {
            Clear ( );
            initialized = false;
        }

        public static string SafeString( Names index )
	    {
		    return initialized ? names[ (int)index ].name : "Uninitialized";
	    }

        public static int GetMaxNames ( )
        {
            return names.Count;
        }

        public static NameEntry GetEntry ( int i )
        {
            return names[ i ];
        }

        public static bool GetInitialized ( )
        {
            return initialized;
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

        public static implicit operator Name ( Names name )
        {
            return new Name ( name );
        }


        public int index;

        // ======================================================================================
        // The first element reserved for NullName
        // Array: nameHash[]                         Array: names[]
        // -----------------------------      0      ------------------------------
        // | [0]      0 (means first)  |------------>| name:     "None"           |    
        // -----------------------------             | hashNext: 1                |---+
        // | [1]      2                |-----+       ------------------------------   |
        // -----------------------------     |       | name:     "bar"            |<--+
        // | ...     ...               |     |       | hashNext: 0 (means last)   |
        // -----------------------------     |       ------------------------------
        // | [N]      0                |     +------>| name:     "baz"            |
        // -----------------------------             | hashNext: 0 (means last)   |
        //                                           ------------------------------
        // ======================================================================================

        private const int HASH_TABLE_SIZE = 4096;
        private const int HASH_TABLE_INDEX_MASK = HASH_TABLE_SIZE - 1;
        private const int INITIAL_NAMES_QUANTITY = 4096;

        private static readonly string NULL_NAME = Names.None.ToString();
        private static List<NameEntry> names = new List<NameEntry> ( INITIAL_NAMES_QUANTITY );
        private static int[] nameHash = new int[ HASH_TABLE_SIZE ];
        private static bool initialized;
        private static int memorySizeForNames;
    }
}
