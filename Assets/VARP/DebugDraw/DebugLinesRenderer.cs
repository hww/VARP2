using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.DebugDraw
{
    public unsafe class DebugLineRenderer : System.IDisposable
    {
        public class LinePrimitive : DataStructures.LinkedListNode<LinePrimitive>
        {
            public float hideAt;
            public Color color;
            public Vector3 point1;
            public Vector3 point2;

            public LinePrimitive() : base(null)
            {
                this.value = this;
            }

            public void Render ( )
            {
                GL.Color ( color );
                GL.Vertex ( point1 );
                GL.Vertex ( point2 );
            }
        }

        DataStructures.LinkedList<LinePrimitive> freePrimitives = new DataStructures.LinkedList<LinePrimitive> ( );
        DataStructures.LinkedList<LinePrimitive> usedPrimitives = new DataStructures.LinkedList<LinePrimitive> ( );

        public DebugLineRenderer ( int capacity )
        {
            for ( var i = 0 ; i < capacity ; i++ )
                freePrimitives.AddFirst ( new LinePrimitive ( ) );
        }

        public void Dispose ( )
        {
            freePrimitives.Clear();
            usedPrimitives.Clear ( );
        }

        public void ClearScreen ( )
        {
            var curent = usedPrimitives.First;
            while (curent != null)
            {
                var next = curent.Next;
                curent.Remove ( );
                freePrimitives.AddFirst ( curent );
                curent = next;
            }
        }

        public void Add ( Vector3 point1, Vector3 point2, Color color, float hideAt = 0 )
        {
            var primitive = freePrimitives.First;
            if ( primitive == null )
                primitive = new LinePrimitive ( );
            else
                primitive.Remove ( );
            primitive.Value.point1 = point1;
            primitive.Value.point2 = point2;
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
            GL.Begin ( GL.LINES );
            while ( curent != null )
            {
                var next = curent.Next;
                if ( Time.time > curent.Value.hideAt )
                {
                    curent.Remove ( );
                    freePrimitives.AddFirst ( curent );
                }
                else
                    curent.Value.Render ( );
                curent = next;
            }
            GL.End ( );
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
