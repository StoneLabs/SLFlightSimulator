using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkedAutoPilot : AutoPilot
{
    private float throttle = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;
    private float yaw = 0.0f;

    void Start()
    {
    }

    void Update()
    {
    }


    public override float GetPitch()
    {
        return 0.0f;
    }

    public override float GetRoll()
    {
        return 0.0f;
    }

    public override float GetThrottle()
    {
        return 0.0f;
    }

    public override float GetYaw()
    {
        return 0.0f;
    }
}
