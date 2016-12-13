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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv, file 'CsvWriter.cs'.
*/
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Nortal.Utilities.Csv
{
	/// <summary>
	/// Simple class to help writing correct CSV files.
	/// </summary>
	public class CsvWriter
	{
		public CsvWriter(TextWriter writer)
			: this(writer, new CsvSettings())
		{
		}

		public CsvWriter(TextWriter writer, CsvSettings settings)
		{
			this.Writer = writer;
			this.Settings = settings;
			this.LineAlreadyStarted = false;
			this.FormattingCulture = Thread.CurrentThread.CurrentCulture;
		}

		private CsvSettings Settings { get; set; }
		private TextWriter Writer { get; set; }
		private Boolean LineAlreadyStarted { get; set; }

		/// <summary>
		/// Culture to use when automatically formatting IFormattables.
		/// </summary>
		public CultureInfo FormattingCulture { get; set; }

		#region writing entire line
		/// <summary>
		/// Writes given preformatted string directly to file followed by row delimiter. No escaping will be done.
		/// </summary>
		/// <param name="line"></param>
		public void WriteRawLine(String line)
		{
			if (line == null) { throw new ArgumentNullException("line"); }

			this.Writer.Write(line);
			this.Writer.Write(Settings.RowDelimiter);
			this.LineAlreadyStarted = false;
		}

		/// <summary>
		/// Writes a collection of values to CSV followed by row delimiter, escaping special characters when necessary.
		/// </summary>
		/// <param name="values"></param>
		public void WriteLine(params String[] values)
		{
			if (values == null) { throw new ArgumentNullException("values"); }
			foreach (var value in values)
			{
				this.Write(value);
			}
			this.WriteRawLine(String.Empty);
		}

		/// <summary>
		/// Writes a collection of values to CSV, applying default formatting and escaping special characters when necessary, and followed by a row delimiter..
		/// </summary>
		/// <param name="values"></param>
		public void WriteLine(params IFormattable[] values)
		{
			if (values == null) { throw new ArgumentNullException("values"); }
			foreach (var value in values)
			{
				this.Write(value, format: null);
			}
			this.WriteRawLine(String.Empty);
		}
		#endregion

		#region writing single value
		/// <summary>
		/// Writes a preformatted string as the next value, to CSV file. No escaping will be done.
		/// </summary>
		/// <param name="value"></param>
		private void WriteRawValue(String value)
		{
			if (this.LineAlreadyStarted) { Writer.Write(this.Settings.FieldDelimiter); }
			this.Writer.Write(value);
			this.LineAlreadyStarted = true;
		}

		/// <summary>
		/// Writes next value to current csv row, escaping special characters 
		/// </summary>
		/// <param name="value"></param>
		public void Write(String value)
		{
			this.WriteRawValue(WrapValueForCsv(value, this.Settings));
		}

		/// <summary>
		/// Writes next value to current csv row, escaping special characters 
		/// </summary>
		/// <param name="formattable"></param>
		/// <param name="format"></param>
		public void Write(IFormattable formattable, String format)
		{
			if (formattable == null)
			{
				this.WriteRawValue(null);
				return;
			}
			if (format == null) { format = GetDefaultFormatFor(formattable); }
			String formatted = formattable.ToString(format, this.FormattingCulture);
			this.Write(formatted);
		}

		/// <summary>
		/// Override to choose format for a formattable value.
		/// </summary>
		/// <param name="formattable"></param>
		/// <returns></returns>
		protected virtual string GetDefaultFormatFor(IFormattable formattable)
		{
			DateTime? date;
			if ((date = formattable as DateTime?) != null)
			{
				return "s"; //Sortable, displays 2008-04-10T06:30:00
			}
			return null;
		}

		#endregion

		/// <summary>
		/// Escapes given value based on CSV rules, a
		/// </summary>
		/// <param name="value"></param>
		/// <param name="settings"></param>
		/// <returns>value suitable for using as single item in csv row.</returns>
		public static String WrapValueForCsv(String value, CsvSettings settings)
		{
			switch (settings.QuotingMode)
			{
				case CsvQuotingMode.Minimal:
					return WrapValueForCsvUsingMinimalMode(value, settings);
				case CsvQuotingMode.QuoteAll:
					return WrapValueForCsvUsingQuoteAllMode(value, settings);
				default:
					throw new NotSupportedException($@"Quoting mode {settings.QuotingMode} is not yet implemented.");
			}
		}

		private static string WrapValueForCsvUsingMinimalMode(string value, CsvSettings settings)
		{
			if (value == null) { return null; }
			if (value.Length == 0) { return value; }

			Boolean containsQuote = value.Contains(settings.QuotingCharacter);
			if (containsQuote
				|| value.Contains(settings.FieldDelimiter)
				|| value.Contains(settings.RowDelimiter))
			{
				return settings.QuotingCharacter
					+ (containsQuote
						? value.Replace(settings.QuotingCharacter.ToString(), settings.QuotingCharacter.ToString() + settings.QuotingCharacter)
						: value)
					+ settings.QuotingCharacter;
			}
			return value;
		}

		private static string WrapValueForCsvUsingQuoteAllMode(string value, CsvSettings settings)
		{
			String wrapped = settings.QuotingCharacter 
				+ value?.Replace(settings.QuotingCharacter.ToString(), settings.QuotingCharacter.ToString() + settings.QuotingCharacter)
				+ settings.QuotingCharacter;
			return wrapped;
		}
	}
}
