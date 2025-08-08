using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public struct AssetPathTableProperty : ITableProperty
	{
		private static readonly string s_PropertyName = $"k_{nameof(AssetPathTableProperty)}";

		private readonly Object m_RootObject;
		public Object RootObject => m_RootObject;

		private readonly string m_PropertyPath;
		public string PropertyPath => m_PropertyPath;

		private readonly string m_ControlName;
		public string ControlName => m_ControlName;

		private readonly string m_AssetPath;
		public string AssetPath => m_AssetPath;

		public AssetPathTableProperty(Object rootObject, string assetPath, string controlName)
		{
			m_RootObject = rootObject;
			m_PropertyPath = s_PropertyName;
			m_ControlName = controlName;
			m_AssetPath = assetPath;
		}

		public string GetProperty()
		{
			return m_AssetPath;
		}

		public string GetProperty(FlatFileFormatSettings formatSettings)
		{
			return GetProperty();
		}

		public void SetProperty(string value, FlatFileFormatSettings formatSettings)
		{
			// Cannot change asset path property.
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
