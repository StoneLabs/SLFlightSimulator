using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Procedural Terrain manager. This script spawns chunks using specified MapGenerator
/// </summary>
public class ProceduralTerrain : MonoBehaviour
{
    public ProceduralTerrainSettings settings;  // Generator settings
    public Transform viewer;                    // Reference to viewer for distance calculations
    public GameObject chunkContainer;           // Container to spawn chunks in (for organization)
    public MapGenerator generator;              // Map Generator for generation of chunks
    public Material terrainMaterial;            // 

    // Storage for generated chunks. key = chunk coordinate
    private Dictionary<Vector2Int, TerrainChunk> chunks = new Dictionary<Vector2Int, TerrainChunk>();

    // Size of a chunk in units as calculated from the settings
    public int ChunkSize
    {
        get { return UnscaledChunkSize * settings.scale; }
    }

    // Unscaled size of chunk as specified by mapgenerator
    public int UnscaledChunkSize
    {
        get { return (MapGenerator.chunkSize - 1); }
    }

    // Current chunk (in chunk coordinates) of viewer object
    public Vector2Int ViewerChunkCoordinate
    {
        get
        {
            return new Vector2Int(
                Mathf.FloorToInt(this.viewer.position.x / ChunkSize),
                Mathf.FloorToInt(this.viewer.position.z / ChunkSize));
        }
    }

    // Position of viewer in chunk coordinates
    public Vector2 ViewerChunkPosition
    {
        get
        {
            return new Vector2(
                this.viewer.position.x / ChunkSize,
                this.viewer.position.z / ChunkSize);
        }
    }

    // Spawn chunks as needed if not already loaded
    private void Update()
    {
        for (int chunkX = ViewerChunkCoordinate.x - settings.viewDistance + 1; chunkX < ViewerChunkCoordinate.x + settings.viewDistance; chunkX++)
            for (int chunkY = ViewerChunkCoordinate.y - settings.viewDistance + 1; chunkY < ViewerChunkCoordinate.y + settings.viewDistance; chunkY++)
            {
                Vector2Int chunk = new Vector2Int(chunkX, chunkY);

                if (chunks.ContainsKey(chunk))
                    continue;

                LoadChunk(chunk);
            }
    }

    /// <summary>
    /// Create and spawn new chunk object of specified coordinate
    /// </summary>
    /// <param name="chunkPosition">coordinate of chunk being spawned</param>
    private void LoadChunk(Vector2Int chunkPosition)
    {
        GameObject chunkObject = new GameObject();
        chunkObject.name = chunkPosition.ToString();

        // Set position and parent
        chunkObject.transform.SetParent(chunkContainer.transform);
        chunkObject.transform.position = new Vector3((chunkPosition.x + 0.5f) * ChunkSize, 0, (chunkPosition.y + 0.5f) * ChunkSize);
        chunkObject.transform.localScale *= settings.scale;

        // Add components
        var meshFilter = chunkObject.AddComponent<MeshFilter>();
        var meshCollider = chunkObject.AddComponent<MeshCollider>();
        var meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        var chunk = chunkObject.AddComponent<TerrainChunk>();
        
        // Set references
        meshRenderer.sharedMaterial = new Material(terrainMaterial);
        chunk.terrain = this;
        chunk.generator = generator;
        chunk.chunkCoordinate = chunkPosition;
        chunk.Setup();

        // Add chunk to dict
        chunks.Add(chunkPosition, chunk);
    }

    // Remove chunk from dict
    public void UnloadChunk(TerrainChunk chunk)
    {
        chunks.Remove(chunk.chunkCoordinate);
        Destroy(chunk.gameObject);
    }

    // Check wether chunk is in real distance to viewer
    public bool IsInRealDistance(TerrainChunk chunk, float distance)
    {
        return IsInRealDistance(chunk.chunkCoordinate, distance);
    }

    // Check wether chunk is in real distance to viewer
    public bool IsInRealDistance(Vector2Int chunkCoordinate, float distance)
    {
        Bounds bounds = new Bounds(
            new Vector3(chunkCoordinate.x + 0.5f, 0, chunkCoordinate.y + 0.5f), 
            new Vector3(1, 1, 1));

        return bounds.SqrDistance(new Vector3(ViewerChunkPosition.x, 0, ViewerChunkPosition.y)) <= (distance / ChunkSize);
    }
    // Check wether chunk is in real distance to viewer
    public bool IsInDistance(TerrainChunk chunk, uint distance)
    {
        return IsInDistance(chunk.chunkCoordinate, distance);
    }
    // Check wether chunk is in real distance to viewer
    public bool IsInDistance(Vector2Int chunkCoordinate, uint distance)
    {
        if (chunkCoordinate.x <= ViewerChunkCoordinate.x - distance || chunkCoordinate.x >= ViewerChunkCoordinate.x + distance)
            return false;

        if (chunkCoordinate.y <= ViewerChunkCoordinate.y - distance || chunkCoordinate.y >= ViewerChunkCoordinate.y + distance)
            return false;

        return true;
    }


    // Check wether chunk is in view distance
    public bool IsInViewDistance(Vector2Int chunkCoordinate)
    {
        return IsInDistance(chunkCoordinate, (uint)settings.viewDistance);
    }

    // Check wether chunk is in view distance
    public bool IsInViewDistance(TerrainChunk chunk)
    {
        return IsInDistance(chunk, (uint)settings.viewDistance);
    }

    // Calculate target LOD of chunk based on distance from viewer
    public int LODTarget(TerrainChunk chunk)
    {
        for (int i = 0; i < 6; i++)
        {
            if (IsInDistance(chunk.chunkCoordinate, (uint)settings.LODDistances[i]))
                return i;
        }
        return 6;
    }

    // Check wether chunk need collider based on view distance
    public bool NeedsCollider(TerrainChunk chunk)
    {
        return IsInRealDistance(chunk, settings.ColliderGenerationRadius);
    }
}
