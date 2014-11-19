using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MeshGenerator : Generator<Mesh> {
	private Block[,,] blocks;
	private Mesh mesh;
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uv;
	private int faceCount = 0; // Current number of faces in the overall mesh
	private float tileFraction = 0.5f; // The fraction of the tilesheet one tile takes up (assumes all equally sized tiles)
	private BlockUV uvMap_stone = new BlockUV(new Vector2(0, 1));
	private BlockUV uvMap_dirt = new BlockUV(new Vector2(0, 0));
	private BlockUV uvMap_grass = new BlockUV(new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));

	/// <summary>
	/// 
	/// </summary>
	/// <param name="blocks"></param>
	public MeshGenerator(Block[,,] blocks) {
		this.blocks = blocks;
	}

	/// <summary>
	/// 
	/// </summary>
	public override IEnumerator Generate() {
		Thread generationThread = new Thread(generation);
		generationThread.Start();
		yield return null;

		while (generationThread.IsAlive) yield return null;
		
		// Mesh
		mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		yield return null;
		mesh.triangles = triangles.ToArray();
		yield return null;
		mesh.uv = uv.ToArray();
		yield return null;

		// Prepare the mesh for use
		mesh.Optimize();
		yield return null;
		mesh.RecalculateNormals();
		yield return null;

		Done();
		yield break;
	}

	public override Mesh GetResult() {
		return mesh;
	}

	private void generation() {
		lock (_lock) {
			vertices = new List<Vector3>();
			triangles = new List<int>();
			uv = new List<Vector2>();
			faceCount = 0;
			buildMesh();
		}
	}
	/// <summary>
	/// 
	/// </summary>
	private void buildMesh() {
		for (int x = 0; x < blocks.GetLength(0); ++x) {
			for (int y = 0; y < blocks.GetLength(1); ++y) {
				for (int z = 0; z < blocks.GetLength(2); ++z) {
					if (blocks[x, y, z].type != BlockType.AIR) {
						BlockUV uvMap;
						if (blocks[x, y, z].type == BlockType.DIRT) uvMap = uvMap_dirt;
						else if (blocks[x, y, z].type == BlockType.GRASS) uvMap = uvMap_grass;
						else if (blocks[x, y, z].type == BlockType.STONE) uvMap = uvMap_stone;
						else uvMap = uvMap_stone;

						generateVertices(x, y, z, uvMap);
					}
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <param name="uvMap"></param>
	private void generateVertices(int x, int y, int z, BlockUV uvMap) {
		// Top
		if (!isSolidAndExists(x, y + 1, z)) {
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x, y + 1, z));
			generateQuad();
			generateUV(uvMap.texCoord_top);
		}
		// Bottom
		if (!isSolidAndExists(x, y - 1, z)) {
			vertices.Add(new Vector3(x, y, z));
			vertices.Add(new Vector3(x + 1, y, z));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			vertices.Add(new Vector3(x, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_bottom);
		}
		// Left
		if (!isSolidAndExists(x - 1, y, z)) {
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x, y + 1, z));
			vertices.Add(new Vector3(x, y, z));
			vertices.Add(new Vector3(x, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_left);
		}
		// Right
		if (!isSolidAndExists(x + 1, y, z)) {
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			vertices.Add(new Vector3(x + 1, y, z));
			generateQuad();
			generateUV(uvMap.texCoord_right);
		}
		// Front
		if (!isSolidAndExists(x, y, z - 1)) {
			vertices.Add(new Vector3(x, y + 1, z));
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x + 1, y, z));
			vertices.Add(new Vector3(x, y, z));
			generateQuad();
			generateUV(uvMap.texCoord_front);
		}
		// Back
		if (!isSolidAndExists(x, y, z + 1)) {
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x, y, z + 1));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_back);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	private void generateQuad() {
		int quadOffset = faceCount * 4;
		triangles.Add(0 + quadOffset);
		triangles.Add(1 + quadOffset);
		triangles.Add(3 + quadOffset);
		triangles.Add(1 + quadOffset);
		triangles.Add(2 + quadOffset);
		triangles.Add(3 + quadOffset);

		++faceCount;
	}

	/// <summary>
	/// 
	/// </summary>
	private void generateUV(Vector2 texCoord) {
		uv.Add(new Vector2(tileFraction * texCoord.x, tileFraction * texCoord.y + tileFraction));
		uv.Add(new Vector2(tileFraction * texCoord.x + tileFraction, tileFraction * texCoord.y + tileFraction));
		uv.Add(new Vector2(tileFraction * texCoord.x + tileFraction, tileFraction * texCoord.y));
		uv.Add(new Vector2(tileFraction * texCoord.x, tileFraction * texCoord.y));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <returns>True if a block exists at the specified coordinates and that block is solid. False otherwise.</returns>
	private bool isSolidAndExists(int x, int y, int z) {
		if (x >= 0 && y >= 0 && z >= 0
			&& x < blocks.GetLength(0) && y < blocks.GetLength(1) && z < blocks.GetLength(2)
			&& blocks[x, y, z] != null && blocks[x, y, z].solid)
			return true;
		else
			return false;
	}
}
