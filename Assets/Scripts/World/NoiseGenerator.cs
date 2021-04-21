using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public static class NoiseGenerator
{
    // Reference: https://adrianb.io/2014/08/09/perlinnoise.html
    public static float[,] GenerateNoisemap(int width, int height, int seed, float scale, uint octaves, float persistance, float lacunarity, Vector2 offset)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Illegal dimensions supplied to Noisemap generator");

        if (scale <= 0)
            throw new ArgumentException("Illegal scale supplied to Noisemap generator");

        if (octaves <= 0)
            throw new ArgumentException("Illegal number of octaves supplied to Noisemap generator");

        // System random from seed
        System.Random random = new System.Random(seed);
        Vector2[] seedOffsets = new Vector2[octaves];

        // Generate random offsets for each octave
        for (int octave = 0; octave < octaves; octave++)
            seedOffsets[octave] = new Vector2(random.Next((int)-1e6, (int)1e6), random.Next((int)-1e6, (int)1e6));

        // 2d float array to store perlin noise data
        float[,] noiseMap = new float[width, height];
        Vector2 noiseRange = new Vector2(float.MaxValue, float.MinValue); // (min, max)

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float frequency = 1;
                float amplitude = 1;
                noiseMap[x, y] = 0;

                for (int octave = 0; octave < octaves; octave++)
                {
                    float perlinX = (x + offset.x - width/2) / scale * frequency + seedOffsets[octave].x;
                    float perlinY = (y + offset.y - height/2) / scale * frequency + seedOffsets[octave].y;

                    noiseMap[x, y] += amplitude * (Mathf.PerlinNoise(perlinX, perlinY) * 2 - 1);

                    // Update frequency, amplitude for next iteration so that
                    //  frequency = lacunarity^octave
                    //  amplitude = persistance^octave
                    frequency *= lacunarity;
                    amplitude *= persistance;
                }

                noiseRange.x = Math.Min(noiseRange.x, noiseMap[x, y]);
                noiseRange.y = Math.Max(noiseRange.y, noiseMap[x, y]);
            }

        // Remap array to range (0, 1)
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                noiseMap[x, y] = (noiseMap[x, y] - noiseRange.x) / (noiseRange.y - noiseRange.x);

        return noiseMap;
    }
}
