namespace System.Windows.Forms;

internal class ToolWindowManager : InternalWindowManager
{
	public ToolWindowManager(Form form)
		: base(form)
	{
	}

	public override void SetWindowState(FormWindowState old_state, FormWindowState window_state)
	{
	}
}
