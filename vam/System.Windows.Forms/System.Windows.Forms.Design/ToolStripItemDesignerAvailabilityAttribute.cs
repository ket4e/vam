namespace System.Windows.Forms.Design;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ToolStripItemDesignerAvailabilityAttribute : Attribute
{
	private ToolStripItemDesignerAvailability visibility;

	public static readonly ToolStripItemDesignerAvailabilityAttribute Default = new ToolStripItemDesignerAvailabilityAttribute();

	public ToolStripItemDesignerAvailability ItemAdditionVisibility => visibility;

	public ToolStripItemDesignerAvailabilityAttribute()
	{
		visibility = ToolStripItemDesignerAvailability.None;
	}

	public ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability visibility)
	{
		this.visibility = visibility;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ToolStripItemDesignerAvailabilityAttribute))
		{
			return false;
		}
		return ItemAdditionVisibility == (obj as ToolStripItemDesignerAvailabilityAttribute).ItemAdditionVisibility;
	}

	public override int GetHashCode()
	{
		return (int)visibility;
	}

	public override bool IsDefaultAttribute()
	{
		return visibility == ToolStripItemDesignerAvailability.None;
	}
}
