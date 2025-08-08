using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class MultiColumnHeaderUtilityTest
	{
		private TestUtility m_TestUtility;

		private List<TestUtility.TempScriptableObject> m_TempObjects;
		private MultiColumnHeaderState.Column[] m_Columns;
		private MultiColumnHeaderState m_MultiColumnHeaderState;
		private MultiColumnHeader m_MultiColumnHeader;

		[SetUp]
		public void SetUp()
		{
			m_TestUtility = new TestUtility();

			m_TempObjects = new List<TestUtility.TempScriptableObject>();
			var columns = new List<MultiColumnHeaderState.Column>();
			for (var i = 0; i < 5; i++)
			{
				var tempScriptableObject = TestUtility.TempScriptableObject.Create(i, m_TestUtility);
				m_TempObjects.Add(tempScriptableObject);
			}
			var serializedObject = new SerializedObject(m_TempObjects[0]);
			var headerFormat = HeaderFormat.Friendly;
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty(UnityConstants.Field.Name), true, HeaderFormat.Default));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyInt"), true, HeaderFormat.Advanced));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyByte"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyShort"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyLong"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyUInt"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyULong"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyBool"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyFloat"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyDouble"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyChar"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyString"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyColor"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyGameObject"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyLayerMask"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyEnum"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyAnimationCurve"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyGradient"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedInt"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedByte"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedShort"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedLong"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedUInt"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedULong"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedBool"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedFloat"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedDouble"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedChar"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedString"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedColor"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedGameObject"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedLayerMask"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedEnum"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedAnimationCurve"), true, headerFormat));
			columns.Add(ColumnUtility.CreatePropertyColumn(serializedObject.FindProperty("m_MyRootObject.m_MyNestedObject.m_MyNestedGradient"), true, headerFormat));
			m_Columns = columns.ToArray();
			m_MultiColumnHeaderState = new MultiColumnHeaderState(m_Columns);
			m_MultiColumnHeader = new MultiColumnHeader(m_MultiColumnHeaderState);
		}

		[Test]
		public void GetSorted_Ascending_ReturnsSortedAscending()
		{
			ValidateSortingOrder(true);
		}

		[Test]
		public void GetSorted_Descending_ReturnsSortedDescending()
		{
			ValidateSortingOrder(false);
		}

		[Test]
		public void HeaderFormat_ShouldUseCorrectFormat()
		{
			// Default
			Assert.AreEqual(m_Columns[0].headerContent.text, "Name");
			// Advanced
			Assert.AreEqual(m_Columns[1].headerContent.text, "m_MyRootObject.m_MyInt");
			// Friendly
			Assert.AreEqual(m_Columns[2].headerContent.text, "My Root Object My Byte");
		}

		[Test]
		public void ResizeToHeaderWidth_ShouldResizeAllColumnsToHeaderWidth()
		{
			m_MultiColumnHeader.ResizeToHeaderWidth();
			foreach (var column in m_MultiColumnHeader.state.columns)
			{
				var expected = Mathf.Max(EditorStyles.label.CalcSize(column.headerContent).x, column.minWidth);
				Assert.AreEqual(expected, column.width);
			}
		}

		[Test]
		public void ResizeToHeaderWidth_ShouldResizeAllColumnsToHeaderWidthWithPadding()
		{
			var padding = 5;
			m_MultiColumnHeader.ResizeToHeaderWidth(padding);
			foreach (var column in m_MultiColumnHeader.state.columns)
			{
				var expected = Mathf.Max(EditorStyles.label.CalcSize(column.headerContent).x + padding, column.minWidth);
				Assert.AreEqual(expected, column.width);
			}
		}

		[Test]
		public void ResizeToHeaderWidth_ShouldResizeAllColumnsToMinWidthWhenInvalid()
		{
			var padding = -1000;
			m_MultiColumnHeader.ResizeToHeaderWidth(padding);
			foreach (var column in m_MultiColumnHeader.state.columns)
			{
				Assert.AreEqual(column.minWidth, column.width);
			}
		}

		[Test]
		public void ResizeToMinWidth_ShouldResizeAllColumnsToMinWidth()
		{
			m_MultiColumnHeader.ResizeToMinWidth();
			foreach (var column in m_MultiColumnHeader.state.columns)
			{
				Assert.AreEqual(column.minWidth, column.width);
			}
		}

		private void ValidateSortingOrder(bool isAscending)
		{
			var unsorted = m_TempObjects.ToArray();
			var lastIndex = unsorted.Length - 1;

			var sortedFirstIndex = 0;
			var sortedLastIndex = lastIndex;

			if (!isAscending)
			{
				sortedFirstIndex = sortedLastIndex;
				sortedLastIndex = 0;
			}

			var columnIndex = 0;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			var sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual("MyName0", sortedTempObjects[sortedFirstIndex].name);
			Assert.AreEqual("MyName" + lastIndex, sortedTempObjects[sortedLastIndex].name);

			unsorted[0].name = "My Name-0";
			unsorted[1].name = "My Name-1";
			unsorted[2].name = "My Name-2";
			unsorted[3].name = "My Name-10";
			unsorted[4].name = "My Name-11";
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual("My Name-0", sortedTempObjects[sortedFirstIndex].name);
			Assert.AreEqual("My Name-11", sortedTempObjects[sortedLastIndex].name);

			unsorted[0].name = "0_MyName0";
			unsorted[1].name = "1_MyName1";
			unsorted[2].name = "10_MyName2";
			unsorted[3].name = "100_MyName10";
			unsorted[4].name = "100_MyName11";
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual("0_MyName0", sortedTempObjects[sortedFirstIndex].name);
			Assert.AreEqual("100_MyName11", sortedTempObjects[sortedLastIndex].name);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyInt);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyInt);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyByte);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyByte);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyShort);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyShort);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyLong);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyLong);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyUInt);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyUInt);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyULong);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyULong);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(false, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyBool);
			Assert.AreEqual(true, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyBool);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0.0f, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyFloat);
			Assert.AreEqual(4.0f, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyFloat);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0.0d, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyDouble);
			Assert.AreEqual(4.0d, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyDouble);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual('A', sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyChar);
			Assert.AreEqual('E', sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyChar);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual("MyString0", sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyString);
			Assert.AreEqual("MyString" + lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyString);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.Colors[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyColor);
			Assert.AreEqual(m_TestUtility.Colors[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyColor);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.GameObjects[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyGameObject);
			Assert.AreEqual(m_TestUtility.GameObjects[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyGameObject);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.LayerMasks[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyLayerMask);
			Assert.AreEqual(m_TestUtility.LayerMasks[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyLayerMask);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(TestUtility.MyEnum.Value0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyEnum);
			Assert.AreEqual(TestUtility.MyEnum.Value4, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyEnum);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.AnimationCurves[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyAnimationCurve);
			Assert.AreEqual(m_TestUtility.AnimationCurves[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyAnimationCurve);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.Gradients[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyGradient);
			Assert.AreEqual(m_TestUtility.Gradients[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyGradient);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedInt);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedInt);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedByte);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedByte);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedShort);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedShort);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedLong);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedLong);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedUInt);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedUInt);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedULong);
			Assert.AreEqual(lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedULong);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(false, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedBool);
			Assert.AreEqual(true, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedBool);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0.0f, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedFloat);
			Assert.AreEqual(4.0f, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedFloat);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(0.0d, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedDouble);
			Assert.AreEqual(4.0d, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedDouble);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual('A', sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedChar);
			Assert.AreEqual('E', sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedChar);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual("MyNestedString0", sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedString);
			Assert.AreEqual("MyNestedString" + lastIndex, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedString);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.Colors[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedColor);
			Assert.AreEqual(m_TestUtility.Colors[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedColor);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.GameObjects[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedGameObject);
			Assert.AreEqual(m_TestUtility.GameObjects[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedGameObject);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.LayerMasks[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedLayerMask);
			Assert.AreEqual(m_TestUtility.LayerMasks[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedLayerMask);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(TestUtility.MyEnum.Value0, sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedEnum);
			Assert.AreEqual(TestUtility.MyEnum.Value4, sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedEnum);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.AnimationCurves[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedAnimationCurve);
			Assert.AreEqual(m_TestUtility.AnimationCurves[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedAnimationCurve);

			columnIndex++;
			m_MultiColumnHeader.sortedColumnIndex = columnIndex;
			m_MultiColumnHeader.SetSortDirection(columnIndex, isAscending);
			sortedTempObjects = m_MultiColumnHeader.GetSorted(unsorted);
			Assert.AreEqual(m_TestUtility.Gradients[0], sortedTempObjects[sortedFirstIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedGradient);
			Assert.AreEqual(m_TestUtility.Gradients[4], sortedTempObjects[sortedLastIndex].m_MyRootObject.m_MyNestedObject.m_MyNestedGradient);
		}
	}
}
