/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Linq.Expressions;
using System;


namespace VARP.Utils
{

    public static partial class Debug
    {
        /// <summary>
        /// gets variable name as a string, good for saving data by name Example use ' StringExtensions.GetVariableName(() => myVar) '
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetVariableName<T>(Expression<Func<T>> expr)
        {
            var body = ((MemberExpression)expr.Body);
            return body.Member.Name;
        }
    }
}
