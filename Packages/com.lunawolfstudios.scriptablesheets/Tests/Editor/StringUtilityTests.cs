using LunaWolfStudiosEditor.ScriptableSheets.Shared;
using NUnit.Framework;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class StringUtilityTests
	{
		[TestCase("This is placeholder {t}_{i}.", 5, "foo", "This is placeholder foo_5.")]
		[TestCase("This is placeholder {{t}}_{{i}}.", 10, "bar", "This is placeholder {bar}_{10}.")]
		public void ExpandAll_ReturnsTextWithExpandedText(string text, int index, string type, string expected)
		{
			var result = text.ExpandIndex(index).ExpandType(type);
			Assert.AreEqual(expected, result);
		}

		[TestCase("This is placeholder {t}_{i}.", 5, "foo", "This is placeholder foo_05.", 2)]
		[TestCase("This is placeholder {{t}}_{{i}}.", 10, "bar", "This is placeholder {bar}_{010}.", 3)]
		public void ExpandAll_ReturnsTextWithExpandedTextAndPadding(string text, int index, string type, string expected, int padding)
		{
			var result = text.ExpandIndex(index, padding).ExpandType(type);
			Assert.AreEqual(expected, result);
		}

		[TestCase("This is placeholder {i}.", 5, "This is placeholder 5.")]
		[TestCase("This is placeholder {{i}}.", 5, "This is placeholder {5}.")]
		[TestCase("{i} is the index. I repeat {i}.", 10, "10 is the index. I repeat 10.")]
		[TestCase("This is a test.", 5, "This is a test.")]
		[TestCase("", 5, "")]
		public void ExpandIndex_ReturnsTextWithExpandedIndex(string text, int index, string expected)
		{
			var result = text.ExpandIndex(index);
			Assert.AreEqual(expected, result);
		}

		[TestCase("This is placeholder {i}.", 5, "This is placeholder 5.", 1)]
		[TestCase("This is placeholder {{i}}.", 5, "This is placeholder {00005}.", 5)]
		[TestCase("{i} is the index. I repeat {i}.", 10, "0000000010 is the index. I repeat 0000000010.", 10)]
		[TestCase("This is a test.", 5, "This is a test.", 5)]
		[TestCase("", 5, "", 1)]
		public void ExpandIndex_ReturnsTextWithExpandedIndexAndPadding(string text, int index, string expected, int padding)
		{
			var result = text.ExpandIndex(index, padding);
			Assert.AreEqual(expected, result);
		}

		[TestCase("This is placeholder {t}.", "foo", "This is placeholder foo.")]
		[TestCase("This is placeholder {{t}}.", "bar", "This is placeholder {bar}.")]
		[TestCase("{t} is the type. I repeat {t}.", "foo", "foo is the type. I repeat foo.")]
		[TestCase("This is a test.", "bar", "This is a test.")]
		[TestCase("", "foo", "")]
		public void ExpandType_ReturnsTextWithExpandedType(string text, string type, string expected)
		{
			var result = text.ExpandType(type);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void GetEscapedText_ReplacesBackslashes()
		{
			var originalText = "This is a \\ test";
			var expectedText = "This is a \\\\ test";
			var result = originalText.GetEscapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void GetEscapedText_ReplacesCarriageReturn()
		{
			var originalText = "This is a \r test";
			var expectedText = "This is a \\r test";
			var result = originalText.GetEscapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void GetEscapedText_ReplacesLineFeed()
		{
			var originalText = "This is a \n test";
			var expectedText = "This is a \\n test";
			var result = originalText.GetEscapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void GetEscapedText_ReplacesTab()
		{
			var originalText = "This is a \t test";
			var expectedText = "This is a \\t test";
			var result = originalText.GetEscapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void GetUnescapedText_ReplacesDoubleBackslashes()
		{
			var originalText = "This is a \\\\ test";
			var expectedText = "This is a \\ test";
			var result = originalText.GetUnescapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void GetUnescapedText_ReplacesCarriageReturnEscape()
		{
			var originalText = "This is a \\r test";
			var expectedText = "This is a \r test";
			var result = originalText.GetUnescapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void GetUnescapedText_ReplacesLineFeedEscape()
		{
			var originalText = "This is a \\n test";
			var expectedText = "This is a \n test";
			var result = originalText.GetUnescapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void GetUnescapedText_ReplacesTabEscape()
		{
			var originalText = "This is a \\t test";
			var expectedText = "This is a \t test";
			var result = originalText.GetUnescapedText();
			Assert.AreEqual(expectedText, result);
		}

		[Test]
		public void MatchesSearch_StartsWith_CaseSensitive()
		{
			var source = "Hello world";
			var settings = new SearchSettings
			{
				CaseSensitive = true,
				StartsWith = true,
			};
			var result = source.MatchesSearch("Hello", settings);
			Assert.IsTrue(result);

			result = source.MatchesSearch("hello", settings);
			Assert.IsFalse(result);
		}

		[Test]
		public void MatchesSearch_StartsWith_CaseInsensitive()
		{
			var source = "Hello world";
			var settings = new SearchSettings
			{
				CaseSensitive = false,
				StartsWith = true,
			};
			var result = source.MatchesSearch("hello", settings);
			Assert.IsTrue(result);

			result = source.MatchesSearch("ello", settings);
			Assert.IsFalse(result);
		}

		[Test]
		public void MatchesSearch_Contains_CaseSensitive()
		{
			var source = "Hello world";
			var settings = new SearchSettings
			{
				CaseSensitive = true,
				StartsWith = false,
			};
			var result = source.MatchesSearch("world", settings);
			Assert.IsTrue(result);

			result = source.MatchesSearch("World", settings);
			Assert.IsFalse(result);
		}

		[Test]
		public void MatchesSearch_Contains_CaseInsensitive()
		{
			var source = "Hello world";
			var settings = new SearchSettings
			{
				CaseSensitive = false,
				StartsWith = false,
			};
			var result = source.MatchesSearch("WORLD", settings);
			Assert.IsTrue(result);

			result = source.MatchesSearch("World Hello", settings);
			Assert.IsFalse(result);
		}

		[Test]
		public void UnwrapLayerMask_RemovesLayerMaskWrapper()
		{
			var originalText = "LayerMask(3)";
			var expectedText = "3";
			var result = originalText.UnwrapLayerMask();
			Assert.AreEqual(expectedText, result);

			originalText = "LayerMask(-6)";
			expectedText = "-6";
			result = originalText.UnwrapLayerMask();
			Assert.AreEqual(expectedText, result);

			originalText = "LayerMask(12)";
			expectedText = "12";
			result = originalText.UnwrapLayerMask();
			Assert.AreEqual(expectedText, result);
		}
	}
}
