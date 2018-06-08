using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.DebugDraw
{
    public class DrawTriangles : System.IDisposable
    {
        public class TrianglePrimitive : DataStructures.LinkedListNode<TrianglePrimitive>
        {
            public float hideAt;
            public Color color;
            public Vector3 point1;
            public Vector3 point2;
            public Vector3 point3;

            public TrianglePrimitive ( ) : base(null)
            {
                this.value = this;
            }

            public void Render ( )
            {
                GL.Color ( color );
                GL.Vertex ( point1 );
                GL.Vertex ( point2 );
                GL.Vertex ( point3 );
            }

            public void RenderLines ( )
            {
                GL.Color ( color );
                GL.Vertex ( point1 );
                GL.Vertex ( point2 );
                GL.Vertex ( point2 );
                GL.Vertex ( point3 );
                GL.Vertex ( point3 );
                GL.Vertex ( point1 );
            }
        }

        DataStructures.LinkedList<TrianglePrimitive> freePrimitives = new DataStructures.LinkedList<TrianglePrimitive> ( );
        DataStructures.LinkedList<TrianglePrimitive> usedPrimitives = new DataStructures.LinkedList<TrianglePrimitive> ( );

        public DrawTriangles ( int capacity )
        {
            for ( var i = 0 ; i < capacity ; i++ )
                freePrimitives.AddFirst ( new TrianglePrimitive ( ) );
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

        public void Add ( Vector3 point1, Vector3 point2, Vector3 point3, Color color, float hideAt = 0 )
        {
            var primitive = freePrimitives.First;
            if ( primitive == null )
                primitive = new TrianglePrimitive ( );
            else
                primitive.Remove ( );
            primitive.Value.point1 = point1;
            primitive.Value.point2 = point2;
            primitive.Value.point3 = point3;
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
            GL.Begin ( GL.TRIANGLES );
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

        public void RenderLines ( Material material )
        {
            var curent = usedPrimitives.First;
            if ( curent != null )
                material.SetPass ( 0 );
            while ( curent != null )
            {
                var next = curent.Next;
                curent.Value.RenderLines ( );
                if ( Time.time >= curent.Value.hideAt )
                {
                    curent.Remove ( );
                    freePrimitives.AddFirst ( curent );
                }
                curent = next;
            }
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
