using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using NUnit.Framework;
using System.Text;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	[TestFixture]
	public class StringBuilderUtilityTests
	{
		[Test]
		public void Wrap_WithOpenAndCloseChars_ShouldWrapValueCorrectly()
		{
			var sb = new StringBuilder();
			sb.Wrap("text", '(', ')');
			Assert.AreEqual("(text)", sb.ToString());
		}

		[Test]
		public void Wrap_WithWrapperObject_ShouldWrapValueCorrectly()
		{
			var sb = new StringBuilder();
			var wrapper = new Wrapper('[', ']');
			sb.Wrap("value", wrapper);
			Assert.AreEqual("[value]", sb.ToString());
		}

		[Test]
		public void Wrap_WithWrapperObjectSingleChar_ShouldWrapValueCorrectly()
		{
			var sb = new StringBuilder();
			var wrapper = new Wrapper('\"');
			sb.Wrap("value", wrapper);
			Assert.AreEqual("\"value\"", sb.ToString());
		}

		[Test]
		public void Wrap_WithEmptyString_ShouldWrapEmptyStringCorrectly()
		{
			var sb = new StringBuilder();
			sb.Wrap(string.Empty, '<', '>');
			Assert.AreEqual("<>", sb.ToString());
		}

		[Test]
		public void Wrap_WithNullValue_ShouldWrapNullValue()
		{
			var sb = new StringBuilder();
			sb.Wrap(null, '{', '}');
			Assert.AreEqual("{}", sb.ToString());
		}
	}
}
