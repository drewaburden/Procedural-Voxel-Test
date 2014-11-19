using UnityEngine;
using System.Collections;

/// <summary>
/// An abstract representation of a class that generates a generic type of object within a coroutine and
/// then fires an event to notify observers that they may now get the resulting object.
/// </summary>
/// <typeparam name="T">The type of object that the inheriting class claims to generate</typeparam>
public abstract class Generator<T> {
	public object _lock = new Object(); // Lock for multithreading

	/// <summary>
	/// Event fired when the generation coroutines and threads have all completed.
	/// </summary>
	public delegate void DoneEvent();
	public event DoneEvent OnDone;

	/// <summary>
	/// Begins generation of the generic object using the implementation specified by the inheriting class.
	/// </summary>
	public abstract IEnumerator Generate();

	/// <summary>
	/// Gets the result of the generation.
	/// </summary>
	/// <returns>The generic object that the inheriting class generates</returns>
	public abstract T GetResult();

	/// <summary>
	/// Fires the OnDone event. Executes any and all callback methods added to the OnDone event.
	/// </summary>
	public void Done() {
		if (OnDone != null) OnDone();
	}
}
