using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class SerializedTablePropertyTests
	{
		private MyScriptableObject m_MyScriptableObject;

		private SerializedTableProperty m_NameProperty;
		private SerializedTableProperty m_MyIntProperty;
		private SerializedTableProperty m_MyByteProperty;
		private SerializedTableProperty m_MyShortProperty;
		private SerializedTableProperty m_MyLongProperty;
		private SerializedTableProperty m_MyUIntProperty;
		private SerializedTableProperty m_MyULongProperty;
		private SerializedTableProperty m_MyEnumProperty;
		private SerializedTableProperty m_MyFlaggedEnumProperty;
		private SerializedTableProperty m_MyBoolProperty;
		private SerializedTableProperty m_MyFloatProperty;
		private SerializedTableProperty m_MyDoubleProperty;
		private SerializedTableProperty m_MyStringProperty;
		private SerializedTableProperty m_MyColorProperty;
		private SerializedTableProperty m_MySpriteProperty;
		private SerializedTableProperty m_MyLayerMaskProperty;
		private SerializedTableProperty m_MyStringArraySizeProperty;
		private SerializedTableProperty m_MyCharProperty;
		private SerializedTableProperty m_MyAnimationCurveProperty;
		private SerializedTableProperty m_MyGradientProperty;

		private FlatFileFormatSettings m_FormatSettings;

		private Sprite m_TestSprite;
		private string m_TestSpriteGuid;
		private string m_TestSpriteName;

		private Sprite m_BuiltinBackgroundSprite;
		private string m_BuiltinBackgroundSpriteGuid;
		private string m_BuiltinBackgroundSpriteName;

		[SetUp]
		public void SetUp()
		{
			m_MyScriptableObject = ScriptableObject.CreateInstance<MyScriptableObject>();
			m_MyScriptableObject.myInt = int.MaxValue;
			m_MyScriptableObject.myByte = byte.MaxValue;
			m_MyScriptableObject.myShort = short.MaxValue;
			m_MyScriptableObject.myLong = long.MaxValue;
			m_MyScriptableObject.myUInt = uint.MaxValue;
			m_MyScriptableObject.myULong = ulong.MaxValue;
			m_MyScriptableObject.myEnum = MyEnum.Value1;
			m_MyScriptableObject.myFlaggedEnum = MyFlaggedEnum.Value1 | MyFlaggedEnum.Value2 | MyFlaggedEnum.Value3;
			m_MyScriptableObject.myBool = true;
			m_MyScriptableObject.myFloat = 3.14f;
			m_MyScriptableObject.myDouble = 2.718281828459;
			m_MyScriptableObject.myString = "foo";
			m_MyScriptableObject.myColor = Color.red;
			m_MyScriptableObject.mySprite = null;
			m_MyScriptableObject.myLayerMask = 1 << 3;
			m_MyScriptableObject.myStringArray = new string[] { "foo", "bar" };
			m_MyScriptableObject.myChar = 'a';
			m_MyScriptableObject.myAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);
			m_MyScriptableObject.myGradient = new Gradient();
			m_MyScriptableObject.myGradient.SetKeys
			(
				new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
				new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
			);

			m_NameProperty = new SerializedTableProperty(m_MyScriptableObject, "m_Name", "nameControl");
			m_MyIntProperty = new SerializedTableProperty(m_MyScriptableObject, "myInt", "myIntControl");
			m_MyByteProperty = new SerializedTableProperty(m_MyScriptableObject, "myByte", "myByteControl");
			m_MyShortProperty = new SerializedTableProperty(m_MyScriptableObject, "myShort", "myShortControl");
			m_MyLongProperty = new SerializedTableProperty(m_MyScriptableObject, "myLong", "myLongControl");
			m_MyUIntProperty = new SerializedTableProperty(m_MyScriptableObject, "myUInt", "myUIntControl");
			m_MyULongProperty = new SerializedTableProperty(m_MyScriptableObject, "myULong", "myULongControl");
			m_MyEnumProperty = new SerializedTableProperty(m_MyScriptableObject, "myEnum", "myEnumControl");
			m_MyFlaggedEnumProperty = new SerializedTableProperty(m_MyScriptableObject, "myFlaggedEnum", "myFlaggedEnumControl");
			m_MyBoolProperty = new SerializedTableProperty(m_MyScriptableObject, "myBool", "myBoolControl");
			m_MyFloatProperty = new SerializedTableProperty(m_MyScriptableObject, "myFloat", "myFloatControl");
			m_MyDoubleProperty = new SerializedTableProperty(m_MyScriptableObject, "myDouble", "myDoubleControl");
			m_MyStringProperty = new SerializedTableProperty(m_MyScriptableObject, "myString", "myStringControl");
			m_MyColorProperty = new SerializedTableProperty(m_MyScriptableObject, "myColor", "myColorControl");
			m_MySpriteProperty = new SerializedTableProperty(m_MyScriptableObject, "mySprite", "mySpriteControl");
			m_MyLayerMaskProperty = new SerializedTableProperty(m_MyScriptableObject, "myLayerMask", "myLayerMaskControl");
			m_MyStringArraySizeProperty = new SerializedTableProperty(m_MyScriptableObject, "myStringArray.Array.size", "myStringArraySizeControl");
			m_MyCharProperty = new SerializedTableProperty(m_MyScriptableObject, "myChar", "myCharControl");
			m_MyAnimationCurveProperty = new SerializedTableProperty(m_MyScriptableObject, "myAnimationCurve", "myAnimationCurveControl");
			m_MyGradientProperty = new SerializedTableProperty(m_MyScriptableObject, "myGradient", "myGradientControl");

			m_FormatSettings = new FlatFileFormatSettings
			{
				UseStringEnums = true,
				IgnoreCase = true,
			};

			m_TestSprite = Resources.Load<Sprite>("TestSprite");
			var testSpriteAssetPath = AssetDatabase.GetAssetPath(m_TestSprite);
			m_TestSpriteGuid = AssetDatabase.AssetPathToGUID(testSpriteAssetPath);
			m_TestSpriteName = m_TestSprite.name;

			m_BuiltinBackgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
			var builtinBackgroundSpriteAssetPath = AssetDatabase.GetAssetPath(m_BuiltinBackgroundSprite);
			m_BuiltinBackgroundSpriteGuid = AssetDatabase.AssetPathToGUID(builtinBackgroundSpriteAssetPath);
			m_BuiltinBackgroundSpriteName = m_BuiltinBackgroundSprite.name;
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(m_MyScriptableObject);
		}

		[Test]
		public void Constructor_ShouldInitializeFields()
		{
			Assert.AreEqual(m_MyScriptableObject, m_MyIntProperty.RootObject);
			Assert.AreEqual("myInt", m_MyIntProperty.PropertyPath);
			Assert.AreEqual("myIntControl", m_MyIntProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyByteProperty.RootObject);
			Assert.AreEqual("myByte", m_MyByteProperty.PropertyPath);
			Assert.AreEqual("myByteControl", m_MyByteProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyShortProperty.RootObject);
			Assert.AreEqual("myShort", m_MyShortProperty.PropertyPath);
			Assert.AreEqual("myShortControl", m_MyShortProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyLongProperty.RootObject);
			Assert.AreEqual("myLong", m_MyLongProperty.PropertyPath);
			Assert.AreEqual("myLongControl", m_MyLongProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyUIntProperty.RootObject);
			Assert.AreEqual("myUInt", m_MyUIntProperty.PropertyPath);
			Assert.AreEqual("myUIntControl", m_MyUIntProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyULongProperty.RootObject);
			Assert.AreEqual("myULong", m_MyULongProperty.PropertyPath);
			Assert.AreEqual("myULongControl", m_MyULongProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyEnumProperty.RootObject);
			Assert.AreEqual("myEnum", m_MyEnumProperty.PropertyPath);
			Assert.AreEqual("myEnumControl", m_MyEnumProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyEnumProperty.RootObject);
			Assert.AreEqual("myFlaggedEnum", m_MyFlaggedEnumProperty.PropertyPath);
			Assert.AreEqual("myFlaggedEnumControl", m_MyFlaggedEnumProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyBoolProperty.RootObject);
			Assert.AreEqual("myBool", m_MyBoolProperty.PropertyPath);
			Assert.AreEqual("myBoolControl", m_MyBoolProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyFloatProperty.RootObject);
			Assert.AreEqual("myFloat", m_MyFloatProperty.PropertyPath);
			Assert.AreEqual("myFloatControl", m_MyFloatProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyDoubleProperty.RootObject);
			Assert.AreEqual("myDouble", m_MyDoubleProperty.PropertyPath);
			Assert.AreEqual("myDoubleControl", m_MyDoubleProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyStringProperty.RootObject);
			Assert.AreEqual("myString", m_MyStringProperty.PropertyPath);
			Assert.AreEqual("myStringControl", m_MyStringProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyColorProperty.RootObject);
			Assert.AreEqual("myColor", m_MyColorProperty.PropertyPath);
			Assert.AreEqual("myColorControl", m_MyColorProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MySpriteProperty.RootObject);
			Assert.AreEqual("mySprite", m_MySpriteProperty.PropertyPath);
			Assert.AreEqual("mySpriteControl", m_MySpriteProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyLayerMaskProperty.RootObject);
			Assert.AreEqual("myLayerMask", m_MyLayerMaskProperty.PropertyPath);
			Assert.AreEqual("myLayerMaskControl", m_MyLayerMaskProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyStringArraySizeProperty.RootObject);
			Assert.AreEqual("myStringArray.Array.size", m_MyStringArraySizeProperty.PropertyPath);
			Assert.AreEqual("myStringArraySizeControl", m_MyStringArraySizeProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyCharProperty.RootObject);
			Assert.AreEqual("myChar", m_MyCharProperty.PropertyPath);
			Assert.AreEqual("myCharControl", m_MyCharProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyAnimationCurveProperty.RootObject);
			Assert.AreEqual("myAnimationCurve", m_MyAnimationCurveProperty.PropertyPath);
			Assert.AreEqual("myAnimationCurveControl", m_MyAnimationCurveProperty.ControlName);

			Assert.AreEqual(m_MyScriptableObject, m_MyGradientProperty.RootObject);
			Assert.AreEqual("myGradient", m_MyGradientProperty.PropertyPath);
			Assert.AreEqual("myGradientControl", m_MyGradientProperty.ControlName);
		}

		[Test]
		public void GetSerializedObject_ShouldReturnSerializedObject()
		{
			var serializedObject = m_MyIntProperty.GetSerializedObject();
			Assert.IsNotNull(serializedObject);
			Assert.AreEqual(m_MyScriptableObject, serializedObject.targetObject);
		}

		[Test]
		public void GetSerializedProperty_ShouldReturnSerializedProperty()
		{
			var serializedObject = m_MyIntProperty.GetSerializedObject();
			var property = m_MyIntProperty.GetSerializedProperty(serializedObject);
			Assert.IsNotNull(property);
			Assert.AreEqual("myInt", property.propertyPath);
			Assert.AreEqual(m_MyScriptableObject.myInt, property.intValue);
		}

		[Test]
		public void GetProperty_ShouldReturnIntValueAsString()
		{
			var propertyValue = m_MyIntProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual(int.MaxValue.ToString(), propertyValue);

			propertyValue = m_MyByteProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual(byte.MaxValue.ToString(), propertyValue);

			propertyValue = m_MyShortProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual(short.MaxValue.ToString(), propertyValue);

			propertyValue = m_MyLongProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual(long.MaxValue.ToString(), propertyValue);

#if UNITY_2022_1_OR_NEWER
			propertyValue = m_MyUIntProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual(uint.MaxValue.ToString(), propertyValue);

			propertyValue = m_MyULongProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual(ulong.MaxValue.ToString(), propertyValue);
#endif
		}

		[Test]
		public void SetProperty_ShouldSetIntValue()
		{
			m_MyIntProperty.SetProperty(int.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(int.MinValue, m_MyScriptableObject.myInt);

			m_MyByteProperty.SetProperty(byte.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(byte.MinValue, m_MyScriptableObject.myByte);

			m_MyShortProperty.SetProperty(short.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(short.MinValue, m_MyScriptableObject.myShort);

			m_MyLongProperty.SetProperty(long.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(long.MinValue, m_MyScriptableObject.myLong);

#if UNITY_2022_1_OR_NEWER
			m_MyUIntProperty.SetProperty(uint.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(uint.MinValue, m_MyScriptableObject.myUInt);

			m_MyULongProperty.SetProperty(ulong.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(ulong.MinValue, m_MyScriptableObject.myULong);
#endif
		}

		[Test]
		public void SetProperty_IgnoresSetIntValue_WhenInvalid()
		{
			m_MyScriptableObject.myInt = 8;
			m_MyIntProperty.SetProperty(long.MaxValue.ToString(), m_FormatSettings);
			Assert.AreEqual(8, m_MyScriptableObject.myInt);

			m_MyScriptableObject.myByte = 0x1;
			m_MyByteProperty.SetProperty("invalid byte", m_FormatSettings);
			Assert.AreEqual(0x1, m_MyScriptableObject.myByte);

			m_MyScriptableObject.myShort = 4;
			m_MyShortProperty.SetProperty("invalid short", m_FormatSettings);
			Assert.AreEqual(4, m_MyScriptableObject.myShort);

			m_MyScriptableObject.myLong = 200;
			m_MyLongProperty.SetProperty(ulong.MaxValue.ToString(), m_FormatSettings);
			Assert.AreEqual(200, m_MyScriptableObject.myLong);

#if UNITY_2022_1_OR_NEWER
			m_MyScriptableObject.myUInt = 40;
			m_MyUIntProperty.SetProperty(int.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(40, m_MyScriptableObject.myUInt);

			m_MyScriptableObject.myULong = 400;
			m_MyULongProperty.SetProperty(long.MinValue.ToString(), m_FormatSettings);
			Assert.AreEqual(400, m_MyScriptableObject.myULong);
#endif
		}

		[Test]
		public void SetProperty_ShouldIgnore_WhenValueNull()
		{
			Assert.IsNotNull(m_MyScriptableObject.myInt);
			m_MyIntProperty.SetProperty(null, m_FormatSettings);
			Assert.IsNotNull(m_MyScriptableObject.myInt);

			Assert.IsNotNull(m_MyScriptableObject.myString);
			m_MyStringProperty.SetProperty(null, m_FormatSettings);
			Assert.IsNotNull(m_MyScriptableObject.myString);
		}

		[Test]
		public void IsInputFieldProperty_ShouldReturnTrueForValidInputField()
		{
			var isInputField = m_MyIntProperty.IsInputFieldProperty(true);
			Assert.IsTrue(isInputField);
		}

		[Test]
		public void IsInputFieldProperty_ShouldReturnFalseForInvalidInputField()
		{
			var isInputField = m_MyEnumProperty.IsInputFieldProperty(true);
			Assert.IsFalse(isInputField);
		}

		[Test]
		public void NeedsSelectionBorder_ShouldReturnTrue()
		{
			var needsSelectionBorder = m_NameProperty.NeedsSelectionBorder(true);
			Assert.IsTrue(needsSelectionBorder);

			needsSelectionBorder = m_MyAnimationCurveProperty.NeedsSelectionBorder();
			Assert.IsTrue(needsSelectionBorder);

			needsSelectionBorder = m_MyGradientProperty.NeedsSelectionBorder();
			Assert.IsTrue(needsSelectionBorder);
		}

		[Test]
		public void NeedsSelectionBorder_ShouldReturnFalse()
		{
			var needsSelectionBorder = m_NameProperty.NeedsSelectionBorder();
			Assert.IsFalse(needsSelectionBorder);

			needsSelectionBorder = m_MyIntProperty.NeedsSelectionBorder();
			Assert.IsFalse(needsSelectionBorder);

			needsSelectionBorder = m_MyColorProperty.NeedsSelectionBorder();
			Assert.IsFalse(needsSelectionBorder);

			needsSelectionBorder = m_MySpriteProperty.NeedsSelectionBorder();
			Assert.IsFalse(needsSelectionBorder);
		}

		[Test]
		public void GetProperty_ShouldReturnEnumValueAsString_WhenUsingStringEnums()
		{
			var propertyValue = m_MyEnumProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("Value1", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetEnumValue_WhenUsingStringEnums()
		{
			m_MyEnumProperty.SetProperty("value2", m_FormatSettings);
			Assert.AreEqual(MyEnum.Value2, m_MyScriptableObject.myEnum);

			m_MyEnumProperty.SetProperty("10", m_FormatSettings);
			Assert.AreEqual(MyEnum.Value1, m_MyScriptableObject.myEnum);

			m_MyEnumProperty.SetProperty("Enum:Value3", m_FormatSettings);
			Assert.AreEqual(MyEnum.Value3, m_MyScriptableObject.myEnum);
		}

		[Test]
		public void SetProperty_ShouldNotSetEnumValue_WhenCaseMatters()
		{
			var settings = new FlatFileFormatSettings()
			{
				UseStringEnums = true,
				IgnoreCase = false,
			};
			m_MyEnumProperty.SetProperty("value0", settings);
			Assert.AreNotEqual(MyEnum.Value0, m_MyScriptableObject.myEnum);

			m_MyEnumProperty.SetProperty("20", m_FormatSettings);
			Assert.AreEqual(MyEnum.Value2, m_MyScriptableObject.myEnum);
		}

		[Test]
		public void GetProperty_ShouldReturnEnumValue_WhenNotUsingStringEnums()
		{
			var propertyValue = m_MyEnumProperty.GetProperty(new FlatFileFormatSettings());
			Assert.AreEqual("10", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetEnumValue_WhenNotUsingStringEnums()
		{
			m_MyEnumProperty.SetProperty("10", new FlatFileFormatSettings());
			Assert.AreEqual(MyEnum.Value1, m_MyScriptableObject.myEnum);
		}

		[Test]
		public void GetProperty_ShouldReturnFlaggedEnumValue()
		{
			var propertyValue = m_MyFlaggedEnumProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("7", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetFlaggedEnumValue()
		{
			m_MyFlaggedEnumProperty.SetProperty("8", m_FormatSettings);
			Assert.AreEqual(MyFlaggedEnum.Value4, m_MyScriptableObject.myFlaggedEnum);
		}

		[Test]
		public void SetProperty_ShouldSetFlaggedEnumValue_WhenWrapped()
		{
			m_MyFlaggedEnumProperty.SetProperty("LayerMask(1)", m_FormatSettings);
			Assert.AreEqual(MyFlaggedEnum.Value1, m_MyScriptableObject.myFlaggedEnum);
		}

		[Test]
		public void SetProperty_ShouldSetFlaggedEnumValue_WhenUsingStringEnumsAndPrefixed()
		{
			m_MyFlaggedEnumProperty.SetProperty("Enum:Value2", m_FormatSettings);
			Assert.AreEqual(MyFlaggedEnum.Value2, m_MyScriptableObject.myFlaggedEnum);

			m_MyFlaggedEnumProperty.SetProperty("Value3", m_FormatSettings);
			Assert.AreNotEqual(MyFlaggedEnum.Value3, m_MyScriptableObject.myFlaggedEnum);
		}

		[Test]
		public void GetProperty_ShouldReturnBoolValueAsString()
		{
			var propertyValue = m_MyBoolProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("True", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetBoolValue()
		{
			m_MyBoolProperty.SetProperty("False", m_FormatSettings);
			Assert.AreEqual(false, m_MyScriptableObject.myBool);
		}

		[Test]
		public void SetProperty_ShouldSetBoolValue_WhenIntValue()
		{
			m_MyBoolProperty.SetProperty("1", m_FormatSettings);
			Assert.AreEqual(true, m_MyScriptableObject.myBool);

			m_MyBoolProperty.SetProperty("0", m_FormatSettings);
			Assert.AreEqual(false, m_MyScriptableObject.myBool);

			m_MyBoolProperty.SetProperty("10", m_FormatSettings);
			Assert.AreEqual(true, m_MyScriptableObject.myBool);

			m_MyBoolProperty.SetProperty("-10", m_FormatSettings);
			Assert.AreEqual(false, m_MyScriptableObject.myBool);
		}

		[Test]
		public void SetProperty_IgnoresSetBoolValue_WhenInvalid()
		{
			m_MyScriptableObject.myBool = true;
			m_MyBoolProperty.SetProperty("invalid bool", m_FormatSettings);
			Assert.AreEqual(true, m_MyScriptableObject.myBool);
		}

		[Test]
		public void GetProperty_ShouldReturnFloatValueAsString()
		{
			var propertyValue = m_MyFloatProperty.GetProperty(m_FormatSettings);
			var expectedFloatValue = 3.14f;
			Assert.AreEqual(expectedFloatValue.ToString(), propertyValue);

			propertyValue = m_MyDoubleProperty.GetProperty(m_FormatSettings);
			var expectedDoubleValue = 2.718281828459;
			Assert.AreEqual(expectedDoubleValue.ToString(), propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetFloatValue()
		{
			var newFloatValue = 2.71f;
			m_MyFloatProperty.SetProperty(newFloatValue.ToString(), m_FormatSettings);
			Assert.AreEqual(newFloatValue, m_MyScriptableObject.myFloat);

			var newDoubleValue = 3.141592653589;
			m_MyDoubleProperty.SetProperty(newDoubleValue.ToString(), m_FormatSettings);
			Assert.AreEqual(newDoubleValue, m_MyScriptableObject.myDouble);
		}

		[Test]
		public void SetProperty_IgnoresSetFloatValue_WhenInvalid()
		{
			m_MyScriptableObject.myFloat = 3.14f;
			m_MyFloatProperty.SetProperty("invalid float", m_FormatSettings);
			Assert.AreEqual(3.14f, m_MyScriptableObject.myFloat);

			m_MyScriptableObject.myDouble = 2.718281828459;
			m_MyDoubleProperty.SetProperty("invalid double", m_FormatSettings);
			Assert.AreEqual(2.718281828459, m_MyScriptableObject.myDouble);
		}

		[Test]
		public void GetProperty_ShouldReturnStringValue()
		{
			var propertyValue = m_MyStringProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("foo", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetStringValue()
		{
			m_MyStringProperty.SetProperty("bar", m_FormatSettings);
			Assert.AreEqual("bar", m_MyScriptableObject.myString);

			m_MyStringProperty.SetProperty(string.Empty, m_FormatSettings);
			Assert.AreEqual(string.Empty, m_MyScriptableObject.myString);
		}

		[Test]
		public void GetProperty_ShouldReturnColorValueAsString()
		{
			var propertyValue = m_MyColorProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("FF0000FF", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetColorValue()
		{
			m_MyColorProperty.SetProperty("#00FF00", m_FormatSettings);
			Assert.AreEqual(Color.green, m_MyScriptableObject.myColor);

			m_MyColorProperty.SetProperty("0000FF", m_FormatSettings);
			Assert.AreEqual(Color.blue, m_MyScriptableObject.myColor);
		}

		[Test]
		public void SetProperty_IgnoresSetColorValue_WhenInvalid()
		{
			m_MyScriptableObject.myColor = Color.red;
			m_MyColorProperty.SetProperty("invalid color", m_FormatSettings);
			Assert.AreEqual(Color.red, m_MyScriptableObject.myColor);
		}

		[Test]
		public void GetProperty_ShouldReturnSpriteValueAsString()
		{
			m_MyScriptableObject.mySprite = m_TestSprite;
			var propertyValue = m_MySpriteProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual($"{m_TestSpriteGuid}&{m_TestSpriteName}", propertyValue);
		}

		[Test]
		public void GetProperty_ShouldReturnSpriteValueAsString_WhenBuiltinAsset()
		{
			m_MyScriptableObject.mySprite = m_BuiltinBackgroundSprite;
			var propertyValue = m_MySpriteProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual($"{m_BuiltinBackgroundSpriteGuid}&{m_BuiltinBackgroundSpriteName}", propertyValue);
		}

		[Test]
		public void GetProperty_ShouldReturnNullValue_WhenSpriteNull()
		{
			m_MyScriptableObject.mySprite = null;
			Assert.IsNull(m_MyScriptableObject.mySprite);
			var propertyValue = m_MySpriteProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual(propertyValue, SerializedTableProperty.NullObjectValue);
		}

		[Test]
		public void SetProperty_ShouldSetSpriteValue()
		{
			m_MyScriptableObject.mySprite = null;
			Assert.IsNull(m_MyScriptableObject.mySprite);
			m_MySpriteProperty.SetProperty($"{m_TestSpriteGuid}&{m_TestSpriteName}", m_FormatSettings);
			Assert.AreEqual(m_TestSprite, m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_ShouldSetNullSpriteValue_WhenNullString()
		{
			m_MyScriptableObject.mySprite = m_BuiltinBackgroundSprite;
			Assert.IsNotNull(m_MyScriptableObject.mySprite);
			m_MySpriteProperty.SetProperty(SerializedTableProperty.NullObjectValue, m_FormatSettings);
			Assert.IsNull(m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_ShouldSetNullSpriteValue_WhenEmptyString()
		{
			m_MyScriptableObject.mySprite = m_BuiltinBackgroundSprite;
			Assert.IsNotNull(m_MyScriptableObject.mySprite);
			m_MySpriteProperty.SetProperty(string.Empty, m_FormatSettings);
			Assert.IsNull(m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_ShouldSetSpriteValue_WhenUnityObjectWrapperJSON()
		{
			m_MyScriptableObject.mySprite = null;
			Assert.IsNull(m_MyScriptableObject.mySprite);
			m_MySpriteProperty.SetProperty($"UnityEditor.ObjectWrapperJSON:{{\"guid\":\"{m_TestSpriteGuid}\",\"localId\":21300000,\"type\":3,\"instanceID\":23098}}", m_FormatSettings);
			Assert.AreEqual(m_TestSprite, m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_ShouldSetSpriteValue_WhenBuiltinAsset()
		{
			m_MyScriptableObject.mySprite = null;
			Assert.IsNull(m_MyScriptableObject.mySprite);
			m_MySpriteProperty.SetProperty($"{m_BuiltinBackgroundSpriteGuid}&{m_BuiltinBackgroundSpriteName}", m_FormatSettings);
			Assert.AreEqual(m_BuiltinBackgroundSprite, m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_IgnoresSetSpriteValue_WhenBuiltinAsset_HasInvalidName()
		{
			m_MyScriptableObject.mySprite = m_TestSprite;

			m_MySpriteProperty.SetProperty($"{m_BuiltinBackgroundSpriteGuid}&invalid-name", m_FormatSettings);
			Assert.AreEqual(m_TestSprite, m_MyScriptableObject.mySprite);

			m_MySpriteProperty.SetProperty($"{m_BuiltinBackgroundSpriteGuid}&", m_FormatSettings);
			Assert.AreEqual(m_TestSprite, m_MyScriptableObject.mySprite);

			m_MySpriteProperty.SetProperty($"{m_BuiltinBackgroundSpriteGuid}", m_FormatSettings);
			Assert.AreEqual(m_TestSprite, m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_ShouldSetSpriteValue_WhenBuiltinAssetAndUnityObjectWrapperJSON()
		{
			m_MyScriptableObject.mySprite = null;
			Assert.IsNull(m_MyScriptableObject.mySprite);
			m_MySpriteProperty.SetProperty($"UnityEditor.ObjectWrapperJSON:{{\"guid\":\"{m_BuiltinBackgroundSpriteGuid}\",\"localId\":10907,\"type\":0,\"instanceID\":668}}", m_FormatSettings);
			Assert.AreEqual(m_BuiltinBackgroundSprite, m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_IgnoresSetSpriteValue_WhenUnityObjectWrapperJSON_HasIncorrectLocalId()
		{
			m_MyScriptableObject.mySprite = m_TestSprite;
			m_MySpriteProperty.SetProperty($"UnityEditor.ObjectWrapperJSON:{{\"guid\":\"{m_BuiltinBackgroundSpriteGuid}\",\"localId\":1234567890,\"type\":0,\"instanceID\":668}}", m_FormatSettings);
			Assert.AreEqual(m_TestSprite, m_MyScriptableObject.mySprite);
		}

		[Test]
		public void SetProperty_IgnoresSetSpriteValue_WhenUnityObjectWrapperJSON_IsInvalid()
		{
			m_MyScriptableObject.mySprite = m_BuiltinBackgroundSprite;

			m_MySpriteProperty.SetProperty($"UnityEditor.ObjectWrapperJSON:{{\"guid\":\"{m_TestSpriteGuid}\"\"localId\":21300000\"type\":3\"instanceID\":23098}}", m_FormatSettings);
			Assert.AreEqual(m_BuiltinBackgroundSprite, m_MyScriptableObject.mySprite);

			m_MySpriteProperty.SetProperty($"Invalid.JSON{{\"guid\":\"{m_TestSpriteGuid}\",\"localId\":21300000,\"type\":3,\"instanceID\":23098}}", m_FormatSettings);
			Assert.AreEqual(m_BuiltinBackgroundSprite, m_MyScriptableObject.mySprite);
		}

		[Test]
		public void GetProperty_ShouldReturnLayerMaskValueAsString()
		{
			var propertyValue = m_MyLayerMaskProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("8", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetLayerMaskValue()
		{
			m_MyLayerMaskProperty.SetProperty("-1", m_FormatSettings);
			Assert.AreEqual(-1, m_MyScriptableObject.myLayerMask.value);

			m_MyLayerMaskProperty.SetProperty("LayerMask(2)", m_FormatSettings);
			Assert.AreEqual(2, m_MyScriptableObject.myLayerMask.value);
		}

		[Test]
		public void SetProperty_IgnoresSetLayerMaskValue_WhenInvalid()
		{
			m_MyScriptableObject.myLayerMask = 1;
			m_MyLayerMaskProperty.SetProperty("invalid layer mask", m_FormatSettings);
			Assert.AreEqual(1, m_MyScriptableObject.myLayerMask.value);
		}

		[Test]
		public void GetProperty_ShouldReturnArraySizeAsString()
		{
			var propertyValue = m_MyStringArraySizeProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("2", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetArraySize()
		{
			m_MyStringArraySizeProperty.SetProperty("5", m_FormatSettings);
			Assert.AreEqual(5, m_MyScriptableObject.myStringArray.Length);
		}

		[Test]
		public void GetProperty_ShouldReturnCharValueAsString()
		{
			var propertyValue = m_MyCharProperty.GetProperty(m_FormatSettings);
			Assert.AreEqual("a", propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetCharValue()
		{
			m_MyCharProperty.SetProperty("b", m_FormatSettings);
			Assert.AreEqual('b', m_MyScriptableObject.myChar);

			m_MyCharProperty.SetProperty("abcdefg", m_FormatSettings);
			Assert.AreEqual('a', m_MyScriptableObject.myChar);
		}

		[Test]
		public void SetProperty_IgnoreSetCharValue_WhenEmptyString()
		{
			m_MyScriptableObject.myChar = 'c';
			m_MyCharProperty.SetProperty(string.Empty, m_FormatSettings);
			Assert.AreEqual('c', m_MyScriptableObject.myChar);
		}

		[Test]
		public void GetProperty_ShouldReturnAnimationCurveAsString()
		{
			var propertyValue = m_MyAnimationCurveProperty.GetProperty(m_FormatSettings);
			var expectedValue = "{\"m_AnimationCurve\":{\"serializedVersion\":\"2\",\"m_Curve\":[{\"serializedVersion\":\"3\",\"time\":0.0,\"value\":0.0,\"inSlope\":0.0,\"outSlope\":1.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0},{\"serializedVersion\":\"3\",\"time\":1.0,\"value\":1.0,\"inSlope\":1.0,\"outSlope\":0.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0}],\"m_PreInfinity\":2,\"m_PostInfinity\":2,\"m_RotationOrder\":4}}";
			Assert.AreEqual(expectedValue, propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetAnimationCurve()
		{
			var propertyValue = "{\"m_AnimationCurve\":{\"serializedVersion\":\"2\",\"m_Curve\":[{\"serializedVersion\":\"3\",\"time\":0.0,\"value\":1.0,\"inSlope\":0.0,\"outSlope\":-1.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0},{\"serializedVersion\":\"3\",\"time\":1.0,\"value\":0.0,\"inSlope\":-1.0,\"outSlope\":0.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0}],\"m_PreInfinity\":2,\"m_PostInfinity\":2,\"m_RotationOrder\":4}}";
			m_MyAnimationCurveProperty.SetProperty(propertyValue, m_FormatSettings);
			var expectedValue = AnimationCurve.Linear(0, 1, 1, 0);
			Assert.AreEqual(expectedValue, m_MyScriptableObject.myAnimationCurve);
		}

		[Test]
		public void SetProperty_IgnoresSetAnimationCurve_WhenJSON_IsInvalid()
		{
			var expectedValue = AnimationCurve.Linear(1, 1, 1, 1);
			m_MyScriptableObject.myAnimationCurve = expectedValue;
			var propertyValue = "Invalid.JSON{\"m_AnimationCurve\":{\"serializedVersion\":\"2\",\"m_Curve\":[{\"serializedVersion\":\"3\",\"time\":0.0,\"value\":1.0,\"inSlope\":0.0,\"outSlope\":-1.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0},{\"serializedVersion\":\"3\",\"time\":1.0,\"value\":0.0,\"inSlope\":-1.0,\"outSlope\":0.0,\"tangentMode\":0,\"weightedMode\":0,\"inWeight\":0.0,\"outWeight\":0.0}],\"m_PreInfinity\":2,\"m_PostInfinity\":2,\"m_RotationOrder\":4}}";
			m_MyAnimationCurveProperty.SetProperty(propertyValue, m_FormatSettings);
			Assert.AreEqual(expectedValue, m_MyScriptableObject.myAnimationCurve);
		}

		[Test]
		public void GetProperty_ShouldReturnGradientAsString()
		{
			var propertyValue = m_MyGradientProperty.GetProperty(m_FormatSettings);
#if UNITY_2022_1_OR_NEWER
			var expectedValue = "{\"m_Gradient\":{\"serializedVersion\":\"2\",\"key0\":{\"r\":1.0,\"g\":0.0,\"b\":0.0,\"a\":1.0},\"key1\":{\"r\":0.0,\"g\":0.0,\"b\":1.0,\"a\":0.0},\"key2\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key3\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key4\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key5\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key6\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key7\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"ctime0\":0,\"ctime1\":65535,\"ctime2\":0,\"ctime3\":0,\"ctime4\":0,\"ctime5\":0,\"ctime6\":0,\"ctime7\":0,\"atime0\":0,\"atime1\":65535,\"atime2\":0,\"atime3\":0,\"atime4\":0,\"atime5\":0,\"atime6\":0,\"atime7\":0,\"m_Mode\":0,\"m_ColorSpace\":-1,\"m_NumColorKeys\":2,\"m_NumAlphaKeys\":2}}";
#else
			var expectedValue = "{\"m_Gradient\":{\"serializedVersion\":\"2\",\"key0\":{\"r\":1.0,\"g\":0.0,\"b\":0.0,\"a\":1.0},\"key1\":{\"r\":0.0,\"g\":0.0,\"b\":1.0,\"a\":0.0},\"key2\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key3\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key4\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key5\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key6\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key7\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"ctime0\":0,\"ctime1\":65535,\"ctime2\":0,\"ctime3\":0,\"ctime4\":0,\"ctime5\":0,\"ctime6\":0,\"ctime7\":0,\"atime0\":0,\"atime1\":65535,\"atime2\":0,\"atime3\":0,\"atime4\":0,\"atime5\":0,\"atime6\":0,\"atime7\":0,\"m_Mode\":0,\"m_NumColorKeys\":2,\"m_NumAlphaKeys\":2}}";
#endif
			Assert.AreEqual(expectedValue, propertyValue);
		}

		[Test]
		public void SetProperty_ShouldSetGradient()
		{
			var propertyValue = "{\"m_Gradient\":{\"serializedVersion\":\"2\",\"key0\":{\"r\":0.0,\"g\":0.0,\"b\":1.0,\"a\":1.0},\"key1\":{\"r\":1.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key2\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key3\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key4\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key5\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key6\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key7\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"ctime0\":0,\"ctime1\":65535,\"ctime2\":0,\"ctime3\":0,\"ctime4\":0,\"ctime5\":0,\"ctime6\":0,\"ctime7\":0,\"atime0\":0,\"atime1\":65535,\"atime2\":0,\"atime3\":0,\"atime4\":0,\"atime5\":0,\"atime6\":0,\"atime7\":0,\"m_Mode\":0,\"m_ColorSpace\":-1,\"m_NumColorKeys\":2,\"m_NumAlphaKeys\":2}}";
			m_MyGradientProperty.SetProperty(propertyValue, m_FormatSettings);
			var expectedValue = new Gradient();
			expectedValue.SetKeys
			(
				new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) },
				new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
			);
			Assert.AreEqual(expectedValue, m_MyScriptableObject.myGradient);
		}

		[Test]
		public void SetProperty_IgnoresSetGradient_WhenJSON_IsInvalid()
		{
			var expectedValue = new Gradient();
			expectedValue.SetKeys
			(
				new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) },
				new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
			);
			m_MyScriptableObject.myGradient = expectedValue;
			var propertyValue = "Invalid.JSON{\"m_Gradient\":{\"serializedVersion\":\"2\",\"key0\":{\"r\":0.0,\"g\":0.0,\"b\":1.0,\"a\":1.0},\"key1\":{\"r\":1.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key2\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key3\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key4\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key5\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key6\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"key7\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"ctime0\":0,\"ctime1\":65535,\"ctime2\":0,\"ctime3\":0,\"ctime4\":0,\"ctime5\":0,\"ctime6\":0,\"ctime7\":0,\"atime0\":0,\"atime1\":65535,\"atime2\":0,\"atime3\":0,\"atime4\":0,\"atime5\":0,\"atime6\":0,\"atime7\":0,\"m_Mode\":0,\"m_ColorSpace\":-1,\"m_NumColorKeys\":2,\"m_NumAlphaKeys\":2}}";
			m_MyGradientProperty.SetProperty(propertyValue, m_FormatSettings);
			Assert.AreEqual(expectedValue, m_MyScriptableObject.myGradient);
		}

		private class MyScriptableObject : ScriptableObject
		{
			public int myInt;
			public byte myByte;
			public short myShort;
			public long myLong;
			public uint myUInt;
			public ulong myULong;
			public MyEnum myEnum;
			public MyFlaggedEnum myFlaggedEnum;
			public bool myBool;
			public float myFloat;
			public double myDouble;
			public string myString;
			public Color myColor;
			public Sprite mySprite;
			public LayerMask myLayerMask;
			public string[] myStringArray;
			public char myChar;
			public AnimationCurve myAnimationCurve;
			public Gradient myGradient;
		}

		private enum MyEnum
		{
			Value0 = 0,
			Value1 = 10,
			Value2 = 20,
			Value3 = 30,
		}

		[System.Flags]
		private enum MyFlaggedEnum
		{
			Value0 = 0,
			Value1 = 1,
			Value2 = 2,
			Value3 = 4,
			Value4 = 8,
		}
	}
}
