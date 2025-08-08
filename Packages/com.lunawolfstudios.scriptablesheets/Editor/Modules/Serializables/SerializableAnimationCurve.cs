using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Serializables
{
	[System.Serializable]
	public struct SerializableAnimationCurve
	{
		[SerializeField]
		private AnimationCurve m_AnimationCurve;
		public AnimationCurve AnimationCurve { get => m_AnimationCurve; set => m_AnimationCurve = value; }

		public SerializableAnimationCurve(AnimationCurve animationCurve)
		{
			m_AnimationCurve = animationCurve;
		}

		public SerializableAnimationCurve(SerializedProperty property)
		{
			m_AnimationCurve = property.animationCurveValue;
		}
	}
}
