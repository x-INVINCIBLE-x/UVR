using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using LunaWolfStudiosEditor.ScriptableSheets.PastePad;
using LunaWolfStudiosEditor.ScriptableSheets.Scanning;
using LunaWolfStudiosEditor.ScriptableSheets.Settings;
using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace LunaWolfStudiosEditor.ScriptableSheets
{
	public class ScriptableSheetsEditorWindow : EditorWindow, IHasCustomMenu
	{
		public static readonly List<ScriptableSheetsEditorWindow> Instances = new List<ScriptableSheetsEditorWindow>();

		private static readonly Dictionary<Type, MonoScript> s_MonoScriptCache = new Dictionary<Type, MonoScript>();

		private const string JsonExtension = "json";
		private const int FirstAndLastPageThreshold = 4;

		private readonly ObjectScanner m_Scanner = new ObjectScanner();
		private readonly Paginator m_Paginator = new Paginator();
		private readonly TableNav<ITableProperty> m_TableNav = new TableNav<ITableProperty>();
		private readonly TableSmartPaste<ITableProperty> m_TableSmartPaste = new TableSmartPaste<ITableProperty>();

		private List<Object> m_SortedObjects = new List<Object>();
		private Type m_SelectedType;
		private Type m_PreviousSelectedType;
		private string m_NewAssetPath;
		private int m_PreviousSelectedPage = 1;

		private Table<ITableProperty> m_PropertyTable;
		private TableAction m_TableAction;
		private int[] m_CachedVisibleColumns;
		private string m_SelectedFilepath;
		private string m_ImportedFileContents;
		private bool m_IsImportJson;

		private SearchField m_SearchField;

		private MultiColumnHeaderState.Column[] m_Columns;
		private MultiColumnHeaderState m_MultiColumnHeaderState;
		private MultiColumnHeader m_MultiColumnHeader;
		private Dictionary<string, int> m_MultiColumnTooltipPaths;
		private bool m_SortingChanged;

		private Rect m_ScrollViewArea;
		private Rect m_TableScrollViewRect;
		private Vector2 m_ScrollPosition;
		private Vector2 m_SheetAssetScrollPosition;
		private Vector2 m_ObjectTypeScrollPosition;

		private ScriptableSheetsSettings m_Settings;
		private bool m_ForceRefreshColumnLayout;
		private bool m_Reinitialized;

		// Cached window session data.
		private SheetAsset m_SelectableSheetAssets;
		private SheetAsset m_SelectedSheetAsset;
		private int m_SelectedTypeIndex;
		private Dictionary<SheetAsset, HashSet<int>> m_PinnedIndexSets;
		private int m_NewAmount;
		private string m_SearchInput;

		[MenuItem("Window/Scriptable Sheets")]
		public static void ShowWindow()
		{
			var window = CreateInstance<ScriptableSheetsEditorWindow>();
			window.titleContent = SheetsContent.Window.Title;
			window.minSize = new Vector2(600, 400);
			window.Show();
		}

		private void OnEnable()
		{
			Instances.Add(this);
			Undo.undoRedoPerformed += OnUndoRedoPerformed;
			m_SearchField = new SearchField();
			m_Settings = ScriptableSheetsSettings.instance;

			var windowSessionState = m_Settings.LoadWindowSessionState(this);
			if (windowSessionState == null)
			{
				m_SelectableSheetAssets = SheetAsset.Default;
				m_SelectedSheetAsset = SheetAsset.ScriptableObject;
				m_SelectedTypeIndex = 0;
				m_PinnedIndexSets = new Dictionary<SheetAsset, HashSet<int>>();
				m_NewAmount = 1;
				m_SearchInput = string.Empty;
				foreach (SheetAsset sheetAsset in Enum.GetValues(typeof(SheetAsset)))
				{
					m_PinnedIndexSets.Add(sheetAsset, new HashSet<int>());
				}
			}
			else
			{
				m_SelectableSheetAssets = windowSessionState.SelectableSheetAssets;
				m_SelectedSheetAsset = windowSessionState.SelectedSheetAsset;
				m_SelectedTypeIndex = windowSessionState.SelectedTypeIndex;
				m_PinnedIndexSets = windowSessionState.PinnedIndexSets;
				m_NewAmount = windowSessionState.NewAmount;
				m_SearchInput = windowSessionState.SearchInput;
			}

			// Workaround because we cannot directly scan for objects within OnEnable.
			// This is due to a bug with calling AssetDatabase.Refresh within OnEnable when opening a window via custom Layout.
			m_Reinitialized = true;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
		}

		private void OnDestroy()
		{
			m_Settings.WindowDestroyed();
			Instances.Remove(this);
		}

		private void OnInspectorUpdate()
		{
			if (m_Settings.Workload.AutoUpdate)
			{
				Repaint();
			}
		}

		public void ForceRefreshColumnLayout()
		{
			m_ForceRefreshColumnLayout = true;
		}

		public WindowSessionState GetWindowSessionState()
		{
			var windowSession = new WindowSessionState()
			{
				InstanceId = GetInstanceID(),
				Title = titleContent.text,
				Position = position.ToString(),
				SelectableSheetAssets = m_SelectableSheetAssets,
				SelectedSheetAsset = m_SelectedSheetAsset,
				SelectedTypeIndex = m_SelectedTypeIndex,
				PinnedIndexSets = m_PinnedIndexSets,
				NewAmount = m_NewAmount,
				SearchInput = m_SearchInput,
			};
			return windowSession;
		}

		public void ResetSelectedType()
		{
			m_PreviousSelectedType = null;
			m_SelectedTypeIndex = m_PinnedIndexSets[m_SelectedSheetAsset].FirstOrDefault();
		}

		public void ScanObjects()
		{
			m_Scanner.ScanObjects(m_Settings.ObjectManagement.Scan, m_SelectedSheetAsset);
		}

		private bool IsScriptableObject()
		{
			return m_SelectedSheetAsset == SheetAsset.ScriptableObject && m_SelectedType != null && m_SelectedType.IsSubclassOf(typeof(ScriptableObject));
		}


		private void OnUndoRedoPerformed()
		{
			Repaint();
		}

		private void OnGUI()
		{
			if (m_Reinitialized || m_Settings.Workload.AutoScan)
			{
				m_Reinitialized = false;
				ScanObjects();
			}

			// Handle table nav and smart paste inputs up front to prevent the events being used.
			if (m_TableNav.UpdateFocusedCoordinate(m_PropertyTable, IsScriptableObject()))
			{
				// Repaint if there was keyboard navigation to force column highlighting for non text fields.
				if (m_TableNav.WasKeyboardNav)
				{
					Repaint();
				}
				if (!m_TableNav.IsEditingTextField)
				{
					if (m_Settings.DataTransfer.SmartPasteEnabled && m_TableSmartPaste.UpdatePasteContent())
					{
						SetTableAction(TableAction.SmartPaste);
					}
					else
					{
						m_TableSmartPaste.TryCopySingleCell(m_PropertyTable, m_TableNav.FocusedCoordinate, GetFlatFileFormatSettings());
					}
				}
			}

			if (m_Settings.Workload.Debug)
			{
				Debug.Log($"Focused cell coordinate '{m_TableNav.FocusedCoordinate}'.");
			}

			EditorGUILayout.BeginHorizontal();
			GUI.SetNextControlName(string.Empty);
			var previousSelectedSheetAsset = m_SelectedSheetAsset;
			m_SelectableSheetAssets = (SheetAsset) EditorGUILayout.EnumFlagsField(string.Empty, m_SelectableSheetAssets, SheetLayout.Property);
			if (m_SelectableSheetAssets == SheetAsset.Default)
			{
				m_SelectableSheetAssets = m_Settings.UserInterface.DefaultSheetAssets;
				m_SelectedSheetAsset = m_SelectableSheetAssets.FirstFlagOrDefault();
			}
			if (GUILayout.Button(SheetsContent.Button.Rescan, SheetLayout.InlineButton))
			{
				ScanObjects();
				EditorGUILayout.EndHorizontal();
				return;
			}
			m_SheetAssetScrollPosition = EditorGUILayout.BeginScrollView(m_SheetAssetScrollPosition, SheetLayout.DoubleLineHeight);
			EditorGUILayout.BeginHorizontal();
			foreach (SheetAsset sheetAsset in Enum.GetValues(typeof(SheetAsset)))
			{
				if (sheetAsset != SheetAsset.Default)
				{
					var isSelected = m_SelectedSheetAsset == sheetAsset;
					if (m_SelectableSheetAssets.HasFlag(sheetAsset))
					{
						var assetNameContent = SheetsContent.Label.GetAssetNameContent(sheetAsset.ToString());
						var width = GUI.skin.button.CalcSize(assetNameContent).x;
						if (GUILayout.Button(assetNameContent, SheetLayout.GetButtonStyle(isSelected), GUILayout.Width(width)))
						{
							m_SelectedSheetAsset = sheetAsset;
						}
					}
					else if (isSelected)
					{
						// If the selected sheet asset is disabled then default to the next selected sheet asset.
						m_SelectedSheetAsset = m_SelectableSheetAssets.FirstFlagOrDefault();
					}
					if (previousSelectedSheetAsset != m_SelectedSheetAsset)
					{
						ResetSelectedType();
						previousSelectedSheetAsset = m_SelectedSheetAsset;
						ScanObjects();
						// Exit early after Scanning Objects.
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndScrollView();
						EditorGUILayout.EndHorizontal();
						return;
					}
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();

			var isScriptableObject = IsScriptableObject();
			m_Scanner.Objects.RemoveAll(o => o == null);
			if (m_Scanner.Objects.Count <= 0)
			{
				if (m_SelectedType == null && m_SelectedSheetAsset == SheetAsset.ScriptableObject && m_Scanner.ObjectTypes.Length > 0)
				{
					m_SelectedTypeIndex = 0;
					m_SelectedType = m_Scanner.ObjectTypes[m_SelectedTypeIndex];
					isScriptableObject = IsScriptableObject();
				}
				if (!isScriptableObject || m_Settings.ObjectManagement.Scan.Option != ScanOption.Assembly)
				{
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.HelpBox($"Did not find any objects of type {m_SelectedSheetAsset} under path {m_Settings.ObjectManagement.Scan.Path}. Try updating the path or creating a new asset.", MessageType.Warning);
					m_NewAssetPath = string.Empty;
					m_Paginator.GoToFirstPage();
					m_SortedObjects.Clear();
					return;
				}
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			var previousSelectedTypeIndex = m_SelectedTypeIndex;
			GUI.SetNextControlName(string.Empty);
			m_SelectedTypeIndex = EditorGUILayout.Popup(string.Empty, m_SelectedTypeIndex, m_Scanner.ObjectTypeNames);
			var activePinnedIndexSet = m_PinnedIndexSets[m_SelectedSheetAsset];
			if (m_Settings.UserInterface.AutoPin && previousSelectedTypeIndex != m_SelectedTypeIndex)
			{
				activePinnedIndexSet.Add(m_SelectedTypeIndex);
			}
			if (activePinnedIndexSet.Contains(m_SelectedTypeIndex))
			{
				if (GUILayout.Button(SheetsContent.Button.Unpin, SheetLayout.InlineButton))
				{
					activePinnedIndexSet.Remove(m_SelectedTypeIndex);
				}
			}
			else
			{
				if (GUILayout.Button(SheetsContent.Button.Pin, SheetLayout.InlineButton))
				{
					activePinnedIndexSet.Add(m_SelectedTypeIndex);
				}
			}
			if (activePinnedIndexSet.Count > 1 && GUILayout.Button(SheetsContent.Button.UnpinAll, SheetLayout.InlineButton))
			{
				activePinnedIndexSet.Clear();
			}
			EditorGUILayout.EndHorizontal();

			if (activePinnedIndexSet.Count > 0)
			{
				m_ObjectTypeScrollPosition = EditorGUILayout.BeginScrollView(m_ObjectTypeScrollPosition, SheetLayout.DoubleLineHeight);
				EditorGUILayout.BeginHorizontal();
				foreach (var index in activePinnedIndexSet)
				{
					if (index < m_Scanner.ObjectTypes.Length)
					{
						var isSelected = m_SelectedTypeIndex == index;
						var objectTypeContent = SheetsContent.Label.GetObjectTypeContent(m_Scanner.FriendlyObjectTypeNames[index], m_Scanner.ObjectTypeNames[index]);
						var width = GUI.skin.button.CalcSize(objectTypeContent).x;
						if (GUILayout.Button(objectTypeContent, SheetLayout.GetButtonStyle(isSelected), GUILayout.Width(width)))
						{
							m_SelectedTypeIndex = index;
						}
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndScrollView();
			}

			if (m_SelectedTypeIndex >= m_Scanner.ObjectTypes.Length)
			{
				m_SelectedTypeIndex = 0;
			}
			m_SelectedType = m_Scanner.ObjectTypes[m_SelectedTypeIndex];
			var filteredObjects = m_Scanner.Objects.Where(o => o.GetType() == m_SelectedType).ToArray();
			if (isScriptableObject)
			{
				if (!s_MonoScriptCache.TryGetValue(m_SelectedType, out MonoScript monoScript))
				{
					var tempScriptableObject = CreateInstance(m_SelectedType);
					monoScript = MonoScript.FromScriptableObject(tempScriptableObject);
					s_MonoScriptCache.Add(m_SelectedType, monoScript);
					DestroyImmediate(tempScriptableObject);
				}
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.ObjectField(string.Empty, monoScript, typeof(MonoScript), false);
				EditorGUI.EndDisabledGroup();
			}

			EditorGUILayout.BeginHorizontal();
			if (isScriptableObject)
			{
				if (GUILayout.Button(SheetsContent.Button.GetCreateContent(m_NewAmount), SheetLayout.InlineButton))
				{
					var confirmed = true;
					if (m_NewAmount > 9000)
					{
						confirmed = EditorUtility.DisplayDialog("It's Over 9000!!!", "What!? 9000!? There's no way that can be right!", "Send it", "Cancel");
					}
					if (confirmed)
					{
						for (var i = 0; i < m_NewAmount; i++)
						{
							var selectedTypeName = m_SelectedType.Name;
							var newObjectName = m_Settings.ObjectManagement.NewObjectName;
							if (string.IsNullOrEmpty(newObjectName))
							{
								newObjectName = selectedTypeName;
							}
							var prefix = m_Settings.ObjectManagement.NewObjectPrefix;
							var suffix = m_Settings.ObjectManagement.NewObjectSuffix;
							if (m_Settings.ObjectManagement.UseExpansion)
							{
								var newObjectIndex = i + m_Settings.ObjectManagement.StartingIndex;
								var indexPadding = m_Settings.ObjectManagement.IndexPadding;
								newObjectName = newObjectName.ExpandAll(newObjectIndex, selectedTypeName, indexPadding);
								prefix = prefix.ExpandAll(newObjectIndex, selectedTypeName, indexPadding);
								suffix = suffix.ExpandAll(newObjectIndex, selectedTypeName, indexPadding);
							}
							newObjectName = $"{prefix}{newObjectName}{suffix}.asset";
							if (string.IsNullOrEmpty(m_NewAssetPath))
							{
								m_NewAssetPath = m_Settings.ObjectManagement.Scan.Path;
							}
							if (!AssetDatabase.IsValidFolder(m_NewAssetPath))
							{
								// If the path got deleted somehow, attempt to recreate it.
								Directory.CreateDirectory(m_NewAssetPath);
								AssetDatabase.Refresh();
								if (!AssetDatabase.IsValidFolder(m_NewAssetPath))
								{
									m_NewAssetPath = UnityConstants.DefaultAssetPath;
								}
							}
							var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(m_NewAssetPath + "/" + newObjectName);
							if (m_Settings.Workload.Debug)
							{
								Debug.Log($"Creating new asset of type {m_SelectedType} with name {newObjectName} at path {uniqueAssetPath}");
							}
							var newScriptableObject = CreateInstance(m_SelectedType);
							AssetDatabase.CreateAsset(newScriptableObject, uniqueAssetPath);
							m_Scanner.Objects.Add(newScriptableObject);
						}
						AssetDatabase.Refresh();
						EditorGUILayout.EndHorizontal();
						GUIUtility.keyboardControl = 0;
						m_Paginator.SetObjectsPerPage(m_Settings.Workload.RowsPerPage);
						m_Paginator.SetTotalObjects(m_Paginator.TotalObjects + m_NewAmount);
						if (!m_Paginator.IsOnLastPage())
						{
							m_Paginator.GoToLastPage();
						}
						return;
					}
				}
				// Force reset the control name.
				GUI.SetNextControlName(string.Empty);
				m_NewAmount = EditorGUILayout.IntField(m_NewAmount, SheetLayout.PropertySmall);
				m_NewAmount = Mathf.Clamp(m_NewAmount, 1, 9999);
				m_NewAssetPath = SheetLayout.DrawAssetPathSettingGUI(GUIContent.none, SheetsContent.Button.EditNewAssetPath, m_NewAssetPath, SheetLayout.Empty);
			}

			if (filteredObjects.Length <= 0)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.HelpBox($"Did not find any objects of type {m_SelectedType} under path {m_Settings.ObjectManagement.Scan.Path}. Try updating the path or creating a new asset.", MessageType.Warning);
				m_Paginator.GoToFirstPage();
				m_SortedObjects.Clear();
				return;
			}
			if (filteredObjects[0] == null)
			{
				EditorGUILayout.EndHorizontal();
				// An SO was deleted externally. Repaint the window.
				Repaint();
				return;
			}
			var hasSelectedTypeChanged = m_PreviousSelectedType != m_SelectedType;
			m_PreviousSelectedType = m_SelectedType;

			if (hasSelectedTypeChanged)
			{
				m_TableNav.ResetTextFieldEditing();
			}

			if (string.IsNullOrEmpty(m_NewAssetPath) || hasSelectedTypeChanged)
			{
				var defaultNewAssetPath = AssetDatabase.GetAssetPath(filteredObjects[0]).Replace($"{filteredObjects[0].name}.asset", string.Empty);
				if (string.IsNullOrEmpty(defaultNewAssetPath))
				{
					defaultNewAssetPath = m_Settings.ObjectManagement.Scan.Path;
				}
				m_NewAssetPath = defaultNewAssetPath;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if (m_MultiColumnHeader == null || hasSelectedTypeChanged || m_ForceRefreshColumnLayout)
			{
				m_ForceRefreshColumnLayout = false;
				if (m_Settings.UserInterface.OverrideArraySize)
				{
					var tempScriptableObject = CreateInstance(m_SelectedType);
					RefreshColumnLayout(tempScriptableObject, hasSelectedTypeChanged);
					DestroyImmediate(tempScriptableObject);
				}
				else
				{
					RefreshColumnLayout(filteredObjects[0], hasSelectedTypeChanged);
				}
			}

			if (m_SortingChanged || hasSelectedTypeChanged)
			{
				m_SortedObjects = m_MultiColumnHeader.GetSorted(filteredObjects);
				m_SortingChanged = false;
			}
			else
			{
				// Add new objects to the bottom of the current sort.
				m_SortedObjects = m_SortedObjects.Intersect(filteredObjects).ToList();
				m_SortedObjects.AddRange(filteredObjects.Except(m_SortedObjects));
			}

			EditorGUILayout.BeginHorizontal();
			var visibleColumnsLength = m_MultiColumnHeaderState.visibleColumns.Length;
			var excessVisibleColumns = visibleColumnsLength - m_Settings.Workload.VisibleColumnLimit;
			if (excessVisibleColumns > 0)
			{
				m_MultiColumnHeaderState.visibleColumns = m_MultiColumnHeaderState.visibleColumns.Take(visibleColumnsLength - excessVisibleColumns).ToArray();
			}
			GUI.enabled = m_MultiColumnHeaderState.visibleColumns.Length < Mathf.Min(m_Columns.Length, m_Settings.Workload.VisibleColumnLimit);
			if (GUILayout.Button(SheetsContent.Button.ShowColumns, SheetLayout.InlineButton))
			{
				m_MultiColumnHeaderState.visibleColumns = m_Columns.GetClampedColumns(m_Settings.Workload.VisibleColumnLimit);
			}
			GUI.enabled = true;
			var totalColumns = m_MultiColumnHeaderState.columns.Length;
			var totalVisibleColumns = m_MultiColumnHeaderState.visibleColumns.Length;
			var columnLabelContent = SheetsContent.Label.GetColumnContent(totalVisibleColumns, m_Settings.Workload.VisibleColumnLimit, totalColumns);
			var columnLabelWidth = GUI.skin.label.CalcSize(columnLabelContent).x;
			// GUI.color does not work in light theme so change the normal text color and then reset it based on light or dark theme.
			// https://docs.unity3d.com/ScriptReference/GUI-color.html
			var centerLabelStyleTextColor = SheetLayout.CenterLabelStyle.normal.textColor;
			if (totalColumns > totalVisibleColumns && totalVisibleColumns >= m_Settings.Workload.VisibleColumnLimit)
			{
				SheetLayout.CenterLabelStyle.normal.textColor = Color.yellow;
			}
			EditorGUILayout.LabelField(columnLabelContent, SheetLayout.CenterLabelStyle, GUILayout.Width(columnLabelWidth));
			SheetLayout.CenterLabelStyle.normal.textColor = centerLabelStyleTextColor;
			GUI.enabled = m_MultiColumnHeaderState.visibleColumns.Length > 1;
			if (GUILayout.Button(SheetsContent.Button.HideColumns, SheetLayout.InlineButton))
			{
				m_MultiColumnHeaderState.visibleColumns = new int[] { 0 };
			}
			GUI.enabled = true;
			SheetLayout.DrawVerticalLine();
			if (GUILayout.Button(SheetsContent.Button.Stretch, SheetLayout.InlineButton))
			{
				m_MultiColumnHeader.ResizeToFit();
			}
			if (GUILayout.Button(SheetsContent.Button.Compact, SheetLayout.InlineButton))
			{
				m_MultiColumnHeader.ResizeToMinWidth();
			}
			if (GUILayout.Button(SheetsContent.Button.Expand, SheetLayout.InlineButton))
			{
				m_MultiColumnHeader.ResizeToHeaderWidth(SheetLayout.InlineLabelSpacing);
			}
			SheetLayout.DrawVerticalLine();
			if (GUILayout.Button(SheetsContent.Button.CopyToClipboard, SheetLayout.InlineButton))
			{
				SetTableAction(TableAction.Copy);
			}
			EditorGUI.BeginDisabledGroup(!m_TableNav.HasFocus);
			if (GUILayout.Button(SheetsContent.Button.CopyRowToClipboard, SheetLayout.InlineButton))
			{
				SetTableAction(TableAction.CopyRow);
			}
			if (GUILayout.Button(SheetsContent.Button.CopyColumnToClipboard, SheetLayout.InlineButton))
			{
				SetTableAction(TableAction.CopyColumn);
			}
			if (GUILayout.Button(SheetsContent.Button.SmartPaste, SheetLayout.InlineButton))
			{
				SetTableAction(TableAction.SmartPaste);
			}
			EditorGUI.EndDisabledGroup();
			SheetLayout.DrawVerticalLine();
			if (GUILayout.Button(SheetsContent.Button.ImportFile, SheetLayout.InlineButton))
			{
				var filePath = EditorUtility.OpenFilePanel("Import", Application.dataPath, "*");
				if (!string.IsNullOrEmpty(filePath))
				{
					m_ImportedFileContents = File.ReadAllText(filePath);
					if (!string.IsNullOrEmpty(m_ImportedFileContents))
					{
						var extension = FlatFileUtility.GetExtensionFromPath(filePath);
						if (!string.IsNullOrEmpty(extension))
						{
							// Auto detect new delimiter based on file extension.
							if (FlatFileUtility.FlatFileDelimiters.TryGetValue(extension, out string delimiter))
							{
								m_Settings.DataTransfer.SetColumnDelimiter(delimiter);
							}
							else if (extension == JsonExtension)
							{
								m_IsImportJson = true;
							}
						}
						SetTableAction(TableAction.Import);
					}
					else
					{
						Debug.LogWarning($"File at path '{filePath}' is empty.");
					}
				}
			}
			if (GUILayout.Button(SheetsContent.Button.SaveToDisk, SheetLayout.InlineButton))
			{
				var dataPath = Application.dataPath;
				var fileExtension = "dsv";
				if (FlatFileUtility.FlatFileExtensions.TryGetValue(m_Settings.DataTransfer.GetColumnDelimiter(), out string delimiterExtension))
				{
					fileExtension = delimiterExtension;
				}
				m_SelectedFilepath = EditorUtility.SaveFilePanel("Save to", dataPath, $"{m_SelectedType.Name}", fileExtension);
				if (!string.IsNullOrEmpty(m_SelectedFilepath))
				{
					SetTableAction(TableAction.Save);
				}
			}
			SheetLayout.DrawVerticalLine();
			if (GUILayout.Button(SheetsContent.Button.NewPastePad, SheetLayout.InlineButton))
			{
				PastePadEditorWindow.ShowWindow();
			}

			GUILayout.FlexibleSpace();
			GUI.SetNextControlName(string.Empty);
			m_SearchInput = m_SearchField.OnGUI(m_SearchInput, SheetLayout.SearchBar);
			var useStringEnums = m_Settings.DataTransfer.UseStringEnums;
			var ignoreEnumCasing = m_Settings.DataTransfer.IgnoreCase;
			var matchingObjects = SearchFilter.GetObjects(m_SearchInput, m_SortedObjects, m_Settings.ObjectManagement.Search, useStringEnums, ignoreEnumCasing);

			m_Paginator.SetObjectsPerPage(m_Settings.Workload.RowsPerPage);
			m_Paginator.SetTotalObjects(matchingObjects.Count);
			var totalPages = m_Paginator.GetTotalPages();
			if (totalPages > 1)
			{
				SheetLayout.DrawVerticalLine();
				var showFirstAndLastPageButtons = totalPages > FirstAndLastPageThreshold;
				if (showFirstAndLastPageButtons)
				{
					EditorGUI.BeginDisabledGroup(m_Paginator.IsOnFirstPage());
					if (GUILayout.Button(SheetsContent.Button.FirstPage, SheetLayout.InlineButton))
					{
						m_Paginator.GoToFirstPage();
					}
					EditorGUI.EndDisabledGroup();
				}
				if (GUILayout.Button(SheetsContent.Button.PreviousPage, SheetLayout.InlineButton))
				{
					m_Paginator.PreviousPage();
				}
				var pageLabelContent = SheetsContent.Label.GetPageContent(m_Paginator.CurrentPage, totalPages, m_Paginator.TotalObjects);
				var pageLabelWidth = GUI.skin.label.CalcSize(pageLabelContent).x;
				EditorGUILayout.LabelField(pageLabelContent, SheetLayout.CenterLabelStyle, GUILayout.Width(pageLabelWidth));
				if (GUILayout.Button(SheetsContent.Button.NextPage, SheetLayout.InlineButton))
				{
					m_Paginator.NextPage();
				}
				if (showFirstAndLastPageButtons)
				{
					EditorGUI.BeginDisabledGroup(m_Paginator.IsOnLastPage());
					if (GUILayout.Button(SheetsContent.Button.LastPage, SheetLayout.InlineButton))
					{
						m_Paginator.GoToLastPage();
					}
					EditorGUI.EndDisabledGroup();
				}
			}
			EditorGUILayout.EndHorizontal();

			SheetLayout.DrawHorizontalLine();

			// Reset text field selection when page changes.
			if (m_PreviousSelectedPage != m_Paginator.CurrentPage)
			{
				m_PreviousSelectedPage = m_Paginator.CurrentPage;
				m_TableNav.ResetTextFieldEditing();
			}
			// Ignore pagination if we're trying to perform an action on all rows.
			var paginatedObjects = m_Settings.DataTransfer.PageRowsOnly || m_TableAction == TableAction.None ? m_Paginator.GetPageObjects(matchingObjects) : matchingObjects;
			var totalRows = paginatedObjects.Count;

			GUILayout.FlexibleSpace();
			var windowRect = GUILayoutUtility.GetLastRect();
			windowRect.width = position.width;
			windowRect.height = position.height;
			var rowHeight = EditorGUIUtility.singleLineHeight * SheetLayout.RowLineHeight;
			var columnHeaderRowRect = new Rect(windowRect)
			{
				height = rowHeight,
			};
			m_MultiColumnHeader.OnGUI(columnHeaderRowRect, m_ScrollPosition.x);

			// GetRect returns an empty rect during certain event types like layout.
			// In versions after 2022.3.43f1 and 6000.0.15f1. Unity starts returning a rect with float.MaxValue so we need to check that as well.
			// So validate the width and height before updating the scroll view.
			// https://forum.unity.com/threads/guilayoututility-getrect-with-inconsistent-results.8278/
			{
				var scrollViewArea = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
				if (scrollViewArea.width > 1 && scrollViewArea.width < float.MaxValue && scrollViewArea.height > 1 && scrollViewArea.height < float.MaxValue)
				{
					m_ScrollViewArea = scrollViewArea;
					m_TableScrollViewRect = new Rect(windowRect)
					{
						height = (totalRows + SheetLayout.TableViewRowPadding) * rowHeight,
						xMax = m_MultiColumnHeaderState.widthOfAllVisibleColumns,
					};
					if (!m_TableNav.WasKeyboardNav && m_TableNav.HasFocus && m_PropertyTable.IsValidCoordinate(m_TableNav.PreviousFocusedCoordinate))
					{
						if (m_PropertyTable.TryGet(m_TableNav.PreviousFocusedCoordinate, out ITableProperty property))
						{
							// Force reselect property if it shifted during a layout update. Usually caused by virtualization setting.
							if (GUI.GetNameOfFocusedControl() != property.ControlName)
							{
								GUI.FocusControl(property.ControlName);
							}
						}
						else
						{
							// Reset the focused control because it is out of view. Cannot use empty string because it'll try to select a valid empty string control.
							GUI.FocusControl("null");
						}
					}
				}
			}
			m_ScrollPosition = GUI.BeginScrollView(m_ScrollViewArea, m_ScrollPosition, m_TableScrollViewRect, false, false);
			var scrollStart = new Vector2(m_ScrollViewArea.x + m_ScrollPosition.x, m_ScrollViewArea.y + m_ScrollPosition.y);
			var scrollEnd = new Vector2(scrollStart.x + m_ScrollViewArea.width, scrollStart.y + m_ScrollViewArea.height - rowHeight);

			Profiler.BeginSample("DrawTable");
			m_PropertyTable = new Table<ITableProperty>(totalRows, m_MultiColumnHeaderState.visibleColumns.Length);
			var startingRowIndex = 0;
			// Add a buffer to the starting column index for scrolling.
			var startingColumnIndex = -1;
			if (m_Settings.Workload.Virtualization && m_TableAction == TableAction.None)
			{
				var adjustedScrollStartY = scrollStart.y - rowHeight - m_ScrollViewArea.y;
				startingRowIndex = Mathf.Max(0, Mathf.CeilToInt(adjustedScrollStartY / rowHeight) - 1);

				var totalColumnWidth = 0f;
				foreach (var visibleColumnIndex in m_MultiColumnHeaderState.visibleColumns)
				{
					totalColumnWidth += m_MultiColumnHeader.GetColumnRect(visibleColumnIndex).width;
					if (totalColumnWidth > scrollStart.x)
					{
						break;
					}
					startingColumnIndex++;
				}
			}
			startingColumnIndex = Mathf.Max(0, startingColumnIndex);
			var lastVisibleRow = false;
			for (var rowIndex = startingRowIndex; rowIndex < totalRows; rowIndex++)
			{
				Profiler.BeginSample("DrawRow");
				var rootObject = paginatedObjects[rowIndex];
				var isFirstFilteredObject = rootObject == filteredObjects[0];
				var assetPath = AssetDatabase.GetAssetPath(rootObject);

				var rowRect = new Rect(columnHeaderRowRect);
				rowRect.y += rowHeight * (rowIndex + 1);

				var visualRowRect = new Rect(rowRect)
				{
					x = m_ScrollPosition.x
				};
				EditorGUI.DrawRect(visualRowRect, rowIndex % 2 == 0 ? SheetLayout.DarkerColor : SheetLayout.LighterColor);

				var serializedObject = new SerializedObject(rootObject);
				serializedObject.Update();

				var columnIndex = 0;
				if (columnIndex >= startingColumnIndex && m_MultiColumnHeader.IsColumnVisible(columnIndex))
				{
					var visibleColumnIndex = m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
					var columnRect = m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
					columnRect.y = rowRect.y;
					var actionRect = m_MultiColumnHeader.GetCellRect(visibleColumnIndex, columnRect);

					if (m_Settings.UserInterface.ShowRowIndex)
					{
						var rowIndexLabel = SheetsContent.Label.GetRowIndex(rowIndex);
						EditorGUI.LabelField(actionRect, rowIndexLabel);
						actionRect.x += SheetLayout.PropertyWidthSmall / 2;
					}
					var firstActionRect = new Rect(actionRect.x, actionRect.y, SheetLayout.InlineButtonWidth, actionRect.height);
					var secondActionRect = new Rect(firstActionRect.xMax + SheetLayout.InlineButtonSpacing, actionRect.y, SheetLayout.InlineButtonWidth, actionRect.height);
					if (GUI.Button(firstActionRect, SheetsContent.Button.Select))
					{
						EditorGUIUtility.PingObject(rootObject);
						Selection.activeObject = rootObject;
					}
					if (GUI.Button(secondActionRect, SheetsContent.Button.Delete))
					{
						var assetName = rootObject.name;
						if (!m_Settings.UserInterface.ConfirmDelete || EditorUtility.DisplayDialog($"Delete {assetName} asset?", $"{assetPath}\n\nYou cannot undo the delete assets action.", "Delete", "Cancel"))
						{
							if (m_Settings.Workload.Debug)
							{
								Debug.Log($"Deleting asset with name {assetName} at path {assetPath}");
							}
							AssetDatabase.DeleteAsset(assetPath);
							m_Scanner.Objects.Remove(rootObject);
							DestroyImmediate(rootObject, true);
							GUI.EndScrollView(true);
							GUIUtility.keyboardControl = 0;
							return;
						}
						actionRect.x = secondActionRect.xMax + SheetLayout.InlineButtonSpacing;
					}
				}

				columnIndex++;
				if (columnIndex >= startingColumnIndex && m_MultiColumnHeader.IsColumnVisible(columnIndex))
				{
					var visibleColumnIndex = m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
					var columnRect = m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
					columnRect.y = rowRect.y;
					var textFieldRect = m_MultiColumnHeader.GetCellRect(visibleColumnIndex, columnRect);

					var nameProperty = serializedObject.FindProperty(UnityConstants.Field.Name);
					var nextControlName = m_TableNav.SetNextControlName(m_PropertyTable, rowIndex, visibleColumnIndex);
					EditorGUI.BeginDisabledGroup(m_Settings.UserInterface.LockNames);
					var newName = EditorGUI.TextField(textFieldRect, rootObject.name);
					EditorGUI.EndDisabledGroup();
					var nameTableProperty = new SerializedTableProperty(rootObject, nameProperty.propertyPath, nextControlName);
					m_PropertyTable.Set(rowIndex, visibleColumnIndex, nameTableProperty);
					if (newName != rootObject.name)
					{
						AssetDatabase.RenameAsset(assetPath, newName);
						// Exit early for performance.
						// We prefer this over using a delayed field because the delayed field is less reliable especially when changing asset type mid edit.
						return;
					}
				}

				if (m_Settings.UserInterface.ShowAssetPath)
				{
					columnIndex++;
					if (columnIndex >= startingColumnIndex && m_MultiColumnHeader.IsColumnVisible(columnIndex))
					{
						var visibleColumnIndex = m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
						var columnRect = m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
						columnRect.y = rowRect.y;
						var textFieldRect = m_MultiColumnHeader.GetCellRect(visibleColumnIndex, columnRect);

						EditorGUI.BeginDisabledGroup(true);
						var nextControlName = m_TableNav.SetNextControlName(m_PropertyTable, rowIndex, visibleColumnIndex);
						var assetPathTableProperty = new AssetPathTableProperty(rootObject, assetPath, nextControlName);
						EditorGUI.TextField(textFieldRect, assetPathTableProperty.GetProperty());
						m_PropertyTable.Set(rowIndex, visibleColumnIndex, assetPathTableProperty);
						EditorGUI.EndDisabledGroup();
					}
				}

				if (m_Settings.UserInterface.ShowGuid)
				{
					columnIndex++;
					if (columnIndex >= startingColumnIndex && m_MultiColumnHeader.IsColumnVisible(columnIndex))
					{
						var visibleColumnIndex = m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
						var columnRect = m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
						columnRect.y = rowRect.y;
						var textFieldRect = m_MultiColumnHeader.GetCellRect(visibleColumnIndex, columnRect);

						EditorGUI.BeginDisabledGroup(true);
						var nextControlName = m_TableNav.SetNextControlName(m_PropertyTable, rowIndex, visibleColumnIndex);
						var guidTableProperty = new GuidTableProperty(rootObject, assetPath, nextControlName);
						EditorGUI.TextField(textFieldRect, guidTableProperty.GetProperty());
						m_PropertyTable.Set(rowIndex, visibleColumnIndex, guidTableProperty);
						EditorGUI.EndDisabledGroup();
					}
				}

				var iterator = serializedObject.GetIterator();
				var lastVisibleColumn = false;
				var includeChildren = true;
				while (iterator.NextVisible(includeChildren))
				{
					// Some Unity assets like Prefabs include the m_Name property in their iterator. Skip over it because we draw it separately for all Objects.
					if (iterator.propertyPath == UnityConstants.Field.Name)
					{
						continue;
					}
					includeChildren = m_Settings.UserInterface.ShowChildren;
					if (!m_MultiColumnTooltipPaths.TryGetValue(iterator.propertyPath, out int nextColumnIndex))
					{
						continue;
					}
					if (nextColumnIndex >= startingColumnIndex && iterator.IsPropertyVisible(m_Settings.UserInterface.ShowArrays, m_Settings.UserInterface.ShowReadOnly, out bool isReadOnlyUnityField))
					{
						if (m_MultiColumnHeader.IsColumnVisible(nextColumnIndex))
						{
							var visibleColumnIndex = m_MultiColumnHeader.GetVisibleColumnIndex(nextColumnIndex);
							var columnRect = m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
							columnRect.y = rowRect.y;
							var propertyRect = m_MultiColumnHeader.GetCellRect(visibleColumnIndex, columnRect);
							var propertyControlName = m_TableNav.SetNextControlName(m_PropertyTable, rowIndex, visibleColumnIndex);
							var isCustomField = isScriptableObject && !isReadOnlyUnityField;
							Profiler.BeginSample("DrawProperty");
							EditorGUI.BeginDisabledGroup(!iterator.editable || isReadOnlyUnityField);
							iterator.DrawProperty(propertyRect, rootObject, isCustomField, out bool arraySizeChanged);
							if (m_Settings.UserInterface.ShowArrays && isFirstFilteredObject && arraySizeChanged)
							{
								// The first filtered Object drives the column layout. One of its array sizes changed so the column layout gets refreshed.
								m_ForceRefreshColumnLayout = true;
							}
							EditorGUI.EndDisabledGroup();
							Profiler.EndSample();
							var tableProperty = new SerializedTableProperty(rootObject, iterator.propertyPath, propertyControlName);
							m_PropertyTable.Set(rowIndex, visibleColumnIndex, tableProperty);
							if (m_Settings.Workload.Virtualization)
							{
								if (propertyRect.x >= scrollEnd.x)
								{
									lastVisibleColumn = true;
								}
								if (propertyRect.y >= scrollEnd.y)
								{
									lastVisibleRow = true;
								}
								if (lastVisibleColumn && m_TableAction == TableAction.None)
								{
									break;
								}
							}
						}
					}
				}
				serializedObject.ApplyModifiedProperties();
				Profiler.EndSample();
				if (lastVisibleRow && m_TableAction == TableAction.None)
				{
					break;
				}
			}
			Profiler.EndSample();

			var tableNavVisualState = new TableNavVisualState()
			{
				MultiColumnHeader = m_MultiColumnHeader,
				ScrollViewArea = m_ScrollViewArea,
				ColumnHeaderRowRect = columnHeaderRowRect,
				RowHeight = rowHeight,
				ScrollStart = scrollStart,
				ScrollEnd = scrollEnd,
			};
			m_TableNav.UpdateFocusVisuals(m_PropertyTable, m_Settings.UserInterface.TableNav, tableNavVisualState, ref m_ScrollPosition, m_Settings.UserInterface.LockNames);

			GUI.EndScrollView(true);

			// Handle file actions after property table is drawn.
			if (m_TableAction != TableAction.None)
			{
				var TableAction = m_TableAction;
				m_TableAction = TableAction.None;
				var flatFileFormatSettings = GetFlatFileFormatSettings();
				switch (TableAction)
				{
					case TableAction.Copy:
						EditorGUIUtility.systemCopyBuffer = m_PropertyTable.ToFlatFileFormat(flatFileFormatSettings);
						break;

					case TableAction.CopyRow:
						flatFileFormatSettings.FirstRowIndex = m_TableNav.FocusedCoordinate.x;
						flatFileFormatSettings.FirstRowOnly = true;
						EditorGUIUtility.systemCopyBuffer = m_PropertyTable.ToFlatFileFormat(flatFileFormatSettings);
						break;

					case TableAction.CopyColumn:
						flatFileFormatSettings.FirstColumnIndex = m_TableNav.FocusedCoordinate.y;
						flatFileFormatSettings.FirstColumnOnly = true;
						EditorGUIUtility.systemCopyBuffer = m_PropertyTable.ToFlatFileFormat(flatFileFormatSettings);
						break;

					case TableAction.CopyJson:
						EditorGUIUtility.systemCopyBuffer = m_PropertyTable.ToJsonFormat(m_Settings.DataTransfer.JsonSerializationFormat, flatFileFormatSettings);
						break;

					case TableAction.SmartPaste:
						var focusedCoordinate = m_TableNav.FocusedCoordinate;
						//  If page rows or visible columns are disabled, then apply offsets so we're still starting the paste from the selected cell.
						if (!m_Settings.DataTransfer.PageRowsOnly)
						{
							focusedCoordinate.x += (m_Paginator.CurrentPage - 1) * m_Paginator.ObjectsPerPage;
						}
						if (!m_Settings.DataTransfer.VisibleColumnsOnly)
						{
							focusedCoordinate.y = m_CachedVisibleColumns[focusedCoordinate.y];
						}
						flatFileFormatSettings.SetStartingIndex(focusedCoordinate);
						m_TableSmartPaste.Paste(m_PropertyTable, flatFileFormatSettings);
						// This notifies the selected field to immediately update its contents after a paste action.
						m_TableNav.ResetTextFieldEditing();
						break;

					case TableAction.Import:
						if (m_IsImportJson)
						{
							m_IsImportJson = false;
							m_PropertyTable.FromJsonFormat(m_ImportedFileContents, m_Settings.DataTransfer.JsonSerializationFormat, flatFileFormatSettings);
						}
						else
						{
							m_PropertyTable.FromFlatFileFormat(m_ImportedFileContents, flatFileFormatSettings);
						}
						m_ImportedFileContents = string.Empty;
						GUIUtility.keyboardControl = 0;
						break;

					case TableAction.Save:
						var extension = FlatFileUtility.GetExtensionFromPath(m_SelectedFilepath);
						var flatFileContents = string.Empty;
						if (extension == JsonExtension)
						{
							flatFileContents = m_PropertyTable.ToJsonFormat(m_Settings.DataTransfer.JsonSerializationFormat, flatFileFormatSettings);
						}
						else
						{
							// Auto detect delimiter to use based on file extension if possible.
							if (FlatFileUtility.FlatFileDelimiters.TryGetValue(extension, out string delimiter))
							{
								flatFileFormatSettings.ColumnDelimiter = delimiter;
							}
							flatFileContents = m_PropertyTable.ToFlatFileFormat(flatFileFormatSettings);
						}
						File.WriteAllText(m_SelectedFilepath, flatFileContents);
						if (m_SelectedFilepath.Contains(Application.dataPath))
						{
							AssetDatabase.Refresh();
						}
						m_SelectedFilepath = string.Empty;
						break;

					default:
						Debug.LogWarning($"Unsupported {nameof(TableAction)} {TableAction}.");
						break;
				}
			}
			if (m_CachedVisibleColumns != null && m_CachedVisibleColumns.Length > 0)
			{
				m_MultiColumnHeaderState.visibleColumns = m_CachedVisibleColumns;
				m_CachedVisibleColumns = null;
			}

			if (m_Settings.Workload.AutoSave)
			{
				AssetDatabase.SaveAssets();
			}
		}

		private void RefreshColumnLayout(Object obj, bool hasSelectedTypeChanged)
		{
			var columns = new List<MultiColumnHeaderState.Column>();

			var extraPadding = m_Settings.UserInterface.ShowRowIndex ? SheetLayout.PropertyWidthSmall / 2 : 0;
			var width = SheetLayout.InlineButtonWidth * 2 + SheetLayout.InlineButtonSpacing * 2 + extraPadding;
			var actionColumnLabel = ColumnUtility.GetColumnIndexLabel(m_Settings.UserInterface.ShowColumnIndex, columns.Count);
			var actionColumn = ColumnUtility.CreateActionsColumn($"{actionColumnLabel}Actions", width);
			columns.Add(actionColumn);

			var serializedObject = new SerializedObject(obj);

			var isScriptableObject = IsScriptableObject();
			var nameProperty = serializedObject.FindProperty(UnityConstants.Field.Name);
			var nameColumnLabel = ColumnUtility.GetColumnIndexLabel(m_Settings.UserInterface.ShowColumnIndex, columns.Count);
			var nameColumn = ColumnUtility.CreatePropertyColumn(nameProperty, isScriptableObject, m_Settings.UserInterface.HeaderFormat, nameColumnLabel);
			columns.Add(nameColumn);

			if (m_Settings.UserInterface.ShowAssetPath)
			{
				var assetPathColumnLabel = ColumnUtility.GetColumnIndexLabel(m_Settings.UserInterface.ShowColumnIndex, columns.Count);
				var assetPathColumn = ColumnUtility.CreateAssetPathColumn($"{assetPathColumnLabel}Asset Path");
				columns.Add(assetPathColumn);
			}

			if (m_Settings.UserInterface.ShowGuid)
			{
				var guidColumnLabel = ColumnUtility.GetColumnIndexLabel(m_Settings.UserInterface.ShowColumnIndex, columns.Count);
				var guidColumn = ColumnUtility.CreateGuidColumn($"{guidColumnLabel}GUID");
				columns.Add(guidColumn);
			}

			var iterator = serializedObject.GetIterator();
			var includeChildren = true;
			while (iterator.NextVisible(includeChildren))
			{
				includeChildren = m_Settings.UserInterface.ShowChildren;
				if (iterator.IsPropertyVisible(m_Settings.UserInterface.ShowArrays, m_Settings.UserInterface.ShowReadOnly, out bool isReadOnly))
				{
					if (m_Settings.UserInterface.OverrideArraySize && iterator.propertyType == SerializedPropertyType.ArraySize)
					{
						iterator.intValue = m_Settings.UserInterface.ArraySize;
					}
					var propertyColumnLabel = ColumnUtility.GetColumnIndexLabel(m_Settings.UserInterface.ShowColumnIndex, columns.Count);
					var propertyColumn = ColumnUtility.CreatePropertyColumn(iterator, isScriptableObject, m_Settings.UserInterface.HeaderFormat, propertyColumnLabel);
					columns.Add(propertyColumn);
				}
			}

			// Remove duplicate columns like the name field for Prefabs.
			m_Columns = columns.GroupBy(c => c.headerContent.tooltip).Select(g => g.First()).ToArray();

			int[] cachedVisibleColumns;
			if (m_MultiColumnHeader == null || hasSelectedTypeChanged || m_Columns.Length != m_MultiColumnHeaderState.columns.Length)
			{
				m_MultiColumnHeaderState = new MultiColumnHeaderState(m_Columns)
				{
					visibleColumns = m_Columns.GetClampedColumns(m_Settings.Workload.VisibleColumnLimit)
				};
			}
			else
			{
				cachedVisibleColumns = m_MultiColumnHeaderState.visibleColumns;
				m_MultiColumnHeaderState = new MultiColumnHeaderState(m_Columns)
				{
					visibleColumns = cachedVisibleColumns.Take(m_Settings.Workload.VisibleColumnLimit).ToArray()
				};
			}
			m_MultiColumnHeader = new MultiColumnHeader(m_MultiColumnHeaderState);
			m_MultiColumnHeader.ResizeToHeaderWidth(SheetLayout.InlineLabelSpacing);
			m_MultiColumnHeader.sortedColumnIndex = 1;
			m_MultiColumnHeader.sortingChanged += OnSortingChanged;

			// Cache to map column tooltip paths directly to an index.
			m_MultiColumnTooltipPaths = m_MultiColumnHeaderState.columns
				.Select((column, index) => new KeyValuePair<string, int>(column.headerContent.tooltip, index))
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		private void SetTableAction(TableAction TableAction)
		{
			if (!m_Settings.DataTransfer.VisibleColumnsOnly)
			{
				// Temporarily restore all columns and cache current visible settings.
				m_CachedVisibleColumns = m_MultiColumnHeaderState.visibleColumns;
				m_MultiColumnHeaderState.visibleColumns = Enumerable.Range(0, m_Columns.Length).ToArray();
			}
			m_TableAction = TableAction;
		}

		private void OnSortingChanged(MultiColumnHeader multiColumnHeader)
		{
			m_SortingChanged = true;
			GUIUtility.keyboardControl = 0;
		}

		private FlatFileFormatSettings GetFlatFileFormatSettings()
		{
			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = m_Settings.DataTransfer.GetRowDelimiter(),
				ColumnDelimiter = m_Settings.DataTransfer.GetColumnDelimiter(),
				// Skip Actions column.
				FirstColumnIndex = 1,
				RemoveEmptyRows = m_Settings.DataTransfer.RemoveEmptyRows,
				UseStringEnums = m_Settings.DataTransfer.UseStringEnums,
				IgnoreCase = m_Settings.DataTransfer.IgnoreCase,
				WrapOption = m_Settings.DataTransfer.WrapOption,
			};
			if (m_Settings.DataTransfer.Headers)
			{
				formatSettings.ColumnHeaders = m_MultiColumnHeaderState.visibleColumns.Select
				(
					// Remove column index if it's in the header name.
					i => m_Settings.UserInterface.ShowColumnIndex ? m_Columns[i].headerContent.text.Remove(0, i.ToString().Length + 1) : m_Columns[i].headerContent.text
				).ToArray();
			}
			return formatSettings;
		}

		void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(SheetsContent.Window.ContextMenu.NewSheet, false, ShowWindow);
			menu.AddItem(SheetsContent.Window.ContextMenu.NewPastePad, false, PastePadEditorWindow.ShowWindow);
			menu.AddItem(SheetsContent.Window.ContextMenu.OpenSettings, false, ScriptableSheetsSettingsEditorWindow.ShowWindow);
			menu.AddItem(SheetsContent.Window.ContextMenu.Copy, false, () => SetTableAction(TableAction.Copy));
			menu.AddItem(SheetsContent.Window.ContextMenu.CopyJson, false, () => SetTableAction(TableAction.CopyJson));
		}
	}
}
