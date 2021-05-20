using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    public List<AeroSurface> surfaces = new List<AeroSurface>();
    public Transform backRudder;
    public Transform leftRudder;
    public Transform rightRudder;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 30, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody plane = GetComponent<Rigidbody>();

        //plane.AddForce(transform.up * 6000);
        plane.AddForce(transform.forward * 500);
        foreach (AeroSurface surface in surfaces)
        {
            plane.AddForceAtPosition(surface.LiftForce, surface.transform.position);
            plane.AddForceAtPosition(surface.DragForce, surface.transform.position);
        }

        leftRudder.localRotation = Quaternion.identity;
        rightRudder.localRotation = Quaternion.identity;
        backRudder.localRotation = Quaternion.identity;
        if (Input.GetKey(KeyCode.A))
        {
            leftRudder.localRotation = Quaternion.Euler(new Vector3(3, 0, 0));
            rightRudder.localRotation = Quaternion.Euler(new Vector3(-3, 0, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            leftRudder.localRotation = Quaternion.Euler(new Vector3(-3, 0, 0));
            rightRudder.localRotation = Quaternion.Euler(new Vector3(3, 0, 0));
        }

        if (Input.GetKey(KeyCode.S))
            backRudder.localRotation = Quaternion.Euler(new Vector3(5, 0, 0));
        if (Input.GetKey(KeyCode.W))
            backRudder.localRotation = Quaternion.Euler(new Vector3(-5, 0, 0));
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 150, 400, 400), $"Velocity: {GetComponent<Rigidbody>().velocity.magnitude * 3.6}km/h");
    }
}
