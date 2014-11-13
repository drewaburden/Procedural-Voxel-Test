using UnityEngine;
using System.Collections;

public class Block {
	public int x;
	public int y;
	public int z;
	public BlockType type;
	public bool solid;

	public Block(int x = 0, int y = 0, int z = 0, BlockType type = BlockType.AIR, bool solid = false) {
		this.x = x;
		this.y = y;
		this.z = z;
		this.type = type;
		this.solid = solid;
	}
}
