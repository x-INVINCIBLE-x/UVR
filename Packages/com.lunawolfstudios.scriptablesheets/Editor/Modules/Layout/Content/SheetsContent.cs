using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public class SheetsContent : Content
	{
		public class Button
		{
			public static readonly GUIContent EditNewAssetPath = GetIconContent(EditorIcon.EditPath, $"Specify the folder where new assets are created");
			public static readonly GUIContent Rescan = GetIconContent(EditorIcon.Refresh, $"Rescan");
			public static readonly GUIContent Pin = GetIconContent(EditorIcon.ToggleOff, $"Pin to toolbar");
			public static readonly GUIContent Unpin = GetIconContent(EditorIcon.ToggleOn, $"Unpin from toolbar");
			public static readonly GUIContent UnpinAll = GetIconContent(EditorIcon.ToggleGroup, $"Unpin all from toolbar");
			public static readonly GUIContent ShowColumns = GetIconContent(EditorIcon.VisibilityOn, $"Show all Columns");
			public static readonly GUIContent HideColumns = GetIconContent(EditorIcon.VisibilityOff, $"Hide all Columns");
			public static readonly GUIContent Stretch = GetIconContent(EditorIcon.Stretch, $"Stretch to fit view area");
			public static readonly GUIContent Compact = GetIconContent(EditorIcon.Compact, $"Compact to minimum width");
			public static readonly GUIContent Expand = GetIconContent(EditorIcon.Expand, $"Expand to fit headers");
			public static readonly GUIContent ImportFile = GetIconContent(EditorIcon.Open, $"Import File");
			public static readonly GUIContent CopyToClipboard = GetIconContent(EditorIcon.Copy, $"Copy to Clipboard");
			public static readonly GUIContent CopyRowToClipboard = GetIconContent(EditorIcon.CopyX, $"Copy selected row to Clipboard");
			public static readonly GUIContent CopyColumnToClipboard = GetIconContent(EditorIcon.CopyY, $"Copy selected column to Clipboard");
			public static readonly GUIContent SmartPaste = GetIconContent(EditorIcon.Paste, $"Smart Paste starting from the selected cell");
			public static readonly GUIContent SaveToDisk = GetIconContent(EditorIcon.Save, $"Save to Disk");
			public static readonly GUIContent NewPastePad = GetIconContent(EditorIcon.Edit, $"New Paste Pad Window");
			public static readonly GUIContent FirstPage = GetIconContent(EditorIcon.FirstPage, "First Page");
			public static readonly GUIContent PreviousPage = GetIconContent(EditorIcon.Previous, "Previous Page");
			public static readonly GUIContent NextPage = GetIconContent(EditorIcon.Next, "Next Page");
			public static readonly GUIContent LastPage = GetIconContent(EditorIcon.LastPage, "Last Page");
			public static readonly GUIContent Select = GetIconContent(EditorIcon.Select, "Select Object");
			public static readonly GUIContent Delete = GetIconContent(EditorIcon.Delete, "Delete Object");

			private static readonly GUIContent s_Create = EditorGUIUtility.IconContent(EditorIcon.Create);

			public static GUIContent GetCreateContent(int amount)
			{
				s_Create.tooltip = $"Create ({amount}) new asset(s) at the specified path";
				return s_Create;
			}
		}

		public class Label
		{
			private static readonly GUIContent s_AssetName = new GUIContent();
			private static readonly GUIContent s_ColumnInfo = new GUIContent();
			private static readonly GUIContent s_ObjectType = new GUIContent();
			private static readonly GUIContent s_PageInfo = new GUIContent();
			private static readonly GUIContent s_RowIndex = new GUIContent();

			public static GUIContent GetAssetNameContent(string assetName)
			{
				s_AssetName.text = assetName;
				return s_AssetName;
			}

			public static GUIContent GetColumnContent(int visibleColumns, int columnLimit, int totalColumns)
			{
				s_ColumnInfo.text = $"({visibleColumns}/{Mathf.Min(columnLimit, totalColumns)})";
				s_ColumnInfo.tooltip = $"Visible columns: {visibleColumns}\nVisible column limit: {columnLimit}\nTotal columns: {totalColumns}";
				return s_ColumnInfo;
			}

			public static GUIContent GetObjectTypeContent(string objectTypeName, string tooltip)
			{
				s_ObjectType.text = objectTypeName;
				s_ObjectType.tooltip = tooltip;
				return s_ObjectType;
			}

			public static GUIContent GetPageContent(int currentPage, int totalPages, int totalElements)
			{
				s_PageInfo.text = $"({currentPage}/{totalPages})";
				s_PageInfo.tooltip = $"Total rows: {totalElements}";
				return s_PageInfo;
			}

			public static GUIContent GetRowIndex(int index)
			{
				s_RowIndex.text = index.ToString();
				return s_RowIndex;
			}
		}

		public class Window
		{
			public static readonly GUIContent Title = GetIconContent(EditorIcon.ScriptableObject, "Scriptable Sheets", string.Empty);

			public class ContextMenu
			{
				public static readonly GUIContent NewSheet = new GUIContent("New Sheets Window");
				public static readonly GUIContent NewPastePad = new GUIContent("New Paste Pad Window");
				public static readonly GUIContent OpenSettings = new GUIContent("Open Settings Window");
				public static readonly GUIContent Copy = new GUIContent("Copy");
				public static readonly GUIContent CopyJson = new GUIContent("Copy Json");
			}
		}
	}
}
