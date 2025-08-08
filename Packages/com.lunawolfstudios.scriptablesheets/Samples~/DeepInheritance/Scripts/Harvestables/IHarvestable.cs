namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	public interface IHarvestable
	{
		Sowable SowableChild { get; set; }
		HarvestableCategory HarvestableCategory { get; set; }
		float HarvestYield { get; set; }
		SeasonalHarvest SeasonalHarvestStart { get; set; }
		SeasonalHarvest SeasonalHarvestEnd { get; set; }
	}
}
