using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	public class ReflectionUtilityTests
	{
		[Test]
		public void GetNestedFieldInfo_SimpleField_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(BaseClass), "m_BaseField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(1, fieldInfo.Length);
			Assert.AreEqual("m_BaseField", fieldInfo[0].Name);
			Assert.AreEqual(typeof(int), fieldInfo[0].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_GenericField_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(GenericSprite), "m_BaseField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(1, fieldInfo.Length);
			Assert.AreEqual("m_BaseField", fieldInfo[0].Name);
			Assert.AreEqual(typeof(int), fieldInfo[0].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(GenericSprite), "m_FieldName");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(1, fieldInfo.Length);
			Assert.AreEqual("m_FieldName", fieldInfo[0].Name);
			Assert.AreEqual(typeof(string), fieldInfo[0].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(GenericSprite), "m_GenericField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(1, fieldInfo.Length);
			Assert.AreEqual("m_GenericField", fieldInfo[0].Name);
			Assert.AreEqual(typeof(Sprite), fieldInfo[0].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(GenericSprite), "m_Count");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(1, fieldInfo.Length);
			Assert.AreEqual("m_Count", fieldInfo[0].Name);
			Assert.AreEqual(typeof(int), fieldInfo[0].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_NestedGenericField_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(NestedGenericSprite), "m_NestedGenericSprite.m_BaseField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_NestedGenericSprite", fieldInfo[0].Name);
			Assert.AreEqual(typeof(GenericSprite), fieldInfo[0].FieldType);
			Assert.AreEqual("m_BaseField", fieldInfo[1].Name);
			Assert.AreEqual(typeof(int), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(NestedGenericSprite), "m_NestedGenericSprite.m_FieldName");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_NestedGenericSprite", fieldInfo[0].Name);
			Assert.AreEqual(typeof(GenericSprite), fieldInfo[0].FieldType);
			Assert.AreEqual("m_FieldName", fieldInfo[1].Name);
			Assert.AreEqual(typeof(string), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(NestedGenericSprite), "m_NestedGenericSprite.m_GenericField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_NestedGenericSprite", fieldInfo[0].Name);
			Assert.AreEqual(typeof(GenericSprite), fieldInfo[0].FieldType);
			Assert.AreEqual("m_GenericField", fieldInfo[1].Name);
			Assert.AreEqual(typeof(Sprite), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(NestedGenericSprite), "m_NestedGenericSprite.m_Count");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_NestedGenericSprite", fieldInfo[0].Name);
			Assert.AreEqual(typeof(GenericSprite), fieldInfo[0].FieldType);
			Assert.AreEqual("m_Count", fieldInfo[1].Name);
			Assert.AreEqual(typeof(int), fieldInfo[1].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_ExposedReference_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(MyExposedReference), "m_ExposedReference.exposedName");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_ExposedReference", fieldInfo[0].Name);
			Assert.AreEqual(typeof(ExposedReference<MyScriptableObject>), fieldInfo[0].FieldType);
			Assert.AreEqual("exposedName", fieldInfo[1].Name);
			Assert.AreEqual(typeof(PropertyName), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(MyExposedReference), "m_ExposedReference.defaultValue");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_ExposedReference", fieldInfo[0].Name);
			Assert.AreEqual(typeof(ExposedReference<MyScriptableObject>), fieldInfo[0].FieldType);
			Assert.AreEqual("defaultValue", fieldInfo[1].Name);
			Assert.AreEqual(typeof(Object), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(MyExposedReference), "m_ExposedReference.m_Count");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_ExposedReference", fieldInfo[0].Name);
			Assert.AreEqual(typeof(ExposedReference<MyScriptableObject>), fieldInfo[0].FieldType);
			Assert.AreEqual("m_Count", fieldInfo[1].Name);
			Assert.AreEqual(typeof(int), fieldInfo[1].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_DerivedExposedReference_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedExposedReference), "m_ExposedReference.exposedName");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_ExposedReference", fieldInfo[0].Name);
			Assert.AreEqual(typeof(ExposedReference<MyScriptableObject>), fieldInfo[0].FieldType);
			Assert.AreEqual("exposedName", fieldInfo[1].Name);
			Assert.AreEqual(typeof(PropertyName), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedExposedReference), "m_ExposedReference.defaultValue");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_ExposedReference", fieldInfo[0].Name);
			Assert.AreEqual(typeof(ExposedReference<MyScriptableObject>), fieldInfo[0].FieldType);
			Assert.AreEqual("defaultValue", fieldInfo[1].Name);
			Assert.AreEqual(typeof(Object), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedExposedReference), "m_DerivedExposedReference.exposedName");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_DerivedExposedReference", fieldInfo[0].Name);
			Assert.AreEqual(typeof(ExposedReference<MyScriptableObject>), fieldInfo[0].FieldType);
			Assert.AreEqual("exposedName", fieldInfo[1].Name);
			Assert.AreEqual(typeof(PropertyName), fieldInfo[1].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedExposedReference), "m_DerivedExposedReference.defaultValue");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(2, fieldInfo.Length);
			Assert.AreEqual("m_DerivedExposedReference", fieldInfo[0].Name);
			Assert.AreEqual(typeof(ExposedReference<MyScriptableObject>), fieldInfo[0].FieldType);
			Assert.AreEqual("defaultValue", fieldInfo[1].Name);
			Assert.AreEqual(typeof(Object), fieldInfo[1].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_InheritedField_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedClass), "m_BaseField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(1, fieldInfo.Length);
			Assert.AreEqual("m_BaseField", fieldInfo[0].Name);
			Assert.AreEqual(typeof(int), fieldInfo[0].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedClass), "m_DerivedField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(1, fieldInfo.Length);
			Assert.AreEqual("m_DerivedField", fieldInfo[0].Name);
			Assert.AreEqual(typeof(string), fieldInfo[0].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_NestedField_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(Item), "m_Consumables.Array.data[0].m_Icon.m_Name");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(3, fieldInfo.Length);
			Assert.AreEqual("m_Consumables", fieldInfo[0].Name);
			Assert.AreEqual("m_Icon", fieldInfo[1].Name);
			Assert.AreEqual("m_Name", fieldInfo[2].Name);
			Assert.AreEqual(typeof(string), fieldInfo[2].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(Item), "m_Consumables.Array.data[0].m_Icon.m_Icon");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(3, fieldInfo.Length);
			Assert.AreEqual("m_Consumables", fieldInfo[0].Name);
			Assert.AreEqual("m_Icon", fieldInfo[1].Name);
			Assert.AreEqual("m_Icon", fieldInfo[2].Name);
			Assert.AreEqual(typeof(Sprite), fieldInfo[2].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_NestedArrayField_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedClass), "m_Array.m_NestedArray.Array.data[0].m_InnerNestedArray.Array.data[0].m_InnerField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(4, fieldInfo.Length);
			Assert.AreEqual("m_Array", fieldInfo[0].Name);
			Assert.AreEqual("m_NestedArray", fieldInfo[1].Name);
			Assert.AreEqual("m_InnerNestedArray", fieldInfo[2].Name);
			Assert.AreEqual("m_InnerField", fieldInfo[3].Name);
			Assert.AreEqual(typeof(string), fieldInfo[3].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedClass), "m_Array.m_NestedArray.Array.data[0].m_InnerNestedArray.Array.data[0].m_DerivedInnerField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(4, fieldInfo.Length);
			Assert.AreEqual("m_Array", fieldInfo[0].Name);
			Assert.AreEqual("m_NestedArray", fieldInfo[1].Name);
			Assert.AreEqual("m_InnerNestedArray", fieldInfo[2].Name);
			Assert.AreEqual("m_DerivedInnerField", fieldInfo[3].Name);
			Assert.AreEqual(typeof(Sprite), fieldInfo[3].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_NestedListField_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedClass), "m_List.m_NestedList.Array.data[0].m_InnerNestedList.Array.data[0].m_InnerField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(4, fieldInfo.Length);
			Assert.AreEqual("m_List", fieldInfo[0].Name);
			Assert.AreEqual("m_NestedList", fieldInfo[1].Name);
			Assert.AreEqual("m_InnerNestedList", fieldInfo[2].Name);
			Assert.AreEqual("m_InnerField", fieldInfo[3].Name);
			Assert.AreEqual(typeof(string), fieldInfo[3].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(DerivedClass), "m_List.m_NestedList.Array.data[0].m_InnerNestedList.Array.data[0].m_DerivedInnerField");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(4, fieldInfo.Length);
			Assert.AreEqual("m_List", fieldInfo[0].Name);
			Assert.AreEqual("m_NestedList", fieldInfo[1].Name);
			Assert.AreEqual("m_InnerNestedList", fieldInfo[2].Name);
			Assert.AreEqual("m_DerivedInnerField", fieldInfo[3].Name);
			Assert.AreEqual(typeof(Sprite), fieldInfo[3].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_WhenDeeplyNestedAndWrapped_ReturnsFieldInfo()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(Wrapper3), "m_Wrapper2.m_Wrapper1.m_Wrapper0.m_Depth3.m_Depth3");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(5, fieldInfo.Length);
			Assert.AreEqual("m_Wrapper2", fieldInfo[0].Name);
			Assert.AreEqual("m_Wrapper1", fieldInfo[1].Name);
			Assert.AreEqual("m_Wrapper0", fieldInfo[2].Name);
			Assert.AreEqual("m_Depth3", fieldInfo[3].Name);
			Assert.AreEqual("m_Depth3", fieldInfo[4].Name);
			Assert.AreEqual(typeof(int), fieldInfo[4].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(Wrapper3), "m_Wrapper2.m_Wrapper1.m_Wrapper0.m_Depth3.m_Depth2");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(5, fieldInfo.Length);
			Assert.AreEqual("m_Wrapper2", fieldInfo[0].Name);
			Assert.AreEqual("m_Wrapper1", fieldInfo[1].Name);
			Assert.AreEqual("m_Wrapper0", fieldInfo[2].Name);
			Assert.AreEqual("m_Depth3", fieldInfo[3].Name);
			Assert.AreEqual("m_Depth2", fieldInfo[4].Name);
			Assert.AreEqual(typeof(string), fieldInfo[4].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(Wrapper3), "m_Wrapper2.m_Wrapper1.m_Wrapper0.m_Depth3.m_Depth1");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(5, fieldInfo.Length);
			Assert.AreEqual("m_Wrapper2", fieldInfo[0].Name);
			Assert.AreEqual("m_Wrapper1", fieldInfo[1].Name);
			Assert.AreEqual("m_Wrapper0", fieldInfo[2].Name);
			Assert.AreEqual("m_Depth3", fieldInfo[3].Name);
			Assert.AreEqual("m_Depth1", fieldInfo[4].Name);
			Assert.AreEqual(typeof(bool), fieldInfo[4].FieldType);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(Wrapper3), "m_Wrapper2.m_Wrapper1.m_Wrapper0.m_Depth3.m_Depth0");
			Assert.NotNull(fieldInfo);
			Assert.AreEqual(5, fieldInfo.Length);
			Assert.AreEqual("m_Wrapper2", fieldInfo[0].Name);
			Assert.AreEqual("m_Wrapper1", fieldInfo[1].Name);
			Assert.AreEqual("m_Wrapper0", fieldInfo[2].Name);
			Assert.AreEqual("m_Depth3", fieldInfo[3].Name);
			Assert.AreEqual("m_Depth0", fieldInfo[4].Name);
			Assert.AreEqual(typeof(Sprite), fieldInfo[4].FieldType);
		}

		[Test]
		public void GetNestedFieldInfo_WhenInvalidField_ReturnsNull()
		{
			var fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(BaseClass), "m_InvalidFieldPath");
			Assert.IsNull(fieldInfo);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(BaseClass), "m_Invalid.Field.Path");
			Assert.IsNull(fieldInfo);

			fieldInfo = ReflectionUtility.GetNestedFieldInfo(typeof(Item), "m_Consumables.Array.data[0].m_Icon.m_Name.m_Value");
			Assert.IsNull(fieldInfo);
		}

		[Test]
		public void GetNestedFieldType_SimpleField_ReturnsFieldType()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(BaseClass), "m_BaseField");
			Assert.AreEqual(typeof(int), fieldType);
		}

		[Test]
		public void GetNestedFieldType_InheritedField_ReturnsFieldType()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_BaseField");
			Assert.AreEqual(typeof(int), fieldType);

			fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_DerivedField");
			Assert.AreEqual(typeof(string), fieldType);
		}

		[Test]
		public void GetNestedFieldType_ArrayField_ReturnsFieldType()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(BaseClass), "m_SpriteArray");
			Assert.AreEqual(typeof(Sprite), fieldType);
		}

		[Test]
		public void GetNestedFieldType_ListField_ReturnsFieldType()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(BaseClass), "m_MaterialList");
			Assert.AreEqual(typeof(Material), fieldType);
		}

		[Test]
		public void GetNestedFieldType_InnerArrayField_ReturnsFieldType()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(Item), "m_Consumables.Array.data[0].m_Icon.m_Name");
			Assert.AreEqual(typeof(string), fieldType);

			fieldType = ReflectionUtility.GetNestedFieldType(typeof(Item), "m_Consumables.Array.data[0].m_Icon.m_Icon");
			Assert.AreEqual(typeof(Sprite), fieldType);
		}

		[Test]
		public void GetNestedFieldType_NestedArrayField_ReturnsFieldType()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_Array.m_NestedArray.Array.data[0].m_InnerNestedArray.Array.data[0].m_InnerField");
			Assert.AreEqual(typeof(string), fieldType);

			fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_Array.m_NestedArray.Array.data[0].m_InnerNestedArray.Array.data[0].m_DerivedInnerField");
			Assert.AreEqual(typeof(Sprite), fieldType);
		}

		[Test]
		public void GetNestedFieldType_NestedCollections_ReturnsFieldType()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_Array.m_NestedArray.Array.data[0].m_InnerNestedArray.Array.data[0].m_InnerSpriteArray");
			Assert.AreEqual(typeof(Sprite), fieldType);

			fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_Array.m_NestedArray.Array.data[0].m_InnerNestedArray.Array.data[0].m_InnerMaterialList");
			Assert.AreEqual(typeof(Material), fieldType);
		}

		[Test]
		public void GetNestedFieldType_WhenInvalidField_ReturnsNull()
		{
			var fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_InvalidFieldPath");
			Assert.IsNull(fieldType);

			fieldType = ReflectionUtility.GetNestedFieldType(typeof(DerivedClass), "m_Consumables.Array.data[0].m_Icon.m_Name.m_Value");
			Assert.IsNull(fieldType);
		}

		private struct Item
		{
			private Consumable[] m_Consumables;
		}

		private struct Consumable
		{
			private Icon m_Icon;
		}

		private struct Icon
		{
			private string m_Name;
			private Sprite m_Icon;
		}

		private class BaseClass
		{
			private int m_BaseField;
			private ArrayClass m_Array;
			private ListClass m_List;
			private Sprite[] m_SpriteArray;
			private List<Material> m_MaterialList;
		}

		private class DerivedClass : BaseClass
		{
			private string m_DerivedField;
		}

		private class BaseGeneric : BaseClass
		{
			private string m_FieldName;
		}

		private class Generic<T> : BaseGeneric
		{
			private T m_GenericField;
		}

		private class GenericSprite : Generic<Sprite>
		{
			private int m_Count;
		}

		private class NestedGenericSprite
		{
			private GenericSprite m_NestedGenericSprite;
		}

		private class MyScriptableObject : ScriptableObject
		{
			private int m_Count;
		}

		private class MyExposedReference
		{
			private ExposedReference<MyScriptableObject> m_ExposedReference;
		}

		private class DerivedExposedReference : MyExposedReference
		{
			private ExposedReference<MyScriptableObject> m_DerivedExposedReference;
		}

		private class ArrayClass
		{
			private NestedArrayClass[] m_NestedArray;
		}

		private class NestedArrayClass
		{
			private DerivedInnerClass[] m_InnerNestedArray;
		}

		private class ListClass
		{
			private List<NestedListClass> m_NestedList;
		}

		private class NestedListClass
		{
			private List<DerivedInnerClass> m_InnerNestedList;
		}

		private class InnerClass
		{
			private string m_InnerField;
			private Sprite[] m_InnerSpriteArray;
			private List<Material> m_InnerMaterialList;
		}

		private class DerivedInnerClass : InnerClass
		{
			private Sprite m_DerivedInnerField;
		}

		private class Depth0
		{
			private Sprite m_Depth0;
		}

		private class Depth1 : Depth0
		{
			private bool m_Depth1;
		}

		private class Depth2 : Depth1
		{
			private string m_Depth2;
		}

		private class Depth3 : Depth2
		{
			private int m_Depth3;
		}

		private class Wrapper0
		{
			private Depth3 m_Depth3;
		}

		private class Wrapper1
		{
			private Wrapper0 m_Wrapper0;
		}

		private class Wrapper2
		{
			private Wrapper1 m_Wrapper1;
		}

		private class Wrapper3
		{
			private Wrapper2 m_Wrapper2;
		}
	}
}