using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be extended by all control elements. Rotates transform depending on steering input. Must be added to PlaneManager.
/// </summary>
public class ControlSurface : MonoBehaviour
{
    public enum ControlAxis { None, Pitch, Yaw, Roll, Flaps }
    public ControlAxis axis;

    // Steering axis impact on Transform axis
    [Range(-25, 25)]
    public float ImpactX = 0.0f;
    [Range(-25, 25)]
    public float ImpactY = 0.0f;
    [Range(-25, 25)]
    public float ImpactZ = 0.0f;

    public bool instantaneous = true;
    [Range(0, 10)]
    public float speed = 1f;

    public virtual void control(float pitch, float yaw, float roll, float flaps)
    {
        if (axis == ControlAxis.None)
            return;

        float input = (axis == ControlAxis.Pitch ? pitch : (axis == ControlAxis.Yaw ? yaw : (axis == ControlAxis.Flaps ? flaps : roll)));
        Quaternion targetRotation = Quaternion.Euler(new Vector3(ImpactX * input, ImpactY * input, ImpactZ * input));

        if (instantaneous)
            transform.localRotation = targetRotation;
        else
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Mathf.Clamp01(speed * Time.deltaTime));
    }
}
