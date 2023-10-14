// MonoSingleton.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   November 21, 2022

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Claim a singleton if this is the first instance to awake.
	/// </summary>
	public abstract class MonoSingleton<T> : MonoBehaviour
	{
		public static bool HasInstance { get { return _hasInstance; } }
		public static MonoSingleton<T> Instance { get { return _instance; } }

		private static bool _hasInstance;
		private static MonoSingleton<T> _instance;

		/// <summary>
		/// Initialize.
		/// </summary>
		public virtual void Awake()
		{
			if (_instance == null)
			{
				_hasInstance = true;
				_instance = this;
				InitializeSingleton();
			}
		}

		/// <summary>
		/// Terminate.
		/// </summary>
		public virtual void OnDestroy()
		{
			if (_instance == null)
			{
				_hasInstance = false;
				_instance = null;
				TerminateSingleton();
			}
		}

		/// <summary>
		/// Set up singleton variables.
		/// </summary>
		protected abstract void InitializeSingleton();

		/// <summary>
		/// Tear down singleton variables.
		/// </summary>
		protected abstract void TerminateSingleton();
	}
}