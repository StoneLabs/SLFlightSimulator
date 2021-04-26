using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public bool autoUpdate = true;

    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        mapGenerator.settings.Subscribe(() => { if (autoUpdate) mapGenerator.EditorRender(); });
        if (DrawDefaultInspector() && autoUpdate)
            mapGenerator.EditorRender();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Editor preview Actiones", EditorStyles.boldLabel);
        autoUpdate = GUILayout.Toggle(autoUpdate, "Auto update");
        if (GUILayout.Button("Generate editor preview"))
            mapGenerator.EditorRender();
    }
}
