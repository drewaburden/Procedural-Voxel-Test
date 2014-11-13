using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Material))]
public class BlockTextures : MonoBehaviour {
	public int tileSize = 32;
	public List<Vector2> textureCoordinates = new List<Vector2>();
	public List<BlockType> correspondingBlockType = new List<BlockType>();
	//public Vector2 texStone = new Vector2(0, 1);
	//public Vector2 texDirt = new Vector2(0, 0);

	void Start() {
		//renderer.material.mainTexture.width;
	}
}
