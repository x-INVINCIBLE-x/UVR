using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LunaWolfStudiosEditor.ScriptableSheets.Comparables
{
	public static class ComparableUtility
	{
		public static IComparable GetAssetPathComparable<TObject>(TObject obj) where TObject : Object
		{
			var assetPath = AssetDatabase.GetAssetPath(obj);
			if (!string.IsNullOrEmpty(assetPath))
			{
				return assetPath;
			}
			else
			{
				Debug.LogWarning($"Cannot find asset path for '{obj.name}'.");
			}
			return string.Empty;
		}

		public static IComparable GetGUIDComparable<TObject>(TObject obj) where TObject : Object
		{
			var assetPath = AssetDatabase.GetAssetPath(obj);
			if (!string.IsNullOrEmpty(assetPath))
			{
				var guid = AssetDatabase.AssetPathToGUID(assetPath);
				return guid;
			}
			else
			{
				Debug.LogWarning($"Cannot find asset path for '{obj.name}'.");
			}
			return string.Empty;
		}

		public static IComparable GetPropertyComparable<TObject>(TObject obj, string propertyPath) where TObject : Object
		{
			var serializedObject = new SerializedObject(obj);
			var property = serializedObject.FindProperty(propertyPath);
			if (property != null)
			{
				var propertyType = property.propertyType;
				switch (propertyType)
				{
					case SerializedPropertyType.Integer:
						if (property.IsTypeInt())
						{
							return property.intValue;
						}
						if (property.IsTypeLong())
						{
							return property.longValue;
						}
#if UNITY_2022_1_OR_NEWER
						else if (property.IsTypeUInt())
						{
							return property.uintValue;
						}
						else if (property.IsTypeULong())
						{
							return property.ulongValue;
						}
#endif
						else
						{
							return property.intValue;
						}

					case SerializedPropertyType.LayerMask:
					case SerializedPropertyType.Enum:
					case SerializedPropertyType.ArraySize:
					case SerializedPropertyType.Character:
						return property.intValue;

					case SerializedPropertyType.Boolean:
						return property.boolValue;

					case SerializedPropertyType.Float:
						if (property.IsTypeFloat())
						{
							return property.floatValue;
						}
						else if (property.IsTypeDouble())
						{
							return property.doubleValue;
						}
						else
						{
							return property.floatValue;
						}

					case SerializedPropertyType.String:
						return property.stringValue;

					case SerializedPropertyType.Color:
						return (ColorComparable) property.colorValue;

					case SerializedPropertyType.ObjectReference:
						if (property.objectReferenceValue != null)
						{
							return property.objectReferenceValue.name;
						}
						break;

					case SerializedPropertyType.AnimationCurve:
						return (AnimationCurveComparable) property.animationCurveValue;

					case SerializedPropertyType.Gradient:
						return (GradientComparable) property.GetGradientValue();

					default:
						Debug.LogWarning($"Unsupported property type '{propertyType}' for property at path '{property.propertyPath}' for Object '{obj.name}'.");
						break;
				}
			}
			return null;
		}
	}
}
