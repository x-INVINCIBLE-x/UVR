using System;

namespace LunaWolfStudiosEditor.ScriptableSheets.Comparables
{
	public abstract class AbstractComparable<TComparable, TValue> : IComparable where TComparable : AbstractComparable<TComparable, TValue>
	{
		private readonly TValue m_Value;

		protected AbstractComparable(TValue value)
		{
			m_Value = value;
		}

		public int CompareTo(object obj)
		{
			if (obj == null || !(obj is TComparable))
			{
				throw new ArgumentException($"Object of type {obj.GetType()} is not a {nameof(TComparable)}.");
			}
			var other = (TComparable) obj;
			return CompareTo(m_Value, other.m_Value);
		}

		public abstract int CompareTo(TValue value, TValue other);
	}
}
