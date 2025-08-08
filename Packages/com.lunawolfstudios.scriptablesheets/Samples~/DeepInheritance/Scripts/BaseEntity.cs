using System;
using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.DeepInheritance
{
	[Serializable]
	public class BaseEntity : ScriptableObject, IBaseEntity
	{
		[Header(nameof(IBaseEntity))]
		[SerializeField]
		private string m_FriendlyName;
		public string FriendlyName { get => m_FriendlyName; set => m_FriendlyName = value; }

		[SerializeField]
		private string m_ScientificName;
		public string ScientificName { get => m_ScientificName; set => m_ScientificName = value; }

		[TextArea]
		[SerializeField]
		private string m_Description;
		public string Description { get => m_Description; set => m_Description = value; }

		[SerializeField]
		private NativeRegion m_NativeRegions;
		public NativeRegion NativeRegions { get => m_NativeRegions; set => m_NativeRegions = value; }

		[SerializeField]
		private Habitat m_Habitats;
		public Habitat Habitats { get => m_Habitats; set => m_Habitats = value; }

		[SerializeField]
		private EntityAssets m_Assets;
		public EntityAssets Assets { get => m_Assets; set => m_Assets = value; }
	}

	[Serializable]
	public class BaseEntity<T> : BaseEntity, IBaseEntity<T> where T : Enum
	{
		[Header(nameof(IBaseEntity) + "<T>")]
		[SerializeField]
		private T m_Variant;
		public T Variant { get => m_Variant; set => m_Variant = value; }
	}
}
