using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This represents the entire game world's collection of Chunks and handles generation of new Chunks.
/// </summary>
/// <seealso cref="Chunk"/>
public class World : MonoBehaviour {
	public bool regenerate = false; // Whether or not we should regenerate on the next frame
	[Range(-50000, 50000)] // We don't want too high or low values, because the seed value is added to existing values, so it could easily overflow
	public int seed = 0; // Seed for the random terrain generation. 0 means pick a random seed.
	public int numChunksX = 1, numChunksY = 1, numChunksZ = 1; // The number of Chunks to generate on each axis of the world
	public Chunk chunkPrefab; // The prefab that represents a generic Chunk

	private Chunk[,,] chunks; // The Chunks that make up the World
	private GenerationProgressPanel generationProgress; // The panel that displays the progress of the world generation

	void Awake() {
		// Find the progress panel
		generationProgress = FindObjectOfType<GenerationProgressPanel>();
	}

	void Start() {
		// Start generating the world
		SeedRand();
		StartCoroutine("build");
	}

	void Update() {
		// If the regenerate button is down
		if (Input.GetAxisRaw("Regenerate") > 0.0f) {
			regenerate = true;
		}
		// If the regenerate button is not down, but it was last frame
		else if (regenerate) {
			Generate();
			regenerate = false;
		}
	}

	/// <summary>
	/// Begins the world generation
	/// </summary>
	public void Generate() {
		StopCoroutine("build");
		Destroy();
		SeedRand();
		StartCoroutine("build");
	}

	/// <summary>
	/// Destroy all existing chunks
	/// </summary>
	public void Destroy() {
		for (int x = 0; x < chunks.GetLength(0); ++x) {
			for (int y = 0; y < chunks.GetLength(1); ++y) {
				for (int z = 0; z < chunks.GetLength(2); ++z) {
					if (chunks[x, y, z] && chunks[x, y, z].gameObject)
						Destroy(chunks[x, y, z].gameObject);
				}
			}
		}
	}

	/// <summary>
	/// Seeds the random number generator and the perlin noise generator.
	/// </summary>
	/// <param name="seed">Seed value to use. If equal to zero, a random seed value is chosen. Values clamped between -50000 and 50000.</param>
	public void SeedRand() { SeedRand(seed); }
	public void SeedRand(int seed) {
		// If the seed is 0, assume that means to choose a random seed
		if (seed == 0) {
			Random.seed = (int) System.DateTime.Now.Ticks;
			Noise.Perlin3D.seed = Random.Range(-50000, 50000);
		}
		// Otherwise, seed with the specified value
		else if (seed != Noise.Perlin3D.seed) Noise.Perlin3D.seed = Mathf.Clamp(seed, -50000, 50000);
	}

	// Builds the world and updates the progress percentage
	private IEnumerator build() {
		// Set up the progress panel
		generationProgress.percent = 0.0f;
		generationProgress.visible = true;
		// Maintain the current iteration and the total number of iterations so we can calculate the percentage
		// of the world that has been generated.
		int iteration = 0;
		int totalLength = numChunksX * numChunksY * numChunksZ;

		// Instantiate and begin generation on all the chunks
		chunks = new Chunk[numChunksX, numChunksY, numChunksZ];
		for (int x = 0; x < chunks.GetLength(0); ++x) {
			for (int y = 0; y < chunks.GetLength(1); ++y) {
				for (int z = 0; z < chunks.GetLength(2); ++z) {
					// Instantiate the generic chunk, name it something helpful, and make it a child of the World GameObject
					chunks[x, y, z] = ((Chunk)
						Instantiate(chunkPrefab,
						new Vector3(x * chunkPrefab.sizeX, y * chunkPrefab.sizeY, z * chunkPrefab.sizeZ),
						new Quaternion(0, 0, 0, 0)));
					chunks[x, y, z].transform.name = "Chunk (" + x + ", " + y + ", " + z + ")";
					chunks[x, y, z].transform.parent = transform;
					// Tell the chunk which chunk it is in the World
					chunks[x, y, z].x = x;
					chunks[x, y, z].y = y;
					chunks[x, y, z].z = z;
					yield return null;

					// Begin generating the actual Chunk
					chunks[x, y, z].Generate();
					// Update the progress display
					generationProgress.percent = ++iteration / (float) totalLength;
					yield return null;
				}
			}
		}

		// Make sure all generated meshes are ready to be displayed, even if they aren't visible yet.
		Shader.WarmupAllShaders();
		// Hide the progress panel
		generationProgress.visible = false;
		yield break;
	}
}
