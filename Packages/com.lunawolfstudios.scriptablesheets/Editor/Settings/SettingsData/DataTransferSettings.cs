using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using System;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets
{
	[Serializable]
	public class DataTransferSettings : AbstractBaseSettings, IScriptableSettings
	{
		[SerializeField]
		private bool m_SmartPasteEnabled;
		public bool SmartPasteEnabled { get => m_SmartPasteEnabled; set => m_SmartPasteEnabled = value; }

		[SerializeField]
		private bool m_Headers;
		public bool Headers { get => m_Headers; set => m_Headers = value; }

		[SerializeField]
		private bool m_PageRowsOnly;
		public bool PageRowsOnly { get => m_PageRowsOnly; set => m_PageRowsOnly = value; }

		[SerializeField]
		private bool m_VisibleColumnsOnly;
		public bool VisibleColumnsOnly { get => m_VisibleColumnsOnly; set => m_VisibleColumnsOnly = value; }

		[SerializeField]
		private bool m_RemoveEmptyRows;
		public bool RemoveEmptyRows { get => m_RemoveEmptyRows; set => m_RemoveEmptyRows = value; }

		[SerializeField]
		private bool m_UseStringEnums;
		public bool UseStringEnums { get => m_UseStringEnums; set => m_UseStringEnums = value; }

		[SerializeField]
		private bool m_IgnoreCase;
		public bool IgnoreCase { get => m_IgnoreCase; set => m_IgnoreCase = value; }

		[SerializeField]
		private string m_RowDelimiter;

		[SerializeField]
		private string m_ColumnDelimiter;

		[SerializeField]
		private WrapOption m_WrapOption;
		public WrapOption WrapOption { get => m_WrapOption; set => m_WrapOption = value; }

		[SerializeField]
		private JsonSerializationFormat m_JsonSerializationFormat;
		public JsonSerializationFormat JsonSerializationFormat { get => m_JsonSerializationFormat; set => m_JsonSerializationFormat = value; }

		public override GUIContent FoldoutContent => SettingsContent.Foldouts.DataTransfer;

		public DataTransferSettings()
		{
			Foldout = true;
			m_SmartPasteEnabled = true;
			m_Headers = false;
			m_PageRowsOnly = true;
			m_VisibleColumnsOnly = true;
			m_RemoveEmptyRows = true;
			m_UseStringEnums = true;
			m_IgnoreCase = true;
			SetRowDelimiter(Environment.NewLine);
			SetColumnDelimiter("\t");
			m_WrapOption = WrapOption.None;
			m_JsonSerializationFormat = JsonSerializationFormat.Flat;
		}

		public string GetRowDelimiter()
		{
			return m_RowDelimiter.GetUnescapedText();
		}

		public void SetRowDelimiter(string value)
		{
			m_RowDelimiter = value.GetEscapedText();
		}

		public string GetColumnDelimiter()
		{
			return m_ColumnDelimiter.GetUnescapedText();
		}

		public void SetColumnDelimiter(string value)
		{
			m_ColumnDelimiter = value.GetEscapedText();
		}

		protected override void DrawProperties()
		{
			m_SmartPasteEnabled = EditorGUILayout.Toggle(SettingsContent.Toggle.SmartPaste, m_SmartPasteEnabled);
			m_Headers = EditorGUILayout.Toggle(SettingsContent.Toggle.Headers, m_Headers);
			m_PageRowsOnly = EditorGUILayout.Toggle(SettingsContent.Toggle.PageRowsOnly, m_PageRowsOnly);
			m_VisibleColumnsOnly = EditorGUILayout.Toggle(SettingsContent.Toggle.VisibleColumnsOnly, m_VisibleColumnsOnly);
			m_RemoveEmptyRows = EditorGUILayout.Toggle(SettingsContent.Toggle.RemoveEmptyRows, m_RemoveEmptyRows);
			m_UseStringEnums = EditorGUILayout.Toggle(SettingsContent.Toggle.UseStringEnums, m_UseStringEnums);
			if (m_UseStringEnums)
			{
				SheetLayout.Indent();
				m_IgnoreCase = EditorGUILayout.Toggle(SettingsContent.Toggle.IgnoreCase, m_IgnoreCase);
				SheetLayout.Unindent();
			}
			m_RowDelimiter = EditorGUILayout.TextField(SettingsContent.TextField.SmartPasteRowDelimiter, m_RowDelimiter);
			m_ColumnDelimiter = EditorGUILayout.TextField(SettingsContent.TextField.SmartPasteColumnDelimiter, m_ColumnDelimiter);
			m_WrapOption = (WrapOption) EditorGUILayout.EnumPopup(SettingsContent.Dropdown.WrapOption, m_WrapOption);
			m_JsonSerializationFormat = (JsonSerializationFormat) EditorGUILayout.EnumPopup(SettingsContent.Dropdown.JsonSerializationFormat, m_JsonSerializationFormat);
		}
	}
}
