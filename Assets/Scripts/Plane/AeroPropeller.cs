using UnityEngine;

public class AeroPropeller : MonoBehaviour
{
    public AeroEngine engine;

    [Header("Performance")]
    public float PropellerLength = 1.0f;
    public float AngularDrag = 1.0f;
    public float CD = 1.0f;
    public float CT = 1.0f;

    [Header("Visualization (3D)")]
    public Transform propellerBone;
    public Vector3 propellerBoneFactor = new Vector3(0.01f, 0, 0);

    [Header("Visualization (Debug)")]
    [Range(0, 1e4f)]
    public float GizmosThrustDivider = 50;

    public float RPS
    {
        get
        {
            return (engine.RPM / engine.gearRatio) / 60.0f;
        }
    }

    public float VelocityTip
    {
        get
        {
            return (PropellerLength * PropellerLength) * (2 * Mathf.PI) * RPS;
        }
    }

    public float DiskArea
    {
        get
        {
            return PropellerLength * PropellerLength * Mathf.PI;
        }
    }

    public Vector3 Thrust
    { 
        get
        {
            float magnitude = engine.plane.environment.CalculateDensity(transform.position.y) * VelocityTip * VelocityTip * CT * DiskArea;
            return transform.forward * magnitude;
        }
    }

    public float CounterTorque
    {
        get
        {
            return engine.plane.environment.CalculateDensity(transform.position.y) * VelocityTip * VelocityTip * PropellerLength * CD;
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, Thrust, Thrust.magnitude / GizmosThrustDivider, Color.white);
        GizmosUtils.SetTR(transform);
        GizmosUtils.DrawArrow(Vector3.zero, Vector3.right, PropellerLength, Color.white);
        GizmosUtils.DrawArrow(Vector3.zero, Vector3.left, PropellerLength, Color.white);
        GizmosUtils.DrawArrow(Vector3.zero, Vector3.up, PropellerLength, Color.white);
        GizmosUtils.DrawArrow(Vector3.zero, Vector3.down, PropellerLength, Color.white);
#endif
    }

    //float Drag = engine.plane.environment.CalculateDensity(transform.position.y) * VelocityTip * CD;
}