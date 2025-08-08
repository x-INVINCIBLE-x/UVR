using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using LunaWolfStudiosEditor.ScriptableSheets.Scanning;
using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets
{
	[System.Serializable]
	public class UserInterfaceSettings : AbstractBaseSettings, IScriptableSettings
	{
		[SerializeField]
		private SheetAsset m_DefaultSheetAssets;
		public SheetAsset DefaultSheetAssets { get => m_DefaultSheetAssets; set => m_DefaultSheetAssets = value; }

		[SerializeField]
		private bool m_AutoPin;
		public bool AutoPin { get => m_AutoPin; set => m_AutoPin = value; }

		[SerializeField]
		private bool m_ConfirmDelete;
		public bool ConfirmDelete { get => m_ConfirmDelete; set => m_ConfirmDelete = value; }

		[SerializeField]
		private HeaderFormat m_HeaderFormat;
		public HeaderFormat HeaderFormat { get => m_HeaderFormat; set => m_HeaderFormat = value; }

		[SerializeField]
		private bool m_LockNames;
		public bool LockNames { get => m_LockNames; set => m_LockNames = value; }

		[SerializeField]
		private bool m_ShowRowIndex;
		public bool ShowRowIndex { get => m_ShowRowIndex; set => m_ShowRowIndex = value; }

		[SerializeField]
		private bool m_ShowColumnIndex;
		public bool ShowColumnIndex { get => m_ShowColumnIndex; set => m_ShowColumnIndex = value; }

		[SerializeField]
		private bool m_ShowChildren;
		public bool ShowChildren { get => m_ShowChildren; set => m_ShowChildren = value; }

		[SerializeField]
		private bool m_ShowArrays;
		public bool ShowArrays { get => m_ShowArrays; set => m_ShowArrays = value; }

		[SerializeField]
		private bool m_OverrideArraySize;
		public bool OverrideArraySize { get => m_OverrideArraySize; set => m_OverrideArraySize = value; }

		[SerializeField]
		private int m_ArraySize;
		public int ArraySize { get => m_ArraySize; set => m_ArraySize = value; }

		[SerializeField]
		private bool m_ShowAssetPath;
		public bool ShowAssetPath { get => m_ShowAssetPath; set => m_ShowAssetPath = value; }

		[SerializeField]
		private bool m_ShowGuid;
		public bool ShowGuid { get => m_ShowGuid; set => m_ShowGuid = value; }

		[SerializeField]
		private bool m_ShowReadOnly;
		public bool ShowReadOnly { get => m_ShowReadOnly; set => m_ShowReadOnly = value; }

		[SerializeField]
		private TableNavSettings m_TableNav;
		public TableNavSettings TableNav { get => m_TableNav; set => m_TableNav = value; }

		public override GUIContent FoldoutContent => SettingsContent.Foldouts.UserInterface;

		public UserInterfaceSettings()
		{
			Foldout = true;
			m_DefaultSheetAssets = SheetAsset.ScriptableObject;
			m_AutoPin = true;
			m_ConfirmDelete = true;
			m_HeaderFormat = HeaderFormat.Friendly;
			m_LockNames = false;
			m_ShowRowIndex = false;
			m_ShowColumnIndex = false;
			m_ShowChildren = true;
			m_ShowArrays = true;
			m_OverrideArraySize = false;
			m_ArraySize = 10;
			m_ShowAssetPath = false;
			m_ShowGuid = false;
			m_ShowReadOnly = false;
			m_TableNav = new TableNavSettings();
		}

		protected override void DrawProperties()
		{
			m_DefaultSheetAssets = (SheetAsset) EditorGUILayout.EnumFlagsField(SettingsContent.Dropdown.DefaultSheetAssets, m_DefaultSheetAssets);
			if (m_DefaultSheetAssets == SheetAsset.Default)
			{
				m_DefaultSheetAssets = SheetAsset.ScriptableObject;
			}
			m_AutoPin = EditorGUILayout.Toggle(SettingsContent.Toggle.AutoPin, m_AutoPin);
			m_ConfirmDelete = EditorGUILayout.Toggle(SettingsContent.Toggle.ConfirmDelete, m_ConfirmDelete);
			m_HeaderFormat = (HeaderFormat) EditorGUILayout.EnumPopup(SettingsContent.Dropdown.HeaderFormat, m_HeaderFormat);
			m_LockNames = EditorGUILayout.Toggle(SettingsContent.Toggle.LockNames, m_LockNames);
			m_ShowRowIndex = EditorGUILayout.Toggle(SettingsContent.Toggle.ShowRowIndex, m_ShowRowIndex);
			m_ShowColumnIndex = EditorGUILayout.Toggle(SettingsContent.Toggle.ShowColumnIndex, m_ShowColumnIndex);
			m_ShowChildren = EditorGUILayout.Toggle(SettingsContent.Toggle.ShowChildren, m_ShowChildren);
			if (m_ShowChildren)
			{
				SheetLayout.Indent();
				m_ShowArrays = EditorGUILayout.Toggle(SettingsContent.Toggle.ShowArrays, m_ShowArrays);
				if (m_ShowArrays)
				{
					SheetLayout.Indent();
					m_OverrideArraySize = EditorGUILayout.Toggle(SettingsContent.Toggle.OverrideArraySize, m_OverrideArraySize);
					if (m_OverrideArraySize)
					{
						SheetLayout.Indent();
						m_ArraySize = Mathf.Clamp(EditorGUILayout.IntField(SettingsContent.DigitField.ArraySize, m_ArraySize), 0, 1000);
						SheetLayout.Unindent();
					}
					SheetLayout.Unindent();
				}
				SheetLayout.Unindent();
			}
			m_ShowAssetPath = EditorGUILayout.Toggle(SettingsContent.Toggle.ShowAssetPath, m_ShowAssetPath);
			m_ShowGuid = EditorGUILayout.Toggle(SettingsContent.Toggle.ShowGuid, m_ShowGuid);
			m_ShowReadOnly = EditorGUILayout.Toggle(SettingsContent.Toggle.ShowReadOnly, m_ShowReadOnly);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(SettingsContent.Label.TableNav, EditorStyles.boldLabel);
			m_TableNav.AutoScroll = EditorGUILayout.Toggle(SettingsContent.Toggle.AutoScroll, m_TableNav.AutoScroll);
			m_TableNav.AutoSelect = EditorGUILayout.Toggle(SettingsContent.Toggle.AutoSelect, m_TableNav.AutoSelect);
			m_TableNav.HighlightAlpha = EditorGUILayout.Slider(SettingsContent.DigitField.HighlightAlpha, m_TableNav.HighlightAlpha, 0.0f, 0.25f);
			m_TableNav.HighlightSelectedRow = EditorGUILayout.Toggle(SettingsContent.Toggle.HighlightSelectedRow, m_TableNav.HighlightSelectedRow);
			m_TableNav.HighlightSelectedColumn = EditorGUILayout.Toggle(SettingsContent.Toggle.HighlightSelectedColumn, m_TableNav.HighlightSelectedColumn);
		}
	}
}
