namespace Nortal.Utilities.Csv
{
	/// <summary>
	/// Describes options for wrapping individual values with quoting character.
	/// </summary>
	public enum CsvQuotingMode : byte
	{
		/// <summary>
		/// Omits quoting characters if they are not required by RFC4180.
		/// </summary>
		Minimal = 0,
		/// <summary>
		/// Wraps all values with quotes.
		/// </summary>
		QuoteAll = 1,
		
		//To consider extensions in future, i.e. to force quote only all non-nulls or non-empty values, etc. Or maybe not.
	}
}
