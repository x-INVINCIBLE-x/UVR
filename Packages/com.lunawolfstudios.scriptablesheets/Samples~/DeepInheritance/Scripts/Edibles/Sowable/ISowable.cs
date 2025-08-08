namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	public interface ISowable
	{
		Harvestable HarvestableParent { get; set; }
		SowableCategory SowableCategory { get; set; }
	}
}
