using UnityEngine;
using System.Collections;

public class PanelController : MonoBehaviour {
	public RectTransform panel; // The parent UI element for the panel. This object is what is enabled/disabled.

	// Whether or not the panel is currently visible
	private bool _visible = false;
	public bool visible {
		get { return _visible; }
		set {
			_visible = value;
			panel.gameObject.SetActive(_visible);
		}
	}

	void Start() {
		// Get the initial visibility as set in the inspector
		visible = panel.gameObject.activeSelf;
	}

	/// <summary>
	/// Toggles the visibility of the UI element this controller is handling.
	/// </summary>
	public void Toggle() {
		visible = !visible;
	}
}
