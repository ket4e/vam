using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
public class ToolStripOverflowButton : ToolStripDropDownButton
{
	private class ToolStripOverflowButtonAccessibleObject : AccessibleObject
	{
	}

	public override bool HasDropDownItems
	{
		get
		{
			if (drop_down == null)
			{
				return false;
			}
			return base.DropDown.DisplayedItems.Count > 0;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new bool RightToLeftAutoMirrorImage
	{
		get
		{
			return base.RightToLeftAutoMirrorImage;
		}
		set
		{
			base.RightToLeftAutoMirrorImage = value;
		}
	}

	protected internal override Padding DefaultMargin => new Padding(0, 1, 0, 2);

	internal ToolStripOverflowButton(ToolStrip ts)
	{
		base.InternalOwner = ts;
		base.Parent = ts;
		base.Visible = false;
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		return new Size(16, base.Parent.Height);
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripOverflowButtonAccessibleObject();
	}

	protected override ToolStripDropDown CreateDefaultDropDown()
	{
		ToolStripDropDown toolStripDropDown = new ToolStripOverflow(this);
		toolStripDropDown.DefaultDropDownDirection = ToolStripDropDownDirection.BelowLeft;
		toolStripDropDown.OwnerItem = this;
		return toolStripDropDown;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (base.Owner != null)
		{
			base.Owner.Renderer.DrawOverflowButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
		}
	}

	protected internal override void SetBounds(Rectangle bounds)
	{
		base.SetBounds(bounds);
	}
}
