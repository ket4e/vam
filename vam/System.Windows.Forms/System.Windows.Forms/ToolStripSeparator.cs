using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
public class ToolStripSeparator : ToolStripItem
{
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			base.BackgroundImage = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	public override bool CanSelect => false;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ToolStripItemDisplayStyle DisplayStyle
	{
		get
		{
			return base.DisplayStyle;
		}
		set
		{
			base.DisplayStyle = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new bool DoubleClickEnabled
	{
		get
		{
			return base.DoubleClickEnabled;
		}
		set
		{
			base.DoubleClickEnabled = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			base.Enabled = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			base.Font = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Image Image
	{
		get
		{
			return base.Image;
		}
		set
		{
			base.Image = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ContentAlignment ImageAlign
	{
		get
		{
			return base.ImageAlign;
		}
		set
		{
			base.ImageAlign = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new int ImageIndex
	{
		get
		{
			return base.ImageIndex;
		}
		set
		{
			base.ImageIndex = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new string ImageKey
	{
		get
		{
			return base.ImageKey;
		}
		set
		{
			base.ImageKey = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new ToolStripItemImageScaling ImageScaling
	{
		get
		{
			return base.ImageScaling;
		}
		set
		{
			base.ImageScaling = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Color ImageTransparentColor
	{
		get
		{
			return base.ImageTransparentColor;
		}
		set
		{
			base.ImageTransparentColor = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ContentAlignment TextAlign
	{
		get
		{
			return base.TextAlign;
		}
		set
		{
			base.TextAlign = value;
		}
	}

	[DefaultValue(ToolStripTextDirection.Horizontal)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override ToolStripTextDirection TextDirection
	{
		get
		{
			return base.TextDirection;
		}
		set
		{
			base.TextDirection = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new TextImageRelation TextImageRelation
	{
		get
		{
			return base.TextImageRelation;
		}
		set
		{
			base.TextImageRelation = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new string ToolTipText
	{
		get
		{
			return base.ToolTipText;
		}
		set
		{
			base.ToolTipText = value;
		}
	}

	protected internal override Padding DefaultMargin => default(Padding);

	protected override Size DefaultSize => new Size(6, 6);

	internal override ToolStripTextDirection DefaultTextDirection => ToolStripTextDirection.Horizontal;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler DisplayStyleChanged
	{
		add
		{
			base.DisplayStyleChanged += value;
		}
		remove
		{
			base.DisplayStyleChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler EnabledChanged
	{
		add
		{
			base.EnabledChanged += value;
		}
		remove
		{
			base.EnabledChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler TextChanged
	{
		add
		{
			base.TextChanged += value;
		}
		remove
		{
			base.TextChanged -= value;
		}
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		return new Size(6, 6);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		ToolStripItemAccessibleObject toolStripItemAccessibleObject = new ToolStripItemAccessibleObject(this);
		toolStripItemAccessibleObject.default_action = "Press";
		toolStripItemAccessibleObject.role = AccessibleRole.Separator;
		toolStripItemAccessibleObject.state = AccessibleStates.None;
		return toolStripItemAccessibleObject;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (base.Owner != null)
		{
			if (base.IsOnDropDown)
			{
				base.Owner.Renderer.DrawSeparator(new ToolStripSeparatorRenderEventArgs(e.Graphics, this, (base.Owner.Orientation != 0) ? true : false));
			}
			else
			{
				base.Owner.Renderer.DrawSeparator(new ToolStripSeparatorRenderEventArgs(e.Graphics, this, (base.Owner.Orientation == Orientation.Horizontal) ? true : false));
			}
		}
	}

	protected internal override void SetBounds(Rectangle rect)
	{
		base.SetBounds(rect);
	}
}
