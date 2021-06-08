﻿using UnityEngine;
using UnityEngine.SocialPlatforms;

class WheelSlipPreventer : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public PlaneManager manager;
    public TouchDownWheelDetector touchDownDetector;

    public Axis slipAxis;
    public float CounterForceMagnitude = 1000;
    public float GizmosThrustDivider = 50;


    public Vector3 LocalSlip
    {
        get
        {
            if (!touchDownDetector.IsTouchDown)
                return Vector3.zero;

            Vector3 worldVelocity = manager.physics.body.GetPointVelocity(transform.position) - manager.physics.body.velocity;
            Vector3 localVelocity = transform.worldToLocalMatrix * worldVelocity; switch (slipAxis)
            {
                case Axis.X:
                    return new Vector3(localVelocity.x, 0, 0);
                case Axis.Y:
                    return new Vector3(0, localVelocity.y, 0);
                case Axis.Z:
                    return new Vector3(0, 0, localVelocity.z);
                default:
                    throw new System.Exception("Illegal State in WheelSlipPreventer");
            }
        }
    }
    public Vector3 CounterSlipForce
    {
        get
        {
            return transform.parent.rotation * -LocalSlip * CounterForceMagnitude;
        }
    }

    public void FixedUpdate()
    {
        manager.physics.body.AddForceAtPosition(CounterSlipForce, transform.position);
    }

    public void OnDrawGizmos()
    {
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, CounterSlipForce, CounterSlipForce.magnitude / GizmosThrustDivider);
    }
}
