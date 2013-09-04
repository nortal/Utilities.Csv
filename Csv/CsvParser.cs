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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv, file 'CsvParser.cs'.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Nortal.Utilities.Csv
{
	/// <summary>
	/// CSV parser based on <see cref="http://tools.ietf.org/rfc/rfc4180.txt">RFC4180</see>.
	/// Parses csv input into rows and row items, automatically unescaping when required.
	/// </summary>
	public class CsvParser : IDisposable
	{
		public CsvParser(String csv)
			: this(csv, new CsvSettings())
		{
		}

		public CsvParser(String csv, CsvSettings settings)
		{
			if (csv == null) { throw new ArgumentNullException("csv"); }
			if (settings == null) { throw new ArgumentNullException("settings"); }

			var reader = new StringReader(csv);
			this.OwnedReader = reader;
			Initialize(reader, settings);
		}

		public CsvParser(TextReader reader)
			: this(reader, new CsvSettings())
		{
		}

		public CsvParser(TextReader reader, CsvSettings settings)
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }
			if (settings == null) { throw new ArgumentNullException("settings"); }

			Initialize(reader, settings);
		}

		private void Initialize(TextReader reader, CsvSettings settings)
		{
			var lexer = new CsvLexer(settings);
			this.Lexemes = lexer.Scan(reader).GetEnumerator();
		}

		private IDisposable OwnedReader { get; set; }
		private IEnumerator<CsvLexeme> Lexemes { get; set; }

		private CsvLexeme CurrentLexeme { get { return this.Lexemes.Current; } }
		private CsvSyntaxItem CurrentLexemeType { get { return this.CurrentLexeme.Type; } }

		/// <summary>
		/// Indicates that the csv file may contain more rows. Note that the remaining row may be an empty row (created by a trailing newline).
		/// </summary>
		public Boolean HasMoreRows { get { return this.CurrentLexemeType != CsvSyntaxItem.EndOfFile; } }

		/// <summary>
		/// Parses input to end returning all remaining rows. If file ends with trailing newline then last empty row is automatically omitted.
		/// </summary>
		/// <returns></returns>
		public String[][] ReadToEnd()
		{
			var rows = new List<string[]>();
			while (HasMoreRows)
			{
				var nextRowValues = ReadNextRow();
				if (HasMoreRows || nextRowValues != null)
				{
					rows.Add(nextRowValues);
				}
			}
			return rows.ToArray();
		}

		/// <summary>
		/// Reads next line from csv file.
		/// </summary>
		/// <returns>Collection of values -or- null for empty row</returns>
		public String[] ReadNextRow()
		{
			if (!this.HasMoreRows) { return null; }
			String[] rowValues = ReadNextCsvValues().ToArray();

			//handle the case of trailing newline 
			if (!HasMoreRows && rowValues.Length == 1 && rowValues[0] == null) { return null; }
			return rowValues;
		}

		private IEnumerable<String> ReadNextCsvValues()
		{
			do
			{
				String nextValue = ReadNextCsvValue();
				yield return nextValue;
			} while (this.CurrentLexemeType == CsvSyntaxItem.Delimiter);
		}

		private String ReadNextCsvValue()
		{
			String nextValue = null;
			if (this.CurrentLexemeType == CsvSyntaxItem.EndOfFile) { return null; }

			Boolean isQuoted = false;
			Boolean IsQuoteModeOn = false;
			while (true)
			{
				if (!this.Lexemes.MoveNext()) { throw new InvalidOperationException("Unexpected end of lexer output, expected EndOfFile token."); }

				if (IsQuoteModeOn)
				{
					if (this.CurrentLexeme.Type == CsvSyntaxItem.Quote) { IsQuoteModeOn = false; }
					else { nextValue += this.CurrentLexeme.Value; }
					continue;
				}

				Debug.Assert(IsQuoteModeOn == false);
				switch (this.CurrentLexemeType)
				{
					case CsvSyntaxItem.Delimiter:
					case CsvSyntaxItem.LineSeparator:
					case CsvSyntaxItem.EndOfFile:
						return nextValue;
					case CsvSyntaxItem.Quote:
						Debug.Assert(IsQuoteModeOn == false);
						if (isQuoted)
						{
							nextValue += this.CurrentLexeme.Value;
						}
						else
						{
							isQuoted = true;
						}
						IsQuoteModeOn = true;
						continue;
					case CsvSyntaxItem.Text:
						nextValue += this.CurrentLexeme.Value;
						continue;
					default:
						throw new NotSupportedException(this.CurrentLexemeType.ToString());
				}
			}
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (disposing)
			{
				this.Lexemes.Dispose();
				var ownedReader = this.OwnedReader;
				if (ownedReader != null)
				{
					this.OwnedReader = null;
					ownedReader.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#region static API
		/// <summary>
		/// Splits a single row to a collection of csv items.
		/// </summary>
		/// <param name="singleRow"></param>
		/// <returns>values in csv row</returns>
		public static String[] ParseSingleRow(String singleRow)
		{
			return ParseSingleRow(singleRow, new CsvSettings());
		}

		/// <summary>
		/// Splits a single row to a collection of csv items.
		/// </summary>
		/// <param name="singleRow"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static String[] ParseSingleRow(String singleRow, CsvSettings settings)
		{
			if (singleRow == null) { throw new ArgumentNullException("singleRow"); }
			if (settings == null) { throw new ArgumentNullException("settings"); }

			using (CsvParser parser = new CsvParser(singleRow, settings))
			{
				String[] firstRow = parser.ReadNextRow();
				return firstRow;
			}
		}

		/// <summary>
		/// Parses csv file to rows and values.
		/// </summary>
		/// <param name="csv"></param>
		/// <returns></returns>
		public static String[][] Parse(string csv)
		{
			return Parse(csv, new CsvSettings());
		}

		/// <summary>
		/// Parses csv file to rows and values.
		/// </summary>
		/// <param name="csv"></param>
		/// <returns></returns>
		public static String[][] Parse(string csv, CsvSettings settings)
		{
			if (csv == null) { throw new ArgumentNullException("csv"); }
			if (settings == null) { throw new ArgumentNullException("settings"); }

			using (CsvParser parser = new CsvParser(csv, settings))
			{
				return parser.ReadToEnd();
			}
		}
		#endregion
	}
}
