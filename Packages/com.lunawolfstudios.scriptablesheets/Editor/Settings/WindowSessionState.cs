using LunaWolfStudiosEditor.ScriptableSheets.Scanning;
using System.Collections.Generic;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets
{
	[System.Serializable]
	public class WindowSessionState
	{
		[SerializeField]
		private int m_InstanceId;
		public int InstanceId { get => m_InstanceId; set => m_InstanceId = value; }

		[SerializeField]
		private string m_Title;
		public string Title { get => m_Title; set => m_Title = value; }

		[SerializeField]
		private string m_Position;
		public string Position { get => m_Position; set => m_Position = value; }

		[SerializeField]
		private SheetAsset m_SelectableSheetAssets;
		public SheetAsset SelectableSheetAssets { get => m_SelectableSheetAssets; set => m_SelectableSheetAssets = value; }

		[SerializeField]
		private SheetAsset m_SelectedSheetAsset;
		public SheetAsset SelectedSheetAsset { get => m_SelectedSheetAsset; set => m_SelectedSheetAsset = value; }

		[SerializeField]
		private int m_SelectedTypeIndex;
		public int SelectedTypeIndex { get => m_SelectedTypeIndex; set => m_SelectedTypeIndex = value; }

		[SerializeField]
		private Dictionary<SheetAsset, HashSet<int>> m_PinnedIndexSets;
		public Dictionary<SheetAsset, HashSet<int>> PinnedIndexSets { get => m_PinnedIndexSets; set => m_PinnedIndexSets = value; }

		[SerializeField]
		private int m_NewAmount;
		public int NewAmount { get => m_NewAmount; set => m_NewAmount = value; }

		[SerializeField]
		private string m_SearchInput;
		public string SearchInput { get => m_SearchInput; set => m_SearchInput = value; }
	}
}
