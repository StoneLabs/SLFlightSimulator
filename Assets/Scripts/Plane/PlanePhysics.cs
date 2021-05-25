﻿using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 30, ForceMode.VelocityChange);
        DryMass = body.mass;
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
}