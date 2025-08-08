using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.ComponentPresets
{
	public class ApplyTransformPreset : MonoBehaviour
	{
		[SerializeField]
		private RigidbodyPreset m_RigidbodyPreset;

		[SerializeField]
		private TransformPreset m_TransformPreset;

		[SerializeField]
		private bool m_ApplyOnAwake;

		private void Awake()
		{
			if (m_ApplyOnAwake)
			{
				Apply();
			}
		}

		public void Apply()
		{
			if (m_TransformPreset != null)
			{
				m_TransformPreset.Apply(transform);
			}
			if (m_RigidbodyPreset != null)
			{
				var rigidbody = GetComponent<Rigidbody>();
				if (rigidbody != null)
				{
					m_RigidbodyPreset.Apply(rigidbody);
				}
			}
		}
	}
}
