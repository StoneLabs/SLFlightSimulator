using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoisemap(int width, int height, float scale)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Illegal dimensions supplied to Noisemap generator");

        if (scale <= 0)
            throw new ArgumentException("Illegal scale supplied to Noisemap generator");

        // 2d float array to store perlin noise data
        float[,] noiseMap = new float[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x ++)
                noiseMap[x, y] = Mathf.PerlinNoise(x / scale, y / scale);

        return noiseMap;
    }
}
