using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody plane = GetComponent<Rigidbody>();
        plane.AddForce(transform.up * 10000);
        plane.AddForce(transform.forward * 4000);

        if (Input.GetKey(KeyCode.A))
        {
            plane.AddForceAtPosition(transform.up * 700, transform.TransformPoint(Vector3.right));
            plane.AddForceAtPosition(transform.up * -700, transform.TransformPoint(Vector3.left));
        }
        if (Input.GetKey(KeyCode.D))
        {
            plane.AddForceAtPosition(transform.up * -700, transform.TransformPoint(Vector3.right));
            plane.AddForceAtPosition(transform.up * 700, transform.TransformPoint(Vector3.left));
        }
        if (Input.GetKey(KeyCode.W))
            plane.AddForceAtPosition(transform.up * 300, transform.TransformPoint(Vector3.back));

        if (Input.GetKey(KeyCode.S))
            plane.AddForceAtPosition(transform.up * -300, transform.TransformPoint(Vector3.back));
    }
}
