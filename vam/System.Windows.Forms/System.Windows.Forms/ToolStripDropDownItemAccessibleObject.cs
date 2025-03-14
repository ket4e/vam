namespace System.Windows.Forms;

public class ToolStripDropDownItemAccessibleObject : ToolStripItem.ToolStripItemAccessibleObject
{
	public override AccessibleRole Role => base.Role;

	public ToolStripDropDownItemAccessibleObject(ToolStripDropDownItem item)
		: base(item)
	{
	}

	public override void DoDefaultAction()
	{
		base.DoDefaultAction();
	}

	public override AccessibleObject GetChild(int index)
	{
		return (owner_item as ToolStripDropDownItem).DropDownItems[index].AccessibilityObject;
	}

	public override int GetChildCount()
	{
		return (owner_item as ToolStripDropDownItem).DropDownItems.Count;
	}
}
