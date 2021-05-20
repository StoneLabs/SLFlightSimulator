﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AeroSurface))]
public class AeroSurfaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AeroSurface surface = (AeroSurface)target;
        bool defaultChange = DrawDefaultInspector();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);

        EditorGUILayout.LabelField($"AoA*:\t{surface.AngleOfAttack}");
        EditorGUILayout.LabelField($"Area:\t{surface.SurfaceArea}m");
        EditorGUILayout.LabelField($"Wind:\t{surface.Wind.magnitude}m/s");
        EditorGUILayout.LabelField($"Wind:\t{surface.WindSpeedFront}m/s (applicable) [{surface.WindSpeedFront / surface.Wind.magnitude * 100.0f}%]");
    }
}
