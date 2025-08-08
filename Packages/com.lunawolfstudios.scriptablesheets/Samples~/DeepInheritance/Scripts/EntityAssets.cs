using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[System.Serializable]
	public class EntityAssets
	{
		[SerializeField]
		private Sprite m_PortraitIcon;
		public Sprite PortraitIcon => m_PortraitIcon;

		[SerializeField]
		private GameObject m_Prefab;
		public GameObject Prefab => m_Prefab;
	}
}
