namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public class InstallerTypeAttribute : Attribute
{
	private Type installer;

	public virtual Type InstallerType => installer;

	public InstallerTypeAttribute(string typeName)
	{
		installer = Type.GetType(typeName, throwOnError: false);
	}

	public InstallerTypeAttribute(Type installerType)
	{
		installer = installerType;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is InstallerTypeAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((InstallerTypeAttribute)obj).InstallerType == installer;
	}

	public override int GetHashCode()
	{
		return installer.GetHashCode();
	}
}
