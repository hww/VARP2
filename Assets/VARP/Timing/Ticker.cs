﻿/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Diagnostics;

namespace VARP.Timing
{

    using DataStructures;

    public class FTicker
    {
        #region Singletone

        private static FTicker coreTicker;
        public static FTicker GetCoreTicker()
        {
            if (coreTicker == null) coreTicker = new FTicker();
            return coreTicker;
        }

        #endregion

        // @return true if have to be fired again or false to terminate
        // @deltaTime 
        public delegate bool FTickerDelegate(float deltaTime);

        // Single delegate item
        public class FElement
        {
            public LinkedListNode<FElement> link;
            // Time that this delegate must not fire before
            public double fireTime;
            // Delay that this delegate was scheduled with. Kept here so that if the delegate returns true, we will reschedule it.
            public float delayTime;
            // Delegate to call
            private FTickerDelegate theDelegate;

            // This is the ctor that the code will generally use. 
            public FElement(double inFireTime, float inDelayTime, FTickerDelegate inDelegate, object inDelegateHandle = null)
            {
                delayTime = inDelayTime;
                fireTime = inDelayTime;
                theDelegate = inDelegate;
            }

            // Invoke the delegate if possible 
            public bool Tick(float deltaTime)
            {
                if (theDelegate == null) return false;
                if (theDelegate(deltaTime)) return true;
                theDelegate = null; // terminate
                return false;
            }

            public bool Equals(FTickerDelegate inDelegate)
            {
                return theDelegate == inDelegate;
            }

            public bool EqualsHandle(int inHandle)
            {
                return GetHashCode() == inHandle;
            }

            public void Terminate()
            {
                theDelegate = null;
            }

            public bool IsTerminated { get { return theDelegate == null; } }
        };
        // @inDelegae the function will be fired
        // @inDelay delay before fire
        // @handle can be used later to find this delegate
        public int AddTicker(FTickerDelegate inDelegate, float inDelay)
        {
            var e = new FElement(currentTime + inDelay, inDelay, inDelegate);
            elements.AddFirst(e);
            return e.GetHashCode();
        }

        public void RemoveTicker(int inHandle)
        {
            foreach (var el in elements)
            {
                if (el.EqualsHandle(inHandle))
                    el.Terminate();
            }
        }
        public void RemoveTicker(FTickerDelegate inDelegate)
        {
            foreach (var el in elements)
                if (el.Equals(inDelegate))
                    el.Terminate();

        }

        public void Tick(float deltaTime)
        {
            lock (lockObject)
            {
                // Do not call it more that once per frame
                if (oncePerFrame.IsNotOnce) return;
                // Benchmarking
                var timer = Stopwatch.StartNew();
                isInTick = true;
                currentTime += deltaTime;

                var element = elements.First;
                while (element != null)
                {
                    // just in case deleting the element, check who is next
                    var next = element.Next;
                    // optionally: set current element for some of side effect tests
                    currentElement = element.Value;
                    // Tick
                    if (currentElement.Tick(deltaTime))
                        currentElement.fireTime = currentTime + currentElement.delayTime;
                    else
                        currentElement.link.Remove();
                    element = next;
                }
                // Benchmarking end
                timer.Stop();
                totalTimeMicroseconds = timer.ElapsedMilliseconds;
            }
        }

        // --------------------------------------------------------------------

        private object lockObject;              //< Lock object
        private OncePerFrame oncePerFrame;      //< Last frame count (prevent call twice in frame)
        private double currentTime;             //< Current time of the ticker
        private bool isInTick;                  //< State to track whether CurrentElement is valid. 
        private FElement currentElement;        //< Current element being ticked (only valid during tick).
        private long totalTimeMicroseconds;     //< Time of single invoke. Used for benchmarking
        // List of delegates
        private readonly LinkedList<FElement> elements = new LinkedList<FElement>();
    }

    // The base class for objects which have to be called time to time.
    public class FTickerObjectBase
    {
        // Constructor
        //
        // @param InDelay Delay until next fire; 0 means "next frame"
        // @param Ticker the ticker to register with. Defaults to FTicker::GetCoreTicker().
        public FTickerObjectBase(float InDelay = 0.0f, FTicker inTicker = null)
        {
            ticker = inTicker ?? FTicker.GetCoreTicker();
            tickHandle = ticker.AddTicker(Tick, InDelay);
        }

        /** Virtual destructor. */
        ~FTickerObjectBase()
        {
            if (ticker != null) ticker.RemoveTicker(tickHandle);
            ticker = null;
        }

        // Pure virtual that must be overloaded by the inheriting class.
        //
        // @param DeltaTime	time passed since the last call.
        // @return true if should continue ticking
        protected virtual bool Tick(float DeltaTime) { return false; }


        // Ticker to register with 
        private FTicker ticker;
        // Delegate for callbacks to Tick
        private readonly int tickHandle;
    };
}
