using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	public static class ReflectionUtility
	{
		/// <summary>
		/// Searches all inherited types of the target type for the specified property path.
		/// Returns the field info for each part of the property path.
		/// </summary>
		public static FieldInfo[] GetNestedFieldInfo(Type rootType, string propertyPath)
		{
			Profiler.BeginSample(nameof(GetNestedFieldInfo));
			var targetType = rootType;
			var fieldNameParts = propertyPath.Replace(UnityConstants.ArrayPropertyPath, "[").Split('.');
			var nestedFieldInfo = new List<FieldInfo>();
			FieldInfo field = null;
			var index = 0;
			var collectionTypes = new Queue<Type>();
			while (targetType != null && index < fieldNameParts.Length)
			{
				var fieldNamePart = fieldNameParts[index];
				if (fieldNamePart.Contains('['))
				{
					// Get the field name before the array indexer.
					var fieldName = fieldNamePart.Substring(0, fieldNamePart.IndexOf('['));
					field = targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}
				else
				{
					field = targetType.GetField(fieldNamePart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}
				if (field != null)
				{
					index++;
					nestedFieldInfo.Add(field);
					if (nestedFieldInfo.Count < fieldNameParts.Length)
					{
						targetType = field.FieldType;
					}
				}
				else
				{
					// Field was not found. If the current type is a collection let's track its respective element or argument type. 
					if (targetType.IsArray)
					{
						collectionTypes.Enqueue(targetType.GetElementType());
					}
					else if (targetType.IsGenericType)
					{
						collectionTypes.Enqueue(targetType.GetGenericArguments()[0]);
					}
					targetType = targetType.BaseType;
					if (targetType == null && collectionTypes.Count > 0)
					{
						// We've reached the end of this target type. Let's check the previous collection types.
						targetType = collectionTypes.Dequeue();
					}
				}
			}
			Profiler.EndSample();
			if (nestedFieldInfo.Count > 0 && nestedFieldInfo.Count == fieldNameParts.Length)
			{
				return nestedFieldInfo.ToArray();
			}
			Debug.LogWarning($"Unable to find field at path '{propertyPath}' for type '{rootType}'.");
			return null;
		}

		public static Type GetNestedFieldType(Type rootType, string propertyPath)
		{
			var nestedFields = GetNestedFieldInfo(rootType, propertyPath);
			if (nestedFields != null)
			{
				var lastFieldType = nestedFields.Last().FieldType;
				if (lastFieldType.IsArray)
				{
					return lastFieldType.GetElementType();
				}
				else if (lastFieldType.IsGenericType)
				{
					return lastFieldType.GetGenericArguments()[0];
				}
				else
				{
					return lastFieldType;
				}
			}
			return null;
		}
	}
}
