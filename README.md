Nortal.Utilities.Csv
====================================

Yet another library to help work with data formatted as Comma Separated Values (CSV), providing API for both reading and writing.
This implementation is based on RFC 4180 (http://tools.ietf.org/rfc/rfc4180.txt) to read all valid csv files and write VALID csv files which can be read by other correct tools.

Features include:
* supports CSV special symbols within data values (linefeeds, commas and quotes)
* can customize field separator symbol
* can customize quoting character
* can customize row delimiter (i.e. windows vs linux)
* can work with csv streams for both reading and writing
* can be extended to provide custom formatting for dates, numbers, etc.

Implementation
-----------------
Licenced under Apache Licence v2.0.
Requires .Net Framework 3.5+

Getting started
---------------

First, reference Nortal.Utilities.Csv.dll to your project and include namespace "Nortal.Utilities.Csv".

To parse whole csv file: 

```csharp
String csvString = File.ReadAllText(sampleFilePath);
String[][] data = CsvParser.Parse(csvString);
```

To stream csv file one row at a time:

	using (var parser = new CsvParser(stream))
	{
		String[] line1 = parser.ReadNextRow();
		String[] line2 = parser.ReadNextRow();
		...
	}

To write values in CSV format:

	using (var writer = new StringWriter())
	{
		var csv = new CsvWriter(writer, new CsvSettings());
		csv.Write("1");                            // writing one value at a time
		csv.Write(DateTime.Now, "s");              // or with explicit format
		csv.WriteLine("3");                        // finish ongoing line
		csv.WriteLine(100.333, DateTime.Now, "6"); // another line with implicit formatting:
	}
