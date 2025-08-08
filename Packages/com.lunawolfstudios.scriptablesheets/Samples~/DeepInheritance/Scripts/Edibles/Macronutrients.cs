using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[System.Serializable]
	public class Macronutrients
	{
		[Range(0, 1000)]
		[SerializeField]
		private float m_Carbohydrates;
		public float Carbohydrates { get => m_Carbohydrates; set => m_Carbohydrates = value; }

		[Range(0, 1000)]
		[SerializeField]
		private float m_Fat;
		public float Fat { get => m_Fat; set => m_Fat = value; }

		[Range(0, 1000)]
		[SerializeField]
		private float m_Protein;
		public float Protein { get => m_Protein; set => m_Protein = value; }
	}
}
