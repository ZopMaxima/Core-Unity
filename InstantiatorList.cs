// InstantiatorList.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   January 24, 2021

using System;
using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Instantiate a list prefabricated objects.
	/// </summary>
	public class InstantiatorList : MonoBehaviour
	{
		/// <summary>
		/// Instantiation parameters.
		/// </summary>
		[Serializable]
		public class Parameters
		{
			public GameObject Prefab;
			public Transform Parent;
			public Vector3 Position;
			public Vector3 Rotation;
			public bool LocalSpace;
		}

		public Parameters[] Instantiations;

		/// <summary>
		/// Cache references.
		/// </summary>
		public void Awake()
		{
			for (int i = 0; i < Instantiations.Length; i++)
			{
				Parameters p = Instantiations[i];
				if (p != null)
				{
					Instantiator.Instantiate(p.Prefab, p.Parent, p.Position, p.Rotation, p.LocalSpace);
				}
			}
		}

		/// <summary>
		/// Show the selected position.
		/// </summary>
		public void OnDrawGizmos()
		{
			for (int i = 0; i < Instantiations.Length; i++)
			{
				Parameters p = Instantiations[i];
				if (p != null)
				{
					Instantiator.DrawTransformGizmo(p.Prefab, p.Parent, p.Position, p.Rotation, p.LocalSpace);
				}
			}
		}
	}
}