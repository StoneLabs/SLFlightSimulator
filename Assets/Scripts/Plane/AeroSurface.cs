﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    public enum ControlMode { None, Pitch, Yaw, Roll }
    public ControlMode controlMode;
    public AnimationCurve CA_Curve;
    public Environment environment;

    [Header("Simulated Input")]
    public Vector3 simulatedWind;
    [Range(0, 2 * Mathf.PI)]
    public float simulatedDrag;

    [Header("Visualization Settings")]
    [Range(0, 1e4f)]
    public float GizmosLiftDivisor = 1e3f;
    public bool useSimulatedWind = false;
    [Space(10)]
    public bool showFront = false;
    public bool showWindSpeedFront = false;
    public bool showLocalSpaceCopy = false;

    // WindSpeed = -velocity + wind - Vector3.Cross(angularVelocity, relativePosition)

    public Vector3 Wind
    {
        get
        {
            if (useSimulatedWind)
                return simulatedWind;
            else
                return Vector3.zero;
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

    public float LiftForce
    {
        get
        {
            return 0.5f * environment.CalculateDensity(0) * (WindSpeedFront * WindSpeedFront) * CA_Curve.Evaluate(AngleOfAttack) * SurfaceArea;
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
            GizmosUtils.DrawArrow(Vector3.zero, Wind, simulatedDrag, Color.red);
            GizmosUtils.DrawArrow(Vector3.zero, Vector3.Cross(transform.right, Wind), LiftForce / GizmosLiftDivisor, Color.blue);

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
