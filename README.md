Nortal.Utilities.Csv
====================================

This lightweight library helps to work with data formatted as Comma Separated Values (CSV).
The focus is on standard-compliance (RFC 4180) and performance (by providing tools to stream csv for both reading and writing).

Features include:
* supports CSV special symbols within data values (linebreaks, commas and quotes)
* can work with csv streams for both reading and writing
* can customize field separator symbol
* can customize quoting character
* can customize row delimiter (i.e. windows vs linux)
* supports cultures for autoformatting for dates, numbers, etc.

Implementation
-----------------
This implementation is based on RFC 4180 (http://tools.ietf.org/rfc/rfc4180.txt).

.Net Standerd 2.0 library, can be used in strongly named projects.

Licenced under Apache Licence v2.0.

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

To use non-default setting, for example TAB as field separator:

	var settings = new CsvSettings() { FieldDelimiter = '\t' };
	var csv = new CsvWriter(writer, settings); // .. or similarly CsvParser for reading
