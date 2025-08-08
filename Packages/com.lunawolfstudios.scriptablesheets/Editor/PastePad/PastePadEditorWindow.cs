using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.PastePad
{
	public class PastePadEditorWindow : EditorWindow, IHasCustomMenu
	{
		private string m_Text;
		public string Text => m_Text;

		private bool m_WordWrap;
		private Vector2 m_ScrollPosition;

		private bool m_Clear;

		public static void ShowWindow()
		{
			var window = CreateInstance<PastePadEditorWindow>();
			window.titleContent = PastePadContent.Window.Title;
			window.minSize = new Vector2(300, 300);
			window.Show();
		}

		private void OnGUI()
		{
			if (m_Clear)
			{
				// Deselect the control OnGUI and then clear the text.
				GUI.FocusControl("null");
				m_Text = string.Empty;
				m_Clear = false;
			}
			m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
			PastePadLayout.TextAreaStyle.wordWrap = m_WordWrap;
			m_Text = EditorGUILayout.TextArea(m_Text, PastePadLayout.TextAreaStyle, PastePadLayout.TextAreaOptions);
			EditorGUILayout.EndScrollView();
		}

		void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(PastePadContent.Window.ContextMenu.New, false, ShowWindow);
			menu.AddItem(PastePadContent.Window.ContextMenu.Clear, false, Clear);
			menu.AddItem(PastePadContent.Window.ContextMenu.Copy, false, Copy);
			menu.AddItem(PastePadContent.Window.ContextMenu.Save, false, Save);
			menu.AddItem(PastePadContent.Window.ContextMenu.WordWrap, m_WordWrap, ToggleWordWrap);
		}

		private void Clear()
		{
			m_Clear = true;
		}

		private void Copy()
		{
			EditorGUIUtility.systemCopyBuffer = m_Text;
		}

		private void Save()
		{
			var selectedFilepath = EditorUtility.SaveFilePanel("Save to", Application.dataPath, "data", "txt");
			if (!string.IsNullOrEmpty(selectedFilepath))
			{
				File.WriteAllText(selectedFilepath, m_Text);
			}
		}

		private void ToggleWordWrap()
		{
			m_WordWrap = !m_WordWrap;
		}
	}
}
