using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Plane Physics Manager
/// </summary>
public class PlanePhysics : MonoBehaviour
{
    // References
    public Rigidbody body; // Phyics body reference
    public PlaneManager manager;
    public List<AeroSurface> surfaces = new List<AeroSurface>();
    public List<AeroEngine> engines = new List<AeroEngine>();

    /// <summary>
    /// Dry mass ob plane. Set by Start weight of body
    /// </summary>
    public float DryMass
    {
        get;
        private set;
    }

    /// <summary>
    /// Current Airspeed of plane
    /// </summary>
    public Vector3 AirSpeed
    {
        get
        {
            return body.velocity - manager.environment.CalculateWind(body.position);
        }
    }

    /// <summary>
    /// Current heading of plane
    /// </summary>
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

    /// <summary>
    /// Current G-Force on plane
    /// </summary>
    public Vector3 GForce { get; private set; }
    

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float spawnFuel;

    void Start()
    {
        // Start plane with initial speed
        GetComponent<Rigidbody>().AddForce(transform.forward * manager.respawnVelocity, ForceMode.VelocityChange);
        DryMass = body.mass;
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        spawnFuel = manager.fuelLevel;
    }

    private Vector3 lastFrameVelocity;
    private void FixedUpdate()
    {
        // Calculate G force based on change of velocity over frame
        GForce = ((lastFrameVelocity - body.velocity) / (Time.fixedDeltaTime) + Physics.gravity) / Physics.gravity.magnitude;
        lastFrameVelocity = body.velocity;
    }

    /// <summary>
    /// Respawns plane at given spawn point. Some energy may be preserved of the respawn. 
    /// </summary>
    /// <param name="spawn">Selected spawnpoint</param>
    public void Respawn(RespawnPoint spawn)
    {
        // Turn off smoke to prevent trail following plane to new respawn
        manager.SetSmoke(false);

        // Respawn plane by setting position, rotation, velocity, etc.
        FreezeSimulation(false);

        manager.fuelLevel = spawnFuel;
        body.MovePosition(spawn.transform.position);
        body.MoveRotation(spawn.transform.rotation);
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(spawn.spawnInMotion ? transform.forward * manager.respawnVelocity : Vector3.zero, ForceMode.VelocityChange);

        foreach (AeroEngine engine in engines)
            engine.Respawn();
    }

    /// <summary>
    /// Freezes simulation of plane
    /// </summary>
    /// <param name="state">simulation state</param>
    public void FreezeSimulation(bool state = true)
    {
        body.isKinematic = state;
    }

    // Apply physics calculation
    public void applyPhysics()
    {
        // Change weight based on fuel
        body.mass = DryMass + manager.fuelLevel * manager.fuelWeight;

        // Apply engine force
        foreach (AeroEngine engine in engines)
            body.AddForceAtPosition(engine.Thrust, engine.ThrustLocation.position);

        // Apply aeroSurface forces
        foreach (AeroSurface surface in surfaces)
        {
            body.AddForceAtPosition(surface.LiftForce, surface.transform.position);
            body.AddForceAtPosition(surface.DragForce, surface.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize G-Force
#if UNITY_EDITOR
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, GForce, GForce.magnitude, Color.magenta);
#endif
    }
}
