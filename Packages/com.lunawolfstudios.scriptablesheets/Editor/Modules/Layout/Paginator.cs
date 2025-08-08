using System.Collections.Generic;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Layout
{
	public class Paginator
	{
		public int TotalObjects { get; private set; }
		public int ObjectsPerPage { get; private set; }
		public int CurrentPage { get; private set; }

		public Paginator()
		{
			CurrentPage = 1;
		}

		public Paginator(int objectsPerPage, int totalObjects)
		{
			CurrentPage = 1;
			ObjectsPerPage = objectsPerPage;
			TotalObjects = totalObjects;
		}

		public List<T> GetPageObjects<T>(List<T> objects)
		{
			var startIndex = (CurrentPage - 1) * ObjectsPerPage;
			var count = Mathf.Min(ObjectsPerPage, objects.Count - startIndex);
			return objects.GetRange(startIndex, count);
		}

		public int GetTotalPages()
		{
			return (TotalObjects + ObjectsPerPage - 1) / ObjectsPerPage;
		}

		public void GoToFirstPage()
		{
			CurrentPage = 1;
		}

		public void GoToLastPage()
		{
			CurrentPage = GetTotalPages();
		}

		public bool IsOnFirstPage()
		{
			return CurrentPage == 1;
		}

		public bool IsOnLastPage()
		{
			return CurrentPage == GetTotalPages();
		}

		public void NextPage()
		{
			CurrentPage++;
			if (CurrentPage > GetTotalPages())
			{
				GoToFirstPage();
			}
		}

		public void PreviousPage()
		{
			CurrentPage--;
			if (CurrentPage < 1)
			{
				GoToLastPage();
			}
		}

		public void SetTotalObjects(int totalObjects)
		{
			if (TotalObjects != totalObjects)
			{
				TotalObjects = totalObjects;
				if (CurrentPage > GetTotalPages())
				{
					CurrentPage = 1;
				}
			}
		}

		public void SetObjectsPerPage(int objectsPerPage)
		{
			if (ObjectsPerPage != objectsPerPage)
			{
				ObjectsPerPage = objectsPerPage;
				if (CurrentPage > GetTotalPages())
				{
					CurrentPage = 1;
				}
			}
		}
	}
}
