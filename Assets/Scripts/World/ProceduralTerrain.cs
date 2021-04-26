using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralTerrain : MonoBehaviour
{
    public ProceduralTerrainSettings settings;
    public Transform viewer;
    public GameObject chunkContainer;
    public MapGenerator generator;
    public Material terrainMaterial;

    private Dictionary<Vector2Int, TerrainChunk> chunks = new Dictionary<Vector2Int, TerrainChunk>();

    public int ChunkSize
    {
        get { return UnscaledChunkSize * settings.scale; }
    }
    public int UnscaledChunkSize
    {
        get { return (MapGenerator.chunkSize - 1); }
    }

    public Vector2Int ViewerChunkCoordinate
    {
        get
        {
            return new Vector2Int(
                Mathf.FloorToInt(this.viewer.position.x / ChunkSize),
                Mathf.FloorToInt(this.viewer.position.z / ChunkSize));
        }
    }
    public Vector2 ViewerChunkPosition
    {
        get
        {
            return new Vector2(
                this.viewer.position.x / ChunkSize,
                this.viewer.position.z / ChunkSize);
        }
    }

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

    private void LoadChunk(Vector2Int chunkPosition)
    {
        GameObject chunkObject = new GameObject();
        chunkObject.name = chunkPosition.ToString();

        // Set position and parent
        chunkObject.transform.SetParent(chunkContainer.transform);
        chunkObject.transform.position = new Vector3((chunkPosition.x + 0.5f) * ChunkSize, 0, (chunkPosition.y + 0.5f) * ChunkSize);
        chunkObject.transform.localScale *= settings.scale;

        var meshFilter = chunkObject.AddComponent<MeshFilter>();
        var meshCollider = chunkObject.AddComponent<MeshCollider>();
        var meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        var chunk = chunkObject.AddComponent<TerrainChunk>();
        meshRenderer.sharedMaterial = new Material(terrainMaterial);
        chunk.terrain = this;
        chunk.generator = generator;
        chunk.chunkCoordinate = chunkPosition;
        chunk.Setup();

        chunks.Add(chunkPosition, chunk);
    }

    public void UnloadChunk(TerrainChunk chunk)
    {
        chunks.Remove(chunk.chunkCoordinate);
        Destroy(chunk.gameObject);
    }

    public bool IsInRealDistance(TerrainChunk chunk, float distance)
    {
        return IsInRealDistance(chunk.chunkCoordinate, distance);
    }
    public bool IsInRealDistance(Vector2Int chunkCoordinate, float distance)
    {
        Bounds bounds = new Bounds(
            new Vector3(chunkCoordinate.x + 0.5f, 0, chunkCoordinate.y + 0.5f), 
            new Vector3(1, 1, 1));

        return bounds.SqrDistance(new Vector3(ViewerChunkPosition.x, 0, ViewerChunkPosition.y)) <= (distance / ChunkSize);
    }
    public bool IsInDistance(TerrainChunk chunk, uint distance)
    {
        return IsInDistance(chunk.chunkCoordinate, distance);
    }
    public bool IsInDistance(Vector2Int chunkCoordinate, uint distance)
    {
        if (chunkCoordinate.x <= ViewerChunkCoordinate.x - distance || chunkCoordinate.x >= ViewerChunkCoordinate.x + distance)
            return false;

        if (chunkCoordinate.y <= ViewerChunkCoordinate.y - distance || chunkCoordinate.y >= ViewerChunkCoordinate.y + distance)
            return false;

        return true;
    }

    public bool IsInViewDistance(Vector2Int chunkCoordinate)
    {
        return IsInDistance(chunkCoordinate, (uint)settings.viewDistance);
    }

    public bool IsInViewDistance(TerrainChunk chunk)
    {
        return IsInDistance(chunk, (uint)settings.viewDistance);
    }

    public int LODTarget(TerrainChunk chunk)
    {
        for (int i = 0; i < 6; i++)
        {
            if (IsInDistance(chunk.chunkCoordinate, (uint)settings.LODDistances[i]))
                return i;
        }
        return 6;
    }

    public bool NeedsCollider(TerrainChunk chunk)
    {
        return IsInRealDistance(chunk, settings.ColliderGenerationRadius);
    }
}
