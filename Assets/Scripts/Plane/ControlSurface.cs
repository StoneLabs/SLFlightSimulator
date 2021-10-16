using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be extended by all control elements. Rotates transform depending on steering input. Must be added to PlaneManager.
/// </summary>
public class ControlSurface : MonoBehaviour
{
    public enum ControlAxis { None, Pitch, Yaw, Roll}
    public ControlAxis axis;

    // Steering axis impact on Transform axis
    [Range(-5, 5)]
    public float ImpactX = 0.0f;
    [Range(-5, 5)]
    public float ImpactY = 0.0f;
    [Range(-5, 5)]
    public float ImpactZ = 0.0f;

    public virtual void control(float pitch, float yaw, float roll)
    {
        if (axis == ControlAxis.None)
            return;

        float input = (axis == ControlAxis.Pitch ? pitch : (axis == ControlAxis.Yaw ? yaw : roll));
        transform.localRotation = Quaternion.Euler(new Vector3(ImpactX * input, ImpactY * input, ImpactZ * input));
    }
}
