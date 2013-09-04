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
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nortal.Utilities.Csv.Tests
{
	[TestClass]
	
	public class ParserTests
	{

		[TestMethod, TestCategory("Parser")]
		public void TestSimpleRow()
		{
			string csv = @"Value1,2,3,4";

			List<String> expectedValues = new List<string>();
			expectedValues.Add("Value1");
			expectedValues.Add("2");
			expectedValues.Add("3");
			expectedValues.Add("4");

			ParseAndValidateRow(csv, expectedRows: 1, rowToCompare: 0, expectedRowValues: expectedValues);
		}

		[TestMethod, TestCategory("Parser")]
		public void TestRowCountSimple()
		{
			string csv = @"row1,Value
row2,Value
row3,Value
row4,Value
row5,Value
row6,Value";

			ParseAndValidateRow(csv, expectedRows: 6);
		}

		[TestMethod, TestCategory("Parser")]
		public void TestRowCountTrailingNewLine()
		{
			string csv = @"row1,Value
";

			ParseAndValidateRow(csv, expectedRows: 1);
		}


		[TestMethod, TestCategory("Parser")]
		public void TestQuotedValueSimple()
		{
			string csv = @"1,""quotedValue"",3";

			List<String> expectedValues = new List<string>();
			expectedValues.Add("1");
			expectedValues.Add("quotedValue");
			expectedValues.Add("3");

			ParseAndValidateRow(csv, expectedRows: 1, rowToCompare: 0, expectedRowValues: expectedValues);
		}

		[TestMethod, TestCategory("Parser")]
		public void TestQuotedValueAdvanced()
		{
			string csv = @"1,""quoted
""""Value"""""",3";

			List<String> expectedValues = new List<string>();
			expectedValues.Add("1");
			expectedValues.Add("quoted\r\n\"Value\"");
			expectedValues.Add("3");

			ParseAndValidateRow(csv, expectedRows: 1, rowToCompare: 0, expectedRowValues: expectedValues);
		}


		[TestMethod, TestCategory("Parser")]
		public void TestEmptyFile()
		{
			string csv = @"";
			ParseAndValidateRow(csv, expectedRows: 0);
		}

		[TestMethod, TestCategory("Parser")]
		public void TestEmptyValue()
		{
			string csv = @",,";

			List<String> expectedValues = new List<string>();
			expectedValues.Add(null);
			expectedValues.Add(null);
			expectedValues.Add(null);

			ParseAndValidateRow(csv, expectedRows: 1, rowToCompare: 0, expectedRowValues: expectedValues);
		}

		private static void ParseAndValidateRow(string csv, int expectedRows, int? rowToCompare = null, List<String> expectedRowValues = null)
		{
			if (rowToCompare != null && rowToCompare >= expectedRows) { throw new ArgumentException("rowToComapare", "Cannot expect rowValues for row after expected row count"); }
			using (var parser = new CsvParser(csv))
			{
				String[][] rows = parser.ReadToEnd();
				Assert.AreEqual(expectedRows, rows.Length, "Expected different number or rows.");

				if (rowToCompare != null)
				{
					String[] row = rows[rowToCompare.Value];
					CollectionAssert.AreEqual(expectedRowValues, row);
				}
			}
		}
	}
}
