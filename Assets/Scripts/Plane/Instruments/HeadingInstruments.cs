using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heading instrument. Visualizes heading using disk rotator
/// </summary>
public class HeadingInstruments : Instrument
{
    public Transform needle;
    [Range(-90, 90)]
    public float degreeOffset = 45f;
    [Range(-90, 90)]
    public float degreeperDegree = -1f;

    void Update()
    {
        // Rotate disk as required
        needle.localRotation = Quaternion.Euler(90 + manager.physics.Heading * degreeperDegree + degreeOffset, -90, -90);
    }
}
