using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSurface : MonoBehaviour
{
    public enum ControlAxis { None, Pitch, Yaw, Roll}
    public ControlAxis axis;

    [Range(-5, 5)]
    public float ImpactX = 0.0f;
    [Range(-5, 5)]
    public float ImpactY = 0.0f;
    [Range(-5, 5)]
    public float ImpactZ = 0.0f;

    public void control(float pitch, float yaw, float roll)
    {
        if (axis == ControlAxis.None)
            return;

        float input = (axis == ControlAxis.Pitch ? pitch : (axis == ControlAxis.Yaw ? yaw : roll));
        transform.localRotation = Quaternion.Euler(new Vector3(ImpactX * input, ImpactY * input, ImpactZ * input));
    }
}
