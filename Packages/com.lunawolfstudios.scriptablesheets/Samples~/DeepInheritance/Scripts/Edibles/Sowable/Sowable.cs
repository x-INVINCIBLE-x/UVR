using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[System.Serializable]
	public class Sowable : Edible, ISowable
	{
		[Header(nameof(ISowable))]
		[SerializeField] 
		private Harvestable m_HarvestableParent;
		public Harvestable HarvestableParent { get => m_HarvestableParent; set => m_HarvestableParent = value; }

		[SerializeField] 
		private SowableCategory m_SowableCategory;
		public SowableCategory SowableCategory { get => m_SowableCategory; set => m_SowableCategory = value; }
	}

	[System.Serializable]
	public class Sowable<T> : Sowable, IBaseEntity<T> where T : System.Enum
	{
		[Header(nameof(IBaseEntity) + "<T>")]
		[SerializeField] 
		private T m_Variant;
		public T Variant { get => m_Variant; set => m_Variant = value; }
	}
}
