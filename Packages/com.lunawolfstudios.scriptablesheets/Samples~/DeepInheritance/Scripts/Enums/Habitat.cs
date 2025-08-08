namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[System.Flags]
	public enum Habitat
	{
		None = 0,
		Agricultural = 1,
		BurntAreas = 2,
		Caves = 4,
		Coastal = 8,
		Desert = 16,
		Forests = 32,
		Grasslands = 64,
		Marshes = 128,
		Meadows = 256,
		Mountains = 512,
		Swamps = 1024,
		Tundra = 2048,
		Urban = 4096,
		Wetlands = 8192,
		Woodlands = 16384,
		Global = Agricultural | BurntAreas | Caves | Coastal | Desert | Forests | Grasslands | Marshes | Meadows | Mountains | Swamps | Tundra | Urban | Wetlands | Woodlands
	}

}