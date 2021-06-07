using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWheel : ControlSurface
{
    public PlaneManager manager;
    public float baseForce = 100;

    private float input = 0;
    private bool isTouchDown = false;

    public float GizmosThrustDivider = 50.0f;

    public override void control(float pitch, float yaw, float roll)
    {
        if (axis == ControlAxis.None)
            return;

        input = (axis == ControlAxis.Pitch ? pitch : (axis == ControlAxis.Yaw ? yaw : roll));
    }

    private void OnTriggerEnter(Collider other)
    {
        isTouchDown = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isTouchDown = false;
    }

    private Vector3 ForceVector
    {
        get
        {
            if (!isTouchDown)
                return Vector3.zero;
            return transform.forward * ImpactZ * baseForce * input +
                    transform.up     * ImpactY * baseForce * input +
                    transform.right  * ImpactX * baseForce * input;
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

    public void OnGUI()
    {
        GUI.Label(new Rect(500, 500, 500, 500), isTouchDown.ToString());
    }
}
