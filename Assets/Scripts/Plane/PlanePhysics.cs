using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    public Rigidbody body;
    public PlaneManager manager;
    public List<AeroSurface> surfaces = new List<AeroSurface>();
    public List<AeroEngine> engines = new List<AeroEngine>();

    public float DryMass
    {
        get;
        private set;
    }
    public Vector3 AirSpeed
    {
        get
        {
            return body.velocity - manager.environment.CalculateWind(body.position);
        }
    }
    public float Heading
    {
        get
        {
            float angleNorth = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up);
            if (angleNorth < 0)
                angleNorth = (angleNorth + 90.0f) + 270.0f;
            return angleNorth;
        }
    }
    public Vector3 GForce { get; private set; }
    

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float spawnFuel;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 30, ForceMode.VelocityChange);
        DryMass = body.mass;
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        spawnFuel = manager.fuelLevel;
    }

    private Vector3 lastFrameVelocity;
    private void FixedUpdate()
    {
        GForce = ((lastFrameVelocity - body.velocity) / (Time.fixedDeltaTime) + Physics.gravity) / Physics.gravity.magnitude;
        lastFrameVelocity = body.velocity;
    }

    public void Respawn()
    {
        manager.fuelLevel = spawnFuel;
        body.MovePosition(spawnPosition);
        body.MoveRotation(spawnRotation);
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 30, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    public void applyPhysics()
    {
        body.mass = DryMass + manager.fuelLevel * manager.fuelWeight;

        foreach (AeroEngine engine in engines)
            body.AddForceAtPosition(engine.Thrust, engine.transform.position);

        foreach (AeroSurface surface in surfaces)
        {
            body.AddForceAtPosition(surface.LiftForce, surface.transform.position);
            body.AddForceAtPosition(surface.DragForce, surface.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, GForce, GForce.magnitude, Color.magenta);
#endif
    }
}
