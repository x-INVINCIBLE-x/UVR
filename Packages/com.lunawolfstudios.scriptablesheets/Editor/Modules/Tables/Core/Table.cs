using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public class Table<T>
	{
		private readonly T[,] m_Data;

		public int Rows { get; private set; }
		public int Columns { get; private set; }
		public int Count => Rows * Columns;

		public Table(int rows, int columns)
		{
			Rows = rows;
			Columns = columns;
			m_Data = new T[rows, columns];
		}

		public T Get(Vector2Int coordinate)
		{
			return Get(coordinate.x, coordinate.y);
		}

		public T Get(int row, int column)
		{
			return m_Data[row, column];
		}

		public void Set(Vector2Int coordinate, T value)
		{
			Set(coordinate.x, coordinate.y, value);
		}

		public void Set(int row, int column, T value)
		{
			m_Data[row, column] = value;
		}

		public bool TryGet(Vector2Int coordinate, out T data)
		{
			return TryGet(coordinate.x, coordinate.y, out data);
		}

		public bool TryGet(int row, int column, out T data)
		{
			data = Get(row, column);
			return data != null;
		}

		public bool IsValidCoordinate(int row, int column)
		{
			return row >= 0 && row < Rows && column >= 0 && column < Columns;
		}

		public bool IsValidCoordinate(Vector2Int coordinate)
		{
			return IsValidCoordinate(coordinate.x, coordinate.y);
		}

		public bool IsValidCoordinate(string formattedCoordinate, out int row, out int column)
		{
			var parts = formattedCoordinate.Split('x');
			if (parts.Length == 2 && int.TryParse(parts[0], out row) && int.TryParse(parts[1], out column))
			{
				return IsValidCoordinate(row, column);
			}
			row = 0;
			column = 0;
			return false;
		}

		public bool IsValidCoordinate(string formattedCoordinate, out Vector2Int coordinate)
		{
			if (IsValidCoordinate(formattedCoordinate, out int row, out int column))
			{
				coordinate = new Vector2Int(row, column);
				return true;
			}
			coordinate = Vector2Int.zero;
			return false;
		}

		public string ToString(int row, int column)
		{
			return $"{row}x{column}";
		}

		public override string ToString()
		{
			return $"{Rows}x{Columns}";
		}
	}
}
