using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    public enum ControlMode { None, Pitch, Yaw, Roll }
    public ControlMode controlMode;

    public float SurfaceArea
    {
        get
        {
            return transform.lossyScale.x * transform.lossyScale.z;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Range(0, 2 * Mathf.PI)]
    public float simulatedThrust;
    [Range(0, 2*Mathf.PI)]
    public float simulatedDrag;

    public Vector3 wind;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (controlMode == ControlMode.None)
            Gizmos.color = new Color(0.529f, 0.808f, 0.922f, 0.6f);
        else
            Gizmos.color = new Color(0.906f, 0.576f, 0.443f, 0.6f);

        // Width and depth of surface
        float width = transform.lossyScale.x, depth = transform.lossyScale.z;

        // Rotate Gizmo with object
        GizmosUtils.SetTR(transform);
        GizmosUtils.DrawPlane(Vector3.zero, new Vector2(width, depth), Color.black);

        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, wind, simulatedDrag, Color.red);
        GizmosUtils.DrawArrow(Vector3.zero, Vector3.Cross(transform.right, wind), simulatedThrust, Color.blue);
        GizmosUtils.DrawArrow(Vector3.zero, wind, 1, Color.green);
        GizmosUtils.DrawArrow(Vector3.zero, transform.forward, 1, Color.yellow);
#endif
    }
}
