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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv.Tests, file 'LexerTests.cs'.
*/
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nortal.Utilities.Csv.Tests
{
	[TestClass]
	public class LexerTests
	{


		[TestMethod, TestCategory("Lexer")]
		public void TestLexer()
		{
			String csv = @",OtherText,
""""AndMoreText";

			var expected = new CsvLexeme[]
			{
				new CsvLexeme(CsvSyntaxItem.Delimiter, ","),
				new CsvLexeme(CsvSyntaxItem.Text, "OtherText"),
				new CsvLexeme(CsvSyntaxItem.Delimiter, ","),
				new CsvLexeme(CsvSyntaxItem.LineSeparator, "\r\n"),
				new CsvLexeme(CsvSyntaxItem.Quote, "\""),
				new CsvLexeme(CsvSyntaxItem.Quote, "\""),
				new CsvLexeme(CsvSyntaxItem.Text, "AndMoreText"),
				new CsvLexeme(CsvSyntaxItem.EndOfFile, null)
			};

			var lexer = new CsvLexer(new CsvSettings());
			var actual = lexer.Scan(csv).ToArray();

			Func<CsvLexeme, CsvSyntaxItem> typeSelector = (lexeme => lexeme.Type);
			Func<CsvLexeme, String> valueSelector = (lexeme => lexeme.Value);

			CollectionAssert.AreEqual(expected.Select(typeSelector).ToArray(), actual.Select(typeSelector).ToArray());
			CollectionAssert.AreEqual(expected.Select(valueSelector).ToArray(), actual.Select(valueSelector).ToArray());
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification="FileStream can be safely disposed multiple times.")]
		public void TestLexerCanReadFileStream()
		{
			var lexer = new CsvLexer(new CsvSettings());

			var executionPath = new FileInfo(Assembly.GetAssembly(this.GetType()).Location);
			String sampleFilePath = Path.Combine(executionPath.Directory.FullName, "Sample.csv");

			using (var fileStream = File.OpenRead(sampleFilePath))
			using (var reader = new StreamReader(fileStream))
			{
				var lexemes = lexer.Scan(reader).ToArray();
				int actualTextLexemes = lexemes.Where(i => i.Type == CsvSyntaxItem.Text).Count();
				Assert.AreEqual(4, actualTextLexemes, "Different number of values captured.");
				Assert.AreEqual(CsvSyntaxItem.EndOfFile, lexemes.Last().Type);
			}
		}
	}
}
