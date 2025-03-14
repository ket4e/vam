namespace System.ComponentModel;

public class PropertyChangingEventArgs : EventArgs
{
	private string name;

	public virtual string PropertyName => name;

	public PropertyChangingEventArgs(string propertyName)
	{
		name = propertyName;
	}
}
