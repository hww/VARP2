using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.DebugDraw
{
    public class DrawStrings : System.IDisposable
    {
        public class TextPrimitive : DataStructures.LinkedListNode<TextPrimitive>
        {
            public float hideAt;
            public Vector3 position;
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
                screenPos.z = 0;
                VARP.UnityFontTools.FontRenderer.RenderText ( screenPos, font, text );
            }

            public void Render3D ( Font font )
            {
                throw new System.NotImplementedException ( );
                //var camPos = Camera.current.transform.position;
                //var quaternion = Quaternion.LookRotation ( camPos - position, Vector3.up );
                //var matrix = Matrix4x4.TRS ( position, quaternion, Vector3.one );
                GL.Color ( color );
                //GL.LoadOrtho ( ); //< ????
                GL.Begin ( GL.QUADS );
                VARP.UnityFontTools.FontRenderer.RenderText ( position, font, text );
                GL.End ( );
            }
        }

        DataStructures.LinkedList<TextPrimitive> freePrimitives = new DataStructures.LinkedList<TextPrimitive> ( );
        DataStructures.LinkedList<TextPrimitive> usedPrimitives = new DataStructures.LinkedList<TextPrimitive> ( );

        public DrawStrings ( int capacity )
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

        public void Add ( Vector3 position, string text, Color color, float hideAt = 0)
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

        public void Render3D ( Font font, Material material )
        {
            var curent = usedPrimitives.First;
            if ( curent == null )
                return;
            VARP.UnityFontTools.FontRenderer.RenderTextBefore3D ( font, material );
            while ( curent != null )
            {
                var next = curent.Next;
                if ( Time.time > curent.Value.hideAt )
                {
                    curent.Remove ( );
                    freePrimitives.AddFirst ( curent );
                }
                else
                    curent.Value.Render3D ( font );
                curent = next;
            }
            VARP.UnityFontTools.FontRenderer.RenderTextAfter3D ( );
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
