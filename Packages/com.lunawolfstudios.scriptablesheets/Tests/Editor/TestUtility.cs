using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	public class TestUtility
	{
		public enum MyEnum
		{
			Value0,
			Value1,
			Value2,
			Value3,
			Value4,
		}

		public readonly Color[] Colors = new Color[]
		{
			Color.red,
			Color.yellow,
			Color.green,
			Color.blue,
			Color.magenta,
		};

		public readonly GameObject[] GameObjects = new GameObject[]
		{
			new GameObject("a"),
			new GameObject("b"),
			new GameObject("c"),
			new GameObject("d"),
			new GameObject("e"),
		};

		public readonly LayerMask[] LayerMasks = new LayerMask[]
		{
			1 << 0,
			1 << 1,
			1 << 2,
			1 << 4,
			1 << 5
		};


		public readonly AnimationCurve[] AnimationCurves = new AnimationCurve[]
		{
			AnimationCurve.Linear(0, 0, 0, 0),
			AnimationCurve.Linear(0, 0, 1, 1),
			AnimationCurve.Linear(0, 0, 1, 2),
			AnimationCurve.Linear(0, 0, 1, 3),
			AnimationCurve.Linear(0, 0, 1, 4),
		};

		public readonly Gradient[] Gradients = new Gradient[]
		{
			GetGradient(0),
			GetGradient(1),
			GetGradient(2),
			GetGradient(3),
			GetGradient(4),
		};

		public static Gradient GetGradient(int index)
		{
			var gradient = new Gradient();
			if (index == 4)
			{
				gradient.SetKeys
				(
					new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
					new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
				);
			}
			else if (index == 3)
			{
				gradient.SetKeys
				(
					new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.magenta, 1.0f) },
					new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
				);
			}
			else if (index == 2)
			{
				gradient.SetKeys
				(
					new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.yellow, 1.0f) },
					new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
				);
			}
			else if (index == 1)
			{
				gradient.SetKeys
				(
					new GradientColorKey[] { new GradientColorKey(Color.magenta, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
					new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
				);
			}
			else
			{
				gradient.SetKeys
				(
					new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.green, 1.0f) },
					new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
				);
			}
			return gradient;
		}

		[System.Serializable]
		public class TempScriptableObject : ScriptableObject
		{
			public RootObject m_MyRootObject;

			public static TempScriptableObject Create(int index, TestUtility testUtility)
			{
				var tempScriptableObject = CreateInstance<TempScriptableObject>();
				tempScriptableObject.name = "MyName" + index;
				var boolValue = index % 2 == 0;
				tempScriptableObject.m_MyRootObject = new RootObject
				{
					m_MyInt = index,
					m_MyByte = (byte) index,
					m_MyShort = (short) index,
					m_MyLong = index,
					m_MyUInt = (uint) index,
					m_MyULong = (ulong) index,
					m_MyBool = boolValue,
					m_MyFloat = index,
					m_MyDouble = index,
					m_MyChar = (char) (index + 65),
					m_MyString = "MyString" + index,
					m_MyColor = testUtility.Colors[index],
					m_MyGameObject = testUtility.GameObjects[index],
					m_MyLayerMask = testUtility.LayerMasks[index],
					m_MyEnum = (MyEnum) index,
					m_MyAnimationCurve = testUtility.AnimationCurves[index],
					m_MyGradient = testUtility.Gradients[index],
					m_MyNestedObject = new NestedObject()
					{
						m_MyNestedInt = index,
						m_MyNestedByte = (byte) index,
						m_MyNestedShort = (short) index,
						m_MyNestedLong = index,
						m_MyNestedUInt = (uint) index,
						m_MyNestedULong = (ulong) index,
						m_MyNestedBool = boolValue,
						m_MyNestedFloat = index,
						m_MyNestedDouble = index,
						m_MyNestedChar = (char) (index + 65),
						m_MyNestedString = "MyNestedString" + index,
						m_MyNestedColor = testUtility.Colors[index],
						m_MyNestedGameObject = testUtility.GameObjects[index],
						m_MyNestedLayerMask = testUtility.LayerMasks[index],
						m_MyNestedEnum = (MyEnum) index,
						m_MyNestedAnimationCurve = testUtility.AnimationCurves[index],
						m_MyNestedGradient = testUtility.Gradients[index],
					}
				};
				return tempScriptableObject;
			}
		}

		[System.Serializable]
		public class RootObject
		{
			public int m_MyInt;
			public byte m_MyByte;
			public short m_MyShort;
			public long m_MyLong;
			public uint m_MyUInt;
			public ulong m_MyULong;
			public bool m_MyBool;
			public float m_MyFloat;
			public double m_MyDouble;
			public char m_MyChar;
			public string m_MyString;
			public Color m_MyColor;
			public GameObject m_MyGameObject;
			public LayerMask m_MyLayerMask;
			public MyEnum m_MyEnum;
			public NestedObject m_MyNestedObject;
			public AnimationCurve m_MyAnimationCurve;
			public Gradient m_MyGradient;
		}

		[System.Serializable]
		public class NestedObject
		{
			public int m_MyNestedInt;
			public byte m_MyNestedByte;
			public short m_MyNestedShort;
			public long m_MyNestedLong;
			public uint m_MyNestedUInt;
			public ulong m_MyNestedULong;
			public bool m_MyNestedBool;
			public float m_MyNestedFloat;
			public double m_MyNestedDouble;
			public char m_MyNestedChar;
			public string m_MyNestedString;
			public Color m_MyNestedColor;
			public GameObject m_MyNestedGameObject;
			public LayerMask m_MyNestedLayerMask;
			public MyEnum m_MyNestedEnum;
			public AnimationCurve m_MyNestedAnimationCurve;
			public Gradient m_MyNestedGradient;
		}
	}
}
