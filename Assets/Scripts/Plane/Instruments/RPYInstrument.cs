using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPYInstrument : Instrument
{
    public Transform ball;
    [Range(0, 100)]
    public float speed = 10.0f;
    public Vector3 installCorrection;

    void Update()
    {
        Vector3 target = new Vector3(90, manager.physics.body.rotation.eulerAngles.y, 0) + installCorrection;
        ball.rotation = Quaternion.Slerp(ball.rotation, Quaternion.Euler(target), speed * Time.deltaTime);
    }
}
