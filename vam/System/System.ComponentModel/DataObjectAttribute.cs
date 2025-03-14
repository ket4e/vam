namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DataObjectAttribute : Attribute
{
	public static readonly DataObjectAttribute DataObject;

	public static readonly DataObjectAttribute Default;

	public static readonly DataObjectAttribute NonDataObject;

	private readonly bool _isDataObject;

	public bool IsDataObject => _isDataObject;

	public DataObjectAttribute()
		: this(isDataObject: true)
	{
	}

	public DataObjectAttribute(bool isDataObject)
	{
		_isDataObject = isDataObject;
	}

	static DataObjectAttribute()
	{
		DataObject = new DataObjectAttribute(isDataObject: true);
		NonDataObject = new DataObjectAttribute(isDataObject: false);
		Default = NonDataObject;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DataObjectAttribute))
		{
			return false;
		}
		return ((DataObjectAttribute)obj).IsDataObject == IsDataObject;
	}

	public override int GetHashCode()
	{
		return IsDataObject.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return Default.Equals(this);
	}
}
