using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wheel brake indicator instrument. Changes material based on wheel brake status
/// </summary>
public class WheelBrakeIndicatorInstrument : Instrument
{
    public MeshRenderer indicator;
    public Material onMaterial;
    public Material offMaterial;

    private void Update()
    {
        // Change material
        if (manager.WheelBreaks)
            indicator.material = onMaterial;
        else
            indicator.material = offMaterial;
    }
}
