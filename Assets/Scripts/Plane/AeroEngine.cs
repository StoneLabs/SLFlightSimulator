using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// AeroEngine. Works in tandem with AeroPropeller to simulate engine/propeller system.
/// </summary>
public class AeroEngine : MonoBehaviour
{
    public PlaneManager plane;

    [Header("Performance")]
    [Tooltip("Engine Torque per RPM")]
    public AnimationCurve enginePower;
    [Tooltip("Engine:Propeller Rotation Ratio in X:1")]
    public float gearRatio = 3.0f;

    [Tooltip("Engine Power multiplicator per air pressure")]
    public AnimationCurve enginePowerDensityFactor;

    [Range(0.0f, 1.0f)]
    public float minThrottle = 0.2f;

    [Range(0.0f, 5000f)]
    public float minRPMDead = 250;
    [Range(0.0f, 5000f)]
    public float maxRPMDead = 3300;
    private bool engineDead = false;

    public float RPM { get; private set; }


    [Header("Fuel")]
    [Tooltip("Fuel Consumption in Liter/Minute per (100 EnginePower)")]
    public float fuelConsumption = 2;
    [Range(0, 1)]
    public float fuelStarvePercentage = 0.02f;

    [Header("Propeller")]
    public AeroPropeller propeller;

    [Header("Sound")]
    public AudioSource soundSource;
    [Tooltip("per RPM")]
    public AnimationCurve soundPitch;
    [Tooltip("per RPM")]
    public AnimationCurve soundVolumeMultiplier;
    private float soundVolumeBase;

    /// <summary>
    /// Wether engine has starved
    /// </summary>
    public bool Starved
    {
        get
        {
            return plane.FuelPercentage < fuelStarvePercentage;
        }
    }

    /// <summary>
    /// Current fuel consumption per deltaT (MUST be multiplied with deltaT)
    /// </summary>
    public float FuelConsumption
    {
        get
        {
            if (Starved || engineDead)
                return 0.0f;

            return fuelConsumption * (EnginePower / 100.0f) / 60.0f;
        }
    }

    /// <summary>
    /// Current power output of engine
    /// </summary>
    public float EnginePower
    {
        get
        {
            if (Starved || engineDead)
                return 0.0f;

            return
                enginePower.Evaluate(RPM) * 
                (plane.Throttle * (1.0f - minThrottle) + minThrottle) *
                enginePowerDensityFactor.Evaluate(plane.environment.CalculateDensity(ThrustLocation.position.y));
        }
    }

    /// <summary>
    /// Thrust of system
    /// </summary>
    public Vector3 Thrust
    {
        get
        {
            return propeller.Thrust;
        }
    }

    /// <summary>
    /// Location of thrust to be applied to the system
    /// </summary>
    public Transform ThrustLocation
    {
        get
        {
            return propeller.transform;
        }
    }

    private void Start()
    {
        // Set init RPM and sound volume
        RPM = 1000;
        soundVolumeBase = soundSource.volume;
    }

    public void Respawn()
    {
        // Reset engine and rpm
        engineDead = false;
        if (RPM < minRPMDead)
            RPM = 1000;
    }

    void Update()
    {
        // Change sound volume/pitch
        soundSource.pitch = soundPitch.Evaluate(RPM);
        soundSource.volume = soundVolumeBase * soundVolumeMultiplier.Evaluate(RPM);

        // Simulate RPM changes
        float RPSAcceleration = (EnginePower - (propeller.CounterTorque / gearRatio)) / propeller.AngularDrag;
        this.RPM += RPSAcceleration * Time.deltaTime * 60.0f;

        // Simulate engine death
        if (RPM < minRPMDead || RPM > maxRPMDead)
            engineDead = true;
    }

    /// <summary>
    /// Renders engine death/starved box
    /// </summary>
    void EngineBox(int id) 
    { 
        GUI.Label(new Rect(5, 20, 220, 20), id == 0 ? "Engine died due to over/under RPM!" : "Out of fuel!"); 
    }
    private void OnGUI()
    {
        // Show state boxes
        if (Starved)
            GUI.Window(1, new Rect((Screen.width / 2) - (230 / 2), 50, 230, 50), EngineBox, "ENGINE DIED!");
        if (engineDead)
            GUI.Window(0, new Rect((Screen.width / 2) - (230 / 2), 50, 230, 50), EngineBox, "ENGINE DIED!");
    }
}
