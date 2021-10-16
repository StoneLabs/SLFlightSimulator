using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RPY Instrument. Visualizes rotation on ball element.
/// </summary>
public class RPYInstrument : Instrument
{
    public Transform ball;
    [Range(0, 100)]
    public float speed = 10.0f; // Speed of ball rotation
    public Vector3 installCorrection;

    void Update()
    {
        // Calculate target rotation based on plane rotation
        Vector3 target = new Vector3(90, manager.physics.body.rotation.eulerAngles.y, 0) + installCorrection;

        // Perform slow SLERP to simulate moment of inertia of ball
        ball.rotation = Quaternion.Slerp(ball.rotation, Quaternion.Euler(target), speed * Time.deltaTime);
    }
}
