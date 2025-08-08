using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.RPG
{
	[System.Serializable]
	public class Consumable
	{
		[SerializeField]
		private string m_DisplayName;
		public string DisplayName { get => m_DisplayName; set => m_DisplayName = value; }

		[SerializeField]
		private Color m_Color;
		public Color Color { get => m_Color; set => m_Color = value; }

		[SerializeField]
		private Sprite m_Icon;
		public Sprite Icon { get => m_Icon; set => m_Icon = value; }

		[SerializeField]
		private float m_HealthRestored;
		public float HealthRestored { get => m_HealthRestored; set => m_HealthRestored = value; }

		[SerializeField]
		private float m_ManaRestored;
		public float ManaRestored { get => m_ManaRestored; set => m_ManaRestored = value; }

		[SerializeField]
		private float m_StaminaRestored;
		public float StaminaRestored { get => m_StaminaRestored; set => m_StaminaRestored = value; }

		[SerializeField]
		private float m_DurationInSeconds;
		public float DurationInSeconds { get => m_DurationInSeconds; set => m_DurationInSeconds = value; }

		[SerializeField]
		private bool m_IsStackable;
		public bool IsStackable { get => m_IsStackable; set => m_IsStackable = value; }
	}
}
