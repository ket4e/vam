namespace System.ComponentModel.Design.Serialization;

public class ResolveNameEventArgs : EventArgs
{
	private string name;

	private object value;

	public string Name => name;

	public object Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public ResolveNameEventArgs(string name)
	{
		this.name = name;
	}
}
