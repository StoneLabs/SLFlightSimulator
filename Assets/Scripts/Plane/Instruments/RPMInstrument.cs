using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPMInstrument : Instrument
{
    public int engine;
    public Transform needle;
    [Range(-90, 90)]
    public float dregreePerFeet = 9f;

    void Update()
    {
        needle.localRotation = Quaternion.Euler(90 + manager.physics.engines[engine].RPM / 100.0f * dregreePerFeet, -90, -90);
    }
}