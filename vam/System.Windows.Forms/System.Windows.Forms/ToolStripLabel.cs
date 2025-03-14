using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
public class ToolStripLabel : ToolStripItem
{
	private Color active_link_color;

	private bool is_link;

	private LinkBehavior link_behavior;

	private Color link_color;

	private bool link_visited;

	private Color visited_link_color;

	private static object UIAIsLinkChangedEvent;

	public Color ActiveLinkColor
	{
		get
		{
			return active_link_color;
		}
		set
		{
			active_link_color = value;
			Invalidate();
		}
	}

	public override bool CanSelect => false;

	[DefaultValue(false)]
	public bool IsLink
	{
		get
		{
			return is_link;
		}
		set
		{
			is_link = value;
			Invalidate();
			OnUIAIsLinkChanged(EventArgs.Empty);
		}
	}

	[DefaultValue(LinkBehavior.SystemDefault)]
	public LinkBehavior LinkBehavior
	{
		get
		{
			return link_behavior;
		}
		set
		{
			link_behavior = value;
			Invalidate();
		}
	}

	public Color LinkColor
	{
		get
		{
			return link_color;
		}
		set
		{
			link_color = value;
			Invalidate();
		}
	}

	[DefaultValue(false)]
	public bool LinkVisited
	{
		get
		{
			return link_visited;
		}
		set
		{
			link_visited = value;
			Invalidate();
		}
	}

	public Color VisitedLinkColor
	{
		get
		{
			return visited_link_color;
		}
		set
		{
			visited_link_color = value;
			Invalidate();
		}
	}

	internal event EventHandler UIAIsLinkChanged
	{
		add
		{
			base.Events.AddHandler(UIAIsLinkChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAIsLinkChangedEvent, value);
		}
	}

	public ToolStripLabel()
		: this(null, null, isLink: false, null, string.Empty)
	{
	}

	public ToolStripLabel(Image image)
		: this(null, image, isLink: false, null, string.Empty)
	{
	}

	public ToolStripLabel(string text)
		: this(text, null, isLink: false, null, string.Empty)
	{
	}

	public ToolStripLabel(string text, Image image)
		: this(text, image, isLink: false, null, string.Empty)
	{
	}

	public ToolStripLabel(string text, Image image, bool isLink)
		: this(text, image, isLink, null, string.Empty)
	{
	}

	public ToolStripLabel(string text, Image image, bool isLink, EventHandler onClick)
		: this(text, image, isLink, onClick, string.Empty)
	{
	}

	public ToolStripLabel(string text, Image image, bool isLink, EventHandler onClick, string name)
		: base(text, image, onClick, name)
	{
		active_link_color = Color.Red;
		is_link = isLink;
		link_behavior = LinkBehavior.SystemDefault;
		link_color = Color.FromArgb(0, 0, 255);
		link_visited = false;
		visited_link_color = Color.FromArgb(128, 0, 128);
	}

	static ToolStripLabel()
	{
		UIAIsLinkChanged = new object();
	}

	internal void OnUIAIsLinkChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIAIsLinkChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		ToolStripItemAccessibleObject toolStripItemAccessibleObject = new ToolStripItemAccessibleObject(this);
		toolStripItemAccessibleObject.role = AccessibleRole.StaticText;
		toolStripItemAccessibleObject.state = AccessibleStates.ReadOnly;
		return toolStripItemAccessibleObject;
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		base.OnMouseEnter(e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (base.Owner == null)
		{
			return;
		}
		Color textColor = ((!Enabled) ? SystemColors.GrayText : ForeColor);
		Image image = ((!Enabled) ? ToolStripRenderer.CreateDisabledImage(Image) : Image);
		base.Owner.Renderer.DrawLabelBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
		CalculateTextAndImageRectangles(out var text_rect, out var image_rect);
		if (base.IsOnDropDown)
		{
			text_rect = ((!base.ShowMargin) ? new Rectangle(7, text_rect.Top, text_rect.Width, text_rect.Height) : new Rectangle(35, text_rect.Top, text_rect.Width, text_rect.Height));
			if (image_rect != Rectangle.Empty)
			{
				image_rect = new Rectangle(new Point(4, 3), GetImageSize());
			}
		}
		if (image_rect != Rectangle.Empty)
		{
			base.Owner.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, image, image_rect));
		}
		if (!(text_rect != Rectangle.Empty))
		{
			return;
		}
		if (is_link)
		{
			if (Pressed)
			{
				switch (link_behavior)
				{
				case LinkBehavior.SystemDefault:
				case LinkBehavior.AlwaysUnderline:
				case LinkBehavior.HoverUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, active_link_color, new Font(Font, FontStyle.Underline), TextAlign));
					break;
				case LinkBehavior.NeverUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, active_link_color, Font, TextAlign));
					break;
				}
			}
			else if (Selected)
			{
				switch (link_behavior)
				{
				case LinkBehavior.SystemDefault:
				case LinkBehavior.AlwaysUnderline:
				case LinkBehavior.HoverUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, link_color, new Font(Font, FontStyle.Underline), TextAlign));
					break;
				case LinkBehavior.NeverUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, link_color, Font, TextAlign));
					break;
				}
			}
			else if (link_visited)
			{
				switch (link_behavior)
				{
				case LinkBehavior.SystemDefault:
				case LinkBehavior.AlwaysUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, visited_link_color, new Font(Font, FontStyle.Underline), TextAlign));
					break;
				case LinkBehavior.HoverUnderline:
				case LinkBehavior.NeverUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, visited_link_color, Font, TextAlign));
					break;
				}
			}
			else
			{
				switch (link_behavior)
				{
				case LinkBehavior.SystemDefault:
				case LinkBehavior.AlwaysUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, link_color, new Font(Font, FontStyle.Underline), TextAlign));
					break;
				case LinkBehavior.HoverUnderline:
				case LinkBehavior.NeverUnderline:
					base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, link_color, Font, TextAlign));
					break;
				}
			}
		}
		else
		{
			base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, textColor, Font, TextAlign));
		}
	}

	protected internal override bool ProcessMnemonic(char charCode)
	{
		base.Parent.SelectNextToolStripItem(this, forward: true);
		return true;
	}
}
