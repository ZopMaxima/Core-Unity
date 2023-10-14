// AutoPhysicsMaterial2D.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   November 5, 2022 - Looks like it's "Remember Remember" of November.

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// An extension of physics material data.
	/// </summary>
	[CreateAssetMenu(fileName = "[APMat] AutoPhysicsMaterial2D", menuName = "Zop/AutoPhysicsMaterial2D", order = 50)]
	public class AutoPhysicsMaterial2D : ScriptableObject
	{
		public string Name;
		public float Density;
		public PhysicsMaterial2D Material;
	}
}