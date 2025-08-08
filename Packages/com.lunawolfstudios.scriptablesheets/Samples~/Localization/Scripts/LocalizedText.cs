using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.Localization
{
	[System.Serializable]
	public class LocalizedText : ScriptableObject
	{
		[SerializeField]
		private string m_English;
		public string English { get => m_English; set => m_English = value; }

		[SerializeField]
		private string m_French;
		public string French { get => m_French; set => m_French = value; }

		[SerializeField]
		private string m_German;
		public string German { get => m_German; set => m_German = value; }

		[SerializeField]
		private string m_Italian;
		public string Italian { get => m_Italian; set => m_Italian = value; }

		[SerializeField]
		private string m_Polish;
		public string Polish { get => m_Polish; set => m_Polish = value; }

		[SerializeField]
		private string m_Russian;
		public string Russian { get => m_Russian; set => m_Russian = value; }

		[SerializeField]
		private string m_Spanish;
		public string Spanish { get => m_Spanish; set => m_Spanish = value; }

		public string GetLocalizedText(Language language)
		{
			switch (language)
			{
				case Language.English:
					return English;

				case Language.French:
					return French;

				case Language.German:
					return German;

				case Language.Italian:
					return Italian;

				case Language.Polish:
					return Polish;

				case Language.Russian:
					return Russian;

				case Language.Spanish:
					return Spanish;

				default:
					return English;
			}
		}
	}
}
