namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	public interface IBaseEntity
	{
		string FriendlyName { get; set; }
		string ScientificName { get; set; }
		string Description { get; set; }
		NativeRegion NativeRegions { get; set; }
		Habitat Habitats { get; set; }
	}

	public interface IBaseEntity<T> : IBaseEntity where T : System.Enum
	{
		T Variant { get; set; }
	}
}
