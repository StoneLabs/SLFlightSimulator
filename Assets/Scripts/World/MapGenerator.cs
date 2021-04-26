using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum EditorMode
    {
        COLORED,
        FLAT_COLORED,
        HEIGHTMAP,
        FLAT_HEIGHTMAP,
        FALLOFF,
        FLAT_FALLOFF
    }

    public const int chunkSize = 241;

    [Header("Editor Preview Properties")]
    public EditorMode editorMode = EditorMode.COLORED;
    public NoiseGenerator.NormMode editorNormalMode = NoiseGenerator.NormMode.Local;
    public MeshTextureRenderer editorRenderer;
    public bool editorCreateCollider = false;

    [Header("Generator settings")]
    public MapGeneratorSettings settings;

    private float[,] fallOffMap;

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

    private void OnValidate()
    {
        fallOffMap = NoiseGenerator.GenerateFalloffMap(chunkSize, settings.falloffDistance, settings.falloffHardness);
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

        mapData.map = NoiseGenerator.GenerateNoisemap(chunkSize, chunkSize, settings.seed, settings.noiseScale,
            settings.octaves, settings.persistance, settings.lacunarity, mode, settings.offset + localOffset);

        if (settings.applyFalloff)
            mapData.map = NoiseGenerator.SubstractMap(mapData.map, fallOffMap);

        mapData.textureData = TextureGenerator.ColorArrayFromGrayscaleMap(mapData.map, settings.regions);
        for (int i = 0; i <= 6; i++)
            mapData.LODMeshData[i] = MeshGenerator.GenerateTerrainMesh(mapData.map, i, settings.heightFactor, settings.heightCurve);

        return mapData;
    }

    public void EditorRender()
    {
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
}
