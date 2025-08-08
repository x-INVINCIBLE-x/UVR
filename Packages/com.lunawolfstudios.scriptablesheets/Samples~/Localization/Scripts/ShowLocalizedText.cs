using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.Localization
{
	public class ShowLocalizedText : MonoBehaviour
	{
		[SerializeField]
		private Language m_Language;

		[SerializeField]
		private LocalizedText m_LocalizedText;

		private void OnGUI()
		{
			if (m_LocalizedText != null)
			{
				var text = m_LocalizedText.GetLocalizedText(m_Language);
				GUI.Label(new Rect(10, 10, Screen.width, 50), text);
			}
		}
	}
}
