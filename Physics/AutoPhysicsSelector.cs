// AutoPhysicsSelector.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   November 7, 2022

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Automated physics setup for prefabs.
	/// </summary>
	public class AutoPhysicsSelector : MonoBehaviour
	{
		public AutoPhysicsSelection[] Selections;

		public Rigidbody Rigidbody { get { return _rigidbody; } }

		private Rigidbody _rigidbody;

		/// <summary>
		/// Initialize.
		/// </summary>
		private void Awake()
		{
			Apply();
		}

		/// <summary>
		/// Apply on inspector change.
		/// </summary>
		private void OnValidate()
		{
			Apply();
		}

		/// <summary>
		/// Update physics system's auto-mass.
		/// </summary>
		private void Apply()
		{
			_rigidbody = GetComponent<Rigidbody>();
			if (_rigidbody != null)
			{
				float mass = 0;

				// Count
				int iEnd = Selections.Length;
				for (int i = 0; i < iEnd; i++)
				{
					AutoPhysicsSelection selection = Selections[i];
					if (selection != null)
					{
						int jEnd = selection.Colliders.Length;
						for (int j = 0; j < jEnd; j++)
						{
							Collider c = selection.Colliders[j];
							if (c != null)
							{
								mass += ApplyTo(c, selection);
							}
						}
					}
				}

				// Apply
				_rigidbody.mass = Mathf.Max(1, mass);
			}
		}

		/// <summary>
		/// Update physics system's auto-mass.
		/// </summary>
		private float ApplyTo(Collider collider, AutoPhysicsSelection selection)
		{
			float mass = 0;

			// Apply
			if (selection.Shell > 0) // TODO: Calculate shells by volume subtraction?
			{
				// TODO: Overlap?
				float surface = collider.GetSurface();
				float volume = surface * selection.Shell * selection.Solid;
				mass += volume * selection.Material.Density;
				collider.sharedMaterial = selection.Material.Material;
			}
			else
			{
				// TODO: Overlap?
				float volume = collider.GetVolume() * selection.Solid;
				mass += volume * selection.Material.Density;
				collider.sharedMaterial = selection.Material.Material;
			}

			// Return
			return mass;
		}
	}
}