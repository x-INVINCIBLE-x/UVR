using System;
using System.Linq;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Comparables
{
	public class AnimationCurveComparable : AbstractComparable<AnimationCurveComparable, AnimationCurve>, IComparable
	{
		public AnimationCurveComparable(AnimationCurve value) : base(value)
		{
		}

		public override int CompareTo(AnimationCurve value, AnimationCurve other)
		{
			var averageTime = value.keys.Average(k => k.time);
			var otherAverageTime = other.keys.Average(k => k.time);

			var averageValues = value.keys.Average(k => k.value);
			var otherAverageValues = other.keys.Average(k => k.value);

			var average = value.Evaluate(averageTime) + averageValues;
			var otherAverage = other.Evaluate(otherAverageTime) + otherAverageValues;

			return average.CompareTo(otherAverage);
		}

		public static explicit operator AnimationCurveComparable(AnimationCurve value)
		{
			return new AnimationCurveComparable(value);
		}
	}
}
