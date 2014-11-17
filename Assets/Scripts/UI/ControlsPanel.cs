using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlsPanel : MonoBehaviour {
	public RectTransform panel;

	private bool buttonDown = false;

	void Update() {
		if (Input.GetAxisRaw("Controls display") > 0.0f) {
			buttonDown = true;
		}
		else if (buttonDown) {
			buttonDown = false;
			SetVisible(!panel.gameObject.activeSelf);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="visible"></param>
	public void SetVisible(bool visible) {
		panel.gameObject.SetActive(visible);
	}
}
