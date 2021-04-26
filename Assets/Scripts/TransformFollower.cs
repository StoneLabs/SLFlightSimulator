using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    public Transform reference;
    public bool followX;
    public bool followY;
    public bool followZ;

    private Vector3 offset;

    void Start()
    {
        offset = this.transform.position - reference.position;
    }

    void Update()
    {
        if (followX)
            this.transform.position = new Vector3(reference.position.x + offset.x, this.transform.position.y, this.transform.position.z);
        if (followY)
            this.transform.position = new Vector3(this.transform.position.x, reference.position.y + offset.y, this.transform.position.z);
        if (followZ)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, reference.position.z + offset.z);
    }
}
