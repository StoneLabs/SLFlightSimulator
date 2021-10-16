using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Component to follow a transform and keep the relative position</summary>
public class TransformFollower : MonoBehaviour
{
    public Transform reference;
    public bool followX;
    public bool followY;
    public bool followZ;

    private Vector3 offset;

    void Start()
    {
        // Save offset for later use
        offset = this.transform.position - reference.position;
    }

    void Update()
    {
        // Move along the object in desired axes
        if (followX)
            this.transform.position = new Vector3(reference.position.x + offset.x, this.transform.position.y, this.transform.position.z);
        if (followY)
            this.transform.position = new Vector3(this.transform.position.x, reference.position.y + offset.y, this.transform.position.z);
        if (followZ)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, reference.position.z + offset.z);
    }
}
