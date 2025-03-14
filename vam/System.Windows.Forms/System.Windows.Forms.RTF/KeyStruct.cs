namespace System.Windows.Forms.RTF;

internal struct KeyStruct
{
	public Major Major;

	public Minor Minor;

	public string Symbol;

	public KeyStruct(Major major, Minor minor, string symbol)
	{
		Major = major;
		Minor = minor;
		Symbol = symbol;
	}
}
