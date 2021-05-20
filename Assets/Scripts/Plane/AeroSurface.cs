using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    public enum ControlMode { None, Pitch, Yaw, Roll }
    public ControlMode controlMode;
    public AnimationCurve CA_Curve;
    public AnimationCurve CD_Curve;
    [Range(0, 1.0f)]
    [Description("More realistic, but can lead to oscillation")]
    public float RotationWindImpact = 0.1f;
    public Rigidbody plane;
    public Environment environment;

    [Header("Simulated Input")]
    public Vector3 simulatedWind;

    [Header("Visualization Settings")]
    [Range(0, 1e4f)]
    public float GizmosLiftDivisor = 1e3f;
    [Range(0, 1e4f)]
    public float GizmosDragDivisor = 1e3f;
    public bool useSimulatedWind = false;
    public bool useRealWindInGame = true;
    [Space(10)]
    public bool showFront = false;
    public bool showWindSpeedFront = false;
    public bool showLocalSpaceCopy = false;


    // WindSpeed = -velocity + wind - Vector3.Cross(angularVelocity, relativePosition)

    public void Start()
    {
        if (useRealWindInGame)
            useSimulatedWind = false;
    }

    public Vector3 RelativePosition
    {
        get 
        {
            return transform.position - plane.worldCenterOfMass;
        }
    }

    public Vector3 Wind
    {
        get
        {
            if (useSimulatedWind)
                return simulatedWind;
            else
                return -plane.velocity + Vector3.zero - RotationWindImpact * Vector3.Cross(plane.angularVelocity, RelativePosition);
        }
    }

    public float AngleOfAttack
    {
        get
        {
            return Vector3.SignedAngle(
                transform.forward, 
                Vector3.ProjectOnPlane(-Wind, transform.right), 
                transform.right);
        }
    }

    public float WindSpeedFront
    {
        get
        {
            return Vector3.ProjectOnPlane(-Wind, transform.right).magnitude;
        }
    }

    public Vector3 LiftForce
    {
        get
        {
            float magnitude = 0.5f * environment.CalculateDensity(0) * (WindSpeedFront * WindSpeedFront) * CA_Curve.Evaluate(AngleOfAttack) * SurfaceArea;
            return Vector3.Cross(transform.right, Wind).normalized * magnitude;
        }
    }

    public Vector3 DragForce
    {
        get
        {
            float magnitude = 0.5f * environment.CalculateDensity(0) * (WindSpeedFront * WindSpeedFront) * CD_Curve.Evaluate(AngleOfAttack) * SurfaceArea;
            return Wind.normalized * magnitude;
        }
    }

    public float SurfaceArea
    {
        get
        {
            return transform.lossyScale.x * transform.lossyScale.z;
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (controlMode == ControlMode.None)
            Gizmos.color = new Color(0.529f, 0.808f, 0.922f, 0.6f);
        else
            Gizmos.color = new Color(0.906f, 0.576f, 0.443f, 0.6f);

        // Width and depth of surface
        float width = transform.lossyScale.x, depth = transform.lossyScale.z;

        // Project wind on UP/FORWARD plane of wing
        Vector3 WindProjection = Vector3.ProjectOnPlane(-Wind, transform.right);


        void renderSurface()
        {
            GizmosUtils.DrawPlane(Vector3.zero, new Vector2(width, depth), Color.black);
        }

        void renderArrows()
        {
            GizmosUtils.DrawArrow(Vector3.zero, DragForce, DragForce.magnitude / GizmosDragDivisor, Color.red);
            GizmosUtils.DrawArrow(Vector3.zero, LiftForce, LiftForce.magnitude / GizmosLiftDivisor, Color.blue);

            if (showFront)
                GizmosUtils.DrawArrow(Vector3.zero, transform.forward, 1, Color.yellow);

            GizmosUtils.DrawArrow(-Wind, Wind, Wind.magnitude, Color.green);
            if (showWindSpeedFront)
            {
                GizmosUtils.DrawLine(transform.forward, WindProjection.normalized, Color.grey);
                GizmosUtils.DrawArrow(WindProjection, -WindProjection, WindProjection.magnitude, Color.magenta);
            }
        }

        // Render plane with rotation, arrows global space
        GizmosUtils.SetTR(transform);
        renderSurface();
        GizmosUtils.SetT(transform);
        renderArrows();


        // Render plane static with rotating forces
        if (showLocalSpaceCopy)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position + Vector3.right * 4, Quaternion.identity, Vector3.one);
            renderSurface();
            Gizmos.matrix = Matrix4x4.TRS(transform.position + Vector3.right * 4, Quaternion.Inverse(transform.rotation), Vector3.one);
            renderArrows();
        }
#endif
    }
}
