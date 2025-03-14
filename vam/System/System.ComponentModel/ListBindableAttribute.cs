namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public sealed class ListBindableAttribute : Attribute
{
	public static readonly ListBindableAttribute Default = new ListBindableAttribute(listBindable: true);

	public static readonly ListBindableAttribute No = new ListBindableAttribute(listBindable: false);

	public static readonly ListBindableAttribute Yes = new ListBindableAttribute(listBindable: true);

	private bool bindable;

	public bool ListBindable => bindable;

	public ListBindableAttribute(bool listBindable)
	{
		bindable = listBindable;
	}

	public ListBindableAttribute(BindableSupport flags)
	{
		if (flags == BindableSupport.No)
		{
			bindable = false;
		}
		else
		{
			bindable = true;
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ListBindableAttribute))
		{
			return false;
		}
		return ((ListBindableAttribute)obj).ListBindable.Equals(bindable);
	}

	public override int GetHashCode()
	{
		return bindable.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return Equals(Default);
	}
}
