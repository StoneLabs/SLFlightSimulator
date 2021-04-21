using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] indices;
    public Vector2[] uv;

    int currentIndex = 0;

    public MeshData(int width, int height)
    {
        vertices = new Vector3[width * height];
        indices = new int[(width - 1) * (height - 1) * 6];
        uv = new Vector2[width * height];
    }

    public void AddTriangle(int a, int b, int c)
    {
        indices[currentIndex++] = a;
        indices[currentIndex++] = b;
        indices[currentIndex++] = c;
    }
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        return mesh;
    }
}

public class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        float centerOffsetX = (width - 1) / 2.0f;
        float centerOffsetY = (height - 1) / 2.0f;

        MeshData meshData = new MeshData(width, height);

        for (int vertexIndex = 0, y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(x - centerOffsetX, map[x, y] * 10, centerOffsetY - y);
                meshData.uv[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }

        return meshData;
    }
}
