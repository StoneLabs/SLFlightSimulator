using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chunk spawned by the ProceduralTerrain script
/// </summary>
[RequireComponent(typeof(MeshTextureRenderer))]
public class TerrainChunk : MonoBehaviour
{
    public ProceduralTerrain terrain;   // terrain reference
    public Vector2Int chunkCoordinate;  // coordinated of this chunk
    public MapGenerator generator;      // Reference to generator

    private MapGenerator.MapData data = null;   // Data given by map generator on callback
    private int currentLOD = -1;                // Current level of detail
    private bool hasCollider = false;           // Wether collider has been calculated

    public void Setup()
    {
        // Request map data for current position
        generator.GenerateMapDataAsync(new Vector2(chunkCoordinate.x * terrain.UnscaledChunkSize, chunkCoordinate.y * terrain.UnscaledChunkSize), (MapGenerator.MapData data) =>
        {
            this.data = data;
        });
    }

    private void Update()
    {
        // Remove this chunk if it is out of distance
        if (!terrain.IsInViewDistance(this))
            terrain.UnloadChunk(this);

        // Dont do anything until data has arrived
        if (data == null)
            return;

        // Calculate current target LOD and update if required
        int LODTarget = terrain.LODTarget(this);
        if (currentLOD != LODTarget && (currentLOD == -1 || Random.value > 0.75))
        {
            currentLOD = LODTarget;
            MeshTextureRenderer renderer = GetComponent<MeshTextureRenderer>();
            renderer.DrawMesh(data.LODMeshData[LODTarget], TextureGenerator.TextureFromColorMap(data.textureData, MapGenerator.chunkSize, MapGenerator.chunkSize));
        }

        // Calculate collider if required
        if (!hasCollider && terrain.NeedsCollider(this))
        {
            MeshTextureRenderer renderer = GetComponent<MeshTextureRenderer>();
            renderer.DrawCollider(data.LODMeshData[terrain.settings.ColliderLOD]);

            hasCollider = true;
        }
    }
}
