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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv, file 'CsvLexeme.cs'.
*/
using System;

namespace Nortal.Utilities.Csv
{
	internal struct CsvLexeme
	{
		internal CsvLexeme(CsvSyntaxItem type, String value): this()
		{
			Type = type;
			Value = value;
		}

		internal String Value { get; set; }
		internal CsvSyntaxItem Type { get; set; }

		public override string ToString()
		{
			if (this.Type == CsvSyntaxItem.Text)
			{
				return String.Format("{0}: '{1}'", this.Type, this.Value);
			}
			return this.Type.ToString();
		}
	}
}
