using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using NUnit.Framework;
using UnityEditor.IMGUI.Controls;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class ColumnUtilityTests
	{
		[Test]
		public void GetClampedColumns_Returns_AllColumns_When_ColumnsCount_LessThan_MaxColumns()
		{
			var columns = new MultiColumnHeaderState.Column[]
			{
				new MultiColumnHeaderState.Column(),
				new MultiColumnHeaderState.Column(),
				new MultiColumnHeaderState.Column()
			};
			var maxColumns = 5;
			var result = columns.GetClampedColumns(maxColumns);
			Assert.AreEqual(columns.Length, result.Length);
			for (var i = 0; i < columns.Length; i++)
			{
				Assert.AreEqual(i, result[i]);
			}
		}

		[Test]
		public void GetClampedColumns_Returns_MaxColumns_When_ColumnsCount_GreaterThan_MaxColumns()
		{
			var columns = new MultiColumnHeaderState.Column[]
			{
				new MultiColumnHeaderState.Column(),
				new MultiColumnHeaderState.Column(),
				new MultiColumnHeaderState.Column(),
				new MultiColumnHeaderState.Column(),
				new MultiColumnHeaderState.Column()
			};
			var maxColumns = 3;
			var result = columns.GetClampedColumns(maxColumns);
			Assert.AreEqual(maxColumns, result.Length);
			for (var i = 0; i < maxColumns; i++)
			{
				Assert.AreEqual(i, result[i]);
			}
		}
	}
}
