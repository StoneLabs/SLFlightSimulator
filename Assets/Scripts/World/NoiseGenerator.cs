using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public static class NoiseGenerator
{
    public enum NormMode
    {
        Local,
        Global
    }

    // Reference: https://adrianb.io/2014/08/09/perlinnoise.html
    public static float[,] GenerateNoisemap(int width, int height, int seed, float scale, uint octaves, float persistance, float lacunarity, NormMode mode, Vector2 offset)
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
        Vector2 localNoiseRange = new Vector2(float.MaxValue, float.MinValue); // (min, max)

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float frequency = 1;
                float amplitude = 1;
                noiseMap[x, y] = 0;

                for (int octave = 0; octave < octaves; octave++)
                {
                    float perlinX = (x + offset.x - width/ 2 + seedOffsets[octave].x) / scale * frequency;
                    float perlinY = (y - offset.y - height/ 2 + seedOffsets[octave].y) / scale * frequency;

                    noiseMap[x, y] += amplitude * (Mathf.PerlinNoise(perlinX, perlinY) * 2 - 1);

                    // Update frequency, amplitude for next iteration so that
                    //  frequency = lacunarity^octave
                    //  amplitude = persistance^octave
                    frequency *= lacunarity;
                    amplitude *= persistance;
                }

                localNoiseRange.x = Math.Min(localNoiseRange.x, noiseMap[x, y]);
                localNoiseRange.y = Math.Max(localNoiseRange.y, noiseMap[x, y]);
            }

        // Calculate theoretical max and min values for Global mode
        float maxHeight = 0;
        {
            float amplitude = 1;
            for (int octave = 0; octave < octaves; octave++)
            {
                maxHeight += amplitude;
                amplitude *= persistance;
            }
        }


        // Remap array to range (0, 1)
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (mode == NormMode.Local)
                    noiseMap[x, y] = (localNoiseRange.y == localNoiseRange.x) ? 0 : (noiseMap[x, y] - localNoiseRange.x) / (localNoiseRange.y - localNoiseRange.x);
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }

        return noiseMap;
    }
}
