namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class BindableAttribute : Attribute
{
	private bool bindable;

	private BindingDirection direction;

	public static readonly BindableAttribute No = new BindableAttribute(BindableSupport.No);

	public static readonly BindableAttribute Yes = new BindableAttribute(BindableSupport.Yes);

	public static readonly BindableAttribute Default = new BindableAttribute(BindableSupport.Default);

	public BindingDirection Direction => direction;

	public bool Bindable => bindable;

	public BindableAttribute(BindableSupport flags)
	{
		if (flags == BindableSupport.No)
		{
			bindable = false;
		}
		if (flags == BindableSupport.Yes || flags == BindableSupport.Default)
		{
			bindable = true;
		}
	}

	public BindableAttribute(bool bindable)
	{
		this.bindable = bindable;
	}

	public BindableAttribute(bool bindable, BindingDirection direction)
	{
		this.bindable = bindable;
		this.direction = direction;
	}

	public BindableAttribute(BindableSupport flags, BindingDirection direction)
		: this(flags)
	{
		this.direction = direction;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is BindableAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((BindableAttribute)obj).Bindable == bindable;
	}

	public override int GetHashCode()
	{
		return bindable.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return bindable == Default.Bindable;
	}
}
