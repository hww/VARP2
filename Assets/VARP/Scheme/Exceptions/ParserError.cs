
namespace VARP.Scheme.Exceptions
{
    using System.Text;
    using Tokenizing;
    using Data;
    using DataStructures;

    /// <summary>
    /// General exception class
    /// </summary>
    public partial class ParserError : BaseSchemeError
    {
        public ParserError ( ) : base ( )
        {
        }
        public ParserError ( string message ) : base ( message )
        {
        }
        public ParserError ( string message, System.Exception innerException ) : base ( message, innerException )
        {
        }


        // -- Result error -------------------------------------------------------------------------------

        /// <summary>
        /// Result error message for single result outputs
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expected"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string ResultErrorMessage ( string name, string expected, object result )
        {
            var locs = GetLocationString ( result );
            var vstr = Inspect ( result );
            return string.Format ( "{0}{1}: contract violation\n   expected: {2}\n   given: {3}", locs, name, expected, vstr );
        }

        /// <summary>
        /// Result error message for multiple results
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expected"></param>
        /// <param name="badPos"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static string ResultErrorMessage ( string name, string expected, int badPos, params object[] results )
        {
            var sb = new StringBuilder ( );
            var loc = string.Empty;
            var badStr = string.Empty;
            for ( var i = 0 ; i < results.Length ; i++ )
            {
                if ( i == badPos )
                {
                    loc = GetLocationString ( results[ i ] );
                    badStr = Inspect ( results[ i ] );
                    continue; // skip bad argument
                }
                sb.Append ( "  " );
                sb.AppendLine ( Inspect ( results[ i ] ) );
            }
            var argStr = sb.ToString ( );
            return string.Format ( "{0}{1}: contract violation\n   expected: {2}\n   given: {3}\n  result position: {4}\n  other result position...:\n{5}", loc, name, expected, badPos, badStr, argStr );
        }
        public static ParserError ResultError ( string name, string expected, object val )
        {
            return new ParserError ( ResultErrorMessage ( name, expected, val ) );
        }
        public static ParserError ResultError ( string name, string expected, int badPos, params object[] vals )
        {
            return new ParserError ( ResultErrorMessage ( name, expected, badPos, vals ) );
        }

        // -- Argument error -----------------------------------------------------------------------------

        public static string ArgumentErrorMessage ( string name, string expected, object val )
        {
            var vstr = Inspect ( val );
            return string.Format ( "{0}: contract violation\n   expected: {1}\n   given: {2}", name, expected, vstr );
        }
        public static string ArgumentErrorMessage ( string name, string expected, int badPos, params object[] vals )
        {
            var sb = new StringBuilder ( );
            var loc = string.Empty;
            var badStr = string.Empty;
            for ( var i = 0 ; i < vals.Length ; i++ )
            {
                if ( i == badPos )
                {
                    loc = GetLocationString ( vals[ i ] );
                    badStr = Inspect ( vals[ i ] );
                    continue; // skip bad argument
                }
                sb.Append ( "  " );
                sb.AppendLine ( Inspect ( vals[ i ] ) );
            }
            var argStr = sb.ToString ( );

            return string.Format ( "{0}{1}: contract violation\n   expected: {2}\n   given: {3}\n  argument position: {4}\n  other arguments...:\n{5}", loc, name, expected, badPos, badStr, argStr );
        }
        public static ParserError ArgumentError ( string name, string expected, object val )
        {
            return new ParserError ( ResultErrorMessage ( name, expected, val ) );
        }
        public static ParserError ArgumentError ( string name, string expected, int badPos, params object[] vals )
        {
            return new ParserError ( ResultErrorMessage ( name, expected, badPos, vals ) );
        }
        public static ParserError ArgumentError ( string name, string expected, int badPos, LinkedList<Variant> val )
        {
            return new ParserError ( ResultErrorMessage ( name, expected, val ) );
        }

        // -- Syntax Error -------------------------------------------------------------------------------

        /// <summary>
        /// Syntax error message
        /// </summary>
        /// <param name="name">name of method where happen error</param>
        /// <param name="message">error message</param>
        /// <param name="expression">expression where happen error</param>
        /// <param name="subexpression">exact token or syntax where happen error</param>
        /// <returns></returns>
        public static string SyntaxErrorMessage ( string name, string message, object expression, object subexpression = null )
        {
            if ( subexpression == null )
            {
                var expStr = Inspect ( expression );
                var expLoc = GetLocationString ( expression );
                return string.Format ( "{0}: {1}: {2} in: {3}", expLoc, name, message, expStr );
            }
            else
            {
                var expStr = Inspect ( expression );
                var subLoc = GetLocationString ( subexpression );
                var subStr = Inspect ( expression );
                return string.Format ( "{0}: {1}: {2} in: {3}\n error syntax: {4}", subLoc, name, message, expStr, subStr );
            }
        }
        /// <summary>
        /// Create new syntax error exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <param name="expression"></param>
        /// <param name="subexpression"></param>
        /// <returns></returns>
        public static TokenizerError SyntaxError ( string name, string message, object expression, object subexpression = null )
        {
            return new TokenizerError ( SyntaxErrorMessage ( name, message, expression, subexpression ) );
        }

    }
}