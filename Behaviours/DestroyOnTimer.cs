// DestroyOnTimer.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   November 21, 2022

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Destroy this object after a timer expires.
	/// </summary>
	public class DestroyOnTimer : MonoBehaviour
	{
		public float Time = 10.0f;

		/// <summary>
		/// Initialize.
		/// </summary>
		private void Start()
		{
			GameObject.Destroy(gameObject, Time);
		}
	}
}