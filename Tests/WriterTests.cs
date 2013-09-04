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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv.Tests, file 'WriterTests.cs'.
*/
using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nortal.Utilities.Csv.Tests
{
	[TestClass]
	public class WriterTest
	{
		[TestMethod]
		public void TestWritingSimple()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings());
				csv.WriteLine("1", "2", "3");
				csv.WriteLine("4", "5", "6");

				String expected = @"1,2,3" + Environment.NewLine + "4,5,6" + Environment.NewLine;

				Assert.AreEqual(expected, writer.ToString());
			}
		}

		[TestMethod]
		public void TestWritingSpecialChars()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings());
				csv.WriteLine("simple", "\"", Environment.NewLine, ",");

				String expected = "simple,\"\"\"\",\"\r\n\",\",\"" + Environment.NewLine;

				Assert.AreEqual(expected, writer.ToString());
			}
		}

		[TestMethod]
		public void TestWritingRawLine()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings());
				csv.WriteRawLine("1,2,3");
				csv.WriteRawLine("4,5,6" + Environment.NewLine + "7,8,9");

				String expected = @"1,2,3" + Environment.NewLine
					+ "4,5,6" + Environment.NewLine
					+ "7,8,9" + Environment.NewLine;

				Assert.AreEqual(expected, writer.ToString());
			}
		}


		[TestMethod]
		public void TestWritingSingleValuesFirst()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings());
				csv.Write("1");
				csv.Write("2");
				csv.WriteLine("3");
				csv.Write("4");
				csv.WriteLine("5", "6");

				String expected = @"1,2,3" + Environment.NewLine
					+ "4,5,6" + Environment.NewLine;

				Assert.AreEqual(expected, writer.ToString());
			}
		}

		[TestMethod]
		public void TestWritingFormatting()
		{
			using (var writer = new StringWriter())
			{
				var csv = new CsvWriter(writer, new CsvSettings());
				csv.FormattingCulture = CultureInfo.GetCultureInfo("et-EE");

				csv.Write(100.33333, "F2");
				csv.Write(new DateTime(2012, 02, 28), "d");

				String expected = @"""100,33"",28.02.2012";

				Assert.AreEqual(expected, writer.ToString());
			}
		}
	}
}
