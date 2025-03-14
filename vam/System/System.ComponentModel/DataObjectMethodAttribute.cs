namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Method)]
public sealed class DataObjectMethodAttribute : Attribute
{
	private readonly DataObjectMethodType _methodType;

	private readonly bool _isDefault;

	public DataObjectMethodType MethodType => _methodType;

	public bool IsDefault => _isDefault;

	public DataObjectMethodAttribute(DataObjectMethodType methodType)
		: this(methodType, isDefault: false)
	{
	}

	public DataObjectMethodAttribute(DataObjectMethodType methodType, bool isDefault)
	{
		_methodType = methodType;
		_isDefault = isDefault;
	}

	public override bool Match(object obj)
	{
		if (!(obj is DataObjectMethodAttribute))
		{
			return false;
		}
		return ((DataObjectMethodAttribute)obj).MethodType == MethodType;
	}

	public override bool Equals(object obj)
	{
		return Match(obj) && ((DataObjectMethodAttribute)obj).IsDefault == IsDefault;
	}

	public override int GetHashCode()
	{
		return MethodType.GetHashCode() ^ IsDefault.GetHashCode();
	}
}
