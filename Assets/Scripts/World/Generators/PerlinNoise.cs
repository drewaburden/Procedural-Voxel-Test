using UnityEngine;
using System.Collections;

namespace Noise {
	public static class Perlin3D {
		[Range(-50000, 50000)] // We don't want too high or low values, because the seed value is added to existing values, so it could easily overflow
		public static int seed = 0;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="scale"></param>
		/// <param name="magnitude"></param>
		/// <param name="power"></param>
		/// <returns></returns>
		public static int Generate(int x, int y, int z, float scale, float magnitude, float power = 1.0f) {
			return Mathf.RoundToInt(
				Mathf.Pow(
					Noise.GetNoise(
						seed + x / ((double) scale),
						seed + y / ((double) scale),
						seed + z / ((double) scale))
					* magnitude,
				power));
		}
	}
}