using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MeshTextureRenderer renderer;

    public const int chunkSize = 241;
    [Range(0, 6)]
    public int lod; //1, 2, 4, 6, 8, 10 or 12 (saved in range 0-6)

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

        var map = NoiseGenerator.GenerateNoisemap(chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        var mesh = MeshGenerator.GenerateTerrainMesh(map, lod, heightFactor, heightCurve);

        renderer.DrawMesh(mesh, TextureGenerator.TextureFromGrayscaleMap(map, regions));
    }
}
