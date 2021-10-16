using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control wheel. Applies force depending on yaw steer to simulator wheel rotation. Not physically proper but functional.
/// </summary>
public class ControlWheel : ControlSurface
{
    // References
    public PlaneManager manager;
    public TouchDownWheelDetector touchDownDetector;
    public Vector3 frontForce;
    public float baseForce = 100;

    private float input = 0;

    public float GizmosThrustDivider = 50.0f;

    // Override/implement ControlSurface class's control function for steering information
    public override void control(float pitch, float yaw, float roll)
    {
        if (axis == ControlAxis.None)
            return;

        input = (axis == ControlAxis.Pitch ? pitch : (axis == ControlAxis.Yaw ? yaw : roll));
    }

    // Force vector calculated based on steer and settings
    private Vector3 ForceVector
    {
        get
        {
            if (!touchDownDetector.IsTouchDown)
                return Vector3.zero;

            return (transform.forward * ImpactZ * input + transform.forward * frontForce.z * Mathf.Abs(input) +
                    transform.up      * ImpactY * input + transform.up      * frontForce.y * Mathf.Abs(input) +
                    transform.right   * ImpactX * input + transform.right   * frontForce.x * Mathf.Abs(input))
                    * baseForce;
        }
    }
    public void FixedUpdate()
    {
        // Apply force
        manager.physics.body.AddForceAtPosition(ForceVector, transform.position);
    }

    public void OnDrawGizmos()
    {
        // Visualize force
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, ForceVector, ForceVector.magnitude / GizmosThrustDivider);
    }
}
