using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    [Range(0, 3)]
    public float width = 0;
    [Range(0, 3)]
    public float height = 0;

    public float SurfaceArea
    {
        get
        {
            return transform.lossyScale.x * width * transform.lossyScale.z * height;
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


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, new Vector3(width, 0.001f, height));
#endif
    }
}
