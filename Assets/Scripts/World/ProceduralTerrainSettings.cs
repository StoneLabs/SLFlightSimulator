using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ProceduralTerrainSettings : ScriptableObject
{
    public int viewDistance = 10; // View distance in chunks
    public int[] LODDistances = new int[] { 4, 99, 99, 6, 99, 8 };
    [Range(0, 6)]
    public int ColliderLOD = 2;
    [Range(0, 240)]
    public float ColliderGenerationRadius = 50;

    [Range(1, 100)]
    public int scale = 1;
}
