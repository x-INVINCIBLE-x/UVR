using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public class TableSmartPaste<T> where T : ITableProperty
	{
		private readonly EventModifiers m_CopyPasteModifier;

		private string m_PasteContent;

		public TableSmartPaste()
		{
			m_CopyPasteModifier = Application.platform == RuntimePlatform.OSXEditor ? EventModifiers.Command : EventModifiers.Control;
		}

		/// <summary>
		/// Attempts to update the paste content based on the command name or users key input.
		/// </summary>
		/// <returns>True if the paste content was updated. False otherwise.</returns>
		public bool UpdatePasteContent()
		{
			var e = Event.current;
			var isSmartPaste = e.commandName == "Paste" || IsKeyDownAndModifiers(e, KeyCode.V, m_CopyPasteModifier);
			if (isSmartPaste)
			{
				// Event.Use() should not be called for events of type Layout.
				if (e.type != EventType.Layout)
				{
					e.Use();
					m_PasteContent = EditorGUIUtility.systemCopyBuffer;
					return !string.IsNullOrEmpty(m_PasteContent);
				}
			}
			return false;
		}

		/// <summary>
		/// Attempts to copy a single cell to the system copy buffer based on the command name or users key input.
		/// </summary>
		public void TryCopySingleCell(Table<T> propertyTable, Vector2Int focusedCoordinate, FlatFileFormatSettings formatSettings)
		{
			var e = Event.current;
			var isCopy = e.commandName == "Copy" || IsKeyDownAndModifiers(e, KeyCode.C, m_CopyPasteModifier);
			if (isCopy)
			{
				if (propertyTable.TryGet(focusedCoordinate, out T property))
				{
					if (e.type != EventType.Layout)
					{
						e.Use();
						var wrapper = formatSettings.GetWrapper();
						var propertyValue = property.GetProperty(formatSettings);
						EditorGUIUtility.systemCopyBuffer = wrapper == null ? propertyValue : wrapper.Wrap(propertyValue);
					}
				}
			}
		}

		/// <summary>
		/// Pastes the content into the specified property table and resets the cached paste content.
		/// </summary>
		public void Paste(Table<ITableProperty> propertyTable, FlatFileFormatSettings formatSettings)
		{
			propertyTable.FromFlatFileFormat(m_PasteContent ?? EditorGUIUtility.systemCopyBuffer, formatSettings);
			m_PasteContent = null;
		}

		private bool IsKeyDownAndModifiers(Event e, KeyCode keyCode, EventModifiers modifiers)
		{
			return e.type == EventType.KeyDown && e.keyCode == keyCode && (e.modifiers & modifiers) != 0;
		}
	}
}
