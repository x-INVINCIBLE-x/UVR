using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.RPG
{
	[System.Serializable]
	public class Inventory : ScriptableObject
	{
		[SerializeField]
		private int m_MaxCapacity;
		public int MaxCapacity { get => m_MaxCapacity; set => m_MaxCapacity = value; }

		[SerializeField]
		private Color m_BagColor;
		public Color BagColor { get => m_BagColor; set => m_BagColor = value; }

		[SerializeField]
		private Items m_Items;
		public Items m_Item { get => m_Items; set => m_Items = value; }
	}
}
