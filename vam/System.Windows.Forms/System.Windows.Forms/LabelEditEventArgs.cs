namespace System.Windows.Forms;

public class LabelEditEventArgs : EventArgs
{
	private int item;

	private string label;

	private bool cancelEdit;

	public bool CancelEdit
	{
		get
		{
			return cancelEdit;
		}
		set
		{
			cancelEdit = value;
		}
	}

	public int Item => item;

	public string Label => label;

	public LabelEditEventArgs(int item)
	{
		this.item = item;
	}

	public LabelEditEventArgs(int item, string label)
	{
		this.item = item;
		this.label = label;
	}

	internal void SetLabel(string label)
	{
		this.label = label;
	}
}
