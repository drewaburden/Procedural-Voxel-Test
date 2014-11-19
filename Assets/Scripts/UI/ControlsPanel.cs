using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlsPanel : PanelController {
	protected bool buttonDown = false; // Whether or not the button used to toggle the panel is currently being held down

	void Update() {
		// If the controls display panel's toggle button is down
		if (Input.GetAxisRaw("Controls display") > 0.0f) {
			buttonDown = true;
		}
		// If the toggle button is not down, but it was last frame
		else if (buttonDown) {
			Toggle();
			buttonDown = false;
		}
	}
}
