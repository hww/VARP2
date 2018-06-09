/*  Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Collections.Generic;

namespace VARP.Scheme.Data
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

    public struct Name : Inspectable
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

        public bool IsValid
        {
            get { return index >= 0 && index < Names.Count; }
        }

        public bool IsKeyword 
        {
            get { return IsValid && Names[ index ].name[ 0 ] == ':'; }
        }

        public bool IsIdentifier 
        {
            get { return IsValid && Names[ index ].name[ 0 ] != ':'; }
        }

        // -- Cast to other type  --------------------------------------------------------------

        public override string ToString ( )
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

        public static implicit operator Name ( EName name )
        {
            return new Name(name);
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
        public static bool operator == ( Name x, Name y )
        {
            return x.index == y.index;
        }
        public static bool operator != ( Name x, Name y )
        {
            return x.index != y.index;
        }

        // -- Fields -----------------------------------------------------------------------------

        public string Inspect ( InspectOptions options = InspectOptions.PrettyPrint)
        {
            if ( options == InspectOptions.PrettyPrint )
                return SpecialForm.ToSpecialFormString ( this ) ;
            else
                return ToString ( );
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

        public static bool IsInitialized 
        {
            get { return Initialized; }
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
