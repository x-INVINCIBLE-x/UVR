using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public class PastePadContent : Content
	{
		public static class Window
		{
			public static readonly GUIContent Title = GetIconContent(EditorIcon.Edit, "Paste Pad", string.Empty);

			public class ContextMenu
			{
				public static readonly GUIContent New = new GUIContent("New");
				public static readonly GUIContent Clear = new GUIContent("Clear");
				public static readonly GUIContent Copy = new GUIContent("Copy");
				public static readonly GUIContent Save = new GUIContent("Save");
				public static readonly GUIContent WordWrap = new GUIContent("Word Wrap");
			}
		}
	}
}
