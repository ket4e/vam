namespace System.Windows.Forms;

public class TabControlEventArgs : EventArgs
{
	private TabControlAction action;

	private TabPage tab_page;

	private int tab_page_index;

	public TabControlAction Action => action;

	public TabPage TabPage => tab_page;

	public int TabPageIndex => tab_page_index;

	public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
	{
		tab_page = tabPage;
		tab_page_index = tabPageIndex;
		this.action = action;
	}
}
