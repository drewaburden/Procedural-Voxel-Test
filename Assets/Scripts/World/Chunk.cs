using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class Chunk : MonoBehaviour {
	public int sizeX = 16, sizeY = 16, sizeZ = 16;
	[System.NonSerialized]
	public int x = 0, y = 0, z = 0;
	[System.NonSerialized]
	public Block[,,] blocks;

	private Mesh mesh;
	private MeshCollider collision;

	void Start() {
		collision = GetComponent<MeshCollider>();

		Generate();
	}

	public void Generate() {
		Mesh mesh = new Mesh();
		mesh.Clear();
		blocks = new TerrainGenerator(x, y, z, sizeX, sizeY, sizeZ).Generate();
		mesh = new MeshGenerator(blocks).Generate();
		// Collision
		collision.sharedMesh = null; // Clear current collision
		collision.sharedMesh = mesh; // Reset to newly generated mesh
		// Prepare the mesh for use
		mesh.Optimize();
		mesh.RecalculateNormals();

		GetComponent<MeshFilter>().mesh = mesh;
	}

	public Block GetBlock(int x, int y, int z) {
		if (x >= 0 && y >= 0 && z >= 0 && x < blocks.GetLength(0) && y < blocks.GetLength(1) && z < blocks.GetLength(2))
			return blocks[x, y, z];
		else
			return null;
	}
}
