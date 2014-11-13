using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : Generator<Mesh> {
	private Block[,,] blocks;
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uv;
	private int faceCount;
	private float tileFraction = 0.5f;
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
	/// <param name="blocks"></param>
	/// <returns></returns>
	public Mesh Generate() {
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uv = new List<Vector2>();
		faceCount = 0;

		Mesh mesh = new Mesh();
		mesh.Clear();
		buildMesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uv.ToArray();

		return mesh;
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

						generateVertices(x, y, z,
							!isSolidAndExists(x, y + 1, z),
							!isSolidAndExists(x, y - 1, z),
							!isSolidAndExists(x - 1, y, z),
							!isSolidAndExists(x + 1, y, z),
							!isSolidAndExists(x, y, z - 1),
							!isSolidAndExists(x, y, z + 1),
							uvMap);
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
	/// <param name="top"></param>
	/// <param name="bottom"></param>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="front"></param>
	/// <param name="back"></param>
	/// <param name="type"></param>
	private void generateVertices(int x, int y, int z, bool top, bool bottom, bool left, bool right, bool front, bool back, BlockUV uvMap) {
		if (top) {
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x, y + 1, z));
			generateQuad();
			generateUV(uvMap.texCoord_top);
		}
		if (bottom) {
			vertices.Add(new Vector3(x, y, z));
			vertices.Add(new Vector3(x + 1, y, z));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			vertices.Add(new Vector3(x, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_bottom);
		}
		if (left) {
			vertices.Add(new Vector3(x, y + 1, z + 1));
			vertices.Add(new Vector3(x, y + 1, z));
			vertices.Add(new Vector3(x, y, z));
			vertices.Add(new Vector3(x, y, z + 1));
			generateQuad();
			generateUV(uvMap.texCoord_left);
		}
		if (right) {
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x + 1, y + 1, z + 1));
			vertices.Add(new Vector3(x + 1, y, z + 1));
			vertices.Add(new Vector3(x + 1, y, z));
			generateQuad();
			generateUV(uvMap.texCoord_right);
		}
		if (front) {
			vertices.Add(new Vector3(x, y + 1, z));
			vertices.Add(new Vector3(x + 1, y + 1, z));
			vertices.Add(new Vector3(x + 1, y, z));
			vertices.Add(new Vector3(x, y, z));
			generateQuad();
			generateUV(uvMap.texCoord_front);
		}
		if (back) {
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
	/// <returns></returns>
	private bool isSolidAndExists(int x, int y, int z) {
		if (x >= 0 && y >= 0 && z >= 0
			&& x < blocks.GetLength(0) && y < blocks.GetLength(1) && z < blocks.GetLength(2)
			&& blocks[x, y, z] != null && blocks[x, y, z].solid)
			return true;
		else
			return false;
	}
}
