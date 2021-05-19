using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosUtils
{
    public static void SetTR(Transform transform)
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
    }
    public static void SetT(Transform transform)
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
    }

    public static void DrawPlane(Vector3 center, Vector2 size, Color? outlineColor = null)
    {
        Color oldColor = Gizmos.color;
        Gizmos.DrawCube(center, new Vector3(size.x, 0.001f, size.y));

        Gizmos.color = outlineColor ?? Gizmos.color;
        Gizmos.DrawLine(new Vector3(size.x / 2, 0, size.y / 2), new Vector3(size.x / 2, 0, -size.y / 2));
        Gizmos.DrawLine(new Vector3(size.x / 2, 0, size.y / 2), new Vector3(-size.x / 2, 0, size.y / 2));
        Gizmos.DrawLine(new Vector3(-size.x / 2, 0, -size.y / 2), new Vector3(-size.x / 2, 0, size.y / 2));
        Gizmos.DrawLine(new Vector3(-size.x / 2, 0, -size.y / 2), new Vector3(size.x / 2, 0, -size.y / 2));

        Gizmos.color = oldColor;
    }

    public static void DrawArrow(Vector3 pos, Vector3 direction, float length = 1.0f, Color? color = null, float arrowHeadLength = 0.15f, float arrowHeadAngle = 45.0f)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color ?? Gizmos.color;

        direction = Vector3.LerpUnclamped(pos, direction.normalized, length);
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * length * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * length * arrowHeadLength);

        Gizmos.color = oldColor;
    }
}
