using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using LunaWolfStudiosEditor.ScriptableSheets.Scanning;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Settings
{
#if UNITY_2020_1_OR_NEWER

	// We cannot persist a ScriptableSingleton in Unity versions prior to 2020 because FilePathAttribute is internal.
	// https://forum.unity.com/threads/missing-documentation-for-scriptable-singleton.292754/
	[FilePath("UserSettings/ScriptableSheetsSettings.asset", FilePathAttribute.Location.ProjectFolder)]
#endif
	public class ScriptableSheetsSettings : ScriptableSingleton<ScriptableSheetsSettings>
	{
		[SerializeField]
		private DataTransferSettings m_DataTransfer;
		public DataTransferSettings DataTransfer => m_DataTransfer;

		[SerializeField]
		private ObjectManagementSettings m_ObjectManagement;
		public ObjectManagementSettings ObjectManagement => m_ObjectManagement;

		[SerializeField]
		private UserInterfaceSettings m_UserInterface;
		public UserInterfaceSettings UserInterface => m_UserInterface;

		[SerializeField]
		private WorkloadSettings m_Workload;
		public WorkloadSettings Workload => m_Workload;

		[SerializeField]
		private string m_WindowSessionStates;

		private bool m_IsQuitting;
		private bool m_WasWindowDestroyedThisUpdate;

		private ScanOption m_PreviousScanOption;
		private string m_PreviousScanPath;
		private bool m_PreviousCaseSensitive;
		private bool m_PreviousStartsWith;

		private float m_PreviousHiglightAlpha;
		private bool m_PreviousHighlightSelectedRow;
		private bool m_PreviousHighlightSelectedColumn;
		private HeaderFormat m_PreviousHeaderFormat;
		private bool m_PreviousLockNames;
		private bool m_PreviousShowRowIndex;
		private bool m_PreviousShowColumnIndex;
		private bool m_PreviousShowChildren;
		private bool m_PreviousShowArrays;
		private bool m_PreviousOverrideArraySize;
		private int m_PreviousArraySize;
		private bool m_PreviousShowAssetPath;
		private bool m_PreviousShowGuids;
		private bool m_PreviousShowReadOnly;

		private int m_PreviousRowsPerPage;
		private int m_PreviousVisibleColumnLimit;

#if UNITY_2020_1_OR_NEWER
		private string m_FilePath;
		private string m_FolderPath;

		private void Awake()
		{
			if (!System.IO.File.Exists(GetFilePath()))
			{
				ResetDefaultsAndSave();
			}
			m_FilePath = System.IO.Path.Combine(Application.dataPath, "..", GetFilePath());
			m_FolderPath = System.IO.Path.GetDirectoryName(m_FilePath);
		}

#endif

		private void OnEnable()
		{
			CacheReactiveSettings();
			EditorApplication.wantsToQuit += OnEditorApplicationWantsToQuit;
			EditorApplication.update += OnEditorApplicationUpdate;
		}

		private void OnDisable()
		{
			// Save window session state when assembly reloads.
			if (!m_IsQuitting)
			{
				SaveWindowSessions();
			}
		}

		private bool OnEditorApplicationWantsToQuit()
		{
			try
			{
				m_IsQuitting = true;
				SaveWindowSessions();
#if UNITY_2020_1_OR_NEWER
				Save(true);
#endif
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"An error occured while trying to save {nameof(ScriptableSheetsSettings)}.\n{ex.Message}");
			}
			return true;
		}

		private void OnEditorApplicationUpdate()
		{
			m_WasWindowDestroyedThisUpdate = false;
		}

		private void SaveWindowSessions()
		{
			if (ScriptableSheetsEditorWindow.Instances.Count > 0)
			{
				var windowSessionStates = new List<WindowSessionState>();
				foreach (var window in ScriptableSheetsEditorWindow.Instances)
				{
					windowSessionStates.Add(window.GetWindowSessionState());
				}
				m_WindowSessionStates = JsonConvert.SerializeObject(windowSessionStates);
			}
		}

		public void WindowDestroyed()
		{
			// When the user maximizes an editor window then other docked editor windows are destroyed.
			// https://forum.unity.com/threads/how-do-i-prevent-docked-editorwindows-being-destroyed-when-pressing-play-with-maximize-on-play-on.276970/
			if (!m_IsQuitting && !m_WasWindowDestroyedThisUpdate)
			{
				// Save the session states for each window that was destroyed this update so they can be restored when unmaximized.
				SaveWindowSessions();
				m_WasWindowDestroyedThisUpdate = true;
			}
		}

		public WindowSessionState LoadWindowSessionState(ScriptableSheetsEditorWindow window)
		{
			if (!string.IsNullOrWhiteSpace(m_WindowSessionStates))
			{
				try
				{
					var windowSessionStates = JsonConvert.DeserializeObject<List<WindowSessionState>>(m_WindowSessionStates);
					if (windowSessionStates != null && windowSessionStates.Count > 0)
					{
						var instanceId = window.GetInstanceID();
						// Search by instance id first.
						var windowSessionState = windowSessionStates.FirstOrDefault(s => s.InstanceId == instanceId);
						if (windowSessionState == null)
						{
							var title = window.titleContent.text;
							var position = window.position.ToString();
							windowSessionState = windowSessionStates.FirstOrDefault(s => s.Title == title && s.Position == position);
							if (windowSessionState == null)
							{
								// Find the first window sesson state that isn't already in use.
								var activeInstanceIds = new HashSet<int>(ScriptableSheetsEditorWindow.Instances.Select(i => i.GetInstanceID()));
								windowSessionState = windowSessionStates.FirstOrDefault(s => !activeInstanceIds.Contains(s.InstanceId));
							}
						}
						// If we did not find a valid session state for this window then use default state.
						if (windowSessionState != null)
						{
							// Validate the pinned indexes have a key for each SheetAsset type.
							if (windowSessionState.PinnedIndexSets != null && windowSessionState.PinnedIndexSets.Count >= System.Enum.GetNames(typeof(SheetAsset)).Length)
							{
								// Validate each hash set within the dictionary is not null.
								if (!windowSessionState.PinnedIndexSets.Values.Any(v => v == null))
								{
									// Remove the found window session state so it's not loaded by another docked window at the same position.
									windowSessionStates.Remove(windowSessionState);
									m_WindowSessionStates = JsonConvert.SerializeObject(windowSessionStates);
									return windowSessionState;
								}
								else
								{
									Debug.LogWarning($"Failed to load {nameof(WindowSessionState)} data from JSON with value '{m_WindowSessionStates}'. Found null hash set. Using default state.");
								}
							}
							else
							{
								Debug.LogWarning($"Failed to load {nameof(WindowSessionState)} data from JSON with value '{m_WindowSessionStates}'. Index sets were not found or missing values. Using default state.");
							}
						}
					}
				}
				catch (System.Exception ex)
				{
					Debug.LogWarning($"Failed to load {nameof(WindowSessionState)} data from JSON with value '{m_WindowSessionStates}'. {ex.Message}. Using default state.");
				}
			}
			return null;
		}

		public void DrawGUI(bool isSeparateWindow)
		{
			Undo.RecordObject(this, $"{nameof(ScriptableSheetsSettings)}");

			m_DataTransfer.DrawGUI();
			m_ObjectManagement.DrawGUI();
			m_UserInterface.DrawGUI();
			m_Workload.DrawGUI();

			SheetLayout.DrawHorizontalLine();

#if UNITY_2020_1_OR_NEWER
			if (m_Workload.AutoSave || GUILayout.Button(SettingsContent.Button.SaveChangesToDisk))
			{
				Save(true);
			}
			if (GUILayout.Button(SettingsContent.Button.OpenFile))
			{
				Application.OpenURL(m_FilePath);
			}
			if (GUILayout.Button(SettingsContent.Button.OpenFolder))
			{
				Application.OpenURL(m_FolderPath);
			}
#endif
			if (GUILayout.Button(SettingsContent.Button.ResetDefaults))
			{
				if (EditorUtility.DisplayDialog("Scriptable Sheets", "Reset settings to default values?", "Confirm", "Cancel"))
				{
					ResetDefaultsAndSave();
				}
			}
			if (!isSeparateWindow && GUILayout.Button(SettingsContent.Button.OpenWindow))
			{
				ScriptableSheetsSettingsEditorWindow.ShowWindow();
			}

			// Repaint and refresh column layouts immediately as reactive settings change.
			var hasScanOptionChanged = m_PreviousScanOption != m_ObjectManagement.Scan.Option;
			var hasScanPathChanged = m_PreviousScanPath != m_ObjectManagement.Scan.Path;
			var hasHeaderFormatChanged = m_PreviousHeaderFormat != m_UserInterface.HeaderFormat;
			var hasShowRowIndexChanged = m_PreviousShowRowIndex != m_UserInterface.ShowRowIndex;
			var hasShowColumnIndexChanged = m_PreviousShowColumnIndex != m_UserInterface.ShowColumnIndex;
			var hasShowChildrenChanged = m_PreviousShowChildren != m_UserInterface.ShowChildren;
			var hasShowArraysChanged = m_PreviousShowArrays != m_UserInterface.ShowArrays;
			var hasOverrideArraySizeChanged = m_PreviousOverrideArraySize != m_UserInterface.OverrideArraySize;
			var hasArraySizeChanged = m_PreviousArraySize != m_UserInterface.ArraySize;
			var hasShowAssetPathChanged = m_PreviousShowAssetPath != m_UserInterface.ShowAssetPath;
			var hasShowGuidChanged = m_PreviousShowGuids != m_UserInterface.ShowGuid;
			var hasShowReadOnlyChanged = m_PreviousShowReadOnly != m_UserInterface.ShowReadOnly;
			var hasVisibleColumnLimitChanged = m_PreviousVisibleColumnLimit != m_Workload.VisibleColumnLimit;

			var needsColumnRefresh = hasScanOptionChanged || hasScanPathChanged
				|| hasHeaderFormatChanged || hasShowRowIndexChanged
				|| hasShowColumnIndexChanged || hasShowChildrenChanged
				|| hasShowArraysChanged || hasOverrideArraySizeChanged || hasArraySizeChanged
				|| hasShowGuidChanged || hasShowAssetPathChanged
				|| hasShowReadOnlyChanged || hasVisibleColumnLimitChanged;

			var hasCaseSensitiveChanged = m_PreviousCaseSensitive != m_ObjectManagement.Search.CaseSensitive;
			var hasStartsWithChanged = m_PreviousStartsWith != m_ObjectManagement.Search.StartsWith;
			var hasHighlightAlphaChanged = m_PreviousHiglightAlpha != m_UserInterface.TableNav.HighlightAlpha;
			var hasHighlightSelectedRowChanged = m_PreviousHighlightSelectedRow != m_UserInterface.TableNav.HighlightSelectedRow;
			var hasHighlightSelectedColumnChanged = m_PreviousHighlightSelectedColumn != m_UserInterface.TableNav.HighlightSelectedColumn;
			var hasLockNamesChanged = m_PreviousLockNames != m_UserInterface.LockNames;
			var hasRowsPerPageChanged = m_PreviousRowsPerPage != m_Workload.RowsPerPage;

			var needsRepaint = needsColumnRefresh || hasCaseSensitiveChanged
				|| hasStartsWithChanged || hasHighlightAlphaChanged
				|| hasHighlightSelectedRowChanged || hasHighlightSelectedColumnChanged
				|| hasLockNamesChanged || hasRowsPerPageChanged;

			if (needsRepaint)
			{
				foreach (var window in ScriptableSheetsEditorWindow.Instances)
				{
					if (hasScanPathChanged)
					{
						if (m_ObjectManagement.Scan.Option != ScanOption.Assembly)
						{
							window.ResetSelectedType();
						}
						window.ScanObjects();
					}
					else if (hasScanOptionChanged)
					{
						window.ScanObjects();
					}
					if (needsColumnRefresh)
					{
						window.ForceRefreshColumnLayout();
					}
					window.Repaint();
				}
			}

			CacheReactiveSettings();
		}

		private void CacheReactiveSettings()
		{
			m_PreviousScanOption = m_ObjectManagement.Scan.Option;
			m_PreviousScanPath = m_ObjectManagement.Scan.Path;
			m_PreviousCaseSensitive = m_ObjectManagement.Search.CaseSensitive;
			m_PreviousStartsWith = m_ObjectManagement.Search.StartsWith;

			m_PreviousHiglightAlpha = m_UserInterface.TableNav.HighlightAlpha;
			m_PreviousHighlightSelectedRow = m_UserInterface.TableNav.HighlightSelectedRow;
			m_PreviousHighlightSelectedColumn = m_UserInterface.TableNav.HighlightSelectedColumn;
			m_PreviousHeaderFormat = m_UserInterface.HeaderFormat;
			m_PreviousLockNames = m_UserInterface.LockNames;
			m_PreviousShowRowIndex = m_UserInterface.ShowRowIndex;
			m_PreviousShowColumnIndex = m_UserInterface.ShowColumnIndex;
			m_PreviousShowChildren = m_UserInterface.ShowChildren;
			m_PreviousShowArrays = m_UserInterface.ShowArrays;
			m_PreviousOverrideArraySize = m_UserInterface.OverrideArraySize;
			m_PreviousArraySize = m_UserInterface.ArraySize;
			m_PreviousShowAssetPath = m_UserInterface.ShowAssetPath;
			m_PreviousShowGuids = m_UserInterface.ShowGuid;
			m_PreviousShowReadOnly = m_UserInterface.ShowReadOnly;

			m_PreviousRowsPerPage = m_Workload.RowsPerPage;
			m_PreviousVisibleColumnLimit = m_Workload.VisibleColumnLimit;
		}

		private void ResetDefaultsAndSave()
		{
			m_DataTransfer = new DataTransferSettings();
			m_ObjectManagement = new ObjectManagementSettings();
			m_UserInterface = new UserInterfaceSettings();
			m_Workload = new WorkloadSettings();

#if UNITY_2020_1_OR_NEWER
			Save(true);
#endif
		}
	}
}
