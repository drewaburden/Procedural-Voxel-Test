using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour {
	public float moveSpeed = 1.0f;
	public float xRotateSpeed = 1.0f;
	public float yRotateSpeed = 1.0f;
	public float yRotateMin = -90.0f;
	public float yRotateMax = 90.0f;

	private float xRotation = 0.0f;
	private float yRotation = 0.0f;
 
	void Start() {
		Screen.lockCursor = true;
		xRotation = transform.eulerAngles.y;
		yRotation = transform.eulerAngles.x;
	}
 
	void LateUpdate() {
		xRotation += Input.GetAxis("Rotate X") * xRotateSpeed;
		yRotation -= Input.GetAxis("Rotate Y") * yRotateSpeed;
		yRotation = ClampAngle(yRotation, yRotateMin, yRotateMax);
		transform.rotation = Quaternion.Euler(yRotation, xRotation, 0.0f); // z=0 always (locked camera roll)

		// Position
		float moveX = Input.GetAxisRaw("Move X");
		float moveY = Input.GetAxisRaw("Move Y");
		float moveZ = Input.GetAxisRaw("Move Z");
		transform.Translate(moveX * moveSpeed, moveY * moveSpeed, moveZ * moveSpeed, transform);
	}

	public static float ClampAngle(float angle, float min, float max) {
		// Normalize the angle
		if (angle < -360) angle += 360;
		if (angle > 360) angle -= 360;

		// Return clamped angle
		return Mathf.Clamp(angle, min, max);
	}
}