using System.Collections.Generic;
using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.Sampler
{
	[System.Serializable]
	public class SerializedPropertySampler : ScriptableObject
	{
		// Generic
		[SerializeField]
		private int[] m_IntArray = { 1, 2, 3 };
		public int[] IntArray { get => m_IntArray; set => m_IntArray = value; }

		[SerializeField]
		private List<string> m_StringList = new List<string> { "one", "two", "three" };
		public List<string> StringList { get => m_StringList; set => m_StringList = value; }

		[SerializeField]
		private List<Material> m_MaterialList;
		public List<Material> MaterialList { get => m_MaterialList; set => m_MaterialList = value; }

		[SerializeField]
		private SampleStruct m_SampleStruct = new SampleStruct { X = 10, Y = 20 };
		public SampleStruct SampleStruct { get => m_SampleStruct; set => m_SampleStruct = value; }

		[SerializeField]
		private SampleClass m_SampleClass = new SampleClass { Name = "Foo", Id = 1 };
		public SampleClass SampleClass { get => m_SampleClass; set => m_SampleClass = value; }

		// Integer
		[SerializeField]
		private int m_IntValue = int.MaxValue;
		public int IntValue { get => m_IntValue; set => m_IntValue = value; }

		[SerializeField]
		private byte m_ByteValue = byte.MaxValue;
		public byte ByteValue { get => m_ByteValue; set => m_ByteValue = value; }

		[SerializeField]
		private short m_ShortValue = short.MaxValue;
		public short ShortValue { get => m_ShortValue; set => m_ShortValue = value; }

		[SerializeField]
		private long m_LongValue = long.MaxValue;
		public long LongValue { get => m_LongValue; set => m_LongValue = value; }

		[SerializeField]
		private uint m_UIntValue = uint.MaxValue;
		public uint UIntValue { get => m_UIntValue; set => m_UIntValue = value; }

		[SerializeField]
		private ulong m_ULongValue = ulong.MaxValue;
		public ulong ULongValue { get => m_ULongValue; set => m_ULongValue = value; }

		// Boolean
		[SerializeField]
		private bool m_BoolValue = true;
		public bool BoolValue { get => m_BoolValue; set => m_BoolValue = value; }

		// Float
		[SerializeField]
		private float m_FloatValue = 2.71f;
		public float FloatValue { get => m_FloatValue; set => m_FloatValue = value; }

		[SerializeField]
		private double m_DoubleValue = 3.141592653589;
		public double DoubleValue { get => m_DoubleValue; set => m_DoubleValue = value; }

		// String
		[SerializeField]
		private string m_StringValue = "Hello, World!";
		public string StringValue { get => m_StringValue; set => m_StringValue = value; }

		// Color
		[SerializeField]
		private Color m_ColorValue = Color.red;
		public Color ColorValue { get => m_ColorValue; set => m_ColorValue = value; }

		// ObjectReference
		[SerializeField]
		private GameObject m_GameObjectReference;
		public GameObject GameObjectReference { get => m_GameObjectReference; set => m_GameObjectReference = value; }

		// LayerMask
		[SerializeField]
		private LayerMask m_LayerMaskValue;
		public LayerMask LayerMaskValue { get => m_LayerMaskValue; set => m_LayerMaskValue = value; }

		// Enum
		[SerializeField]
		private SampleEnum m_EnumValue = SampleEnum.OptionA;
		public SampleEnum EnumValue { get => m_EnumValue; set => m_EnumValue = value; }

		// Vector2
		[SerializeField]
		private Vector2 m_Vector2Value = new Vector2(1.0f, 2.0f);
		public Vector2 Vector2Value { get => m_Vector2Value; set => m_Vector2Value = value; }

		// Vector3
		[SerializeField]
		private Vector3 m_Vector3Value = new Vector3(1.0f, 2.0f, 3.0f);
		public Vector3 Vector3Value { get => m_Vector3Value; set => m_Vector3Value = value; }

		// Vector4
		[SerializeField]
		private Vector4 m_Vector4Value = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
		public Vector4 Vector4Value { get => m_Vector4Value; set => m_Vector4Value = value; }

		// Rect
		[SerializeField]
		private Rect m_RectValue = new Rect(0, 0, 100, 100);
		public Rect RectValue { get => m_RectValue; set => m_RectValue = value; }

		// ArraySize
		[SerializeField]
		private int m_ArraySize = 3;
		public int ArraySize { get => m_ArraySize; set => m_ArraySize = value; }

		// Character
		[SerializeField]
		private char m_CharValue = 'A';
		public char CharValue { get => m_CharValue; set => m_CharValue = value; }

		// AnimationCurve
		[SerializeField]
		private AnimationCurve m_AnimationCurveValue = AnimationCurve.Linear(0, 0, 1, 1);
		public AnimationCurve AnimationCurveValue { get => m_AnimationCurveValue; set => m_AnimationCurveValue = value; }

		// Bounds
		[SerializeField]
		private Bounds m_BoundsValue = new Bounds(Vector3.zero, Vector3.one);
		public Bounds BoundsValue { get => m_BoundsValue; set => m_BoundsValue = value; }

		// Gradient
		[SerializeField]
		private Gradient m_GradientValue = new Gradient();
		public Gradient GradientValue { get => m_GradientValue; set => m_GradientValue = value; }

		// Quaternion
		[SerializeField]
		private Quaternion m_QuaternionValue = Quaternion.identity;
		public Quaternion QuaternionValue { get => m_QuaternionValue; set => m_QuaternionValue = value; }

		// ExposedReference
		[SerializeField]
		private ExposedReference<ScriptableObject> m_ExposedReferenceValue;
		public ExposedReference<ScriptableObject> ExposedReferenceValue { get => m_ExposedReferenceValue; set => m_ExposedReferenceValue = value; }

		// FixedBufferSize
		[SerializeField]
		private int m_FixedBufferSize = 10;
		public int FixedBufferSize { get => m_FixedBufferSize; set => m_FixedBufferSize = value; }

		// Vector2Int
		[SerializeField]
		private Vector2Int m_Vector2IntValue = new Vector2Int(1, 2);
		public Vector2Int Vector2IntValue { get => m_Vector2IntValue; set => m_Vector2IntValue = value; }

		// Vector3Int
		[SerializeField]
		private Vector3Int m_Vector3IntValue = new Vector3Int(1, 2, 3);
		public Vector3Int Vector3IntValue { get => m_Vector3IntValue; set => m_Vector3IntValue = value; }

		// RectInt
		[SerializeField]
		private RectInt m_RectIntValue = new RectInt(0, 0, 100, 100);
		public RectInt RectIntValue { get => m_RectIntValue; set => m_RectIntValue = value; }

		// BoundsInt
		[SerializeField]
		private BoundsInt m_BoundsIntValue = new BoundsInt(Vector3Int.zero, Vector3Int.one);
		public BoundsInt BoundsIntValue { get => m_BoundsIntValue; set => m_BoundsIntValue = value; }

#if UNITY_2022_1_OR_NEWER
		// ManagedReference
		[SerializeReference]
		private SampleClass m_ManagedReferenceValue;
		public SampleClass ManagedReferenceValue { get => m_ManagedReferenceValue; set => m_ManagedReferenceValue = value; }
#endif

		// Hash128
		[SerializeField]
		private Hash128 m_Hash128Value = new Hash128(1, 2, 3, 4);
		public Hash128 Hash128Value { get => m_Hash128Value; set => m_Hash128Value = value; }
	}

	[System.Serializable]
	public struct SampleStruct
	{
		[SerializeField]
		private int m_X;
		public int X { get => m_X; set => m_X = value; }

		[SerializeField]
		private int m_Y;
		public int Y { get => m_Y; set => m_Y = value; }

		[SerializeField]
		private Sprite m_Icon;
		public Sprite Icon { get => m_Icon; set => m_Icon = value; }
	}

	[System.Serializable]
	public class SampleClass
	{
		[SerializeField]
		private string m_Name;
		public string Name { get => m_Name; set => m_Name = value; }

		[SerializeField]
		private int m_Id;
		public int Id { get => m_Id; set => m_Id = value; }

		[SerializeField]
		private Sprite m_Icon;
		public Sprite Icon { get => m_Icon; set => m_Icon = value; }
	}

	public enum SampleEnum
	{
		OptionA = 0,
		OptionB = 1,
		OptionC = 2,
	}
}
