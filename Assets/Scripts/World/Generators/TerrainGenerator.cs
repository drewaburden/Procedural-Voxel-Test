using UnityEngine;
using System.Collections;

public class TerrainGenerator : Generator<Block[,,]> {
	private int chunkX, chunkY, chunkZ, chunkSizeX, chunkSizeY, chunkSizeZ, heightOffset;

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
	public TerrainGenerator(int chunkX, int chunkY, int chunkZ, int chunkSizeX, int chunkSizeY, int chunkSizeZ, int heightOffset = 0) {
		this.chunkX = chunkX;
		this.chunkY = chunkY;
		this.chunkZ = chunkZ;
		this.chunkSizeX = chunkSizeX;
		this.chunkSizeY = chunkSizeY;
		this.chunkSizeZ = chunkSizeZ;
		this.heightOffset = heightOffset;
	}

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
	/// <returns></returns>
	public Block[,,] Generate() {
		Block[,,] blocks = new Block[chunkSizeX, chunkSizeY, chunkSizeZ];

		for (int x = 0; x < blocks.GetLength(0); x++) {
			int worldX = x + (chunkX * chunkSizeX);
			for (int z = 0; z < blocks.GetLength(2); z++) {
				int worldZ = z + (chunkZ * chunkSizeZ);
				int stoneLayer = Noise.Perlin3D.Generate(worldX, 0, worldZ, 100, 3, 4.5f);
				stoneLayer += Noise.Perlin3D.Generate(worldX, 300, worldZ, 20, 4) + 10;
				int dirtLayer = Noise.Perlin3D.Generate(worldX, 100, worldZ, 50, 2);

				for (int y = 0; y < blocks.GetLength(1); y++) {
					int worldY = y + (chunkY * chunkSizeY);
					if (y <= stoneLayer) {
						blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.STONE, true);

						// Caves
						if (Noise.Perlin3D.Generate(worldX, worldY * 2, worldZ, 24, 16) > 10) {
							blocks[x, y, z].type = BlockType.AIR;
							blocks[x, y, z].solid = false;
						}
						// Dirt patches
						else if (Noise.Perlin3D.Generate(worldX, worldY, worldZ, 12, 16) > 10) {
							blocks[x, y, z].type = BlockType.DIRT;
						}
					}
					else if (y <= dirtLayer + stoneLayer) {
						blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.DIRT, true);
					}
					else {
						blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.AIR, false);
						// Make original surface dirt (i.e., not patches of dirt within stone) into grass blocks
						if (y != 0 && blocks[x, y - 1, z].type == BlockType.DIRT) blocks[x, y - 1, z].type = BlockType.GRASS;
					}

				}
			}
		}

		return blocks;
	}
}
