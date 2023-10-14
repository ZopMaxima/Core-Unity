// AutoPhysicsSelection.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   November 7, 2022

using System;
using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Automated physics setup for prefabs.
	/// </summary>
	[Serializable]
	public class AutoPhysicsSelection
	{
		public AutoPhysicsMaterial Material;
		[Tooltip("Assign a manual shape to calculate this volume, else use each collider.")]
		public AutoPhysicsVolume Shape;
		[Tooltip("A 0-1 percentage representing the solid volume within this shape.")]
		[Range(0.0f, 1.0f)]
		public float Solid = 1.0f;
		[Tooltip("Assign a shell for this shape to calculate by surface area, else it is considered solid and uses volume.")]
		public float Shell = -1.0f;
		public Collider[] Colliders;
	}
}