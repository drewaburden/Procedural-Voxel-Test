using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour {
	public float translationSpeed = 1.0f; // Speed at which the camera translates
	public float xRotateSpeed = 1.0f; // Speed at which the camera rotates around the x-axis
	public float yRotateSpeed = 1.0f; // Speed at which the camera rotates around the y-axis
	public float yRotateMin = -90.0f; // Minimum euler angle of the camera rotation around the y-axis
	public float yRotateMax = 90.0f; // Maximum euler angle of the camera rotation around the y-axis

	private float xRotation, yRotation; // Current rotation angles

	void Start() {
		Screen.lockCursor = true; // Keep mouse inside the window and hide it
		// Get initial rotation angles
		xRotation = transform.eulerAngles.x;
		yRotation = transform.eulerAngles.y;
	}
 
	// Update after other things that might have affected the camera
	void LateUpdate() {
		// Rotation
		xRotation += Input.GetAxis("Rotate X") * xRotateSpeed;
		yRotation -= Input.GetAxis("Rotate Y") * yRotateSpeed;
		yRotation = Util.ClampAngle(yRotation, yRotateMin, yRotateMax);
		transform.rotation = Quaternion.Euler(yRotation, xRotation, 0.0f); // z=0 always (locked camera roll)

		// Position
		float moveX = Input.GetAxisRaw("Move X");
		float moveY = Input.GetAxisRaw("Move Y");
		float moveZ = Input.GetAxisRaw("Move Z");
		transform.Translate(moveX * translationSpeed, moveY * translationSpeed, moveZ * translationSpeed, transform);
	}
}