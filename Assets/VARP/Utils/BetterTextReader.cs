/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Collections.Generic;
using System.IO;

namespace VARP.Utils
{
    /// <summary>
    /// Allow to make peek with offset index
    /// EXAMPLE:
    /// char c = betterReader.PeekAt(2); 
    /// </summary>
    internal class BetterTextReader
    {
        private readonly TextReader reader;
        private readonly List<int> queue = new List<int>();

        public BetterTextReader(TextReader reader)
        {
            this.reader = reader;
        }
        /// <summary>
        /// Close stream
        /// </summary>
        public void Close()
        {
            reader.Close();
        }
        /// <summary>
        /// Peek symbol with offset
        /// Do not change pointer in stream
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int PeekAt(int offset)
        {
            while (queue.Count < (offset + 1))
                queue.Add(reader.Read());
            return queue[offset];
        }
        /// <summary>
        /// Read symbol from stream 
        /// and increment current pointer
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            if (queue.Count > 0)
            {
                var item = queue[0];
                queue.RemoveAt(0);
                return item;
            }
            else return reader.Read();
        }
    }
}