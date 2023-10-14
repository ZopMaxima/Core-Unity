// DestroyOnFallout.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 6, 2022

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Invoke an action this object if it leaves reasonable game boundaries.
	/// </summary>
	public class DestroyOnFallout : ActionOnFallout
	{
		/// <summary>
		/// Handle the fall-out event.
		/// </summary>
		protected override void OnFallout()
		{
			GameObject.Destroy(gameObject);
		}
	}
}