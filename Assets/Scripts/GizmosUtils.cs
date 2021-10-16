using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// Utilitiez for easier gizmos work
/// </summary>
public static class GizmosUtils
{
    /// <summary>
    /// Set Transform and rotation reference for following operations
    /// </summary>
    /// <param name="transform">Reference system</param>
    public static void SetTR(Transform transform)
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
    }

    /// <summary>
    /// Set Transform reference for following operations
    /// </summary>
    /// <param name="transform"></param>
    public static void SetT(Transform transform)
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
    }

    /// <summary>
    /// Draw Plane in space (x, z) plane
    /// </summary>
    /// <param name="center">Center of plane</param>
    /// <param name="size">size in (x, z) dimension</param>
    /// <param name="outlineColor">Edge color, null = dont change</param>
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

    /// <summary>
    /// Draw arrow in 3d space
    /// </summary>
    /// <param name="pos">Starting position in current reference frame</param>
    /// <param name="direction">Direction of arrow</param>
    /// <param name="length">Length of arrow</param>
    /// <param name="color">Color of arrow</param>
    /// <param name="arrowHeadLength">Length of head in relation to body</param>
    /// <param name="arrowHeadAngle">Angle of head lines from body</param>
    public static void DrawArrow(Vector3 pos, Vector3 direction, float length = 1.0f, Color? color = null, float arrowHeadLength = 0.15f, float arrowHeadAngle = 45.0f)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color ?? Gizmos.color;

        direction = length * direction.normalized;
        Gizmos.DrawRay(pos, direction);

        if (length * direction != Vector3.zero)
        {
            Vector3 right = Quaternion.LookRotation(length * direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(length * direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * length * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * length * arrowHeadLength);
        }

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// Draw line in 3d space
    /// </summary>
    /// <param name="from">Starting position in current reference system/param>
    /// <param name="to">End position in current reference system</param>
    /// <param name="color">Color, null = dont change</param>
    public static void DrawLine(Vector3 from, Vector3 to, Color? color = null)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color ?? Gizmos.color;

        Gizmos.DrawLine(from, to);

        Gizmos.color = oldColor;
    }
}
