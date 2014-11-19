using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CollisionGenerator : Generator<MeshCollider> {
	private Mesh mesh;
	private MeshCollider collision;

	public CollisionGenerator(Mesh mesh, MeshCollider collision) {
		this.mesh = mesh;
		this.collision = collision;
	}

	public override IEnumerator Generate() {		
		// Collision
		collision.sharedMesh = mesh;
		yield return null;
		
		Done();
		yield break;
	}

	public override MeshCollider GetResult() {
		return collision;
	}
}
