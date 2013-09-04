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
			// Load ISO4801 settings
			this.FieldDelimiter = ',';
			this.QuotingCharacter = '"';
			this.RowDelimiter = Environment.NewLine;
		}

		public char FieldDelimiter { get; private set; }
		public String RowDelimiter { get; private set; }
		public char QuotingCharacter { get; private set; }
	}
}
