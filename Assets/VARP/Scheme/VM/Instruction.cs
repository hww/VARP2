﻿/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Diagnostics;
using System.Text;

namespace VARP.Scheme.VM
{
    using Exceptions;

    public enum OpCode
    {
        NOP,
        MOVE,       //<    A B      R(A) := R(B)
        LOADK,      //<    A Bx     R(A) := K(Bx)
        LOADBOOL,   //<    A B C    R(A) := (Bool)B; if (C) PC++
        LOADNIL,    //<    A B      R(A) := ... := R(B) := nil
        GETUPVAL,   //<    A B      R(A) := U[B]
        GETGLOBAL,  //<    A Bx     R(A) := G[K(Bx)]
        GETTABLE,   //<    A B C    R(A) := R(B)[RK(C)]
        SETGLOBAL,  //<    A Bx     G[K(Bx)] := R(A)
        SETUPVAL,   //<    A B      U[B] := R(A)
        SETTABLE,   //<    A B C    R(A)[RK(B)] := RK(C)
        NEWTABLE,   //<    A B C    R(A) := {} (size = B,C)
        SELF,       //<    A B C    R(A+1) := R(B); R(A) := R(B)[RK(C)]
        ADD,        //<    A B C    R(A) := RK(B) + RK(C)
        SUB,        //<    A B C    R(A) := RK(B) - RK(C)
        MUL,        //<    A B C    R(A) := RK(B) * RK(C)
        DIV,        //<    A B C    R(A) := RK(B) / RK(C)
        MOD,        //<    A B C    R(A) := RK(B) % RK(C)
        POW,        //<    A B C    R(A) := RK(B) ^ RK(C)
        NEG,        //<    A B      R(A) := -R(B)
        NOT,        //<    A B      R(A) := not R(B)
        AND,        //<    A B C    R(A) := RK(B) and RK(C)
        OR,         //<    A B C    R(A) := RK(B) or RK(C)
        LEN,        //<    A B	    R(A) := length of R(B)	
        CONCAT,     //<    A B      C R(A) := R(B) .. ... .. R(C)
        JMP,        //<    sBx      PC += sBx
        EQ,         //<    A B C    if ((RK(B) == RK(C)) ~= A) then PC++
        LT,         //<    A B C    if ((RK(B) < RK(C)) ~= A) then PC++
        LE,         //<    A B C    if ((RK(B) <= RK(C)) ~= A) then PC++
        GT,
        GE,
        NE,
        TEST,       //<    A B C    if not (R(A) <=> C) then PC++
        TESTSET,    //<    A B C	if (R(B) <=> C) then R(A) := R(B) else pc++
        CALL,       //<    A B C    R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1))
        TAILCALL,   //<    A B C    return R(A)(R(A+1), ... ,R(A+B-1))
        RETURN,     //<    A B      return R(A), ... ,R(A+B-2) (see note)
        RESULT,     //<    A B      result R(A), ... ,R(A+B-2) (see note) ( same as RETURN but no closing frame )
        FORLOOP,    //<    A sBx    R(A)+=R(A+2); if R(A) <?= R(A+1) then { pc+=sBx; R(A+3)=R(A) }
        FORPREP,    //<    A sBx	R(A)-=R(A+2); pc+=sBx		
        TFORLOOP,   //<    A C      R(A+3), ... ,R(A+2+C) := R(A)(R(A+1), R(A+2)); 
                    //              if R(A+3) ~= nil then R(A+2)=R(A+3) else pc++
        SETLIST,    //<    A Bx     R(A)[Bx-Bx%FPF+i] := R(A+i), 1 <= i <= Bx%FPF+1
        CLOSE,      //<    A        close stack variables up to R(A)
        CLOSURE,    //<    A Bx     R(A) := closure(KPROTO[Bx], R(A), ... ,R(A+n))
        VARARG      //<    A B	    R(A), R(A+1), ..., R(A+B-1) = varargs
    };

    [System.Serializable]
    public  struct Instruction
    {
        public static Instruction Nop = MakeA(OpCode.NOP,0);

        public uint PackedValue;

        #region Inctruction Fileds

        private const int OpCodeShift = 0;
        private const int OpCodeMask = 0x3F;

        public OpCode OpCode
        {
            get { return (OpCode)((PackedValue >> OpCodeShift) & OpCodeMask); }
            set { SetField((int)value, OpCodeShift, OpCodeMask); }
        }

        private const int AShift = 6;
        internal const int AMask = 0xFF;

        public int A
        {
            get { return (int)(PackedValue >> AShift) & AMask; }
            set { SetField(value, AShift, AMask); }
        }

        private const int BShift = 23;
        internal const int BMask = 0x1FF;

        public int B
        {
            get { return (int)(PackedValue >> BShift) & BMask; }
            set { SetField(value, BShift, BMask); }
        }

        private const int CShift = 14;
        internal const int CMask = 0x1FF;
        internal const int BitK = 1 << 8;
        internal const int BitKNeg = BitK - 1;

        public int C
        {
            get { return (int)(PackedValue >> CShift) & CMask; }
            set { SetField(value, CShift, CMask); }
        }

        private const int BxShift = 14;
        internal const int BxMask = 0x3FFFF;
        internal const int BitKx = 0x20000;

        public int Bx
        {
            get { return (int)(PackedValue >> BxShift) & BxMask; }
            set { SetField(value, BxShift, BxMask); }
        }

        private const int SBxMax = (BxMask >> 1);

        public int SBx
        {
            get { return Bx - SBxMax; }
            set { Bx = value + SBxMax; }
        }

        private const int AxShift = 6;
        private const int AxMask = 0x3FFFFFF;

        public int Ax
        {
            get { return (int)(PackedValue >> AxShift) & AxMask; }
            set { SetField(value, AxShift, AxMask); }
        }
        private void SetField(int value, int shift, int mask)
        {
            Debug.Assert((value & ~mask) == 0);
            PackedValue = (uint)(PackedValue & ~(mask << shift)) | (uint)(value << shift);
        }


        internal const int FieldsPerFlush = 50;

        #endregion

        public Instruction(uint code)
        {
            PackedValue = code;
        }

        /// <summary>
        /// Make instruction of format A
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeA(OpCode code, int a)
        {
            if (IsNotValueInRange(a, 0, AMask))
                throw SchemeError.RangeError("Opcode.A", "Opcode", "A", a, code, 0, AMask);

            var inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            return inst;
        }

        /// <summary>
        /// Make instruction of format A,B
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeAB(OpCode code, int a, int b)
        {
            if (IsNotValueInRange(a, 0, AMask))
                throw SchemeError.RangeError("Opcode.A.B", "Opcode", "A", a, code, 0, AMask);

            if (IsNotValueInRange(b, 0, BMask))
                throw SchemeError.RangeError("Opcode.A.B", "Opcode", "B", b, code, 0, BMask);

            var inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.B = b;
            return inst;
        }

        /// <summary>
        /// Make instruction of format A,B,C
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeABC(OpCode code, int a, int b, int c)
        {
            if (IsNotValueInRange(a, 0, AMask))
                throw SchemeError.RangeError("Opcode.A.B.C", "Opcode", "A", a, code, 0, AMask);

            if (IsNotValueInRange(b, 0, BMask))
                throw SchemeError.RangeError("Opcode.A.B.C", "Opcode", "B", b, code, 0, BMask);

            if (IsNotValueInRange(c, 0, CMask))
                throw SchemeError.RangeError("Opcode.A.B.C", "Opcode", "C", c, code, 0, CMask);

            var inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.B = b;
            inst.C = c;
            return inst;
        }
        /// <summary>
        /// Make instruction of format A,BX
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeABX(OpCode code, int a, int bx)
        {
            if (IsNotValueInRange(a, 0, AMask))
                throw SchemeError.RangeError("Opcode.A.Bx", "Opcode", "A", a, code, 0, AMask);

            if (IsNotValueInRange(bx, -BxMask, BxMask))
                throw SchemeError.RangeError("Opcode.A.Bx", "Opcode", "Bx", bx, code, -BxMask, BxMask);

            var inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.Bx = bx;
            return inst;
        }

        /// <summary>
        /// Make instruction of format A,B or SBX
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeASBX(OpCode code, int a, int sbx)
        {
            if (IsNotValueInRange(a, 0, AMask))
                throw SchemeError.RangeError("Opcode.A.SBx", "Opcode", "A", a, code, 0, AMask);

            if (IsNotValueInRange(sbx,-SBxMax, SBxMax))
                throw SchemeError.RangeError("Opcode.A.SBx", "Opcode", "SBx", sbx, code, -SBxMax, SBxMax);

            var inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.SBx = sbx;
            return inst;
        }

        private static bool IsNotValueInRange(int val, int min, int max)
        {
            return val < min || val > max;
        }

        public override string ToString()
        {
            var ret = new StringBuilder();

            ret.Append(OpCode.ToString());

            switch (OpCode)
            {
                case OpCode.MOVE:
                    ret.AppendFormat(": R({0}) = R({1})", A, B);
                    break;

                case OpCode.LOADK:
                    ret.AppendFormat(": R({0}) = K({1})", A, Bx);
                    break;

                case OpCode.LOADBOOL:
                    ret.AppendFormat(": R({0}) = (bool){1}", A, B != 0);
                    if (C != 0)
                        ret.Append("; PC++");
                    break;

                case OpCode.LOADNIL:
                    ret.AppendFormat(B == 1 ? ": R({0}) = nil" : ": R({0}..{1}) = nil", A, A + B - 1);
                    break;

                case OpCode.GETUPVAL:
                    ret.AppendFormat(": R({0}) = U({1})", A, B);
                    break;

                case OpCode.GETGLOBAL:
                    // LUA ret.AppendFormat(": R({0}) = G[K({1})]", A, Bx);
                    ret.AppendFormat(": R({0}) = G[Rt({1})]", A, Bx);
                    break;

                case OpCode.GETTABLE:
                    ret.AppendFormat(": R{0} = R({1})[{2}]", A, B, Rk(C));
                    break;

                case OpCode.SETGLOBAL:
                    //LUA ret.AppendFormat(": G[K{1}] = R({0})", A, Bx);
                    ret.AppendFormat(": G[Rt{1}] = R({0})", A, Bx);
                    break;

                case OpCode.SETUPVAL:
                    ret.AppendFormat(": U({1}) = R({0})", A, B);
                    break;

                case OpCode.SETTABLE:
                    ret.AppendFormat(": R({0})[{1}] = {2}", A, Rk(B), Rk(C));
                    break;

                case OpCode.NEWTABLE:
                    ret.AppendFormat(": R({0}) = {} (arr={1}, hash={2})", A, B, C);
                    break;

                case OpCode.SELF:
                    ret.AppendFormat(": R({0}) = R({1}), R({2}) = R({1})[{3}]", A + 1, B, A, Rk(C));
                    break;

                case OpCode.ADD:
                case OpCode.SUB:
                case OpCode.MUL:
                case OpCode.DIV:
                case OpCode.MOD:
                case OpCode.POW:
                    ret.AppendFormat(": R({0}) = {1} {3} {2}", A, Rk(B), Rk(C), GetArithOp(OpCode));
                    break;

                case OpCode.NEG:
                    ret.AppendFormat(": R({0}) = - R({1})", A, B);
                    break;

                case OpCode.NOT:
                    ret.AppendFormat(": R({0}) = not {1}", A, Rk(B));
                    break;

                case OpCode.LEN:
                    break;

                case OpCode.CONCAT:
                    ret.AppendFormat(": R({0}) = R({1} ... {2})", A, B, C);
                    break;

                case OpCode.JMP:
                    ret.Append(":");
                    if (SBx != 0)
                        ret.AppendFormat(" PC {1}= {0}", System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    if (A != 0)
                        ret.AppendFormat(" CloseUpVals( R# >= {0} )", A + 1);
                    break;

                case OpCode.EQ:
                case OpCode.LT:
                case OpCode.LE:
                case OpCode.NE:
                case OpCode.GT:
                case OpCode.GE:
                    ret.AppendFormat(": R({0}) = {1} {2} {3}", A, Rk(B), GetCmpOp(OpCode), Rk(C));
                    break;

                case OpCode.TEST:
                    ret.AppendFormat(": if( R{0}.AsBool == {1} ) then PC++", A, C);
                    break;

                case OpCode.TESTSET:
                    ret.AppendFormat(": if( R{0}.AsBool == {1} ) then PC++ else R{2} = {0}", B, C, A);
                    break;

                case OpCode.CALL:
                    {
                        /// result
                        if (C == 0)
                            ret.AppendFormat(":");
                        else if (C == 1)
                            ret.AppendFormat(": R{0} =", A);
                        else
                            ret.AppendFormat(": R{0}..R{1} =", A, A + C - 1);
                        /// function
                        ret.AppendFormat(" R{0}", A);
                        /// arguments
                        if (B == 0)
                            ret.AppendFormat("()");
                        else if (B == 1)
                            ret.AppendFormat("(R{0})", A + 1);
                        else
                            ret.AppendFormat("(R{0}..R{1})", A + 1, A + 1 + B - 1);
                    }
                    break;


                case OpCode.TAILCALL:
                    ret.AppendFormat(": return R{0}", A);

                    if (B == 0)
                        ret.AppendFormat("( R{0}... )", A + 1);
                    else if (B == 1)
                        ret.Append("()");
                    else if (B == 2)
                        ret.AppendFormat("( R{0} )", A + 1);
                    else
                        ret.AppendFormat("( R{0}..R{1} )", A + 1, A + 1 + B - 2);
                    break;

                case OpCode.RETURN:
                    if (B <= 0)
                        ret.AppendFormat(": void");
                    else if (B == 1)
                        ret.AppendFormat(": R{0}", A);
                    else
                        ret.AppendFormat(": R{0}..R{1}", A, A + B - 1);
                    break;

                case OpCode.RESULT:
                    if (B <= 0)
                        ret.AppendFormat(": void");
                    else if (B == 1)
                        ret.AppendFormat(": R{0}", A);
                    else
                        ret.AppendFormat(": R{0}..R{1}", A, A + B - 1);
                    break;

                case OpCode.FORLOOP:
                    {
                        ret.AppendFormat(": R({0}) += R({1}); if R({0}) <?= R({2}) then PC+= {3}", A, A + 2, A + 1, System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    }
                    break;

                case OpCode.FORPREP:
                    break;

                case OpCode.TFORLOOP:
                    {
                        ret.AppendFormat(": if(type(R({1})) == table) {{ R({0}) = R({1}), PC {3}= {2} }}", A, A + 1, System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    }
                    break;

                case OpCode.SETLIST:
                    ret.AppendFormat(": R({0}) R({1})", A, B);
                    break;

                case OpCode.CLOSE:
                    ret.AppendFormat(": close stack up to R({0})", A);
                    break;

                case OpCode.CLOSURE:
                    ret.AppendFormat(": R({0}) = MakeClosure( K{1} )", A, Bx);
                    break;

                case OpCode.VARARG:
                    ret.AppendFormat(": R({0}) = MakeClosure( P{1} )", A, Bx);
                    break;

                default:
                    ret.Append("BAD!!!!!");
                    break;
            }

            return ret.ToString();
        }

        private static string GetArithOp(OpCode op)
        {
            switch (op)
            {
                case OpCode.ADD: return "+";
                case OpCode.SUB: return "-";
                case OpCode.MUL: return "*";
                case OpCode.DIV: return "/";
                case OpCode.MOD: return "%";
                case OpCode.POW: return "^";
                default: return string.Empty;
            }
        }

        private static string GetCmpOp(OpCode op)
        {
            switch (op)
            {
                case OpCode.EQ: return "==";
                case OpCode.LT: return "<";
                case OpCode.LE: return "<=";
                case OpCode.NE: return "!=";
                case OpCode.GT: return ">";
                case OpCode.GE: return ">=";
                default: return string.Empty;
            }
        }
        private static string Rk(int i)
        {
            if ((i & BitK) != 0)
                return string.Format("K({0})", (i & ~BitK));
            else
                return string.Format("R({0})", i);
        }
        private static string R(int i)
        {
            return string.Format("R({0})", i);
        }
    }
}