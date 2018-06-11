using System;
using UnityEngine;

namespace VARP.Terminal
{
    /** This class represent the cursor behavior for terminal emulator */
    public class TerminalCursor
    {
        const int TAB_SIZE = 8;
        private int x;
        private int y;
        public int xMin;
        public int yMin;
        public int xMax;
        public int yMax;
        public TerminalRenderer terminal;
        public TerminalCursor ( TerminalRenderer terminal)
        {
            this.terminal = terminal;
        }

        // Set window geometry
        public void SetWindow ( int xmin, int ymin, int xmax, int ymax )
        {
            Debug.Assert ( xmax > xmin && ymax > ymin );
            this.xMin = Clamp ( xmin, 0, terminal.sizeX );
            this.yMin = Clamp ( ymin, 0, terminal.sizeY );
            this.xMax = Clamp ( xmax, 0, terminal.sizeX );
            this.yMax = Clamp ( ymax, 0, terminal.sizeY );
            this.x = xmin;
            this.y = ymin;
        }
        // Set X,Y position inside given window 
        public void SetPosition ( int x, int y )
        {
            X = x;
            Y = y;
        }
        // Get/Set X position inside given window 
        public int X
        {
            get { return x; }
            set { x = ClampX ( x ); }
        }
        // Get/Set Y position inside given window 
        public int Y
        {
            get { return y; }
            set { y = ClampY ( y ); }
        }
        // Increment decrement X position of cursor and update Y position 
        // or scroll terminal if it needed
        public void AddX ( int increment )
        {
            var value = x + increment;
            if ( value < xMin )
            {
                x = xMax;
                AddY ( -1 );
            }
            else if ( x > xMax )
            {
                x = xMin;
                AddY ( 1 );
            }
            else
            {
                x = value;
            }
        }
        // Increment decrement Y position of cursor and scroll terminal if it needed
        public void AddY ( int increment )
        {
            var value = y + increment;
            if ( value < yMin )
            {
                x = yMax;
                terminal.Scroll(1);
            }
            else if ( value > xMax )
            {
                y = yMin;
                terminal.Scroll ( -1 );
            }
            else
            {
                y = value;
            }
        }
        // Move to next line 
        public void NewLine ( )
        {
            x = xMin;
            AddY ( 1 );
        }
        public int GetLastColumnOfThisTab ( )
        {
            return ClampX ( ( ( x / TAB_SIZE ) + 1 ) * TAB_SIZE - 1 );
        }

        // -- Private methods ----------------------------------------------------------------------------

        private int Clamp ( int value, int min, int max )
        {
            if ( value < min )
                return min;
            if ( value > max )
                return max;
            return value;
        }
        private int ClampX ( int value ) { return Clamp ( value, xMin, xMax ); }
        private int ClampY ( int value ) { return Clamp ( value, yMin, yMax ); }
    }

}

