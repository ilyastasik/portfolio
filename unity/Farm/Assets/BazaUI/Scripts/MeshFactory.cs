using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFactory : MonoBehaviour
{
    [NonSerialized] public Mesh quad;

    public void Init() {
        quad = Quad();
    }

    public Mesh Quad() {
        Mesh mesh = new Mesh();

        mesh.SetVertices(new List<Vector3>() {
            new Vector3(0,0),
            new Vector3(0,1),
            new Vector3(1,1),
            new Vector3(1,0),
        });

        mesh.SetTriangles(new int[] {
            0,1,2,
            0,2,3,
        }, 0);

        mesh.SetColors(new List<Color>() {
            Color.white,
            Color.white,
            Color.white,
            Color.white
        });

        mesh.SetUVs(0, new List<Vector2>() {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0)
        });

        return mesh;
    }

}

public class Mesh_Quad {

}

public class Mesh_Slice9 {

    public Mesh mesh;
    Vector3[] verts;
    int[] tris;
    Vector4[] uvs;
    Color[] colors;

    const int ROW = 4;
    const int VERTS_COUNT = 16;
    const int QUADS_COUNT = 9;

    public void Create() {
        mesh = new Mesh();

        verts = new Vector3[VERTS_COUNT];
        tris = new int[QUADS_COUNT * 6];
        uvs = new Vector4[VERTS_COUNT];
        colors = new Color[VERTS_COUNT];

        for (int i = 0; i < colors.Length; i++) {
            colors[i] = Color.white;
        }

        int ind = 0;
        for (int i = 0; i < QUADS_COUNT; i++) {
            int x = i % 3;
            int y = i / 3;

            tris[ind++] = y * 4 + x;
            tris[ind++] = (y + 1) * 4 + x;
            tris[ind++] = (y + 1) * 4 + (x + 1);

            tris[ind++] = y * 4 + x;
            tris[ind++] = (y + 1) * 4 + (x + 1);
            tris[ind++] = y * 4 + (x + 1);
        }

        SetSize(new Vector2(1, 1), new Vector4(0.1f, 0.1f, 0.1f, 0.1f),
            Vector4.zero);

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris,0);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <param name="borders">Left; Bottom; Right; Top;</param>
    public void SetSize(Vector2 size, Vector4 borders, Vector4 spriteBorders) {
        for (int x = 0; x < ROW; x++) {
            for (int y = 0; y < ROW; y++) {
                int ind = y * ROW + x;

                int xIsEdge = (x == 0 || x == 3) ? 1 : 0;
                int xSign = x >= 2 ? 1 : -1;

                float px = -xSign * borders[1 + xSign] * (1 - xIsEdge);
                float vx = 0.5f * size.x * xSign + px;

                int yIsEdge = (y == 0 || y == 3) ? 1 : 0;
                int ySign = y >= 2 ? 1 : -1;

                float py = -ySign * borders[2 + ySign] * (1 - yIsEdge);
                float vy = 0.5f * size.y * ySign + py;

                float uvx = xSign * (1 - spriteBorders[1 + xSign] * (1 - xIsEdge));
                float uvy = ySign * (1 - spriteBorders[2 + ySign] * (1 - yIsEdge));

                verts[ind] = new Vector3(vx, vy, 0);
                //uvs[ind] = new Vector4(0.5f + 0.5f * px,0.5f + 0.5f * py);
                uvs[ind] = new Vector4(0.5f + 0.5f * uvx, 0.5f + 0.5f * uvy);
            }
        }

        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);

        /*
        // corners
        verts[0] = new Vector3(-size.x * 0.5f, -size.y * 0.5f);
        verts[12] = new Vector3(-size.x * 0.5f, size.y * 0.5f);
        verts[15] = new Vector3(size.x * 0.5f, size.y * 0.5f);
        verts[3] = new Vector3(size.x * 0.5f, -size.y * 0.5f);

        */
    }

    /*
    
     int ind = y * ROW + x;

                int xIsEdge = (x == 0 || x == 3) ? 1 : 0;
                int xSign = x >= 2 ? 1 : -1;

                float px = xSign * (1 - borders[1 + xSign] * (1 - xIsEdge));
                float vx = 0.5f * size.x * (0.5f + px);

                int yIsEdge = (y == 0 || y == 3) ? 1 : 0;
                int ySign = y >= 2 ? 1 : -1;

                float py = ySign * (1 - borders[2 + ySign] * (1 - yIsEdge));
                float vy = 0.5f * size.y * (0.5f + py);

                float uvx = xSign * (1 - spriteBorders[1 + xSign] * (1 - xIsEdge));
                float uvy = ySign * (1 - spriteBorders[2 + ySign] * (1 - yIsEdge));

                verts[ind] = new Vector3(vx, vy, 0);
                //uvs[ind] = new Vector4(0.5f + 0.5f * px,0.5f + 0.5f * py);
                uvs[ind] = new Vector4(0.5f + 0.5f * uvx, 0.5f + 0.5f * uvy);

    */

}
