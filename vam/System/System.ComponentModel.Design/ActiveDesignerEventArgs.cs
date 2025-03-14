namespace System.ComponentModel.Design;

public class ActiveDesignerEventArgs : EventArgs
{
	private IDesignerHost oldDesigner;

	private IDesignerHost newDesigner;

	public IDesignerHost NewDesigner => newDesigner;

	public IDesignerHost OldDesigner => oldDesigner;

	public ActiveDesignerEventArgs(IDesignerHost oldDesigner, IDesignerHost newDesigner)
	{
		this.oldDesigner = oldDesigner;
		this.newDesigner = newDesigner;
	}
}
