using System.Globalization;

namespace System.Windows.Forms;

public class InputLanguageChangedEventArgs : EventArgs
{
	private CultureInfo culture;

	private byte charset;

	private InputLanguage input_language;

	public byte CharSet => charset;

	public CultureInfo Culture => culture;

	public InputLanguage InputLanguage => input_language;

	public InputLanguageChangedEventArgs(CultureInfo culture, byte charSet)
	{
		this.culture = culture;
		charset = charSet;
		input_language = InputLanguage.FromCulture(culture);
	}

	public InputLanguageChangedEventArgs(InputLanguage inputLanguage, byte charSet)
	{
		culture = inputLanguage.Culture;
		charset = charSet;
		input_language = inputLanguage;
	}
}
