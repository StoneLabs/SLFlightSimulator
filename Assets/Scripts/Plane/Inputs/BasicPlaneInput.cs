using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class BasicPlaneInput : PlaneInput
{
    bool autoPilotEngaged = false;
    bool breakesEngaged = false;
    private void Update()
    {
        if (Input.GetKeyDown("t"))
            autoPilotEngaged = !autoPilotEngaged;
        if (Input.GetKeyDown("b"))
            breakesEngaged = !breakesEngaged;
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
    public override bool IsAutoPilot()
    {
        return autoPilotEngaged;
    }
}
