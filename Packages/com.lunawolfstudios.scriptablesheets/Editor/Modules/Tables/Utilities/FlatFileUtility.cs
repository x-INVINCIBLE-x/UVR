using System.Collections.Generic;
using System.IO;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public static class FlatFileUtility
	{
		public static readonly Dictionary<string, string> FlatFileExtensions = new Dictionary<string, string>()
		{
			{ ",", "csv" },
			{ "|", "psv" },
			{ ";", "ssv" },
			{ "\t", "tsv" },
		};

		public static readonly Dictionary<string, string> FlatFileDelimiters = new Dictionary<string, string>()
		{
			{ "csv", "," },
			{ "psv", "|" },
			{ "ssv", ";" },
			{ "tsv", "\t" },
		};

		public static string GetExtensionFromPath(string path)
		{
			return Path.GetExtension(path).Replace(".", string.Empty);
		}
	}
}
