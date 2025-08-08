using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.Collections
{
	[System.Serializable]
	public class CollectionSample : ScriptableObject
	{
		[SerializeField]
		private MaterialCollection m_Materials;
		public Material[] Materials { get => m_Materials.ObjectsArray; }

		[SerializeField]
		private StringCollection m_Strings;
		public string[] Strings { get => m_Strings.ObjectsArray; }
	}
}
