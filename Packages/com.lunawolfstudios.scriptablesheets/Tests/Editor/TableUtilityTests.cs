using LunaWolfStudiosEditor.ScriptableSheets.Tables;
using NUnit.Framework;
using System.Text;
using UnityEngine;

namespace LunaWolfStudiosEditor.ScriptableSheets.EditorTests
{
	public class TableUtilityTests
	{
		[Test]
		public void FromFlatFileFormat_UpdatesPropertyTable()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "row1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_KeepsTrailingSpaces()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "  row1-col1  ,  row1-col2  \n  row2-col1  ,  row2-col2  \n  row3-col1  ,  row3-col2  ";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("  row1-col1  ", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("  row1-col2  ", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("  row2-col1  ", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("  row2-col2  ", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("  row3-col1  ", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("  row3-col2  ", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_RemovesEmptyEntries()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "row1-col1,row1-col2\n\nrow2-col1,row2-col2\n\n\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				RemoveEmptyRows = true,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_FromSpecifiedCoordinate()
		{
			var propertyTable = GetEmptyTable(3, 3);
			var flatFileContent = "row2-col2,row2-col3\nrow3-col2,row3-col3";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				FirstRowIndex = 1,
				FirstColumnIndex = 1,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual(string.Empty, propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 2).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col3", propertyTable.Get(1, 2).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col3", propertyTable.Get(2, 2).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_WithCustomDelimiters()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "row1-col1YYrow1-col2XXrow2-col1YYrow2-col2XXrow3-col1YYrow3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "XX",
				ColumnDelimiter = "YY",
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_IgnoresWrappingWhenContentUnwrapped()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "row1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_HandlesMismatchedWrapping()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\"row1-col1\",\"row1-col2\"\n\"row2-col1\",row2-col2\nrow3-col1,\"row3-col2\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValues()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\"row1-col1\",\"row1-col2\"\n\"row2-col1\",\"row2-col2\"\n\"row3-col1\",\"row3-col2\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValuesWithWrappedHeaders()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\"column[1]\",\"column[2]\"\n\"row1-col1\",\"row1-col2\"\n\"row2-col1\",\"row2-col2\"\n\"row3-col1\",\"row3-col2\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
				ColumnHeaders = new string[] { "column[1]", "column[2]" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValuesWithMissingHeaders()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\"row1-col1\",\"row1-col2\"\n\"row2-col1\",\"row2-col2\"\n\"row3-col1\",\"row3-col2\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
				ColumnHeaders = new string[] { "column-1", "column-2" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValuesWithSingleQuotes()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "'\"row1-col1\"','\"row1-col2\"'\n'\"row2-col1\"','\"row2-col2\"'\n'\"row3-col1\"','\"row3-col2\"'";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.SingleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("\"row1-col1\"", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("\"row1-col2\"", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("\"row2-col1\"", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("\"row2-col2\"", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("\"row3-col1\"", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("\"row3-col2\"", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValuesWithCurlyBraces()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "{row1-col1},{row1-col2}\n{row2-col1},{row2-col2}\n{row3-col1},{row3-col2}";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.CurlyBraces,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValuesWithExtraCurlyBraces()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "{{row1-col1},{{row1-col2}\n{{row2-col1},{{row2-col2}\n{{row3-col1},{{row3-col2}";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.CurlyBraces,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("{row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("{row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("{row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("{row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("{row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("{row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValuesAndKeepsWrappedTrailingSpaces()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "  \"  row1-col1  \",\"  row1-col2  \"  \n  \"  row2-col1  \",\"  row2-col2  \"  \n  \"  row3-col1  \",\"  row3-col2  \"  ";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("  row1-col1  ", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("  row1-col2  ", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("  row2-col1  ", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("  row2-col2  ", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("  row3-col1  ", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("  row3-col2  ", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsEmptyValues()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\"row1-col1\",\"\"\n\"row2-col1\",\"row2-col2\"\n\"row3-col1\",\"\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValues_WhenColumnDelimHasExtraChars()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\"row1-col1\"+_+\"row1-col2\"\n\"row2-col1\"+_+\"row2-col2\"\n\"row3-col1\"+_+\"row3-col2\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = "+_+",
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValuesWhenMoreCellsThanContent()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\"row1-col1\",\"row1-col2\"\n\"row2-col1\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValues_FromSpecifiedCoordinate()
		{
			var propertyTable = GetEmptyTable(3, 3);
			var flatFileContent = "\"row2-col2\",\"row2-col3\"\n\"row3-col2\",\"row3-col3\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				FirstRowIndex = 1,
				FirstColumnIndex = 1,
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual(string.Empty, propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 2).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col3", propertyTable.Get(1, 2).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col3", propertyTable.Get(2, 2).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_UnwrapsValues_FromSpecifiedCoordinate_HandlesOverflow()
		{
			var propertyTable = GetEmptyTable(3, 3);
			var flatFileContent = "\"row2-col2\",\"row2-col3\",\"row2-col4\"\n\"row3-col2\",\"row3-col3\",\"row3-col4\"";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				FirstRowIndex = 1,
				FirstColumnIndex = 1,
				WrapOption = WrapOption.DoubleQuotes,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual(string.Empty, propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 2).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col3", propertyTable.Get(1, 2).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col3", propertyTable.Get(2, 2).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_RemovesHeaders()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "column-1,column-2\nrow1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1", "column-2" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_RemovesHeadersWhenSkippingFirstColumn()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "column-2\nrow1-col2\nrow2-col2\nrow3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1", "column-2" },
				FirstColumnIndex = 1,
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual(string.Empty, propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_RemovesHeadersWhenSingleMatch()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "column-2\nrow1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1", "column-2", "column-3" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_RemovesHeadersWhenSingleColumnMatch()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "column-1,column-2,column-3\nrow1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-2" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_RemovesHeadersWhenExtraContent()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "\t\t\tcolumn-1,column-2,column-3\t\t\t\nrow1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1,column-2,column-3" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_RemovesHeadersWhenSingleRow()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "column-1,column-2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1", "column-2" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual(string.Empty, propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_IgnoresHeaderWhenSingleMatch()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "row1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1", "row1-col1" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_IgnoresHeadersWhenMismatch()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "row1-col1,row1-col2\nrow2-col1,row2-col2\nrow3-col1,row3-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1", "column-2" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("row2-col1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("row2-col2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("row3-col1", propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual("row3-col2", propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromFlatFileFormat_IgnoresHeadersWhenMismatchAndSingleRow()
		{
			var propertyTable = GetEmptyTable(3, 2);
			var flatFileContent = "row1-col1,row1-col2";

			var formatSettings = new FlatFileFormatSettings()
			{
				RowDelimiter = "\n",
				ColumnDelimiter = ",",
				ColumnHeaders = new string[] { "column-1", "column-2" },
			};

			propertyTable.FromFlatFileFormat(flatFileContent, formatSettings);

			Assert.AreEqual("row1-col1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("row1-col2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 0).GetProperty(formatSettings));
			Assert.AreEqual(string.Empty, propertyTable.Get(2, 1).GetProperty(formatSettings));
		}

		[Test]
		public void FromJsonFlatFormat_UpdatesPropertyTable()
		{
			var propertyTable = GetEmptyTable(2, 3, true);
			var flatFileContent = new StringBuilder();
			flatFileContent.AppendLine("{");
			flatFileContent.AppendLine("  \"Object0\": {");
			flatFileContent.AppendLine("    \"Property1\": \"Object0-Value1\",");
			flatFileContent.AppendLine("    \"Property2\": \"Object0-Value2\",");
			flatFileContent.AppendLine("    \"Property3\": \"Object0-Value3\"");
			flatFileContent.AppendLine("  },");
			flatFileContent.AppendLine("  \"Object1\": {");
			flatFileContent.AppendLine("    \"Property1\": \"Object1-Value1\",");
			flatFileContent.AppendLine("    \"Property2\": \"Object1-Value2\",");
			flatFileContent.AppendLine("    \"Property3\": \"Object1-Value3\"");
			flatFileContent.AppendLine("  }");
			flatFileContent.Append("}");
			var formatSettings = new FlatFileFormatSettings();
			propertyTable.FromJsonFormat(flatFileContent.ToString(), JsonSerializationFormat.Flat, formatSettings);
			Assert.AreEqual("Object0-Value1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("Object0-Value2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("Object0-Value3", propertyTable.Get(0, 2).GetProperty(formatSettings));
			Assert.AreEqual("Object1-Value1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("Object1-Value2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("Object1-Value3", propertyTable.Get(1, 2).GetProperty(formatSettings));
		}

		[Test]
		public void FromJsonFlatFormat_UpdatesPropertyTable_WithMismatchedPropertyPaths()
		{
			var propertyTable = GetEmptyTable(2, 3, true);
			var flatFileContent = new StringBuilder();
			flatFileContent.AppendLine("{");
			flatFileContent.AppendLine("  \"Object0\": {");
			flatFileContent.AppendLine("    \"1\": \"Object0-Value1\",");
			flatFileContent.AppendLine("    \"2\": \"Object0-Value2\",");
			flatFileContent.AppendLine("    \"3\": \"Object0-Value3\"");
			flatFileContent.AppendLine("  },");
			flatFileContent.AppendLine("  \"Object1\": {");
			flatFileContent.AppendLine("    \"1\": \"Object1-Value1\",");
			flatFileContent.AppendLine("    \"2\": \"Object1-Value2\",");
			flatFileContent.AppendLine("    \"3\": \"Object1-Value3\"");
			flatFileContent.AppendLine("  }");
			flatFileContent.Append("}");
			var formatSettings = new FlatFileFormatSettings();
			propertyTable.FromJsonFormat(flatFileContent.ToString(), JsonSerializationFormat.Flat, formatSettings);
			Assert.AreEqual("Object0-Value1", propertyTable.Get(0, 0).GetProperty(formatSettings));
			Assert.AreEqual("Object0-Value2", propertyTable.Get(0, 1).GetProperty(formatSettings));
			Assert.AreEqual("Object0-Value3", propertyTable.Get(0, 2).GetProperty(formatSettings));
			Assert.AreEqual("Object1-Value1", propertyTable.Get(1, 0).GetProperty(formatSettings));
			Assert.AreEqual("Object1-Value2", propertyTable.Get(1, 1).GetProperty(formatSettings));
			Assert.AreEqual("Object1-Value3", propertyTable.Get(1, 2).GetProperty(formatSettings));
		}

		[Test]
		public void ToFlatFileFormat_ReturnsCorrectFormat_WhenNoSettingsProvided()
		{
			var table = new Table<ITableProperty>(2, 3);
			table.Set(0, 0, new TablePropertyMock("Object0", "myInt", "1"));
			table.Set(0, 1, new TablePropertyMock("Object0", "myInt", "2"));
			table.Set(0, 2, new TablePropertyMock("Object0", "myInt", "3"));
			table.Set(1, 0, new TablePropertyMock("Object1", "myInt", "4"));
			table.Set(1, 1, new TablePropertyMock("Object1", "myInt", "5"));
			table.Set(1, 2, new TablePropertyMock("Object1", "myInt", "6"));

			var expected = "123456";
			var result = table.ToFlatFileFormat(new FlatFileFormatSettings());
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ToFlatFileFormat_ReturnsCorrectFormat_WhenPropertyIsNull()
		{
			var table = new Table<ITableProperty>(2, 3);
			table.Set(0, 0, new TablePropertyMock("Object0", "myInt", "1"));
			table.Set(0, 1, null);
			table.Set(0, 2, new TablePropertyMock("Object0", "myInt", "3"));
			table.Set(1, 0, null);
			table.Set(1, 1, new TablePropertyMock("Object1", "myInt", "5"));
			table.Set(1, 2, null);

			var expected = "135";
			var result = table.ToFlatFileFormat(new FlatFileFormatSettings());
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ToFlatFileFormat_ReturnsCorrectFormat_WithCustomSettings()
		{
			var table = new Table<ITableProperty>(2, 3);
			table.Set(0, 0, new TablePropertyMock("Object0", "myInt", "1"));
			table.Set(0, 1, new TablePropertyMock("Object0", "myInt", "2"));
			table.Set(0, 2, new TablePropertyMock("Object0", "myInt", "3"));
			table.Set(1, 0, new TablePropertyMock("Object1", "myInt", "4"));
			table.Set(1, 1, new TablePropertyMock("Object1", "myInt", "5"));
			table.Set(1, 2, new TablePropertyMock("Object1", "myInt", "6"));

			var settings = new FlatFileFormatSettings
			{
				FirstColumnIndex = 0,
				FirstRowIndex = 0,
				ColumnDelimiter = "|",
				RowDelimiter = "\n",
				WrapOption = WrapOption.None,
				ColumnHeaders = new string[] { "A", "B", "C" }
			};

			var expected = "A|B|C\n1|2|3\n4|5|6";
			var result = table.ToFlatFileFormat(settings);
			Assert.AreEqual(expected, result);

			settings.WrapOption = WrapOption.DoubleQuotes;
			expected = "\"A\"|\"B\"|\"C\"\n\"1\"|\"2\"|\"3\"\n\"4\"|\"5\"|\"6\"";
			result = table.ToFlatFileFormat(settings);
			Assert.AreEqual(expected, result);

			settings.FirstColumnIndex = 1;
			expected = "\"B\"|\"C\"\n\"2\"|\"3\"\n\"5\"|\"6\"";
			result = table.ToFlatFileFormat(settings);
			Assert.AreEqual(expected, result);

			settings.FirstRowIndex = 1;
			expected = "\"B\"|\"C\"\n\"5\"|\"6\"";
			result = table.ToFlatFileFormat(settings);
			Assert.AreEqual(expected, result);

			settings.FirstRowOnly = true;
			expected = "\"B\"|\"C\"\n\"5\"|\"6\"";
			result = table.ToFlatFileFormat(settings);
			Assert.AreEqual(expected, result);

			settings.FirstColumnOnly = true;
			expected = "\"B\"\n\"5\"";
			result = table.ToFlatFileFormat(settings);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ToFlatFileFormat_ReturnsEmptyString_WhenNoData()
		{
			var table = new Table<ITableProperty>(0, 0);
			var settings = new FlatFileFormatSettings();
			var result = table.ToFlatFileFormat(settings);
			Assert.AreEqual(string.Empty, result);
		}

		[Test]
		public void ToJsonFormat_ReturnsCorrectJsonFlatFormat()
		{
			var propertyTable = GetPopulatedTable();
			var expected = new StringBuilder();
			expected.AppendLine("{");
			expected.AppendLine("  \"Object0\": {");
			expected.AppendLine("    \"Property1\": \"Value1\",");
			expected.AppendLine("    \"Property2\": \"Value2\",");
			expected.AppendLine("    \"Property3\": \"Value3\"");
			expected.AppendLine("  },");
			expected.AppendLine("  \"Object1\": {");
			expected.AppendLine("    \"Property1\": \"Value1\",");
			expected.AppendLine("    \"Property2\": \"Value2\",");
			expected.AppendLine("    \"Property3\": \"Value3\"");
			expected.AppendLine("  }");
			expected.Append("}");
			var settings = new FlatFileFormatSettings
			{
				FirstColumnIndex = 1
			};
			var result = propertyTable.ToJsonFormat(JsonSerializationFormat.Flat, settings);
			Assert.AreEqual(expected.ToString(), result);
		}

		[Test]
		public void ToJsonFormat_ReturnsCorrectJsonFlatFormat_WhenPropertyIsNull()
		{
			var propertyTable = GetPopulatedTable(true);
			var expected = new StringBuilder();
			expected.AppendLine("{");
			expected.AppendLine("  \"Object0\": {");
			expected.AppendLine("    \"Property1\": \"Value1\",");
			expected.AppendLine("    \"Property3\": \"Value3\"");
			expected.AppendLine("  },");
			expected.AppendLine("  \"Object1\": {");
			expected.AppendLine("    \"Property1\": \"Value1\",");
			expected.AppendLine("    \"Property3\": \"Value3\"");
			expected.AppendLine("  }");
			expected.Append("}");
			var settings = new FlatFileFormatSettings
			{
				FirstColumnIndex = 1
			};
			var result = propertyTable.ToJsonFormat(JsonSerializationFormat.Flat, settings);
			Assert.AreEqual(expected.ToString(), result);
		}

		[Test]
		public void ToJsonFormat_FlatFormat_ReturnsEmptyJson_WhenNoData()
		{
			var table = new Table<ITableProperty>(0, 0);
			var settings = new FlatFileFormatSettings();
			var result = table.ToJsonFormat(JsonSerializationFormat.Flat, settings);
			Assert.AreEqual("{}", result);

			table = new Table<ITableProperty>(10, 0);
			settings = new FlatFileFormatSettings
			{
				FirstColumnIndex = 1
			};
			result = table.ToJsonFormat(JsonSerializationFormat.Flat, settings);
			Assert.AreEqual("{}", result);
		}

		[Test]
		public void ToJsonFormat_ReturnsCorrectJsonHierarchyFormat()
		{
			var scriptableObject0 = ScriptableObject.CreateInstance<MyScriptableObject>();
			scriptableObject0.name = "Object0";
			scriptableObject0.myInt = 0;
			scriptableObject0.myString = "Hello World!";
			scriptableObject0.myBool = true;

			var scriptableObject1 = ScriptableObject.CreateInstance<MyScriptableObject>();
			scriptableObject1.name = "Object1";
			scriptableObject1.myInt = 1;
			scriptableObject1.myString = "Goodbye World!";
			scriptableObject1.myBool = false;

			var propertyTable = GetScriptableObjectTable(scriptableObject0, scriptableObject1);
			var expected = new StringBuilder();
			expected.AppendLine("{");
			expected.AppendLine("  \"Object0\": {");
			expected.AppendLine("    \"myInt\": 0,");
			expected.AppendLine("    \"myString\": \"Hello World!\",");
			expected.AppendLine("    \"myBool\": true");
			expected.AppendLine("  },");
			expected.AppendLine("  \"Object1\": {");
			expected.AppendLine("    \"myInt\": 1,");
			expected.AppendLine("    \"myString\": \"Goodbye World!\",");
			expected.AppendLine("    \"myBool\": false");
			expected.AppendLine("  }");
			expected.Append("}");
			var result = propertyTable.ToJsonFormat(JsonSerializationFormat.Hierarchy, new FlatFileFormatSettings());
			Assert.AreEqual(expected.ToString(), result);
		}

		[Test]
		public void ToJsonFormat_HierarchyFormat_ReturnsEmptyJson_WhenNoData()
		{
			var table = new Table<ITableProperty>(0, 0);
			var settings = new FlatFileFormatSettings();
			var result = table.ToJsonFormat(JsonSerializationFormat.Hierarchy, settings);
			Assert.AreEqual("{}", result);

			table = new Table<ITableProperty>(10, 0);
			settings = new FlatFileFormatSettings
			{
				FirstColumnIndex = 1
			};
			result = table.ToJsonFormat(JsonSerializationFormat.Hierarchy, settings);
			Assert.AreEqual("{}", result);
		}

		private Table<ITableProperty> GetPopulatedTable(bool withNullProperty = false)
		{
			var propertyTable = new Table<ITableProperty>(2, 4);
			for (var x = 0; x < 2; x++)
			{
				for (var y = 1; y < 4; y++)
				{
					if (withNullProperty && y % 2 == 0)
					{
						propertyTable.Set(x, y, null);
					}
					else
					{
						var mockObject = new TablePropertyMock($"Object{x}", $"Property{y}", $"Value{y}");
						propertyTable.Set(x, y, mockObject);
					}
				}
			}
			return propertyTable;
		}

		private Table<ITableProperty> GetScriptableObjectTable(params ScriptableObject[] scriptableObjects)
		{
			var propertyTable = new Table<ITableProperty>(2, 4);
			for (var x = 0; x < 2; x++)
			{
				var mockObject = new TablePropertyMock(scriptableObjects[x]);
				propertyTable.Set(x, 1, mockObject);
			}
			return propertyTable;
		}

		private Table<ITableProperty> GetEmptyTable(int rows, int columns, bool useIndexedPropertyPaths = false)
		{
			var propertyTable = new Table<ITableProperty>(rows, columns);
			var formatSettings = new FlatFileFormatSettings();
			for (var row = 0; row < rows; row++)
			{
				for (var column = 0; column < columns; column++)
				{
					var propertyPath = useIndexedPropertyPaths ? $"Property{column + 1}" : string.Empty;
					var tableProperty = new TablePropertyMock(propertyPath);
					tableProperty.SetProperty(string.Empty, formatSettings);
					propertyTable.Set(row, column, tableProperty);
				}
			}
			return propertyTable;
		}

		private class MyScriptableObject : ScriptableObject
		{
			public int myInt;
			public string myString;
			public bool myBool;
		}

		private class TablePropertyMock : ITableProperty
		{
			private readonly Object m_RootObject;
			public Object RootObject => m_RootObject;

			private readonly string m_PropertyPath;
			public string PropertyPath => m_PropertyPath;

			public string ControlName => string.Empty;

			private string m_Value;

			public TablePropertyMock()
			{
				m_PropertyPath = string.Empty;
			}

			public TablePropertyMock(string propertyPath)
			{
				m_PropertyPath = propertyPath;
			}

			public TablePropertyMock(Object rootObject)
			{
				m_RootObject = rootObject;
			}

			public TablePropertyMock(string name, string path)
			{
				m_RootObject = new GameObject(name);
				m_PropertyPath = path;
			}

			public TablePropertyMock(string name, string path, string value)
			{
				m_RootObject = new GameObject(name);
				m_PropertyPath = path;
				m_Value = value;
			}

			public string GetProperty(FlatFileFormatSettings formatSettings)
			{
				return m_Value;
			}

			public void SetProperty(string value, FlatFileFormatSettings formatSettings)
			{
				m_Value = value;
			}

			public bool IsInputFieldProperty(bool isScriptableObject)
			{
				return false;
			}

			public bool NeedsSelectionBorder(bool lockNames = false)
			{
				return true;
			}
		}
	}
}
