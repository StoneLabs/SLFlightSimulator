using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralTerrain : MonoBehaviour
{
    public int viewDistance = 16; // View distance in chunks
    public int[] LODDistances = new int[] {4, 6, 8, 10, 12, 14};
    public Transform viewer;
    public GameObject chunkContainer;
    public MapGenerator generator;
    public Material terrainMaterial;

    private Dictionary<Vector2Int, TerrainChunk> chunks = new Dictionary<Vector2Int, TerrainChunk>();

    public int ChunkSize
    {
        get { return MapGenerator.chunkSize - 1; }
    }

    public Vector2Int ViewerChunkPosition
    {
        get
        {
            return new Vector2Int(
                Mathf.FloorToInt(this.viewer.position.x / ChunkSize),
                Mathf.FloorToInt(this.viewer.position.z / ChunkSize));
        }
    }

    private void Update()
    {
        for (int chunkX = ViewerChunkPosition.x - viewDistance + 1; chunkX < ViewerChunkPosition.x + viewDistance; chunkX++)
            for (int chunkY = ViewerChunkPosition.y - viewDistance + 1; chunkY < ViewerChunkPosition.y + viewDistance; chunkY++)
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

        var meshFilter = chunkObject.AddComponent<MeshFilter>();
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

    public bool IsInDistance(Vector2Int chunkCoordinate, uint distance)
    {
        if (chunkCoordinate.x <= ViewerChunkPosition.x - distance || chunkCoordinate.x >= ViewerChunkPosition.x + distance)
            return false;

        if (chunkCoordinate.y <= ViewerChunkPosition.y - distance || chunkCoordinate.y >= ViewerChunkPosition.y + distance)
            return false;

        return true;
    }

    public bool IsInViewDistance(Vector2Int chunkCoordinate)
    {
        return IsInDistance(chunkCoordinate, (uint)viewDistance);
    }

    public bool IsInViewDistance(TerrainChunk chunk)
    {
        return IsInViewDistance(chunk.chunkCoordinate);
    }

    public int LODTarget(TerrainChunk chunk)
    {
        for (int i = 0; i < 6; i++)
        {
            if (IsInDistance(chunk.chunkCoordinate, (uint)LODDistances[i]))
                return i;
        }
        return 6;
    }
}
