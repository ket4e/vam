namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class DesignTimeVisibleAttribute : Attribute
{
	private bool visible;

	public static readonly DesignTimeVisibleAttribute Default = new DesignTimeVisibleAttribute(visible: true);

	public static readonly DesignTimeVisibleAttribute No = new DesignTimeVisibleAttribute(visible: false);

	public static readonly DesignTimeVisibleAttribute Yes = new DesignTimeVisibleAttribute(visible: true);

	public bool Visible => visible;

	public DesignTimeVisibleAttribute()
		: this(visible: true)
	{
	}

	public DesignTimeVisibleAttribute(bool visible)
	{
		this.visible = visible;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DesignTimeVisibleAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((DesignTimeVisibleAttribute)obj).Visible == visible;
	}

	public override int GetHashCode()
	{
		return visible.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return visible == Default.Visible;
	}
}
