using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class InputLanguageChangingEventArgs : CancelEventArgs
{
	private CultureInfo culture;

	private bool system_charset;

	private InputLanguage input_language;

	public bool SysCharSet => system_charset;

	public CultureInfo Culture => culture;

	public InputLanguage InputLanguage => input_language;

	public InputLanguageChangingEventArgs(CultureInfo culture, bool sysCharSet)
	{
		this.culture = culture;
		system_charset = sysCharSet;
		input_language = InputLanguage.FromCulture(culture);
	}

	public InputLanguageChangingEventArgs(InputLanguage inputLanguage, bool sysCharSet)
	{
		culture = inputLanguage.Culture;
		system_charset = sysCharSet;
		input_language = inputLanguage;
	}
}
