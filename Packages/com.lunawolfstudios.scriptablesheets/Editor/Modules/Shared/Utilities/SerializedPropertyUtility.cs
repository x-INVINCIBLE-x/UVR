using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	public static class SerializedPropertyUtility
	{
		public static readonly Object[] UnityBuiltInAssets;

		private static readonly List<SerializedPropertyType> s_InputFieldPropertyTypes = new List<SerializedPropertyType>()
		{
			SerializedPropertyType.Integer,
			SerializedPropertyType.Float,
			SerializedPropertyType.String,
			SerializedPropertyType.ArraySize,
			SerializedPropertyType.Character,
		};

		// Some Object fields are marked as editable when they shouldn't be. This list helps catch those.
		private static readonly List<string> s_ReadOnlyUnityFields = new List<string>() { UnityConstants.Field.Script };

		// Unity has a duplicate StaticEditorFlags value for ContributeGI and LightmapStatic so we remove one of them.
		private static readonly string[] s_StaticEditorFlags = Enum.GetNames(typeof(StaticEditorFlags)).Where(name => name != "LightmapStatic").ToArray();

		static SerializedPropertyUtility()
		{
			UnityBuiltInAssets = AssetDatabase.LoadAllAssetsAtPath(UnityConstants.Path.BuiltInExtra);
		}

		// Draw our own fields to eliminate unwanted header and text attributes that property field uses by default.
		public static void DrawProperty(this SerializedProperty property, Rect propertyRect, Object rootObject, bool isCustomField, out bool arraySizeChanged)
		{
			arraySizeChanged = false;
			// There are some fields on other Unity Asset types that are inaccessible. So only draw our own custom fields.
			if (isCustomField)
			{
				var propertyType = property.propertyType;
				switch (propertyType)
				{
					case SerializedPropertyType.Integer:
						if (property.IsTypeInt())
						{
							property.intValue = EditorGUI.IntField(propertyRect, property.intValue);
						}
						else if (property.IsTypeLong())
						{
							property.longValue = EditorGUI.LongField(propertyRect, property.longValue);
						}
#if UNITY_2022_1_OR_NEWER
						else if (property.IsTypeUInt())
						{
							property.uintValue = DrawUtility.GUI.UIntField(propertyRect, property.uintValue);
						}
						else if (property.IsTypeULong())
						{
							property.ulongValue = DrawUtility.GUI.ULongField(propertyRect, property.ulongValue);
						}
#endif
						else
						{
							property.intValue = EditorGUI.IntField(propertyRect, property.intValue);
						}
						break;

					case SerializedPropertyType.ArraySize:
						var previousValue = property.intValue;
						property.intValue = EditorGUI.IntField(propertyRect, property.intValue);
						if (previousValue != property.intValue)
						{
							arraySizeChanged = true;
						}
						break;

					case SerializedPropertyType.Boolean:
						property.boolValue = DrawUtility.GUI.ToggleCenter(propertyRect, property.boolValue);
						break;

					case SerializedPropertyType.Float:
						if (property.IsTypeFloat())
						{
							property.floatValue = EditorGUI.FloatField(propertyRect, property.floatValue);
						}
						else if (property.IsTypeDouble())
						{
							property.doubleValue = EditorGUI.DoubleField(propertyRect, property.floatValue);
						}
						else
						{
							property.floatValue = EditorGUI.FloatField(propertyRect, property.floatValue);
						}
						break;

					case SerializedPropertyType.String:
						property.stringValue = EditorGUI.TextField(propertyRect, property.stringValue);
						break;

					case SerializedPropertyType.Color:
						property.colorValue = EditorGUI.ColorField(propertyRect, GUIContent.none, property.colorValue, true, true, false);
						break;

					case SerializedPropertyType.ObjectReference:
						var objectType = ReflectionUtility.GetNestedFieldType(rootObject.GetType(), property.propertyPath);
						if (objectType != null)
						{
							property.objectReferenceValue = EditorGUI.ObjectField(propertyRect, property.objectReferenceValue, objectType, false);
						}
						else
						{
							EditorGUI.PropertyField(propertyRect, property, GUIContent.none, false);
						}
						break;

					case SerializedPropertyType.LayerMask:
						property.intValue = EditorGUI.MaskField(propertyRect, GUIContent.none, property.intValue, InternalEditorUtility.layers);
						break;

					case SerializedPropertyType.Enum:
						if (property.TryGetEnumType(rootObject, out Type enumType))
						{
							if (enumType.HasFlagsAttribute())
							{
								// Remove none and bitwise combination values.
								var filteredEnumNames = new List<string>();
								foreach (var value in Enum.GetValues(enumType))
								{
									var intValue = (int) value;
									if (intValue != 0 && (intValue & (intValue - 1)) == 0)
									{
										filteredEnumNames.Add(value.ToString());
									}
								}
								property.intValue = EditorGUI.MaskField(propertyRect, GUIContent.none, property.intValue, filteredEnumNames.ToArray());
							}
							else
							{
								property.intValue = Convert.ToInt32(EditorGUI.EnumPopup(propertyRect, (Enum) Enum.ToObject(enumType, property.intValue)));
							}
						}
						break;

					case SerializedPropertyType.Character:
						var stringValue = EditorGUI.TextField(propertyRect, ((char) property.intValue).ToString());
						if (!string.IsNullOrEmpty(stringValue))
						{
							property.intValue = stringValue[0];
						}
						break;

					case SerializedPropertyType.AnimationCurve:
						property.animationCurveValue = EditorGUI.CurveField(propertyRect, property.animationCurveValue);
						break;

					case SerializedPropertyType.Gradient:
						var gradientValue = property.GetGradientValue();
						property.SetGradientValue(EditorGUI.GradientField(propertyRect, gradientValue));
						break;

					default:
						EditorGUI.PropertyField(propertyRect, property, GUIContent.none, false);
						break;
				}
			}
			else
			{
				// Draw Unity layer and tag fields accordingly.
				switch (property.propertyPath)
				{
					case UnityConstants.Field.Layer:
						property.intValue = EditorGUI.LayerField(propertyRect, property.intValue);
						break;

					case UnityConstants.Field.StaticEditorFlags:
						var newValue = EditorGUI.MaskField(propertyRect, property.intValue, s_StaticEditorFlags);
						// Unity uses -1 to represent everything in a mask field, but StaticEditorFlags uses int.MaxValue to represent everything.
						if (newValue <= -1)
						{
							newValue = int.MaxValue;
						}
						property.intValue = newValue;
						break;

					case UnityConstants.Field.Tag:
						property.stringValue = EditorGUI.TagField(propertyRect, property.stringValue);
						break;

					default:
						if (property.propertyType == SerializedPropertyType.Boolean)
						{
							property.boolValue = DrawUtility.GUI.ToggleCenter(propertyRect, property.boolValue);
						}
						else
						{
							EditorGUI.PropertyField(propertyRect, property, GUIContent.none, false);
						}
						break;
				}
			}
		}

		public static SerializedPropertyType GetSheetsPropertyType(this SerializedProperty property, bool isScriptableObject)
		{
			if (isScriptableObject)
			{
				return property.propertyType;
			}
			// Handle special cases like Layers and Tags where Unity considers them int and string fields respectively.
			switch (property.propertyPath)
			{
				case UnityConstants.Field.Layer:
					return SerializedPropertyType.LayerMask;

				case UnityConstants.Field.StaticEditorFlags:
				case UnityConstants.Field.Tag:
					return SerializedPropertyType.Enum;

				default:
					return property.propertyType;
			}
		}

		public static string FriendlyPropertyPath(this SerializedProperty property)
		{
			return FriendlyPropertyPath(property.propertyPath, property.displayName);
		}

		public static string FriendlyPropertyPath(string propertyPath, string displayName)
		{
			var friendlyPath = propertyPath;
			if (friendlyPath.Contains('.'))
			{
				friendlyPath = friendlyPath.Replace(UnityConstants.ArrayPropertyPath, "[");
				friendlyPath = friendlyPath.Replace($".{UnityConstants.Field.Array}.{UnityConstants.Field.ArraySize}", $".{UnityConstants.Field.ArraySize}");
				friendlyPath = friendlyPath.Replace("m_", string.Empty);
				if (friendlyPath.Length > 1)
				{
					var lastIndex = friendlyPath.LastIndexOf('.');
					if (lastIndex > 0)
					{
						var secondLastIndex = friendlyPath.LastIndexOf('.', lastIndex - 1);
						if (secondLastIndex > -1)
						{
							friendlyPath = friendlyPath.Substring(secondLastIndex + 1);
						}
						else if (friendlyPath.Contains(']') && !friendlyPath.Contains("]."))
						{
							var lastSegment = friendlyPath.Substring(lastIndex + 1);
							friendlyPath = lastSegment;
						}
					}
					friendlyPath = friendlyPath.Replace('.', ' ').Trim();
					friendlyPath = Regex.Replace(friendlyPath, @"(\p{Ll})(\p{Lu})", "$1 $2");
					friendlyPath = Regex.Replace(friendlyPath, @"\b[a-z]", c => c.Value.ToUpper());
					if (!string.IsNullOrEmpty(friendlyPath))
					{
						return friendlyPath;
					}
				}
			}
			return displayName;
		}

		public static string FriendlyType(this SerializedProperty property)
		{
			return property.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
		}

		public static string GetFloatStringValue(this SerializedProperty property)
		{
			// We can't use SerializedProperty.numericType until 2022.1 so we can roll our own by checking type.
			switch (property.type)
			{
				case UnityConstants.Type.Float:
					return property.floatValue.ToString();

				case UnityConstants.Type.Double:
					return property.doubleValue.ToString();

				default:
					return property.floatValue.ToString();
			}
		}

		public static string GetIntStringValue(this SerializedProperty property)
		{
			switch (property.type)
			{
				case UnityConstants.Type.Int:
					return property.intValue.ToString();

				case UnityConstants.Type.Long:
					return property.longValue.ToString();

				case UnityConstants.Type.UInt:
#if UNITY_2022_1_OR_NEWER
					return property.uintValue.ToString();
#else
					return property.intValue.ToString();
#endif
				case UnityConstants.Type.ULong:
#if UNITY_2022_1_OR_NEWER
					return property.ulongValue.ToString();
#else
					return property.longValue.ToString();
#endif
				default:
					return property.intValue.ToString();
			}
		}

		public static bool TrySetFloatValue(this SerializedProperty property, string value)
		{
			switch (property.type)
			{
				case UnityConstants.Type.Float:
					if (float.TryParse(value, out float floatValue))
					{
						property.floatValue = floatValue;
						return true;
					}
					else
					{
						return false;
					}

				case UnityConstants.Type.Double:
					if (double.TryParse(value, out double doubleValue))
					{
						property.doubleValue = doubleValue;
						return true;
					}
					else
					{
						return false;
					}

				default:
					if (float.TryParse(value, out float defaultValue))
					{
						property.floatValue = defaultValue;
						return true;
					}
					else
					{
						return false;
					}
			}
		}

		public static bool TrySetIntValue(this SerializedProperty property, string value)
		{
			switch (property.type)
			{
				case UnityConstants.Type.Int:
					if (int.TryParse(value, out int intValue))
					{
						property.intValue = intValue;
						return true;
					}
					else
					{
						return false;
					}

				case UnityConstants.Type.Long:
					if (long.TryParse(value, out long longValue))
					{
						property.longValue = longValue;
						return true;
					}
					else
					{
						return false;
					}

#if UNITY_2022_1_OR_NEWER
				case UnityConstants.Type.UInt:
					if (uint.TryParse(value, out uint uintValue))
					{
						property.uintValue = uintValue;
						return true;
					}
					else
					{
						return false;
					}

				case UnityConstants.Type.ULong:
					if (ulong.TryParse(value, out ulong ulongValue))
					{
						property.ulongValue = ulongValue;
						return true;
					}
					else
					{
						return false;
					}
#endif
				default:
					if (int.TryParse(value, out int defaultValue))
					{
						property.intValue = defaultValue;
						return true;
					}
					else
					{
						return false;
					}
			}
		}

		public static Gradient GetGradientValue(this SerializedProperty property)
		{
#if UNITY_2022_1_OR_NEWER
			return property.gradientValue;
#else
			var propertyInfo = property.GetGradientPropertyInfo();
			return propertyInfo?.GetValue(property, null) as Gradient;
#endif
		}

		public static void SetGradientValue(this SerializedProperty property, Gradient newValue)
		{
#if UNITY_2022_1_OR_NEWER
			property.gradientValue = newValue;
#else
			var propertyInfo = property.GetGradientPropertyInfo();
			propertyInfo?.SetValue(property, newValue);
#endif
		}

#if !UNITY_2022_1_OR_NEWER
		private static System.Reflection.PropertyInfo GetGradientPropertyInfo(this SerializedProperty property)
		{
			var propertyName = "gradientValue";
			var bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
			System.Reflection.PropertyInfo propertyInfo = typeof(SerializedProperty).GetProperty(propertyName, bindingFlags);
			if (propertyInfo != null)
			{
				return propertyInfo;
			}
			else
			{
				Debug.LogWarning($"Unable to find property '{propertyName}' for {nameof(SerializedProperty)} at {property.propertyPath}.");
				return null;
			}
		}
#endif

		public static bool IsArraySizeOrElement(this SerializedProperty property)
		{
			return property.propertyType == SerializedPropertyType.ArraySize || property.IsArrayElement();
		}

		public static bool IsArrayElement(this SerializedProperty property)
		{
			return IsArrayElement(property.propertyPath);
		}

		public static bool IsArrayElement(string propertyPath)
		{
			return propertyPath.Contains('[');
		}

		public static bool IsInputFieldProperty(this SerializedProperty property, bool isScriptableObject)
		{
			var propertyType = property.GetSheetsPropertyType(isScriptableObject);
			return s_InputFieldPropertyTypes.Contains(propertyType);
		}

		public static bool IsPropertyVisible(this SerializedProperty property, bool showArrays, bool showReadOnly, out bool isReadOnly)
		{
			var isArraySizeOrElement = property.IsArraySizeOrElement();
			isReadOnly = property.IsReadOnly();
			return (showArrays || !isArraySizeOrElement) && (showReadOnly || !isReadOnly) && !property.hasVisibleChildren
				&& property.propertyType != SerializedPropertyType.Generic && property.propertyType != SerializedPropertyType.ManagedReference;
		}

		public static bool IsReadOnly(this SerializedProperty property)
		{
			return s_ReadOnlyUnityFields.Contains(property.name) || !property.editable;
		}

		public static bool IsTypeDouble(this SerializedProperty property)
		{
			return property.type == UnityConstants.Type.Double;
		}

		public static bool IsTypeFloat(this SerializedProperty property)
		{
			return property.type == UnityConstants.Type.Float;
		}

		public static bool IsTypeInt(this SerializedProperty property)
		{
			return property.type == UnityConstants.Type.Int;
		}

		public static bool IsTypeLong(this SerializedProperty property)
		{
			return property.type == UnityConstants.Type.Long;
		}

		public static bool IsTypeUInt(this SerializedProperty property)
		{
			return property.type == UnityConstants.Type.UInt;
		}

		public static bool IsTypeULong(this SerializedProperty property)
		{
			return property.type == UnityConstants.Type.ULong;
		}

		public static bool TryGetEnumType(this SerializedProperty property, Object rootObject, out Type enumType)
		{
			var enumPropertyPath = property.propertyPath;
			enumType = ReflectionUtility.GetNestedFieldType(rootObject.GetType(), enumPropertyPath);
			if (enumType != null && enumType.IsEnum)
			{
				return true;
			}
			else
			{
				Debug.LogWarning($"Property on {nameof(Object)} {rootObject.name} at path {enumPropertyPath} is not a valid type of enum.");
				return false;
			}
		}
	}
}
