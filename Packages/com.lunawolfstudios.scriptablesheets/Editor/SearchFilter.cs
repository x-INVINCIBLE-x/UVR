using LunaWolfStudiosEditor.ScriptableSheets.Comparables;
using LunaWolfStudiosEditor.ScriptableSheets.Serializables;
using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public static class SearchFilter
	{
		private const char FilterDelimiter = ':';
		private const string FilterPattern = @"([\w\.\[\]]+)([><=!]=?|<=?)(.+)";

		public static List<Object> GetObjects(string input, List<Object> objs, SearchSettings settings, bool useStringEnums, bool ignoreEnumCasing)
		{
			if (objs.Count > 0 && !string.IsNullOrEmpty(input))
			{
				if (input.Contains(FilterDelimiter))
				{
					var command = input.Split(FilterDelimiter)[0];
					var filter = input.Substring(command.Length + 1);
					if (!string.IsNullOrWhiteSpace(command) && !string.IsNullOrWhiteSpace(filter))
					{
						switch (command)
						{
							case "p":
							case "prop":
							case "property":
								var match = Regex.Match(filter, FilterPattern);
								if (match.Success)
								{
									var propertyPath = match.Groups[1].Value;
									var filterOperation = match.Groups[2].Value;
									var filterValue = match.Groups[3].Value;
									if (!string.IsNullOrEmpty(propertyPath) && !string.IsNullOrEmpty(filterValue) && (filterOperation.Length >= 2 || filterValue != "="))
									{
										var serializedObject = new SerializedObject(objs[0]);
										var property = serializedObject.FindProperty(propertyPath);
										if (property != null)
										{
											// Convert string enum values to their index values before comparing.
											if (useStringEnums && property.propertyType == SerializedPropertyType.Enum && property.TryGetEnumType(objs[0], out Type enumType))
											{
												try
												{
													filterValue = filterValue.Replace(UnityConstants.EnumPrefix, string.Empty);
													var enumObject = Enum.Parse(enumType, filterValue, ignoreEnumCasing);
													filterValue = ((int) enumObject).ToString();
												}
												catch (Exception)
												{
													// Handle mismatched enum names gracefully.
												}
											}
											return objs.Where(obj => MatchesPropertyFilter(obj, propertyPath, filterValue, filterOperation)).ToList();
										}
									}
								}
								break;

							case "g":
							case "guid":
								return objs.Where(obj => MatchesGuidFilter(obj, filter, settings)).ToList();

							case "ap":
							case "path":
							case "assetpath":
								return objs.Where(obj => MatchesPathFilter(obj, filter, settings)).ToList();

							default:
								Debug.LogWarning($"'{command}' is not a valid search filter command.");
								break;
						}
					}
				}
				else
				{
					return objs.Where(obj => obj.name.MatchesSearch(input, settings)).ToList();
				}
			}
			return objs;
		}

		private static bool MatchesPropertyFilter(Object obj, string propertyPath, string filterValue, string filterOperation)
		{
			var propertyValue = ComparableUtility.GetPropertyComparable(obj, propertyPath);
			if (propertyValue == null)
			{
				if (filterOperation == "=" || filterOperation == "==")
				{
					return filterValue == "?";
				}
				else
				{
					return false;
				}
			}
			switch (propertyValue)
			{
				case int propertyIntValue:
					if (int.TryParse(filterValue, out int intValue))
					{
						return MatchesFilterOperation(propertyIntValue, intValue, filterOperation);
					}
					else if (filterValue.Length == 1)
					{
						return MatchesFilterOperation(propertyIntValue, filterValue[0], filterOperation);
					}
					else
					{
						return false;
					}

				case long propertyLongValue:
					if (long.TryParse(filterValue, out long longValue))
					{
						return MatchesFilterOperation(propertyLongValue, longValue, filterOperation);
					}
					else
					{
						return false;
					}

#if UNITY_2022_1_OR_NEWER
				case uint propertyUIntValue:
					if (uint.TryParse(filterValue, out uint uintValue))
					{
						return MatchesFilterOperation(propertyUIntValue, uintValue, filterOperation);
					}
					else
					{
						return false;
					}

				case ulong propertyULongValue:
					if (ulong.TryParse(filterValue, out ulong ulongValue))
					{
						return MatchesFilterOperation(propertyULongValue, ulongValue, filterOperation);
					}
					else
					{
						return false;
					}
#endif
				case bool propertyBoolValue:
					if (bool.TryParse(filterValue, out bool boolValue))
					{
						return MatchesFilterOperation(propertyBoolValue, boolValue, filterOperation);
					}
					else if (int.TryParse(filterValue, out int boolIntValue))
					{
						return MatchesFilterOperation(propertyBoolValue, boolIntValue >= 1, filterOperation);
					}
					else
					{
						return false;
					}

				case float propertyFloatValue:
					if (float.TryParse(filterValue, out float floatValue))
					{
						return MatchesFilterOperation(propertyFloatValue, floatValue, filterOperation);
					}
					else
					{
						return false;
					}

				case double propertyDoubleValue:
					if (double.TryParse(filterValue, out double doubleValue))
					{
						return MatchesFilterOperation(propertyDoubleValue, doubleValue, filterOperation);
					}
					else
					{
						return false;
					}

				case string propertyStringValue:
					return MatchesFilterOperation(propertyStringValue, filterValue, filterOperation);

				case ColorComparable propertyColorValue:
					if (!filterValue.StartsWith("#"))
					{
						filterValue = '#' + filterValue;
					}
					if (ColorUtility.TryParseHtmlString(filterValue, out Color colorValue))
					{
						return MatchesFilterOperation(propertyColorValue, (ColorComparable) colorValue, filterOperation);
					}
					else
					{
						return false;
					}

				case AnimationCurveComparable propertyAnimationCurveValue:
					// Handle case where user might copy from the inspector.
					var animationCurveJson = filterValue.Replace("UnityEditor.AnimationCurveWrapperJSON:{\"c", "{\"m_AnimationC");
					try
					{
						var animationCurveValue = JsonUtility.FromJson<SerializableAnimationCurve>(animationCurveJson).AnimationCurve;
						return MatchesFilterOperation(propertyAnimationCurveValue, (AnimationCurveComparable) animationCurveValue, filterOperation);
					}
					catch (Exception)
					{
						// Handle JSON exceptions gracefully.
					}
					return false;

				case GradientComparable propertyGradientValue:
					// Handle case where user might copy from the inspector.
					var gradientJson = filterValue.Replace("UnityEditor.GradientWrapperJSON:{\"g", "{\"m_G");
					try
					{
						var gradientValue = JsonUtility.FromJson<SerializableGradient>(gradientJson).Gradient;
						return MatchesFilterOperation(propertyGradientValue, (GradientComparable) gradientValue, filterOperation);
					}
					catch (Exception)
					{
						// Handle JSON exceptions gracefully.
					}
					return false;

				default:
					Debug.LogWarning($"Unsupported property type '{propertyValue.GetType()}' for property '{propertyPath}' on Object '{obj.name}'.");
					return false;
			}
		}

		private static bool MatchesGuidFilter(Object obj, string guid, SearchSettings searchSettings)
		{
			var objGuid = ComparableUtility.GetGUIDComparable(obj);
			return objGuid.ToString().MatchesSearch(guid, searchSettings);
		}

		private static bool MatchesPathFilter(Object obj, string path, SearchSettings searchSettings)
		{
			var objAssetPath = ComparableUtility.GetAssetPathComparable(obj);
			return objAssetPath.ToString().MatchesSearch(path, searchSettings);
		}

		private static bool MatchesFilterOperation<T>(T propertyValue, T filterValue, string filterOperation) where T : IComparable
		{
			switch (filterOperation)
			{
				case "=":
				case "==":
					return propertyValue.CompareTo(filterValue) == 0;

				case "!=":
					return propertyValue.CompareTo(filterValue) != 0;

				case ">":
					return propertyValue.CompareTo(filterValue) > 0;

				case "<":
					return propertyValue.CompareTo(filterValue) < 0;

				case ">=":
					return propertyValue.CompareTo(filterValue) >= 0;

				case "<=":
					return propertyValue.CompareTo(filterValue) <= 0;

				default:
					return false;
			}
		}
	}
}
