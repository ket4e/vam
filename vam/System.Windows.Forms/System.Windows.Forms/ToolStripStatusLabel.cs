using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
public class ToolStripStatusLabel : ToolStripLabel
{
	private ToolStripStatusLabelBorderSides border_sides;

	private Border3DStyle border_style;

	private bool spring;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public new ToolStripItemAlignment Alignment
	{
		get
		{
			return base.Alignment;
		}
		set
		{
			base.Alignment = value;
		}
	}

	[DefaultValue(ToolStripStatusLabelBorderSides.None)]
	public ToolStripStatusLabelBorderSides BorderSides
	{
		get
		{
			return border_sides;
		}
		set
		{
			border_sides = value;
		}
	}

	[DefaultValue(Border3DStyle.Flat)]
	public Border3DStyle BorderStyle
	{
		get
		{
			return border_style;
		}
		set
		{
			border_style = value;
		}
	}

	[DefaultValue(false)]
	public bool Spring
	{
		get
		{
			return spring;
		}
		set
		{
			if (spring != value)
			{
				spring = value;
				CalculateAutoSize();
			}
		}
	}

	protected internal override Padding DefaultMargin => new Padding(0, 3, 0, 2);

	public ToolStripStatusLabel()
		: this(string.Empty, null, null, string.Empty)
	{
	}

	public ToolStripStatusLabel(Image image)
		: this(string.Empty, image, null, string.Empty)
	{
	}

	public ToolStripStatusLabel(string text)
		: this(text, null, null, string.Empty)
	{
	}

	public ToolStripStatusLabel(string text, Image image)
		: this(text, image, null, string.Empty)
	{
	}

	public ToolStripStatusLabel(string text, Image image, EventHandler onClick)
		: this(text, image, onClick, string.Empty)
	{
	}

	public ToolStripStatusLabel(string text, Image image, EventHandler onClick, string name)
		: base(text, image, isLink: false, onClick, name)
	{
		border_style = Border3DStyle.Flat;
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		return base.GetPreferredSize(constrainingSize);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
	}
}
