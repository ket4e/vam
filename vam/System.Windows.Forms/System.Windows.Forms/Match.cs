using System.Collections;

namespace System.Windows.Forms;

internal class Match
{
	private string mimeType;

	private int priority;

	private ArrayList matchlets = new ArrayList();

	public string MimeType
	{
		get
		{
			return mimeType;
		}
		set
		{
			mimeType = value;
		}
	}

	public int Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
		}
	}

	public ArrayList Matchlets => matchlets;
}
