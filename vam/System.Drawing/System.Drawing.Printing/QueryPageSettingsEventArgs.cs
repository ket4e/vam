namespace System.Drawing.Printing;

public class QueryPageSettingsEventArgs : PrintEventArgs
{
	private PageSettings pageSettings;

	public PageSettings PageSettings
	{
		get
		{
			return pageSettings;
		}
		set
		{
			pageSettings = value;
		}
	}

	public QueryPageSettingsEventArgs(PageSettings pageSettings)
	{
		this.pageSettings = pageSettings;
	}
}
