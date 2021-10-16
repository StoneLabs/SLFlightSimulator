using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fuel Instrument
/// </summary>
public class FuelInstrument : Instrument
{
    public Transform needle;
    [Range(-90, 90)]
    public float maxAngle = 45;

    void Update()
    {
        // Show percentage of Fuel in system
        needle.localRotation = Quaternion.Euler(-90, manager.FuelPercentage * maxAngle, 0);
    }
}
