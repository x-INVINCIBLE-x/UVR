using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public static class PastePadLayout
	{
		public static readonly GUILayoutOption[] TextAreaOptions = { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
		public static readonly GUIStyle TextAreaStyle = new GUIStyle(EditorStyles.textArea);
	}
}
