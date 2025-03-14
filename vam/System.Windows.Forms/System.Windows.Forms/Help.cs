using System.Drawing;

namespace System.Windows.Forms;

public class Help
{
	private Help()
	{
	}

	public static void ShowHelp(Control parent, string url)
	{
		ShowHelp(parent, url, HelpNavigator.TableOfContents, null);
	}

	public static void ShowHelp(Control parent, string url, HelpNavigator navigator)
	{
		ShowHelp(parent, url, navigator, null);
	}

	[System.MonoTODO("Stub, does nothing")]
	public static void ShowHelp(Control parent, string url, HelpNavigator command, object parameter)
	{
	}

	public static void ShowHelp(Control parent, string url, string keyword)
	{
		if (keyword == null || keyword == string.Empty)
		{
			ShowHelp(parent, url, HelpNavigator.TableOfContents, null);
		}
		ShowHelp(parent, url, HelpNavigator.Topic, keyword);
	}

	public static void ShowHelpIndex(Control parent, string url)
	{
		ShowHelp(parent, url, HelpNavigator.Index, null);
	}

	[System.MonoTODO("Stub, does nothing")]
	public static void ShowPopup(Control parent, string caption, Point location)
	{
	}
}
