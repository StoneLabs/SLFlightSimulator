using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    public List<AeroSurface> surfaces = new List<AeroSurface>();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 30, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    public void applyForces(float throttle)
    {
        Rigidbody plane = GetComponent<Rigidbody>();

        //plane.AddForce(transform.up * 6000);
        plane.AddForce(transform.forward * throttle * 500);
        foreach (AeroSurface surface in surfaces)
        {
            plane.AddForceAtPosition(surface.LiftForce, surface.transform.position);
            plane.AddForceAtPosition(surface.DragForce, surface.transform.position);
        }
    }
}
