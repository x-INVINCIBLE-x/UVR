namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	public interface IEdible
	{
		EdibleCategory EdibleCategory { get; set; }
		Macronutrients Macronutrients { get; set; }
		float Water { get; set; }
	}
}
