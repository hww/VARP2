using System;
using UnityEngine;

namespace VARP.Terminal
{
    public partial class TerminalRenderer : ITerminal
    {
        // ----------------------------------------------------------------------------------------------------
        // -- Construtors 
        // ----------------------------------------------------------------------------------------------------

        public TerminalRenderer ( int charsX, int charsY )
        {
            sizeX = charsX;
            sizeY = charsY;
            charactersPositions = new Vector3[ sizeY, sizeX ];
            charactersBuffer = new TerminalCharacter[ sizeY, sizeX ];
            negativeCharactersIndices = new IntPosition[ sizeY * sizeX ];
            // load resources 
            theme = new TangoTheme ( );
            cursor = new TerminalCursor ( this );
            defaultMaterial = ReadMaterial ( "VARP/DebugDraw/GLFontZOff" );
            defaultFont = ReadFont ( "VARP/DebugDraw/GLFont" );
            backgroundMaterial = ReadMaterial ( "VARP/DebugDraw/GLlineZOff" );
            // get font's information
            CharacterInfo space;
            defaultFont.GetCharacterInfo ( ' ', out space );
            lineHeight = defaultFont.lineHeight;
            charWidth = space.advance;
            textRectangle = new Rect ( 0, 0, charWidth * sizeX, lineHeight * sizeY );
            backgroundRectangle = new Rect ( textRectangle.min - backgrounOffset, textRectangle.max + backgrounOffset );
            // fill buffer coordiates
            for ( var y = 0 ; y < sizeY ; y++ )
            {
                for ( var x = 0 ; x < sizeX ; x++ )
                    charactersPositions[ y, x ] = new Vector3 ( (float)x * charWidth, (float)y * lineHeight, 0 );
            }
            // clear screen
            Clear ( );
        }

        // ----------------------------------------------------------------------------------------------------
        // -- ITerminal Methods 
        // ----------------------------------------------------------------------------------------------------

        public void Write ( string message )
        {
            if ( message == null )
                throw new ArgumentNullException ( "message" );
            if ( message == string.Empty )
                return;
            var idx = 0;
            WriteInternal ( message, ref idx );
        }
        // Write text to the input field and add new line
        public void WriteLine ( string message )
        {
            if ( message == null )
                throw new ArgumentNullException ( "message" );
            if ( message == string.Empty )
                return;
            var idx = 0;
            WriteInternal ( message, ref idx );
            cursor.NewLine ( );
        }
        private void WriteInternal ( string message, ref int idx )
        {

            while ( idx < message.Length )
            {
                var c = message[ idx ];
                if ( c < ' ' )
                {
                    // Escape codes 
                    switch ( c )
                    {
                        case '\a':
                            Beep ( );
                            idx++;
                            break;
                        case '\b':
                            cursor.AddX ( -1 );
                            WriteVisibleCharacter ( ' ', cursor.X, cursor.Y );
                            idx++;
                            break;
                        case '\t':
                            Tab ( );
                            idx++;
                            break;
                        case '\n':
                            cursor.NewLine ( );
                            idx++;
                            break;
                        case '\r':
                            cursor.X = cursor.xMin;
                            idx++;
                            break;
                        case (char)27:
                            WriteEscapeCharacter ( message, ref idx );
                            break;
                    }
                }
                else if ( c == (char)127 )
                {
                    for ( var x = cursor.X ; x < cursor.xMax - 1 ; x++ )
                        charactersBuffer[ cursor.Y, x ] = charactersBuffer[ cursor.Y, x + 1 ];
                    WriteVisibleCharacter ( ' ', cursor.X, cursor.Y );
                    idx++;
                    break;
                }
                else
                {
                    WriteVisibleCharacter ( c, cursor.X, cursor.Y );
                    cursor.AddX ( 1 );
                    idx++;
                    break;
                }
            }
        }
        public void WriteEscapeCharacter ( string message, ref int idx )
        {
            idx++;
            switch ( message[ idx ] )
            {
                case '[':
                    break;
            }
        }
        public void WriteVisibleCharacter ( char c, int x, int y )
        {
            charactersBuffer[ y, x ] = new TerminalCharacter ( ) { character = c, foreground = foregroundColor };
        }

        // Produce beep sound if it is defined
        public void Beep ( )
        {
            //if (beepClip!=null) beepSound.PlayOneShot(beepClip);
        }
        // ----------------------------------------------------------------------------------------------------
        // -- Clear 
        // ----------------------------------------------------------------------------------------------------

        // Clear terminal screen
        public void Clear ( )
        {
            ClearRectangle ( cursor.xMin, cursor.yMin, cursor.xMax, cursor.yMax );
            cursor.SetPosition ( 0, 0 );
        }

        private void ClearRectangle ( int x1, int y1, int x2, int y2 )
        {
            for ( var y = y1 ; y <= y2 ; y++ )
            {
                for ( var x = x1 ; x <= x2 ; x++ )
                    charactersBuffer[ y, x ] = new TerminalCharacter ( ) { character = ' ', foreground = foregroundColor };
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // Cursor Position
        // ----------------------------------------------------------------------------------------------------

        // Set cursor
        public void SetCursor ( int x, int y )
        {
            cursor.SetPosition ( x, y );
        }
        private void Tab ( )
        {
            var xtab = cursor.GetLastColumnOfThisTab ( );
            for ( var i = 0 ; i <= xtab ; i++ )
            {
                WriteVisibleCharacter ( ' ', cursor.X, cursor.Y );
                cursor.AddX ( 1 );
            }
        }
        private Vector3 GetCursorCoordinates ( )
        {
            return new Vector3 ( charWidth * cursor.X, lineHeight * cursor.Y, 0 );
        }
        private Vector3 GetCursorSize ( )
        {
            return new Vector3 ( charWidth, lineHeight, 0 );
        }
        public void SetWindow ( int xmin, int ymin, int xmax, int ymax )
        {
            cursor.SetWindow ( xmin, ymin, xmax, ymax );
        }
        // ----------------------------------------------------------------------------------------------------
        // -- Coloring
        // ----------------------------------------------------------------------------------------------------

        // Reset foreground color to defaut
        public void ResetColor ( )
        {
            foregroundColor = theme.foreground;
            backgroundColor = theme.background;
        }
        public void SetColor ( Color color )
        {
            foregroundColor = color;
        }
        public void SetBackgroundColor ( Color color )
        {
            backgroundColor = color;
        }

        // ----------------------------------------------------------------------------------------------------
        // -- Scrolling 
        // ----------------------------------------------------------------------------------------------------

        // @linesNumber > 0 is scrolling down
        public void Scroll ( int linesOffset )
        {
            if ( linesOffset > 0 )
                ScrollDown ( linesOffset );
            else if ( linesOffset < 0 )
                ScrollUp ( -linesOffset );
        }
        private void ScrollUp ( int linesNumber )
        {
            var targetLine = cursor.yMin;
            for ( var sourceLine = targetLine + linesNumber ; sourceLine < cursor.yMax ; sourceLine++, targetLine++ )
                CopyLine ( cursor.xMin, sourceLine, cursor.xMax, targetLine );
            ClearRectangle ( cursor.xMin, targetLine, cursor.xMax, cursor.yMax );
        }
        private void ScrollDown ( int linesNumber )
        {
            var targetLine = cursor.yMax;
            for ( var sourceLine = cursor.yMax - linesNumber ; sourceLine >= 0 ; sourceLine--, targetLine-- )
                CopyLine ( cursor.xMin, sourceLine, cursor.xMax, targetLine );
            ClearRectangle ( cursor.xMin, 0, cursor.xMax, targetLine );
        }
        private void CopyLine ( int x1, int y1, int x2, int y2 )
        {
            if ( y1 == y2 )
                return;
            for ( var x = x1 ; x <= x2 ; x++ )
                charactersBuffer[ y2, x ] = charactersBuffer[ y1, x ];
        }

        // ----------------------------------------------------------------------------------------------------
        // -- Rendering
        // ----------------------------------------------------------------------------------------------------

        public void Render ( )
        {
            if ( Input.GetKeyDown ( KeyCode.BackQuote ) )
                isVisible = !isVisible;
            if ( isVisible )
            {
                // Render terminal
                if ( Input.GetKeyDown ( KeyCode.Tab ) )
                {
                    //if (inputField.isFocused)
                    //    inputField.text = Console.OnAutocomplete(inputField.text, inputField.caretPosition);
                }
                else if ( Input.GetKeyDown ( KeyCode.Return ) )
                {
                    //Console.OnReadLine(inputField.text);
                }
                RenderScreen ( );
            }
        }

        public void RenderScreen ( )
        {
            GL.invertCulling = true;
            GL.PushMatrix ( );
            GL.LoadPixelMatrix ( );
            GL.Begin ( GL.QUADS );
            // Render background
            backgroundMaterial.SetPass(0);
            if ( isBackgroundVisible )
            {
                GL.Color ( backgroundColor );
                GL.Vertex ( new Vector3 ( backgroundRectangle.min.x, backgroundRectangle.max.y, 0 ) );
                GL.Vertex ( new Vector3 ( backgroundRectangle.max.x, backgroundRectangle.max.y, 0 ) );
                GL.Vertex ( new Vector3 ( backgroundRectangle.max.x, backgroundRectangle.min.y, 0 ) );
                GL.Vertex ( new Vector3 ( backgroundRectangle.min.x, backgroundRectangle.min.y, 0 ) );
            }
            // Render text buffer
            defaultMaterial.SetPass ( 0 );
            var negativeCharsCount = 0;
            for ( var y = 0 ; y < sizeY ; y++ )
            {
                for ( var x = 0 ; x < sizeX ; x++ )
                {
                    if ( charactersBuffer[ y, x ].isNegative )
                        negativeCharactersIndices[ negativeCharsCount++ ] = new IntPosition ( x, y );
                    RenderCharacter ( charactersPositions[ y, x ], charactersBuffer[ y, x ].character, charactersBuffer[ y, x ].foreground );
                }
            }
            // Render negative characters
            backgroundMaterial.SetPass ( 0 );
            GL.Color ( theme.selectionColor );
            for ( var i = 0 ; i < negativeCharsCount ; i++ )
            {
                var xyidx = negativeCharactersIndices[ i ];
                var min = charactersPositions[ xyidx.y, xyidx.x ];
                var max = GetCursorSize ( );
                GL.Vertex ( new Vector3 ( min.x, max.y, 0 ) );
                GL.Vertex ( new Vector3 ( max.x, max.y, 0 ) );
                GL.Vertex ( new Vector3 ( max.x, min.y, 0 ) );
                GL.Vertex ( new Vector3 ( min.x, min.y, 0 ) );
            }
            // Render cursor
            if ( isCursorVisible )
            {
                var min = GetCursorCoordinates ( );
                var max = min + GetCursorSize ( );
                GL.Color ( theme.cursorColor );
                GL.Vertex ( new Vector3 ( min.x, max.y, 0 ) );
                GL.Vertex ( new Vector3 ( max.x, max.y, 0 ) );
                GL.Vertex ( new Vector3 ( max.x, min.y, 0 ) );
                GL.Vertex ( new Vector3 ( min.x, min.y, 0 ) );
            }
            GL.End ( );
            GL.PopMatrix ( );
            GL.invertCulling = false;
        }
        private void RenderCharacter ( Vector3 posisiton, char character, Color color )
        {
            CharacterInfo info;
            if ( defaultFont.GetCharacterInfo ( character, out info ) )
            {
                GL.Color ( color );
                GL.MultiTexCoord ( 0, info.uvTopLeft );
                GL.Vertex ( posisiton + new Vector3 ( info.minX, info.maxY, 0 ) );
                GL.MultiTexCoord ( 0, info.uvTopRight );
                GL.Vertex ( posisiton + new Vector3 ( info.maxX, info.maxY, 0 ) );
                GL.MultiTexCoord ( 0, info.uvBottomRight );
                GL.Vertex ( posisiton + new Vector3 ( info.maxX, info.minY, 0 ) );
                GL.MultiTexCoord ( 0, info.uvBottomLeft );
                GL.Vertex ( posisiton + new Vector3 ( info.minX, info.minY, 0 ) );
            }
        }

        // -- Frame buffer -----------------------------------------------------------------------------------

        private struct TerminalCharacter
        {
            public char character;         // character code
            public Color foreground;       // foreground color
            public bool isNegative;        // is negative font
        }
        private struct IntPosition
        {
            public int x;
            public int y;
            public IntPosition ( int x, int y ) { this.x = x; this.y = y; }
        }
        private TerminalCharacter[,] charactersBuffer;
        private Vector3[,] charactersPositions;
        private IntPosition[] negativeCharactersIndices;

        // -- terminal's fields -------------------------------------------------------------------------------

        public bool isVisible;
        public bool isBackgroundVisible;
        public bool isCursorVisible;
        public TerminalCursor cursor;
        public readonly int sizeX;
        public readonly int sizeY;
        public readonly Font defaultFont;
        public readonly Font negativeFont;
        public readonly Material defaultMaterial;
        public readonly Material backgroundMaterial;
        public readonly float lineHeight;
        public readonly float charWidth;
        public readonly Rect textRectangle;
        public readonly Rect backgroundRectangle;
        public readonly Vector2 backgrounOffset = new Vector2 ( 0, 0 );
        private readonly Xresources theme;
        private Color foregroundColor = Color.white;
        private Color backgroundColor = Color.black;

        private static int TAB_SIZE = 8;

        // -- Read resources ----------------------------------------------------------------------------------

        private static Font ReadFont ( string fontPath )
        {
            var font = Resources.Load ( fontPath, typeof ( Font ) ) as Font;
            if ( font == null )
                UnityEngine.Debug.LogErrorFormat ( "Font is not exists: '{0}'", fontPath );
            return font;
        }

        private static Material ReadMaterial ( string materialPath )
        {
            var material = Resources.Load ( materialPath, typeof ( Material ) ) as Material;
            if ( material == null )
                UnityEngine.Debug.LogErrorFormat ( "Material is not exists: '{0}'", materialPath );
            return material;
        }
    }
}

