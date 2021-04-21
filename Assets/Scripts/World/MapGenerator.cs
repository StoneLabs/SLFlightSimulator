using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MeshTextureRenderer renderer;

    public const int chunkSize = 241;
    [Range(0, 6)]
    public int lod; //1, 2, 4, 6, 8, 10 or 12 (saved in range 0-6)

    [Range(0, 100)]
    public int heightFactor = 0;
    public AnimationCurve heightCurve;

    [Range(0.0001f, 50)]
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
        public MeshData meshData;
        public Color[] textureData;

        public MapData()
        {
        }

        public MapData(float[,] map, MeshData meshData, Color[] texture)
        {
            this.map = map;
            this.meshData = meshData;
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
    public void GenerateMapDataAsync(Action<MapData> callback)
    {
        new Thread(() =>
        {
            lock (generatorResults)
            {
                MapData data = GenerateMapData();
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

    public MapData GenerateMapData()
    {
        MapData mapData = new MapData();

        mapData.map = NoiseGenerator.GenerateNoisemap(chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        mapData.meshData = MeshGenerator.GenerateTerrainMesh(mapData.map, lod, heightFactor, heightCurve);
        mapData.textureData = TextureGenerator.ColorArrayFromGrayscaleMap(mapData.map, regions);

        return mapData;
    }
}
