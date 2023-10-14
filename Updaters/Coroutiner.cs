// Coroutiner.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   March 19, 2023

using System;
using System.Collections;
using UnityEngine;

namespace Zop
{
	/// <summary>
	/// Purpose: Ease of use for coroutine timing.
	/// </summary>
	public static class Coroutiner
	{
		/// <summary>
		/// Returns a coroutine-capable monobehaviour.
		/// </summary>
		private static CoroutineUpdater GetUpdater()
		{
			if (!CoroutineUpdater.HasInstance)
			{
				GameObject instance = new GameObject(typeof(CoroutineUpdater).Name, typeof(CoroutineUpdater));
				GameObject.DontDestroyOnLoad(instance);
			}
			return CoroutineUpdater.Instance as CoroutineUpdater;
		}

		/// <summary>
		/// Run a coroutine.
		/// </summary>
		public static void StartCoroutine(IEnumerator coroutine)
		{
			if (coroutine != null)
			{
				GetUpdater().StartCoroutine(coroutine);
			}
		}

		/// <summary>
		/// Run a coroutine.
		/// </summary>
		public static void NextFixedUpdate(Action action)
		{
			if (action != null)
			{
				GetUpdater().OnFixedUpdate.Add(action);
			}
		}

		/// <summary>
		/// Run a coroutine.
		/// </summary>
		public static void NextEndOfFixedUpdate(Action action)
		{
			if (action != null)
			{
				GetUpdater().OnCoroutineFixedUpdate.Add(action);
			}
		}

		/// <summary>
		/// Run a coroutine.
		/// </summary>
		public static void NextUpdate(Action action)
		{
			if (action != null)
			{
				GetUpdater().OnUpdate.Add(action);
			}
		}

		/// <summary>
		/// Run a coroutine.
		/// </summary>
		public static void NextEndOfUpdate(Action action)
		{
			if (action != null)
			{
				GetUpdater().OnCoroutineUpdate.Add(action);
			}
		}

		/// <summary>
		/// Run a coroutine.
		/// </summary>
		public static void NextLateUpdate(Action action)
		{
			if (action != null)
			{
				GetUpdater().OnLateUpdate.Add(action);
			}
		}

		/// <summary>
		/// Run a coroutine.
		/// </summary>
		public static void NextEndOfFrame(Action action)
		{
			if (action != null)
			{
				GetUpdater().OnEndOfFrame.Add(action);
			}
		}
	}
}