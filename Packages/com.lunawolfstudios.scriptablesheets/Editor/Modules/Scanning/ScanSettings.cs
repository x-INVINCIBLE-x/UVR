using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Scanning
{
	[System.Serializable]
	public class ScanSettings
	{
		[SerializeField]
		private ScanOption m_Option = ScanOption.Default;
		public ScanOption Option { get => m_Option; set => m_Option = value; }

		[SerializeField]
		private string m_Path = UnityConstants.DefaultAssetPath;
		public string Path { get => m_Path; set => m_Path = value; }
	}
}
