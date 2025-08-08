using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.RPG
{
	[System.Serializable]
	public class Unit : ScriptableObject
	{
		[SerializeField]
		private string m_DisplayName;
		public string DisplayName { get => m_DisplayName; set => m_DisplayName = value; }

		[SerializeField]
		private Color m_NameColor;
		public Color NameColor { get => m_NameColor; set => m_NameColor = value; }

		[SerializeField]
		private int m_Level;
		public int Level { get => m_Level; set => m_Level = value; }

		[SerializeField]
		private float m_Health;
		public float Health { get => m_Health; set => m_Health = value; }

		[SerializeField]
		private Sprite m_PortraitIcon;
		public Sprite Sprite { get => m_PortraitIcon; set => m_PortraitIcon = value; }

		[SerializeField]
		private Weapon m_Weapon;
		public Weapon Weapon { get => m_Weapon; set => m_Weapon = value; }

		[SerializeField]
		private Material m_VisualSkin;
		public Material VisualSkin { get => m_VisualSkin; set => m_VisualSkin = value; }

		[SerializeField]
		private Gradient m_HighlightGradient;
		public Gradient HighlightGradient { get => m_HighlightGradient; set => m_HighlightGradient = value; }

		[SerializeField]
		private AnimationCurve m_SkillCurve;
		public AnimationCurve SkillCurve { get => m_SkillCurve; set => m_SkillCurve = value; }

		[SerializeField]
		private bool m_IsShiny;
		public bool Shiny { get => m_IsShiny; set => m_IsShiny = value; }

		[SerializeField]
		private Vector3 m_StartPosition;
		public Vector3 StartPosition { get => m_StartPosition; set => m_StartPosition = value; }

		[SerializeField]
		private Quaternion m_StartRotation;
		public Quaternion StartRotation { get => m_StartRotation; set => m_StartRotation = value; }
	}
}
