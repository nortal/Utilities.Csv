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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv, file 'CsvSettings.cs'.
*/
using System;

namespace Nortal.Utilities.Csv
{
	/// <summary>
	/// Defines Csv syntax (field delimiter, etc). Defaults to values from RFC4180
	/// </summary>
	public class CsvSettings
	{
		public CsvSettings()
		{
			// Load ISO4180 settings
			this.FieldDelimiter = ',';
			this.QuotingCharacter = '"';
			this.RowDelimiter = "\r\n";
			this.QuotingMode = CsvQuotingMode.Minimal;
		}

		/// <summary>
		/// Symbol to separate values within a csv row. Default is comma (',').
		/// </summary>
		public char FieldDelimiter { get; set; }

		/// <summary>
		/// Line separator, default is newline. Any string up to length of 2 could be used.
		/// </summary>
		public String RowDelimiter { get; set; }

		/// <summary>
		/// Symbol to optionally wrap values with. Defaults to '"'.
		/// </summary>
		public char QuotingCharacter { get; set; }

		/// <summary>
		/// Determines how individual values are wrapped with quoting characters.
		/// Applies only to writing CSV using CsvWriter class. Parser handles all wrapping styles valid by RFC4180 rules.
		/// The default value is CsvQuotingMode.Minimal.
		/// </summary>
		public CsvQuotingMode QuotingMode { get; set; }
	}
}
