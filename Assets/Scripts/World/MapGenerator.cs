using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MeshTextureRenderer renderer;

    [Range(0, 200)]
    public int mapWidth = 0;
    [Range(0, 200)]
    public int mapHeight = 0;
    [Range(0, 100)]
    public int heightFactor = 0;
    public AnimationCurve heightCurve;

    [Range(0.0001f, 50)]
    public float noiseScale = 0;

    public uint octaves;

    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public Vector2 offset;
    public int seed;

    public Gradient regions;

    // This is used in the Editor extension
    public bool autoUpdate;

    public void GenerateMap()
    {
        if (renderer == null)
            return;

        var map = NoiseGenerator.GenerateNoisemap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        var mesh = MeshGenerator.GenerateTerrainMesh(map, heightFactor, heightCurve);

        renderer.DrawMesh(mesh, TextureGenerator.TextureFromGrayscaleMap(map, regions));
    }
}
