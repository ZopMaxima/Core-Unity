// AutoPhysicsSelector2D.cs
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
	public class AutoPhysicsSelector2D : MonoBehaviour
	{
		public AutoPhysicsSelection2D[] Selections;

		public Rigidbody2D Rigidbody { get { return _rigidbody; } }

		private Rigidbody2D _rigidbody;

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
			_rigidbody = GetComponent<Rigidbody2D>();
			if (_rigidbody != null)
			{
				_rigidbody.useAutoMass = true;
				int iEnd = Selections.Length;
				for (int i = 0; i < iEnd; i++)
				{
					AutoPhysicsSelection2D selection = Selections[i];
					if (selection != null)
					{
						int jEnd = selection.Colliders.Length;
						for (int j = 0; j < jEnd; j++)
						{
							Collider2D c = selection.Colliders[j];
							if (c != null)
							{
								ApplyTo(c, selection);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Update physics system's auto-mass.
		/// </summary>
		private void ApplyTo(Collider2D collider, AutoPhysicsSelection2D selection)
		{
			// Prism, else imitate 3D counterparts.
			if (selection.Depth > 0)
			{
				if (selection.Shell > 0) // TODO: Calculate shells by volume subtraction?
				{
					// TODO: Overlap?
					float area = collider.GetArea();
					float surface = collider.GetSurface(selection.Depth);
					float volume = surface * selection.Shell * selection.Solid;
					collider.density = selection.Material.Density * (1.0f / area) * volume;
					collider.sharedMaterial = selection.Material.Material;
				}
				else
				{
					// TODO: Overlap?
					collider.density = selection.Material.Density * selection.Depth * selection.Solid;
					collider.sharedMaterial = selection.Material.Material;
				}
			}
			else
			{
				if (selection.Shell > 0) // TODO: Calculate shells by volume subtraction?
				{
					// TODO: Overlap?
					float area = collider.GetArea();
					float surface = collider.GetSurface();
					float volume = surface * selection.Shell * selection.Solid;
					collider.density = selection.Material.Density * (1.0f / area) * volume;
					collider.sharedMaterial = selection.Material.Material;
				}
				else
				{
					// TODO: Overlap?
					float area = collider.GetArea();
					float volume = collider.GetVolume() * selection.Solid;
					collider.density = selection.Material.Density * (1.0f / area) * volume;
					collider.sharedMaterial = selection.Material.Material;
				}
			}
		}
	}
}