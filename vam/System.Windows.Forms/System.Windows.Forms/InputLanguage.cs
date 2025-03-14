using System.Globalization;

namespace System.Windows.Forms;

public sealed class InputLanguage
{
	private static InputLanguageCollection all;

	private IntPtr handle;

	private CultureInfo culture;

	private string layout_name;

	private static InputLanguage current_input;

	private static InputLanguage default_input;

	public static InputLanguage CurrentInputLanguage
	{
		get
		{
			if (current_input == null)
			{
				current_input = FromCulture(CultureInfo.CurrentUICulture);
			}
			return current_input;
		}
		set
		{
			if (InstalledInputLanguages.Contains(value))
			{
				current_input = value;
			}
		}
	}

	public static InputLanguage DefaultInputLanguage
	{
		get
		{
			if (default_input == null)
			{
				default_input = FromCulture(CultureInfo.CurrentUICulture);
			}
			return default_input;
		}
	}

	public static InputLanguageCollection InstalledInputLanguages
	{
		get
		{
			if (all == null)
			{
				all = new InputLanguageCollection(new InputLanguage[1]
				{
					new InputLanguage(IntPtr.Zero, new CultureInfo(string.Empty), "US")
				});
			}
			return all;
		}
	}

	public CultureInfo Culture => culture;

	public IntPtr Handle => handle;

	public string LayoutName => layout_name;

	[System.MonoInternalNote("Pull Microsofts InputLanguages and enter them here")]
	internal InputLanguage()
	{
	}

	internal InputLanguage(IntPtr handle, CultureInfo culture, string layout_name)
		: this()
	{
		this.handle = handle;
		this.culture = culture;
		this.layout_name = layout_name;
	}

	public static InputLanguage FromCulture(CultureInfo culture)
	{
		foreach (InputLanguage installedInputLanguage in InstalledInputLanguages)
		{
			if (culture.EnglishName == installedInputLanguage.culture.EnglishName)
			{
				return new InputLanguage(installedInputLanguage.handle, installedInputLanguage.culture, installedInputLanguage.layout_name);
			}
		}
		return new InputLanguage(InstalledInputLanguages[0].handle, InstalledInputLanguages[0].culture, InstalledInputLanguages[0].layout_name);
	}

	public override bool Equals(object value)
	{
		if (value is InputLanguage && ((InputLanguage)value).culture == culture && ((InputLanguage)value).handle == handle && ((InputLanguage)value).layout_name == layout_name)
		{
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
