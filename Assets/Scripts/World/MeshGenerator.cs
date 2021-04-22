using System;
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
    public static MeshData GeneratePlaneMesh(float width, float height)
    {
        MeshData meshData = new MeshData(2, 2);
        float centerOffsetX = (width - 1) / 2.0f;
        float centerOffsetY = (height - 1) / 2.0f;

        for (int vertexIndex = 0, y = 0; y <= 1; y += 1)
            for (int x = 0; x <= 1; x += 1)
            {
                meshData.vertices[vertexIndex] = new Vector3(x * width - centerOffsetX, 0, centerOffsetY - y * height);
                meshData.uv[vertexIndex] = new Vector2(x, y);

                vertexIndex++;
            }

        meshData.AddTriangle(0, 1, 2);
        meshData.AddTriangle(1, 3, 2);

        return meshData;
    }

    public static MeshData GenerateTerrainMesh(float[,] map, int lod, float heightFactor, AnimationCurve heightFunction)
    {
        if (lod < 0 || lod > 6)
            throw new ArgumentException("LOD for mesh generation must be in [0, 6]");

        // Clone heightFunction for multi-threaded access
        heightFunction = new AnimationCurve(heightFunction.keys);

        int width = map.GetLength(0);
        int height = map.GetLength(1);

        float centerOffsetX = (width - 1) / 2.0f;
        float centerOffsetY = (height - 1) / 2.0f;

        lod = (lod == 0) ? 1 : lod * 2;
        int verticesPerLine = (width - 1) / lod + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        for (int vertexIndex = 0, y = 0; y < height; y += lod)
            for (int x = 0; x < width; x += lod)
            {
                meshData.vertices[vertexIndex] = new Vector3(x - centerOffsetX, heightFunction.Evaluate(map[x, y]) * heightFactor, centerOffsetY - y);
                meshData.uv[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }

        return meshData;
    }
}
