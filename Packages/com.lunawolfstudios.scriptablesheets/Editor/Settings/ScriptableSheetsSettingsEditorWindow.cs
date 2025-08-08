using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Settings
{
	public class ScriptableSheetsSettingsEditorWindow : EditorWindow
	{
		private Vector2 m_ScrollPosition;

		public static void ShowWindow()
		{
			var window = GetWindow<ScriptableSheetsSettingsEditorWindow>();
			window.titleContent = SettingsContent.Window.Title;
			window.Show();
		}

		private void OnEnable()
		{
			Undo.undoRedoPerformed += OnUndoRedoPerformed;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
		}

		private void OnUndoRedoPerformed()
		{
			Repaint();
		}

		private void OnGUI()
		{
			m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
			ScriptableSheetsSettings.instance.DrawGUI(true);
			EditorGUILayout.EndScrollView();
		}
	}
}
