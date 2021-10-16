using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inspired by https://answers.unity.com/questions/25965/camera-orbit-on-mouse-drag.html

/// <summary>
/// Component for orbiting object on left click.
/// Returns to locked initial position
/// </summary>
public class OrbitResetCamera : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;

    private float distance;
    public bool startDistanceAutomatic = true;
    public float startDistance = 10.0f;

    public float xSpeed = 1f;
    public float ySpeed = 1f;
    public float yLowerLimit = -20f;
    public float yUpperLimit = 80f;
    public float distanceMin = .5f;
    public float distanceMax = 15f;
    public float smoothing = 2f;

    float rotationY = 0.0f;
    float rotationX = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f;

    void Start()
    {
        startDistance = this.transform.localPosition.magnitude;
        startPosition = this.transform.localPosition;
        startRotation = this.transform.localRotation;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            // Orbit around parent
            Cursor.lockState = CursorLockMode.Locked;

            // Allow rotation
            velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.01f;
            velocityY += ySpeed * Input.GetAxis("Mouse Y") * distance * 0.01f;

            rotationY += velocityX;
            rotationX = ClampAngle(rotationX - velocityY, yLowerLimit, yUpperLimit);
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

            // Allow scrolling
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + transform.parent.position;

            // Apply transformation changes
            transform.rotation = rotation;
            transform.position = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothing);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothing);
        }
        else
        {
            // Return to fixed position
            Cursor.lockState = CursorLockMode.None;

            distance = startDistance;
            this.transform.localRotation = startRotation;
            this.transform.localPosition = startPosition;

            velocityX = 0.0f;
            velocityY = 0.0f;
            rotationY = transform.eulerAngles.y;
            rotationX = transform.eulerAngles.x;
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
