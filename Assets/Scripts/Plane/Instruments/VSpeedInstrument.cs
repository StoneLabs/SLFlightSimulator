﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSpeedInstrument : Instrument
{
    public Transform needle;
    [Range(-90, 90)]
    public float dregreePerFeet = 9;
    public bool clamp = true;

    void Update()
    {
        float hundretFeetPerMinute = UnitConverter.Meter2Feet(manager.physics.body.velocity.y) / 100 * 60;
        if (clamp)
            needle.localRotation = Quaternion.Euler(-90, Mathf.Clamp(hundretFeetPerMinute * dregreePerFeet, -175, 175), 0);
        else
            needle.localRotation = Quaternion.Euler(-90, hundretFeetPerMinute * dregreePerFeet, 0);
    }
}
