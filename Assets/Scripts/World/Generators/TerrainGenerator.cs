using UnityEngine;
using System.Collections;

public class TerrainGenerator : Generator<Block[,,]> {
	private int chunkX, chunkY, chunkZ, chunkSizeX, chunkSizeY, chunkSizeZ;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="chunkX"></param>
	/// <param name="chunkY"></param>
	/// <param name="chunkZ"></param>
	/// <param name="chunkSizeX"></param>
	/// <param name="chunkSizeY"></param>
	/// <param name="chunkSizeZ"></param>
	/// <param name="heightOffset"></param>
	public TerrainGenerator(int chunkX, int chunkY, int chunkZ, int chunkSizeX, int chunkSizeY, int chunkSizeZ) {
		this.chunkX = chunkX;
		this.chunkY = chunkY;
		this.chunkZ = chunkZ;
		this.chunkSizeX = chunkSizeX;
		this.chunkSizeY = chunkSizeY;
		this.chunkSizeZ = chunkSizeZ;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public Block[,,] Generate() {
		return Generate(0, 0, 0, 0, 0, 0, 0);
	}
	public Block[, ,] Generate(
		int heightOffset,
		int mountain_bottomScale, int mountain_bottomMagnitude, int mountain_bottomPower,
		int mountain_topScale, int mountain_topMagnitude, int mountain_topPower) {
		Block[,,] blocks = new Block[chunkSizeX, chunkSizeY, chunkSizeZ];

		for (int x = 0; x < blocks.GetLength(0); x++) {
			int worldX = x + (chunkX * chunkSizeX);
			for (int z = 0; z < blocks.GetLength(2); z++) {
				int worldZ = z + (chunkZ * chunkSizeZ);

				// Generate base stone/dirt layer values
				int stoneLayer = Noise.Perlin3D.Generate(worldX, 0, worldZ, 175.0f, 2.75f, 4.0f)
					+ Noise.Perlin3D.Generate(worldX, 300, worldZ, 40.0f, 4.0f, 1.2f)
					+ heightOffset;
				int dirtLayer = Noise.Perlin3D.Generate(worldX, 100, worldZ, 50.0f, 2.0f, 1.0f) + 1;

				for (int y = 0; y < blocks.GetLength(1); y++) {
					int worldY = y + (chunkY * chunkSizeY);

					// Make sure the very bottom layer of the world is all stone
					if (worldY == 0) {
						blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.STONE, true);
					}
					// Stone
					else if (worldY <= stoneLayer) {
						blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.STONE, true);
						// Caves
						if (worldY <= 20 && Noise.Perlin3D.Generate(worldX, worldY * 2, worldZ, 24, 16) > 10) {
							blocks[x, y, z].type = BlockType.AIR;
							blocks[x, y, z].solid = false;
						}
						// Dirt patches
						else if (Noise.Perlin3D.Generate(worldX, worldY + worldY/2, worldZ, 24, 11, 1.2f) > 10) {
							blocks[x, y, z].type = BlockType.DIRT;
						}
					}
					// Surface dirt
					else if (worldY <= dirtLayer + stoneLayer) {
						// Make the very top layer of the surface dirt into grass blocks
						if (worldY + 1 > dirtLayer + stoneLayer)
							blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.GRASS, true);
						else
							blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.DIRT, true);
					}
					// Air
					else {						
						blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.AIR, false);
					}

				}
			}
		}

		return blocks;
	}
}
