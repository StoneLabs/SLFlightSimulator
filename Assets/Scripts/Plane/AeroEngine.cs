using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroEngine : MonoBehaviour
{
    public PlaneManager plane;
    public AnimationCurve forceCurve;
    public AnimationCurve fuelConsumptionCurve;
    [Range(0, 1)]
    public float fuelStarvePercentage = 0.02f;
    [Range(0, 1e4f)]
    public float GizmosThrustDivider = 50;

    public bool Starved
    {
        get
        {
            return plane.FuelPercentage < fuelStarvePercentage;
        }
    }
    public Vector3 Thrust
    {
        get
        {
            if (Starved)
                return transform.forward * 0.0f;
            return transform.forward * forceCurve.Evaluate(plane.throttle);
        }
    }
    public float FuelConsumption
    {
        get
        {
            if (Starved)
                return 0.0f;
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
