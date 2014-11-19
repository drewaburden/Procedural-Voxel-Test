using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// This represents a variable-sized collection of Blocks, referred to as a Chunk.
/// </summary>
/// <seealso cref="Block"/>
[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class Chunk : MonoBehaviour {
	[Range(1, 32)] // Values above 32 sometimes result in a Mesh with more vertices than Unity supports
	public int sizeX = 16, sizeY = 16, sizeZ = 16; // The number of blocks in each dimension of this chunk
	
	[System.NonSerialized]
	public int x = 0, y = 0, z = 0; // The coordinates of this chunk, not in worldspace. The number of blocks per chunk does not factor into this.
	[System.NonSerialized]
	public Block[,,] blocks; // The Blocks that make up this chunk

	private MeshFilter filter;
	private MeshCollider collision;
	private TerrainGenerator terrainGen;
	private MeshGenerator meshGen;
	private CollisionGenerator collisionGen;

	void Awake() {
		collision = GetComponent<MeshCollider>();
		filter = GetComponent<MeshFilter>();
	}

	/// <summary>
	/// Begin the generation process for this Chunk. If the chunk has already been generated, the previous data will be replaced
	/// with the newly generated data.
	/// </summary>
	public void Generate() {
		// Generate the terrain data in a coroutine
		terrainGen = new TerrainGenerator(x, y, z, sizeX, sizeY, sizeZ);
		terrainGen.OnDone += OnTerrainGenDone;
		StartCoroutine(terrainGen.Generate());
	}

	/// <summary>
	/// When the terrain generation coroutine finishes, this method is called by an event so we can continue with the next
	/// part of the generation process.
	/// </summary>
	public void OnTerrainGenDone() {
		terrainGen.OnDone -= OnTerrainGenDone; // Remove this callback in case Generate() is called again later
		blocks = terrainGen.GetResult();
		
		// Generate the mesh vertices, triangles, uv, normals, etc. in a coroutine
		meshGen = new MeshGenerator(blocks);
		meshGen.OnDone += OnMeshGenDone;
		StartCoroutine(meshGen.Generate());
	}
	/// <summary>
	/// When the mesh generation coroutine finishes, this method is called by an event so we can continue with the next
	/// part of the generation process.
	/// </summary>
	public void OnMeshGenDone() {
		meshGen.OnDone -= OnMeshGenDone; // Remove this callback in case Generate() is called again later
		filter.mesh = meshGen.GetResult();
		
		// Generate the collision vertices and triangles in a coroutine
		collisionGen = new CollisionGenerator(meshGen.GetResult(), collision);
		collisionGen.OnDone += OnCollisionGenDone;
		StartCoroutine(collisionGen.Generate());
	}
	/// <summary>
	/// When the collision generation coroutine finishes, this method is called by an event so we can finalize the generation.
	/// </summary>
	public void OnCollisionGenDone() {
		collisionGen.OnDone -= OnCollisionGenDone; // Remove this callback in case Generate() is called again later
		collision = collisionGen.GetResult();
	}

	/// <summary>
	/// Gives the block at the specified local chunk coordinate, if it exists; otherwise, it returns null.
	/// </summary>
	/// <param name="x">X position with respect to this chunk</param>
	/// <param name="y">Y position with respect to this chunk</param>
	/// <param name="z">Z position with respect to this chunk</param>
	/// <returns>Block at the specified coordinate, if it exists; otherwise, returns null.</returns>
	public Block GetBlock(int x, int y, int z) {
		if (x >= 0 && y >= 0 && z >= 0 && x < blocks.GetLength(0) && y < blocks.GetLength(1) && z < blocks.GetLength(2))
			return blocks[x, y, z];
		else
			return null;
	}
}
