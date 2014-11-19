using UnityEngine;

public static class Util {
	/// <summary>
	/// Constrains a specified euler angle value to within specified minimum and maximum euler angle values.
	/// </summary>
	/// <param name="angle">Input euler angle</param>
	/// <param name="min">Minimum valid euler angle for the resultant angle</param>
	/// <param name="max">Maximum valid euler angle for the resultant angle</param>
	/// <returns>
	/// Specified <paramref name="angle"/> constrained between the specified <paramref name="min"/> and <paramref name="max"/> values
	/// </returns>
	public static float ClampAngle(float angle, float min, float max) {
		// Normalize the angle
		if (angle < -360) angle += 360;
		if (angle > 360) angle -= 360;

		// Return clamped angle
		return Mathf.Clamp(angle, min, max);
	}
}
