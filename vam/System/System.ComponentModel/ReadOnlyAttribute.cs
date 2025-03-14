namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class ReadOnlyAttribute : Attribute
{
	private bool read_only;

	public static readonly ReadOnlyAttribute No;

	public static readonly ReadOnlyAttribute Yes;

	public static readonly ReadOnlyAttribute Default;

	public bool IsReadOnly => read_only;

	public ReadOnlyAttribute(bool read_only)
	{
		this.read_only = read_only;
	}

	static ReadOnlyAttribute()
	{
		No = new ReadOnlyAttribute(read_only: false);
		Yes = new ReadOnlyAttribute(read_only: true);
		Default = new ReadOnlyAttribute(read_only: false);
	}

	public override int GetHashCode()
	{
		return read_only.GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (!(o is ReadOnlyAttribute))
		{
			return false;
		}
		return ((ReadOnlyAttribute)o).IsReadOnly.Equals(read_only);
	}

	public override bool IsDefaultAttribute()
	{
		return Equals(Default);
	}
}
