using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

/// <summary>
/// Basic plane Input. Input based on Unity axes. Allows for keyboard and controller input.
/// </summary>
public class BasicPlaneInput : PlaneInput
{
    bool autoPilotEngaged = false;
    bool breakesEngaged = false;
    bool flapsEngaged = false;
    private void Update()
    {
        if (Input.GetKeyDown("t"))
            autoPilotEngaged = !autoPilotEngaged;
        if (Input.GetKeyDown("b"))
            breakesEngaged = !breakesEngaged;
        if (Input.GetKeyDown("f"))
            flapsEngaged = !flapsEngaged;
    }

    public override float GetThrottle()
    {
        return (Input.GetAxis("Throttle") + 1) / 2.0f;
    }
    public override float GetPitch()
    {
        return Input.GetAxis("Vertical");
    }
    public override float GetYaw()
    {
        return Input.GetAxis("QE");
    }
    public override float GetRoll()
    {
        return Input.GetAxis("Horizontal");
    }
    public override bool GetBreak()
    {
        return breakesEngaged;
    }
    public override bool GetFlaps()
    {
        return flapsEngaged;
    }
    public override bool IsAutoPilot()
    {
        return autoPilotEngaged;
    }
}
