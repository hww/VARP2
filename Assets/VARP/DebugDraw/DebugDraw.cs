using UnityEngine;

namespace VARP.DebugDraw
{
    public partial class DebugDraw 
    {
        public static void AddLine ( Vector3 point1,
                        Vector3 point2,
                        Color color,
                        float lineWidth = 1f,
                        float duration = 0f,
                        bool depthEnabled = true )
        {
            var renderer = depthEnabled ? LinesZOn : LinesZOff;
            renderer.Add ( point1, point2, color, Time.time + duration );
        }

        public static void AddRay ( Vector3 point1,
                Vector3 direction,
                Color color,
                float lineWidth = 1f,
                float duration = 0f,
                bool depthEnabled = true )
        {
            var renderer = depthEnabled ? LinesZOn : LinesZOff;
            renderer.Add ( point1, point1 + direction, color, Time.time + duration );
            AddBox ( point1, Quaternion.identity, centerPointBoxSize, color, duration );
        }

        public static void AddTriangle ( Vector3 vertex1,
                          Vector3 vertex2,
                          Vector3 vertex3,
                          Color color,
                          float lineWidth = 1f,
                          float duration = 0f,
                          bool depthEnabled = true )
        {
            var renderer = depthEnabled ? TrianglesZOn : TrianglesZOff;
            renderer.Add ( vertex1, vertex2, vertex3, color, Time.time + duration );
        }

        public static void AddQuad ( Vector3 vertex1,
                  Vector3 vertex2,
                  Vector3 vertex3,
                  Vector3 vertex4,
                  Color color,
                  float lineWidth = 1f,
                  float duration = 0f,
                  bool depthEnabled = true )
        {
            var renderer = depthEnabled ? QuadsZOn : QuadsZOff;
            renderer.Add ( vertex1, vertex2, vertex3, vertex4, color, Time.time + duration );
        }

        public static void AddCross ( Vector3 position,
                        float size,
                        Color color,
                        float duration = 0f,
                        bool depthEnabled = true )
        {
            var up = Vector3.up * size;
            var right = Vector3.right * size;
            var forward = Vector3.forward * size;
            var renderer = depthEnabled ? LinesZOn : LinesZOff;
            var hideAt = Time.time + duration;
            renderer.Add ( position - up, position + up, color, hideAt );
            renderer.Add ( position - right, position + right, color, hideAt );
            renderer.Add ( position - forward, position + forward, color, hideAt );
        }

        public static void AddCircle ( Vector3 center,
                        Vector3 normal,
                        float radius,
                        Color color,
                        float duration = 0f,
                        bool depthEnabled = true )
        {
            var renderer = depthEnabled ? CirclesZOn : CirclesZOff;
            renderer.Add ( center, normal.normalized, radius, color, Time.time + duration );
        }

        static int radialSegments = 4;
        static int verticalSegments = 4;

        public static void AddSphere ( Vector3 center,
                        float radius,
                        Color color,
                        float duration = 0f,
                        bool depthEnabled = true )
        {

            var renderer = depthEnabled ? CirclesZOn : CirclesZOff;
            var hideAt = Time.time + duration;
            if ( radialSegments > 2 )
            {
                for ( int i = 0 ; i < radialSegments ; i++ )
                {
                    Vector3 normal = new Vector3 ( Mathf.Sin ( ( i * Mathf.PI ) / radialSegments ), 0, Mathf.Cos ( ( i * Mathf.PI ) / radialSegments ) );
                    renderer.Add ( center, normal, radius, color, hideAt );
                }
            }
            else
            {
                renderer.Add ( center, Vector3.forward, radius, color, hideAt );
                renderer.Add ( center, Vector3.right, radius, color, hideAt );
            }

            if ( verticalSegments > 1 )
            {
                for ( int i = 1 ; i < verticalSegments ; i++ )
                {
                    Vector3 c = center + Vector3.up * ( -radius + ( i * 2 * ( radius / ( verticalSegments ) ) ) );

                    // Radius of base circle is a=sqrt(h(2R-h)), 
                    float height = ( (float)i / verticalSegments ) * radius * 2;
                    float ra = Mathf.Sqrt ( height * ( 2 * radius - height ) );

                    renderer.Add ( c, Vector3.up, ra, color, hideAt );
                }
            }
            else
            {
                renderer.Add ( center, Vector3.up, radius, color, hideAt );
            }
        }


        public static void AddBox ( Vector3 center,
                        Quaternion rotation,
                        Vector3 size,
                        Color color,
                        float duration = 0f,
                        bool depthEnabled = true )
        {
            Vector3 hSize = size * 0.5f;

            // Calculate max and min point
            Vector3 min = center + rotation * -hSize;
            Vector3 max = center + rotation * hSize;

            Vector3 x = rotation * new Vector3 ( size.x, 0, 0 );
            Vector3 y = rotation * new Vector3 ( 0, size.y, 0 );
            Vector3 z = rotation * new Vector3 ( 0, 0, size.z );

            var renderer = depthEnabled ? LinesZOn : LinesZOff;
            var hideAt = Time.time + duration;
            // Bottom
            renderer.Add ( min, min + x, color, hideAt );
            renderer.Add ( min + x, min + x + z, color, hideAt );
            renderer.Add ( min + x + z, min + z, color, hideAt );
            renderer.Add ( min + z, min, color, hideAt );

            // Top
            renderer.Add ( max, max - x, color, duration );
            renderer.Add ( max - x, max - x - z, color, hideAt );
            renderer.Add ( max - x - z, max - z, color, hideAt );
            renderer.Add ( max - z, max, color, hideAt );

            // Side
            renderer.Add ( min, min + y, color, hideAt );
            renderer.Add ( min + x, min + x + y, color, hideAt );
            renderer.Add ( min + x + z, min + x + z + y, color, hideAt );
            renderer.Add ( min + z, min + z + y, color, hideAt );
        }


        public static void AddPlane ( Vector3 center,
                Vector3 normal,
                float size,
                Color color,
                float duration = 0f,
                bool depthEnabled = true )
        {
            normal.Normalize ( );
            var halfsize = size * 0.5f;
            var forward = normal == Vector3.up ?
                Vector3.ProjectOnPlane ( Vector3.forward, normal ).normalized :
                Vector3.ProjectOnPlane ( Vector3.up, normal );
            var right = Vector3.Cross ( normal, forward );
            AddLine ( center, center + normal, color, duration );

            forward *= halfsize;
            right *= halfsize;

            var p1 = center + forward + right;
            var p2 = center + forward - right;
            var p3 = center - forward - right;
            var p4 = center - forward + right;

            AddQuad ( p1, p2, p3, p4, color, duration );
        }

        static readonly Vector3 centerPointBoxSize = Vector3.one * 0.05f;

        public static void AddAxes ( Transform transform,
                      float size,
                      Color color,
                      float duration = 0f,
                      bool depthEnabled = true )
        {
            var center = transform.position;
            var forward = transform.forward;
            var right = transform.right;
            var up = transform.up;

            AddLine ( center, center + right, Color.red, duration );
            AddLine ( center, center + forward, Color.blue, duration );
            AddLine ( center, center + up, Color.green, duration );
            AddBox ( center, transform.rotation, centerPointBoxSize, color, duration );
        }


        public static void AddAABB ( Vector3 minCoords,
                      Vector3 maxCoords,
                      Color color,
                      float lineWidth = 1f,
                      float duration = 0f,
                      bool depthEnabled = true )
        {

            var size = ( maxCoords - minCoords ) * 0.5f;
            var center = minCoords + size * 0.5f;
            AddBox ( center,
                     Quaternion.identity,
                     size,
                     color,
                     duration,
                     depthEnabled );
        }

        public static void AddOBB ( Matrix4x4 centerTransform,
                     Vector3 scaleXYZ,
                     Vector3 maxCoords,
                     Color color,
                     float lineWidth = 1f,
                     float duration = 0f,
                     bool depthEnabled = true )
        {
            throw new System.NotImplementedException ( );
        }

        public static void AddText ( Vector3 position,
            string text,
            Color color,
            float duration = 0f,
            bool depthEnabled = true )
        {
           // Debug.Assert ( depthEnabled == false, "Not implemented shader");
            var renderer = depthEnabled ? StringsZOn : StringsZOff;
            renderer.Add ( position, text, color, Time.time + duration );
        }

        public static void Init()
        {
            if ( IsInitialized )
                return;
            IsInitialized = true;

            TextFont = ReadFont ( "VARP/DebugDraw/GLFont" );

            LineMaterialZOff = ReadMaterial ( "VARP/DebugDraw/GLlineZOff" );
            LineMaterialZOn = ReadMaterial ( "VARP/DebugDraw/GLlineZOn" );
            TextMaterialZOff = ReadMaterial ( "VARP/DebugDraw/GLFontZOff" );
            TextMaterialZOn = ReadMaterial ( "VARP/DebugDraw/GLFontZOn" );

            LinesZOff = new DrawLines ( INITIAL_PRIMITIVES_QUANTITY );
            QuadsZOff = new DrawQuads ( INITIAL_PRIMITIVES_QUANTITY );
            TrianglesZOff = new DrawTriangles ( INITIAL_PRIMITIVES_QUANTITY );
            LinesZOn = new DrawLines ( INITIAL_PRIMITIVES_QUANTITY );
            QuadsZOn = new DrawQuads ( INITIAL_PRIMITIVES_QUANTITY );
            TrianglesZOn = new DrawTriangles ( INITIAL_PRIMITIVES_QUANTITY );

            CirclesZOff = new DrawCircles ( INITIAL_PRIMITIVES_QUANTITY );
            CirclesZOn = new DrawCircles ( INITIAL_PRIMITIVES_QUANTITY );

            StringsZOff = new DrawStrings ( INITIAL_PRIMITIVES_QUANTITY );
            StringsZOn = new DrawStrings ( INITIAL_PRIMITIVES_QUANTITY );
        }

        private static Font ReadFont( string fontPath )
        {
            var font = Resources.Load ( fontPath, typeof ( Font ) ) as Font;
            if ( font == null )
                Debug.LogErrorFormat ( "Font is not exists: '{0}'", fontPath );
            return font;
        }

        private static Material ReadMaterial(string materialPath )
        {
            var material = Resources.Load ( materialPath, typeof ( Material ) ) as Material;
            if ( material == null )
                Debug.LogErrorFormat ( "Material is not exists: '{0}'", materialPath );
            return material;
        }

        public static void DeInit()
        {
            IsInitialized = false;

            LinesZOff.Dispose ( );
            QuadsZOff.Dispose ( );
            TrianglesZOff.Dispose ( );
            CirclesZOff.Dispose ( );
            StringsZOff.Dispose ( );

            LinesZOn.Dispose ( );
            QuadsZOn.Dispose ( );
            TrianglesZOn.Dispose ( );
            CirclesZOn.Dispose ( );
            StringsZOn.Dispose ( );

            LineMaterialZOff = null;
            LineMaterialZOn = null;
            TextMaterialZOff = null;
            TextMaterialZOn = null;
            TextFont = null;
        }

        public static void PostInit ( )
        {

        }

        public static void Render ( )
        {
            GL.PushMatrix ( );
            GL.Color ( Color.white );

            try
            {
                // Depth Test On
                LinesZOn.Render ( LineMaterialZOn );
                CirclesZOn.Render ( LineMaterialZOn );
                TrianglesZOn.Render ( LineMaterialZOn );
                QuadsZOn.Render ( LineMaterialZOn );
                StringsZOn.Render3D ( TextFont, TextMaterialZOn );

                // Depth Test Off
                LinesZOff.Render ( LineMaterialZOff );
                CirclesZOff.Render ( LineMaterialZOff );
                TrianglesZOff.Render ( LineMaterialZOff );
                QuadsZOff.Render ( LineMaterialZOff );
                StringsZOff.Render ( TextFont, TextMaterialZOff );
            }
            catch (System.Exception ex)
            {
                Debug.LogException ( ex );
                GL.Color ( Color.white );
                GL.PopMatrix ( );
                return;
            }

            GL.Color ( Color.white );
            GL.PopMatrix ( );
        }

        const int INITIAL_PRIMITIVES_QUANTITY = 100;

        static bool IsInitialized;

        static Material LineMaterialZOff;
        static Material LineMaterialZOn;
        static Material TextMaterialZOff;
        static Material TextMaterialZOn;
        static Font TextFont;

        static DrawLines LinesZOff;
        static DrawQuads QuadsZOff;
        static DrawCircles CirclesZOff;
        static DrawStrings StringsZOff;
        static DrawTriangles TrianglesZOff;

        static DrawLines LinesZOn;
        static DrawQuads QuadsZOn;
        static DrawCircles CirclesZOn;
        static DrawStrings StringsZOn;
        static DrawTriangles TrianglesZOn;
    }
}