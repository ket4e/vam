namespace System.Windows.Forms;

public class ConvertEventArgs : EventArgs
{
	private object object_value;

	private Type desired_type;

	public Type DesiredType => desired_type;

	public object Value
	{
		get
		{
			return object_value;
		}
		set
		{
			object_value = value;
		}
	}

	public ConvertEventArgs(object value, Type desiredType)
	{
		object_value = value;
		desired_type = desiredType;
	}
}
