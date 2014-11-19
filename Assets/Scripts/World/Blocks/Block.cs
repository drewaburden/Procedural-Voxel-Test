using UnityEngine;
using System.Collections;

/// <summary>
/// An instance of this class represents a single block in the world and all of its properties.
/// </summary>
public class Block {
	public int x, y, z; // Coordinates of the block in the entire worldspace
	public BlockType type; // Type of the block
	public bool solid; // Whether or not the block needs collision

	public Block(int x = 0, int y = 0, int z = 0, BlockType type = BlockType.AIR, bool solid = false) {
		this.x = x;
		this.y = y;
		this.z = z;
		this.type = type;
		this.solid = solid;
	}
}
