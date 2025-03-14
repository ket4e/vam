using System.ComponentModel;

namespace System.Windows.Forms;

public class TabControlCancelEventArgs : CancelEventArgs
{
	private TabControlAction action;

	private TabPage tab_page;

	private int tab_page_index;

	public TabControlAction Action => action;

	public TabPage TabPage => tab_page;

	public int TabPageIndex => tab_page_index;

	public TabControlCancelEventArgs(TabPage tabPage, int tabPageIndex, bool cancel, TabControlAction action)
		: base(cancel)
	{
		tab_page = tabPage;
		tab_page_index = tabPageIndex;
		this.action = action;
	}
}
