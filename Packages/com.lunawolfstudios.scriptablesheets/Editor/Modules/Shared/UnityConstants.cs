namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	/// <summary>
	/// Constant field names and paths found within the Unity Editor.
	/// </summary>
	public static class UnityConstants
	{
		public const string DefaultAssetPath = "Assets";

		public static readonly string ArrayPropertyPath = $".{Field.Array}.{Field.ArrayData}[";
		public static readonly string EnumPrefix = "Enum:";
		public static readonly string ObjectWrapperJSON = "UnityEditor.ObjectWrapperJSON:";

		public static class Field
		{
			public const string Array = "Array";
			public const string ArrayData = "data";
			public const string ArraySize = "size";
			public const string Layer = "m_Layer";
			public const string Name = "m_Name";
			public const string Script = "m_Script";
			public const string StaticEditorFlags = "m_StaticEditorFlags";
			public const string Tag = "m_TagString";
		}

		public static class Path
		{
			public const string BuiltInExtra = "Resources/unity_builtin_extra";
		}

		public static class Type
		{
			public const string Double = "double";
			public const string Float = "float";
			public const string Int = "int";
			public const string Long = "long";
			public const string UInt = "uint";
			public const string ULong = "ulong";
		}
	}
}
