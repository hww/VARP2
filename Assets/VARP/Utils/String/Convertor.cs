using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace VARP.Utils.String
{
    public static partial class Convertor
    {
        /// <summary>
        /// Convert string to byte-array
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(this string me)
        {
            return new ASCIIEncoding().GetBytes(me);
        }
    }
}