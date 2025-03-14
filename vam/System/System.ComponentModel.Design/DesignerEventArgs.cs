namespace System.ComponentModel.Design;

public class DesignerEventArgs : EventArgs
{
	private IDesignerHost host;

	public IDesignerHost Designer => host;

	public DesignerEventArgs(IDesignerHost host)
	{
		this.host = host;
	}
}
