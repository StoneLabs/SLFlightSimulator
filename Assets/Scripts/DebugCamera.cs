using System;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DebugCamera : MonoBehaviour
{
	[Tooltip("Camera movement acceleration")]
	public float acceleration = 50;
	[Tooltip("Camera movement multiplier for sprint")]
	public float sprintFactor = 4;
	[Tooltip("Camera movement damping")]
	public float damping = 5;
	[Tooltip("Camera rotation sensitivity")]
	public float rotationSensivity = 1;
	[Tooltip("Wether to focus on the component being enabled")]
	public bool focusOnEnable = true;

	private Vector3 velocity;

	bool Focused
	{
		get
		{
			return Cursor.lockState == CursorLockMode.Locked;
		}
		set
		{
			Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = value == false;
		}
	}

	void OnEnable()
	{
		if (focusOnEnable) 
			Focused = true;
	}

	void OnDisable()
	{
		Focused = false;
	}

	void Update()
	{
		if (Focused)
			UpdateCamera();

		if (!Focused && Input.GetMouseButtonDown(0))
			Focused = true;

		if (Focused && Input.GetKeyDown(KeyCode.Escape))
			Focused = false;

		// Move camera
		velocity = Vector3.Lerp(velocity, Vector3.zero, damping * Time.deltaTime);
		transform.position += velocity * Time.deltaTime;
	}

	void UpdateCamera()
	{
		// Update camera movement
		Vector3 input = Vector3.zero;

		void map(KeyCode key, Vector3 action) { if (Input.GetKey(key)) input += action; }

		map(KeyCode.W, Vector3.forward);
		map(KeyCode.S, Vector3.back);
		map(KeyCode.D, Vector3.right);
		map(KeyCode.A, Vector3.left);
		map(KeyCode.Space, Vector3.up);
		map(KeyCode.LeftControl, Vector3.down);

		// Transform input vector
		input = transform.TransformVector(input);
		input *= acceleration;

		// Faster movement with Shift
		if (Input.GetKey(KeyCode.LeftShift))
			input *= sprintFactor;

		velocity += input * Time.deltaTime;

		// Update camera rotation
		Vector2 mouseDelta = rotationSensivity * new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
		transform.Rotate(Vector3.up, mouseDelta.x, Space.World);
		transform.Rotate(Vector3.right, mouseDelta.y, Space.Self);
	}
}