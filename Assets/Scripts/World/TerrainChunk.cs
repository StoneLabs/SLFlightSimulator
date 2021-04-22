using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshTextureRenderer))]
public class TerrainChunk : MonoBehaviour
{
    public ProceduralTerrain terrain;
    public Vector2Int chunkCoordinate;
    public MapGenerator generator;

    private MapGenerator.MapData data = null; 
    private int currentLOD = -1;
    private bool hasCollider = false;

    public void Setup()
    {
        generator.GenerateMapDataAsync(new Vector2(chunkCoordinate.x * terrain.ChunkSize, chunkCoordinate.y * terrain.ChunkSize), (MapGenerator.MapData data) =>
        {
            this.data = data;
        });
    }

    private void Update()
    {
        if (!terrain.IsInViewDistance(this))
            terrain.UnloadChunk(this);

        if (data == null)
            return;

        int LODTarget = terrain.LODTarget(this);
        if (currentLOD != LODTarget && (currentLOD == -1 || Random.value > 0.75))
        {
            currentLOD = LODTarget;
            MeshTextureRenderer renderer = GetComponent<MeshTextureRenderer>();
            renderer.DrawMesh(data.LODMeshData[LODTarget], TextureGenerator.TextureFromColorMap(data.textureData, MapGenerator.chunkSize, MapGenerator.chunkSize));
        }

        if (!hasCollider && terrain.NeedsCollider(this))
        {
            MeshTextureRenderer renderer = GetComponent<MeshTextureRenderer>();
            renderer.DrawCollider(data.LODMeshData[terrain.ColliderLOD]);

            hasCollider = true;
        }
    }
}
