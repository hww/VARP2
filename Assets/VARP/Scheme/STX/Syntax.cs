/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System.Collections.Generic;

namespace VARP.Scheme.STX
{
    using System;
    using Data;
    using DataStructures;

    /// <summary>
    /// This class is next step after tokenizer. The token is just string with
    /// source file location info. The syntax is the string converted to variant
    /// class additionaly it may have source code location info
    /// </summary>
    public abstract class Syntax : SObject
    {
        // May to have the source code location by could be just null
        public Location location;

        public Syntax ( Location location ) 
        {
            this.location = location;
        }

        // -- Cast Syntax To ... Methods ---------------------------------------------------------------

        public abstract object getDatum ( );
        public Location getLocation ( ) { return location != null ? location : Location.NullLocation; }
        public virtual List<Syntax> toList ( Location location = null ) { return null; }
        public virtual LinkedList<Syntax> toLinkedList ( Location location = null) { return null; }
        
        // -- Datum Extractor Methods --------------------------------------------------------------------

        /// <summary>
        /// Method safely cast the syntax's expression to the Datum
        /// </summary>
        /// <param name="stx"></param>
        /// <returns></returns>
        public static object SyntaxToDatum ( Syntax stx )
        {
            UnityEngine.Debug.Assert ( stx != null );
            return stx.getDatum ( );
        }

        // -- Convert Syntax to the syntax List -----------------------------------------------------------

        public static List<Syntax> SyntaxToList ( Syntax stx, Location location = null )
        {
            UnityEngine.Debug.Assert ( stx != null );
            return stx.toList ( location );
        }
        public static LinkedList<Syntax> SyntaxToLinkedList ( Syntax stx, Location location = null )
        {
            UnityEngine.Debug.Assert ( stx != null );
            return stx.toLinkedList ( location );
        }

        public virtual bool IsSymbol { get { return false; } }
        public virtual bool IsIdentifier { get { return false; } }
        public virtual bool IsLiteral { get { return false; } }
        public virtual bool IsExpression { get { return false; } }

        public override bool AsBool ( ) { return true; }
        public override string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            return string.Format ( "#<syntax:{0} {1}>", getLocation ( ).GetLocationStringShort ( ), ToString() );
        }

        public readonly static Syntax Lambda = new SyntaxName ( (Variant)(Name)EName.Lambda );
        public readonly static Syntax Void = new SyntaxName ( (Variant)(Name)EName.Void );
        public readonly static Syntax Nil = new SyntaxName ( (Variant)(Name)EName.None );
        public readonly static Syntax True = new SyntaxName ( (Variant)(Name)EName.True );
        public readonly static Syntax False = new SyntaxName ( (Variant)(Name)EName.False );

        // -- Factory Methods ----------------------------------------------------------------------------
        public static SyntaxChar Create ( char value, Location location = null) { return new SyntaxChar ( value, location ); }
        public static SyntaxBool Create ( bool value, Location location = null ) { return new SyntaxBool ( value, location ); }
        public static SyntaxInteger Create ( int value, Location location = null ) { return new SyntaxInteger ( value, location ); }
        public static SyntaxFloat Create ( float value, Location location = null ) { return new SyntaxFloat ( value, location ); }
        public static SyntaxName Create ( Name value, Location location = null ) { return new SyntaxName ( value, location ); }
        public static SyntaxPair Create ( Pair value, Location location = null ) { return new SyntaxPair ( value, location ); }
        public static SyntaxString Create ( string value, Location location = null ) { return new SyntaxString ( value, location ); }
        public static SyntaxList Create ( List<Syntax> value, Location location = null ) { return new SyntaxList ( value, location ); }
        public static SyntaxLinkedList Create ( LinkedList<Syntax> value, Location location = null ) { return new SyntaxLinkedList ( value, location ); }

    }

    public class SyntaxChar : Syntax
    {
        public char asChar;
        public SyntaxChar ( char value, Location location = null) : base ( location ) { asChar = value; }
        public override object getDatum ( ) { return asChar; }
        public override string ToString ( ) { return NamedCharacter.CharacterToName(asChar); }
        public override bool IsLiteral { get { return true; } }
    }

    public class SyntaxBool : Syntax
    {
        public bool asBool;
        public SyntaxBool(bool value, Location location = null ) : base(location) { asBool = value; }
        public override object getDatum()  { return asBool; }
        public override string ToString ( ) { return asBool ? "#t" : "#f"; }
        public override bool IsLiteral { get { return true; } }
    }

    public class SyntaxInteger : Syntax
    {
        public int asInteger;
        public SyntaxInteger ( int value, Location location = null ) : base(location) { asInteger = value; }
        public override object getDatum ( ) { return asInteger; }
        public override string ToString ( ) { return asInteger.ToString ( ); }
        public override bool IsLiteral { get { return true; } }
    }

    public class SyntaxFloat : Syntax
    {
        public float asFloat;
        public SyntaxFloat ( float value, Location location = null ) : base(location) { asFloat = value; }
        public override object getDatum ( ) { return asFloat; }
        public override string ToString ( ) { return asFloat.ToString ( ); }
        public override bool IsLiteral { get { return true; } }
    }

    public class SyntaxPair : Syntax
    {
        public Pair asPair;
        public SyntaxPair ( Pair value, Location location = null ) : base(location) { asPair = value; }
        public override object getDatum ( ) { return asPair; }
        public override string ToString ( ) { return asPair.ToString ( ); }
        public override bool IsLiteral { get { return true; } }
    }

    public class SyntaxName : Syntax
    {
        public Name asName;
        public SyntaxName ( Name value, Location location = null ) : base ( location ) { asName = value; }
        public override object getDatum ( ) { return asName; }
        public override string ToString ( ) { return asName.ToString ( ); }
        public override bool IsIdentifier { get { return asName.IsIdentifier; } }
        public override bool IsSymbol { get { return true; } }
        public override bool IsLiteral { get { return asName.IsKeyword; } }
    }

    public class SyntaxString : Syntax
    {
        public string asString;
        public SyntaxString ( string value, Location location = null ) : base(location) { asString = value != null ? value : string.Empty; }
        public override object getDatum ( ) { return asString; }
        public override string ToString ( ) { return string.Format("\"{0}\"", asString); }
        public override bool IsLiteral { get { return true; } }
    }

    public class SyntaxList : Syntax
    {
        public List<Syntax> asList;
        public SyntaxList ( List<Syntax> value, Location location = null ) : base(location) { asList = value; }
        public override object getDatum ( ) {
            var result = new LinkedList<object> ( );
            foreach ( var v in asList )
                result.AddLast ( SyntaxToDatum ( v ) );
            return result;
        }
        public override string ToString ( ) {
            if ( asList == null )
                return "#()";
            var sb = new System.Text.StringBuilder ( );
            sb.Append ( "#(" );
            for (var i=0 ; i<asList.Count ; i++)
            {
                if (i > 0)
                    sb.Append ( " " );
                sb.Append ( SObject.ObjectToString(asList[i]));
            }
            sb.Append ( ")" );
            return sb.ToString();
        }

        private object StringBuilder ( )
        {
            throw new NotImplementedException ( );
        }

        public override List<Syntax> toList( Location location = null )
        {
            var result = new List<Syntax> ( );
            foreach ( var v in asList )
                result.Add ( v );
            return result;
        }
        public override LinkedList<Syntax> toLinkedList ( Location location = null )
        {
            var result = new LinkedList<Syntax> ( );
            foreach ( var v in asList )
                result.AddLast ( v );
            return result;
        }
        public override bool IsLiteral { get { return true; } }
    }

    public class SyntaxLinkedList : Syntax
    {
        public LinkedList<Syntax> asLinkedList;
        public SyntaxLinkedList ( LinkedList<Syntax> value, Location location = null ) : base(location) { asLinkedList = value; }
        public override object getDatum ( ) {
            var result = new LinkedList<object> ( );
            foreach ( var val in asLinkedList )
                result.AddLast ( SyntaxToDatum ( val ) );
            return result;
        }
        public override string ToString ( ) { return asLinkedList == null ? "()" : asLinkedList.ToString ( ); }

        public override List<Syntax> toList ( Location location = null )
        {
            var result = new List<Syntax> ( );
            foreach ( var v in asLinkedList )
                result.Add ( v );
            return result;
        }
        public override LinkedList<Syntax> toLinkedList ( Location location = null)
        {
            var result = new LinkedList<Syntax> ( );
            foreach ( var v in asLinkedList )
                result.AddLast ( v );
            return result;
        }
        public override bool IsExpression { get { return true; } }
    }
}