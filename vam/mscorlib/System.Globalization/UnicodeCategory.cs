using System.Runtime.InteropServices;

namespace System.Globalization;

[Serializable]
[ComVisible(true)]
public enum UnicodeCategory
{
	UppercaseLetter,
	LowercaseLetter,
	TitlecaseLetter,
	ModifierLetter,
	OtherLetter,
	NonSpacingMark,
	SpacingCombiningMark,
	EnclosingMark,
	DecimalDigitNumber,
	LetterNumber,
	OtherNumber,
	SpaceSeparator,
	LineSeparator,
	ParagraphSeparator,
	Control,
	Format,
	Surrogate,
	PrivateUse,
	ConnectorPunctuation,
	DashPunctuation,
	OpenPunctuation,
	ClosePunctuation,
	InitialQuotePunctuation,
	FinalQuotePunctuation,
	OtherPunctuation,
	MathSymbol,
	CurrencySymbol,
	ModifierSymbol,
	OtherSymbol,
	OtherNotAssigned
}
