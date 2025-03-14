using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DefaultProperty("Value")]
public class ToolStripProgressBar : ToolStripControlHost
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[DefaultValue(100)]
	public int MarqueeAnimationSpeed
	{
		get
		{
			return ProgressBar.MarqueeAnimationSpeed;
		}
		set
		{
			ProgressBar.MarqueeAnimationSpeed = value;
		}
	}

	[DefaultValue(100)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public int Maximum
	{
		get
		{
			return ProgressBar.Maximum;
		}
		set
		{
			ProgressBar.Maximum = value;
		}
	}

	[DefaultValue(0)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public int Minimum
	{
		get
		{
			return ProgressBar.Minimum;
		}
		set
		{
			ProgressBar.Minimum = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ProgressBar ProgressBar => (ProgressBar)base.Control;

	[DefaultValue(false)]
	[Localizable(true)]
	public virtual bool RightToLeftLayout
	{
		get
		{
			return ProgressBar.RightToLeftLayout;
		}
		set
		{
			ProgressBar.RightToLeftLayout = value;
		}
	}

	[DefaultValue(10)]
	public int Step
	{
		get
		{
			return ProgressBar.Step;
		}
		set
		{
			ProgressBar.Step = value;
		}
	}

	[DefaultValue(ProgressBarStyle.Blocks)]
	public ProgressBarStyle Style
	{
		get
		{
			return ProgressBar.Style;
		}
		set
		{
			ProgressBar.Style = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[DefaultValue(0)]
	[Bindable(true)]
	public int Value
	{
		get
		{
			return ProgressBar.Value;
		}
		set
		{
			ProgressBar.Value = value;
		}
	}

	protected internal override Padding DefaultMargin => new Padding(1, 2, 1, 1);

	protected override Size DefaultSize => new Size(100, 15);

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyEventHandler KeyDown
	{
		add
		{
			base.KeyDown += value;
		}
		remove
		{
			base.KeyDown -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyPressEventHandler KeyPress
	{
		add
		{
			base.KeyPress += value;
		}
		remove
		{
			base.KeyPress -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyEventHandler KeyUp
	{
		add
		{
			base.KeyUp += value;
		}
		remove
		{
			base.KeyUp -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler LocationChanged
	{
		add
		{
			base.LocationChanged += value;
		}
		remove
		{
			base.LocationChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler OwnerChanged
	{
		add
		{
			base.OwnerChanged += value;
		}
		remove
		{
			base.OwnerChanged -= value;
		}
	}

	public event EventHandler RightToLeftLayoutChanged
	{
		add
		{
			ProgressBar.RightToLeftLayoutChanged += value;
		}
		remove
		{
			ProgressBar.RightToLeftLayoutChanged -= value;
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler Validated
	{
		add
		{
			base.Validated += value;
		}
		remove
		{
			base.Validated -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event CancelEventHandler Validating
	{
		add
		{
			base.Validating += value;
		}
		remove
		{
			base.Validating -= value;
		}
	}

	public ToolStripProgressBar()
		: base(new ProgressBar())
	{
	}

	public ToolStripProgressBar(string name)
		: this()
	{
		base.Name = name;
	}

	public void Increment(int value)
	{
		ProgressBar.Increment(value);
	}

	public void PerformStep()
	{
		ProgressBar.PerformStep();
	}

	protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
	{
	}

	protected override void OnSubscribeControlEvents(Control control)
	{
		base.OnSubscribeControlEvents(control);
	}

	protected override void OnUnsubscribeControlEvents(Control control)
	{
		base.OnUnsubscribeControlEvents(control);
	}
}
