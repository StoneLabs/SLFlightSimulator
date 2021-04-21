using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralTerrain : MonoBehaviour
{
    public const int viewDistance = 2; // View distance in chunks
    public Transform viewer;
    public GameObject chunkContainer;

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
        GameObject chunkObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        chunkObject.transform.SetParent(chunkContainer.transform);
        chunkObject.transform.position = new Vector3((chunkPosition.x + 0.5f) * ChunkSize, -1, (chunkPosition.y + 0.5f) * ChunkSize);
        chunkObject.transform.localScale = new Vector3(ChunkSize / 10.0f, 1, ChunkSize / 10.0f); // 10.0f is plane size
        chunkObject.name = chunkPosition.ToString();

        TerrainChunk chunk = chunkObject.AddComponent<TerrainChunk>();
        chunk.terrain = this;
        chunk.chunkCoordinate = chunkPosition;

        chunks.Add(chunkPosition, chunk);
    }

    public void UnloadChunk(TerrainChunk chunk)
    {
        chunks.Remove(chunk.chunkCoordinate);
        Destroy(chunk.gameObject);
    }

    public bool IsInViewDistance(Vector2Int chunkCoordinate)
    {
        if (chunkCoordinate.x <= ViewerChunkPosition.x - viewDistance || chunkCoordinate.x >= ViewerChunkPosition.x + viewDistance)
            return false;

        if (chunkCoordinate.y <= ViewerChunkPosition.y - viewDistance || chunkCoordinate.y >= ViewerChunkPosition.y + viewDistance)
            return false;

        return true;
    }

    public bool IsInViewDistance(TerrainChunk chunk)
    {
        return IsInViewDistance(chunk.chunkCoordinate);
    }
}
