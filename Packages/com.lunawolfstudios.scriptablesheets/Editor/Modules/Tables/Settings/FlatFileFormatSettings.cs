using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	[System.Serializable]
	public struct FlatFileFormatSettings
	{
		private static readonly Dictionary<WrapOption, Wrapper> s_Wrappers = new Dictionary<WrapOption, Wrapper>()
		{
			{ WrapOption.None, null },
			{ WrapOption.DoubleQuotes, new Wrapper('"') },
			{ WrapOption.SingleQuotes, new Wrapper('\'') },
			{ WrapOption.CurlyBraces, new Wrapper('{', '}') },
			{ WrapOption.SquareBrackets, new Wrapper('[', ']') },
			{ WrapOption.Parentheses, new Wrapper('(', ')') },
			{ WrapOption.AngleBrackets, new Wrapper('<', '>') },
		};

		[SerializeField]
		private string m_RowDelimiter;
		public string RowDelimiter { get => m_RowDelimiter; set => m_RowDelimiter = value; }

		[SerializeField]
		private string m_ColumnDelimiter;
		public string ColumnDelimiter { get => m_ColumnDelimiter; set => m_ColumnDelimiter = value; }

		[SerializeField]
		private string[] m_ColumnHeaders;
		public string[] ColumnHeaders { get => m_ColumnHeaders; set => m_ColumnHeaders = value; }

		[SerializeField]
		private int m_FirstRowIndex;
		public int FirstRowIndex { get => m_FirstRowIndex; set => m_FirstRowIndex = value; }

		[SerializeField]
		private int m_FirstColumnIndex;
		public int FirstColumnIndex { get => m_FirstColumnIndex; set => m_FirstColumnIndex = value; }

		[SerializeField]
		private bool m_FirstRowOnly;
		public bool FirstRowOnly { get => m_FirstRowOnly; set => m_FirstRowOnly = value; }

		[SerializeField]
		private bool m_FirstColumOnly;
		public bool FirstColumnOnly { get => m_FirstColumOnly; set => m_FirstColumOnly = value; }

		[SerializeField]
		private bool m_RemoveEmptyRows;
		public bool RemoveEmptyRows { get => m_RemoveEmptyRows; set => m_RemoveEmptyRows = value; }

		[SerializeField]
		private bool m_UseStringEnums;
		public bool UseStringEnums { get => m_UseStringEnums; set => m_UseStringEnums = value; }

		[SerializeField]
		private bool m_IgnoreCase;
		public bool IgnoreCase { get => m_IgnoreCase; set => m_IgnoreCase = value; }

		[SerializeField]
		private WrapOption m_WrapOption;
		public WrapOption WrapOption { get => m_WrapOption; set => m_WrapOption = value; }

		public bool HasHeaders => ColumnHeaders?.Length > 0;
		public bool HasWrapping => m_WrapOption != WrapOption.None;

		public void SetStartingIndex(Vector2Int coordinate)
		{
			m_FirstRowIndex = coordinate.x;
			m_FirstColumnIndex = coordinate.y;
		}

		public Wrapper GetWrapper()
		{
			if (s_Wrappers.TryGetValue(m_WrapOption, out Wrapper wrapper))
			{
				return wrapper;
			}
			else
			{
				Debug.LogWarning($"Unsupported {nameof(Tables.WrapOption)} {m_WrapOption}.");
				return null;
			}
		}

		public string GetJoinedColumnHeaders(Wrapper wrapper)
		{
			var includedColumnHeaders = m_ColumnHeaders.Skip(FirstColumnIndex);
			if (wrapper == null)
			{
				return string.Join(m_ColumnDelimiter, includedColumnHeaders);
			}
			else
			{
				var wrappedColumnHeaders = includedColumnHeaders.Select(h => wrapper.Wrap(h)).ToArray();
				return string.Join(m_ColumnDelimiter, wrappedColumnHeaders);
			}
		}
	}
}
