using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Serializables
{
	[System.Serializable]
	public struct SerializableGradient
	{
		[SerializeField]
		private Gradient m_Gradient;
		public Gradient Gradient { get => m_Gradient; set => m_Gradient = value; }

		public SerializableGradient(Gradient gradient)
		{
			m_Gradient = gradient;
		}

		public SerializableGradient(SerializedProperty property)
		{
			m_Gradient = property.GetGradientValue();
		}
	}
}
