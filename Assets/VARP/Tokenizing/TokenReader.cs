/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.IO;

namespace VARP.Tokenizing
{
    public sealed class TokenReader
    {
        /// <summary>
        /// Constructor that can be used by subclasses that don't want to provide tokens via a TextReader
        /// </summary>
        protected TokenReader() { }

        /// <summary>
        /// Constructs a token stream that reads token from the given TextReader
        /// </summary>
        /// <param name="textStream">The stream to read tokens from</param>
        public TokenReader(TextReader textStream, string filePath)
        {
            stream = textStream;
            this.filePath = filePath;

            charNumber = 0;
            lineNumber = 1;
            colNumber = 0;
        }

        private TextReader stream;

        #region Tracking where we are in the stream

        private const int contextLen = 8;

        private int charNumber;
        private int lineNumber;
        private int colNumber;
        private string filePath;

        /// <summary>
        /// Peeks at the next character from the stream
        /// </summary>
        public int Peek()
        {
            return stream.Peek();
        }

        /// <summary>
        /// Reads the next character from the stream
        /// </summary>
        public int Read()
        {
            var chr = stream.Read();

            if (chr != -1)
            {
                charNumber++;
                colNumber++;
            }

            if (chr == '\n')
            {
                lineNumber++;
                colNumber = 0;
            }

            return chr;
        }

        public string FilePath { get { return filePath; } }
        public int CharNumber { get { return charNumber; } }
        public int LineNumber { get { return lineNumber; } }
        public int ColNumber { get { return colNumber; } }


        #endregion
    }
}