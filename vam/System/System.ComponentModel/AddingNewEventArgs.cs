namespace System.ComponentModel;

public class AddingNewEventArgs : EventArgs
{
	private object obj;

	public object NewObject
	{
		get
		{
			return obj;
		}
		set
		{
			obj = value;
		}
	}

	public AddingNewEventArgs()
		: this(null)
	{
	}

	public AddingNewEventArgs(object newObject)
	{
		obj = newObject;
	}
}
