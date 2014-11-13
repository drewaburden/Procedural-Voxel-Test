using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour {
	public bool flashlightOn = true;

	private bool buttonDown = false;

	void Start() {
		light.enabled = flashlightOn;
	}

	void Update () {
		if (Input.GetAxis("Flashlight toggle") > 0.0f) {
			buttonDown = true;
		}
		else if (buttonDown) {
			flashlightOn = !flashlightOn;
			light.enabled = flashlightOn;
			buttonDown = false;
		}
	}
}
