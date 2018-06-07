using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public Material mat;

    public bool onPostRender;
    public bool onRenderObject;
    void OnPostRender ( )
    {
        if ( onPostRender )
            DoRender ( );
    }

    void OnRenderObject ( )
    {
        if ( onRenderObject )
            DoRender ( );
    }

    void DoRender()
    { 
        if ( !mat )
        {
            Debug.LogError ( "Please Assign a material on the inspector" );
            return;
        }
        GL.PushMatrix ( );
        mat.SetPass ( 0 );
        GL.LoadOrtho ( );
        GL.Begin ( GL.TRIANGLES );
        GL.Color ( new Color ( 1, 1, 1, 1 ) );
        GL.Vertex3 ( 0.5F, 0.25F, 0 );
        GL.Vertex3 ( 0.25F, 0.25F, 0 );
        GL.Vertex3 ( 0.375F, 0.5F, 0 );
        GL.Vertex3 ( 2.5F, 2.25F, 0 );
        GL.Vertex3 ( 2.25F, 2.25F, 0 );
        GL.Vertex3 ( 2.375F, 2.5F, 0 );
        GL.End ( );
        GL.Begin ( GL.QUADS );
        GL.Color ( new Color ( 0.5F, 0.5F, 0.5F, 1 ) );
        GL.Vertex3 ( 0.5F, 0.5F, 0 );
        GL.Vertex3 ( 0.5F, 0.75F, 0 );
        GL.Vertex3 ( 0.75F, 0.75F, 0 );
        GL.Vertex3 ( 0.75F, 0.5F, 0 );
        GL.End ( );
        GL.Begin ( GL.LINES );
        GL.Color ( new Color ( 1, 0, 0, 1 ) );
        GL.Vertex3 ( 0, 0, 0 );
        GL.Vertex3 ( 0.75F, 0.75F, 0 );
        GL.Vertex3 ( 2.75F, 4.75F, 1 );
        GL.Vertex3 ( 5.75F, 6.0F, 1 );
        GL.End ( );
        GL.PopMatrix ( );
    }
}