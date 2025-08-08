using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets
{
	public abstract class AbstractBaseSettings : IScriptableSettings
	{
		[SerializeField]
		private bool m_Foldout;
		public bool Foldout { get => m_Foldout; set => m_Foldout = value; }

		public abstract GUIContent FoldoutContent { get; }

		public void DrawGUI()
		{
			SheetLayout.DrawHorizontalLine();
			m_Foldout = EditorGUILayout.Foldout(m_Foldout, FoldoutContent);
			if (m_Foldout)
			{
				SheetLayout.Indent();
				DrawProperties();
				SheetLayout.Unindent();
			}
		}

		protected abstract void DrawProperties();
	}
}
