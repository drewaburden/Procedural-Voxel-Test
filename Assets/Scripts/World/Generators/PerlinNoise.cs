using UnityEngine;
using System.Collections;

namespace Noise {
	public static class Perlin3D {
		[Range(-50000, 50000)] // We don't want too high or low values, because the seed value is added to existing values, so it could easily overflow
		public static int seed = 0; // Added to all values to offset them. Making the seed random effectively randomizes the Perlin generator.

		/// <summary>
		/// Wrapper for a third-party, public domain implementation of the Perlin noise algorithm.
		/// This wrapper method allows us to warp the input values in different ways to give us different degrees, frequency,
		/// and patterns of noise.
		/// </summary>
		/// <param name="x">X coordinate for which to return a Perlin noise value</param>
		/// <param name="y">Y coordinate for which to return a Perlin noise value</param>
		/// <param name="z">Z coordinate for which to return a Perlin noise value</param>
		/// <param name="smoothness">
		/// Higher or lower smoothness means that there is more or less of a difference in generated values between adjacent coordinates,
		/// respectively.
		/// </param>
		/// <param name="scale">
		/// Higher or lower scale values means that the given coordinates will be seen as on a higher or lower grid size, respectively.
		/// This will scale the overall generated data up on all axes.
		/// </param>
		/// <param name="power">
		/// Higher or lower power values means more or less noise, respectively. 1.0f is the default power. Smoothness dampens this
		/// to some degree.
		/// </param>
		/// <returns></returns>
		public static int Generate(int x, int y, int z, float smoothness, float scale, float power = 1.0f) {
			return Mathf.RoundToInt(
				Mathf.Pow(
					Noise.GetNoise(
						seed + x / ((double) smoothness),
						seed + y / ((double) smoothness),
						seed + z / ((double) smoothness))
					* scale,
				power));
		}
	}
}