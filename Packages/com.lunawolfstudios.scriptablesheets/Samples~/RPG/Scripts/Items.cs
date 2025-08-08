using System.Collections.Generic;
using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.RPG
{
	[System.Serializable]
	public class Items
	{
		[SerializeField]
		private Weapon[] m_Weapons;
		public Weapon[] WeaponName { get => m_Weapons; set => m_Weapons = value; }

		[SerializeField]
		private List<Consumable> m_Consumables;
		public List<Consumable> Consumables { get => m_Consumables; set => m_Consumables = value; }
	}
}
