// WaitForSecondsCache.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   December 3, 2023

using System.Collections.Generic;
using UnityEngine;

namespace Zop
{
	/// <summary>
	/// Returns repeatable wait timers.
	/// </summary>
	public static class WaitForSecondsCache
	{
		private static readonly Dictionary<float, WaitForSeconds> _cache = new Dictionary<float, WaitForSeconds>();

		/// <summary>
		/// Returns a wait instruction with the given duration.
		/// </summary>
		public static WaitForSeconds Get(float duration)
		{
			if (!_cache.TryGetValue(duration, out WaitForSeconds wait))
			{
				wait = new WaitForSeconds(duration);
				_cache.Add(duration, wait);
			}
			return wait;
		}
	}
}