using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour
{
    public int mapWidth = 0;
    public int mapHeight = 0;

    [Range(0.0001f, 20)]
    public float noiseScale = 0;

    // This is used in the Editor extension
    public bool autoUpdate;

    public void GenerateMap()
    {
        var map = NoiseGenerator.GenerateNoisemap(mapWidth, mapHeight, noiseScale);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(map);
    }
}
