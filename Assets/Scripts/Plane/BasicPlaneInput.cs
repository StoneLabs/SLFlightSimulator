using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlaneInput : MonoBehaviour
{
    public float GetThrottle()
    {
        return (Input.GetAxis("Throttle") + 1) / 2.0f;
    }
    public float GetPitch()
    {
        return Input.GetAxis("Vertical");
    }
    public float GetYaw()
    {
        return Input.GetAxis("QE");
    }
    public float GetRoll()
    {
        return Input.GetAxis("Horizontal");
    }
}
