using UnityEngine;
using System.Collections;

/// <summary>
/// Used to specify UV coordinates on a tilesheet texture.
/// </summary>
public class BlockUV {
	public Vector2 texCoord_top, texCoord_bottom, texCoord_left, texCoord_right, texCoord_front, texCoord_back;

	public BlockUV(Vector2 texCoord) : this(texCoord, texCoord, texCoord, texCoord, texCoord, texCoord) { }
	public BlockUV(Vector2 texCoord_top, Vector2 texCoord_bottom, Vector2 texCoord_left, Vector2 texCoord_right, Vector2 texCoord_front, Vector2 texCoord_back) {
		this.texCoord_top = texCoord_top;
		this.texCoord_bottom = texCoord_bottom;
		this.texCoord_left = texCoord_left;
		this.texCoord_right = texCoord_right;
		this.texCoord_front = texCoord_front;
		this.texCoord_back = texCoord_back;
	}
}
