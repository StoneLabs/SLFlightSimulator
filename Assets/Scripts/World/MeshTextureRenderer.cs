using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextureRenderer))]
public class MeshTextureRenderer : MonoBehaviour
{
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = meshData.CreateMesh();

        TextureRenderer renderer = GetComponent<TextureRenderer>();
        renderer.DrawTexture(texture);
    }
}
