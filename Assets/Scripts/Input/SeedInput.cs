using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(InputField))]
public class SeedInput : MonoBehaviour {
	// Event fired when the seed value has changed
	public delegate void OnChangedEvent(int newSeed);
	public event OnChangedEvent OnChanged;

	// The integer representation of the entered seed
	private int _seed = 0;
	public int seed {
		get { return _seed; }
		set {
			_seed = value;
			input.value = _seed.ToString();
			OnUpdateChanged();
		}
	}
	private int oldSeed; // Used to check whether we need to fire an OnChanged event

	private InputField input;

	void Awake() {
		input = GetComponent<InputField>();
		input.onValidateInput += Validate;
		oldSeed = seed;
	}

	/// <summary>
	/// Callback for when the InputField has focus.
	/// </summary>
	public void OnUpdateChanged() {
		// If there is something in the input field and it isn't just a minus sign
		if (input.value.Length > 0 && input.value != "-") {
			// Try to parse the text for an integer value
			if (int.TryParse(input.value, out _seed)) {
				// If that succeeded, make sure the value is clamped within the seed range
				_seed = Mathf.Clamp(_seed, -50000, 50000);
			}
			else {
				// Otherwise, reset the seed
				_seed = 0;
			}

			// Update the input text
			input.value = _seed.ToString();
			if (_seed == -50000 || _seed == 50000) input.MoveTextEnd(false); // Move cursor to the end if the value was clamped

			// Fire the OnChanged event if the seed changed
			if (OnChanged != null && oldSeed != _seed) {
				oldSeed = _seed;
				OnChanged(_seed);
			}
		}
	}

	/// <summary>
	/// Callback for when a character is entered into the InputField and needs validation.
	/// </summary>
	/// <param name="text">Current text</param>
	/// <param name="charIndex">Index of the character to be entered</param>
	/// <param name="addedChar">The character trying to be entered</param>
	/// <returns></returns>
	public char Validate(string text, int charIndex, char addedChar) {
		// If the char that is trying to be added is a number or is a minus sign
		// at the front of the input, allow that character to be entered.
		if (addedChar.ToString().Length > 0 &&
			char.IsNumber(addedChar.ToString(), 0)
			|| (charIndex == 0 && addedChar == '-' && (text.Length == 0 || text[0] != '-'))) {

			return addedChar;
		}
		// Otherwise, don't allow that character.
		else
			return '\0';
	}
}
