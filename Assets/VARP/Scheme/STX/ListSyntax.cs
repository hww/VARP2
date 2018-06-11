using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VARP.Scheme.STX
{
    using Data;

    public class ListSyntax : List<Syntax>
    {

        public override string ToString ( )
        {
            var sb = new StringBuilder ( );
            var appendSpace = false;
            sb.Append ( "#(" );
            foreach ( var v in this )
            {
                if ( appendSpace )
                    sb.Append ( " " );
                sb.Append ( Datum.ObjectToString(v) );
                appendSpace |= true;
            }
            sb.Append ( ")" );
            return sb.ToString ( );
        }

        public string Inspect ( InspectOptions options = InspectOptions.Default )
        {
            var sb = new StringBuilder ( );
            var appendSpace = false;
            sb.Append ( "#(" );
            foreach ( var v in this )
            {
                if ( appendSpace )
                    sb.Append ( " " );
                sb.Append ( Inspector.InspectObject ( v, options ) );
                appendSpace |= true;
            }
            sb.Append ( ")" );
            return sb.ToString ( );
        }
    }
}
