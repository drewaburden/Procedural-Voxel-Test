using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	public bool regenerate = false;
	public int seed = 0;
	public int numChunksX = 1, numChunksY = 1, numChunksZ = 1;
	public Chunk chunkPrefab;

	private Chunk[,,] chunks;
	private GenerationProgressPanel generationProgress;

	void Awake() {
		generationProgress = FindObjectOfType<GenerationProgressPanel>();
	}

	void Start() {
		// Create the chunks
		seedRand();
		StartCoroutine("build");
	}

	void Update() {
		if (Input.GetAxisRaw("Regenerate") > 0.0f) {
			regenerate = true;
		}
		else if (regenerate) {
			StopCoroutine("build");
			destroy();
			seedRand();
			StartCoroutine("build");
			regenerate = false;
		}
	}

	private IEnumerator build() {
		generationProgress.SetPercent(0.0f);
		generationProgress.SetVisible(true);
		chunks = new Chunk[numChunksX, numChunksY, numChunksZ];
		int iteration = 0;
		int totalLength = chunks.GetLength(0) * chunks.GetLength(1) * chunks.GetLength(2);
		for (int x = 0; x < chunks.GetLength(0); ++x) {
			for (int y = 0; y < chunks.GetLength(1); ++y) {
				for (int z = 0; z < chunks.GetLength(2); ++z) {
					chunks[x, y, z] = ((Chunk)
						Instantiate(chunkPrefab,
						new Vector3(x * chunkPrefab.sizeX, y * chunkPrefab.sizeY, z * chunkPrefab.sizeZ),
						new Quaternion(0, 0, 0, 0)));
					chunks[x, y, z].transform.name = "Chunk (" + x + ", " + y + ", " + z + ")";
					chunks[x, y, z].transform.parent = transform;
					chunks[x, y, z].x = x;
					chunks[x, y, z].y = y;
					chunks[x, y, z].z = z;
					chunks[x, y, z].Generate();
					generationProgress.SetPercent((float) ++iteration / totalLength);
					yield return null;
				}
			}
		}
		generationProgress.SetVisible(false);
		yield break;
	}

	private void destroy() {
		for (int x = 0; x < chunks.GetLength(0); ++x) {
			for (int y = 0; y < chunks.GetLength(1); ++y) {
				for (int z = 0; z < chunks.GetLength(2); ++z) {
					if (chunks[x, y, z] && chunks[x, y, z].gameObject)
						Destroy(chunks[x, y, z].gameObject);
				}
			}
		}
	}

	private void seedRand() {
		// If the seed is 0, assume that means to choose a random seed
		if (seed == 0) {
			Random.seed = (int) System.DateTime.Now.Ticks;
			Noise.Perlin3D.seed = Random.Range(-50000, 50000);
		}
		else if (seed != Noise.Perlin3D.seed) Noise.Perlin3D.seed = seed;
	}
}
