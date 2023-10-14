// DestroyOnBurnup.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 6, 2022

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Invoke an action on this object if it reaches excessive speeds.
	/// </summary>
	public class DestroyOnBurnup : ActionOnBurnup
	{
		/// <summary>
		/// Handle the burn-up event.
		/// </summary>
		protected override void OnBurnup()
		{
			GameObject.Destroy(gameObject);
		}
	}
}