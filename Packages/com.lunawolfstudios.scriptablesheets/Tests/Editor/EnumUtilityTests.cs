using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using NUnit.Framework;
using System;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class EnumUtilityTests
	{
		[Test]
		public void FirstFlagOrDefault_ReturnsFirstSetFlag()
		{
			var flags = Flagged.Flag1 | Flagged.Flag3;
			var result = flags.FirstFlagOrDefault();
			Assert.AreEqual(Flagged.Flag1, result);

			flags = Flagged.Flag3 | Flagged.Flag2;
			result = flags.FirstFlagOrDefault();
			Assert.AreEqual(Flagged.Flag2, result);
		}

		[Test]
		public void FirstFlagOrDefault_NoSetFlag_ReturnsDefault()
		{
			var flags = Flagged.None;
			var result = flags.FirstFlagOrDefault();
			Assert.AreEqual(Flagged.None, result);
		}

		[Test]
		public void FirstFlagOrDefault_NotFlaggedEnum_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => EnumUtility.FirstFlagOrDefault(NotFlagged.Value1));
		}

		[Flags]
		public enum Flagged
		{
			None = 0,
			Flag1 = 1,
			Flag2 = 2,
			Flag3 = 4
		}

		public enum NotFlagged
		{
			None = 0,
			Value1 = 1,
			Value2 = 2,
			Value3 = 4
		}
	}
}
