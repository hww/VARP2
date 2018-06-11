

namespace VARP.Terminal
{
    public class TangoTheme : Xresources
    {
        public TangoTheme ( )
        {
            // Tango color palette
            background = ParseHtmlColor ( "#2e3436" );
            foreground = ParseHtmlColor ( "#eeeeec" );
            cursorColor = ParseHtmlColor ( "#8ae234" );
            // foreground color for underline
            selectionColor = ParseHtmlColor ( "#808ae234" );
            // black dark/light
            colors[ 0 ] = ParseHtmlColor ( "#2e3436" );
            colors[ 8 ] = ParseHtmlColor ( "#6e706b" );
            // red dark/light
            colors[ 1 ] = ParseHtmlColor ( "#cc0000" );
            colors[ 9 ] = ParseHtmlColor ( "#ef2929" );
            // green dark/light
            colors[ 2 ] = ParseHtmlColor ( "#4e9a06" );
            colors[ 10 ] = ParseHtmlColor ( "#8ae234" );
            // yellow dark/light
            colors[ 3 ] = ParseHtmlColor ( "#edd400" );
            colors[ 11 ] = ParseHtmlColor ( "#fce94f" );
            // blue dark/light
            colors[ 4 ] = ParseHtmlColor ( "#3465a4" );
            colors[ 12 ] = ParseHtmlColor ( "#729fcf" );
            // magenta dark/light
            colors[ 5 ] = ParseHtmlColor ( "#92659a" );
            colors[ 13 ] = ParseHtmlColor ( "#c19fbe" );
            // cyan dark/light
            colors[ 6 ] = ParseHtmlColor ( "#07c7ca" );
            colors[ 14 ] = ParseHtmlColor ( "#63e9e9" );
            // white dark/light
            colors[ 7 ] = ParseHtmlColor ( "#d3d7cf" );
            colors[ 15 ] = ParseHtmlColor ( "#eeeeec" );
        }


    }

}

