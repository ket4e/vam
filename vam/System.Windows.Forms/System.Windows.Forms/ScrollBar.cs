using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
[DefaultProperty("Value")]
[DefaultEvent("Scroll")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public abstract class ScrollBar : Control
{
	private enum TimerType
	{
		HoldButton,
		RepeatButton,
		HoldThumbArea,
		RepeatThumbArea
	}

	internal enum ThumbMoving
	{
		None,
		Forward,
		Backwards
	}

	private const int thumb_min_size = 8;

	private const int thumb_notshown_size = 40;

	private int position;

	private int minimum;

	private int maximum;

	private int large_change;

	private int small_change;

	internal int scrollbutton_height;

	internal int scrollbutton_width;

	private Rectangle first_arrow_area = default(Rectangle);

	private Rectangle second_arrow_area = default(Rectangle);

	private Rectangle thumb_pos = default(Rectangle);

	private Rectangle thumb_area = default(Rectangle);

	internal ButtonState firstbutton_state;

	internal ButtonState secondbutton_state;

	private bool firstbutton_pressed;

	private bool secondbutton_pressed;

	private bool thumb_pressed;

	private float pixel_per_pos;

	private Timer timer = new Timer();

	private TimerType timer_type;

	private int thumb_size = 40;

	internal bool use_manual_thumb_size;

	internal int manual_thumb_size;

	internal bool vert;

	internal bool implicit_control;

	private int lastclick_pos;

	private int thumbclick_offset;

	private Rectangle dirty;

	internal ThumbMoving thumb_moving;

	private bool first_button_entered;

	private bool second_button_entered;

	private bool thumb_entered;

	private static object ScrollEvent;

	private static object ValueChangedEvent;

	private static object UIAScrollEvent;

	private static object UIAValueChangeEvent;

	internal Rectangle FirstArrowArea
	{
		get
		{
			return first_arrow_area;
		}
		set
		{
			first_arrow_area = value;
		}
	}

	internal Rectangle SecondArrowArea
	{
		get
		{
			return second_arrow_area;
		}
		set
		{
			second_arrow_area = value;
		}
	}

	private int MaximumAllowed => (!use_manual_thumb_size) ? (maximum - LargeChange + 1) : (maximum - manual_thumb_size + 1);

	internal Rectangle ThumbPos
	{
		get
		{
			return thumb_pos;
		}
		set
		{
			thumb_pos = value;
		}
	}

	internal bool FirstButtonEntered
	{
		get
		{
			return first_button_entered;
		}
		private set
		{
			if (first_button_entered != value)
			{
				first_button_entered = value;
				if (ThemeEngine.Current.ScrollBarHasHotElementStyles)
				{
					Invalidate(first_arrow_area);
				}
			}
		}
	}

	internal bool SecondButtonEntered
	{
		get
		{
			return second_button_entered;
		}
		private set
		{
			if (second_button_entered != value)
			{
				second_button_entered = value;
				if (ThemeEngine.Current.ScrollBarHasHotElementStyles)
				{
					Invalidate(second_arrow_area);
				}
			}
		}
	}

	internal bool ThumbEntered
	{
		get
		{
			return thumb_entered;
		}
		private set
		{
			if (thumb_entered != value)
			{
				thumb_entered = value;
				if (ThemeEngine.Current.ScrollBarHasHotElementStyles)
				{
					Invalidate(thumb_pos);
				}
			}
		}
	}

	internal bool ThumbPressed
	{
		get
		{
			return thumb_pressed;
		}
		private set
		{
			if (thumb_pressed != value)
			{
				thumb_pressed = value;
				if (ThemeEngine.Current.ScrollBarHasPressedThumbStyle)
				{
					Invalidate(thumb_pos);
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override bool AutoSize
	{
		get
		{
			return base.AutoSize;
		}
		set
		{
			base.AutoSize = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			if (!(base.BackColor == value))
			{
				base.BackColor = value;
				Refresh();
			}
		}
	}

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
			if (base.BackgroundImage != value)
			{
				base.BackgroundImage = value;
			}
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

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Padding DefaultMargin => Padding.Empty;

	protected override ImeMode DefaultImeMode => ImeMode.Disable;

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
			if (!base.Font.Equals(value))
			{
				base.Font = value;
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			if (!(base.ForeColor == value))
			{
				base.ForeColor = value;
				Refresh();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new ImeMode ImeMode
	{
		get
		{
			return base.ImeMode;
		}
		set
		{
			if (base.ImeMode != value)
			{
				base.ImeMode = value;
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(10)]
	[MWFCategory("Behaviour")]
	[MWFDescription("Scroll amount when clicking in the scroll area")]
	public int LargeChange
	{
		get
		{
			return Math.Min(large_change, maximum - minimum + 1);
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("LargeChange", $"Value '{value}' must be greater than or equal to 0.");
			}
			if (large_change != value)
			{
				large_change = value;
				CalcThumbArea();
				UpdatePos(Value, update_thumbpos: true);
				InvalidateDirty();
				OnUIAValueChanged(new ScrollEventArgs(ScrollEventType.LargeIncrement, value));
			}
		}
	}

	[MWFCategory("Behaviour")]
	[MWFDescription("Highest value for scrollbar")]
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
			if (maximum != value)
			{
				maximum = value;
				OnUIAValueChanged(new ScrollEventArgs(ScrollEventType.Last, value));
				if (maximum < minimum)
				{
					minimum = maximum;
				}
				if (Value > maximum)
				{
					Value = maximum;
				}
				CalcThumbArea();
				UpdatePos(Value, update_thumbpos: true);
				InvalidateDirty();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[MWFCategory("Behaviour")]
	[MWFDescription("Smallest value for scrollbar")]
	[DefaultValue(0)]
	public int Minimum
	{
		get
		{
			return minimum;
		}
		set
		{
			if (minimum != value)
			{
				minimum = value;
				OnUIAValueChanged(new ScrollEventArgs(ScrollEventType.First, value));
				if (minimum > maximum)
				{
					maximum = minimum;
				}
				CalcThumbArea();
				UpdatePos(Value, update_thumbpos: true);
				InvalidateDirty();
			}
		}
	}

	[MWFDescription("Scroll amount when clicking scroll arrows")]
	[DefaultValue(1)]
	[MWFCategory("Behaviour")]
	public int SmallChange
	{
		get
		{
			return (small_change <= LargeChange) ? small_change : LargeChange;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("SmallChange", $"Value '{value}' must be greater than or equal to 0.");
			}
			if (small_change != value)
			{
				small_change = value;
				UpdatePos(Value, update_thumbpos: true);
				InvalidateDirty();
				OnUIAValueChanged(new ScrollEventArgs(ScrollEventType.SmallIncrement, value));
			}
		}
	}

	[DefaultValue(false)]
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

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Bindable(false)]
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

	[MWFCategory("Behaviour")]
	[Bindable(true)]
	[DefaultValue(0)]
	[MWFDescription("Current value for scrollbar")]
	public int Value
	{
		get
		{
			return position;
		}
		set
		{
			if (value < minimum || value > maximum)
			{
				throw new ArgumentOutOfRangeException("Value", $"'{value}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'");
			}
			if (position != value)
			{
				position = value;
				OnValueChanged(EventArgs.Empty);
				if (base.IsHandleCreated)
				{
					Rectangle original_thumbpos = thumb_pos;
					UpdateThumbPos(((!vert) ? thumb_area.X : thumb_area.Y) + (int)((float)(position - minimum) * pixel_per_pos), dirty: false, update_value: false);
					MoveThumb(original_thumbpos, (!vert) ? thumb_pos.X : thumb_pos.Y);
				}
			}
		}
	}

	internal Rectangle UIAThumbArea => thumb_area;

	internal Rectangle UIAThumbPosition => thumb_pos;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler AutoSizeChanged
	{
		add
		{
			base.AutoSizeChanged += value;
		}
		remove
		{
			base.AutoSizeChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BackColorChanged
	{
		add
		{
			base.BackColorChanged += value;
		}
		remove
		{
			base.BackColorChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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
	public new event EventHandler Click
	{
		add
		{
			base.Click += value;
		}
		remove
		{
			base.Click -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ForeColorChanged
	{
		add
		{
			base.ForeColorChanged += value;
		}
		remove
		{
			base.ForeColorChanged -= value;
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event MouseEventHandler MouseClick
	{
		add
		{
			base.MouseClick += value;
		}
		remove
		{
			base.MouseClick -= value;
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event MouseEventHandler MouseDown
	{
		add
		{
			base.MouseDown += value;
		}
		remove
		{
			base.MouseDown -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event MouseEventHandler MouseMove
	{
		add
		{
			base.MouseMove += value;
		}
		remove
		{
			base.MouseMove -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event MouseEventHandler MouseUp
	{
		add
		{
			base.MouseUp += value;
		}
		remove
		{
			base.MouseUp -= value;
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

	public event ScrollEventHandler Scroll
	{
		add
		{
			base.Events.AddHandler(ScrollEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ScrollEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	public event EventHandler ValueChanged
	{
		add
		{
			base.Events.AddHandler(ValueChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ValueChangedEvent, value);
		}
	}

	internal event ScrollEventHandler UIAScroll
	{
		add
		{
			base.Events.AddHandler(UIAScrollEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAScrollEvent, value);
		}
	}

	internal event ScrollEventHandler UIAValueChanged
	{
		add
		{
			base.Events.AddHandler(UIAValueChangeEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAValueChangeEvent, value);
		}
	}

	public ScrollBar()
	{
		position = 0;
		minimum = 0;
		maximum = 100;
		large_change = 10;
		small_change = 1;
		timer.Tick += OnTimer;
		base.MouseEnter += OnMouseEnter;
		base.MouseLeave += OnMouseLeave;
		base.KeyDown += OnKeyDownSB;
		base.MouseDown += OnMouseDownSB;
		base.MouseUp += OnMouseUpSB;
		base.MouseMove += OnMouseMoveSB;
		base.Resize += OnResizeSB;
		base.TabStop = false;
		base.Cursor = Cursors.Default;
		SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, value: false);
	}

	static ScrollBar()
	{
		Scroll = new object();
		ValueChanged = new object();
		UIAScroll = new object();
		UIAValueChangeEvent = new object();
	}

	internal void SetValues(int maximum, int large_change)
	{
		SetValues(-1, maximum, -1, large_change);
	}

	internal void SetValues(int minimum, int maximum, int small_change, int large_change)
	{
		bool flag = false;
		if (minimum != -1 && this.minimum != minimum)
		{
			this.minimum = minimum;
			if (minimum > this.maximum)
			{
				this.maximum = minimum;
			}
			flag = true;
			position = Math.Max(position, minimum);
		}
		if (maximum != -1 && this.maximum != maximum)
		{
			this.maximum = maximum;
			if (maximum < this.minimum)
			{
				this.minimum = maximum;
			}
			flag = true;
			position = Math.Min(position, maximum);
		}
		if (small_change != -1 && this.small_change != small_change)
		{
			this.small_change = small_change;
		}
		if (this.large_change != large_change)
		{
			this.large_change = large_change;
			flag = true;
		}
		if (flag)
		{
			CalcThumbArea();
			UpdatePos(Value, update_thumbpos: true);
			InvalidateDirty();
		}
	}

	protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
	{
		if (vert)
		{
			return base.GetScaledBounds(bounds, factor, (specified & BoundsSpecified.Height) | (specified & BoundsSpecified.Location));
		}
		return base.GetScaledBounds(bounds, factor, (specified & BoundsSpecified.Width) | (specified & BoundsSpecified.Location));
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
		if (base.Enabled)
		{
			firstbutton_state = (secondbutton_state = ButtonState.Normal);
		}
		else
		{
			firstbutton_state = (secondbutton_state = ButtonState.Inactive);
		}
		Refresh();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		CalcButtonSizes();
		CalcThumbArea();
		UpdateThumbPos(thumb_area.Y + (int)((float)(position - minimum) * pixel_per_pos), dirty: true, update_value: false);
	}

	protected virtual void OnScroll(ScrollEventArgs se)
	{
		ScrollEventHandler scrollEventHandler = (ScrollEventHandler)base.Events[Scroll];
		if (scrollEventHandler != null)
		{
			if (se.NewValue < Minimum)
			{
				se.NewValue = Minimum;
			}
			if (se.NewValue > Maximum)
			{
				se.NewValue = Maximum;
			}
			scrollEventHandler(this, se);
		}
	}

	private void SendWMScroll(ScrollBarCommands cmd)
	{
		if (base.Parent != null && base.Parent.IsHandleCreated)
		{
			if (vert)
			{
				XplatUI.SendMessage(base.Parent.Handle, Msg.WM_VSCROLL, (IntPtr)(int)cmd, (!implicit_control) ? Handle : IntPtr.Zero);
			}
			else
			{
				XplatUI.SendMessage(base.Parent.Handle, Msg.WM_HSCROLL, (IntPtr)(int)cmd, (!implicit_control) ? Handle : IntPtr.Zero);
			}
		}
	}

	protected virtual void OnValueChanged(EventArgs e)
	{
		((EventHandler)base.Events[ValueChanged])?.Invoke(this, e);
	}

	public override string ToString()
	{
		return $"{GetType().FullName}, Minimum: {minimum}, Maximum: {maximum}, Value: {position}";
	}

	protected void UpdateScrollInfo()
	{
		Refresh();
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	private void CalcButtonSizes()
	{
		if (vert)
		{
			if (base.Height < ThemeEngine.Current.ScrollBarButtonSize * 2)
			{
				scrollbutton_height = base.Height / 2;
			}
			else
			{
				scrollbutton_height = ThemeEngine.Current.ScrollBarButtonSize;
			}
		}
		else if (base.Width < ThemeEngine.Current.ScrollBarButtonSize * 2)
		{
			scrollbutton_width = base.Width / 2;
		}
		else
		{
			scrollbutton_width = ThemeEngine.Current.ScrollBarButtonSize;
		}
	}

	private void CalcThumbArea()
	{
		int num = ((!use_manual_thumb_size) ? LargeChange : manual_thumb_size);
		if (vert)
		{
			thumb_area.Height = base.Height - scrollbutton_height - scrollbutton_height;
			thumb_area.X = 0;
			thumb_area.Y = scrollbutton_height;
			thumb_area.Width = base.Width;
			if (base.Height < 40)
			{
				thumb_size = 0;
			}
			else
			{
				double num2 = (double)num / (double)(1 + maximum - minimum);
				thumb_size = 1 + (int)((double)thumb_area.Height * num2);
				if (thumb_size < 8)
				{
					thumb_size = 8;
				}
				if (LargeChange == 0)
				{
					thumb_size = 17;
				}
			}
			pixel_per_pos = (float)(thumb_area.Height - thumb_size) / (float)(maximum - minimum - num + 1);
			return;
		}
		thumb_area.Y = 0;
		thumb_area.X = scrollbutton_width;
		thumb_area.Height = base.Height;
		thumb_area.Width = base.Width - scrollbutton_width - scrollbutton_width;
		if (base.Width < 40)
		{
			thumb_size = 0;
		}
		else
		{
			double num3 = (double)num / (double)(1 + maximum - minimum);
			thumb_size = 1 + (int)((double)thumb_area.Width * num3);
			if (thumb_size < 8)
			{
				thumb_size = 8;
			}
			if (LargeChange == 0)
			{
				thumb_size = 17;
			}
		}
		pixel_per_pos = (float)(thumb_area.Width - thumb_size) / (float)(maximum - minimum - num + 1);
	}

	private void LargeIncrement()
	{
		int newValue = Math.Min(MaximumAllowed, position + large_change);
		ScrollEventArgs scrollEventArgs = new ScrollEventArgs(ScrollEventType.LargeIncrement, newValue);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		scrollEventArgs = new ScrollEventArgs(ScrollEventType.EndScroll, Value);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		OnUIAScroll(new ScrollEventArgs(ScrollEventType.LargeIncrement, Value));
	}

	private void LargeDecrement()
	{
		int newValue = Math.Max(Minimum, position - large_change);
		ScrollEventArgs scrollEventArgs = new ScrollEventArgs(ScrollEventType.LargeDecrement, newValue);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		scrollEventArgs = new ScrollEventArgs(ScrollEventType.EndScroll, Value);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		OnUIAScroll(new ScrollEventArgs(ScrollEventType.LargeDecrement, Value));
	}

	private void OnResizeSB(object o, EventArgs e)
	{
		if (base.Width > 0 && base.Height > 0)
		{
			CalcButtonSizes();
			CalcThumbArea();
			UpdatePos(position, update_thumbpos: true);
			Refresh();
		}
	}

	internal override void OnPaintInternal(PaintEventArgs pevent)
	{
		ThemeEngine.Current.DrawScrollBar(pevent.Graphics, pevent.ClipRectangle, this);
	}

	private void OnTimer(object source, EventArgs e)
	{
		ClearDirty();
		switch (timer_type)
		{
		case TimerType.HoldButton:
			SetRepeatButtonTimer();
			break;
		case TimerType.RepeatButton:
			if ((firstbutton_state & ButtonState.Pushed) == ButtonState.Pushed && position != Minimum)
			{
				SmallDecrement();
				SendWMScroll(ScrollBarCommands.SB_LINEUP);
			}
			if ((secondbutton_state & ButtonState.Pushed) == ButtonState.Pushed && position != Maximum)
			{
				SmallIncrement();
				SendWMScroll(ScrollBarCommands.SB_LINEDOWN);
			}
			break;
		case TimerType.HoldThumbArea:
			SetRepeatThumbAreaTimer();
			break;
		case TimerType.RepeatThumbArea:
		{
			Rectangle rectangle = thumb_area;
			Point point = PointToScreen(new Point(thumb_area.X, thumb_area.Y));
			rectangle.X = point.X;
			rectangle.Y = point.Y;
			if (!rectangle.Contains(Control.MousePosition))
			{
				timer.Enabled = false;
				thumb_moving = ThumbMoving.None;
				DirtyThumbArea();
				InvalidateDirty();
			}
			Point pt = PointToClient(Control.MousePosition);
			if (vert)
			{
				lastclick_pos = pt.Y;
			}
			else
			{
				lastclick_pos = pt.X;
			}
			if (thumb_moving == ThumbMoving.Forward)
			{
				if ((vert && thumb_pos.Y + thumb_size > lastclick_pos) || (!vert && thumb_pos.X + thumb_size > lastclick_pos) || !thumb_area.Contains(pt))
				{
					timer.Enabled = false;
					thumb_moving = ThumbMoving.None;
					Refresh();
					return;
				}
				LargeIncrement();
				SendWMScroll(ScrollBarCommands.SB_PAGEDOWN);
			}
			else if ((vert && thumb_pos.Y < lastclick_pos) || (!vert && thumb_pos.X < lastclick_pos))
			{
				timer.Enabled = false;
				thumb_moving = ThumbMoving.None;
				SendWMScroll(ScrollBarCommands.SB_PAGEUP);
				Refresh();
			}
			else
			{
				LargeDecrement();
				SendWMScroll(ScrollBarCommands.SB_PAGEUP);
			}
			break;
		}
		}
		InvalidateDirty();
	}

	private void MoveThumb(Rectangle original_thumbpos, int value)
	{
		if (vert)
		{
			int num = value - original_thumbpos.Y;
			if (num < 0)
			{
				original_thumbpos.Y += num;
				original_thumbpos.Height -= num;
			}
			else
			{
				original_thumbpos.Height += num;
			}
			XplatUI.ScrollWindow(Handle, original_thumbpos, 0, num, with_children: false);
		}
		else
		{
			int num = value - original_thumbpos.X;
			if (num < 0)
			{
				original_thumbpos.X += num;
				original_thumbpos.Width -= num;
			}
			else
			{
				original_thumbpos.Width += num;
			}
			XplatUI.ScrollWindow(Handle, original_thumbpos, num, 0, with_children: false);
		}
		Update();
	}

	private void OnMouseMoveSB(object sender, MouseEventArgs e)
	{
		if (!base.Enabled)
		{
			return;
		}
		FirstButtonEntered = first_arrow_area.Contains(e.Location);
		SecondButtonEntered = second_arrow_area.Contains(e.Location);
		if (thumb_size == 0)
		{
			return;
		}
		ThumbEntered = thumb_pos.Contains(e.Location);
		if (firstbutton_pressed)
		{
			if (!first_arrow_area.Contains(e.X, e.Y) && (firstbutton_state & ButtonState.Pushed) == ButtonState.Pushed)
			{
				firstbutton_state = ButtonState.Normal;
				Invalidate(first_arrow_area);
				Update();
			}
			else if (first_arrow_area.Contains(e.X, e.Y))
			{
				firstbutton_state = ButtonState.Pushed;
				Invalidate(first_arrow_area);
				Update();
			}
		}
		else if (secondbutton_pressed)
		{
			if (!second_arrow_area.Contains(e.X, e.Y) && (secondbutton_state & ButtonState.Pushed) == ButtonState.Pushed)
			{
				secondbutton_state = ButtonState.Normal;
				Invalidate(second_arrow_area);
				Update();
			}
			else if (second_arrow_area.Contains(e.X, e.Y))
			{
				secondbutton_state = ButtonState.Pushed;
				Invalidate(second_arrow_area);
				Update();
			}
		}
		else
		{
			if (!thumb_pressed)
			{
				return;
			}
			if (vert)
			{
				int num = e.Y - thumbclick_offset;
				if (num < thumb_area.Y)
				{
					num = thumb_area.Y;
				}
				else if (num > thumb_area.Bottom - thumb_size)
				{
					num = thumb_area.Bottom - thumb_size;
				}
				if (num != thumb_pos.Y)
				{
					Rectangle original_thumbpos = thumb_pos;
					UpdateThumbPos(num, dirty: false, update_value: true);
					MoveThumb(original_thumbpos, thumb_pos.Y);
					OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, position));
				}
				SendWMScroll(ScrollBarCommands.SB_THUMBTRACK);
			}
			else
			{
				int num2 = e.X - thumbclick_offset;
				if (num2 < thumb_area.X)
				{
					num2 = thumb_area.X;
				}
				else if (num2 > thumb_area.Right - thumb_size)
				{
					num2 = thumb_area.Right - thumb_size;
				}
				if (num2 != thumb_pos.X)
				{
					Rectangle original_thumbpos2 = thumb_pos;
					UpdateThumbPos(num2, dirty: false, update_value: true);
					MoveThumb(original_thumbpos2, thumb_pos.X);
					OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, position));
				}
				SendWMScroll(ScrollBarCommands.SB_THUMBTRACK);
			}
		}
	}

	private void OnMouseDownSB(object sender, MouseEventArgs e)
	{
		ClearDirty();
		if (!base.Enabled || (e.Button & MouseButtons.Left) == 0)
		{
			return;
		}
		if (firstbutton_state != ButtonState.Inactive && first_arrow_area.Contains(e.X, e.Y))
		{
			SendWMScroll(ScrollBarCommands.SB_LINEUP);
			firstbutton_state = ButtonState.Pushed;
			firstbutton_pressed = true;
			Invalidate(first_arrow_area);
			Update();
			if (!timer.Enabled)
			{
				SetHoldButtonClickTimer();
				timer.Enabled = true;
			}
		}
		if (secondbutton_state != ButtonState.Inactive && second_arrow_area.Contains(e.X, e.Y))
		{
			SendWMScroll(ScrollBarCommands.SB_LINEDOWN);
			secondbutton_state = ButtonState.Pushed;
			secondbutton_pressed = true;
			Invalidate(second_arrow_area);
			Update();
			if (!timer.Enabled)
			{
				SetHoldButtonClickTimer();
				timer.Enabled = true;
			}
		}
		if (thumb_size > 0 && thumb_pos.Contains(e.X, e.Y))
		{
			ThumbPressed = true;
			SendWMScroll(ScrollBarCommands.SB_THUMBTRACK);
			if (vert)
			{
				thumbclick_offset = e.Y - thumb_pos.Y;
				lastclick_pos = e.Y;
			}
			else
			{
				thumbclick_offset = e.X - thumb_pos.X;
				lastclick_pos = e.X;
			}
		}
		else
		{
			if (thumb_size <= 0 || !thumb_area.Contains(e.X, e.Y))
			{
				return;
			}
			if (vert)
			{
				lastclick_pos = e.Y;
				if (e.Y > thumb_pos.Y + thumb_pos.Height)
				{
					SendWMScroll(ScrollBarCommands.SB_PAGEDOWN);
					LargeIncrement();
					thumb_moving = ThumbMoving.Forward;
					Dirty(new Rectangle(0, thumb_pos.Y + thumb_pos.Height, base.ClientRectangle.Width, base.ClientRectangle.Height - (thumb_pos.Y + thumb_pos.Height) - scrollbutton_height));
				}
				else
				{
					SendWMScroll(ScrollBarCommands.SB_PAGEUP);
					LargeDecrement();
					thumb_moving = ThumbMoving.Backwards;
					Dirty(new Rectangle(0, scrollbutton_height, base.ClientRectangle.Width, thumb_pos.Y - scrollbutton_height));
				}
			}
			else
			{
				lastclick_pos = e.X;
				if (e.X > thumb_pos.X + thumb_pos.Width)
				{
					SendWMScroll(ScrollBarCommands.SB_PAGEDOWN);
					thumb_moving = ThumbMoving.Forward;
					LargeIncrement();
					Dirty(new Rectangle(thumb_pos.X + thumb_pos.Width, 0, base.ClientRectangle.Width - (thumb_pos.X + thumb_pos.Width) - scrollbutton_width, base.ClientRectangle.Height));
				}
				else
				{
					SendWMScroll(ScrollBarCommands.SB_PAGEUP);
					thumb_moving = ThumbMoving.Backwards;
					LargeDecrement();
					Dirty(new Rectangle(scrollbutton_width, 0, thumb_pos.X - scrollbutton_width, base.ClientRectangle.Height));
				}
			}
			SetHoldThumbAreaTimer();
			timer.Enabled = true;
			InvalidateDirty();
		}
	}

	private void OnMouseUpSB(object sender, MouseEventArgs e)
	{
		ClearDirty();
		if (!base.Enabled)
		{
			return;
		}
		timer.Enabled = false;
		if (thumb_moving != 0)
		{
			DirtyThumbArea();
			thumb_moving = ThumbMoving.None;
		}
		if (firstbutton_pressed)
		{
			firstbutton_state = ButtonState.Normal;
			if (first_arrow_area.Contains(e.X, e.Y))
			{
				SmallDecrement();
			}
			SendWMScroll(ScrollBarCommands.SB_LINEUP);
			firstbutton_pressed = false;
			Dirty(first_arrow_area);
		}
		else if (secondbutton_pressed)
		{
			secondbutton_state = ButtonState.Normal;
			if (second_arrow_area.Contains(e.X, e.Y))
			{
				SmallIncrement();
			}
			SendWMScroll(ScrollBarCommands.SB_LINEDOWN);
			Dirty(second_arrow_area);
			secondbutton_pressed = false;
		}
		else if (thumb_pressed)
		{
			OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, position));
			OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, position));
			SendWMScroll(ScrollBarCommands.SB_THUMBPOSITION);
			ThumbPressed = false;
			return;
		}
		InvalidateDirty();
	}

	private void OnKeyDownSB(object o, KeyEventArgs key)
	{
		if (base.Enabled)
		{
			ClearDirty();
			switch (key.KeyCode)
			{
			case Keys.Up:
				SmallDecrement();
				break;
			case Keys.Down:
				SmallIncrement();
				break;
			case Keys.PageUp:
				LargeDecrement();
				break;
			case Keys.PageDown:
				LargeIncrement();
				break;
			case Keys.Home:
				SetHomePosition();
				break;
			case Keys.End:
				SetEndPosition();
				break;
			}
			InvalidateDirty();
		}
	}

	internal void SafeValueSet(int value)
	{
		value = Math.Min(value, maximum);
		value = Math.Max(value, minimum);
		Value = value;
	}

	private void SetEndPosition()
	{
		int maximumAllowed = MaximumAllowed;
		ScrollEventArgs scrollEventArgs = new ScrollEventArgs(ScrollEventType.Last, maximumAllowed);
		OnScroll(scrollEventArgs);
		maximumAllowed = scrollEventArgs.NewValue;
		scrollEventArgs = new ScrollEventArgs(ScrollEventType.EndScroll, maximumAllowed);
		OnScroll(scrollEventArgs);
		maximumAllowed = scrollEventArgs.NewValue;
		SetValue(maximumAllowed);
	}

	private void SetHomePosition()
	{
		int newValue = Minimum;
		ScrollEventArgs scrollEventArgs = new ScrollEventArgs(ScrollEventType.First, newValue);
		OnScroll(scrollEventArgs);
		newValue = scrollEventArgs.NewValue;
		scrollEventArgs = new ScrollEventArgs(ScrollEventType.EndScroll, newValue);
		OnScroll(scrollEventArgs);
		newValue = scrollEventArgs.NewValue;
		SetValue(newValue);
	}

	private void SmallIncrement()
	{
		int newValue = Math.Min(MaximumAllowed, position + SmallChange);
		ScrollEventArgs scrollEventArgs = new ScrollEventArgs(ScrollEventType.SmallIncrement, newValue);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		scrollEventArgs = new ScrollEventArgs(ScrollEventType.EndScroll, Value);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		OnUIAScroll(new ScrollEventArgs(ScrollEventType.SmallIncrement, Value));
	}

	private void SmallDecrement()
	{
		int newValue = Math.Max(Minimum, position - SmallChange);
		ScrollEventArgs scrollEventArgs = new ScrollEventArgs(ScrollEventType.SmallDecrement, newValue);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		scrollEventArgs = new ScrollEventArgs(ScrollEventType.EndScroll, Value);
		OnScroll(scrollEventArgs);
		Value = scrollEventArgs.NewValue;
		OnUIAScroll(new ScrollEventArgs(ScrollEventType.SmallDecrement, Value));
	}

	private void SetHoldButtonClickTimer()
	{
		timer.Enabled = false;
		timer.Interval = 200;
		timer_type = TimerType.HoldButton;
		timer.Enabled = true;
	}

	private void SetRepeatButtonTimer()
	{
		timer.Enabled = false;
		timer.Interval = 50;
		timer_type = TimerType.RepeatButton;
		timer.Enabled = true;
	}

	private void SetHoldThumbAreaTimer()
	{
		timer.Enabled = false;
		timer.Interval = 200;
		timer_type = TimerType.HoldThumbArea;
		timer.Enabled = true;
	}

	private void SetRepeatThumbAreaTimer()
	{
		timer.Enabled = false;
		timer.Interval = 50;
		timer_type = TimerType.RepeatThumbArea;
		timer.Enabled = true;
	}

	private void UpdatePos(int newPos, bool update_thumbpos)
	{
		int num = ((newPos < minimum) ? minimum : ((newPos <= MaximumAllowed) ? newPos : MaximumAllowed));
		if (num < minimum)
		{
			num = minimum;
		}
		if (num > maximum)
		{
			num = maximum;
		}
		if (update_thumbpos)
		{
			if (vert)
			{
				UpdateThumbPos(thumb_area.Y + (int)((float)(num - minimum) * pixel_per_pos), dirty: true, update_value: false);
			}
			else
			{
				UpdateThumbPos(thumb_area.X + (int)((float)(num - minimum) * pixel_per_pos), dirty: true, update_value: false);
			}
			SetValue(num);
		}
		else
		{
			position = num;
			((EventHandler)base.Events[ValueChanged])?.Invoke(this, EventArgs.Empty);
		}
	}

	private void UpdateThumbPos(int pixel, bool dirty, bool update_value)
	{
		float num = 0f;
		if (vert)
		{
			if (dirty)
			{
				Dirty(thumb_pos);
			}
			if (pixel < thumb_area.Y)
			{
				thumb_pos.Y = thumb_area.Y;
			}
			else if (pixel > thumb_area.Bottom - thumb_size)
			{
				thumb_pos.Y = thumb_area.Bottom - thumb_size;
			}
			else
			{
				thumb_pos.Y = pixel;
			}
			thumb_pos.X = 0;
			thumb_pos.Width = ThemeEngine.Current.ScrollBarButtonSize;
			thumb_pos.Height = thumb_size;
			num = thumb_pos.Y - thumb_area.Y;
			num /= pixel_per_pos;
			if (dirty)
			{
				Dirty(thumb_pos);
			}
		}
		else
		{
			if (dirty)
			{
				Dirty(thumb_pos);
			}
			if (pixel < thumb_area.X)
			{
				thumb_pos.X = thumb_area.X;
			}
			else if (pixel > thumb_area.Right - thumb_size)
			{
				thumb_pos.X = thumb_area.Right - thumb_size;
			}
			else
			{
				thumb_pos.X = pixel;
			}
			thumb_pos.Y = 0;
			thumb_pos.Width = thumb_size;
			thumb_pos.Height = ThemeEngine.Current.ScrollBarButtonSize;
			num = thumb_pos.X - thumb_area.X;
			num /= pixel_per_pos;
			if (dirty)
			{
				Dirty(thumb_pos);
			}
		}
		if (update_value)
		{
			UpdatePos((int)num + minimum, update_thumbpos: false);
		}
	}

	private void SetValue(int value)
	{
		if (value < minimum || value > maximum)
		{
			throw new ArgumentException($"'{value}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'");
		}
		if (position != value)
		{
			position = value;
			OnValueChanged(EventArgs.Empty);
			UpdatePos(value, update_thumbpos: true);
		}
	}

	private void ClearDirty()
	{
		dirty = Rectangle.Empty;
	}

	private void Dirty(Rectangle r)
	{
		if (dirty == Rectangle.Empty)
		{
			dirty = r;
		}
		else
		{
			dirty = Rectangle.Union(dirty, r);
		}
	}

	private void DirtyThumbArea()
	{
		if (thumb_moving == ThumbMoving.Forward)
		{
			if (vert)
			{
				Dirty(new Rectangle(0, thumb_pos.Y + thumb_pos.Height, base.ClientRectangle.Width, base.ClientRectangle.Height - (thumb_pos.Y + thumb_pos.Height) - scrollbutton_height));
			}
			else
			{
				Dirty(new Rectangle(thumb_pos.X + thumb_pos.Width, 0, base.ClientRectangle.Width - (thumb_pos.X + thumb_pos.Width) - scrollbutton_width, base.ClientRectangle.Height));
			}
		}
		else if (thumb_moving == ThumbMoving.Backwards)
		{
			if (vert)
			{
				Dirty(new Rectangle(0, scrollbutton_height, base.ClientRectangle.Width, thumb_pos.Y - scrollbutton_height));
			}
			else
			{
				Dirty(new Rectangle(scrollbutton_width, 0, thumb_pos.X - scrollbutton_width, base.ClientRectangle.Height));
			}
		}
	}

	private void InvalidateDirty()
	{
		Invalidate(dirty);
		Update();
		dirty = Rectangle.Empty;
	}

	private void OnMouseEnter(object sender, EventArgs e)
	{
		if (ThemeEngine.Current.ScrollBarHasHoverArrowButtonStyle)
		{
			Region region = new Region(first_arrow_area);
			region.Union(second_arrow_area);
			Invalidate(region);
		}
	}

	private void OnMouseLeave(object sender, EventArgs e)
	{
		Region region = new Region();
		region.MakeEmpty();
		bool flag = false;
		if (ThemeEngine.Current.ScrollBarHasHoverArrowButtonStyle)
		{
			region.Union(first_arrow_area);
			region.Union(second_arrow_area);
			flag = true;
		}
		else if (ThemeEngine.Current.ScrollBarHasHotElementStyles)
		{
			if (first_button_entered)
			{
				region.Union(first_arrow_area);
				flag = true;
			}
			else if (second_button_entered)
			{
				region.Union(second_arrow_area);
				flag = true;
			}
		}
		if (ThemeEngine.Current.ScrollBarHasHotElementStyles && thumb_entered)
		{
			region.Union(thumb_pos);
			flag = true;
		}
		first_button_entered = false;
		second_button_entered = false;
		thumb_entered = false;
		if (flag)
		{
			Invalidate(region);
		}
		region.Dispose();
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		base.OnMouseWheel(e);
	}

	internal void OnUIAScroll(ScrollEventArgs args)
	{
		((ScrollEventHandler)base.Events[UIAScroll])?.Invoke(this, args);
	}

	internal void OnUIAValueChanged(ScrollEventArgs args)
	{
		((ScrollEventHandler)base.Events[UIAValueChangeEvent])?.Invoke(this, args);
	}

	internal void UIALargeIncrement()
	{
		LargeIncrement();
	}

	internal void UIALargeDecrement()
	{
		LargeDecrement();
	}

	internal void UIASmallIncrement()
	{
		SmallIncrement();
	}

	internal void UIASmallDecrement()
	{
		SmallDecrement();
	}
}
