using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
public class ToolStripDropDownButton : ToolStripDropDownItem
{
	private bool show_drop_down_arrow = true;

	[DefaultValue(true)]
	public new bool AutoToolTip
	{
		get
		{
			return base.AutoToolTip;
		}
		set
		{
			base.AutoToolTip = value;
		}
	}

	[DefaultValue(true)]
	public bool ShowDropDownArrow
	{
		get
		{
			return show_drop_down_arrow;
		}
		set
		{
			if (show_drop_down_arrow != value)
			{
				show_drop_down_arrow = value;
				CalculateAutoSize();
			}
		}
	}

	protected override bool DefaultAutoToolTip => true;

	public ToolStripDropDownButton()
		: this(string.Empty, null, null, string.Empty)
	{
	}

	public ToolStripDropDownButton(Image image)
		: this(string.Empty, image, null, string.Empty)
	{
	}

	public ToolStripDropDownButton(string text)
		: this(text, null, null, string.Empty)
	{
	}

	public ToolStripDropDownButton(string text, Image image)
		: this(text, image, null, string.Empty)
	{
	}

	public ToolStripDropDownButton(string text, Image image, EventHandler onClick)
		: this(text, image, onClick, string.Empty)
	{
	}

	public ToolStripDropDownButton(string text, Image image, params ToolStripItem[] dropDownItems)
		: base(text, image, dropDownItems)
	{
	}

	public ToolStripDropDownButton(string text, Image image, EventHandler onClick, string name)
		: base(text, image, onClick, name)
	{
	}

	protected override ToolStripDropDown CreateDefaultDropDown()
	{
		ToolStripDropDownMenu toolStripDropDownMenu = new ToolStripDropDownMenu();
		toolStripDropDownMenu.OwnerItem = this;
		return toolStripDropDownMenu;
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			if (base.DropDown.Visible)
			{
				HideDropDown(ToolStripDropDownCloseReason.ItemClicked);
			}
			else
			{
				ShowDropDown();
			}
		}
		base.OnMouseDown(e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (base.Owner != null)
		{
			Color textColor = ((!Enabled) ? SystemColors.GrayText : ForeColor);
			Image image = ((!Enabled) ? ToolStripRenderer.CreateDisabledImage(Image) : Image);
			base.Owner.Renderer.DrawDropDownButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
			CalculateTextAndImageRectangles(out var text_rect, out var image_rect);
			if (text_rect != Rectangle.Empty)
			{
				base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, textColor, Font, TextAlign));
			}
			if (image_rect != Rectangle.Empty)
			{
				base.Owner.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, image, image_rect));
			}
			if (ShowDropDownArrow)
			{
				base.Owner.Renderer.DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, this, new Rectangle(base.Width - 10, 0, 6, base.Height), Color.Black, ArrowDirection.Down));
			}
		}
	}

	protected internal override bool ProcessMnemonic(char charCode)
	{
		if (!Selected)
		{
			base.Parent.ChangeSelection(this);
		}
		if (HasDropDownItems)
		{
			ShowDropDown();
		}
		else
		{
			PerformClick();
		}
		return true;
	}

	internal override Size CalculatePreferredSize(Size constrainingSize)
	{
		Size result = base.CalculatePreferredSize(constrainingSize);
		if (ShowDropDownArrow)
		{
			result.Width += 9;
		}
		return result;
	}
}
