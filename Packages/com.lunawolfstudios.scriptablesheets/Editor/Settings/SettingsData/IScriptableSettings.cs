using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets
{
	public interface IScriptableSettings
	{
		bool Foldout { get; set; }
		GUIContent FoldoutContent { get; }

		void DrawGUI();
	}
}
