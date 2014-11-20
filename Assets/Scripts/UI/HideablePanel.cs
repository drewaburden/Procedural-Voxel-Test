using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HideablePanel : PanelController {
	public string inputAxis = "";

	protected bool buttonDown = false; // Whether or not the button used to toggle the panel is currently being held down

	void Update() {
		// If the panel's toggle button is down
		if (Input.GetAxisRaw(inputAxis) > 0.0f) {
			buttonDown = true;
		}
		// If the toggle button is not down, but it was last frame
		else if (buttonDown) {
			Toggle();
			buttonDown = false;
		}
	}
}
