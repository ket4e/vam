using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class ToolStripOverflow : ToolStripDropDown, IDisposable, IComponent
{
	private class ToolStripOverflowAccessibleObject : AccessibleObject
	{
	}

	private LayoutEngine layout_engine;

	public override ToolStripItemCollection Items => base.Items;

	public override LayoutEngine LayoutEngine
	{
		get
		{
			if (layout_engine == null)
			{
				layout_engine = new FlowLayout();
			}
			return base.LayoutEngine;
		}
	}

	protected internal override ToolStripItemCollection DisplayedItems => base.DisplayedItems;

	internal ToolStrip ParentToolStrip => base.OwnerItem.Parent;

	public ToolStripOverflow(ToolStripItem parentItem)
	{
		base.OwnerItem = parentItem;
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		return base.GetToolStripPreferredSize(constrainingSize);
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripOverflowAccessibleObject();
	}

	[System.MonoInternalNote("This should stack in rows of ~3, but for now 1 column will work.")]
	protected override void OnLayout(LayoutEventArgs e)
	{
		SetDisplayedItems();
		int num = 0;
		foreach (ToolStripItem displayedItem in DisplayedItems)
		{
			if (displayedItem.Available && displayedItem.GetPreferredSize(Size.Empty).Width > num)
			{
				num = displayedItem.GetPreferredSize(Size.Empty).Width;
			}
		}
		int left = base.Padding.Left;
		num += base.Padding.Horizontal;
		int num2 = base.Padding.Top;
		foreach (ToolStripItem displayedItem2 in DisplayedItems)
		{
			if (displayedItem2.Available)
			{
				num2 += displayedItem2.Margin.Top;
				int num3 = 0;
				num3 = ((!(displayedItem2 is ToolStripSeparator)) ? displayedItem2.GetPreferredSize(Size.Empty).Height : 7);
				displayedItem2.SetBounds(new Rectangle(left, num2, num, num3));
				num2 += displayedItem2.Height + displayedItem2.Margin.Bottom;
			}
		}
		base.Size = new Size(num + base.Padding.Horizontal, num2 + base.Padding.Bottom);
	}

	protected override void SetDisplayedItems()
	{
		displayed_items.Clear();
		if (base.OwnerItem != null && base.OwnerItem.Parent != null)
		{
			foreach (ToolStripItem item in base.OwnerItem.Parent.Items)
			{
				if (item.Placement == ToolStripItemPlacement.Overflow && item.Available && !(item is ToolStripSeparator))
				{
					displayed_items.AddNoOwnerOrLayout(item);
				}
			}
		}
		PerformLayout();
	}
}
