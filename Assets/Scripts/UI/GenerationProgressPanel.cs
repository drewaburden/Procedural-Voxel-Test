using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenerationProgressPanel : MonoBehaviour {
	public RectTransform panel;
	public Text percentText;
	public Scrollbar progressBar;
	public string percentSign = "%";

	private float percentage = 0.0f;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="visible"></param>
	public void SetVisible(bool visible) {
		panel.gameObject.SetActive(visible);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="newPercent"></param>
	public void SetPercent(float newPercent) {
		percentage = Mathf.Clamp01(newPercent);
		percentText.text = Mathf.Clamp(Mathf.RoundToInt(percentage * 100.0f), 0, 99) + percentSign;
		progressBar.size = percentage;
	}
}
