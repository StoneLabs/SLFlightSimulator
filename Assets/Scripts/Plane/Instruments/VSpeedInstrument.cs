using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSpeedInstrument : Instrument
{
    public Transform needle;
    [Range(-90, 90)]
    public float dregreePerFeet = 9;

    void Update()
    {
        needle.localRotation = Quaternion.Euler(-90, manager.physics.body.velocity.y, 0);
    }
}
