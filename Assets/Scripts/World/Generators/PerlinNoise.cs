using UnityEngine;
using System.Collections;

namespace Noise {
	public static class Perlin3D {
		public static int seed = 0;

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