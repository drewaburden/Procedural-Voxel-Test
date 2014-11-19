using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour {
	public bool flashlightOn = true; // Whether or not the flashlight is currently on

	private bool buttonDown = false; // Whether or not the flashlight toggle button is currently being held down

	void Start() {
		light.enabled = flashlightOn;
	}

	void Update () {
		// If the flashlight toggle button is down
		if (Input.GetAxis("Flashlight toggle") > 0.0f) {
			buttonDown = true;
		}
		// If the toggle button is not down, but it was last frame
		else if (buttonDown) {
			Toggle();
			buttonDown = false;
		}
	}

	/// <summary>
	/// Toggles the internal flashlight state and the associated light
	/// </summary>
	public void Toggle() {
		flashlightOn = !flashlightOn;
		light.enabled = flashlightOn;
	}
}
