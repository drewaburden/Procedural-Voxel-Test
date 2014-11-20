using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// This class generates the actual Mesh based on the given terrain data.
/// </summary>
/// <seealso cref="Generator"/>
/// <seealso cref="Chunk"/>
/// <seealso cref="Block"/>
public class MeshGenerator : Generator<Mesh> {
	private Block[,,] blocks; // The given terrain data for which to generate the resulting Mesh
	private Mesh mesh; // The resultant Mesh
	private List<Vector3> vertices; // Mesh vertices
	private List<int> triangles; // Mesh triangles
	private List<Vector2> uv; // UV coordinates for the Mesh
	private int faceCount = 0; // Current number of faces in the overall mesh
	private float tileFraction = 0.5f; // The fraction of the tilesheet one tile takes up (assumes all equally sized tiles)
	// UV coordinates on the tilesheet for each type of visible block. This probably needs to be modularized somehow.
	// Ideally, we want to be able to set textures or texture coordinates for the different Block types within the inspector.
	private BlockUV uvMap_stone = new BlockUV(new Vector2(0, 1));
	private BlockUV uvMap_dirt = new BlockUV(new Vector2(0, 0));
	private BlockUV uvMap_grass = new BlockUV(new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));

	/// <summary>
	/// Initializes the generator with information about the Mesh for which it will be generating.
	/// This does not automatically start the generation.
	/// </summary>
	/// <param name="blocks">The terrain data for which to generate a Mesh</param>
	public MeshGenerator(Block[,,] blocks) {
		this.blocks = blocks;
	}

	/// <summary>
	/// Coroutine that starts a separate thread for the generation, does any required post-processing, and then 
	/// fires the OnDone event.
	/// We do this in a Coroutine so that any post-processing that needs to run on the main Unity thread can
	/// do so without blocking rendering for too long.
	/// </summary>
	public override IEnumerator Generate() {
		// Start the calculation thread
		Thread generationThread = new Thread(generation);
		generationThread.Start();
		yield return null;

		// Wait for the thread to finish
		while (generationThread.IsAlive) yield return null;

		// Post-processing
		// Create actual Mesh from the data we calculated
		mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		yield return null;
		mesh.triangles = triangles.ToArray();
		yield return null;
		mesh.uv = uv.ToArray();
		yield return null;
		// Prepare the Mesh for use
		mesh.Optimize();
		yield return null;
		mesh.RecalculateNormals(); // Recalculates bounds too
		yield return null;

		// Fire OnDone event
		Done();
		yield break;
	}

	/// <summary>
	/// Gets the resulting Mesh. Will not return anything if called before the generation thread has finished.
	/// </summary>
	/// <returns>A Mesh generated based on the Blocks given during the generator instantiation</returns>
	public override Mesh GetResult() {
		lock (_lock) {
			return mesh;
		}
	}

	// Loop through the Block data and generate the vertices, tris, and uv map, where applicable
	protected void generation() {
		lock (_lock) {
			vertices = new List<Vector3>();
			triangles = new List<int>();
			uv = new List<Vector2>();
			faceCount = 0;

			// Build mesh
			for (int x = 0; x < blocks.GetLength(0); ++x) {
				for (int y = 0; y < blocks.GetLength(1); ++y) {
					for (int z = 0; z < blocks.GetLength(2); ++z) {
						// If the Block is visible, we need to generate Mesh data for this Block
						if (blocks[x, y, z].visible) {
							// Which UV map should we use for this Block?
							BlockUV uvMap;
							if (blocks[x, y, z].type == BlockType.DIRT) uvMap = uvMap_dirt;
							else if (blocks[x, y, z].type == BlockType.GRASS) uvMap = uvMap_grass;
							else if (blocks[x, y, z].type == BlockType.STONE) uvMap = uvMap_stone;
							else uvMap = uvMap_stone;
							// Generate the vertices and triangles for this Block
							generateMeshData(x, y, z, uvMap);
						}
					}
				}
			}
		}
	}

	// Generate vertices, tris, and uv map for the specified block location in the Chunk we're working on.
	// Only the faces of blocks which are adjacent to invisible blocks or on the edge of the Chunk will have that face generated.
	// This majorly cuts down on the Mesh's complexity (and therefore, the collision complexity as well).
	protected void generateMeshData(int x, int y, int z, BlockUV uvMap) {
		// Top face
		if (!isVisibleAndExists(x, y + 1, z)) {
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x, y + 1, z));
			generateQuad();
			generateUV(uvMap.texCoord_top);
		}
		// Bottom face
		if (!isVisibleAndExists(x, y - 1, z)) {
			vertices.Add(new Vector3(x, y, z));
			vertices.Add(new Vector3(x + 1, y, z));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			vertices.Add(new Vector3(x, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_bottom);
		}
		// Left face
		if (!isVisibleAndExists(x - 1, y, z)) {
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x, y + 1, z));
			vertices.Add(new Vector3(x, y, z));
			vertices.Add(new Vector3(x, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_left);
		}
		// Right face
		if (!isVisibleAndExists(x + 1, y, z)) {
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			vertices.Add(new Vector3(x + 1, y, z));
			generateQuad();
			generateUV(uvMap.texCoord_right);
		}
		// Front face
		if (!isVisibleAndExists(x, y, z - 1)) {
			vertices.Add(new Vector3(x, y + 1, z));
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x + 1, y, z));
			vertices.Add(new Vector3(x, y, z));
			generateQuad();
			generateUV(uvMap.texCoord_front);
		}
		// Back face
		if (!isVisibleAndExists(x, y, z + 1)) {
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x, y, z + 1));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_back);
		}
	}

	// Generate the triangles to represent a new face for a Block, and a new quad on the Mesh
	protected void generateQuad() {
		int quadOffset = faceCount * 4;
		triangles.Add(0 + quadOffset);
		triangles.Add(1 + quadOffset);
		triangles.Add(3 + quadOffset);
		triangles.Add(1 + quadOffset);
		triangles.Add(2 + quadOffset);
		triangles.Add(3 + quadOffset);

		++faceCount;
	}

	// Denote UV coordinates for the face we're currently working on
	protected void generateUV(Vector2 texCoord) {
		uv.Add(new Vector2(tileFraction * texCoord.x, tileFraction * texCoord.y + tileFraction));
		uv.Add(new Vector2(tileFraction * texCoord.x + tileFraction, tileFraction * texCoord.y + tileFraction));
		uv.Add(new Vector2(tileFraction * texCoord.x + tileFraction, tileFraction * texCoord.y));
		uv.Add(new Vector2(tileFraction * texCoord.x, tileFraction * texCoord.y));
	}

	// True if a block exists at the specified coordinates and that block is visible. False otherwise.
	protected bool isVisibleAndExists(int x, int y, int z) {
		if (x >= 0 && y >= 0 && z >= 0
			&& x < blocks.GetLength(0) && y < blocks.GetLength(1) && z < blocks.GetLength(2)
			&& blocks[x, y, z] != null && blocks[x, y, z].visible)
			return true;
		else
			return false;
	}
}
