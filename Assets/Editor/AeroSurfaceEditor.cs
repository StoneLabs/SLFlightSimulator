using System.Collections;
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
        EditorGUILayout.LabelField($"Wind:\t{surface.Air.magnitude}m/s");
        EditorGUILayout.LabelField($"Wind:\t{surface.AirSpeedFront}m/s (applicable) [{surface.AirSpeedFront / surface.Air.magnitude * 100.0f}%]");
        GUILayout.Space(10);
        EditorGUILayout.LabelField($"Lift:\t{surface.LiftForce} [{surface.LiftForce.magnitude}]");
        EditorGUILayout.LabelField($"Drag:\t{surface.DragForce} [{surface.LiftForce.magnitude}]");
    }
}
