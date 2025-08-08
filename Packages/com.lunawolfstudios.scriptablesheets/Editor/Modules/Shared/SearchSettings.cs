using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	[System.Serializable]
	public class SearchSettings
	{
		[SerializeField]
		private bool m_CaseSensitive = false;
		public bool CaseSensitive { get => m_CaseSensitive; set => m_CaseSensitive = value; }

		[SerializeField]
		private bool m_StartsWith = false;
		public bool StartsWith { get => m_StartsWith; set => m_StartsWith = value; }
	}
}
