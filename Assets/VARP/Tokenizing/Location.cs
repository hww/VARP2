/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Tokenizing
{
    /// <summary>
    /// This class is pointer inside source code. It contains
    /// debugging information.
    /// Using class instead of structure let you in future strip
    /// out of runtime debugging information
    /// </summary>
    public sealed class Location 
    {
        public int LineNumber; 
        public int ColNumber;
        public int CharNumber;
        public string File;

        public Location()
        {

        }
        public Location(int lineNumber, int colNumber, int charNumber, string file)
        {
            LineNumber = lineNumber;
            ColNumber = colNumber;
            CharNumber = charNumber;
            File = file;
        }

        public Location(Location location)
        {
            LineNumber = location.LineNumber;
            ColNumber = location.ColNumber;
            CharNumber = location.CharNumber;
            File = location.File;
        }
    }
}
