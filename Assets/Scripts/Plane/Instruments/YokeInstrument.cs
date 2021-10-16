using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Yoke instrument. Visualizes and simulated yoke based in steering.
/// </summary>
public class YokeInstrument : Instrument
{
    public Transform mainStick;
    [Range(0, 90.0f)]
    public float mainStickRollMultiplier = 45.0f;
    [Range(-0.002f, 0.002f)]
    public float mainStickPitchMultiplier = 0.001f;

    void Update()
    {
        // Rotate and move yoke as required.
        mainStick.localRotation = Quaternion.Euler(new Vector3(90 + manager.SteeringRoll * mainStickRollMultiplier, -90, -90));
        mainStick.localPosition = new Vector3(0, 0, manager.SteeringPitch * mainStickPitchMultiplier);
    }
}
