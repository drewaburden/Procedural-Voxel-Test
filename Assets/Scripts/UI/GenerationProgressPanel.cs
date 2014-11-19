using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenerationProgressPanel : PanelController {
	public Text percentText; // The percentage text display within the progress bar
	public Scrollbar progressBar; // The scrollbar that is used to represent the progress bar

	// The currently displayed percentage. (0.0f = 0%, 1.0f = 100%)
	[Range(0.0f, 1.0f)]
	private float _percent = 0.0f;
	public float percent {
		get { return _percent; }
		set {
			_percent = value;
			// Display the percentage as a truncated integer between 0 and 99. You should never see 100%
			// because that means it's done.
			percentText.text = Mathf.Clamp((int) Mathf.Floor(_percent * 100.0f), 0, 99) + "%";
			// Set the size of the progress bar to the percentage value (since the progress bar size ranges
			// from 0.0f to 1.0f).
			progressBar.size = _percent;
		}
	}
}
