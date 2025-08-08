using LunaWolfStudiosEditor.ScriptableSheets.Layout;
using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LunaWolfStudiosEditor.ScriptableSheets.Scanning
{
	public class ObjectScanner
	{
		private readonly List<Object> m_Objects = new List<Object>();
		public List<Object> Objects => m_Objects;

		private Type[] m_ObjectTypes;
		public Type[] ObjectTypes => m_ObjectTypes;

		private string[] m_ObjectTypeNames;
		public string[] ObjectTypeNames => m_ObjectTypeNames;

		private string[] m_FriendlyObjectTypeNames;
		public string[] FriendlyObjectTypeNames => m_FriendlyObjectTypeNames;

		public void ScanObjects(ScanSettings settings, SheetAsset sheetAsset)
		{
			m_Objects.Clear();

			// Refresh incase new folders or assets were created.
			AssetDatabase.Refresh();

			if (!AssetDatabase.IsValidFolder(settings.Path))
			{
				settings.Path = UnityConstants.DefaultAssetPath;
			}

			var guids = AssetDatabase.FindAssets($"t:{sheetAsset}", new string[] { settings.Path });
			foreach (string guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
				if (asset != null)
				{
					m_Objects.Add(asset);
				}
			}

			if (sheetAsset == SheetAsset.ScriptableObject && settings.Option == ScanOption.Assembly)
			{
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				var objectTypes = new List<Type>();
				foreach (var assembly in assemblies)
				{
					var types = assembly.GetTypes().Where
					(
						type => type.IsSubclassOf(typeof(ScriptableObject))
							&& type.IsSerializable
							&& !type.IsAbstract
							&& !type.IsGenericType
							&& !type.IsNested
							&& !type.IsSubclassOf(typeof(Editor))
							&& !type.IsSubclassOf(typeof(EditorWindow))
					);
					objectTypes.AddRange(types);
				}
				m_ObjectTypes = objectTypes.OrderBy(type => type.FullName).ToArray();
			}
			else
			{
				m_ObjectTypes = m_Objects.Select(o => o.GetType()).Distinct().OrderBy(type => type.FullName).ToArray();
			}

			m_ObjectTypeNames = m_ObjectTypes.Select(type => type.FullName).ToArray();
			// Submenu once we start having a lot of Objects.
			var separator = '.';
			if (m_ObjectTypeNames.Length > SheetLayout.SubMenuThreshold)
			{
				var newSeparator = '/';
				m_ObjectTypeNames = m_ObjectTypeNames.Select(s => s.Replace(separator, newSeparator)).ToArray();
				separator = newSeparator;
			}
			m_FriendlyObjectTypeNames = m_ObjectTypeNames.Select(s => s.Substring(s.LastIndexOf(separator) + 1)).ToArray();
		}
	}
}
