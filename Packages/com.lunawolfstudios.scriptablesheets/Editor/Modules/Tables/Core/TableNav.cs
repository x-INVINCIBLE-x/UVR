using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public class TableNav<T> where T : ITableProperty
	{
		private const char ControlNameDelimiter = '-';

		public Vector2Int FocusedCoordinate { get; private set; }
		public Vector2Int PreviousFocusedCoordinate { get; private set; }
		public bool FocusChanged { get; private set; }
		public bool HasFocus { get; private set; }
		public bool WasKeyboardNav { get; private set; }
		public bool IsEditingTextField { get; private set; }

		/// <summary>
		/// Attempts to find a coordinate within a specified property table using the name of the current focused control.
		/// </summary>
		/// <returns>True if the coordinate is parsed successfully and within the specified property table bounds.</returns>
		public bool UpdateFocusedCoordinate(Table<T> propertyTable, bool isScriptableObject)
		{
			PreviousFocusedCoordinate = FocusedCoordinate;
			FocusChanged = false;
			HasFocus = false;
			WasKeyboardNav = false;
			if (propertyTable != null)
			{
				var focusedName = GUI.GetNameOfFocusedControl();
				if (!string.IsNullOrEmpty(focusedName))
				{
					var formattedCoordinate = focusedName.Split(ControlNameDelimiter)[0];
					if (propertyTable.IsValidCoordinate(formattedCoordinate, out Vector2Int coordinate))
					{
						var e = Event.current;
						if (e.type == EventType.KeyDown)
						{
							// Workaround to ensure we consume tab events when editing text fields.
							// https://forum.unity.com/threads/how-to-disable-the-tab-key-event.90440/#post-586383
							if (e.character == '\t')
							{
								e.Use();
							}
							var newCoordinate = coordinate;
							switch (e.keyCode)
							{
								case KeyCode.Escape:
									IsEditingTextField = false;
									break;

								case KeyCode.KeypadEnter:
								case KeyCode.Return:
									if (propertyTable.TryGet(coordinate, out T property))
									{
										if (property.IsInputFieldProperty(isScriptableObject))
										{
											IsEditingTextField = !IsEditingTextField;
											if (!IsEditingTextField)
											{
												newCoordinate.x += e.shift ? -1 : 1;
											}
										}
									}
									break;

								case KeyCode.Tab:
									newCoordinate.y += e.shift ? -1 : 1;
									e.Use();
									break;
							}
							if (!IsEditingTextField)
							{
								switch (e.keyCode)
								{
									case KeyCode.LeftArrow:
										newCoordinate.y--;
										break;

									case KeyCode.RightArrow:
										newCoordinate.y++;
										break;

									case KeyCode.UpArrow:
										newCoordinate.x--;
										break;

									case KeyCode.DownArrow:
										newCoordinate.x++;
										break;

									default:
										break;
								}
							}
							if (newCoordinate != coordinate && propertyTable.IsValidCoordinate(newCoordinate))
							{
								if (propertyTable.TryGet(newCoordinate, out T property))
								{
									var controlName = property.ControlName;
									GUI.FocusControl(controlName);
									coordinate = newCoordinate;
									WasKeyboardNav = true;
								}
							}
						}
						if (FocusedCoordinate != coordinate)
						{
							IsEditingTextField = false;
							FocusChanged = true;
							FocusedCoordinate = coordinate;
						}
						if (IsEditingTextField && !EditorGUIUtility.editingTextField)
						{
							// Force text field highlighting when we're editing text.
							if (propertyTable.TryGet(FocusedCoordinate, out T property))
							{
								EditorGUIUtility.editingTextField = true;
								EditorGUI.FocusTextInControl(property.ControlName);
							}
						}
						HasFocus = true;
						return true;
					}
					else
					{
						// Reset the focus control if it's an invalid coordinate.
						GUIUtility.keyboardControl = 0;
						GUI.FocusControl("null");
					}
				}
			}
			IsEditingTextField = false;
			FocusedCoordinate = Vector2Int.zero;
			return false;
		}

		public void ResetTextFieldEditing()
		{
			IsEditingTextField = false;
			EditorGUIUtility.editingTextField = false;
		}

		public void UpdateFocusVisuals(Table<T> propertyTable, TableNavSettings settings, TableNavVisualState visualState, ref Vector2 scrollPosition, bool lockNames = false)
		{
			if (propertyTable.IsValidCoordinate(FocusedCoordinate) && HasFocus)
			{
				var needsSelectionBorder = false;
				if (propertyTable.TryGet(FocusedCoordinate, out T property))
				{
					if (settings.AutoSelect && FocusChanged)
					{
						Selection.activeObject = property.RootObject;
					}
					needsSelectionBorder = property.NeedsSelectionBorder(lockNames);
				}
				var selectionColor = GUI.skin.settings.selectionColor;
				var highlightColor = selectionColor;
				highlightColor.a = settings.HighlightAlpha;
				var focusedRowY = visualState.RowHeight * (FocusedCoordinate.x + 1);
				if (settings.HighlightSelectedRow && propertyTable.Rows > 1)
				{
					var rowRect = new Rect(visualState.ColumnHeaderRowRect);
					rowRect.width += scrollPosition.x;
					rowRect.y += focusedRowY;
					EditorGUI.DrawRect(rowRect, highlightColor);
				}
				var focusedColumnRect = visualState.MultiColumnHeader.GetColumnRect(FocusedCoordinate.y);
				if (settings.HighlightSelectedColumn && propertyTable.Columns > 2)
				{
					focusedColumnRect.y = visualState.ColumnHeaderRowRect.y;
					focusedColumnRect.height += visualState.RowHeight * propertyTable.Rows;
					EditorGUI.DrawRect(focusedColumnRect, highlightColor);
				}
				var focusedCellRect = visualState.MultiColumnHeader.GetCellRect(FocusedCoordinate.y, new Rect(visualState.ColumnHeaderRowRect));
				focusedCellRect.y += focusedRowY;
				if (IsEditingTextField)
				{
					var borderThickness = 2;
					var borderColor = selectionColor;
					borderColor.b = 1.0f;
					borderColor.a = 1.0f;
					DrawBorder(focusedCellRect, borderThickness, borderColor);
				}
				else if (needsSelectionBorder)
				{
					var borderThickness = 1;
					var borderColor = selectionColor;
					borderColor.a = 1.0f;
					DrawBorder(focusedCellRect, borderThickness, borderColor);
				}
				if (settings.AutoScroll && WasKeyboardNav)
				{
					var focusedCellEndX = focusedCellRect.x + focusedCellRect.width;
					var focusedCellEndY = focusedCellRect.y + focusedCellRect.height;

					// Scrolling left.
					if (focusedCellRect.x < visualState.ScrollStart.x)
					{
						// Special case to show the Actions column when back on the first column.
						if (FocusedCoordinate.y > 1)
						{
							scrollPosition.x = focusedColumnRect.x - visualState.ScrollViewArea.x;
						}
						else
						{
							scrollPosition.x = 0;
						}
					}
					// Scrolling right.
					else if (focusedCellEndX >= visualState.ScrollEnd.x)
					{
						scrollPosition.x = focusedCellEndX - visualState.ScrollViewArea.width - visualState.ScrollViewArea.x;
						// Special case to scroll to the very right on the last column.
						if (FocusedCoordinate.x >= propertyTable.Columns - 1)
						{
							// Adding row height is not ideal, but Unity doesn't provide a way to get the absolute maximum scroll position X.
							scrollPosition.x += visualState.RowHeight;
						}
					}
					// Scrolling up.
					if (focusedCellRect.y < visualState.ScrollStart.y)
					{
						if (FocusedCoordinate.x > 0)
						{
							scrollPosition.y = focusedCellRect.y - visualState.ScrollViewArea.y;
						}
						else
						{
							scrollPosition.y = 0;
						}
					}
					// Scrolling down.
					else if (focusedCellEndY >= visualState.ScrollEnd.y)
					{
						scrollPosition.y = focusedCellEndY - visualState.ScrollViewArea.height - visualState.ScrollViewArea.y + visualState.RowHeight;
						// Special case to scroll to the very bottom on the last row.
						if (FocusedCoordinate.x >= propertyTable.Rows - 1)
						{
							scrollPosition.y += visualState.RowHeight;
						}
					}
				}
			}
		}

		public string SetNextControlName(Table<T> propertyTable, int row, int column)
		{
			// Ensure a unique control name by assigning a control id.
			// https://forum.unity.com/threads/gui-getnameoffocusedcontrol-is-not-working-correctly.165143/
			var controlName = propertyTable.ToString(row, column) + ControlNameDelimiter + GUIUtility.GetControlID(FocusType.Keyboard).ToString();
			GUI.SetNextControlName(controlName);
			return controlName;
		}

		private static void DrawBorder(Rect rect, int thickness, Color color)
		{
			var halfThickness = thickness * 0.5f;
			// Top
			EditorGUI.DrawRect(new Rect(rect.x - halfThickness, rect.y - halfThickness, rect.width + thickness, thickness), color);
			// Bottom
			EditorGUI.DrawRect(new Rect(rect.x - halfThickness, rect.yMax - halfThickness, rect.width + thickness, thickness), color);
			// Left
			EditorGUI.DrawRect(new Rect(rect.x - halfThickness, rect.y - halfThickness, thickness, rect.height + thickness), color);
			// Right
			EditorGUI.DrawRect(new Rect(rect.xMax - halfThickness, rect.y - halfThickness, thickness, rect.height + thickness), color);
		}
	}
}
