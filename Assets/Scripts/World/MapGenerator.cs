﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public TextureRenderer textureRenderer;

    public int mapWidth = 0;
    public int mapHeight = 0;

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
        var map = NoiseGenerator.GenerateNoisemap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        textureRenderer.DrawTexture(TextureGenerator.TextureFromGrayscaleMap(map, regions));
    }
}
