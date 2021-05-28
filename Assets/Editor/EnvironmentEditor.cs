using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Environment))]
public class EnvironmentEditor : Editor
{
    public bool autoUpdate = true;
    float altitude;
    float temperature;
    float pressure;
    float density;

    private void calculatePreview(Environment environment)
    {
        this.temperature = environment.CalculateTemperature(altitude);
        this.pressure = environment.CalculatePressure(altitude);
        this.density = environment.CalculateDensity(altitude);
    }    

    public override void OnInspectorGUI()
    {
        Environment environment = (Environment)target;
        bool defaultChange = DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor preview", EditorStyles.boldLabel);

        float old_altitude = altitude;
        altitude = EditorGUILayout.Slider(altitude, 0, 100000);

        EditorGUILayout.LabelField($"Temperature:\t{temperature}");
        EditorGUILayout.LabelField($"Pressure:\t\t{pressure}");
        EditorGUILayout.LabelField($"Density:\t\t{density}");

        if ((defaultChange || old_altitude != altitude) && autoUpdate)
            calculatePreview(environment);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Editor preview Actiones", EditorStyles.boldLabel);
        autoUpdate = GUILayout.Toggle(autoUpdate, "Auto update");
        if (GUILayout.Button("Calculate"))
            calculatePreview(environment);
    }
}
