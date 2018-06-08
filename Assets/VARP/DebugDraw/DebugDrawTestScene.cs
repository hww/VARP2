using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VARP.DebugDraw;

public class DebugDrawTestScene : MonoBehaviour {

    [Header("When to Draw")]
    public bool update;
    public bool onDrawGizmos;

    public enum Primitive
    {
        Line,
        Ray,
        Triangle,
        Quad,
        Plane,
        Cross,
        Circle,
        Sphere,
        Box,
        Axes,
        Text
    }

    public Primitive primitive;
    public bool zTestOn;
    public float duration = 0;
    public float size = 1f;
    public Color color = Color.white;
    [TextArea(10,15)]
    public string text;
    [Header ( "Status" )]
    public Vector3 screenPos;

    private void Update ( )
    {
        if ( update )
            Render ( );
    }

    private void OnDrawGizmos ( )
    {
        if ( onDrawGizmos )
            Render ( );
    }

    private void OnRenderObject ( )
    {
        screenPos = Camera.current.WorldToScreenPoint ( transform.position );
    }

    private void Render ( )
    {
        switch ( primitive )
        {
            case Primitive.Line:
                DebugDraw.AddLine ( transform.position, Vector3.zero, color, 1, duration, zTestOn );
                break;
            case Primitive.Ray:
                DebugDraw.AddRay ( transform.position, transform.forward, color, 1, duration, zTestOn );
                break;
            case Primitive.Triangle:
                DebugDraw.AddTriangle ( transform.position, transform.position + transform.forward, transform.position + transform.up, color, duration, 1, zTestOn );
                break;
            case Primitive.Quad:
                DebugDraw.AddQuad ( transform.position, 
                    transform.position + transform.forward, 
                    transform.position + transform.up, 
                    transform.position + transform.right, color, duration, 1, zTestOn );
                break;
            case Primitive.Plane:
                DebugDraw.AddPlane ( transform.position, transform.forward, size, color, duration, zTestOn );
                break;
            case Primitive.Cross:
                DebugDraw.AddCross ( transform.position, size, color, duration, zTestOn );
                break;
            case Primitive.Circle:
                DebugDraw.AddCircle ( transform.position, transform.forward, size, color, duration, zTestOn );
                break;
            case Primitive.Sphere:
                DebugDraw.AddSphere ( transform.position, size, color, duration, zTestOn );
                break;
            case Primitive.Box:
                DebugDraw.AddBox ( transform.position, transform.rotation, transform.localScale, color, duration, zTestOn );
                break;
            case Primitive.Axes:
                DebugDraw.AddAxes ( transform, size, color, duration, zTestOn );
                break;
            case Primitive.Text:
                DebugDraw.AddText ( transform.position, text, color, duration, zTestOn );
                break;
        }
    }


}
