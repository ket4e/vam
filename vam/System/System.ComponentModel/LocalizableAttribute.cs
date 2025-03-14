namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class LocalizableAttribute : Attribute
{
	private bool localizable;

	public static readonly LocalizableAttribute Default = new LocalizableAttribute(localizable: false);

	public static readonly LocalizableAttribute No = new LocalizableAttribute(localizable: false);

	public static readonly LocalizableAttribute Yes = new LocalizableAttribute(localizable: true);

	public bool IsLocalizable => localizable;

	public LocalizableAttribute(bool localizable)
	{
		this.localizable = localizable;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is LocalizableAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((LocalizableAttribute)obj).IsLocalizable == localizable;
	}

	public override int GetHashCode()
	{
		return localizable.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return localizable == Default.IsLocalizable;
	}
}
