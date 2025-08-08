using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Settings
{
	public class ScriptableSheetsSettingsProvider : SettingsProvider
	{
		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider()
		{
			var provider = new ScriptableSheetsSettingsProvider($"Preferences/Scriptable Sheets", SettingsScope.Project)
			{
				keywords = GetSearchKeywordsFromNestedStaticGUIContentFields<SettingsContent>()
			};
			return provider;
		}

		public ScriptableSheetsSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
		{
			Undo.undoRedoPerformed += OnUndoRedoPerformed;
		}

		private void OnUndoRedoPerformed()
		{
			Repaint();
		}

		public override void OnGUI(string searchContext)
		{
			base.OnGUI(searchContext);
			ScriptableSheetsSettings.instance.DrawGUI(false);
		}

		// Unity's default implementation doesn't account for nested classes. So we roll our own.
		public static IEnumerable<string> GetSearchKeywordsFromNestedStaticGUIContentFields<T>()
		{
			var nestedTypes = typeof(T).GetNestedTypes(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			var guiContentFields = nestedTypes.SelectMany(n => n.GetFields(BindingFlags.Static | BindingFlags.Public)).Where(f => f.FieldType == typeof(GUIContent));
			var searchKeywords = guiContentFields.Select(f => ((GUIContent) f.GetValue(null)).text).Where(keyword => !string.IsNullOrEmpty(keyword));
			return searchKeywords;
		}
	}
}
