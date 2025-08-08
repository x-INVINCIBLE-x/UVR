using LunaWolfStudiosEditor.ScriptableSheets.Comparables;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public static class MultiColumnHeaderUtility
	{
		/// <summary>
		/// Returns a list of sorted objects based on the sorting properties of the multi column header.
		/// </summary>
		public static List<TObject> GetSorted<TObject>(this MultiColumnHeader multiColumnHeader, TObject[] unsorted) where TObject : Object
		{
			var sortedColumnIndex = multiColumnHeader.sortedColumnIndex;
			if (sortedColumnIndex >= 0 && unsorted.Length > 1)
			{
				var column = multiColumnHeader.GetColumn(sortedColumnIndex);
				var isAscending = column.sortedAscending;
				if (column is PropertyColumn propertyColumn)
				{
					var propertyPath = propertyColumn.property.propertyPath;
					if (!string.IsNullOrEmpty(propertyPath))
					{
						// Use Alphanumeric sorting algorithm for strings.
						if (propertyColumn.property.propertyType == SerializedPropertyType.String)
						{
							var alphanumComparer = new AlphanumComparer();
							var ordered = isAscending ? unsorted.OrderBy(u => ComparableUtility.GetPropertyComparable(u, propertyPath) as string, alphanumComparer) : unsorted.OrderByDescending(u => ComparableUtility.GetPropertyComparable(u, propertyPath) as string, alphanumComparer);
							return ordered.ToList();
						}
						else
						{
							var ordered = isAscending ? unsorted.OrderBy(u => ComparableUtility.GetPropertyComparable(u, propertyPath)) : unsorted.OrderByDescending(u => ComparableUtility.GetPropertyComparable(u, propertyPath));
							return ordered.ToList();
						}
					}
					else
					{
						Debug.LogWarning($"Cannot sort column {sortedColumnIndex} property path is null or empty.");
					}
				}
				else if (column.headerContent.tooltip == ColumnUtility.AssetPathTooltip)
				{
					var ordered = isAscending ? unsorted.OrderBy(u => ComparableUtility.GetAssetPathComparable(u)) : unsorted.OrderByDescending(u => ComparableUtility.GetAssetPathComparable(u));
					return ordered.ToList();
				}
				else if (column.headerContent.tooltip == ColumnUtility.GuidColumnTooltip)
				{
					var ordered = isAscending ? unsorted.OrderBy(u => ComparableUtility.GetGUIDComparable(u)) : unsorted.OrderByDescending(u => ComparableUtility.GetGUIDComparable(u));
					return ordered.ToList();
				}
				else
				{
					Debug.LogWarning($"Cannot sort column {sortedColumnIndex} it is not a valid {nameof(PropertyColumn)}.");
				}
			}
			return unsorted.ToList();
		}

		/// <summary>
		/// Resizes the MultiColumnHeader to the min width of all its columns.
		/// </summary>
		public static void ResizeToMinWidth(this MultiColumnHeader multiColumnHeader)
		{
			foreach (var column in multiColumnHeader.state.columns)
			{
				column.width = column.minWidth;
			}
		}

		/// <summary>
		/// Resizes the MultiColumnHeader to the fit the header content.
		/// </summary>
		public static void ResizeToHeaderWidth(this MultiColumnHeader multiColumnHeader, int padding = 0)
		{
			foreach (var column in multiColumnHeader.state.columns)
			{
				var headerWidth = EditorStyles.label.CalcSize(column.headerContent).x + padding;
				column.width = Mathf.Clamp(headerWidth, column.minWidth, column.maxWidth);
			}
		}
	}
}
