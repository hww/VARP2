/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.VM
{
    using Data;
    using STX;
    using DataStructures;
    using AST;

    public class Frame 
    {
        public Environment environment;

        public Frame parent;            //< pointer to parent frame
        public Template template;       //< pointer to template
        public Variant[] Values;          //< register set
        public int FrameNum;            //< frame number in hierarhy
        public int PC;                  //< program counter
        public int SP;                  //< stack pointer

        public Frame(Frame parent, Template template)
        {
            this.parent = parent;
            this.template = template;
            Values = new Variant[ template.FrameSize];
            environment = parent == null ? SystemEnvironment.Top : parent.environment;
            SP = template.SP;
            FrameNum = parent == null ? 0 : parent.FrameNum + 1;
        }
        public Frame(Frame parent, Template template, Environment environment)
        {
            this.parent = parent;
            this.template = template;
            Values = new Variant[ template.FrameSize];
            this.environment = environment;
            SP = template.SP;
        }

        /// <summary>
        /// Return string of current location in the source code
        /// </summary>
        /// <returns></returns>
        public string GetLocationString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Get top frame of the environment
        /// </summary>
        /// <returns></returns>
        public Frame GetTopFrame()
        {
            var curent = this;
            while (curent.parent != null)
                curent = curent.parent;
            return curent;
        }

        /// <summary>
        /// Get top frame of the environment
        /// </summary>
        /// <returns></returns>
        public Frame GetFrame(int idx)
        {
            var curent = this;
            while (curent != null)
            {
                if (idx == 0) return curent;
                curent = curent.parent;
                idx--;
            }
            return null;
        }

        /// <summary>
        /// Return depth of the stack
        /// </summary>
        /// <returns></returns>
        public int GetDept()
        {
            return FrameNum-1;
        }

        /// <summary>
        /// Convert to the array
        /// First element is top
        /// </summary>
        /// <returns></returns>
        public Frame[] ToArray() 
        {
            var array = new Frame[GetDept()];
            var curent = this;
            while (curent != null)
            {
                array[curent.FrameNum] = curent;
                curent = curent.parent;
            }
            return array;
    }

        #region Variables Methods

        /// <summary>
        /// Find index of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal virtual int IndexOfVariable( Name name )
        {
            return template.IndexOfArgument(name);
        }

        /// <summary>
        /// Find index and frame of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal virtual Frame IndexOfVariableRecursively(Name name, ref int frameIdx, ref int varIdx)
        {
            var curframe = this;

            while (curframe != null)
            {
                var curVarIndex = curframe.IndexOfVariable(name);
                if (curVarIndex >=0)
                {
                    varIdx = curVarIndex;
                    frameIdx = FrameNum - curframe.FrameNum;
                    return curframe;
                }
                curframe = curframe.parent;
            }

            return null;
        }

        #endregion
    }

}