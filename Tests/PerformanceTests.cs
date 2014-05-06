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

	This file is from project https://github.com/NortalLTD/Utilities.Csv, Nortal.Utilities.Csv.Tests, file 'PerformanceTests.cs'.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Nortal.Utilities.Csv.Tests
{
	[TestClass]
	public class PerformanceTests
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "FileStream can handle disposing multiple times")]
		[TestMethod]
		public void TestLexerCanReadFileStream()
		{
			var executionPath = new FileInfo(Assembly.GetAssembly(this.GetType()).Location);
			String sampleFilePath = Path.Combine(executionPath.Directory.FullName, "Sample200k.csv");

			var stopper = Stopwatch.StartNew();

			using (var fileStream = File.OpenRead(sampleFilePath))
			using (var reader = new StreamReader(fileStream))
			{
				var parser = new CsvParser(reader);
				String[] line = null;
				int lines = 0;
				while ((line = parser.ReadNextRow()) != null)
				{
					lines++;
				}
				stopper.Stop();

				Assert.IsTrue(lines > 30 * 1000, "File unexpectantly small, read lines: " + lines);
				//obviously depends on computer the test is ran on. Still, this check should trigger if something went very wrong:
				Assert.IsTrue(stopper.Elapsed < TimeSpan.FromSeconds(2), "Exceeds expected time, time taken " + stopper.Elapsed);
			}
		}
	}
}
