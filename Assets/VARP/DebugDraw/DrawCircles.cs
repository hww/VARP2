using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CSE0003

namespace VARP.DebugDraw
{
    public unsafe class DrawCircles : System.IDisposable
    {
        public class CirclePrimitive : DataStructures.LinkedListNode<CirclePrimitive>
        {
            public float hideAt;
            public Color color;
            public Vector3 center;
            public Vector3 forward;
            public Vector3 right;
            public float radius;

            public CirclePrimitive ( ) : base(null)
            {
                this.value = this;
            }

            public void Render ( )
            {
                GL.Begin ( GL.LINES );
                GL.Color ( color );
                for ( float theta = 0.0f ; theta < ( 2 * Mathf.PI ) ; theta += 0.01f )
                {
                    Vector3 ci = center + forward * Mathf.Cos ( theta ) * radius + right * Mathf.Sin ( theta ) * radius;
                    GL.Vertex ( ci );

                    if ( theta != 0 )
                        GL.Vertex ( ci );
                }
                GL.End (  );
            }

        }

        DataStructures.LinkedList<CirclePrimitive> freePrimitives = new DataStructures.LinkedList<CirclePrimitive> ( );
        DataStructures.LinkedList<CirclePrimitive> usedPrimitives = new DataStructures.LinkedList<CirclePrimitive> ( );

        public DrawCircles ( int capacity )
        {
            for ( var i = 0 ; i < capacity ; i++ )
                freePrimitives.AddFirst ( new CirclePrimitive ( ) );
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

        public void Add ( Vector3 center, Vector3 normal, float radius, Color color, float hideAt = 0 )
        {
            var primitive = freePrimitives.First;
            if ( primitive == null )
                primitive = new CirclePrimitive ( );
            else
                primitive.Remove ( );

            normal = normal.normalized;
            Vector3 forward = normal == Vector3.up ? 
                Vector3.ProjectOnPlane ( Vector3.forward, normal ).normalized : Vector3.ProjectOnPlane ( Vector3.up, normal ).normalized;
            Vector3 right = Vector3.Cross ( normal, forward );
            primitive.Value.center = center;
            primitive.Value.forward = forward;
            primitive.Value.right = right;
            primitive.Value.radius = radius;
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
