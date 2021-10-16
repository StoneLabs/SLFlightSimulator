using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Altitude Instrument. Visualizes Altitude using needle.
/// </summary>
public class AltitudeInstrument : Instrument
{
    public Transform needleMajor;
    public Transform needleMinor;
    [Range(-90, 90)]
    public float dregreePer100Feet = 36;

    void Update()
    {
        // Show altitude in 100 feet
        float altitudeFeet = UnitConverter.Meter2Feet(manager.physics.body.position.y);
        needleMajor.localRotation = Quaternion.Euler(90 + altitudeFeet / 100.0f * dregreePer100Feet, -90, -90);
        needleMinor.localRotation = Quaternion.Euler(90 + altitudeFeet / 1000.0f * dregreePer100Feet, -90, -90);
    }
}
