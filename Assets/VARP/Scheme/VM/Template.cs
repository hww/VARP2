/* Copyright (c) 2016 Valery Alex P. */

namespace VARP.Scheme.VM
{
    using Data;
    using REPL;
    using STX;
    using System.Text;
    using Tokenizing;

    internal enum VariableType : byte
    {
        Local,
        Global,
        UpValue
    }

    internal struct VariableInfo
    {
        public VariableType Type;           //< up value type
        public Name Name;                   //< variable name
        public short UpVarIndex;            //< index of variable in referenced environment 
        public short UpEnvIdx;              //< index of referenced environment 
        public int LitIdx;                  //< initializer: -1 for required

        public bool IsLocal { get { return Type == VariableType.Local; } }
        public bool IsGlobal { get { return Type == VariableType.Global; } }
        public bool IsUpvalue { get { return Type == VariableType.UpValue; } }

    }

    public sealed class Template : Inspectable
    {
        internal Instruction[] Code;            //< code sequence
        internal VariableInfo[] Variables;      //< local vars info, include required, and optional
        internal Variant[] Literals;            //< list of literals, there will be child templates
        internal Location[] CodeDbg;            //< the location in source code
        internal Template[] Children;           //< nested templates

        internal int ArgsCout;                  //< total argumets count
        internal int ReqArgsNumber;             //< quantity of arguments
        internal int OptArgsNumber;             //< quantity of arguments
        internal int KeyArgsNumber;             //< quantity of arguments
        internal int UpValsNumber;              //< quantity of arguments
        internal int RestArgsNumber;            //< rest arguments number
        internal int RestValueIdx;              //< index of first element
        internal int SP;                        //< stack pointer position
        internal int FrameSize;                 //< full frame size
        internal int ResultIdx;                 //< where is result after this template

        public Template()
        {
            ReqArgsNumber = 0;
            OptArgsNumber = 0;
            KeyArgsNumber = 0;
            UpValsNumber = 0;
            RestValueIdx = -1;
            SP = -1;
        }

        public Template(Variant[] literals, Instruction[] code)
        {
            Literals = literals;
            Code = code;
            ReqArgsNumber = 0;
            OptArgsNumber = 0;
            KeyArgsNumber = 0;
            UpValsNumber = 0;
            RestValueIdx = -1;
            SP = -1;
        }

        public string Inspect( InspectOptions options = InspectOptions.Default )
        {
            return Inspect(0);
        }

        public string Inspect(int ident)
        {
            var sident = new string(' ', ident * 4);

            var sb = new StringBuilder();
            sb.Append(sident);
            sb.AppendFormat("Template: args: {0} frame: {1}\n", ArgsCout, FrameSize);
            /////////////////
            /// arguments ///
            /////////////////
            sb.Append(sident);
            sb.Append("|  arguments:");
            foreach (var v in Variables)
            {
                if (v.IsLocal)
                {
                    sb.Append(" ");
                    sb.Append(v.Name.ToString());
                    switch (v.LitIdx)
                    {
                        case -1:
                            break;
                        case -2:
                            sb.Append(":#f");
                            break;
                        default:
                            sb.Append(":");
                            sb.Append(v.LitIdx.ToString());
                            break;
                    }
                }
            }
            /////////////
            /// &rest ///
            /////////////
            if (RestValueIdx >= 0)
            {
                sb.Append(" &rest: ");
                sb.Append(Variables[RestValueIdx].Name.ToString());
            }
            sb.AppendLine();
            ///////////////
            /// &upvals ///
            ///////////////
            sb.Append(sident);
            sb.Append("|  upvalues:");
            foreach (var v in Variables)
            {
                if (v.IsGlobal)
                {
                    sb.Append(" ");
                    sb.Append(v.Name.ToString());
                }
                else if (v.IsUpvalue)
                {
                    sb.Append(" ");
                    sb.Append(v.Name.ToString());
                    sb.Append(":");
                    sb.Append(v.UpEnvIdx);
                    sb.Append(":");
                    sb.Append(v.UpVarIndex);
                }
            }
            sb.AppendLine();
            ///////////
            /// &sp ///
            ///////////
            sb.Append(sident);
            sb.Append("|  temp:");
            for (var i = SP; i < FrameSize; i++)
                sb.AppendFormat(" T{0}", i);
            sb.AppendLine();
            ////////////
            /// code ///
            ////////////
            sb.Append(sident);
            sb.AppendLine("|  code:");
            var pc = 0;
            foreach (var v in Code)
            {
                sb.Append(sident);
                sb.Append(string.Format("|  [{0}] ", pc));
                sb.Append(Code[pc++].ToString());
                sb.AppendLine();
            }

            if (Literals.Length > 0)
            {
                sb.Append(sident);
                sb.Append("|  literals:");
                sb.AppendLine();
                var lident = new string(' ', (ident + 1) * 4);
                var lidx = 0;
                foreach (var v in Literals)
                {
                    sb.Append(lident);
                    sb.Append(string.Format("  [{0}] ", lidx++));
                    sb.Append(v.ToString());
                    sb.AppendLine();
                }
            }

            if ( Children.Length > 0)
            {
                sb.Append ( sident );
                sb.Append ( "|  chidlren:" );
                sb.AppendLine ( );
                var lident = new string ( ' ', ( ident + 1 ) * 4 );
                var lidx = 0;
                foreach ( var v in Children )
                {
                    sb.Append ( lident );

                    sb.AppendLine ( string.Format ( "  [{0}] -->", lidx++ ) );
                    sb.Append ( v.Inspect ( ident + 2 ) );

                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Find index of the argument
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int IndexOfArgument(Name name)
        {
            for (var i = 0; i < Variables.Length; i++)
            {
                if (Variables[i].Name == name)
                    return i;
            }
            return -1;
        }


        internal VariableInfo GetVariable(int idx)
        {
            return Variables[idx];
        }

        internal Location GetCodeDbg(int idx)
        {
            if (CodeDbg == null) return null;
            return CodeDbg[idx];
        }

        internal void GetUpValue(int idx, ref int envIdx, ref int varIdx)
        {
            envIdx = Variables[idx].UpEnvIdx;
            varIdx = Variables[idx].UpVarIndex;
        }
    }
}