using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(TextureRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshTextureRenderer : MonoBehaviour
{
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;

        TextureRenderer renderer = GetComponent<TextureRenderer>();
        renderer.DrawTexture(texture);
    }

    public void DrawCollider(MeshData meshData)
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = meshData.CreateMesh();
    }
}
