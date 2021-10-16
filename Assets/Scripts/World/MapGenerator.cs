using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Map generator
/// </summary>
public class MapGenerator : MonoBehaviour
{
    // Preview mode for editor preview
    public enum EditorMode
    {
        COLORED,
        FLAT_COLORED,
        HEIGHTMAP,
        FLAT_HEIGHTMAP,
        FALLOFF,
        FLAT_FALLOFF
    }

    // Size of chunk (effective size is -1)
    public const int chunkSize = 241;

    // Settings for editor preview
    [Header("Editor Preview Properties")]
    public EditorMode editorMode = EditorMode.COLORED;
    public NoiseGenerator.NormMode editorNormalMode = NoiseGenerator.NormMode.Local;
    public MeshTextureRenderer editorRenderer;
    public bool editorCreateCollider = false;

    // Show debug information onscreen
    [Header("Debug tools")]
    public bool debugInfoOnScreen = true;
    public Rect debugPosition;
    private int generatorCount = 0;

    // Generator settings object used for heightmap generation
    [Header("Generator settings")]
    public MapGeneratorSettings settings;

    // Not used anymore
    private float[,] fallOffMap;

    // MapData object that is given to the callback method
    public class MapData
    {
        public float[,] map;
        public MeshData[] LODMeshData = new MeshData[7];
        public Color[] textureData;

        public MapData()
        {
        }

        public MapData(float[,] map, MeshData[] LODMeshData, Color[] texture)
        {
            this.map = map;
            this.LODMeshData = LODMeshData;
            this.textureData = texture;
        }
    }

    // Calculate falloff map at validation for performance
    private void OnValidate()
    {
        fallOffMap = NoiseGenerator.GenerateFalloffMap(chunkSize, settings.falloffDistance, settings.falloffHardness);
    }

    // Storage for generator jobs that are to be returned in the main thread
    private class GeneratorJob
    {
        public MapData result;
        public Action<MapData> callback;

        public GeneratorJob()
        {
        }

        public GeneratorJob(MapData jobResult, Action<MapData> callBack)
        {
            this.result = jobResult;
            this.callback = callBack;
        }
    }

    /// <summary>
    /// Queue async generation of chunk
    /// </summary>
    Queue<GeneratorJob> generatorResults = new Queue<GeneratorJob>();
    public void GenerateMapDataAsync(Vector2 offset, Action<MapData> callback)
    {
        // For debug: increase count of running generators length
        generatorCount++;

        // Spawn thread for the generation of the requested chunk
        new Thread(() =>
        {
            lock (generatorResults)
            {
                // Generate mapdata in this async context and queue the answer
                MapData data = GenerateMapData(offset);
                generatorResults.Enqueue(new GeneratorJob(data, callback));

                // Results are available. Generator is done. decrease counter
                generatorCount--; 
            }
        }).Start();
    }

    public void Update()
    {
        // Perform callback operation if results are available.
        // Only one per frame to prevent lag spikes from operations down the chain
        if (generatorResults.Count > 0)
        {
            var job = generatorResults.Dequeue();
            job.callback(job.result);
        }
    }

    /// <summary>
    /// Generate MapData for job given local offset.
    /// </summary>
    /// <param name="localOffset">Local offset in addition to static offset from generator settings</param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public MapData GenerateMapData(Vector2 localOffset, NoiseGenerator.NormMode mode = NoiseGenerator.NormMode.Global)
    {
        // Create job object
        MapData mapData = new MapData();

        // Calculate noise map with local offset
        mapData.map = NoiseGenerator.GenerateNoisemap(chunkSize, chunkSize, settings.seed, settings.noiseScale,
            settings.octaves, settings.persistance, settings.lacunarity, mode, settings.offset + localOffset);

        // Apply falloff if requested. (not used, untested)
        if (settings.applyFalloff)
            mapData.map = NoiseGenerator.SubstractMap(mapData.map, fallOffMap);

        // Calculate Meshes and Colors for all LODs
        // This is done in the async context in which this function was called
        // Mesh object generation is performed later on in the main thread due to unity limitations
        mapData.textureData = TextureGenerator.ColorArrayFromGrayscaleMap(mapData.map, settings.regions);
        for (int i = 0; i <= 6; i++)
            mapData.LODMeshData[i] = MeshGenerator.GenerateTerrainMesh(mapData.map, i, settings.heightFactor, settings.heightCurve);

        // Return job
        return mapData;
    }

    public void EditorRender()
    {
        // Peview settings on given plane as preview in editor
        MapData data = GenerateMapData(Vector2.zero, editorNormalMode);
        MeshData falloff_mesh = MeshGenerator.GenerateTerrainMesh(fallOffMap, settings.lod, settings.heightFactor, AnimationCurve.Linear(0, 0, 1, 1));
        MeshData plane = MeshGenerator.GeneratePlaneMesh(chunkSize, chunkSize);
        switch (editorMode)
        {
            case EditorMode.COLORED:
                editorRenderer.DrawMesh(data.LODMeshData[settings.lod], TextureGenerator.TextureFromColorMap(data.textureData, chunkSize, chunkSize));
                editorRenderer.DrawCollider(data.LODMeshData[settings.lod]);
                break;
            case EditorMode.HEIGHTMAP:
                editorRenderer.DrawMesh(data.LODMeshData[settings.lod], TextureGenerator.TextureFromGrayscaleMap(data.map));
                editorRenderer.DrawCollider(data.LODMeshData[settings.lod]);
                break;
            case EditorMode.FALLOFF:
                editorRenderer.DrawMesh(falloff_mesh, TextureGenerator.TextureFromGrayscaleMap(fallOffMap));
                editorRenderer.DrawCollider(falloff_mesh);
                break;
            case EditorMode.FLAT_COLORED:
                editorRenderer.DrawMesh(plane, TextureGenerator.TextureFromColorMap(data.textureData, chunkSize, chunkSize));
                editorRenderer.DrawCollider(plane);
                break;
            case EditorMode.FLAT_HEIGHTMAP:
                editorRenderer.DrawMesh(plane, TextureGenerator.TextureFromGrayscaleMap(data.map));
                editorRenderer.DrawCollider(plane);
                break;
            case EditorMode.FLAT_FALLOFF:
                editorRenderer.DrawMesh(plane, TextureGenerator.TextureFromGrayscaleMap(fallOffMap));
                editorRenderer.DrawCollider(plane);
                break;
        }
    }

    // Print debug information
    private void OnGUI()
    {
        if (debugInfoOnScreen)
        {
            String debugInfo = "";
            debugInfo += "World generator debug INFO:\n";
            debugInfo += $"World generator jobs running: {generatorCount}\n";
            debugInfo += $"World generatr callback queue length: {this.generatorResults.Count}\n";

            GUI.Label(debugPosition, debugInfo);
        }
    }
}
