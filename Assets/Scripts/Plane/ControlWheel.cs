using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWheel : ControlSurface
{
    public PlaneManager manager;
    public TouchDownWheelDetector touchDownDetector;
    public Vector3 frontForce;
    public float baseForce = 100;

    private float input = 0;

    public float GizmosThrustDivider = 50.0f;

    public override void control(float pitch, float yaw, float roll)
    {
        if (axis == ControlAxis.None)
            return;

        input = (axis == ControlAxis.Pitch ? pitch : (axis == ControlAxis.Yaw ? yaw : roll));
    }

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
        manager.physics.body.AddForceAtPosition(ForceVector, transform.position);
    }

    public void OnDrawGizmos()
    {
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, ForceVector, ForceVector.magnitude / GizmosThrustDivider);
    }
}
