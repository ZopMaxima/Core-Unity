// AutoPhysicsMaterial.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   November 7, 2022

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// An extension of physics material data.
	/// </summary>
	[CreateAssetMenu(fileName = "[APMat] AutoPhysicsMaterial", menuName = "Zop/AutoPhysicsMaterial", order = 50)]
	public class AutoPhysicsMaterial : ScriptableObject
	{
		public string Name;
		public float Density;
		public PhysicMaterial Material;
	}
}