using System;

namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	public static class StringUtility
	{
		public static string ExpandAll(this string text, int index, string type, int padding = 0)
		{
			return text.ExpandIndex(index, padding).ExpandType(type);
		}

		public static string ExpandIndex(this string text, int index, int padding = 0)
		{
			return text.Replace("{i}", index.ToString(new string('0', padding)));
		}

		public static string ExpandType(this string text, string type)
		{
			return text.Replace("{t}", type);
		}

		public static string GetEscapedText(this string text)
		{
			text = text.Replace("\\", "\\\\");
			text = text.Replace("\r", "\\r");
			text = text.Replace("\n", "\\n");
			text = text.Replace("\t", "\\t");
			return text;
		}

		public static string GetUnescapedText(this string text)
		{
			text = text.Replace("\\\\", "\\");
			text = text.Replace("\\r", "\r");
			text = text.Replace("\\n", "\n");
			text = text.Replace("\\t", "\t");
			return text;
		}

		public static bool MatchesSearch(this string text, string searchTerm, SearchSettings settings)
		{
			var stringComparison = settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if (settings.StartsWith)
			{
				return text.StartsWith(searchTerm, stringComparison);
			}
			else
			{
				return text.IndexOf(searchTerm, stringComparison) >= 0;
			}
		}

		public static string UnwrapLayerMask(this string text)
		{
			// Unity wraps LayerMask values with 'LayerMask(#)' when copying from the Inspector.
			return text.Replace("LayerMask(", string.Empty).Replace(")", string.Empty);
		}
	}
}
