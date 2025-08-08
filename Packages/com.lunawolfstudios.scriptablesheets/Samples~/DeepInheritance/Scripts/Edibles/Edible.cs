using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[System.Serializable]
	public class Edible : BaseEntity, IEdible, IMeasurable
	{
		[Header(nameof(IEdible))]
		[SerializeField] private EdibleCategory m_EdibleCategory;
		public EdibleCategory EdibleCategory { get => m_EdibleCategory; set => m_EdibleCategory = value; }

		[SerializeField]
		private Macronutrients m_Macronutrients;
		public Macronutrients Macronutrients { get => m_Macronutrients; set => m_Macronutrients = value; }

		[SerializeField]
		private float m_Water;
		public float Water { get => m_Water; set => m_Water = value; }

		[Header(nameof(IMeasurable))]
		[SerializeField]
		private float m_Mass;
		public float Mass { get => m_Mass; set => m_Mass = value; }

		[SerializeField]
		private float m_Size;
		public float Size { get => m_Size; set => m_Size = value; }
	}

	[System.Serializable]
	public class Edible<T> : Edible, IBaseEntity<T> where T : System.Enum
	{
		[Header(nameof(IBaseEntity) + "<T>")]
		[SerializeField]
		private T m_Variant;
		public T Variant { get => m_Variant; set => m_Variant = value; }
	}
}
