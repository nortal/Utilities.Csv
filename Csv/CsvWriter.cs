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
		/// <summary>
		/// Initializes the writer object to write CSV using standard RFC4180 settings.
		/// </summary>
		/// <param name="writer">Text collector for CSV to be written to.</param>
		public CsvWriter(TextWriter writer)
			: this(writer, (CsvSettings) null)
		{
		}

		/// <summary>
		/// Initializes the writer object with custom settings.
		/// </summary>
		/// <param name="writer">Text collector for CSV to be written to.</param>
		/// <param name="settings">Parameters to control how the CSV will be written. if omitted then defaults to CSV as stated by RFC4180.</param>
		public CsvWriter(TextWriter writer, CsvSettings settings)
		{
			if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
			this.Writer = writer;
			this.Settings = settings ?? new CsvSettings();
			this.LineAlreadyStarted = false;
			this.FormattingCulture = Thread.CurrentThread.CurrentCulture;
		}

		/// <summary>
		/// Defines csv parameters to use when creating fields, row, wrappers.
		/// </summary>
		private CsvSettings Settings { get; set; }

		/// <summary>
		/// Stream where output CSV is written to.
		/// </summary>
		private TextWriter Writer { get; set; }
		
		/// <summary>
		/// Internal tracker used for determining if a field separator is required for next value to be written to writer.
		/// </summary>
		private Boolean LineAlreadyStarted { get; set; }

		/// <summary>
		/// Culture to use when automatically formatting IFormattables to strings.
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
		/// <param name="format">.net formatstring to apply to the formattable object during serializing.</param>
		public void Write(IFormattable formattable, String format = null)
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
		/// Override to choose a default format for a formattable value.
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
		/// Escapes given value based on CSV rules.
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

		/// <summary>
		/// Applies wrapping to given value for using in CSV file. This wrapping method is adding the wrapping characters only if the given value contains any special symbols by given csv settings.
		/// </summary>
		/// <param name="value">original unwrapped value.</param>
		/// <param name="settings"></param>
		/// <returns>CSV value ready to be used in CSV, wrapped if it was necessary.</returns>
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

		/// <summary>
		/// Applies wrapping to given value for using in CSV file. This wrapping method applies wrappers to all values, including empty ones and nulls.
		/// </summary>
		/// <param name="value">The original unwrapped value.</param>
		/// <param name="settings"></param>
		/// <returns>Wrapped value</returns>
		private static string WrapValueForCsvUsingQuoteAllMode(string value, CsvSettings settings)
		{
			String wrapped = settings.QuotingCharacter 
				+ value?.Replace(settings.QuotingCharacter.ToString(), settings.QuotingCharacter.ToString() + settings.QuotingCharacter)
				+ settings.QuotingCharacter;
			return wrapped;
		}
	}
}
