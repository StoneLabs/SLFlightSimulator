using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AeroEngine : MonoBehaviour
{
    public PlaneManager plane;

    [Header("Performance")]
    [Tooltip("Engine Torque per RPM")]
    public AnimationCurve enginePower;
    [Tooltip("Engine:Propeller Ratio in X:1")]
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
    [Tooltip("Fuel Consumption in Liter/Minute")]
    public AnimationCurve fuelConsumptionCurve;
    [Range(0, 1)]
    public float fuelStarvePercentage = 0.02f;

    [Header("Propeller")]
    public AeroPropeller propeller;

    [Header("Sound")]
    public AudioSource soundSource;
    public AnimationCurve soundPitch;

    public bool Starved
    {
        get
        {
            return plane.FuelPercentage < fuelStarvePercentage;
        }
    }
    public float FuelConsumption
    {
        get
        {
            if (Starved || engineDead)
                return 0.0f;

            return fuelConsumptionCurve.Evaluate(plane.Throttle) / 60.0f;
        }
    }
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

    public Vector3 Thrust
    {
        get
        {
            return propeller.Thrust;
        }
    }
    public Transform ThrustLocation
    {
        get
        {
            return propeller.transform;
        }
    }

    private void Start()
    {
        RPM = 1000;
    }

    void Update()
    {
        soundSource.pitch = soundPitch.Evaluate(plane.Throttle);
        if (Starved || engineDead)
            soundSource.mute = true;

        float RPMAcceleration = (EnginePower - (propeller.CounterTorque / gearRatio)) / propeller.AngularDrag;
        this.RPM += RPMAcceleration * Time.deltaTime;

        if (RPM < minRPMDead || RPM > maxRPMDead)
            engineDead = true;
    }

    void EngineBox(int id) 
    { 
        GUI.Label(new Rect(5, 20, 220, 20), id == 0 ? "Engine died due to over/under RPM!" : "Out of fuel!"); 
    }
    private void OnGUI()
    {
        if (Starved)
            GUI.Window(1, new Rect((Screen.width / 2) - (230 / 2), 50, 230, 50), EngineBox, "ENGINE DIED!");
        if (engineDead)
            GUI.Window(0, new Rect((Screen.width / 2) - (230 / 2), 50, 230, 50), EngineBox, "ENGINE DIED!");
    }
}
