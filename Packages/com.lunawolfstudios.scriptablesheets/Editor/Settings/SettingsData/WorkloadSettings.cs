using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets
{
	[System.Serializable]
	public class WorkloadSettings : AbstractBaseSettings, IScriptableSettings
	{
		[SerializeField]
		private bool m_AutoSave;
		public bool AutoSave { get => m_AutoSave; set => m_AutoSave = value; }

		[SerializeField]
		private bool m_AutoScan;
		public bool AutoScan { get => m_AutoScan; set => m_AutoScan = value; }

		[SerializeField]
		private bool m_AutoUpdate;
		public bool AutoUpdate { get => m_AutoUpdate; set => m_AutoUpdate = value; }

		[SerializeField]
		private bool m_Debug;
		public bool Debug { get => m_Debug; set => m_Debug = value; }

		[SerializeField]
		private bool m_Virtualization;
		public bool Virtualization { get => m_Virtualization; set => m_Virtualization = value; }

		[SerializeField]
		private int m_MaxVisibleCells;
		public int MaxVisibleCells { get => m_MaxVisibleCells; set => m_MaxVisibleCells = value; }

		[SerializeField]
		private int m_RowsPerPage;
		public int RowsPerPage { get => m_RowsPerPage; set => m_RowsPerPage = value; }

		[SerializeField]
		private int m_VisibleColumnLimit;
		public int VisibleColumnLimit { get => m_VisibleColumnLimit; set => m_VisibleColumnLimit = value; }

		public override GUIContent FoldoutContent => SettingsContent.Foldouts.Workload;

		public WorkloadSettings()
		{
			Foldout = false;
			m_AutoSave = false;
			m_AutoScan = false;
			m_AutoUpdate = true;
			m_Debug = false;
			m_Virtualization = true;
			m_MaxVisibleCells = 800;
			m_RowsPerPage = 20;
			m_VisibleColumnLimit = 40;
		}

		protected override void DrawProperties()
		{
			m_AutoSave = EditorGUILayout.Toggle(SettingsContent.Toggle.AutoSave, m_AutoSave);
			m_AutoScan = EditorGUILayout.Toggle(SettingsContent.Toggle.AutoScan, m_AutoScan);
			m_AutoUpdate = EditorGUILayout.Toggle(SettingsContent.Toggle.AutoUpdate, m_AutoUpdate);
			m_Debug = EditorGUILayout.Toggle(SettingsContent.Toggle.Debug, m_Debug);
			m_Virtualization = EditorGUILayout.Toggle(SettingsContent.Toggle.Virtualization, m_Virtualization);
			m_MaxVisibleCells = EditorGUILayout.IntSlider(SettingsContent.DigitField.MaxVisibleCells, m_MaxVisibleCells, 100, 1600);
			m_RowsPerPage = EditorGUILayout.IntSlider(SettingsContent.DigitField.RowsPerPage, m_RowsPerPage, 1, Mathf.Min(100, m_MaxVisibleCells / m_VisibleColumnLimit));
			m_VisibleColumnLimit = EditorGUILayout.IntSlider(SettingsContent.DigitField.VisibleColumnLimit, m_VisibleColumnLimit, 1, Mathf.Min(100, m_MaxVisibleCells / m_RowsPerPage));
		}
	}
}
