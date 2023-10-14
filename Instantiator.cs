// Instantiator.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   January 24, 2021

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Instantiate a prefabricated object.
	/// </summary>
	public class Instantiator : MonoBehaviour
	{
		private static readonly Vector3 R = new Vector3(0.5f, 0, 0);
		private static readonly Vector3 U = new Vector3(0, 1.0f, 0);
		private static readonly Vector3 F = new Vector3(0, 0, 0.5f);
		private static readonly Vector3 D = new Vector3(0, -0.1f, 0);

		public GameObject Prefab;
		public Transform Parent;
		public Vector3 Position;
		public Vector3 Rotation;
		public bool LocalSpace;

		/// <summary>
		/// Cache references.
		/// </summary>
		public void Awake()
		{
			Instantiate(Prefab, Parent, Position, Rotation, LocalSpace);
		}

		/// <summary>
		/// Show the selected position.
		/// </summary>
		public void OnDrawGizmos()
		{
			DrawTransformGizmo(Prefab, Parent, Position, Rotation, LocalSpace);
		}

		/// <summary>
		/// Instantiate and reposition a prefab.
		/// </summary>
		public static void Instantiate(GameObject prefab, Transform parent, Vector3 position, Vector3 rotation, bool localSpace)
		{
			if (prefab != null)
			{
				if (localSpace)
				{
					GameObject go = Instantiate(prefab, parent);
					go.transform.localPosition = position;
					go.transform.localRotation = Quaternion.Euler(rotation);
				}
				else
				{
					Instantiate(prefab, position, Quaternion.Euler(rotation), parent);
				}
			}
		}

		/// <summary>
		/// Draw a gizmo to show this transform position.
		/// </summary>
		public static void DrawTransformGizmo(GameObject prefab, Transform parent, Vector3 position, Vector3 rotation, bool localSpace = false)
		{
			DrawTransformGizmo(prefab, parent, position, Quaternion.Euler(rotation), localSpace);
		}

		/// <summary>
		/// Draw a gizmo to show this transform position.
		/// </summary>
		public static void DrawTransformGizmo(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, bool localSpace = false)
		{
			// Select space.
			if (localSpace && parent != null)
			{
				position = parent.transform.position + (parent.transform.rotation * position);
				rotation = parent.transform.rotation * rotation;
			}

			// Draw
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position, position + (rotation * R));
			Gizmos.color = Color.green;
			Gizmos.DrawLine(position, position + (rotation * U));
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(position, position + (rotation * F));
#if UNITY_EDITOR
			UnityEditor.Handles.color = Color.blue;
			UnityEditor.Handles.DrawSolidDisc(position + (rotation * F), Camera.current.transform.forward, 0.05f);
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawSolidDisc(position + (rotation * R), Camera.current.transform.forward, 0.05f);
			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawSolidDisc(position + (rotation * U), Camera.current.transform.forward, 0.05f);
			UnityEditor.Handles.DrawSolidDisc(position, Camera.current.transform.forward, 0.05f);
			UnityEditor.Handles.color = Color.white;
			UnityEditor.Handles.Label(position + D, (prefab != null) ? (prefab.name) : ("null"));
#endif
		}
	}
}