using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    bool autoUpdate = true;
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawDefaultInspector() && autoUpdate)
            mapGenerator.EditorRender();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Editor preview Actiones", EditorStyles.boldLabel);
        autoUpdate = GUILayout.Toggle(autoUpdate, "Auto update");
        if (GUILayout.Button("Generate editor preview"))
            mapGenerator.EditorRender();
    }
}
