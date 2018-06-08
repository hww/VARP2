using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VARP.DataStructures
{
    [StructLayout ( LayoutKind.Explicit )]
    public unsafe struct Variant
    {
        public enum Type : byte
        {
            Undefined,
            Integer,
            Float,
            Bool,
            Name
        }

        [FieldOffset ( 0 )]
        public int asInteger;
        [FieldOffset ( 0 )]
        public float asFloat;
        [FieldOffset ( 0 )]
        public bool asBool;
        [FieldOffset ( 0 )]
        public Name asName;

        [FieldOffset ( 4 )]
        public Type type;

        public void Set( int value )
        {
            type = Type.Integer;
            asInteger = value;
        }
        public void Set ( float value )
        {
            type = Type.Float;
            asFloat = value;
        }
        public void Set ( bool value )
        {
            type = Type.Bool;
            asBool = value;
        }
        public void Set ( Name value )
        {
            type = Type.Name;
            asName = value;
        }

        public override string ToString ( )
        {
            switch ( type )
            {
                case Type.Undefined:
                    return "<unefined>";
                case Type.Integer:
                    return asInteger.ToString ( );
                case Type.Float:
                    return asFloat.ToString ( );
                case Type.Bool:
                    return asBool.ToString ( );
                case Type.Name:
                    return asName.ToString ( );
            }
            return base.ToString ( );
        }
    }

}
