using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

/// <summary>
/// PlaneManager. Centeral manager for aircraft.
/// </summary>
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

    [Header("Aerobatic Smoke")]
    public TrailRenderer[] aerobaticEmitters;
    public bool AerobaticSmokeStart = false;
    public bool AerobaticSmoke { get; private set; } = false; // Needs SetSmokeEmmiters() call to apply

    // Steering axes names by intended effect
    public float Throttle { get; private set; }
    public float SteeringPitch { get; private set; }
    public float SteeringYaw { get; private set; }
    public float SteeringRoll { get; private set; }
    public bool WheelBreaks { get; private set; }

    [Header("Wheels")]
    public PhysicMaterial WheelMaterial;
    public float WheelBrakeMultiplier = 10;
    public float wheelBrakeBaseDynamicFriction = 0.01f;
    public float wheelBrakeBaseStaticFriction = 0.02f;

    [Header("Respawn")]
    public RespawnPoint[] respawnPoints;
    public float respawnVelocity;
    public bool Crashed { get; private set; } = false;

    [Header("Debug settings")]
    public bool drawDebug = false;

    public void Start()
    {
        // Set friction values for wheelbrake to default
        WheelMaterial.dynamicFriction = wheelBrakeBaseDynamicFriction;
        WheelMaterial.staticFriction = wheelBrakeBaseStaticFriction;

        // Set smoke at start
        AerobaticSmoke = AerobaticSmokeStart;
        SetSmokeEmmiters();
    }

    public void OnApplicationQuit()
    {
        // Change back friction as this operation is persistant over simulation stops.... 
        WheelMaterial.dynamicFriction = wheelBrakeBaseDynamicFriction;
        WheelMaterial.staticFriction = wheelBrakeBaseStaticFriction;
    }

    public void Update()
    {
        // Get steering inputs from either autopilot or input manager.
        if (IsAutoPilot())
        {
            Throttle = autoPilot.GetThrottle();
            SteeringPitch = autoPilot.GetPitch();
            SteeringRoll = autoPilot.GetRoll();
            SteeringYaw = autoPilot.GetYaw();
            WheelBreaks = autoPilot.GetBrake();
        }
        else
        {
            Throttle = input.GetThrottle();
            SteeringPitch = input.GetPitch();
            SteeringRoll = input.GetRoll();
            SteeringYaw = input.GetYaw();
            WheelBreaks = input.GetBreak();
        }

        // Clamp values for control axes
        Throttle = Mathf.Clamp01(Throttle);
        SteeringPitch = Mathf.Clamp(SteeringPitch, -1, 1);
        SteeringRoll = Mathf.Clamp(SteeringRoll, -1, 1);
        SteeringYaw = Mathf.Clamp(SteeringYaw, -1, 1);

        // Change friction of 
        WheelMaterial.dynamicFriction = wheelBrakeBaseDynamicFriction * (WheelBreaks ? WheelBrakeMultiplier : 1);
        WheelMaterial.staticFriction = wheelBrakeBaseStaticFriction * (WheelBreaks ? WheelBrakeMultiplier : 1);

        // Toggle wind
        if (Input.GetKeyDown("o"))
            environment.ToggleWind();

        // Toggle Smoke
        if (Input.GetKeyDown("l"))
            SetSmoke();

        // Update fuel values from engine consumption
        foreach (AeroEngine engine in physics.engines)
            fuelLevel -= engine.FuelConsumption * Time.deltaTime * fuelConsumptionMultiplier;
    }

    public void FixedUpdate()
    {
        // Respawn at respawnpoint if corresponding button is pressed
        foreach (RespawnPoint respawnPoint in respawnPoints)
        {
            if (Input.GetKey(respawnPoint.spawnKey))
            {
                Crashed = false;
                physics.Respawn(respawnPoint);
                break;
            }
        }

        // Update control surface positions
        foreach (ControlSurface surface in controlSurfaces)
            surface.control(SteeringPitch, SteeringYaw, SteeringRoll);

        // Perform physics calculation
        physics.applyPhysics();
    }

    /// <summary>
    /// Stop simulation and show crash warning
    /// </summary>
    public void Crash()
    {
        physics.FreezeSimulation();
        Crashed = true;
    }

    /// <summary>
    /// Crash box gui function.
    /// Rendered crash box gui
    /// </summary>
    void CrashBox(int id)
    {
        GUI.Label(new Rect(5, 20, 220, 20), "You crashed! Respawn to reset.");
    }
    public void OnGUI()
    {
        if (!drawDebug)
            return;

        // Visualize debug information on left side

        GUI.Box(new Rect(0, 150, 310, 440), "");
        int y = 150;
        GUI.Label(new Rect(5, y, 300, 400), "PLANE DEBUG INFORMATION");
        GUI.Label(new Rect(5, y += 40, 300, 400), $"World Position ({transform.position.x / 1000:F2}, {transform.position.z / 1000:F2})km");
        if (environment.IsWind)
            GUI.Label(new Rect(5, y += 20, 300, 400), $"Velocity: {GetComponent<Rigidbody>().velocity.magnitude * 3.6f:F2}km/h (Wind: {environment.CalculateWind(physics.body.position).magnitude * 3.6f:F2}km/h)");
        else 
            GUI.Label(new Rect(5, y += 20, 300, 400), $"Velocity: {GetComponent<Rigidbody>().velocity.magnitude * 3.6f:F2}km/h (Wind: OFF)");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Airspeed: {physics.AirSpeed.magnitude * 3.6f:F2}km/h ({physics.GForce.magnitude:F1}g)");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Altitude: {transform.position.y:F2}m ASL");
        GUI.Label(new Rect(5, y += 20, 300, 400), $"Heading: {physics.Heading:F1}*");
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
        GUI.Label(new Rect(5, y += 20, 300, 400), WheelBreaks ? $"Wheel Brakes engaged!" : "");
        GUI.Label(new Rect(5, y += 40, 300, 400), $"Engine 1 RPM: {physics.engines[0].RPM:F0}");

        // Visualize keybinds on right side

        GUI.Box(new Rect(Screen.width - 185, 150, 185, 265 + respawnPoints.Length * 20), "");
        GUI.Label(new Rect(Screen.width - 180, y = 150, 300, 400), $"CONTROLS");
        GUI.Label(new Rect(Screen.width - 180, y += 40, 300, 400), $"Shift/Ctrl - Throttle");
        GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"W/S - Pitch");
        GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"A/D - Roll");
        GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"Q/E - Yaw");
        GUI.Label(new Rect(Screen.width - 180, y += 40, 300, 400), $"T - Toggle Autopilot");
        GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"O - Toggle wind");
        GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"L - Toggle Aerobatic smoke");
        GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"M - Toggle audio");
        GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"B - Toggle Wheel Breaks");

        y += 20;
        foreach (RespawnPoint spawn in respawnPoints)
            GUI.Label(new Rect(Screen.width - 180, y += 20, 300, 400), $"{spawn.spawnKey} - {spawn.spawnName}");

        // Show crashed box if required
        if (Crashed)
            GUI.Window(1, new Rect((Screen.width / 2) - (230 / 2), 150, 230, 50), CrashBox, "CRASH!");
    }

    /// <summary>
    /// Wether plane is currently in autopilot mode
    /// </summary>
    /// <returns></returns>
    public bool IsAutoPilot()
    {
        return input.IsAutoPilot();
    }

    /// <summary>
    /// Apply changes to AerobaticSmoke variable
    /// </summary>
    private void SetSmokeEmmiters()
    {
        foreach (TrailRenderer renderer in aerobaticEmitters)
            renderer.emitting = AerobaticSmoke;
    }

    /// <summary>
    /// Toggles Smoke
    /// </summary>
    public void SetSmoke(bool? state = null)
    {
        AerobaticSmoke = state ?? !AerobaticSmoke;
        SetSmokeEmmiters();
    }
}
