using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface Generator<T> {
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	T Generate();
}
