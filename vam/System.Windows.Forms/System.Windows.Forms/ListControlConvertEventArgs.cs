namespace System.Windows.Forms;

public class ListControlConvertEventArgs : ConvertEventArgs
{
	private object list_item;

	public object ListItem => list_item;

	public ListControlConvertEventArgs(object value, Type desiredType, object listItem)
		: base(value, desiredType)
	{
		list_item = listItem;
	}
}
