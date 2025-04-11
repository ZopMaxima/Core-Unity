// ActionOnBurnup.cs
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
	public abstract class ActionOnBurnup : MonoBehaviour
	{
		private const float MAX_VELOCITY = 1000.0f;
		private const float MAX_VELOCITY_SQR = MAX_VELOCITY * MAX_VELOCITY;

		public bool IsBurned { get { return _isBurned; } }

		private Rigidbody _rigidbody;
		private Rigidbody2D _rigidbody2D;
		private bool _is2D;
		private bool _isBurned;

		/// <summary>
		/// Cache rigidbodies.
		/// </summary>
		protected virtual void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody2D = GetComponent<Rigidbody2D>();
			_is2D = _rigidbody2D != null;
		}

		/// <summary>
		/// Poll velocity.
		/// </summary>
		protected virtual void Update()
		{
			TryBurnup();
		}

		/// <summary>
		/// Poll velocity and apply.
		/// </summary>
		private void TryBurnup()
		{
			if (!_isBurned)
			{
#if UNITY_6000_0_OR_NEWER
				Vector3 velocity = (_is2D && _rigidbody2D != null) ? (_rigidbody2D.linearVelocity) : (_rigidbody != null) ? (_rigidbody.linearVelocity) : (Vector3.zero);
#else
				Vector3 velocity = (_is2D && _rigidbody2D != null) ? (_rigidbody2D.velocity) : (_rigidbody != null) ? (_rigidbody.velocity) : (Vector3.zero);
#endif
				if (velocity.sqrMagnitude > MAX_VELOCITY_SQR)
				{
					_isBurned = true;
					OnBurnup();
				}
			}
		}

		/// <summary>
		/// Handle the burn-up event.
		/// </summary>
		protected abstract void OnBurnup();
	}
}