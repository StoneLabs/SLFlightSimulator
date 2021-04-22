using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum EditorMode
    {
        COLORED,
        HEIGHTMAP,
        FLAT_COLORED,
        FLAT_HEIGHTMAP,
    }

    [Header("Editor Preview Properties")]
    public EditorMode editorMode = EditorMode.COLORED;
    public NoiseGenerator.NormMode editorNormalMode = NoiseGenerator.NormMode.Local;
    public MeshTextureRenderer editorRenderer;

    [Header("Generator Settings")]
    public const int chunkSize = 241;
    [Range(0, 6)]
    public int lod; //1, 2, 4, 6, 8, 10 or 12 (saved in range 0-6)

    [Range(0, 100)]
    public int heightFactor = 0;
    public AnimationCurve heightCurve;

    [Range(0.0001f, 150)]
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

    Queue<GeneratorJob> generatorResults = new Queue<GeneratorJob>();
    public void GenerateMapDataAsync(Vector2 offset, Action<MapData> callback)
    {
        new Thread(() =>
        {
            lock (generatorResults)
            {
                MapData data = GenerateMapData(offset);
                generatorResults.Enqueue(new GeneratorJob(data, callback));
            }
        }).Start();
    }

    public void Update()
    {
        if (generatorResults.Count > 0)
        {
            var job = generatorResults.Dequeue();
            job.callback(job.result);
        }
    }

    public MapData GenerateMapData(Vector2 localOffset, NoiseGenerator.NormMode mode = NoiseGenerator.NormMode.Global)
    {
        MapData mapData = new MapData();

        mapData.map = NoiseGenerator.GenerateNoisemap(chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, mode, offset + localOffset);
        mapData.textureData = TextureGenerator.ColorArrayFromGrayscaleMap(mapData.map, regions);
        for (int i = 0; i <= 6; i++)
            mapData.LODMeshData[i] = MeshGenerator.GenerateTerrainMesh(mapData.map, i, heightFactor, heightCurve);

        return mapData;
    }

    public void EditorRender()
    {
        MapData data = GenerateMapData(Vector2.zero, editorNormalMode);
        switch (editorMode)
        {
            case EditorMode.COLORED:
                editorRenderer.DrawMesh(data.LODMeshData[lod], TextureGenerator.TextureFromColorMap(data.textureData, chunkSize, chunkSize));
                break;
            case EditorMode.HEIGHTMAP:
                editorRenderer.DrawMesh(data.LODMeshData[lod], TextureGenerator.TextureFromGrayscaleMap(data.map));
                break;
            case EditorMode.FLAT_COLORED:
                editorRenderer.DrawMesh(MeshGenerator.GeneratePlaneMesh(chunkSize, chunkSize), TextureGenerator.TextureFromColorMap(data.textureData, chunkSize, chunkSize));
                break;
            case EditorMode.FLAT_HEIGHTMAP:
                editorRenderer.DrawMesh(MeshGenerator.GeneratePlaneMesh(chunkSize, chunkSize), TextureGenerator.TextureFromGrayscaleMap(data.map));
                break;
        }
    }
}
