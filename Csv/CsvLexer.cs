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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv, file 'CsvLexer.cs'.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nortal.Utilities.Csv
{
	/// <summary>
	/// Converts an input string to a stream of lexemes used by Csv Parser.
	/// </summary>
	internal sealed class CsvLexer
	{
		internal CsvLexer(CsvSettings settings)
		{
			if (settings == null) { throw new ArgumentNullException("settings"); }

			this.Settings = settings;
			this.FieldDelimiter = new CsvLexeme(CsvSyntaxItem.Delimiter, settings.FieldDelimiter.ToString());
			this.LineSeparator = new CsvLexeme(CsvSyntaxItem.LineSeparator, settings.RowDelimiter);
			this.Quote = new CsvLexeme(CsvSyntaxItem.Quote, settings.QuotingCharacter.ToString());
		}

		private CsvSettings Settings { get; set; }

		private CsvLexeme FieldDelimiter { get; set; }
		private CsvLexeme LineSeparator { get; set; }
		private CsvLexeme Quote { get; set; }

		internal void ValidateSettings()
		{
			if (this.Settings.RowDelimiter == null) { throw new FormatException("Csv row delimiter cannot be null."); }
			if (this.Settings.RowDelimiter.Length > 2) { throw new FormatException("Csv row delimiter too long, maxium length: 2."); }
		}

		internal IEnumerable<CsvLexeme> Scan(string input)
		{
			using (StringReader reader = new StringReader(input))
			{
				foreach (var item in this.Scan(reader)) { yield return item; }
			}
		}

		internal IEnumerable<CsvLexeme> Scan(TextReader reader)
		{
			ValidateSettings();
			char rowSeparatorFirstChar = Settings.RowDelimiter.First();
			char? rowSeparatorSecondChar = Settings.RowDelimiter.Length > 1
				? Settings.RowDelimiter.Skip(1).FirstOrDefault()
				: (char?)null;

			int current;
			StringBuilder currentLexemeValue = new StringBuilder(50);
			while ((current = reader.Read()) != -1)
			{
				CsvLexeme specialSyntaxItem = new CsvLexeme();
				if (current == Settings.FieldDelimiter)
				{
					specialSyntaxItem = this.FieldDelimiter;
				}
				else if (current == Settings.QuotingCharacter)
				{
					specialSyntaxItem = this.Quote;
				}
				else if (current == rowSeparatorFirstChar)
				{
					if (rowSeparatorSecondChar == null) // single char line separator
					{
						specialSyntaxItem = this.LineSeparator;
					}
					else if (reader.Peek() == rowSeparatorSecondChar) // double char line separator
					{
						reader.Read();//just consume, since matching with rowSeparatorSecondChar is verified
						specialSyntaxItem = this.LineSeparator;
					}
				}

				if (specialSyntaxItem.Type != CsvSyntaxItem.NotSet)
				{
					//finish ongoing lexeme if present:
					if (currentLexemeValue.Length > 0)
					{
						yield return new CsvLexeme(CsvSyntaxItem.Text, currentLexemeValue.ToString());
						currentLexemeValue.Length = 0;
					}
					// yield current lexeme:
					yield return specialSyntaxItem;
				}
				else
				{
					currentLexemeValue.Append((char)current);
				}
			}
			if (currentLexemeValue.Length > 0)
			{
				yield return new CsvLexeme(CsvSyntaxItem.Text, currentLexemeValue.ToString());
			}
			yield return new CsvLexeme(CsvSyntaxItem.EndOfFile, null);
		}
	}
}
