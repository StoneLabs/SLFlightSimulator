using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelInstrument : Instrument
{
    public Transform needle;
    [Range(-90, 90)]
    public float maxAngle = 45;

    void Update()
    {
        needle.localRotation = Quaternion.Euler(-90, (manager.FuelPercentage * 2.0f - 1.0f) * maxAngle, 0);
    }
}
