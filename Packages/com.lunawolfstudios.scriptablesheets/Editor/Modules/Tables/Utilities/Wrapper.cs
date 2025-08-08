using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	[System.Serializable]
	public class Wrapper
	{
		[SerializeField]
		private char m_Open;
		public char Open { get => m_Open; set => m_Open = value; }

		[SerializeField]
		private char m_Close;
		public char Close { get => m_Close; set => m_Close = value; }

		public Wrapper(char openAndClose)
		{
			m_Open = openAndClose;
			m_Close = openAndClose;
		}

		public Wrapper(char open, char close)
		{
			m_Open = open;
			m_Close = close;
		}

		public string Wrap(string value)
		{
			return $"{m_Open}{value}{m_Close}";
		}
	}
}
