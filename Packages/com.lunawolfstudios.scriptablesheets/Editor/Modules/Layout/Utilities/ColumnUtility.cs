using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public static class ColumnUtility
	{
		// Note: Tooltips need to be unique or the table will assume it's the same property.
		public const string ActionsTooltip = "Object Actions";
		public const string AssetPathTooltip = "Object Asset Path";
		public const string GuidColumnTooltip = "Globally Unique Identifier";

		public static MultiColumnHeaderState.Column CreateActionsColumn(string columnName, int width)
		{
			return new MultiColumnHeaderState.Column()
			{
				allowToggleVisibility = false,
				autoResize = true,
				canSort = false,
				headerContent = new GUIContent(columnName, ActionsTooltip),
				headerTextAlignment = TextAlignment.Left,
				minWidth = width,
				maxWidth = width,
				sortingArrowAlignment = TextAlignment.Right,
			};
		}

		public static MultiColumnHeaderState.Column CreateAssetPathColumn(string columnName)
		{
			return new MultiColumnHeaderState.Column()
			{
				allowToggleVisibility = true,
				autoResize = true,
				canSort = true,
				headerContent = new GUIContent(columnName, AssetPathTooltip),
				headerTextAlignment = TextAlignment.Left,
				minWidth = SheetLayout.PropertyWidth,
				maxWidth = int.MaxValue,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Right,
			};
		}

		public static MultiColumnHeaderState.Column CreateGuidColumn(string columnName)
		{
			return new MultiColumnHeaderState.Column()
			{
				allowToggleVisibility = true,
				autoResize = true,
				canSort = true,
				headerContent = new GUIContent(columnName, GuidColumnTooltip),
				headerTextAlignment = TextAlignment.Left,
				minWidth = SheetLayout.GuidPropertyWidth,
				maxWidth = int.MaxValue,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Right,
			};
		}

		public static PropertyColumn CreatePropertyColumn(SerializedProperty serializedProperty, bool isScriptableObject, HeaderFormat headerFormat, string labelPrefix = "")
		{
			string formattedName;
			switch (headerFormat)
			{
				case HeaderFormat.Default:
					formattedName = serializedProperty.displayName;
					break;

				case HeaderFormat.Friendly:
					formattedName = serializedProperty.FriendlyPropertyPath();
					break;

				case HeaderFormat.Advanced:
					formattedName = serializedProperty.propertyPath;
					break;

				default:
					Debug.LogWarning($"Unsupported {nameof(HeaderFormat)} {headerFormat}. Using {nameof(HeaderFormat.Default)} format.");
					formattedName = serializedProperty.displayName;
					break;
			}
			var columnName = $"{labelPrefix}{formattedName}";
			var propertyType = serializedProperty.GetSheetsPropertyType(isScriptableObject);
			var minWidth = SheetLayout.PropertyWidth;
			var propertyPath = serializedProperty.propertyPath;
			switch (propertyType)
			{
				case SerializedPropertyType.Integer:
				case SerializedPropertyType.Boolean:
				case SerializedPropertyType.Float:
				case SerializedPropertyType.Color:
				case SerializedPropertyType.ArraySize:
				case SerializedPropertyType.Character:
					minWidth = SheetLayout.PropertyWidthSmall;
					break;

				default:
					break;
			}
			return new PropertyColumn()
			{
				allowToggleVisibility = true,
				autoResize = true,
				canSort = true,
				headerContent = new GUIContent(columnName, propertyPath),
				headerTextAlignment = TextAlignment.Left,
				minWidth = minWidth,
				maxWidth = int.MaxValue,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Right,
				property = serializedProperty.Copy(),
			};
		}

		public static string GetColumnIndexLabel(bool showIndex, int index)
		{
			return showIndex ? $"{index} " : string.Empty;
		}

		public static int[] GetClampedColumns(this MultiColumnHeaderState.Column[] columns, int maxColumns)
		{
			return Enumerable.Range(0, Mathf.Min(columns.Length, maxColumns)).ToArray();
		}
	}
}
