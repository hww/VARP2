/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Data
{
    public class Pair : SObject
    {
        public object car;
        public object cdr;

        public Pair ( object car, object cdr )
        {
            this.car = car;
            this.cdr = cdr;
        }

        public override bool AsBool ( ) { return true; }
        public override string ToString ( )
        {
            return string.Format ( "({0} . {1})", Datum.ObjectToString  ( car ), Datum.ObjectToString  ( cdr ) );
        }
    }
}
