// GameObjectPool.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   January 18, 2021

using System.Collections.Generic;
using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// A pool for Unity game objects.
	/// </summary>
	public class GameObjectPool
	{
		private GameObject _prefab;
		private Stack<GameObject> _cached = new Stack<GameObject>();
		private HashSet<GameObject> _active = new HashSet<GameObject>();

		/// <summary>
		/// Construct with the reference prefab.
		/// </summary>
		public GameObjectPool(GameObject prefab)
		{
			_prefab = prefab;
		}

		/// <summary>
		/// Get an instance from the pool.
		/// </summary>
		public GameObject Get(Transform parent)
		{
			GameObject result = null;

			// Try the cache.
			bool isNull = true;
			if (_cached.Count > 0)
			{
				do
				{
					result = _cached.Pop();
					isNull = result == null;
				}
				while (isNull && _cached.Count > 0);
			}

			// Instantiate
			if (isNull)
			{
				result = GameObject.Instantiate(_prefab, parent);
			}

			// Return
			_active.Add(result);
			result.TryActivate(true);
			return result;
		}

		/// <summary>
		/// Return an instance to the pool.
		/// </summary>
		public void Return(GameObject instance)
		{
			if (_active.Remove(instance) && instance != null)
			{
				_cached.Push(instance);
				instance.TryActivate(false);
			}
		}

		/// <summary>
		/// Purge all pooled items.
		/// </summary>
		public void Clear()
		{
			foreach (GameObject instance in _active)
			{
				Return(instance);
			}
			while (_cached.Count > 0)
			{
				GameObject.Destroy(_cached.Pop());
			}
			_cached.Clear();
			_active.Clear();
		}
	}
}