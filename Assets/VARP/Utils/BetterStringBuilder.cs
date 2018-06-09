/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Utils
{
    /// <summary>
    /// Has fixed maximum capacity. Does not produce garbage
    /// after .Clear() method.
    /// </summary>
    public class BetterStringBuilder
    {

        private readonly char[] buffer;
        private int bufferPos;

        private string stringCache;

        public BetterStringBuilder(int capacity)
        {
            buffer = new char[capacity];
        }

        /// <summary>
        /// Append string to the buffer
        /// </summary>
        /// <param name="c">Character to add</param>
        public void Append(char c)
        {
            buffer[bufferPos++] = c;
            stringCache = null;
        }

        /// <summary>
        /// Clear buffer
        /// </summary>
        public void Clear()
        {
            bufferPos = 0;
            stringCache = null;
        }

        /// <summary>
        /// Get type of buffer
        /// </summary>
        public int Size
        {
            get { return bufferPos; }
        }

        /// <summary>
        /// Get string of the buffer
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            if (stringCache != null)
                return stringCache;
            return stringCache = new string(buffer, 0, bufferPos);
        }
    }

}
