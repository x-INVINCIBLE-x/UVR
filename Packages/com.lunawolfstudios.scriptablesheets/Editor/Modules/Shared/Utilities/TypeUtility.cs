using System;

namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	public static class TypeUtility
	{
		public static bool HasFlagsAttribute(this Type type)
		{
			return Attribute.IsDefined(type, typeof(FlagsAttribute));
		}
	}
}
