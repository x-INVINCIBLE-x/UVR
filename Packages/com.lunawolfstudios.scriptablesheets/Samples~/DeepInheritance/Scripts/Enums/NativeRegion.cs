namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[System.Flags]
	public enum NativeRegion
	{
		None = 0,
		Africa = 1,
		Antarctica = 2,
		Asia = 4,
		Australia = 8,
		Caribbean = 16,
		CentralAmerica = 32,
		Europe = 64,
		MiddleEast = 128,
		NorthAmerica = 256,
		Oceania = 512,
		SouthAmerica = 1024,
		Global = Africa | Antarctica | Asia | Australia | Caribbean | CentralAmerica | Europe | MiddleEast | NorthAmerica | Oceania | SouthAmerica
	}
}