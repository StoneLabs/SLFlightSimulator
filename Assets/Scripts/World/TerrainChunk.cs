using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshTextureRenderer))]
public class TerrainChunk : MonoBehaviour
{
    public ProceduralTerrain terrain;
    public Vector2Int chunkCoordinate;
    public MapGenerator generator;

    public void Setup()
    {
        generator.GenerateMapDataAsync((MapGenerator.MapData data) =>
        {
            MeshTextureRenderer renderer = GetComponent<MeshTextureRenderer>();
            renderer.DrawMesh(data.meshData, TextureGenerator.TextureFromColorMap(data.textureData, MapGenerator.chunkSize, MapGenerator.chunkSize));
        });
    }

    private void Update()
    {
        if (!terrain.IsInViewDistance(this))
            terrain.UnloadChunk(this);
    }
}
