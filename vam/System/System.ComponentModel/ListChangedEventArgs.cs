namespace System.ComponentModel;

public class ListChangedEventArgs : EventArgs
{
	private ListChangedType changedType;

	private int oldIndex;

	private int newIndex;

	private PropertyDescriptor propDesc;

	public ListChangedType ListChangedType => changedType;

	public int OldIndex => oldIndex;

	public int NewIndex => newIndex;

	public PropertyDescriptor PropertyDescriptor => propDesc;

	public ListChangedEventArgs(ListChangedType listChangedType, int newIndex)
		: this(listChangedType, newIndex, -1)
	{
	}

	public ListChangedEventArgs(ListChangedType listChangedType, PropertyDescriptor propDesc)
	{
		changedType = listChangedType;
		this.propDesc = propDesc;
	}

	public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
	{
		changedType = listChangedType;
		this.newIndex = newIndex;
		this.oldIndex = oldIndex;
	}

	public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, PropertyDescriptor propDesc)
	{
		changedType = listChangedType;
		this.newIndex = newIndex;
		oldIndex = newIndex;
		this.propDesc = propDesc;
	}
}
