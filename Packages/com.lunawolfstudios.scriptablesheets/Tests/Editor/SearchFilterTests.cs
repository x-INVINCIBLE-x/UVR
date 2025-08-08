using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class SearchFilterTests
	{
		private TestUtility m_TestUtility;

		private SearchSettings m_DefaultSearchSettings;
		private List<Object> m_TempObjects;

		[SetUp]
		public void SetUp()
		{
			m_TestUtility = new TestUtility();

			m_DefaultSearchSettings = new SearchSettings();
			m_TempObjects = new List<Object>();
			for (var i = 0; i < 5; i++)
			{
				var tempScriptableObject = TestUtility.TempScriptableObject.Create(i, m_TestUtility);
				m_TempObjects.Add(tempScriptableObject);
			}
		}

		[Test]
		public void GetObjects_ByName()
		{
			var result = SearchFilter.GetObjects("MyName2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("MyName2", result[0].name);
		}

		[Test]
		public void GetObjects_ByPropertyInt()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt>2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyInt > 2));
		}

		[Test]
		public void GetObjects_ByPropertyBool()
		{
			var result = SearchFilter.GetObjects("prop:m_MyRootObject.m_MyBool=true", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(3, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyBool));
		}

		[Test]
		public void GetObjects_ByPropertyColor()
		{
			var result = SearchFilter.GetObjects("property:m_MyRootObject.m_MyColor=FF0000", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyColor == Color.red));

			result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyColor=#FF0000", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyColor == Color.red));
		}

		[Test]
		public void GetObjects_ByPropertyEnum()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyEnum=Value3", m_TempObjects, m_DefaultSearchSettings, true, true);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyEnum == TestUtility.MyEnum.Value3));

			result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyEnum=2", m_TempObjects, m_DefaultSearchSettings, true, true);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyEnum == TestUtility.MyEnum.Value2));
		}

		[Test]
		public void GetObjects_ByPropertyObjectReference()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyGameObject=a", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyGameObject));
		}

		[Test]
		public void GetObjects_ByPropertyObjectReference_WhenNull()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyGameObject=?", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);

			result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyGameObject==?", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);

			var tempScriptableObject = (TestUtility.TempScriptableObject) m_TempObjects[0];
			var tempGameObject = tempScriptableObject.m_MyRootObject.m_MyGameObject;
			tempScriptableObject.m_MyRootObject.m_MyGameObject = null;

			result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyGameObject==?", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.IsNull(((TestUtility.TempScriptableObject) result[0]).m_MyRootObject.m_MyGameObject);

			result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyGameObject!=?", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(4, result.Count);

			tempScriptableObject.m_MyRootObject.m_MyGameObject = tempGameObject;
		}

		[Test]
		public void GetObjects_ByPropertyAnimationCurve()
		{
			var animationCurveJson = "{\"m_AnimationCurve\":{\"serializedVersion\":\"2\",\"m_Curve\":[{\"serializedVersion\":\"3\",\"time\":0.0,\"value\":0.0,\"inSlope\":0.0,\"outSlope\":1.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0},{\"serializedVersion\":\"3\",\"time\":1.0,\"value\":1.0,\"inSlope\":1.0,\"outSlope\":0.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0}],\"m_PreInfinity\":2,\"m_PostInfinity\":2,\"m_RotationOrder\":4}}";
			var result = SearchFilter.GetObjects($"p:m_MyRootObject.m_MyAnimationCurve={animationCurveJson}", m_TempObjects, m_DefaultSearchSettings, true, true);
			Assert.AreEqual(1, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyGradient()
		{
#if UNITY_2022_1_OR_NEWER
			var gradientJson = "{\"m_Gradient\":{\"serializedVersion\":\"2\",\"key0\":{\"r\":1.0,\"g\":0.0,\"b\":0.0,\"a\":1.0},\"key1\":{\"r\":1.0,\"g\":0.0,\"b\":1.0,\"a\":0.0},\"key2\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key3\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key4\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key5\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key6\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key7\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"ctime0\":0,\"ctime1\":65535,\"ctime2\":0,\"ctime3\":0,\"ctime4\":0,\"ctime5\":0,\"ctime6\":0,\"ctime7\":0,\"atime0\":0,\"atime1\":65535,\"atime2\":0,\"atime3\":0,\"atime4\":0,\"atime5\":0,\"atime6\":0,\"atime7\":0,\"m_Mode\":0,\"m_ColorSpace\":-1,\"m_NumColorKeys\":2,\"m_NumAlphaKeys\":2}}";
#else
			var gradientJson = "{\"m_Gradient\":{\"serializedVersion\":\"2\",\"key0\":{\"r\":1.0,\"g\":0.0,\"b\":0.0,\"a\":1.0},\"key1\":{\"r\":1.0,\"g\":0.0,\"b\":1.0,\"a\":0.0},\"key2\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key3\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key4\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key5\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key6\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key7\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"ctime0\":0,\"ctime1\":65535,\"ctime2\":0,\"ctime3\":0,\"ctime4\":0,\"ctime5\":0,\"ctime6\":0,\"ctime7\":0,\"atime0\":0,\"atime1\":65535,\"atime2\":0,\"atime3\":0,\"atime4\":0,\"atime5\":0,\"atime6\":0,\"atime7\":0,\"m_Mode\":0,\"m_NumColorKeys\":2,\"m_NumAlphaKeys\":2}}";
#endif
			var result = SearchFilter.GetObjects($"p:m_MyRootObject.m_MyGradient={gradientJson}", m_TempObjects, m_DefaultSearchSettings, true, true);
			Assert.AreEqual(1, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyNestedInt()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyNestedObject.m_MyNestedInt=3", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyNestedObject.m_MyNestedInt == 3));
		}

		[Test]
		public void GetObjects_EmptyInput()
		{
			var result = SearchFilter.GetObjects("", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_NullInput()
		{
			var result = SearchFilter.GetObjects(null, m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_InvalidPropertyPath()
		{
			var result = SearchFilter.GetObjects("p:invalidPath=2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_InvalidFilterOperation()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt<>2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_InvalidFilterValue()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_EmptyPropertyPath()
		{
			var result = SearchFilter.GetObjects("p:=2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_EmptyFilterValue()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt=", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_EmptyFilterOperation()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_EmptyCommand()
		{
			var result = SearchFilter.GetObjects(":", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_UnsupportedPropertyType()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyGameObject.name=a", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(m_TempObjects.Count, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyInt_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyLong_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyLong=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

#if UNITY_2022_1_OR_NEWER
		[Test]
		public void GetObjects_ByPropertyUInt_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyUInt=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyULong_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyULong=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}
#endif

		[Test]
		public void GetObjects_ByPropertyBool_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyBool=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyFloat_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyFloat=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyDouble_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyDouble=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyColor_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyColor=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyEnum_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyEnum=invalid", m_TempObjects, m_DefaultSearchSettings, true, true);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyAnimationCurve_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyAnimationCurve=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyGradient_Invalid()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyGradient=invalid", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void GetObjects_ByPropertyInt_Equal()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt=2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyInt == 2));
		}

		[Test]
		public void GetObjects_ByPropertyInt_NotEqual()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt!=2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(4, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyInt != 2));
		}

		[Test]
		public void GetObjects_ByPropertyInt_GreaterThan()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt>2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyInt > 2));
		}

		[Test]
		public void GetObjects_ByPropertyInt_LessThan()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt<2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyInt < 2));
		}

		[Test]
		public void GetObjects_ByPropertyInt_GreaterThanOrEqual()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt>=2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(3, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyInt >= 2));
		}

		[Test]
		public void GetObjects_ByPropertyInt_LessThanOrEqual()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyInt<=2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(3, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyInt <= 2));
		}

		[Test]
		public void GetObjects_ByPropertyString_Equal()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyString=MyString2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyString == "MyString2"));
		}

		[Test]
		public void GetObjects_ByPropertyString_NotEqual()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyString!=MyString2", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(4, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyString != "MyString2"));
		}

		[Test]
		public void GetObjects_ByPropertyBool_Equal()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyBool=true", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(3, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyBool == true));
		}

		[Test]
		public void GetObjects_ByPropertyBool_NotEqual()
		{
			var result = SearchFilter.GetObjects("p:m_MyRootObject.m_MyBool!=true", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.All(obj => ((TestUtility.TempScriptableObject) obj).m_MyRootObject.m_MyBool != true));
		}

		[TestCase("m_MyRootObject.m_MyInt", "=", "2", 1)]
		[TestCase("m_MyRootObject.m_MyInt", "!=", "2", 4)]
		[TestCase("m_MyRootObject.m_MyInt", ">", "2", 2)]
		[TestCase("m_MyRootObject.m_MyInt", "<", "2", 2)]
		[TestCase("m_MyRootObject.m_MyInt", ">=", "2", 3)]
		[TestCase("m_MyRootObject.m_MyInt", "<=", "2", 3)]
		[TestCase("m_MyRootObject.m_MyInt", "<=", "2", 3)]
		[TestCase("m_MyRootObject.m_MyLong", "<=", "2", 3)]
		[TestCase("m_MyRootObject.m_MyUInt", "<=", "2", 3)]
		[TestCase("m_MyRootObject.m_MyULong", "<=", "2", 3)]
		[TestCase("m_MyRootObject.m_MyFloat", ">", "2", 2)]
		[TestCase("m_MyRootObject.m_MyDouble", ">", "2", 2)]
		[TestCase("m_MyRootObject.m_MyChar", "=", "A", 1)]
		[TestCase("m_MyRootObject.m_MyChar", "!=", "A", 4)]
		[TestCase("m_MyRootObject.m_MyString", "=", "MyString2", 1)]
		[TestCase("m_MyRootObject.m_MyString", "!=", "MyString2", 4)]
		[TestCase("m_MyRootObject.m_MyBool", "=", "true", 3)]
		[TestCase("m_MyRootObject.m_MyBool", "!=", "true", 2)]
		[TestCase("m_MyRootObject.m_MyBool", "=", "1", 3)]
		[TestCase("m_MyRootObject.m_MyBool", "!=", "1", 2)]
		public void GetObjects_ByProperty(string propertyPath, string filterOperation, string filterValue, int expectedCount)
		{
			var result = SearchFilter.GetObjects($"property:{propertyPath}{filterOperation}{filterValue}", m_TempObjects, m_DefaultSearchSettings, false, false);
			Assert.AreEqual(expectedCount, result.Count);
		}
	}
}
