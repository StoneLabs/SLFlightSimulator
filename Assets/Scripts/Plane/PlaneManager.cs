using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour
{
    [Header("Components")]
    public Environment environment;
    public PlanePhysics physics;
    public BasicPlaneInput input;
    public ControlSurface[] controlSurfaces;

    [Header("Fuel settings")]
    public float fuelCapacity = 1000;
    public float fuelLevel = 500;
    public float fuelConsumptionMultiplier = 1;


    public float throttle
    {
        get;
        private set;
    }
    float steeringPitch;
    float steeringYaw;
    float steeringRoll;


    [Header("Debug settings")]
    public bool drawDebug = false;

    public void Update()
    { 
        throttle = input.GetThrottle();
        steeringPitch = input.GetPitch();
        steeringRoll = input.GetRoll();
        steeringYaw = input.GetYaw();

        foreach (AeroEngine engine in physics.engines)
            fuelLevel -= engine.FuelConsumption * Time.deltaTime * fuelConsumptionMultiplier;
    }

    public void FixedUpdate()
    {
        foreach (ControlSurface surface in controlSurfaces)
            surface.control(steeringPitch, steeringYaw, steeringRoll);

        physics.applyForces();
    }

    public void OnGUI()
    {
        if (!drawDebug)
            return;

        int y = 150;
        GUI.Label(new Rect(0, y, 400, 400), $"Velocity: {GetComponent<Rigidbody>().velocity.magnitude * 3.6f:F2}km/h");
        GUI.Label(new Rect(0, y += 20, 400, 400), $"Altitude: {transform.position.y:F2}m ASL");
        GUI.Label(new Rect(0, y += 40, 400, 400), $"Temperature: {environment.CalculateTemperature(transform.position.y) - 273.15f:F2}C");
        GUI.Label(new Rect(0, y += 20, 400, 400), $"Pressure: {environment.CalculatePressure(transform.position.y):F2}pascal");
        GUI.Label(new Rect(0, y += 20, 400, 400), $"Density: {environment.CalculateDensity(transform.position.y):F2}kg/m3");
        GUI.HorizontalSlider(new Rect(0, y += 40, 200, 40), throttle, 0, 1);
        GUI.HorizontalSlider(new Rect(0, y += 20, 200, 40), steeringPitch, -1, 1);
        GUI.HorizontalSlider(new Rect(0, y += 20, 200, 40), steeringRoll, -1, 1);
        GUI.HorizontalSlider(new Rect(0, y += 20, 200, 40), steeringYaw, -1, 1);
        GUI.HorizontalSlider(new Rect(0, y += 20, 200, 40), fuelLevel/fuelCapacity, -1, 1);
    }
}
