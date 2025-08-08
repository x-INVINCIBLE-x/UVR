using System;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Comparables
{
	public class GradientComparable : AbstractComparable<GradientComparable, Gradient>, IComparable
	{
		public GradientComparable(Gradient value) : base(value)
		{
		}

		public override int CompareTo(Gradient value, Gradient other)
		{
			var averageColor = GetAverageColor(value.colorKeys);
			var otherAverageColor = GetAverageColor(other.colorKeys);

			var redComparison = averageColor.r.CompareTo(otherAverageColor.r);
			if (redComparison != 0)
			{
				return redComparison;
			}

			var greenComparison = averageColor.g.CompareTo(otherAverageColor.g);
			if (greenComparison != 0)
			{
				return greenComparison;
			}

			var blueComparison = averageColor.b.CompareTo(otherAverageColor.b);
			if (blueComparison != 0)
			{
				return blueComparison;
			}

			return averageColor.a.CompareTo(otherAverageColor.a);
		}

		public static Color GetAverageColor(GradientColorKey[] colorKeys)
		{
			var totalColor = Color.clear;
			foreach (var key in colorKeys)
			{
				totalColor += key.color;
			}
			return totalColor / colorKeys.Length;
		}

		public static explicit operator GradientComparable(Gradient value)
		{
			return new GradientComparable(value);
		}
	}
}
