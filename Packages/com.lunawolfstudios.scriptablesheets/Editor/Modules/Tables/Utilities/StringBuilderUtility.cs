using System.Text;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public static class StringBuilderUtility
	{
		public static StringBuilder Wrap(this StringBuilder sb, string value, char open, char close)
		{
			return sb.Append(open).Append(value).Append(close);
		}

		public static StringBuilder Wrap(this StringBuilder sb, string value, Wrapper wrapper)
		{
			return sb.Wrap(value, wrapper.Open, wrapper.Close);
		}
	}
}
