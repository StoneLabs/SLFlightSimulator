using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AeroEngine : MonoBehaviour
{
    public PlaneManager plane;

    [Header("Performance")]
    public AnimationCurve forceCurve;

    public float idleRPM = 1400f;
    public float maxRPM = 4500f;

    public float spinUpSpeedFactor = 1f;
    public float spinDownSpeedFactor = 0.5f;

    [Header("Fuel")]
    [Description("Fuel Consumption in Liter/Minute")]
    public AnimationCurve fuelConsumptionCurve;
    [Range(0, 1)]
    public float fuelStarvePercentage = 0.02f;

    [Header("Visualization")]
    [Range(0, 1e4f)]
    public float GizmosThrustDivider = 50;

    public float RPM { get; private set; }

    public float TargetRPM
    {
        get
        {
            return plane.Throttle * (maxRPM - idleRPM) + idleRPM;
        }
    }
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
            return transform.forward * forceCurve.Evaluate(RPM);
        }
    }
    public float FuelConsumption
    {
        get
        {
            if (Starved)
                return 0.0f;
            return fuelConsumptionCurve.Evaluate(RPM) / 60.0f;
        }
    }

    private void Start()
    {
        RPM = idleRPM;
    }

    void Update()
    {
        if (TargetRPM >= RPM)
            RPM = Mathf.Lerp(RPM, TargetRPM, spinUpSpeedFactor * Time.deltaTime);
        else
            RPM = Mathf.Lerp(RPM, TargetRPM, spinDownSpeedFactor * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, transform.forward, Thrust.magnitude / GizmosThrustDivider, Color.white);
#endif
    }
}
