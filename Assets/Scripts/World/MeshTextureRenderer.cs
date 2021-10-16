using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Mesh and Collider Drawer.
/// </summary>
[RequireComponent(typeof(TextureRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshTextureRenderer : MonoBehaviour
{
    /// <summary>
    /// Creates mesh and sets it as the current mesh for the attached MeshFilter.
    /// </summary>
    /// <param name="meshData">MeshData object</param>
    /// <param name="texture">Texture for mesh</param>
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;

        TextureRenderer renderer = GetComponent<TextureRenderer>();
        renderer.DrawTexture(texture);
    }

    /// <summary>
    /// Create collider from MeshData and set it at attached MeshCOllider
    /// </summary>
    /// <param name="meshData">MeshData object</param>
    public void DrawCollider(MeshData meshData)
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = meshData.CreateMesh();
    }
}
