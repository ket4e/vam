namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event)]
public sealed class InheritanceAttribute : Attribute
{
	private InheritanceLevel level;

	public static readonly InheritanceAttribute Default = new InheritanceAttribute();

	public static readonly InheritanceAttribute Inherited = new InheritanceAttribute(InheritanceLevel.Inherited);

	public static readonly InheritanceAttribute InheritedReadOnly = new InheritanceAttribute(InheritanceLevel.InheritedReadOnly);

	public static readonly InheritanceAttribute NotInherited = new InheritanceAttribute(InheritanceLevel.NotInherited);

	public InheritanceLevel InheritanceLevel => level;

	public InheritanceAttribute()
	{
		level = InheritanceLevel.NotInherited;
	}

	public InheritanceAttribute(InheritanceLevel inheritanceLevel)
	{
		level = inheritanceLevel;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is InheritanceAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((InheritanceAttribute)obj).InheritanceLevel == level;
	}

	public override int GetHashCode()
	{
		return level.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return level == Default.InheritanceLevel;
	}

	public override string ToString()
	{
		return level.ToString();
	}
}
