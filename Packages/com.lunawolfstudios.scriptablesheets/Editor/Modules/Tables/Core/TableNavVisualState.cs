using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public struct TableNavVisualState
	{
		public MultiColumnHeader MultiColumnHeader { get; set; }
		public Rect ScrollViewArea { get; set; }
		public Rect ColumnHeaderRowRect { get; set; }
		public float RowHeight { get; set; }
		public Vector2 ScrollStart { get; set; }
		public Vector2 ScrollEnd { get; set; }
	}
}
