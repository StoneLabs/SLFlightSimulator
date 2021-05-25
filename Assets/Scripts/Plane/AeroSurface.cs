﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    public enum VisualizationColor { Static, Controlled }

    [Header("Surface settings")]
    public AeroProfile profile;

    [Header("Plane/World references")]
    public PlaneManager manager;

    [Header("Simulated Input")]
    public Vector3 simulatedWind;

    [Header("Visualization Settings")]
    public VisualizationColor controlMode;
    [Range(0, 1e4f)]
    public float GizmosLiftDivisor = 1e3f;
    [Range(0, 1e4f)]
    public float GizmosDragDivisor = 1e3f;
    [Space(10)]
    public bool showFront = false;
    public bool showWindSpeedFront = false;
    public bool showLocalSpaceCopy = false;
    public bool showWind = false;


    public Vector3 RelativePosition
    {
        get 
        {
            return transform.position - manager.physics.body.worldCenterOfMass;
        }
    }

    public Vector3 Wind
    {
        get
        {
            // Optimally:
            // WindSpeed = -velocity + wind - Vector3.Cross(angularVelocity, relativePosition)
            if (Application.isPlaying)
                return -manager.physics.body.velocity + Vector3.zero - profile.RotationWindImpact * Vector3.Cross(manager.physics.body.angularVelocity, RelativePosition);
            else
                return simulatedWind;
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
            float magnitude = 0.5f * manager.environment.CalculateDensity(transform.position.y) * (WindSpeedFront * WindSpeedFront) * profile.CA_Curve.Evaluate(AngleOfAttack) * SurfaceArea;
            return Vector3.Cross(transform.right, Wind).normalized * magnitude;
        }
    }

    public Vector3 DragForce
    {
        get
        {
            float magnitude = 0.5f * manager.environment.CalculateDensity(transform.position.y) * (WindSpeedFront * WindSpeedFront) * profile.CD_Curve.Evaluate(AngleOfAttack) * SurfaceArea;
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
        if (controlMode == VisualizationColor.Static)
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

            if (showWind)
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