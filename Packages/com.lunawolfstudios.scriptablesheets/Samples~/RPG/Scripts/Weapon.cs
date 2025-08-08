using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.RPG
{
	[System.Serializable]
	public class Weapon : ScriptableObject
	{
		[SerializeField]
		private string m_WeaponName;
		public string WeaponName { get => m_WeaponName; set => m_WeaponName = value; }

		[SerializeField]
		private int m_Damage;
		public int Damage { get => m_Damage; set => m_Damage = value; }

		[SerializeField]
		private float m_AttackSpeed;
		public float AttackSpeed { get => m_AttackSpeed; set => m_AttackSpeed = value; }

		[SerializeField]
		private Color m_Color;
		public Color Color { get => m_Color; set => m_Color = value; }

		[SerializeField]
		private WeaponCategory m_WeaponCategory;
		public WeaponCategory WeaponCategory { get => m_WeaponCategory; set => m_WeaponCategory = value; }

		[SerializeField]
		private AudioClip m_AttackSound;
		public AudioClip AttackSound { get => m_AttackSound; set => m_AttackSound = value; }

		[SerializeField]
		private GameObject m_ProjectilePrefab;
		public GameObject ProjectilePrefab { get => m_ProjectilePrefab; set => m_ProjectilePrefab = value; }
	}
}
