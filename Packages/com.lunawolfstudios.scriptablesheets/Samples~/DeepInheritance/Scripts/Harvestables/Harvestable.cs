using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[System.Serializable]
	public class Harvestable : BaseEntity, IHarvestable
	{
		[Header(nameof(IHarvestable))]
		[SerializeField]
		private Sowable m_SowableChild;
		public Sowable SowableChild { get => m_SowableChild; set => m_SowableChild = value; }

		[SerializeField]
		private HarvestableCategory m_HarvestableCategory;
		public HarvestableCategory HarvestableCategory { get => m_HarvestableCategory; set => m_HarvestableCategory = value; }

		[SerializeField]
		private float m_HarvestYield;
		public float HarvestYield { get => m_HarvestYield; set => m_HarvestYield = value; }

		[SerializeField]
		private SeasonalHarvest m_SeasonalHarvestStart;
		public SeasonalHarvest SeasonalHarvestStart { get => m_SeasonalHarvestStart; set => m_SeasonalHarvestStart = value; }

		[SerializeField]
		private SeasonalHarvest m_SeasonalHarvestEnd;
		public SeasonalHarvest SeasonalHarvestEnd { get => m_SeasonalHarvestEnd; set => m_SeasonalHarvestEnd = value; }
	}

	[System.Serializable]
	public class Harvestable<T> : Harvestable, IBaseEntity<T> where T : System.Enum
	{
		[Header(nameof(IBaseEntity) + "<T>")]
		[SerializeField]
		private T m_Variant;
		public T Variant { get => m_Variant; set => m_Variant = value; }
	}
}
