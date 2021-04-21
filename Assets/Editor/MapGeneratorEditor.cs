using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawDefaultInspector() && mapGenerator.autoUpdate)
        {
            MapGenerator.MapData data = mapGenerator.GenerateMapData(Vector2.zero);
            mapGenerator.renderer.DrawMesh(data.LODMeshData[mapGenerator.lod], TextureGenerator.TextureFromColorMap(data.textureData, MapGenerator.chunkSize, MapGenerator.chunkSize));
        }

        if (GUILayout.Button("Generate"))
        {
            MapGenerator.MapData data = mapGenerator.GenerateMapData(Vector2.zero);
            mapGenerator.renderer.DrawMesh(data.LODMeshData[mapGenerator.lod], TextureGenerator.TextureFromColorMap(data.textureData, MapGenerator.chunkSize, MapGenerator.chunkSize));
        }
    }
}
