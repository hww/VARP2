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

        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format ( "#<location:{0}:{1} {2}>", lineNumber, colNumber, file );
        }

        // -- Makes string in format: "file-path(lineNumber, ColNumber):"
        public string GetLocationString ( )
        {
            return file==null ? string.Empty : string.Format ( "{0}({1},{2}): ", file, lineNumber, colNumber );
        }

        public string GetLocationStringShort ( )
        {
            return file == null ? string.Empty : string.Format ( ":{0}:{1}", lineNumber, colNumber );
        }

        public static readonly Location NullLocation = new Location ( 0, 0, 0, null );
    }
}
