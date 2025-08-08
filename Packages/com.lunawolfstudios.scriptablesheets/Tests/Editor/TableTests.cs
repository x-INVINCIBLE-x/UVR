using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using NUnit.Framework;
using System;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class TableTests
	{
		[Test]
		public void SetAndGetElement()
		{
			var table = new Table<int>(3, 3);
			var index = new Vector2Int(1, 1);
			var value = 42;
			table.Set(index, value);
			var retrievedValue = table.Get(index);
			Assert.AreEqual(value, retrievedValue);
		}

		[Test]
		public void SetInvalidIndex_ThrowsException()
		{
			var table = new Table<string>(2, 2);
			var index = new Vector2Int(3, 3);
			var value = "Test";
			Assert.Throws<IndexOutOfRangeException>(() => table.Set(index, value));
		}

		[Test]
		public void GetInvalidIndex_ThrowsException()
		{
			var table = new Table<string>(2, 2);
			var index = new Vector2Int(3, 3);
			Assert.Throws<IndexOutOfRangeException>(() => table.Get(index));
		}

		[Test]
		public void ToString_ReturnsCorrectFormat()
		{
			var table = new Table<int>(5, 6);
			var expectedFormat = "5x6";
			var tableFormat = table.ToString();
			Assert.AreEqual(expectedFormat, tableFormat);
		}

		[TestCase(0, 0, ExpectedResult = true)]
		[TestCase(2, 3, ExpectedResult = true)]
		[TestCase(-1, 0, ExpectedResult = false)]
		[TestCase(0, -1, ExpectedResult = false)]
		[TestCase(3, 4, ExpectedResult = false)]
		public bool IsValidCoordinate_Int(int row, int column)
		{
			var table = new Table<int>(3, 4);
			var result = table.IsValidCoordinate(row, column);
			return result;
		}

		[TestCase(0, 0, ExpectedResult = true)]
		[TestCase(2, 3, ExpectedResult = true)]
		[TestCase(-1, 0, ExpectedResult = false)]
		[TestCase(0, -1, ExpectedResult = false)]
		[TestCase(3, 4, ExpectedResult = false)]
		public bool IsValidCoordinate_Vector2Int(int row, int column)
		{
			var table = new Table<int>(3, 4);
			var result = table.IsValidCoordinate(new Vector2Int(row, column));
			return result;
		}

		[TestCase("0x0", ExpectedResult = true)]
		[TestCase("2x3", ExpectedResult = true)]
		[TestCase("-1x0", ExpectedResult = false)]
		[TestCase("0x-1", ExpectedResult = false)]
		[TestCase("3x4", ExpectedResult = false)]
		[TestCase("axb", ExpectedResult = false)]
		public bool IsValidCoordinate_String(string formattedCoordinate)
		{
			var table = new Table<int>(3, 4);
			var result = table.IsValidCoordinate(formattedCoordinate, out Vector2Int coordinate);
			if (!result)
			{
				Assert.AreEqual(Vector2Int.zero, coordinate);
			}
			return result;
		}
	}
}
