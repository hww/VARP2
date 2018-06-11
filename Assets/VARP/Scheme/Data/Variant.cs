using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VARP.Scheme.Data
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

        // -- Setters ------------------------------------------------------------------------

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

        // -- Cast type ------------------------------------------------------------------------

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
                    return asName.ToString();
            }
            return base.ToString ( );
        }

        // implicit conversion bool name = variantVariable
        public static implicit operator bool(Variant right)
        {
            if ( right.type == Type.Bool )
                return right.asBool;
            throw new System.Exception ( );
        }
        // implicit conversion int name = variantVariable
        public static implicit operator int ( Variant right )
        {
            if ( right.type == Type.Integer )
                return right.asInteger;
            if ( right.type == Type.Float )
                return (int)right.asFloat;
            throw new System.Exception ( );
        }
        // implicit conversion float name = variantVariable
        public static implicit operator float ( Variant right )
        {
            if ( right.type == Type.Float )
                return right.asFloat;
            if ( right.type == Type.Integer )
                return (float)right.asInteger;
            throw new System.Exception ( );
        }
        // implicit conversion Name name = variantVariable
        public static implicit operator Name ( Variant right )
        {
            if ( right.type == Type.Name )
                return right.asName;
            throw new System.Exception ( );
        }

        // explicit conversion (Variant)true
        public static explicit operator Variant ( bool value )
        {
            return new Variant ( ) { asBool = value, type = Type.Bool };
        }
        // explicit conversion (Variant)100
        public static explicit operator Variant ( int value )
        {
            return new Variant ( ) { asInteger = value, type = Type.Integer };
        }
        // explicit conversion (Variant)0.1f
        public static explicit operator Variant ( float value )
        {
            return new Variant ( ) { asFloat = value, type = Type.Float };
        }
        // explicit conversion (Variant)Name.Intern("hello")
        public static explicit operator Variant ( Name value )
        {
            return new Variant ( ) { asName = value, type = Type.Name };
        }

        // -- Comparison ------------------------------------------------------------------------

        public override bool Equals ( object obj )
        {
            if ( obj == null || GetType ( ) != obj.GetType ( ) )
                return false;
            Variant other = (Variant)obj;
            return type == other.type && asInteger == other.asInteger;
        }

        public override int GetHashCode ( )
        {
            return asInteger ^ (int)type;
        }

        public string Inspect()
        {
            return string.Format ( "#<variant:{0} {1}>", type.ToString().ToLower(), ToString() );
        }


    }

}
