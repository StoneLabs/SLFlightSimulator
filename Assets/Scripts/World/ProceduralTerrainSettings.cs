using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Asset saveable settings for the terrain generator
/// </summary>
[CreateAssetMenu()]
public class ProceduralTerrainSettings : SubscribeableSettings
{
    public int viewDistance = 10; // View distance in chunks
    public int[] LODDistances = new int[] { 4, 99, 99, 6, 99, 8 }; // Distances for the levels of detail
    [Range(0, 6)]
    public int ColliderLOD = 2; // Level of detail used for the collider
    [Range(0, 240)]
    public float ColliderGenerationRadius = 50;

    [Range(1, 100)]
    public int scale = 1;
}
