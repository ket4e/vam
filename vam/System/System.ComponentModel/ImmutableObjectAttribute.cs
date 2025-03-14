namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class ImmutableObjectAttribute : Attribute
{
	private bool immutable;

	public static readonly ImmutableObjectAttribute Default = new ImmutableObjectAttribute(immutable: false);

	public static readonly ImmutableObjectAttribute No = new ImmutableObjectAttribute(immutable: false);

	public static readonly ImmutableObjectAttribute Yes = new ImmutableObjectAttribute(immutable: true);

	public bool Immutable => immutable;

	public ImmutableObjectAttribute(bool immutable)
	{
		this.immutable = immutable;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ImmutableObjectAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((ImmutableObjectAttribute)obj).Immutable == immutable;
	}

	public override int GetHashCode()
	{
		return immutable.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return immutable == Default.Immutable;
	}
}
