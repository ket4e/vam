namespace System.ComponentModel;

public class PropertyChangedEventArgs : EventArgs
{
	private string propertyName;

	public virtual string PropertyName => propertyName;

	public PropertyChangedEventArgs(string name)
	{
		propertyName = name;
	}
}
