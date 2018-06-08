/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Timing
{
    internal class TimeSpan
    {
        public static float Hours(float time) { return time * 3600f; }
        public static float Minutes(float time) { return time * 60f; }
        public static float Seconds(float time) { return time; }
        public static float Milliseconds(float time) { return time * 1000f; }
    }
}
