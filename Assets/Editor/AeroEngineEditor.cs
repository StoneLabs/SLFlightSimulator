using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AeroEngine))]
public class AeroEngineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AeroEngine engine = (AeroEngine)target;
        bool defaultChange = DrawDefaultInspector();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);

        EditorGUILayout.LabelField($"RPM:\t{engine.RPM} RPM");
        EditorGUILayout.LabelField($"Torque:\t{engine.EnginePower} N");
        EditorGUILayout.LabelField($"HP:\t{engine.EnginePower * engine.RPM / 9550.0f * 1.36f} hp");
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Info (PROP)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Thrust:\t{engine.Thrust.magnitude} N");
        EditorGUILayout.LabelField($"Thrust:\t{engine.Thrust.magnitude / 9.81f} kg eq");
        EditorGUILayout.LabelField($"CT:\t{engine.propeller.CounterTorque} NM");
    }
}
