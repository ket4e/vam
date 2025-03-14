using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultBindingProperty("Value")]
[ComVisible(true)]
[DefaultProperty("Value")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class ProgressBar : Control
{
	private int maximum;

	private int minimum;

	internal int step;

	internal int val;

	internal DateTime start = DateTime.Now;

	internal Rectangle client_area = default(Rectangle);

	internal ProgressBarStyle style;

	private Timer marquee_timer;

	private bool right_to_left_layout;

	private static readonly Color defaultForeColor = SystemColors.Highlight;

	private static object RightToLeftLayoutChangedEvent;

	private int marquee_animation_speed = 100;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool AllowDrop
	{
		get
		{
			return base.AllowDrop;
		}
		set
		{
			base.AllowDrop = value;
		}
	}

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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool CausesValidation
	{
		get
		{
			return base.CausesValidation;
		}
		set
		{
			base.CausesValidation = value;
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override ImeMode DefaultImeMode => base.DefaultImeMode;

	protected override Size DefaultSize => ThemeEngine.Current.ProgressBarDefaultSize;

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool DoubleBuffered
	{
		get
		{
			return base.DoubleBuffered;
		}
		set
		{
			base.DoubleBuffered = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ImeMode ImeMode
	{
		get
		{
			return base.ImeMode;
		}
		set
		{
			base.ImeMode = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(100)]
	public int Maximum
	{
		get
		{
			return maximum;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("Maximum", $"Value '{value}' must be greater than or equal to 0.");
			}
			maximum = value;
			minimum = Math.Min(minimum, maximum);
			val = Math.Min(val, maximum);
			Refresh();
		}
	}

	[DefaultValue(0)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public int Minimum
	{
		get
		{
			return minimum;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("Minimum", $"Value '{value}' must be greater than or equal to 0.");
			}
			minimum = value;
			maximum = Math.Max(maximum, minimum);
			val = Math.Max(val, minimum);
			Refresh();
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
		}
	}

	[DefaultValue(false)]
	[System.MonoTODO("RTL is not supported")]
	[Localizable(true)]
	public virtual bool RightToLeftLayout
	{
		get
		{
			return right_to_left_layout;
		}
		set
		{
			if (right_to_left_layout != value)
			{
				right_to_left_layout = value;
				OnRightToLeftLayoutChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(10)]
	public int Step
	{
		get
		{
			return step;
		}
		set
		{
			step = value;
			Refresh();
		}
	}

	[Browsable(true)]
	[DefaultValue(ProgressBarStyle.Blocks)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public ProgressBarStyle Style
	{
		get
		{
			return style;
		}
		set
		{
			if (value != 0 && value != ProgressBarStyle.Continuous && value != ProgressBarStyle.Marquee)
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ProgressBarStyle));
			}
			if (style == value)
			{
				return;
			}
			style = value;
			if (style == ProgressBarStyle.Marquee)
			{
				if (marquee_timer == null)
				{
					marquee_timer = new Timer();
					marquee_timer.Interval = 10;
					marquee_timer.Tick += marquee_timer_Tick;
				}
				marquee_timer.Start();
			}
			else
			{
				if (marquee_timer != null)
				{
					marquee_timer.Stop();
				}
				Refresh();
			}
		}
	}

	[DefaultValue(100)]
	public int MarqueeAnimationSpeed
	{
		get
		{
			return marquee_animation_speed;
		}
		set
		{
			marquee_animation_speed = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			base.TabStop = value;
		}
	}

	[Bindable(false)]
	[Browsable(false)]
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
			return val;
		}
		set
		{
			if (value < Minimum || value > Maximum)
			{
				throw new ArgumentOutOfRangeException("Value", $"'{value}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'");
			}
			val = value;
			Refresh();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler CausesValidationChanged
	{
		add
		{
			base.CausesValidationChanged += value;
		}
		remove
		{
			base.CausesValidationChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler DoubleClick
	{
		add
		{
			base.DoubleClick += value;
		}
		remove
		{
			base.DoubleClick -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler Enter
	{
		add
		{
			base.Enter += value;
		}
		remove
		{
			base.Enter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler FontChanged
	{
		add
		{
			base.FontChanged += value;
		}
		remove
		{
			base.FontChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler ImeModeChanged
	{
		add
		{
			base.ImeModeChanged += value;
		}
		remove
		{
			base.ImeModeChanged -= value;
		}
	}

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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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
	public new event EventHandler Leave
	{
		add
		{
			base.Leave += value;
		}
		remove
		{
			base.Leave -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event MouseEventHandler MouseDoubleClick
	{
		add
		{
			base.MouseDoubleClick += value;
		}
		remove
		{
			base.MouseDoubleClick -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler PaddingChanged
	{
		add
		{
			base.PaddingChanged += value;
		}
		remove
		{
			base.PaddingChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event PaintEventHandler Paint
	{
		add
		{
			base.Paint += value;
		}
		remove
		{
			base.Paint -= value;
		}
	}

	public event EventHandler RightToLeftLayoutChanged
	{
		add
		{
			base.Events.AddHandler(RightToLeftLayoutChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RightToLeftLayoutChangedEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler TabStopChanged
	{
		add
		{
			base.TabStopChanged += value;
		}
		remove
		{
			base.TabStopChanged -= value;
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

	public ProgressBar()
	{
		maximum = 100;
		minimum = 0;
		step = 10;
		val = 0;
		base.Resize += OnResizeTB;
		SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.UseTextForAccessibility, value: false);
		force_double_buffer = true;
		ForeColor = defaultForeColor;
	}

	static ProgressBar()
	{
		RightToLeftLayoutChanged = new object();
	}

	private void marquee_timer_Tick(object sender, EventArgs e)
	{
		Invalidate();
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
	}

	public void Increment(int value)
	{
		if (Style == ProgressBarStyle.Marquee)
		{
			throw new InvalidOperationException("Increment should not be called if the style is Marquee.");
		}
		int num = Value + value;
		if (num < Minimum)
		{
			num = Minimum;
		}
		if (num > Maximum)
		{
			num = Maximum;
		}
		Value = num;
		Refresh();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		UpdateAreas();
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
	}

	protected override void OnForeColorChanged(EventArgs e)
	{
		base.OnForeColorChanged(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
	{
		((EventHandler)base.Events[RightToLeftLayoutChanged])?.Invoke(this, e);
	}

	public void PerformStep()
	{
		if (Style == ProgressBarStyle.Marquee)
		{
			throw new InvalidOperationException("PerformStep should not be called if the style is Marquee.");
		}
		Increment(Step);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void ResetForeColor()
	{
		ForeColor = defaultForeColor;
	}

	public override string ToString()
	{
		return $"{GetType().FullName}, Minimum: {Minimum.ToString()}, Maximum: {Maximum.ToString()}, Value: {Value.ToString()}";
	}

	private void UpdateAreas()
	{
		ref Rectangle reference = ref client_area;
		int num = 2;
		client_area.Y = num;
		reference.X = num;
		client_area.Width = base.Width - 4;
		client_area.Height = base.Height - 4;
	}

	private void OnResizeTB(object o, EventArgs e)
	{
		if (base.Width > 0 && base.Height > 0)
		{
			UpdateAreas();
			Invalidate();
		}
	}

	internal override void OnPaintInternal(PaintEventArgs pevent)
	{
		ThemeEngine.Current.DrawProgressBar(pevent.Graphics, pevent.ClipRectangle, this);
	}
}
