/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public static class EnumExtensions
{
    /// <summary>
    /// Check if given type is the enum with [Flag] attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="withFlags"></param>
    private static void CheckIsEnum<T>(bool withFlags)
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
        if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
    }
    /// <summary>
    /// Test value if it has given flag enabled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool IsFlagSet<T>(this T value, T flag) where T : struct
    {
#if CHECK_FLAG_ATTRIBUTE
        CheckIsEnum<T>(true);
#endif
        long v = Convert.ToInt64(value);
        long f = Convert.ToInt64(flag);
        return (v & f) != 0;
    }
    /// <summary>
    /// Set given flag to the requested state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public static T SetFlags<T>(this T value, T flags, bool state) where T : struct
    {
#if CHECK_FLAG_ATTRIBUTE
        CheckIsEnum<T>(true);
#endif
        long v = Convert.ToInt64(value);
        long f = Convert.ToInt64(flags);
        if (state)
            v |= f;
        else
            v &= (~f);
        return (T)Enum.ToObject(typeof(T), v);
    }
    /// <summary>
    /// Set the flag
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static T SetFlags<T>(this T value, T flags) where T : struct
    {
        return value.SetFlags(flags, true);
    }
    /// <summary>
    /// Reset the flag
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static T ClearFlags<T>(this T value, T flags) where T : struct
    {
        return value.SetFlags(flags, false);
    }
    /// <summary>
    /// Iterate over all enabled flags in the value
    /// </summary>
    /// <example>
    /// foreach (var flag in enumValue)
    ///     Debug.LogFormat("Enabled flag: {0}", (long)flag);
    /// </example>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
    {
#if CHECK_FLAG_ATTRIBUTE
        CheckIsEnum<T>(true);
#endif
        foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
        {
            if (value.IsFlagSet(flag))
                yield return flag;
        }
    }
    /// <summary>
    /// Combine list of flags by logic OR and return the result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
    {
#if CHECK_FLAG_ATTRIBUTE
        CheckIsEnum<T>(true);
#endif
        long v = 0;
        foreach (T flag in flags)
        {
            long f = Convert.ToInt64(flag);
            v |= f;
        }
        return (T)Enum.ToObject(typeof(T), v);
    }
    /// <summary>
    /// Get description of the 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription<T>(this T value) where T : struct
    {
#if CHECK_FLAG_ATTRIBUTE
        CheckIsEnum<T>(false);
#endif
        string name = Enum.GetName(typeof(T), value);
        if (name != null)
        {
            FieldInfo field = typeof(T).GetField(name);
            if (field != null)
            {
                DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null)
                    return attr.Description;
            }
        }
        return null;
    }
}