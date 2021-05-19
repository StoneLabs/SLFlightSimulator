using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AeroSurface))]
public class AeroSurfaceEditor : Editor
{
    public bool autoUpdate = true;
    float area;

    private void calculatePreview(AeroSurface surface)
    {
        this.area = surface.SurfaceArea;
    }

    public override void OnInspectorGUI()
    {
        AeroSurface surface = (AeroSurface)target;
        bool defaultChange = DrawDefaultInspector();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Editor preview", EditorStyles.boldLabel);

        EditorGUILayout.LabelField($"Area:\t{area}");

        if (autoUpdate)
            calculatePreview(surface);

        GUILayout.Space(10);
        autoUpdate = GUILayout.Toggle(autoUpdate, "Auto-update");
        if (GUILayout.Button("Calculate"))
            calculatePreview(surface);
    }
}
