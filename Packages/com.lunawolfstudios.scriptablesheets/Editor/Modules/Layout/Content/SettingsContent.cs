using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public class SettingsContent : Content
	{
		public static class Button
		{
			public static readonly GUIContent EditScanPath = GetIconContent(EditorIcon.EditPath, $"Specify the folder where Objects are scanned.");
			public static readonly GUIContent OpenFile = new GUIContent("Open File", "Opens the settings file directly with your preferred text editor.");
			public static readonly GUIContent OpenFolder = new GUIContent("Open Folder", "Opens the UserSettings folder for this project.");
			public static readonly GUIContent OpenWindow = new GUIContent("Open Window", "Opens a separate settings window.");
			public static readonly GUIContent ResetDefaults = new GUIContent("Reset Defaults", "Resets all settings to default values.\n\nYou cannot undo this action.");
			public static readonly GUIContent SaveChangesToDisk = new GUIContent("Save Changes to Disk", "Flushes setting changes to disk.");
		}

		public static class DigitField
		{
			public static readonly GUIContent ArraySize = new GUIContent("Size", "Specify how many columns to display for arrays and other collections.");
			public static readonly GUIContent HighlightAlpha = new GUIContent("Highlight Alpha", "The highlight alpha for selected rows and columns.");
			public static readonly GUIContent IndexPadding = new GUIContent("Index Padding", "Ensures the index value has a minimum number of digits by adding leading zeros as needed when using <b>{i}</b>.\n\nFor example setting this to 3 would ensure an index value with 3 digits like 001.");
			public static readonly GUIContent MaxVisibleCells = new GUIContent("Max Visible Cells", "Total number of cells that can be visible at a time. Capped for performance.");
			public static readonly GUIContent RowsPerPage = new GUIContent("Rows Per Page", "Max rows to display per page. Capped for performance.");
			public static readonly GUIContent StartingIndex = new GUIContent("Starting Index", "Starting index when using <b>{i}</b> for the index value.");
			public static readonly GUIContent VisibleColumnLimit = new GUIContent("Visible Column Limit", "Max columns to display at a time. Capped for performance.");
		}

		public static class Dropdown
		{
			public static readonly GUIContent DefaultSheetAssets = new GUIContent("Default Sheet Assets", "The default sheet asset types to display.");
			public static readonly GUIContent HeaderFormat = new GUIContent("Header Format", "The text format for the table header row.\n\n<b>Default</b> uses Unity's default display names.\n\n<b>Friendly</b> uses the display name and property path to generate quasi-identifiers that are easy-to-read.\n\n<b>Advanced</b> uses the full property path as-is.");
			public static readonly GUIContent JsonSerializationFormat = new GUIContent("Json Format", "<b>Flat</b> serializes the table elements as property paths and string values. Recommended for interchangeability between flat file formats.\n\n<b>Hierarchy</b> serializes the table elements using the Object type and utilizes a layered structure.");
			public static readonly GUIContent ScanOption = new GUIContent("Scan Option", "<b>Default</b> scans for types based on existing instances of the selected Object type.\n\n<b>Assembly</b> scans all assemblies for serializable ScriptableObject types.");
			public static readonly GUIContent WrapOption = new GUIContent("Wrap Option", "Controls how cell values and headers are wrapped when transferring flat file data.");
		}

		public static class Label
		{
			public static readonly GUIContent Scanning = new GUIContent("Scanning", "Settings for scanning Objects.");
			public static readonly GUIContent Searching = new GUIContent("Searching", "Settings for searching Objects.");
			public static readonly GUIContent TableNav = new GUIContent("Table Navigation", "Settings for navigating the table view.");
		}

		public static class Foldouts
		{
			public static readonly GUIContent DataTransfer = new GUIContent("Data Transfer", "Settings for how to handle importing and exporting data.");
			public static readonly GUIContent ObjectManagement = new GUIContent("Object Management", "Settings for selecting, creating, and filtering Objects.");
			public static readonly GUIContent UserInterface = new GUIContent("User Interface", "Settings for the user interface.");
			public static readonly GUIContent Workload = new GUIContent("Workload", "Settings that affect computational performance. Modify with caution to optimize efficiency and responsiveness.");
		}

		public static class TextField
		{
			public static readonly GUIContent NewObjectName = new GUIContent("New Object Name", "The name for newly created Objects. Defaults to the type name if left empty.");
			public static readonly GUIContent NewObjectPrefix = new GUIContent("New Object Prefix", "Prefix for newly created Objects.");
			public static readonly GUIContent NewObjectSuffix = new GUIContent("New Object Suffix", "Suffix for newly created Objects.");
			public static readonly GUIContent ScanPath = new GUIContent("Scan Path", "The folder path to scan for Object instances.");
			public static readonly GUIContent SmartPasteRowDelimiter = new GUIContent("Row Delim", "Delimiter to use when splitting rows.");
			public static readonly GUIContent SmartPasteColumnDelimiter = new GUIContent("Column Delim", "Delimiter to use when splitting columns.");
		}

		public static class Toggle
		{
			public static readonly GUIContent AutoPin = new GUIContent("Auto Pin", "Auto pin Object types to the toolbar as they are selected.");
			public static readonly GUIContent AutoSave = new GUIContent("Auto Save", "Auto saves your changes. Recommended to disable for performance.");
			public static readonly GUIContent AutoScan = new GUIContent("Auto Scan", "Auto scans the project for newly imported Objects. Recommended to disable for performance.");
			public static readonly GUIContent AutoScroll = new GUIContent("Auto Scroll", "Auto updates the scroll view when scrolling with keyboard arrows.");
			public static readonly GUIContent AutoSelect = new GUIContent("Auto Select", "Auto select the focused Object in the inspector.");
			public static readonly GUIContent AutoUpdate = new GUIContent("Auto Update", "Auto updates values as they are changed in the Inspector window.\n\nDisabling this can improve performance, but it may cause values to appear out of sync when making changes in the Inspector window.");
			public static readonly GUIContent CaseSensitive = new GUIContent("Case Sensitive", "Search for Objects using exact letter casing.");
			public static readonly GUIContent ConfirmDelete = new GUIContent("Confirm Delete", "Display a warning message before deleting Objects.");
			public static readonly GUIContent Debug = new GUIContent("Debug", "Display debug log messages in the console.");
			public static readonly GUIContent Headers = new GUIContent("Headers", "Include header names when transferring flat file data.");
			public static readonly GUIContent HighlightSelectedRow = new GUIContent("Highlight Row", "Highlight the selected row.");
			public static readonly GUIContent HighlightSelectedColumn = new GUIContent("Highlight Column", "Highlight the selected column.");
			public static readonly GUIContent LockNames = new GUIContent("Lock Names", "Prevents directly editing the Object name field in the table view. Does NOT apply when pasting content or importing files.");
			public static readonly GUIContent RemoveEmptyRows = new GUIContent("Remove Empty Rows", "Remove empty rows when parsing flat file data.");
			public static readonly GUIContent UseStringEnums = new GUIContent("Use String Enums", "Serialize enum values as their string names. For flat files and flat JSON only.");
			public static readonly GUIContent IgnoreCase = new GUIContent("Ignore Case", "Ignore case when deserializing enum values from their string names.");
			public static readonly GUIContent OverrideArraySize = new GUIContent("Override Size", "Enable to override the number of columns displayed for each array.");
			public static readonly GUIContent ShowRowIndex = new GUIContent("Show Row Index", "Display the row index next to each row.");
			public static readonly GUIContent ShowColumnIndex = new GUIContent("Show Column Index", "Display the column index next to each column.");
			public static readonly GUIContent ShowChildren = new GUIContent("Show Children", "Display child Object fields. This includes deeply nested child Objects.");
			public static readonly GUIContent ShowArrays = new GUIContent("Show Arrays", "Display the elements of arrays and other collections as individual columns.");
			public static readonly GUIContent ShowAssetPath = new GUIContent("Show Asset Path", "Display the asset path for each Object.");
			public static readonly GUIContent ShowGuid = new GUIContent("Show GUID", "Display each Objects GUID.");
			public static readonly GUIContent ShowReadOnly = new GUIContent("Show Read-only", "Display read-only fields for each Object.");
			public static readonly GUIContent SmartPaste = new GUIContent("Smart Paste", "Enhance pasting by distributing flat file data across table cells using the specified delimiters.");
			public static readonly GUIContent StartsWith = new GUIContent("Starts With", "Search for Objects that start with the search text entered.");
			public static readonly GUIContent UseExpansion = new GUIContent("Use Expansion", "Use variable expansion when naming newly created Objects.\n\n<b>{i}</b> becomes the index value.\n\n<b>{t}</b> becomes the type value.");
			public static readonly GUIContent Virtualization = new GUIContent("Virtualization", "Improves performance by rendering only the cells within the visible scroll area.");
			public static readonly GUIContent PageRowsOnly = new GUIContent("Page Rows Only", "Restrict data transfer to the rows on the current page only.");
			public static readonly GUIContent VisibleColumnsOnly = new GUIContent("Visible Columns Only", "Restrict data transfer to visible columns only.");
		}

		public static class Window
		{
			public static readonly GUIContent Title = GetIconContent(EditorIcon.Settings, "Scriptable Sheets Settings", string.Empty);
		}
	}
}
