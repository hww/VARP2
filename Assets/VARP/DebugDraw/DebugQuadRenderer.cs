using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.DebugDraw
{
    public class DebugQuadRenderer : System.IDisposable
    {
        public class QuadPrimitive : DataStructures.LinkedListNode<QuadPrimitive>
        {
            public float hideAt;
            public Color color;
            public Vector3 point1;
            public Vector3 point2;
            public Vector3 point3;
            public Vector3 point4;

            public QuadPrimitive ( ) : base(null)
            {
                this.value = this;
            }

            public void Render ( )
            {
                GL.Color ( color );
                GL.Vertex ( point1 );
                GL.Vertex ( point2 );
                GL.Vertex ( point3 );
                GL.Vertex ( point4 );
            }
        }

        DataStructures.LinkedList<QuadPrimitive> freePrimitives = new DataStructures.LinkedList<QuadPrimitive> ( );
        DataStructures.LinkedList<QuadPrimitive> usedPrimitives = new DataStructures.LinkedList<QuadPrimitive> ( );

        public DebugQuadRenderer ( int capacity )
        {
            for ( var i = 0 ; i < capacity ; i++ )
                freePrimitives.AddFirst ( new QuadPrimitive ( ) );
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

        public void Add ( Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, Color color, float hideAt = 0 )
        {
            var primitive = freePrimitives.First;
            if ( primitive == null )
                primitive = new QuadPrimitive ( );
            else
                primitive.Remove ( );
            primitive.Value.point1 = point1;
            primitive.Value.point2 = point2;
            primitive.Value.point3 = point3;
            primitive.Value.point4 = point4;
            primitive.Value.color = color;
            primitive.Value.hideAt = hideAt;
            usedPrimitives.AddFirst ( primitive );
        }

        public void Render ( Material material )
        {
            var curent = usedPrimitives.First;
            if ( curent == null )
                return;

            material.SetPass ( 0 );
            GL.Begin ( GL.QUADS );
            while ( curent != null )
            {
                var next = curent.Next;
                if ( Time.time > curent.Value.hideAt )
                {
                    curent.Remove ( );
                    freePrimitives.AddFirst ( curent );
                }
                else
                    curent.Value.Render (  );
                curent = next;
            }
            GL.End();
        }

        public bool IsEmpty
        {
            get { return usedPrimitives.Count == 0; }
        }

        public int Count
        {
            get { return usedPrimitives.Count; }
        }
    }
}
