using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public ProceduralTerrain terrain;
    public Vector2Int chunkCoordinate;

    private void Update()
    {
        if (!terrain.IsInViewDistance(this))
            terrain.UnloadChunk(this);
    }
}
