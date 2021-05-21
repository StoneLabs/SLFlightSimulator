using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour
{
    public PlanePhysics physics;
    public BasicPlaneInput input;
    public ControlSurface[] controlSurfaces;

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
    }

    public void FixedUpdate()
    {
        foreach (ControlSurface surface in controlSurfaces)
            surface.control(steeringPitch, steeringYaw, steeringRoll);

        physics.applyForces(throttle);
    }

    public void OnGUI()
    {
        if (!drawDebug)
            return;

        GUI.Label(new Rect(0, 150, 400, 400), $"Velocity: {GetComponent<Rigidbody>().velocity.magnitude * 3.6}km/h");
        GUI.HorizontalSlider(new Rect(0, 180, 200, 40), throttle, 0, 1);
        GUI.HorizontalSlider(new Rect(0, 200, 200, 40), steeringPitch, -1, 1);
        GUI.HorizontalSlider(new Rect(0, 220, 200, 40), steeringRoll, -1, 1);
        GUI.HorizontalSlider(new Rect(0, 240, 200, 40), steeringYaw, -1, 1);
    }
}
