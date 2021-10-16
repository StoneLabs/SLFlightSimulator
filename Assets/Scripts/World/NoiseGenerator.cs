using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// References:
// https://adrianb.io/2014/08/09/perlinnoise.html
// https://rtouti.github.io/graphics/perlin-noise-algorithm
// https://github.com/SebLague/Procedural-Landmass-Generation

/// <summary>
/// Noise generators
/// </summary>
public static class NoiseGenerator
{
    public enum NormMode
    {
        Local,
        Global
    }

    /// <summary>
    /// Generate noise map based on input parameters
    /// </summary>
    /// <param name="width">width of map</param>
    /// <param name="height">height of map</param>
    /// <param name="seed">seed of map</param>
    /// <param name="scale">scale of noise generation</param>
    /// <param name="octaves">number of octaves</param>
    /// <param name="persistance">persistance</param>
    /// <param name="lacunarity">lacunarity</param>
    /// <param name="mode">Mode of output. Local = scale to full [0, 1] range. Global = try to scale on global average. This is performed on best guess method. Clipping may rarely occur.</param>
    /// <param name="offset">Offset of map</param>
    /// <returns>Noisemap</returns>
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

    /// <summary>
    /// Calculate unclamped Perlin noise at position. Will not equal output from GenerateNoisemap
    /// </summary>
    /// <param name="x">X Coordinate</param>
    /// <param name="y">Y Coordinate</param>
    /// <param name="seed">Seed for generation</param>
    /// <param name="scale">scale of noise</param>
    /// <param name="octaves">Number of octaves</param>
    /// <param name="persistance">persistance</param>
    /// <param name="lacunarity">lacunarity</param>
    /// <param name="offset">Offset of noise</param>
    /// <returns>Noise value at specified coordinates</returns>
    public static float GlobalUnclampedPerlin(float x, float y, int seed, float scale, uint octaves, float persistance, float lacunarity, Vector2 offset)
    {
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


        float value = 0.0f;
        {
            float frequency = 1;
            float amplitude = 1;

            for (int octave = 0; octave < octaves; octave++)
            {
                float perlinX = (x + offset.x / 2 + seedOffsets[octave].x) / scale * frequency;
                float perlinY = (y - offset.y / 2 + seedOffsets[octave].y) / scale * frequency;

                value += amplitude * (Mathf.PerlinNoise(perlinX, perlinY) * 2 - 1);

                // Update frequency, amplitude for next iteration so that
                //  frequency = lacunarity^octave
                //  amplitude = persistance^octave
                frequency *= lacunarity;
                amplitude *= persistance;
            }
        }

        return value;
    }

    /// <summary>
    /// Generate falloff map. (Falling of towards the edge)
    /// </summary>
    /// <param name="size">Size of fallof map</param>
    /// <param name="fallOffDistance">Distance of falloff start</param>
    /// <param name="falloffHardness">Distance of falloff hardness</param>
    /// <returns>Falloff map</returns>
    public static float[,] GenerateFalloffMap(int size, float fallOffDistance, float falloffHardness)
    {
        float[,] map = new float[size, size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            { 
                float xFromCenter = (x / (float)size) * 2.0f - 1.0f; // [-1, 1]
                float yFromCenter = (y / (float)size) * 2.0f - 1.0f; // [-1, 1]
                float maxFromCenter = Mathf.Max(Mathf.Abs(xFromCenter), Mathf.Abs(yFromCenter));
                map[x, y] = Mathf.Pow(maxFromCenter, falloffHardness) / 
                    (Mathf.Pow(maxFromCenter, falloffHardness) + fallOffDistance * Mathf.Pow(1 - maxFromCenter, falloffHardness));
            }

        return map;
    }

    /// <summary>
    /// Substracts map B from map A. Size of inputs can differ. Smaller map is used as output size.
    /// </summary>
    /// <param name="mapA">Map A</param>
    /// <param name="mapB">Map B</param>
    /// <returns>Map A - Map B</returns>
    public static float[,] SubstractMap(float[,] mapA, float[,] mapB)
    {
        int widthA = mapA.GetLength(0);
        int heightA = mapA.GetLength(1);

        int widthB = mapB.GetLength(0);
        int heightB = mapB.GetLength(1);

        int width = Mathf.Min(widthA, widthB);
        int height = Mathf.Min(heightA, heightB);

        float[,] output = new float[width, height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                output[x, y] = mapA[x, y] - mapB[x, y];

        return output;
    }
}
