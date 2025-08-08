using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public interface ITableProperty
	{
		Object RootObject { get; }
		string PropertyPath { get; }
		string ControlName { get; }

		string GetProperty(FlatFileFormatSettings settings);

		void SetProperty(string value, FlatFileFormatSettings formatSettings);

		bool IsInputFieldProperty(bool isScriptableObject);

		bool NeedsSelectionBorder(bool lockNames = false);
	}
}
