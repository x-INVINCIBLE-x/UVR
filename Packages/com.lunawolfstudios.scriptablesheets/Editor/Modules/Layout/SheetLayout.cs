using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public static class SheetLayout
	{
		public const int InlineButtonWidth = 32;
		public const int InlineButtonSpacing = 8;
		public const int InlineLabelSpacing = 10;
		public const int PropertyWidth = 150;
		public const int PropertyWidthSmall = 50;
		public const int GuidPropertyWidth = 250;
		public const int RowLineHeight = 1;
		public const int SubMenuThreshold = 40;
		public const int TableViewRowPadding = 2;
		private const int SingleIndent = 1;
		private const int InlineButtonHeight = 20;
		private const int SearchBarWidth = 250;

		public static readonly GUILayoutOption[] InlineButton = new GUILayoutOption[] { GUILayout.Width(InlineButtonWidth), GUILayout.Height(InlineButtonHeight) };
		public static readonly GUILayoutOption Property = GUILayout.Width(PropertyWidth);
		public static readonly GUILayoutOption PropertySmall = GUILayout.Width(PropertyWidthSmall);
		public static readonly GUILayoutOption DefaultLabel = GUILayout.Width(PropertyWidth - (SingleIndent * 15f));
		public static readonly GUILayoutOption SearchBar = GUILayout.Width(SearchBarWidth);
		public static readonly GUILayoutOption Empty = GUILayout.Width(0);
		public static readonly GUILayoutOption DoubleLineHeight = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);

		public static readonly GUIStyle CenterLabelStyle = new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.MiddleCenter
		};
		public static readonly GUIStyle DefaultButtonStyle = new GUIStyle(GUI.skin.button);

		public static readonly Color DarkerColor;
		public static readonly Color LighterColor;

		private static GUIStyle s_SelectedButtonStyle;
		private static Texture2D s_SelectionTexture;

		static SheetLayout()
		{
			Color defaultColor = EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
			DarkerColor = defaultColor * .9f;
			LighterColor = defaultColor * 1.1f;
		}

		public static string DrawAssetPathSettingGUI(GUIContent labelContent, GUIContent buttonContent, string path, params GUILayoutOption[] layoutOptions)
		{
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(labelContent, layoutOptions);

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField(path);
			EditorGUI.EndDisabledGroup();

			if (GUILayout.Button(buttonContent, InlineButton))
			{
				// Handle case where folder was deleted.
				if (!AssetDatabase.IsValidFolder(path))
				{
					path = Application.dataPath;
				}
				var selectedFolder = EditorUtility.OpenFolderPanel("Select Path", path, string.Empty);
				if (!string.IsNullOrEmpty(selectedFolder) && selectedFolder.Contains(Application.dataPath))
				{
					path = selectedFolder.Replace(Application.dataPath, "Assets");
					AssetDatabase.Refresh();
				}
			}

			EditorGUILayout.EndHorizontal();
			return path;
		}

		public static Texture2D CreateTexture(Vector2Int size, Color color)
		{
			return CreateTexture(size.x, size.y, color);
		}

		public static Texture2D CreateTexture(int width, int height, Color color)
		{
			var pixels = new Color32[width * height];
			for (var i = 0; i < pixels.Length; ++i)
			{
				pixels[i] = color;
			}
			var result = new Texture2D(width, height, TextureFormat.RGBA32, false);
			result.SetPixels32(pixels);
			result.Apply();
			return result;
		}

		public static void DrawHorizontalLine()
		{
			EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
		}

		public static void DrawVerticalLine()
		{
			EditorGUILayout.LabelField(string.Empty, GUI.skin.verticalSlider, GUILayout.Width(InlineLabelSpacing));
		}

		public static GUIStyle GetButtonStyle(bool isSelected)
		{
			if (isSelected)
			{
				if (s_SelectedButtonStyle == null)
				{
					s_SelectedButtonStyle = new GUIStyle(DefaultButtonStyle);
				}
				// In certain cases the selection texture becomes null. Ensure we get the existing or create a new one each time.
				s_SelectedButtonStyle.normal.background = GetOrCreateSelectionTexture();
				return s_SelectedButtonStyle;
			}
			else
			{
				return DefaultButtonStyle;
			}
		}

		public static Texture2D GetOrCreateSelectionTexture()
		{
			if (s_SelectionTexture == null)
			{
				s_SelectionTexture = CreateTexture(Vector2Int.one, GUI.skin.settings.selectionColor);
			}
			return s_SelectionTexture;
		}

		public static void Indent()
		{
			EditorGUI.indentLevel += SingleIndent;
		}

		public static void Unindent()
		{
			EditorGUI.indentLevel -= SingleIndent;
		}
	}
}
