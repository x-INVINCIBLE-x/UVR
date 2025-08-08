using LunaWolfStudiosEditor.ScriptableSheets.Serializables;
using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using Newtonsoft.Json;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public struct SerializedTableProperty : ITableProperty
	{
		public const string NullObjectValue = "null";
		private const char AssetDelimiter = '&';

		private readonly Object m_RootObject;
		public Object RootObject => m_RootObject;

		private readonly string m_PropertyPath;
		public string PropertyPath => m_PropertyPath;

		private readonly string m_ControlName;
		public string ControlName => m_ControlName;

		public SerializedTableProperty(Object rootObject, string propertyPath, string controlName)
		{
			m_RootObject = rootObject;
			m_PropertyPath = propertyPath;
			m_ControlName = controlName;
		}

		public SerializedObject GetSerializedObject()
		{
			return new SerializedObject(m_RootObject);
		}

		public SerializedProperty GetSerializedProperty()
		{
			return GetSerializedProperty(GetSerializedObject());
		}

		public SerializedProperty GetSerializedProperty(SerializedObject serializedObject)
		{
			return serializedObject.FindProperty(PropertyPath);
		}

		public string GetProperty(FlatFileFormatSettings formatSettings)
		{
			var property = GetSerializedProperty();
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
					return property.GetIntStringValue();

				case SerializedPropertyType.Boolean:
					return property.boolValue.ToString();

				case SerializedPropertyType.Float:
					return property.GetFloatStringValue();

				case SerializedPropertyType.String:
					return property.stringValue;

				case SerializedPropertyType.Color:
					return ColorUtility.ToHtmlStringRGBA(property.colorValue);

				case SerializedPropertyType.LayerMask:
					return property.intValue.ToString();

				case SerializedPropertyType.Enum:
					if (formatSettings.UseStringEnums && property.TryGetEnumType(m_RootObject, out Type enumType))
					{
						if (!enumType.HasFlagsAttribute())
						{
							var enumName = Enum.GetName(enumType, property.intValue);
							if (!string.IsNullOrEmpty(enumName))
							{
								return enumName;
							}
						}
					}
					return property.intValue.ToString();

				case SerializedPropertyType.ObjectReference:
					var objValue = property.objectReferenceValue;
					if (objValue != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(property.objectReferenceValue, out string guid, out long localId))
					{
						return $"{guid}{AssetDelimiter}{objValue.name}";
					}
					else
					{
						return NullObjectValue;
					}

				case SerializedPropertyType.ArraySize:
					return property.intValue.ToString();

				case SerializedPropertyType.Character:
					return ((char) property.intValue).ToString();

				case SerializedPropertyType.AnimationCurve:
					return JsonUtility.ToJson(new SerializableAnimationCurve(property));

				case SerializedPropertyType.Gradient:
					return JsonUtility.ToJson(new SerializableGradient(property));

				default:
					Debug.LogWarning($"Unsupported property type {property.propertyType} for property at path {PropertyPath}.");
					return string.Empty;
			}
		}

		public void SetProperty(string value, FlatFileFormatSettings formatSettings)
		{
			var serializedObject = GetSerializedObject();
			var property = GetSerializedProperty(serializedObject);
			if (property != null)
			{
				if (!property.editable || value == null)
				{
					// Cannot change read-only properties. To set properties null use an empty string or the null object string.
					return;
				}
				var propertyType = property.propertyType;
				switch (property.propertyType)
				{
					case SerializedPropertyType.Integer:
					case SerializedPropertyType.ArraySize:
						if (!property.TrySetIntValue(value))
						{
							LogParseWarning(value, propertyType, $"Not a valid int for numeric type '{property.type}'.");
						}
						break;

					case SerializedPropertyType.LayerMask:
						value = value.UnwrapLayerMask();
						if (int.TryParse(value, out int intValue))
						{
							property.intValue = intValue;
						}
						else
						{
							LogParseWarning(value, propertyType, "Not a valid int.");
						}
						break;

					case SerializedPropertyType.Enum:
						if (formatSettings.UseStringEnums)
						{
							if (property.TryGetEnumType(m_RootObject, out Type enumType))
							{
								if (!enumType.HasFlagsAttribute() || value.Contains(UnityConstants.EnumPrefix))
								{
									try
									{
										// Unity prefixes single enum values with 'Enum:' when copying from the Inspector.
										value = value.Replace(UnityConstants.EnumPrefix, string.Empty);
										// Use Enum.Parse instead of TryParse to support older versions of C#.
										var enumObject = Enum.Parse(enumType, value, formatSettings.IgnoreCase);
										property.intValue = (int) enumObject;
									}
									catch (Exception ex)
									{
										LogParseWarning(value, propertyType, ex.Message);
									}
									break;
								}
								else
								{
									value = value.UnwrapLayerMask();
								}
							}
							else
							{
								LogParseWarning(value, propertyType, "Did not find a valid enum type.");
							}
						}
						if (int.TryParse(value, out int enumValue))
						{
							property.intValue = enumValue;
						}
						else
						{
							LogParseWarning(value, propertyType, "Not a valid int.");
						}
						break;

					case SerializedPropertyType.Boolean:
						if (bool.TryParse(value, out bool boolValue))
						{
							property.boolValue = boolValue;
						}
						else if (int.TryParse(value, out int boolIntValue))
						{
							property.boolValue = boolIntValue > 0;
						}
						else
						{
							LogParseWarning(value, propertyType, "Not a valid bool.");
						}
						break;

					case SerializedPropertyType.Float:
						if (!property.TrySetFloatValue(value))
						{
							LogParseWarning(value, propertyType, "Not a valid float.");
						}
						break;

					case SerializedPropertyType.String:
						if (property.name == UnityConstants.Field.Name)
						{
							if (value != m_RootObject.name)
							{
								var assetPath = AssetDatabase.GetAssetPath(m_RootObject);
								var assetRenameResponse = AssetDatabase.RenameAsset(assetPath, value);
								if (!string.IsNullOrEmpty(assetRenameResponse))
								{
									LogParseWarning(value, propertyType, assetRenameResponse);
								}
							}
						}
						else
						{
							property.stringValue = value;
						}
						break;

					case SerializedPropertyType.Color:
						if (value.Length > 0 && value[0] != '#')
						{
							value = "#" + value;
						}
						if (ColorUtility.TryParseHtmlString(value, out Color parsedColor))
						{
							property.colorValue = parsedColor;
						}
						else
						{
							LogParseWarning(value, propertyType, "Not a valid color hex code.");
						}
						break;

					case SerializedPropertyType.ObjectReference:
						if (value.Length > 0 && value != NullObjectValue)
						{
							// Handle case where user might copy from the inspector.
							UnityObjectWrapper objectWrapper = null;
							if (value.Contains(UnityConstants.ObjectWrapperJSON))
							{
								value = value.Replace(UnityConstants.ObjectWrapperJSON, string.Empty);
								try
								{
									objectWrapper = JsonConvert.DeserializeObject<UnityObjectWrapper>(value);
								}
								catch (Exception ex)
								{
									LogParseWarning(value, propertyType, ex.Message);
									break;
								}
								value = objectWrapper.Guid;
							}

							var parts = value.Split(AssetDelimiter);
							var assetPath = AssetDatabase.GUIDToAssetPath(parts[0]);
							if (!string.IsNullOrEmpty(assetPath))
							{
								// GUIDs for Unity built in extra assets are not unique and must be found with a manual search by filename or localId.
								if (assetPath == UnityConstants.Path.BuiltInExtra)
								{
									Object foundAsset = null;
									var friendlyType = property.FriendlyType();
									if (objectWrapper != null)
									{
										foreach (var asset in SerializedPropertyUtility.UnityBuiltInAssets)
										{
											if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long localId) && objectWrapper.LocalId == localId)
											{
												foundAsset = asset;
												break;
											}
										}
										if (foundAsset != null)
										{
											property.objectReferenceValue = foundAsset;
										}
										else
										{
											LogParseWarning(value, propertyType, $"Unable to find asset at path '{assetPath}' with {objectWrapper}.");
										}
									}
									else if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
									{
										foreach (var asset in SerializedPropertyUtility.UnityBuiltInAssets)
										{
											if (asset.name == parts[1] && friendlyType == asset.GetType().Name)
											{
												foundAsset = asset;
												break;
											}
										}
										if (foundAsset != null)
										{
											property.objectReferenceValue = foundAsset;
										}
										else
										{
											LogParseWarning(value, propertyType, $"Unable to find asset '{parts[1].GetEscapedText()}' at path '{assetPath}'.");
										}
									}
									else
									{
										LogParseWarning(value, propertyType, $"Unable to find asset with GUID '{parts[0]}' at path '{assetPath}'. Please include the asset name.");
									}
								}
								else
								{
									// Certain Object types like Sprite require the Type to be explicit when loading from an asset path.
									var objectType = ReflectionUtility.GetNestedFieldType(m_RootObject.GetType(), property.propertyPath);
									if (objectType != null)
									{
										property.objectReferenceValue = AssetDatabase.LoadAssetAtPath(assetPath, objectType);
									}
									else
									{
										property.objectReferenceValue = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
									}
								}
							}
							else
							{
								LogParseWarning(value, propertyType, $"Unable to find asset path for GUID '{parts[0].GetEscapedText()}'.");
							}
						}
						else
						{
							property.objectReferenceValue = null;
						}
						break;

					case SerializedPropertyType.Character:
						if (value.Length > 0)
						{
							property.intValue = value[0];
						}
						else
						{
							LogParseWarning(value, propertyType, "Char cannot be empty.");
						}
						break;

					case SerializedPropertyType.AnimationCurve:
						// Handle case where user might copy from the inspector.
						value = value.Replace("UnityEditor.AnimationCurveWrapperJSON:{\"c", "{\"m_AnimationC");
						try
						{
							var animationCurve = JsonUtility.FromJson<SerializableAnimationCurve>(value).AnimationCurve;
							property.animationCurveValue = animationCurve;
						}
						catch (Exception ex)
						{
							LogParseWarning(value, propertyType, ex.Message);
						}
						break;

					case SerializedPropertyType.Gradient:
						// Handle case where user might copy from the inspector.
						value = value.Replace("UnityEditor.GradientWrapperJSON:{\"g", "{\"m_G");
						try
						{
							var gradient = JsonUtility.FromJson<SerializableGradient>(value).Gradient;
							property.SetGradientValue(gradient);
						}
						catch (Exception ex)
						{
							LogParseWarning(value, propertyType, ex.Message);
						}
						break;

					default:
						LogParseWarning(value, propertyType, "Unsupported Property Type.");
						break;
				}
				serializedObject.ApplyModifiedProperties();
			}
			else
			{
				if (SerializedPropertyUtility.IsArrayElement(m_PropertyPath))
				{
					Debug.LogWarning($"Unable to find array element at path {m_PropertyPath} on {nameof(Object)} {m_RootObject.name}. Did you update the size of the array?");
				}
				else
				{
					Debug.LogError($"Unable to find property at path {m_PropertyPath} on {nameof(Object)} {m_RootObject.name}");
				}
			}
		}

		private void LogParseWarning(string value, SerializedPropertyType propertyType, string message)
		{
			Debug.LogWarning($"Cannot parse '{value.GetEscapedText()}' to type {propertyType} for property at path {m_PropertyPath} on {nameof(Object)} {m_RootObject.name}.\n{message}");
		}

		public bool IsInputFieldProperty(bool isScriptableObject)
		{
			var property = GetSerializedProperty();
			return property.IsInputFieldProperty(isScriptableObject);
		}

		// Draw our own selected cell border for properties Unity doesn't automatically.
		public bool NeedsSelectionBorder(bool lockNames = false)
		{
			var property = GetSerializedProperty();
			// Name property is readonly on certain fields like Enum TextAssets.
			var isNameProperty = property.propertyPath == UnityConstants.Field.Name;
			return (!isNameProperty && property.IsReadOnly() || (isNameProperty && lockNames)) || property.propertyType == SerializedPropertyType.AnimationCurve || property.propertyType == SerializedPropertyType.Gradient;
		}
	}
}
