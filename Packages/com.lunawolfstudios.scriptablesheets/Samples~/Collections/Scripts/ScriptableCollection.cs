using System.Collections.Generic;
using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.Collections
{
	[System.Serializable]
	public abstract class ScriptableCollection<T> : ScriptableObject
	{
		[SerializeField]
		private T[] m_ObjectsArray;
		public T[] ObjectsArray => m_ObjectsArray;

		[SerializeField]
		private List<T> m_ObjectsList;
		public List<T> ObjectsList => m_ObjectsList;
	}
}
