// UnityUpdate.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 10, 2022

using System;

namespace Zop.Unity
{
	/// <summary>
	/// Selectable enumeration of Unity update methods.
	/// </summary>
	public enum UnityUpdate
	{
		None = 0,
		Update = 1 << 0,
		LateUpdate = 1 << 1,
		FixedUpdate = 1 << 2,
	}

	/// <summary>
	/// Selectable enumeration of Unity update methods.
	/// </summary>
	[Flags]
	public enum UnityUpdateMask
	{
		None = 0,
		Update = UnityUpdate.Update,
		LateUpdate = UnityUpdate.LateUpdate,
		FixedUpdate = UnityUpdate.FixedUpdate,
	}
}