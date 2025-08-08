using System;
using System.Linq;

namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	public static class EnumUtility
	{
		/// <summary>
		/// Returns the first set flag from a valid flagged enum. Returns the default value if no flags are set.
		/// </summary>
		public static T FirstFlagOrDefault<T>(this T flaggedEnum) where T : Enum
		{
			if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
			{
				throw new ArgumentException($"{typeof(T).FullName} is not a flagged enum.");
			}
			return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(f => (int) (object) f != 0 && flaggedEnum.HasFlag(f));
		}
	}
}
