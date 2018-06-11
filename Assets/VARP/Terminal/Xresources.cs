using UnityEngine;

namespace VARP.Terminal
{
    public class Xresources
    {
        // terminal colors for string paser
        public enum EColor
        {
            Black,        
            Red,          
            Green,        
            Yellow,       
            Blue,         
            Magenta,      
            Cyan,         
            LightGray,    
            DarkGray,     
            LightRed,     
            LightGreen,   
            LightYellow,  
            LightBlue,    
            LightMagenta, 
            LightCyan,    
            White         
        }
        
        // -- Public  Methods ----------------------------------------------------------------------------

        public Color this[EColor color]
        {
            get {
                return colors[ (int)color ];
            }
        }

        // -- Public  Fields -----------------------------------------------------------------------------

        public Color background = Color.black;
        public Color foreground = Color.white;
        public Color cursorColor = Color.red;
        public Color selectionColor = new Color(0,0,1,0.5f);

        public Color[] colors = new Color[]
        {
            Color.black,
            Color.black,
            Color.red,
            Color.green,
            Color.yellow,
            Color.blue,
            Color.magenta,
            Color.cyan,
            Color.gray,
            Color.gray,
            Color.red,
            Color.green,
            Color.yellow,
            Color.blue,
            Color.magenta,
            Color.cyan,
            Color.white,
        };

        // -- Privte Methods -----------------------------------------------------------------------------

        protected static Color ParseHtmlColor ( string str )
        {
            Color color;
            if ( ColorUtility.TryParseHtmlString ( str, out color ) )
                return color;
            throw new System.SystemException ( "Can't parse color: " + str );
        }
    }

 
}

