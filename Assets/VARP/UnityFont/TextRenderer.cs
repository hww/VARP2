using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.UnityFontTools
{
    public class FontRenderer 
    {
        public static void RenderTextBefore ( Font font, Material material )
        {
            material.SetPass ( 0 );
            GL.invertCulling = true;
            GL.PushMatrix ( );
            // Screenspace is defined in pixels.The bottom-left of the screen is (0,0);
            // the right-top is ( pixelWidth, pixelHeight ).The z position is in world units from the camera. 
            GL.LoadPixelMatrix ( );
            GL.Begin ( GL.QUADS );
        }


        public static void RenderTextAfter ( )
        {
            GL.End ( );
            GL.PopMatrix ( );
            GL.invertCulling = false;
        }

        public static void RenderTextBefore3D ( Font font, Material material )
        {
            material.SetPass ( 0 );
            GL.invertCulling = true;
            GL.PushMatrix ( );
        }

        public static void RenderTextAfter3D ( )
        {
            GL.PopMatrix ( );
            GL.invertCulling = false;
        }

        const int TAB_SIZE = 8;

        public static void RenderText ( Vector3 position, Font font, string text )
        {
            CharacterInfo info;
            CharacterInfo space;
            font.GetCharacterInfo ( ' ', out space );
            Vector3 pos = position;
            pos.y -= font.lineHeight;
            var x = 0;
            var lineHeight = font.lineHeight;
            var spaceWidth = space.advance;
            foreach ( char c in text )
            {
                switch ( c )
                {
                    case '\n':
                        x = 0;
                        pos.x = position.x;
                        pos.y -= lineHeight;
                        break;
                    case '\t':
                        var nextTabColumn = TAB_SIZE * ( x / TAB_SIZE + 1 );
                        pos.x = position.x + spaceWidth * nextTabColumn;
                        x = nextTabColumn;
                        break;
                    case ' ':
                        pos.x += spaceWidth;
                        x++;
                        break;
                    default:
                        if ( font.GetCharacterInfo ( c, out info ) )
                        {
                            GL.MultiTexCoord ( 0, info.uvTopLeft );
                            GL.Vertex ( pos + new Vector3 ( info.minX, info.maxY, 0 ) );
                            GL.MultiTexCoord ( 0, info.uvTopRight );
                            GL.Vertex ( pos + new Vector3 ( info.maxX, info.maxY, 0 ) );
                            GL.MultiTexCoord ( 0, info.uvBottomRight );
                            GL.Vertex ( pos + new Vector3 ( info.maxX, info.minY, 0 ) );
                            GL.MultiTexCoord ( 0, info.uvBottomLeft );
                            GL.Vertex ( pos + new Vector3 ( info.minX, info.minY, 0 ) );
                            pos.x += info.advance;
                            x++;
                        }
                        break;
                }

            }
        }

        public static void RenderText ( Vector3 position, Font font, string text, float scale )
        {
            CharacterInfo info;
            CharacterInfo space;
            font.GetCharacterInfo ( ' ', out space );
            Vector3 pos = position;
            pos.y -= font.lineHeight * scale;
            pos.z = 0;
            var x = 0;
            var lineHeight = font.lineHeight * scale;
            var spaceWidth = space.advance * scale;
            foreach ( char c in text )
            {
                switch ( c )
                {
                    case '\n':
                        x = 0;
                        pos.x = position.x;
                        pos.y -= lineHeight;
                        break;
                    case '\t':
                        var nextTabColumn = TAB_SIZE * ( x / TAB_SIZE + 1 );
                        pos.x = position.x + spaceWidth * nextTabColumn;
                        x = nextTabColumn;
                        break;
                    case ' ':
                        pos.x += spaceWidth;
                        x++;
                        break;
                    default:
                        if ( font.GetCharacterInfo ( c, out info ) )
                        {

                            GL.MultiTexCoord ( 0, info.uvTopLeft );
                            GL.Vertex ( pos + new Vector3 ( info.minX * scale, info.maxY * scale, 0 ) );

                            GL.MultiTexCoord ( 0, info.uvTopRight );
                            GL.Vertex ( pos + new Vector3 ( info.maxX * scale, info.maxY * scale, 0 ) );

                            GL.MultiTexCoord ( 0, info.uvBottomRight );
                            GL.Vertex ( pos + new Vector3 ( info.maxX * scale, info.minY * scale, 0 ) );

                            GL.MultiTexCoord ( 0, info.uvBottomLeft );
                            GL.Vertex ( pos + new Vector3 ( info.minX * scale, info.minY * scale, 0 ) );

                            pos.x += info.advance * scale;
                            x++;
                        }
                        break;
                }
            }
        }

        public static void BuildTextMesh ( string str, Font font, ref Vector3[] vertices, ref int[] triangles, ref Vector2[] uv )
        {
            // Generate a mesh for the characters we want to print.
            vertices = new Vector3[ str.Length * 4 ];
            triangles = new int[ str.Length * 6 ];
            uv = new Vector2[ str.Length * 4 ];
            Vector3 pos = Vector3.zero;
            for ( int i = 0 ; i < str.Length ; i++ )
            {
                // Get character rendering information from the font
                CharacterInfo info;
                font.GetCharacterInfo ( str[ i ], out info );

                vertices[ 4 * i + 0 ] = pos + new Vector3 ( info.minX, info.maxY, 0 );
                vertices[ 4 * i + 1 ] = pos + new Vector3 ( info.maxX, info.maxY, 0 );
                vertices[ 4 * i + 2 ] = pos + new Vector3 ( info.maxX, info.minY, 0 );
                vertices[ 4 * i + 3 ] = pos + new Vector3 ( info.minX, info.minY, 0 );

                uv[ 4 * i + 0 ] = info.uvTopLeft;
                uv[ 4 * i + 1 ] = info.uvTopRight;
                uv[ 4 * i + 2 ] = info.uvBottomRight;
                uv[ 4 * i + 3 ] = info.uvBottomLeft;

                triangles[ 6 * i + 0 ] = 4 * i + 0;
                triangles[ 6 * i + 1 ] = 4 * i + 1;
                triangles[ 6 * i + 2 ] = 4 * i + 2;

                triangles[ 6 * i + 3 ] = 4 * i + 0;
                triangles[ 6 * i + 4 ] = 4 * i + 2;
                triangles[ 6 * i + 5 ] = 4 * i + 3;

                // Advance character position
                pos += new Vector3 ( info.advance, 0, 0 );
            }
        }

        public static void BuildTextMesh ( string str, Font font, Mesh mesh )
        {
            Vector3[] vertices = null;
            int[] triangles = null;
            Vector2[] uv = null;
            BuildTextMesh ( str, font, ref vertices, ref triangles, ref uv );
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
        }

    }

}
