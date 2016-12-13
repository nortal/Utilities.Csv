using Microsoft.VisualStudio.TestTools.UnitTesting;
/*
	Copyright 2013 Imre Pühvel, AS Nortal
	
	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv.Tests, file 'ParserTests.cs'.
*/
using System;
using System.IO;

namespace Nortal.Utilities.Csv.Tests
{
	[TestClass]
	public class CustomisedSettingsTests
	{

		[TestMethod, TestCategory("CustomizedSettings")]
		public void TestNonDefaultRowDelimiterReading()
		{
			var settings = new CsvSettings() { RowDelimiter = "|" }; // also note, that it is single-char, not double char like default.
			const String csv = "1,2,3|4,\"| as data\",\n\r as data|7";

			using (var parser = new CsvParser(csv, settings))
			{
				String[][] rows = parser.ReadToEnd();
				Assert.AreEqual(3, rows.Length, "Incorrect number of rows");

				CollectionAssert.AreEqual(rows[0], new string[] { "1", "2", "3" });
				CollectionAssert.AreEqual(rows[1], new string[] { "4", "| as data", "\n\r as data" });
				CollectionAssert.AreEqual(rows[2], new string[] { "7" });
			}
		}

		[TestMethod, TestCategory("CustomizedSettings")]
		public void TestNonDefaultRowDelimiterWriting()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings() { RowDelimiter = "|" });
				csv.WriteLine("1", "2", "3");
				csv.WriteLine("4", "5", "6");

				const String expected = @"1,2,3|4,5,6|";

				Assert.AreEqual(expected, writer.ToString());
			}
		}


		[TestMethod, TestCategory("CustomizedSettings")]
		public void TestNonDefaultFieldDelimiterReading()
		{
			var settings = new CsvSettings() { FieldDelimiter = '\t' };
			const String csv = @"1	2	3
4	""	 as data""	, as data
7";

			using (var parser = new CsvParser(csv, settings))
			{
				String[][] rows = parser.ReadToEnd();
				Assert.AreEqual(3, rows.Length, "Incorrect number of rows");

				CollectionAssert.AreEqual(rows[0], new string[] { "1", "2", "3" });
				CollectionAssert.AreEqual(rows[1], new string[] { "4", "	 as data", ", as data" });
				CollectionAssert.AreEqual(rows[2], new string[] { "7" });
			}
		}

		[TestMethod, TestCategory("CustomizedSettings")]
		public void TestNonDefaultFieldDelimiterWriting()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings() { FieldDelimiter = '\t' });
				csv.WriteLine("1", "2", "3");
				csv.WriteLine("4", "5", "6");

				const String expected = @"1	2	3
4	5	6
";

				Assert.AreEqual(expected, writer.ToString());
			}
		}

		[TestMethod, TestCategory("CustomizedSettings")]
		public void TestNonDefaultQuotingCharacterReading()
		{
			var settings = new CsvSettings() { QuotingCharacter = '\'' };
			const String csv = @"1,2,3
4,''' as data',"" as data
7";

			using (var parser = new CsvParser(csv, settings))
			{
				String[][] rows = parser.ReadToEnd();
				Assert.AreEqual(3, rows.Length, "Incorrect number of rows");

				CollectionAssert.AreEqual(rows[0], new string[] { "1", "2", "3" });
				CollectionAssert.AreEqual(rows[1], new string[] { "4", "' as data", "\" as data" });
				CollectionAssert.AreEqual(rows[2], new string[] { "7" });
			}
		}

		[TestMethod, TestCategory("CustomizedSettings")]
		public void TestNonDefaultQuotingCharacterWriting()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings() { QuotingCharacter = '\'' });
				csv.WriteLine("1", ",", Environment.NewLine);
				csv.WriteLine("'quotesAsData'");

				const String expected = @"1,',','
'
'''quotesAsData'''
";

				Assert.AreEqual(expected, writer.ToString());
			}
		}

		[TestMethod, TestCategory("CustomizedSettings")]
		public void TestQuoteAllMode()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings() { QuotingCharacter = '\'', QuotingMode = CsvQuotingMode.QuoteAll });
				csv.WriteLine("1", "", null);
				const String expected =
@"'1','',''
";
				Assert.AreEqual(expected, writer.ToString());
			}
		}
	}
}
