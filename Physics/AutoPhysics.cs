// AutoPhysics.cs
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
	public class AutoPhysics : MonoBehaviour
	{
		private static readonly Collider[] EMPTY = new Collider[0];

		public AutoPhysicsMaterial Material;
		[Tooltip("Assign a manual shape to calculate this volume, else use each collider.")]
		public AutoPhysicsVolume Shape;
		[Tooltip("A 0-1 percentage representing the solid volume within this shape.")]
		[Range(0.0f, 1.0f)]
		public float Solid = 1.0f;
		[Tooltip("Assign a shell for this shape to calculate by surface area, else it is considered solid and uses volume.")]
		public float Shell = -1.0f;

		public Rigidbody Rigidbody { get { return _rigidbody; } }
		public Collider[] Colliders { get { return _colliders; } }

		private Rigidbody _rigidbody;
		private Collider[] _colliders = EMPTY;

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
			_rigidbody = GetComponent<Rigidbody>();
			if (Material != null && _rigidbody != null)
			{
				_colliders = GetComponentsInChildren<Collider>(); // TODO: Stop at any found Rigidbody.

				// Count
				float mass = 0;
				if (Shell > 0) // TODO: Calculate shells by volume subtraction?
				{
					for (int i = 0; i < _colliders.Length; i++)
					{
						// TODO: Overlap?
						float surface = _colliders[i].GetSurface();
						float volume = surface * Shell * Solid;
						mass += volume * Material.Density;
						_colliders[i].sharedMaterial = Material.Material;
					}
				}
				else
				{
					for (int i = 0; i < _colliders.Length; i++)
					{
						// TODO: Overlap?
						float volume = _colliders[i].GetVolume() * Solid;
						mass += volume * Material.Density;
						_colliders[i].sharedMaterial = Material.Material;
					}
				}

				// Apply
				_rigidbody.mass = mass;
			}
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// Inspector extension.
	/// </summary>
	[UnityEditor.CustomEditor(typeof(AutoPhysics), true), UnityEditor.CanEditMultipleObjects]
	public class AutoPhysicsEditor : UnityEditor.Editor
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
					AutoPhysics ap = targets[i] as AutoPhysics;
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