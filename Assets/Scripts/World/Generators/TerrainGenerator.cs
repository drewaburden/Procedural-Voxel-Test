using UnityEngine;
using System.Collections;
using System.Threading;

/// <summary>
/// This class generates the terrain layout for a Chunk and creates the internal Block data.
/// </summary>
/// <seealso cref="Generator"/>
/// <seealso cref="Chunk"/>
/// <seealso cref="Block"/>
public class TerrainGenerator : Generator<Block[, ,]> {
	private Block[, ,] blocks; // The resultant terrain data
	private int chunkX, chunkY, chunkZ, chunkSizeX, chunkSizeY, chunkSizeZ;
	private int seaLevel = 16;

	/// <summary>
	/// Initializes the generator with information about the Chunk for which it will be generating.
	/// This does not automatically start the generation.
	/// </summary>
	/// <param name="chunkX">The Chunk on the x-axis for which to generate terrain.</param>
	/// <param name="chunkY">The Chunk on the y-axis for which to generate terrain.</param>
	/// <param name="chunkZ">The Chunk on the z-axis for which to generate terrain.</param>
	/// <param name="chunkSizeX">The size in the x-dimension of the Chunk to be generated.</param>
	/// <param name="chunkSizeY">The size in the y-dimension of the Chunk to be generated.</param>
	/// <param name="chunkSizeZ">The size in the z-dimension of the Chunk to be generated.</param>
	public TerrainGenerator(int chunkX, int chunkY, int chunkZ, int chunkSizeX, int chunkSizeY, int chunkSizeZ) {
		this.chunkX = chunkX;
		this.chunkY = chunkY;
		this.chunkZ = chunkZ;
		this.chunkSizeX = chunkSizeX;
		this.chunkSizeY = chunkSizeY;
		this.chunkSizeZ = chunkSizeZ;
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
		
		// Fire OnDone event
		Done();
		yield break;
	}

	/// <summary>
	/// Gets the resulting Block data. Will not return anything if called before the generation thread has finished.
	/// </summary>
	/// <returns>A generated 3D array of Blocks</returns>
	public override Block[, ,] GetResult() {
		lock (_lock) {
			return blocks;
		}
	}

	// The actual generation of the Blocks and terrain using Perlin noise.
	// The overall world coordinates are used for each Block in the Perlin noise method to make the Chunks all contiguous.
	protected void generation() {
		lock (_lock) {
			blocks = new Block[chunkSizeX, chunkSizeY, chunkSizeZ];

			for (int x = 0; x < blocks.GetLength(0); x++) {
				int worldX = x + (chunkX * chunkSizeX);
				for (int z = 0; z < blocks.GetLength(2); z++) {
					int worldZ = z + (chunkZ * chunkSizeZ);

					// Generate base stone/dirt layer values
					int stoneLayer = Noise.Perlin3D.Generate(worldX, 0, worldZ, 175.0f, 2.75f, 4.0f)
						+ Noise.Perlin3D.Generate(worldX, 300, worldZ, 40.0f, 4.0f, 1.2f) // Makes less of a smooth slope up the mountains. Looks more natural.
						+ seaLevel; // We want the terrain to start around sealevel
					int dirtLayer = Noise.Perlin3D.Generate(worldX, 100, worldZ, 50.0f, 2.0f, 1.0f) + 1; // +1 guarantees we have at least one layer of grass.

					for (int y = 0; y < blocks.GetLength(1); y++) {
						int worldY = y + (chunkY * chunkSizeY);

						// Make sure the very bottom layer of the world is all stone
						if (worldY == 0) {
							blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.STONE, true, true);
						}
						// Stone
						else if (worldY <= stoneLayer) {
							blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.STONE, true, true);
							// Carve out caves
							if (worldY <= seaLevel && Noise.Perlin3D.Generate(worldX, worldY * 2, worldZ, 24, 16) > 10) {
								blocks[x, y, z].type = BlockType.AIR;
								blocks[x, y, z].solid = false;
								blocks[x, y, z].visible = false;
							}
							// Scatter dirt patches
							else if (Noise.Perlin3D.Generate(worldX, worldY + worldY / 2, worldZ, 24, 11, 1.2f) > 10) {
								blocks[x, y, z].type = BlockType.DIRT;
							}
						}
						// Surface dirt
						else if (worldY <= dirtLayer + stoneLayer) {
							// Make the very top layer of the surface dirt into grass blocks
							if (worldY + 1 > dirtLayer + stoneLayer)
								blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.GRASS, true, true);
							else
								blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.DIRT, true, true);
						}
						// Air
						else {
							blocks[x, y, z] = new Block(worldX, worldY, worldZ, BlockType.AIR, false, false);
						}

					}
				}
			}
		}
	}
}
