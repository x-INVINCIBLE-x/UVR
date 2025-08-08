using System;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.Comparables
{
	public class ColorComparable : AbstractComparable<ColorComparable, Color>, IComparable
	{
		public ColorComparable(Color value) : base(value)
		{
		}

		public override int CompareTo(Color value, Color other)
		{
			Color.RGBToHSV(value, out float xHue, out float xSaturation, out float xValue);
			Color.RGBToHSV(other, out float yHue, out float ySaturation, out float yValue);
			if (xHue < yHue)
			{
				return -1;
			}
			else if (xHue > yHue)
			{
				return 1;
			}
			else
			{
				if (xSaturation < ySaturation)
				{
					return -1;
				}
				else if (xSaturation > ySaturation)
				{
					return 1;
				}
				else
				{
					if (xValue < yValue)
					{
						return -1;
					}
					else if (xValue > yValue)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
			}
		}

		public static explicit operator ColorComparable(Color value)
		{
			return new ColorComparable(value);
		}
	}
}
