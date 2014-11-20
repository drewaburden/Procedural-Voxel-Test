using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// This class generates the collision for a given Mesh.
/// </summary>
/// <seealso cref="Generator"/>
/// <seealso cref="Chunk"/>
/// <seealso cref="Block"/>
public class CollisionGenerator : Generator<MeshCollider> {
	private Mesh mesh; // The Mesh for which to generate the resulting collision
	private MeshCollider collision; // The resultant collider

	/// <summary>
	/// Initializes the generator with information about the Mesh for which it will be generating.
	/// This does not automatically start the generation.
	/// </summary>
	/// <param name="mesh">The Mesh for which to generate collision</param>
	/// <param name="collision">The current MeshCollider associated with the Mesh</param>
	public CollisionGenerator(Mesh mesh, MeshCollider collision) {
		this.mesh = mesh;
		this.collision = collision;
	}

	/// <summary>
	/// Coroutine that starts a separate thread for the generation, does any required post-processing, and then 
	/// fires the OnDone event.
	/// We do this in a Coroutine so that any post-processing that needs to run on the main Unity thread can
	/// do so without blocking rendering for too long.
	/// </summary>
	public override IEnumerator Generate() {
		// Start the calculation thread
		// This particular generation routine does not actually create a separate thread, because all
		// operations use the Unity API.

		// Post-processing
		// Collision
		collision.sharedMesh = mesh;

		// Fire OnDone event
		Done();
		yield break;
	}

	/// <summary>
	/// Gets the resulting MeshCollider. Will not return anything if called before the generation thread has finished.
	/// </summary>
	/// <returns>A collider generated based on the Mesh given during the generator instantiation</returns>
	public override MeshCollider GetResult() {
		lock (_lock) {
			return collision;
		}
	}
}
