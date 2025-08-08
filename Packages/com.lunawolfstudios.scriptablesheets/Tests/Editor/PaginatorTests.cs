using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using NUnit.Framework;
using System.Collections.Generic;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class PaginatorTests
	{
		[Test]
		public void GetPageObjects_ShouldReturnCorrectPageObjects()
		{
			var paginator = new Paginator(3, 10);
			var expectedValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

			var pageObjects = paginator.GetPageObjects(expectedValues);
			Assert.AreEqual(3, pageObjects.Count);
			Assert.AreEqual(1, pageObjects[0]);
			Assert.AreEqual(2, pageObjects[1]);
			Assert.AreEqual(3, pageObjects[2]);

			paginator.NextPage();
			pageObjects = paginator.GetPageObjects(expectedValues);
			Assert.AreEqual(3, pageObjects.Count);
			Assert.AreEqual(4, pageObjects[0]);
			Assert.AreEqual(5, pageObjects[1]);
			Assert.AreEqual(6, pageObjects[2]);

			paginator.NextPage();
			pageObjects = paginator.GetPageObjects(expectedValues);
			Assert.AreEqual(3, pageObjects.Count);
			Assert.AreEqual(7, pageObjects[0]);
			Assert.AreEqual(8, pageObjects[1]);
			Assert.AreEqual(9, pageObjects[2]);

			paginator.NextPage();
			pageObjects = paginator.GetPageObjects(expectedValues);
			Assert.AreEqual(1, pageObjects.Count);
			Assert.AreEqual(10, pageObjects[0]);

			paginator.NextPage();
			pageObjects = paginator.GetPageObjects(expectedValues);
			Assert.AreEqual(3, pageObjects.Count);
			Assert.AreEqual(1, pageObjects[0]);
			Assert.AreEqual(2, pageObjects[1]);
			Assert.AreEqual(3, pageObjects[2]);

			paginator.PreviousPage();
			pageObjects = paginator.GetPageObjects(expectedValues);
			Assert.AreEqual(1, pageObjects.Count);
			Assert.AreEqual(10, pageObjects[0]);
		}

		[Test]
		public void NextPage_ShouldIncrementCurrentPage()
		{
			var paginator = new Paginator(5, 20);
			paginator.NextPage();
			Assert.AreEqual(2, paginator.CurrentPage);
			paginator.NextPage();
			Assert.AreEqual(3, paginator.CurrentPage);
			paginator.NextPage();
			Assert.AreEqual(4, paginator.CurrentPage);
		}

		[Test]
		public void NextPage_ShouldResetToFirstPageIfCurrentPageExceedsTotalPages()
		{
			var paginator = new Paginator(5, 20);
			for (var i = 0; i < 4; i++)
			{
				paginator.NextPage();
			}
			Assert.AreEqual(1, paginator.CurrentPage);
		}

		[Test]
		public void PreviousPage_ShouldDecrementCurrentPage()
		{
			var paginator = new Paginator(5, 20);
			paginator.NextPage();
			paginator.NextPage();
			paginator.NextPage();
			Assert.AreEqual(4, paginator.CurrentPage);
			paginator.PreviousPage();
			Assert.AreEqual(3, paginator.CurrentPage);
			paginator.PreviousPage();
			Assert.AreEqual(2, paginator.CurrentPage);
			paginator.PreviousPage();
			Assert.AreEqual(1, paginator.CurrentPage);
		}

		[Test]
		public void PreviousPage_ShouldSetToLastPageIfCurrentPageBecomesLessThanOne()
		{
			var paginator = new Paginator(5, 20);
			paginator.PreviousPage();
			Assert.AreEqual(paginator.GetTotalPages(), paginator.CurrentPage);
		}

		[Test]
		public void SetTotalObjects_ShouldResetCurrentPageIfExceedsTotalPages()
		{
			var paginator = new Paginator(5, 20);
			paginator.NextPage();
			paginator.SetTotalObjects(10);
			Assert.AreEqual(2, paginator.CurrentPage);
			paginator.SetTotalObjects(5);
			Assert.AreEqual(1, paginator.CurrentPage);
		}

		[Test]
		public void SetTotalObjects_ShouldUpdateTotalObjects()
		{
			var paginator = new Paginator(5, 20);
			paginator.SetTotalObjects(25);
			Assert.AreEqual(25, paginator.TotalObjects);
		}
	}
}
