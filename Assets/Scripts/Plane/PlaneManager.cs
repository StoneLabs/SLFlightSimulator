using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour
{
    [Header("Components")]
    public Environment environment;
    public PlanePhysics physics;
    public ControlSurface[] controlSurfaces;

    [Header("Inputs")]
    public PlaneInput input;
    public AutoPilot autoPilot;

    [Header("Fuel settings")]
    public float fuelCapacity = 40;
    public float fuelLevel = 20;
    public float fuelWeight = 1f;
    [Space(10)]
    public float fuelConsumptionMultiplier = 1;
    public float FuelPercentage
    {
        get
        {
            return Mathf.Clamp01(fuelLevel / fuelCapacity);
        }
    }

    public float Throttle { get; private set; }
    public float SteeringPitch { get; private set; }
    public float SteeringYaw { get; private set; }
    public float SteeringRoll { get; private set; }


    [Header("Debug settings")]
    public bool drawDebug = false;

    public void Update()
    {
        if (IsAutoPilot())
        {
            Throttle = autoPilot.GetThrottle();
            SteeringPitch = autoPilot.GetPitch();
            SteeringRoll = autoPilot.GetRoll();
            SteeringYaw = autoPilot.GetYaw();
        }
        else
        {
            Throttle = input.GetThrottle();
            SteeringPitch = input.GetPitch();
            SteeringRoll = input.GetRoll();
            SteeringYaw = input.GetYaw();
        }

        foreach (AeroEngine engine in physics.engines)
            fuelLevel -= engine.FuelConsumption * Time.deltaTime * fuelConsumptionMultiplier;
    }

    public void FixedUpdate()
    {
        foreach (ControlSurface surface in controlSurfaces)
            surface.control(SteeringPitch, SteeringYaw, SteeringRoll);

        physics.applyPhysics();
    }

    public void OnGUI()
    {
        if (!drawDebug)
            return;

        GUI.Box(new Rect(0, 150, 310, 340), "");
        int y = 150;
        GUI.Label(new Rect(5, y, 300, 400), "PLANE DEBUG INFORMATION");
        GUI.Label(new Rect(5, y += 40, 300, 400), $"World Position ({transform.position.x / 1000:F2}, {transform.position.z / 1000:F2})km");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Velocity: {GetComponent<Rigidbody>().velocity.magnitude * 3.6f:F2}km/h");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Altitude: {transform.position.y:F2}m ASL");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Plane Mass: {physics.body.mass:F2}kg ({physics.DryMass:F2}kg Dry)");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Fuel level: {fuelLevel:F1}L / {fuelCapacity:F1}L ({FuelPercentage*100.0f:F2}%)");
        GUI.Label(new Rect(5, y += 40, 300, 400), $"Temperature: {environment.CalculateTemperature(transform.position.y) - 273.15f:F2}C");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Pressure: {environment.CalculatePressure(transform.position.y):F2}pascal");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Density: {environment.CalculateDensity(transform.position.y):F2}kg/m3");
        GUI.Label(new Rect(5, y += 40, 300, 400), $"Throttle, Pitch, Roll, Yaw:");
        GUI.HorizontalSlider(new Rect(5, y += 20, 300, 40), Throttle, 0, 1);
        GUI.HorizontalSlider(new Rect(5, y += 20, 300, 40), SteeringPitch, -1, 1);
        GUI.HorizontalSlider(new Rect(5, y += 20, 300, 40), SteeringRoll, -1, 1);
        GUI.HorizontalSlider(new Rect(5, y += 20, 300, 40), SteeringYaw, -1, 1);
    }

    public bool IsAutoPilot()
    {
        return input.IsAutoPilot();
    }
}
