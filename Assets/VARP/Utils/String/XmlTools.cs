/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;

namespace VARP.Utils.String
{
    public static partial class XmlTools
    {
        public static string WrapColor(string s, Color c)
        {
            return ColorTagOpen(c) + s + ColorTagClose();
        }

        public static string ColorTagOpen(Color c)
        {
            return "<color=#" + ColorToHex(c) + ">";
        }

        public static string ColorTagClose()
        {
            return "</color>";
        }

        public static string ColorToHex(Color32 color)
        {
            var hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
    }
}