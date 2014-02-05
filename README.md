Nortal.Utilities.Csv
====================================

Yet another library to help work with data formatted as Comma Separated Values (CSV), providing API for both reading and writing.
This implementation is based on RFC 4180 (http://tools.ietf.org/rfc/rfc4180.txt) to read all valid csv files and write VALID csv files which can be read by other RFC-wise correct tools.

Features include:
* supports CSV special symbols within data values (linebreaks, commas and quotes)
* can work with csv streams for both reading and writing
* can customize field separator symbol
* can customize quoting character
* can customize row delimiter (i.e. windows vs linux)
* supports cultures for autoformatting for dates, numbers, etc.

Implementation
-----------------
Licenced under Apache Licence v2.0.
Requires .Net Framework 3.5 Client profile.

Getting started
---------------

To install Nortal.Utilities.Csv download built package from https://www.nuget.org/packages/Nortal.Utilities.Csv or run the following command in the Package Manager Console: 

	PM> Install-Package Nortal.Utilities.Csv

To parse whole csv file: 

```csharp
String csvString = File.ReadAllText(sampleFilePath);
String[][] data = CsvParser.Parse(csvString);
```

For larger files you can stream one row at a time:

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
		csv.Write("MyValue");                    // writing one value at a time
		csv.Write(2, "N2");                      // or with explicit format
		csv.WriteLine(DateTime.Now);             // or with automatic formatting
		csv.WriteLine(1,2,3,4, DateTime.Now);    // another line with many values at once
		File.WriteAllText(@"C:\my.csv", writer.ToString());
	}

