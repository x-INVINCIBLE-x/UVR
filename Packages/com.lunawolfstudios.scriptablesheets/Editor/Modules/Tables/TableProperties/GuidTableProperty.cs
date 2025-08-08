using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public struct GuidTableProperty : ITableProperty
	{
		private static readonly string s_PropertyName = $"k_{nameof(GuidTableProperty)}";

		private readonly Object m_RootObject;
		public Object RootObject => m_RootObject;

		private readonly string m_PropertyPath;
		public string PropertyPath => m_PropertyPath;

		private readonly string m_ControlName;
		public string ControlName => m_ControlName;

		private readonly string m_Guid;
		public string Guid => m_Guid;

		public GuidTableProperty(Object rootObject, string assetPath, string controlName)
		{
			m_RootObject = rootObject;
			m_PropertyPath = s_PropertyName;
			m_ControlName = controlName;
			m_Guid = AssetDatabase.AssetPathToGUID(assetPath);
		}

		public string GetProperty()
		{
			return m_Guid;
		}

		public string GetProperty(FlatFileFormatSettings formatSettings)
		{
			return GetProperty();
		}

		public void SetProperty(string value, FlatFileFormatSettings formatSettings)
		{
			// Cannot change guid property.
			return;
		}

		public bool IsInputFieldProperty(bool isScriptableObject)
		{
			return false;
		}

		public bool NeedsSelectionBorder(bool lockNames = false)
		{
			return true;
		}
	}
}
