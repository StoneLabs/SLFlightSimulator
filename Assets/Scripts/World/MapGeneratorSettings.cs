using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings for map generator
/// </summary>
[CreateAssetMenu()]
public class MapGeneratorSettings : SubscribeableSettings
{
    [Header("Generator Settings")]
    [Range(0, 6)]
    public int lod; //1, 2, 4, 6, 8, 10 or 12 (saved in range 0-6)

    // Height curve for MeshGenerator
    [Range(0, 1000)]
    public int heightFactor = 25;
    public AnimationCurve heightCurve;

    // Scale of noise
    [Range(0.0001f, 5000)]
    public float noiseScale = 25;

    // Number of octaves
    [Range(1, 12)]
    public uint octaves = 4;

    // Persistance value
    [Range(0, 1)]
    public float persistance = 0.5f;

    // Lacunarity value
    [Range(0, 3)]
    public float lacunarity = 2.0f;

    // Offset and seed for displacement
    public Vector2 offset;
    public int seed;

    // gradient used for coloring mesh
    public Gradient regions;

    // Apply falloff? Not used anymore, not tested!
    [Header("Falloff settings")]
    public bool applyFalloff = true;

    [Range(0, 20)]
    public float falloffHardness = 0;
    [Range(0, 20)]
    public float falloffDistance = 0;
}
