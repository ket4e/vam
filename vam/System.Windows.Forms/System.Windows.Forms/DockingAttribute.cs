namespace System.Windows.Forms;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DockingAttribute : Attribute
{
	private DockingBehavior dockingBehavior;

	public static readonly DockingAttribute Default = new DockingAttribute();

	public DockingBehavior DockingBehavior => dockingBehavior;

	public DockingAttribute()
	{
		dockingBehavior = DockingBehavior.Never;
	}

	public DockingAttribute(DockingBehavior dockingBehavior)
	{
		this.dockingBehavior = dockingBehavior;
	}

	public override bool Equals(object obj)
	{
		if (obj is DockingAttribute)
		{
			return dockingBehavior == ((DockingAttribute)obj).DockingBehavior;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return dockingBehavior.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return Default.Equals(this);
	}
}
