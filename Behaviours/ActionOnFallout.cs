// ActionOnFallout.cs
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
	public abstract class ActionOnFallout : MonoBehaviour
	{
		private const float MAX_BOUNDS = 10000.0f;
		private const float MIN_BOUNDS = -MAX_BOUNDS;

		public bool IsFallen { get { return _isFallen; } }

		private bool _isFallen;

		/// <summary>
		/// Poll velocity.
		/// </summary>
		protected virtual void Update()
		{
			TryFallout();
		}

		/// <summary>
		/// Poll velocity and apply.
		/// </summary>
		private void TryFallout()
		{
			if (!_isFallen)
			{
				Vector3 position = transform.position;
				if (position.x > MAX_BOUNDS || position.x < MIN_BOUNDS || position.y > MAX_BOUNDS || position.y < MIN_BOUNDS || position.z > MAX_BOUNDS || position.z < MIN_BOUNDS)
				{
					_isFallen = true;
					OnFallout();
				}
			}
		}

		/// <summary>
		/// Handle the fall-out event.
		/// </summary>
		protected abstract void OnFallout();
	}
}