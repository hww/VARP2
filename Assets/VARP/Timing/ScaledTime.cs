/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEngine;

namespace VARP.Timing
{
    /// <summary>
    /// Scaled Time
    /// 
    /// Allow to read @time and @deltaTime but those values depends on
    /// @timeScale property and @enabled flag.
    /// </summary>
    public class FScaledTime : FTickerObjectBase
    {
        private bool enabled;
        private float deltaTime;
        private float timeScale;
        private float time;

        // Constructors.
        public FScaledTime(FTicker inTicker = null) : base(0, inTicker)
        {
            enabled = true;
            TimeScale = 1f;
        }
        public FScaledTime(bool inEnabled, FTicker inTicker = null) : base(0, inTicker)
        {
            enabled = inEnabled;
            TimeScale = 1f;
        }
        public FScaledTime(float inTimeScale, FTicker inTicker = null) : base(0, inTicker)
        {
            enabled = true;
            TimeScale = inTimeScale;
        }
        public FScaledTime(bool inEnabled, float inTimeScale, FTicker inTicker = null) : base(0, inTicker)
        {
            enabled = inEnabled;
            TimeScale = inTimeScale;
        }

        // Methods.
        public float DetaTime { get { return deltaTime; } }
        public float Time { get { return time; } }
        public float TimeScale { get { return timeScale; } set { timeScale = value; } }
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        // Private stuff
        protected override bool Tick(float deltaTime)
        {
            this.deltaTime = Enabled ? UnityEngine.Time.deltaTime * timeScale : 0;
            time += this.deltaTime;
            return true;
        }
    }
}
