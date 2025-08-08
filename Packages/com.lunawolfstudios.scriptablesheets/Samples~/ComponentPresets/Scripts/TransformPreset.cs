using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.ComponentPresets
{
	[System.Serializable]
	public class TransformPreset : AbstractComponentPreset<Transform>, IComponentPreset
	{
		[SerializeField]
		private Vector3 m_Position;
		public Vector3 Position { get => m_Position; set => m_Position = value; }

		[SerializeField]
		private Vector3 m_EulerAngles;
		public Vector3 EulerAngles { get => m_EulerAngles; set => m_EulerAngles = value; }

		[SerializeField]
		private Vector3 m_LocalScale;
		public Vector3 LocalScale { get => m_LocalScale; set => m_LocalScale = value; }

		protected override void Apply(Transform obj)
		{
			obj.position = m_Position;
			obj.eulerAngles = m_EulerAngles;
			obj.localScale = m_LocalScale;
		}
	}
}
