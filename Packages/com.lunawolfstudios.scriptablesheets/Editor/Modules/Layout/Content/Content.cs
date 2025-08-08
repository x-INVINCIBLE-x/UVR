using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public class Content
	{
		// Workaround because Unity's default implementation for IconContent with a tooltip does not work.
		public static GUIContent GetIconContent(string iconName, string tooltip)
		{
			return new GUIContent(EditorGUIUtility.IconContent(iconName))
			{
				tooltip = tooltip
			};
		}

		public static GUIContent GetIconContent(string iconName, string text, string tooltip)
		{
			return new GUIContent(EditorGUIUtility.IconContent(iconName))
			{
				text = text,
				tooltip = tooltip
			};
		}
	}
}
