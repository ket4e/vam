namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class MergablePropertyAttribute : Attribute
{
	private bool mergable;

	public static readonly MergablePropertyAttribute Default = new MergablePropertyAttribute(allowMerge: true);

	public static readonly MergablePropertyAttribute No = new MergablePropertyAttribute(allowMerge: false);

	public static readonly MergablePropertyAttribute Yes = new MergablePropertyAttribute(allowMerge: true);

	public bool AllowMerge => mergable;

	public MergablePropertyAttribute(bool allowMerge)
	{
		mergable = allowMerge;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MergablePropertyAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((MergablePropertyAttribute)obj).AllowMerge == mergable;
	}

	public override int GetHashCode()
	{
		return mergable.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return mergable == Default.AllowMerge;
	}
}
