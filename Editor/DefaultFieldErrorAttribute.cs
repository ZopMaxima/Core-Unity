// DefaultFieldErrorAttribute.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   December 3, 2023

using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Highlight this inspector field with an error colou when the value is default.
/// </summary>
public class DefaultFieldErrorAttribute : PropertyAttribute { }

#if UNITY_EDITOR

/// <summary>
/// Draw the error highlight for the editor.
/// </summary>
[CustomPropertyDrawer(typeof(DefaultFieldErrorAttribute))]
public class DefaultFieldErrorAttributeDrawer : PropertyDrawer
{
	private static readonly Color _errorColour = new Color(1.0f, 0.5f, 0.5f);

	/// <summary>
	/// Recolour if the property is default.
	/// </summary>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Color background = GUI.backgroundColor;
		GUI.backgroundColor = IsDefaultValue(property) ? _errorColour : background;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.backgroundColor = background;
	}

	/// <summary>
	/// Returns true if the value is default.
	/// </summary>
	private static bool IsDefaultValue(SerializedProperty property)
	{
		switch (property.propertyType)
		{
			case SerializedPropertyType.Integer: return property.intValue == default;
			case SerializedPropertyType.Boolean: return property.boolValue == default;
			case SerializedPropertyType.Float: return property.floatValue == default;
			case SerializedPropertyType.String: return property.stringValue == default;
			case SerializedPropertyType.Color: return property.colorValue == default;
			case SerializedPropertyType.ObjectReference: return property.objectReferenceValue == default;
			case SerializedPropertyType.LayerMask: return property.intValue == default;
			case SerializedPropertyType.Enum: return property.enumValueFlag == default;
			case SerializedPropertyType.Vector2: return property.vector2Value == default;
			case SerializedPropertyType.Vector3: return property.vector3Value == default;
			case SerializedPropertyType.Vector4: return property.vector4Value == default;
			case SerializedPropertyType.Rect: return property.rectValue == default;
			case SerializedPropertyType.ArraySize: return property.intValue == default;
			case SerializedPropertyType.Character: return property.intValue == default;
			case SerializedPropertyType.AnimationCurve: return property.animationCurveValue == default;
			case SerializedPropertyType.Bounds: return property.boundsValue == default;
			case SerializedPropertyType.Gradient: return false; // NOTE: 'gradientValue' is internal, and Gradient is a class.
			case SerializedPropertyType.Quaternion: return property.quaternionValue == default;
			case SerializedPropertyType.ExposedReference: return property.exposedReferenceValue == default;
			case SerializedPropertyType.FixedBufferSize: return property.fixedBufferSize == default; // NOTE: No value accessor, assumed property.
			case SerializedPropertyType.Vector2Int: return property.vector2IntValue == default;
			case SerializedPropertyType.Vector3Int: return property.vector3IntValue == default;
			case SerializedPropertyType.RectInt: return property.rectIntValue.min == default && property.rectIntValue.max == default; // NOTE: No default comparer.
			case SerializedPropertyType.BoundsInt: return property.boundsIntValue == default;
			case SerializedPropertyType.ManagedReference: return property.managedReferenceValue == default;
			case SerializedPropertyType.Hash128: return property.hash128Value == default;
			default: return false;
		}
	}
}
#endif
