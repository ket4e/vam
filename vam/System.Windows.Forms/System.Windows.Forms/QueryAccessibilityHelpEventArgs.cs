using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class QueryAccessibilityHelpEventArgs : EventArgs
{
	private string help_namespace;

	private string help_string;

	private string help_keyword;

	public string HelpKeyword
	{
		get
		{
			return help_keyword;
		}
		set
		{
			help_keyword = value;
		}
	}

	public string HelpNamespace
	{
		get
		{
			return help_namespace;
		}
		set
		{
			help_namespace = value;
		}
	}

	public string HelpString
	{
		get
		{
			return help_string;
		}
		set
		{
			help_string = value;
		}
	}

	public QueryAccessibilityHelpEventArgs()
	{
		help_namespace = null;
		help_string = null;
		help_keyword = null;
	}

	public QueryAccessibilityHelpEventArgs(string helpNamespace, string helpString, string helpKeyword)
	{
		help_namespace = helpNamespace;
		help_string = helpString;
		help_keyword = helpKeyword;
	}
}
