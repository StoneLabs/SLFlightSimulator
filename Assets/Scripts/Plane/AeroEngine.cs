using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroEngine : MonoBehaviour
{
    public PlaneManager plane;
    public AnimationCurve forceCurve;
    public AnimationCurve fuelConsumptionCurve;
    [Range(0, 1e4f)]
    public float GizmosThrustDivider = 50;

    public Vector3 Thrust
    {
        get
        {
            return transform.forward * forceCurve.Evaluate(plane.throttle);
        }
    }
    public float FuelConsumption
    {
        get
        {
            return fuelConsumptionCurve.Evaluate(plane.throttle);
        }
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, transform.forward, Thrust.magnitude / GizmosThrustDivider, Color.white);
#endif
    }
}
