/* Copyright (c) 2016 Valery Alex P. All rights reserved. */
 
using UnityEngine;

namespace VARP.Timing
{
    /// <summary>
    /// The guard for calling function only once per frame
    /// </summary>
    /// <example>
    ///     OncePerFrame tick;
    ///     bool Tick()
    ///     {
    ///         if (tick.IsFirst())
    ///         {
    ///             ... 
    ///         }
    ///     }
    /// </example>
    public struct OncePerFrame
    {
        private int frameCount;
        public OncePerFrame(int inFrameCount = -1)
        {
            frameCount = inFrameCount;
        }

        public bool IsOnce
        {
            get
            {
                if (frameCount == Time.frameCount) return true;
                frameCount = Time.frameCount;
                return false;
            }
        }

        public bool IsNotOnce { get { return !IsOnce; } }
    }
}
