using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	[System.Serializable]
	public class TableNavSettings
	{
		[SerializeField]
		private bool m_AutoScroll = true;
		public bool AutoScroll { get => m_AutoScroll; set => m_AutoScroll = value; }

		[SerializeField]
		private bool m_AutoSelect = true;
		public bool AutoSelect { get => m_AutoSelect; set => m_AutoSelect = value; }

		[SerializeField]
		private float m_HighlightAlpha = 0.08f;
		public float HighlightAlpha { get => m_HighlightAlpha; set => m_HighlightAlpha = value; }

		[SerializeField]
		private bool m_HighlightSelectedRow = true;
		public bool HighlightSelectedRow { get => m_HighlightSelectedRow; set => m_HighlightSelectedRow = value; }

		[SerializeField]
		private bool m_HighlightSelectedColumn = true;
		public bool HighlightSelectedColumn { get => m_HighlightSelectedColumn; set => m_HighlightSelectedColumn = value; }
	}
}
