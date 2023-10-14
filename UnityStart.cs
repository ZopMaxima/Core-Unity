// UnityStart.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 10, 2022

using System;

namespace Zop.Unity
{
	/// <summary>
	/// Selectable enumeration of Unity start methods.
	/// </summary>
	public enum UnityStart
	{
		None = 0,
		Awake = 1 << 0,
		OnEnable = 1 << 1,
		Start = 1 << 2,
	}

	/// <summary>
	/// Selectable enumeration of Unity start methods.
	/// </summary>
	[Flags]
	public enum UnityStartMask
	{
		None = 0,
		Awake = UnityUpdate.Update,
		OnEnable = UnityUpdate.LateUpdate,
		Start = UnityUpdate.FixedUpdate,
	}
}