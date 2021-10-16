using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RPM Instrument. Visualizes engine RPM
/// </summary>
public class RPMInstrument : Instrument
{
    public int engine;
    public Transform needle;
    [Range(-90, 90)]
    public float dregreePerHRPM = 9f;

    void Update()
    {
        // Show RPM in hundret RPM
        needle.localRotation = Quaternion.Euler(90 + manager.physics.engines[engine].RPM / 100.0f * dregreePerHRPM, -90, -90);
    }
}