using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPilotIndicatorInstrument : Instrument
{
    public MeshRenderer indicator;
    public Material onMaterial;
    public Material offMaterial;

    private void Update()
    {
        if (manager.IsAutoPilot())
            indicator.material = onMaterial;
        else
            indicator.material = offMaterial;
    }
}
