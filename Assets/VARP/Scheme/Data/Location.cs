/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Data
{
    using Data;

    /// <summary>
    /// This class is pointer inside source code. It contains
    /// debugging information.
    /// Using class instead of structure let you in future strip
    /// out of runtime debugging information
    /// </summary>
    public sealed class Location : SObject
    {
        public int lineNumber; 
        public int colNumber;
        public int charNumber;
        public string file;

        public Location()
        {
            this.lineNumber = -1; // to disable the location
        }

        public Location(int lineNumber, int colNumber, int charNumber, string file)
        {
            this.lineNumber = lineNumber;
            this.colNumber = colNumber;
            this.charNumber = charNumber;
            this.file = file;
        }

        public Location(Location location)
        {
            lineNumber = location.lineNumber;
            colNumber = location.colNumber;
            charNumber = location.charNumber;
            file = location.file;
        }

        public override string ToString()
        {
            if ( IsValid )
                return string.Format ( "#<location {0}:{1}:{2}>", file, lineNumber, colNumber );
            else
                return string.Empty;
        }

        // Valid location realy reffers to some source code location
        public bool IsValid { get { return lineNumber >= 0; }  }

        public static readonly Location NullLocation = new Location ( 0, 0, 0, null );

    }

    public interface HasLocation
    {
        Location GetLocation ( );
    }
}
