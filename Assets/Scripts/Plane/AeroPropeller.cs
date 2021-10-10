using UnityEngine;

public class AeroPropeller : MonoBehaviour
{
    public AeroEngine engine;

    [Header("Performance")]
    public float PropellerLength = 1.0f;
    public float AngularDrag = 1.0f;
    public float CD = 1.0f;
    public float CT = 1.0f;
    public float Thickness = 0.1f;
    public int blades;

    [Header("Visualization (3D)")]
    public Transform propellerBone;

    [Header("Visualization (Debug)")]
    [Range(0, 1e4f)]
    public float GizmosThrustDivider = 50;



    private void Update()
    {
        propellerBone.Rotate(Vector3.up, RPS * 360.0f * Time.deltaTime, Space.Self);
    }

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
            return PropellerLength * (2 * Mathf.PI) * RPS;
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
            float magnitude = 0.5f * blades * engine.plane.environment.CalculateDensity(transform.position.y) * VelocityTip * VelocityTip * PropellerLength * Thickness * CT;
            return transform.forward * magnitude;
        }
    }

    public float CounterTorque
    {
        get
        {
            // Calculate drag of propeller blades and multiply with the length of the propeller to attain torque in Newton meters
            return PropellerLength * (0.5f * blades * engine.plane.environment.CalculateDensity(transform.position.y) * VelocityTip * VelocityTip * PropellerLength * Thickness * CD);
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