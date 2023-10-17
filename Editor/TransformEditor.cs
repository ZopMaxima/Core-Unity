// TransformEditor.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   January 30, 2021

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Add transform reset buttons as seen on NGUI.
	/// </summary>
	[CustomEditor(typeof(Transform), true), CanEditMultipleObjects]
	class TransformEditor : Editor
	{
		private const string FALLBACK_ARROW = "\u2190";
		private const string TOOLTIP_RESET_POSITION = "Reset Position";
		private const string TOOLTIP_RESET_ROTATION = "Reset Rotation";
		private const string TOOLTIP_RESET_SCALE = "Reset Scale";

		private Texture2D _resetTexture;
		private SerializedProperty _localPosition;
		private SerializedProperty _localRotation;
		private SerializedProperty _localScale;
		private object _rotationGUI;
		private GUIContent _buttonContentP;
		private GUIContent _buttonContentR;
		private GUIContent _buttonContentS;

		/// <summary>
		/// Cache properties.
		/// </summary>
		public void OnEnable()
		{
			if (serializedObject != null)
			{
				_resetTexture = (Texture2D)Resources.Load("EditorRevert", typeof(Texture2D));
				_localPosition = serializedObject.FindProperty("m_LocalPosition");
				_localRotation = serializedObject.FindProperty("m_LocalRotation");
				_localScale = serializedObject.FindProperty("m_LocalScale");
				if (_rotationGUI == null)
				{
					_rotationGUI = System.Activator.CreateInstance(typeof(SerializedProperty).Assembly.GetType("UnityEditor.TransformRotationGUI", false, false));
				}
				_rotationGUI.GetType().GetMethod("OnEnable").Invoke(_rotationGUI, new object[] { _localRotation, new GUIContent("Rotation") });
				_buttonContentP = _resetTexture != null ? new GUIContent(_resetTexture, TOOLTIP_RESET_POSITION) : new GUIContent(FALLBACK_ARROW, TOOLTIP_RESET_POSITION);
				_buttonContentR = _resetTexture != null ? new GUIContent(_resetTexture, TOOLTIP_RESET_ROTATION) : new GUIContent(FALLBACK_ARROW, TOOLTIP_RESET_ROTATION);
				_buttonContentS = _resetTexture != null ? new GUIContent(_resetTexture, TOOLTIP_RESET_SCALE) : new GUIContent(FALLBACK_ARROW, TOOLTIP_RESET_SCALE);
			}
		}

		/// <summary>
		/// Redraw the inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			SerializedObject obj = this.serializedObject;
			if (obj != null)
			{
				obj.Update();

				// Position
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(_buttonContentP, EditorStyles.miniButtonLeft, GUILayout.Width(19)))
				{
					_localPosition.vector3Value = Vector3.zero;
				}
				EditorGUILayout.PropertyField(_localPosition, new GUIContent("Position"));
				EditorGUILayout.EndHorizontal();

				// Rotation
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(_buttonContentR, EditorStyles.miniButtonLeft, GUILayout.Width(19)))
				{
					_localRotation.quaternionValue = Quaternion.identity;
				}
				_rotationGUI.GetType().GetMethod("RotationField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(bool) }, null).Invoke(_rotationGUI, new object[] { false });
				EditorGUILayout.EndHorizontal();

				// Scale
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(_buttonContentS, EditorStyles.miniButtonLeft, GUILayout.Width(19)))
				{
					_localScale.vector3Value = Vector3.one;
				}
				EditorGUILayout.PropertyField(_localScale, new GUIContent("Scale"));
				EditorGUILayout.EndHorizontal();

				// Apply
				obj.ApplyModifiedProperties();
			}
		}
	}
}