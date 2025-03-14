namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class LicenseProviderAttribute : Attribute
{
	private Type Provider;

	public static readonly LicenseProviderAttribute Default = new LicenseProviderAttribute();

	public Type LicenseProvider => Provider;

	public override object TypeId => base.ToString() + ((Provider == null) ? null : Provider.ToString());

	public LicenseProviderAttribute()
	{
		Provider = null;
	}

	public LicenseProviderAttribute(string typeName)
	{
		Provider = Type.GetType(typeName, throwOnError: false);
	}

	public LicenseProviderAttribute(Type type)
	{
		Provider = type;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is LicenseProviderAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((LicenseProviderAttribute)obj).LicenseProvider.Equals(Provider);
	}

	public override int GetHashCode()
	{
		return Provider.GetHashCode();
	}
}
