// CoroutineUpdater.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   March 19, 2023

using System.Collections;
using UnityEngine;
using Zop.Unity;

namespace Zop
{
	/// <summary>
	/// Purpose: Ease of use for coroutine timing.
	/// </summary>
	public class CoroutineUpdater : MonoSingleton<Coroutine>
	{
		public FullAction OnFixedUpdate;
		public FullAction OnCoroutineFixedUpdate;
		public FullAction OnUpdate;
		public FullAction OnCoroutineUpdate;
		public FullAction OnLateUpdate;
		public FullAction OnEndOfFrame;

		private static readonly WaitForFixedUpdate _waitFixed = new WaitForFixedUpdate();
		private static readonly WaitForEndOfFrame _waitEndOfFrame = new WaitForEndOfFrame();

		/// <summary>
		/// Set up singleton variables.
		/// </summary>
		protected override void InitializeSingleton()
		{
			StartCoroutine(FixedUpdateRoutine());
			StartCoroutine(UpdateRoutine());
			StartCoroutine(EndOfFrameRoutine());
		}

		/// <summary>
		/// Tear down singleton variables.
		/// </summary>
		protected override void TerminateSingleton()
		{
			StopAllCoroutines();
		}

		/// <summary>
		/// Update subscribed routines.
		/// </summary>
		private void FixedUpdate()
		{
			OnFixedUpdate.Invoke();
			OnFixedUpdate.Clear();
		}

		/// <summary>
		/// Update subscribed routines.
		/// </summary>
		private IEnumerator FixedUpdateRoutine()
		{
			while (enabled)
			{
				yield return _waitFixed;
				OnCoroutineFixedUpdate.Invoke();
				OnCoroutineFixedUpdate.Clear();
			}
		}

		/// <summary>
		/// Update subscribed routines.
		/// </summary>
		private void Update()
		{
			OnUpdate.Invoke();
			OnUpdate.Clear();
		}

		/// <summary>
		/// Update subscribed routines.
		/// </summary>
		private IEnumerator UpdateRoutine()
		{
			while (enabled)
			{
				yield return null;
				OnCoroutineUpdate.Invoke();
				OnCoroutineUpdate.Clear();
			}
		}

		/// <summary>
		/// Update subscribed routines.
		/// </summary>
		private void LateUpdate()
		{
			OnLateUpdate.Invoke();
			OnLateUpdate.Clear();
		}

		/// <summary>
		/// Update subscribed routines.
		/// </summary>
		private IEnumerator EndOfFrameRoutine()
		{
			while (enabled)
			{
				yield return _waitEndOfFrame;
				OnEndOfFrame.Invoke();
				OnEndOfFrame.Clear();
			}
		}
	}
}