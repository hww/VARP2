using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.DebugDraw
{
    public class DebugTextRenderer : System.IDisposable
    {
        public class TextPrimitive : DataStructures.LinkedListNode<TextPrimitive>
        {
            public float hideAt;
            public Vector3 position;
            public float scale;
            public Color color;
            public string text;

            public TextPrimitive ( ) : base(null)
            {
                this.value = this;
            }

            public void Render ( Font font )
            {
                GL.Color ( color );
                var screenPos = Camera.current.WorldToScreenPoint ( position );
                VARP.UnityFontTools.FontRenderer.RenderText ( screenPos, font, text, scale );
            }
        }

        DataStructures.LinkedList<TextPrimitive> freePrimitives = new DataStructures.LinkedList<TextPrimitive> ( );
        DataStructures.LinkedList<TextPrimitive> usedPrimitives = new DataStructures.LinkedList<TextPrimitive> ( );

        public DebugTextRenderer ( int capacity )
        {
            for ( var i = 0 ; i < capacity ; i++ )
                freePrimitives.AddFirst ( new TextPrimitive ( ) );
        }

        public void Dispose ( )
        {
            freePrimitives.Clear ( );
            usedPrimitives.Clear ( );
        }

        public void ClearScreen ( )
        {
            var curent = usedPrimitives.First;
            while ( curent != null )
            {
                var next = curent.Next;
                curent.Remove ( );
                freePrimitives.AddFirst ( curent );
                curent = next;
            }
        }

        public void Add ( Vector3 position, string text, Color color, float scale = 1, float hideAt = 0)
        {
            var primitive = freePrimitives.First;
            if ( primitive == null )
                primitive = new TextPrimitive ( );
            else
                primitive.Remove ( );
            var textPrim = primitive.Value;
            textPrim.position = position;
            textPrim.text = text;
            textPrim.color = color;
            textPrim.scale = scale;
            textPrim.hideAt = hideAt;
            usedPrimitives.AddFirst ( primitive );
        }

        public void Render ( Font font, Material material )
        {
            var curent = usedPrimitives.First;
            if ( curent == null )
                return;
            VARP.UnityFontTools.FontRenderer.RenderTextBefore ( font, material );
            while ( curent != null )
            {
                var next = curent.Next;
                if ( Time.time > curent.Value.hideAt )
                {
                    curent.Remove ( );
                    freePrimitives.AddFirst ( curent );
                }
                else
                    curent.Value.Render ( font );
                curent = next;
            }
            VARP.UnityFontTools.FontRenderer.RenderTextAfter ( );
        }

        public bool IsEmpty
        {
            get { return usedPrimitives.Count == 0; }
        }

        public int Count
        {
            get { return usedPrimitives.Count; }
        }

        /// <summary>
        /// Converts a coordinate in pixels to screen 0-1 fraction point.
        /// Example: 400, 300, on a 800x600 screen will output 0.5, 0.5 (middle of the screen)
        /// </summary>
        public static Vector2 PixelToScreen ( Vector2 pos )
        {
            return new Vector2 ( pos.x / Screen.width, pos.y / Screen.height );
        }

        /// <summary>
        /// Converts a coordinate in pixels to screen 0-1 fraction point.
        /// Example: 400, 300, on a 800x600 screen will output 0.5, 0.5 (middle of the screen)
        /// </summary>
        public static Vector2 PixelToScreen ( float x, float y )
        {
            return new Vector2 ( x / Screen.width, y / Screen.height );
        }

    }
}
