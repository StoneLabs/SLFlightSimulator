using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AirSpeedInstrument. Visualizes Airspeed.
/// </summary>
public class AirSpeedInstrument : Instrument
{
    public Transform needle;
    [Range(-90, 90)]
    public float dregreePerFeet = 1.5f;

    void Update()
    {
        // Show airspeed in Knots
        float airSpeed = UnitConverter.MeterPerSecond2Knots(manager.physics.AirSpeed.magnitude);
        needle.localRotation = Quaternion.Euler(90 + airSpeed * dregreePerFeet, -90, -90);
    }
}
