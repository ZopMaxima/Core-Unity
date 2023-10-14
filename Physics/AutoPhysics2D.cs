// AutoPhysics2D.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   November 5, 2022 - Looks like it's "Remember Remember" of November.

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Automated physics setup for prefabs.
	/// </summary>
	public class AutoPhysics2D : MonoBehaviour
	{
		private static readonly Collider2D[] EMPTY = new Collider2D[0];

		public AutoPhysicsMaterial2D Material;
		[Tooltip("Assign a manual shape to calculate this volume, else use each collider.")]
		public AutoPhysicsVolume Shape;
		[Tooltip("A 0-1 percentage representing the solid volume within this shape.")]
		[Range(0.0f, 1.0f)]
		public float Solid = 1.0f;
		[Tooltip("Assign a shell for this shape to calculate by surface area, else it is considered solid and uses volume.")]
		public float Shell = -1.0f;
		[Tooltip("Assign a manual Z-axis depth for this shape, else it guesses from colliders.")]
		public float Depth = -1.0f;

		public Rigidbody2D Rigidbody { get { return _rigidbody; } }
		public Collider2D[] Colliders { get { return _colliders; } }

		private Rigidbody2D _rigidbody;
		private Collider2D[] _colliders = EMPTY;

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
		public void Apply()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			if (Material != null && _rigidbody != null)
			{
				_rigidbody.useAutoMass = true;
				_colliders = GetComponentsInChildren<Collider2D>(); // TODO: Stop at any found Rigidbody.

				// Prisms
				if (Depth > 0)
				{
					if (Shell > 0) // TODO: Calculate shells by volume subtraction?
					{
						for (int i = 0; i < _colliders.Length; i++)
						{
							// TODO: Overlap?
							float area = _colliders[i].GetArea();
							float surface = _colliders[i].GetSurface(Depth);
							float volume = surface * Shell * Solid;
							_colliders[i].density = Material.Density * (1.0f / area) * volume;
							_colliders[i].sharedMaterial = Material.Material;
						}
					}
					else
					{
						for (int i = 0; i < _colliders.Length; i++)
						{
							// TODO: Overlap?
							_colliders[i].density = Material.Density * Depth * Solid;
							_colliders[i].sharedMaterial = Material.Material;
						}
					}
				}
				else
				{
					if (Shell > 0) // TODO: Calculate shells by volume subtraction?
					{
						for (int i = 0; i < _colliders.Length; i++)
						{
							// TODO: Overlap?
							float area = _colliders[i].GetArea();
							float surface = _colliders[i].GetSurface();
							float volume = surface * Shell * Solid;
							_colliders[i].density = Material.Density * (1.0f / area) * volume;
							_colliders[i].sharedMaterial = Material.Material;
						}
					}
					else
					{
						for (int i = 0; i < _colliders.Length; i++)
						{
							// TODO: Overlap?
							float area = _colliders[i].GetArea();
							float volume = _colliders[i].GetVolume() * Solid;
							_colliders[i].density = Material.Density * (1.0f / area) * volume;
							_colliders[i].sharedMaterial = Material.Material;
						}
					}
				}
			}
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// Inspector extension.
	/// </summary>
	[UnityEditor.CustomEditor(typeof(AutoPhysics2D), true), UnityEditor.CanEditMultipleObjects]
	public class AutoPhysics2DEditor : UnityEditor.Editor
	{
		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Apply"))
			{
				int end = targets.Length;
				for (int i = 0; i < end; i++)
				{
					AutoPhysics2D ap = targets[i] as AutoPhysics2D;
					if (ap != null)
					{
						ap.Apply();
					}
				}
			}
		}
	}
#endif
}