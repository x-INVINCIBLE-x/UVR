using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Shared
{
	public static class DrawUtility
	{
		public static class GUI
		{
			public static bool ToggleCenter(Rect propertyRect, bool value)
			{
				var centerPoint = (propertyRect.width - 10) / 2;
				propertyRect.x += centerPoint;
				propertyRect.width -= centerPoint;
				return EditorGUI.Toggle(propertyRect, value);
			}

			public static uint UIntField(Rect propertyRect, uint value)
			{
				var textValue = EditorGUI.TextField(propertyRect, value.ToString());
				return uint.TryParse(textValue, out uint newValue) ? newValue : value;
			}

			public static ulong ULongField(Rect propertyRect, ulong value)
			{
				var textValue = EditorGUI.TextField(propertyRect, value.ToString());
				return ulong.TryParse(textValue, out ulong newValue) ? newValue : value;
			}
		}
	}
}
