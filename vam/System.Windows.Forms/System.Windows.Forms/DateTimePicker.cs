using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Windows.Forms;

[DefaultBindingProperty("Value")]
[Designer("System.Windows.Forms.Design.DateTimePickerDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("Value")]
[DefaultEvent("ValueChanged")]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class DateTimePicker : Control
{
	[ComVisible(true)]
	public class DateTimePickerAccessibleObject : ControlAccessibleObject
	{
		private new DateTimePicker owner;

		public override string KeyboardShortcut => base.KeyboardShortcut;

		public override AccessibleRole Role => base.Role;

		public override AccessibleStates State
		{
			get
			{
				AccessibleStates accessibleStates = AccessibleStates.Default;
				if (owner.Checked)
				{
					accessibleStates |= AccessibleStates.Checked;
				}
				return accessibleStates;
			}
		}

		public override string Value => owner.Text;

		public DateTimePickerAccessibleObject(DateTimePicker owner)
			: base(owner)
		{
			this.owner = owner;
		}
	}

	internal enum DateTimePart
	{
		Seconds,
		Minutes,
		AMPMHour,
		Hour,
		Day,
		DayName,
		Month,
		Year,
		AMPMSpecifier,
		Literal
	}

	internal class PartData
	{
		internal string value;

		internal bool is_literal;

		private bool is_selected;

		internal RectangleF drawing_rectangle;

		internal DateTimePart date_time_part;

		private DateTimePicker owner;

		internal bool is_numeric_format
		{
			get
			{
				if (is_literal)
				{
					return false;
				}
				switch (value)
				{
				case "m":
				case "mm":
				case "d":
				case "dd":
				case "h":
				case "hh":
				case "H":
				case "HH":
				case "M":
				case "MM":
				case "s":
				case "ss":
				case "y":
				case "yy":
				case "yyyy":
					return true;
				case "ddd":
				case "dddd":
					return false;
				default:
					return false;
				}
			}
		}

		internal bool Selected
		{
			get
			{
				return is_selected;
			}
			set
			{
				if (value != is_selected)
				{
					owner.EndDateEdit(invalidate: false);
					is_selected = value;
				}
			}
		}

		internal PartData(string value, bool is_literal, DateTimePicker owner)
		{
			this.value = value;
			this.is_literal = is_literal;
			this.owner = owner;
			date_time_part = GetDateTimePart(value);
		}

		internal string GetText(DateTime date)
		{
			if (is_literal)
			{
				return value;
			}
			return GetText(date, value);
		}

		private static DateTimePart GetDateTimePart(string value)
		{
			switch (value)
			{
			case "s":
			case "ss":
				return DateTimePart.Seconds;
			case "m":
			case "mm":
				return DateTimePart.Minutes;
			case "h":
			case "hh":
				return DateTimePart.AMPMHour;
			case "H":
			case "HH":
				return DateTimePart.Hour;
			case "d":
			case "dd":
				return DateTimePart.Day;
			case "ddd":
			case "dddd":
				return DateTimePart.DayName;
			case "M":
			case "MM":
			case "MMMM":
				return DateTimePart.Month;
			case "y":
			case "yy":
			case "yyy":
			case "yyyy":
				return DateTimePart.Year;
			case "t":
			case "tt":
				return DateTimePart.AMPMSpecifier;
			default:
				return DateTimePart.Literal;
			}
		}

		internal static string GetText(DateTime date, string format)
		{
			if (format.StartsWith("g"))
			{
				return " ";
			}
			if (format.Length == 1)
			{
				return date.ToString("%" + format);
			}
			switch (format)
			{
			case "yyyyy":
			case "yyyyyy":
			case "yyyyyyy":
			case "yyyyyyyy":
				return date.ToString("yyyy");
			default:
				if (format.Length > 1)
				{
					return date.ToString(format);
				}
				return string.Empty;
			}
		}
	}

	internal const int check_box_size = 13;

	internal const int check_box_space = 4;

	internal const int up_down_width = 13;

	internal const int initial_timer_delay = 500;

	internal const int subsequent_timer_delay = 100;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly DateTime MaxDateTime = new DateTime(9998, 12, 31, 0, 0, 0);

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);

	protected static readonly Color DefaultMonthBackColor = ThemeEngine.Current.ColorWindow;

	protected static readonly Color DefaultTitleBackColor = ThemeEngine.Current.ColorActiveCaption;

	protected static readonly Color DefaultTitleForeColor = ThemeEngine.Current.ColorActiveCaptionText;

	protected static readonly Color DefaultTrailingForeColor = SystemColors.GrayText;

	internal MonthCalendar month_calendar;

	private bool is_checked;

	private string custom_format;

	private LeftRightAlignment drop_down_align;

	private DateTimePickerFormat format;

	private DateTime max_date;

	private DateTime min_date;

	private bool show_check_box;

	private bool show_up_down;

	private DateTime date_value;

	private bool right_to_left_layout;

	internal bool is_drop_down_visible;

	internal bool is_up_pressed;

	internal bool is_down_pressed;

	internal Timer updown_timer;

	internal bool is_checkbox_selected;

	internal PartData[] part_data;

	internal int editing_part_index = -1;

	internal int editing_number = -1;

	internal string editing_text;

	private bool drop_down_button_entered;

	private static object CloseUpEvent;

	private static object DropDownEvent;

	private static object FormatChangedEvent;

	private static object ValueChangedEvent;

	private static object RightToLeftLayoutChangedEvent;

	private static object UIAMinimumChangedEvent;

	private static object UIAMaximumChangedEvent;

	private static object UIASelectionChangedEvent;

	private static object UIACheckedEvent;

	private static object UIAShowCheckBoxChangedEvent;

	private static object UIAShowUpDownChangedEvent;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
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

	[AmbientValue(null)]
	[Localizable(true)]
	public Font CalendarFont
	{
		get
		{
			return month_calendar.Font;
		}
		set
		{
			month_calendar.Font = value;
		}
	}

	public Color CalendarForeColor
	{
		get
		{
			return month_calendar.ForeColor;
		}
		set
		{
			month_calendar.ForeColor = value;
		}
	}

	public Color CalendarMonthBackground
	{
		get
		{
			return month_calendar.BackColor;
		}
		set
		{
			month_calendar.BackColor = value;
		}
	}

	public Color CalendarTitleBackColor
	{
		get
		{
			return month_calendar.TitleBackColor;
		}
		set
		{
			month_calendar.TitleBackColor = value;
		}
	}

	public Color CalendarTitleForeColor
	{
		get
		{
			return month_calendar.TitleForeColor;
		}
		set
		{
			month_calendar.TitleForeColor = value;
		}
	}

	public Color CalendarTrailingForeColor
	{
		get
		{
			return month_calendar.TrailingForeColor;
		}
		set
		{
			month_calendar.TrailingForeColor = value;
		}
	}

	[DefaultValue(true)]
	[Bindable(true)]
	public bool Checked
	{
		get
		{
			return is_checked;
		}
		set
		{
			if (is_checked == value)
			{
				return;
			}
			is_checked = value;
			if (ShowCheckBox)
			{
				for (int i = 0; i < part_data.Length; i++)
				{
					part_data[i].Selected = false;
				}
				Invalidate(date_area_rect);
				OnUIAChecked();
				OnUIASelectionChanged();
			}
		}
	}

	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(null)]
	public string CustomFormat
	{
		get
		{
			return custom_format;
		}
		set
		{
			if (custom_format != value)
			{
				custom_format = value;
				if (Format == DateTimePickerFormat.Custom)
				{
					CalculateFormats();
				}
			}
		}
	}

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

	[Localizable(true)]
	[DefaultValue(LeftRightAlignment.Left)]
	public LeftRightAlignment DropDownAlign
	{
		get
		{
			return drop_down_align;
		}
		set
		{
			if (drop_down_align != value)
			{
				drop_down_align = value;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			base.ForeColor = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	public DateTimePickerFormat Format
	{
		get
		{
			return format;
		}
		set
		{
			if (format != value)
			{
				format = value;
				RecreateHandle();
				CalculateFormats();
				OnFormatChanged(EventArgs.Empty);
				Invalidate(date_area_rect);
			}
		}
	}

	public DateTime MaxDate
	{
		get
		{
			return max_date;
		}
		set
		{
			if (value < min_date)
			{
				string message = string.Format(CultureInfo.CurrentCulture, "'{0}' is not a valid value for 'MaxDate'. 'MaxDate' must be greater than or equal to MinDate.", value.ToString("G"));
				throw new ArgumentOutOfRangeException("MaxDate", message);
			}
			if (value > MaxDateTime)
			{
				string message2 = string.Format(CultureInfo.CurrentCulture, "DateTimePicker does not support dates after {0}.", MaxDateTime.ToString("G", CultureInfo.CurrentCulture));
				throw new ArgumentOutOfRangeException("MaxDate", message2);
			}
			if (max_date != value)
			{
				max_date = value;
				if (Value > max_date)
				{
					Value = max_date;
					Invalidate(date_area_rect);
				}
				OnUIAMaximumChanged();
			}
		}
	}

	public static DateTime MaximumDateTime => MaxDateTime;

	public DateTime MinDate
	{
		get
		{
			return min_date;
		}
		set
		{
			if (value == DateTime.MinValue)
			{
				value = MinDateTime;
			}
			if (value > MaxDate)
			{
				string message = string.Format(CultureInfo.CurrentCulture, "'{0}' is not a valid value for 'MinDate'. 'MinDate' must be less than MaxDate.", value.ToString("G"));
				throw new ArgumentOutOfRangeException("MinDate", message);
			}
			if (value < MinDateTime)
			{
				string message2 = string.Format(CultureInfo.CurrentCulture, "DateTimePicker does not support dates before {0}.", MinDateTime.ToString("G", CultureInfo.CurrentCulture));
				throw new ArgumentOutOfRangeException("MinDate", message2);
			}
			if (min_date != value)
			{
				min_date = value;
				if (Value < min_date)
				{
					Value = min_date;
					Invalidate(date_area_rect);
				}
				OnUIAMinimumChanged();
			}
		}
	}

	public static DateTime MinimumDateTime => MinDateTime;

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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public int PreferredHeight => (int)Math.Ceiling((double)Font.Height * 1.5);

	[DefaultValue(false)]
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

	[DefaultValue(false)]
	public bool ShowCheckBox
	{
		get
		{
			return show_check_box;
		}
		set
		{
			if (show_check_box != value)
			{
				show_check_box = value;
				Invalidate(date_area_rect);
				OnUIAShowCheckBoxChanged();
			}
		}
	}

	[DefaultValue(false)]
	public bool ShowUpDown
	{
		get
		{
			return show_up_down;
		}
		set
		{
			if (show_up_down != value)
			{
				show_up_down = value;
				Invalidate();
				OnUIAShowUpDownChanged();
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public override string Text
	{
		get
		{
			if (!base.IsHandleCreated)
			{
				return string.Empty;
			}
			if (format == DateTimePickerFormat.Custom)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < part_data.Length; i++)
				{
					stringBuilder.Append(part_data[i].GetText(date_value));
				}
				return stringBuilder.ToString();
			}
			return Value.ToString(GetExactFormat());
		}
		set
		{
			if (value == null || value == string.Empty)
			{
				date_value = DateTime.Now;
				OnValueChanged(EventArgs.Empty);
				OnTextChanged(EventArgs.Empty);
				return;
			}
			DateTime dateTime = ((format != DateTimePickerFormat.Custom) ? DateTime.ParseExact(value, GetExactFormat(), null) : DateTime.ParseExact(value, GetExactFormat(), null));
			if (date_value != dateTime)
			{
				Value = dateTime;
			}
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[Bindable(true)]
	public DateTime Value
	{
		get
		{
			return date_value;
		}
		set
		{
			if (date_value != value)
			{
				if (value < MinDate || value > MaxDate)
				{
					throw new ArgumentOutOfRangeException("value", "value must be between MinDate and MaxDate");
				}
				date_value = value;
				OnValueChanged(EventArgs.Empty);
				Invalidate(date_area_rect);
			}
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => new Size(200, PreferredHeight);

	internal Rectangle date_area_rect => ThemeEngine.Current.DateTimePickerGetDateArea(this);

	internal Rectangle CheckBoxRect
	{
		get
		{
			Rectangle result = new Rectangle(4, base.ClientSize.Height / 2 - 6, 13, 13);
			return result;
		}
	}

	internal Rectangle drop_down_arrow_rect => ThemeEngine.Current.DateTimePickerGetDropDownButtonArea(this);

	internal Rectangle hilight_date_area => Rectangle.Empty;

	internal bool DropDownButtonEntered => drop_down_button_entered;

	internal bool UIAIsCheckBoxSelected => is_checkbox_selected;

	public event EventHandler CloseUp
	{
		add
		{
			base.Events.AddHandler(CloseUpEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CloseUpEvent, value);
		}
	}

	public event EventHandler DropDown
	{
		add
		{
			base.Events.AddHandler(DropDownEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownEvent, value);
		}
	}

	public event EventHandler FormatChanged
	{
		add
		{
			base.Events.AddHandler(FormatChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FormatChangedEvent, value);
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[EditorBrowsable(EditorBrowsableState.Advanced)]
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

	internal event EventHandler UIAMinimumChanged
	{
		add
		{
			base.Events.AddHandler(UIAMinimumChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAMinimumChangedEvent, value);
		}
	}

	internal event EventHandler UIAMaximumChanged
	{
		add
		{
			base.Events.AddHandler(UIAMinimumChanged, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAMinimumChanged, value);
		}
	}

	internal event EventHandler UIASelectionChanged
	{
		add
		{
			base.Events.AddHandler(UIASelectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIASelectionChangedEvent, value);
		}
	}

	internal event EventHandler UIAChecked
	{
		add
		{
			base.Events.AddHandler(UIACheckedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACheckedEvent, value);
		}
	}

	internal event EventHandler UIAShowCheckBoxChanged
	{
		add
		{
			base.Events.AddHandler(UIAShowCheckBoxChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAShowCheckBoxChangedEvent, value);
		}
	}

	internal event EventHandler UIAShowUpDownChanged
	{
		add
		{
			base.Events.AddHandler(UIAShowUpDownChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAShowUpDownChangedEvent, value);
		}
	}

	public DateTimePicker()
	{
		month_calendar = new MonthCalendar(this);
		month_calendar.CalendarDimensions = new Size(1, 1);
		month_calendar.MaxSelectionCount = 1;
		month_calendar.ForeColor = Control.DefaultForeColor;
		month_calendar.BackColor = DefaultMonthBackColor;
		month_calendar.TitleBackColor = DefaultTitleBackColor;
		month_calendar.TitleForeColor = DefaultTitleForeColor;
		month_calendar.TrailingForeColor = DefaultTrailingForeColor;
		month_calendar.Visible = false;
		updown_timer = new Timer();
		updown_timer.Interval = 500;
		is_checked = true;
		custom_format = null;
		drop_down_align = LeftRightAlignment.Left;
		format = DateTimePickerFormat.Long;
		max_date = MaxDateTime;
		min_date = MinDateTime;
		show_check_box = false;
		show_up_down = false;
		date_value = DateTime.Now;
		is_drop_down_visible = false;
		BackColor = SystemColors.Window;
		ForeColor = SystemColors.WindowText;
		month_calendar.DateChanged += MonthCalendarDateChangedHandler;
		month_calendar.DateSelected += MonthCalendarDateSelectedHandler;
		month_calendar.LostFocus += MonthCalendarLostFocusHandler;
		updown_timer.Tick += UpDownTimerTick;
		base.KeyPress += KeyPressHandler;
		base.KeyDown += KeyDownHandler;
		base.GotFocus += GotFocusHandler;
		base.LostFocus += LostFocusHandler;
		base.MouseDown += MouseDownHandler;
		base.MouseUp += MouseUpHandler;
		base.MouseEnter += OnMouseEnter;
		base.MouseLeave += OnMouseLeave;
		base.MouseMove += OnMouseMove;
		Paint += PaintHandler;
		base.Resize += ResizeHandler;
		SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick, value: false);
		SetStyle(ControlStyles.FixedHeight, value: true);
		SetStyle(ControlStyles.Selectable, value: true);
		CalculateFormats();
	}

	static DateTimePicker()
	{
		CloseUp = new object();
		DropDown = new object();
		FormatChanged = new object();
		ValueChanged = new object();
		RightToLeftLayoutChanged = new object();
		UIAMinimumChanged = new object();
		UIAMaximumChanged = new object();
		UIASelectionChanged = new object();
		UIAChecked = new object();
		UIAShowCheckBoxChanged = new object();
		UIAShowUpDownChanged = new object();
	}

	public override string ToString()
	{
		return Text;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return base.CreateAccessibilityInstance();
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
	}

	protected override void DestroyHandle()
	{
		base.DestroyHandle();
	}

	protected override bool IsInputKey(Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
			return true;
		default:
			return false;
		}
	}

	protected virtual void OnCloseUp(EventArgs eventargs)
	{
		((EventHandler)base.Events[CloseUp])?.Invoke(this, eventargs);
	}

	protected virtual void OnDropDown(EventArgs eventargs)
	{
		((EventHandler)base.Events[DropDown])?.Invoke(this, eventargs);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		month_calendar.Font = Font;
		base.Size = new Size(base.Size.Width, PreferredHeight);
		base.OnFontChanged(e);
	}

	protected virtual void OnFormatChanged(EventArgs e)
	{
		((EventHandler)base.Events[FormatChanged])?.Invoke(this, e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
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

	protected override void OnSystemColorsChanged(EventArgs e)
	{
		base.OnSystemColorsChanged(e);
	}

	protected virtual void OnValueChanged(EventArgs eventargs)
	{
		((EventHandler)base.Events[ValueChanged])?.Invoke(this, eventargs);
	}

	internal override int OverrideHeight(int height)
	{
		return DefaultSize.Height;
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	private void ResizeHandler(object sender, EventArgs e)
	{
		Invalidate();
	}

	private void UpDownTimerTick(object sender, EventArgs e)
	{
		if (updown_timer.Interval == 500)
		{
			updown_timer.Interval = 100;
		}
		if (is_down_pressed)
		{
			IncrementSelectedPart(-1);
		}
		else if (is_up_pressed)
		{
			IncrementSelectedPart(1);
		}
		else
		{
			updown_timer.Enabled = false;
		}
	}

	internal float CalculateMaxWidth(string format, Graphics gr, StringFormat string_format)
	{
		float num = 0f;
		Font font = Font;
		switch (format)
		{
		case "M":
		case "MM":
		case "MMM":
		case "MMMM":
		{
			for (int j = 1; j <= 12; j++)
			{
				string text = PartData.GetText(Value.AddMonths(j), format);
				num = Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
			}
			return num;
		}
		case "d":
		case "dd":
		case "ddd":
		case "dddd":
		{
			for (int n = 1; n <= 12; n++)
			{
				string text = PartData.GetText(Value.AddDays(n), format);
				num = Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
			}
			return num;
		}
		case "h":
		case "hh":
		{
			for (int k = 1; k <= 12; k++)
			{
				string text = PartData.GetText(Value.AddHours(k), format);
				num = Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
			}
			return num;
		}
		case "H":
		case "HH":
		{
			for (int m = 1; m <= 24; m++)
			{
				string text = PartData.GetText(Value.AddDays(m), format);
				num = Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
			}
			return num;
		}
		case "m":
		case "mm":
		{
			for (int num2 = 1; num2 <= 60; num2++)
			{
				string text = PartData.GetText(Value.AddMinutes(num2), format);
				num = Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
			}
			return num;
		}
		case "s":
		case "ss":
		{
			for (int l = 1; l <= 60; l++)
			{
				string text = PartData.GetText(Value.AddSeconds(l), format);
				num = Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
			}
			return num;
		}
		case "t":
		case "tt":
		{
			for (int i = 1; i <= 2; i++)
			{
				string text = PartData.GetText(Value.AddHours(i * 12), format);
				num = Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
			}
			return num;
		}
		case "y":
		case "yy":
		case "yyyy":
		{
			string text = PartData.GetText(Value, format);
			return Math.Max(num, gr.MeasureString(text, font, int.MaxValue, string_format).Width);
		}
		default:
			return gr.MeasureString(format, font, int.MaxValue, string_format).Width;
		}
	}

	private string GetExactFormat()
	{
		return format switch
		{
			DateTimePickerFormat.Long => Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern, 
			DateTimePickerFormat.Short => Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern, 
			DateTimePickerFormat.Time => Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern, 
			DateTimePickerFormat.Custom => (custom_format != null) ? custom_format : string.Empty, 
			_ => Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern, 
		};
	}

	private void CalculateFormats()
	{
		StringBuilder stringBuilder = new StringBuilder();
		ArrayList arrayList = new ArrayList();
		bool flag = false;
		char c = '\0';
		string exactFormat = GetExactFormat();
		for (int i = 0; i < exactFormat.Length; i++)
		{
			char c2 = exactFormat[i];
			if (flag && c2 != '\'')
			{
				stringBuilder.Append(c2);
				continue;
			}
			switch (c2)
			{
			case 'H':
			case 'M':
			case 'd':
			case 'g':
			case 'h':
			case 'm':
			case 's':
			case 't':
			case 'y':
				if (c != c2 && c != 0 && stringBuilder.Length != 0)
				{
					arrayList.Add(new PartData(stringBuilder.ToString(), is_literal: false, this));
					stringBuilder.Length = 0;
				}
				stringBuilder.Append(c2);
				break;
			case '\'':
				if (flag && i < exactFormat.Length - 1 && exactFormat[i + 1] == '\'')
				{
					stringBuilder.Append(c2);
					i++;
				}
				else if (stringBuilder.Length == 0)
				{
					flag = !flag;
				}
				else
				{
					arrayList.Add(new PartData(stringBuilder.ToString(), flag, this));
					stringBuilder.Length = 0;
					flag = !flag;
				}
				break;
			default:
				if (stringBuilder.Length != 0)
				{
					arrayList.Add(new PartData(stringBuilder.ToString(), is_literal: false, this));
					stringBuilder.Length = 0;
				}
				arrayList.Add(new PartData(c2.ToString(), is_literal: true, this));
				break;
			}
			c = c2;
		}
		if (stringBuilder.Length >= 0)
		{
			arrayList.Add(new PartData(stringBuilder.ToString(), flag, this));
		}
		part_data = new PartData[arrayList.Count];
		arrayList.CopyTo(part_data);
	}

	private Point CalculateDropDownLocation(Rectangle parent_control_rect, Size child_size, bool align_left)
	{
		Point p = new Point(parent_control_rect.Left + 5, parent_control_rect.Bottom);
		if (!align_left)
		{
			p.X = parent_control_rect.Right - child_size.Width;
		}
		Point point = PointToScreen(p);
		Rectangle workingArea = Screen.FromControl(this).WorkingArea;
		if (point.X < workingArea.X)
		{
			point.X = workingArea.X;
		}
		if (point.Y + child_size.Height > workingArea.Bottom)
		{
			point.Y -= parent_control_rect.Height + child_size.Height;
		}
		if (month_calendar.Parent != null)
		{
			point = month_calendar.Parent.PointToClient(point);
		}
		return point;
	}

	internal void Draw(Rectangle clip_rect, Graphics dc)
	{
		ThemeEngine.Current.DrawDateTimePicker(dc, clip_rect, this);
	}

	internal void DropDownMonthCalendar()
	{
		EndDateEdit(invalidate: true);
		month_calendar.SetDate(date_value);
		Rectangle parent_control_rect = date_area_rect;
		parent_control_rect.Y = base.ClientRectangle.Y;
		parent_control_rect.Height = base.ClientRectangle.Height;
		month_calendar.Location = CalculateDropDownLocation(parent_control_rect, month_calendar.Size, DropDownAlign == LeftRightAlignment.Left);
		month_calendar.Show();
		month_calendar.Focus();
		month_calendar.Capture = true;
		((EventHandler)base.Events[DropDown])?.Invoke(this, EventArgs.Empty);
	}

	internal void HideMonthCalendar()
	{
		is_drop_down_visible = false;
		Invalidate(drop_down_arrow_rect);
		month_calendar.Capture = false;
		if (month_calendar.Visible)
		{
			month_calendar.Hide();
		}
		Focus();
	}

	private int GetSelectedPartIndex()
	{
		for (int i = 0; i < part_data.Length; i++)
		{
			if (part_data[i].Selected && !part_data[i].is_literal)
			{
				return i;
			}
		}
		return -1;
	}

	internal void IncrementSelectedPart(int delta)
	{
		int selectedPartIndex = GetSelectedPartIndex();
		if (selectedPartIndex == -1)
		{
			return;
		}
		EndDateEdit(invalidate: false);
		DateTimePart date_time_part = part_data[selectedPartIndex].date_time_part;
		switch (date_time_part)
		{
		case DateTimePart.Day:
			if (delta < 0)
			{
				if (Value.Day == 1)
				{
					SetPart(DateTime.DaysInMonth(Value.Year, Value.Month), date_time_part);
				}
				else
				{
					SetPart(Value.Day + delta, date_time_part);
				}
			}
			else if (Value.Day == DateTime.DaysInMonth(Value.Year, Value.Month))
			{
				SetPart(1, date_time_part);
			}
			else
			{
				SetPart(Value.Day + delta, date_time_part);
			}
			break;
		case DateTimePart.DayName:
			Value = Value.AddDays(delta);
			break;
		case DateTimePart.AMPMHour:
		case DateTimePart.Hour:
			SetPart(Value.Hour + delta, date_time_part);
			break;
		case DateTimePart.Minutes:
			SetPart(Value.Minute + delta, date_time_part);
			break;
		case DateTimePart.Month:
			SetPart(Value.Month + delta, date_time_part, adjust: true);
			break;
		case DateTimePart.Seconds:
			SetPart(Value.Second + delta, date_time_part);
			break;
		case DateTimePart.AMPMSpecifier:
		{
			int hour = Value.Hour;
			hour = ((hour < 0 || hour > 11) ? (hour - 12) : (hour + 12));
			SetPart(hour, DateTimePart.Hour);
			break;
		}
		case DateTimePart.Year:
			SetPart(Value.Year + delta, date_time_part);
			break;
		}
	}

	internal void SelectPart(int index)
	{
		is_checkbox_selected = false;
		for (int i = 0; i < part_data.Length; i++)
		{
			part_data[i].Selected = i == index;
		}
		Invalidate();
		OnUIASelectionChanged();
	}

	internal void SelectNextPart()
	{
		if (is_checkbox_selected)
		{
			for (int i = 0; i < part_data.Length; i++)
			{
				if (!part_data[i].is_literal)
				{
					is_checkbox_selected = false;
					part_data[i].Selected = true;
					Invalidate();
					break;
				}
			}
		}
		else
		{
			int selectedPartIndex = GetSelectedPartIndex();
			if (selectedPartIndex >= 0)
			{
				part_data[selectedPartIndex].Selected = false;
			}
			for (int j = selectedPartIndex + 1; j < part_data.Length; j++)
			{
				if (!part_data[j].is_literal)
				{
					part_data[j].Selected = true;
					Invalidate();
					break;
				}
			}
			if (GetSelectedPartIndex() == -1)
			{
				if (ShowCheckBox)
				{
					is_checkbox_selected = true;
					Invalidate();
				}
				else
				{
					for (int k = 0; k <= selectedPartIndex; k++)
					{
						if (!part_data[k].is_literal)
						{
							part_data[k].Selected = true;
							Invalidate();
							break;
						}
					}
				}
			}
		}
		OnUIASelectionChanged();
	}

	internal void SelectPreviousPart()
	{
		if (is_checkbox_selected)
		{
			for (int num = part_data.Length - 1; num >= 0; num--)
			{
				if (!part_data[num].is_literal)
				{
					is_checkbox_selected = false;
					part_data[num].Selected = true;
					Invalidate();
					break;
				}
			}
		}
		else
		{
			int selectedPartIndex = GetSelectedPartIndex();
			if (selectedPartIndex >= 0)
			{
				part_data[selectedPartIndex].Selected = false;
			}
			for (int num2 = selectedPartIndex - 1; num2 >= 0; num2--)
			{
				if (!part_data[num2].is_literal)
				{
					part_data[num2].Selected = true;
					Invalidate();
					break;
				}
			}
			if (GetSelectedPartIndex() == -1)
			{
				if (ShowCheckBox)
				{
					is_checkbox_selected = true;
					Invalidate();
				}
				else
				{
					for (int num3 = part_data.Length - 1; num3 >= selectedPartIndex; num3--)
					{
						if (!part_data[num3].is_literal)
						{
							part_data[num3].Selected = true;
							Invalidate();
							break;
						}
					}
				}
			}
		}
		OnUIASelectionChanged();
	}

	private void KeyDownHandler(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Up:
		case Keys.Add:
			if (!ShowCheckBox || Checked)
			{
				IncrementSelectedPart(1);
				e.Handled = true;
			}
			break;
		case Keys.Down:
		case Keys.Subtract:
			if (!ShowCheckBox || Checked)
			{
				IncrementSelectedPart(-1);
				e.Handled = true;
			}
			break;
		case Keys.Left:
			if (!ShowCheckBox || Checked)
			{
				SelectPreviousPart();
				e.Handled = true;
			}
			break;
		case Keys.Right:
			if (!ShowCheckBox || Checked)
			{
				SelectNextPart();
				e.Handled = true;
			}
			break;
		case Keys.F4:
			if (!e.Alt && !is_drop_down_visible)
			{
				DropDownMonthCalendar();
				e.Handled = true;
			}
			break;
		}
	}

	private void KeyPressHandler(object sender, KeyPressEventArgs e)
	{
		switch (e.KeyChar)
		{
		case ' ':
			if (show_check_box && is_checkbox_selected)
			{
				Checked = !Checked;
			}
			break;
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
		{
			int num = e.KeyChar - 48;
			int selectedPartIndex = GetSelectedPartIndex();
			if (selectedPartIndex != -1 && part_data[selectedPartIndex].is_numeric_format)
			{
				DateTimePart date_time_part = part_data[selectedPartIndex].date_time_part;
				if (editing_part_index < 0)
				{
					editing_part_index = selectedPartIndex;
					editing_number = 0;
					editing_text = string.Empty;
				}
				editing_text += num;
				int num2 = 0;
				switch (date_time_part)
				{
				case DateTimePart.Seconds:
				case DateTimePart.Minutes:
				case DateTimePart.AMPMHour:
				case DateTimePart.Hour:
				case DateTimePart.Day:
				case DateTimePart.Month:
					num2 = 2;
					break;
				case DateTimePart.Year:
					num2 = 4;
					break;
				}
				editing_number = editing_number * 10 + num;
				if (editing_text.Length >= num2)
				{
					EndDateEdit(invalidate: false);
				}
				Invalidate(date_area_rect);
			}
			break;
		}
		}
		e.Handled = true;
	}

	private void EndDateEdit(bool invalidate)
	{
		if (editing_part_index == -1)
		{
			return;
		}
		PartData partData = part_data[editing_part_index];
		if (partData.date_time_part == DateTimePart.Year)
		{
			if (editing_number > 0 && editing_number < 30)
			{
				editing_number += 2000;
			}
			else if (editing_number >= 30 && editing_number < 100)
			{
				editing_number += 1900;
			}
		}
		SetPart(editing_number, partData.date_time_part);
		editing_part_index = (editing_number = -1);
		editing_text = null;
		if (invalidate)
		{
			Invalidate(date_area_rect);
		}
	}

	internal void SetPart(int value, DateTimePart dt_part)
	{
		SetPart(value, dt_part, adjust: false);
	}

	internal void SetPart(int value, DateTimePart dt_part, bool adjust)
	{
		switch (dt_part)
		{
		case DateTimePart.Seconds:
			if (value == -1)
			{
				value = 59;
			}
			if (value >= 0 && value <= 59)
			{
				Value = new DateTime(Value.Year, Value.Month, Value.Day, Value.Hour, Value.Minute, value, Value.Millisecond);
			}
			break;
		case DateTimePart.Minutes:
			if (value == -1)
			{
				value = 59;
			}
			if (value >= 0 && value <= 59)
			{
				Value = new DateTime(Value.Year, Value.Month, Value.Day, Value.Hour, value, Value.Second, Value.Millisecond);
			}
			break;
		case DateTimePart.AMPMHour:
			if (value == -1)
			{
				value = 23;
			}
			if (value >= 0 && value <= 23)
			{
				int hour = Value.Hour;
				if (hour >= 12 && hour <= 23 && value < 12)
				{
					value += 12;
				}
				Value = new DateTime(Value.Year, Value.Month, Value.Day, value, Value.Minute, Value.Second, Value.Millisecond);
			}
			break;
		case DateTimePart.Hour:
			if (value == -1)
			{
				value = 23;
			}
			if (value >= 0 && value <= 23)
			{
				Value = new DateTime(Value.Year, Value.Month, Value.Day, value, Value.Minute, Value.Second, Value.Millisecond);
			}
			break;
		case DateTimePart.Day:
		{
			int num3 = DateTime.DaysInMonth(Value.Year, Value.Month);
			if (value >= 1 && value <= 31 && value <= num3)
			{
				Value = new DateTime(Value.Year, Value.Month, value, Value.Hour, Value.Minute, Value.Second, Value.Millisecond);
			}
			break;
		}
		case DateTimePart.Month:
		{
			DateTime dateTime = Value;
			if (adjust)
			{
				switch (value)
				{
				case 0:
					dateTime = dateTime.AddYears(-1);
					value = 12;
					break;
				case 13:
					dateTime = dateTime.AddYears(1);
					value = 1;
					break;
				}
			}
			if (value >= 1 && value <= 12)
			{
				int num2 = DateTime.DaysInMonth(dateTime.Year, value);
				if (dateTime.Day > num2)
				{
					Value = new DateTime(dateTime.Year, value, num2, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				}
				else
				{
					Value = new DateTime(dateTime.Year, value, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				}
			}
			break;
		}
		case DateTimePart.Year:
			if (value >= min_date.Year && value <= max_date.Year)
			{
				int num = DateTime.DaysInMonth(value, Value.Month);
				if (Value.Day > num)
				{
					Value = new DateTime(value, Value.Month, num, Value.Hour, Value.Minute, Value.Second, Value.Millisecond);
				}
				else
				{
					Value = new DateTime(value, Value.Month, Value.Day, Value.Hour, Value.Minute, Value.Second, Value.Millisecond);
				}
			}
			break;
		case DateTimePart.DayName:
			break;
		}
	}

	private void GotFocusHandler(object sender, EventArgs e)
	{
		if (ShowCheckBox)
		{
			is_checkbox_selected = true;
			Invalidate(CheckBoxRect);
			OnUIASelectionChanged();
		}
	}

	private void LostFocusHandler(object sender, EventArgs e)
	{
		int selectedPartIndex = GetSelectedPartIndex();
		if (selectedPartIndex != -1)
		{
			part_data[selectedPartIndex].Selected = false;
			Rectangle rc = Rectangle.Ceiling(part_data[selectedPartIndex].drawing_rectangle);
			rc.Inflate(2, 2);
			Invalidate(rc);
			OnUIASelectionChanged();
		}
		else if (is_checkbox_selected)
		{
			is_checkbox_selected = false;
			Invalidate(CheckBoxRect);
			OnUIASelectionChanged();
		}
	}

	private void MonthCalendarLostFocusHandler(object sender, EventArgs e)
	{
		if (is_drop_down_visible && month_calendar.Focused)
		{
		}
	}

	private void MonthCalendarDateChangedHandler(object sender, DateRangeEventArgs e)
	{
		if (month_calendar.Visible)
		{
			Value = e.Start.Date.Add(Value.TimeOfDay);
		}
	}

	private void MonthCalendarDateSelectedHandler(object sender, DateRangeEventArgs e)
	{
		HideMonthCalendar();
	}

	private void MouseUpHandler(object sender, MouseEventArgs e)
	{
		if (ShowUpDown && (is_up_pressed || is_down_pressed))
		{
			updown_timer.Enabled = false;
			is_up_pressed = false;
			is_down_pressed = false;
			Invalidate(drop_down_arrow_rect);
		}
	}

	private void MouseDownHandler(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		if (ShowCheckBox && CheckBoxRect.Contains(e.X, e.Y))
		{
			is_checkbox_selected = true;
			Checked = !Checked;
			OnUIASelectionChanged();
			return;
		}
		if (Checked)
		{
			is_checkbox_selected = false;
			OnUIASelectionChanged();
		}
		if (ShowUpDown && drop_down_arrow_rect.Contains(e.X, e.Y))
		{
			if (!ShowCheckBox || Checked)
			{
				if (e.Y < base.Height / 2)
				{
					is_up_pressed = true;
					is_down_pressed = false;
					IncrementSelectedPart(1);
				}
				else
				{
					is_up_pressed = false;
					is_down_pressed = true;
					IncrementSelectedPart(-1);
				}
				Invalidate(drop_down_arrow_rect);
				updown_timer.Interval = 500;
				updown_timer.Enabled = true;
			}
			return;
		}
		if (!is_drop_down_visible && drop_down_arrow_rect.Contains(e.X, e.Y))
		{
			DropDownButtonClicked();
			return;
		}
		if (is_drop_down_visible)
		{
			HideMonthCalendar();
		}
		if (ShowCheckBox && !Checked)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < part_data.Length; i++)
		{
			bool selected = part_data[i].Selected;
			if (!part_data[i].is_literal)
			{
				if (part_data[i].drawing_rectangle.Contains(e.X, e.Y))
				{
					part_data[i].Selected = true;
				}
				else
				{
					part_data[i].Selected = false;
				}
				if (selected != part_data[i].Selected)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			Invalidate();
			OnUIASelectionChanged();
		}
	}

	internal void DropDownButtonClicked()
	{
		if (!is_drop_down_visible)
		{
			is_drop_down_visible = true;
			if (!Checked)
			{
				Checked = true;
			}
			Invalidate(drop_down_arrow_rect);
			DropDownMonthCalendar();
		}
		else
		{
			HideMonthCalendar();
		}
	}

	private void PaintHandler(object sender, PaintEventArgs pe)
	{
		if (base.Width > 0 && base.Height > 0 && base.Visible)
		{
			Draw(pe.ClipRectangle, pe.Graphics);
		}
	}

	private void OnMouseEnter(object sender, EventArgs e)
	{
		if (ThemeEngine.Current.DateTimePickerBorderHasHotElementStyle)
		{
			Invalidate();
		}
	}

	private void OnMouseLeave(object sender, EventArgs e)
	{
		drop_down_button_entered = false;
		if (ThemeEngine.Current.DateTimePickerBorderHasHotElementStyle)
		{
			Invalidate();
		}
	}

	private void OnMouseMove(object sender, MouseEventArgs e)
	{
		if (!is_drop_down_visible && ThemeEngine.Current.DateTimePickerDropDownButtonHasHotElementStyle && drop_down_button_entered != drop_down_arrow_rect.Contains(e.Location))
		{
			drop_down_button_entered = !drop_down_button_entered;
			Invalidate(drop_down_arrow_rect);
		}
	}

	internal void OnUIAMinimumChanged()
	{
		((EventHandler)base.Events[UIAMinimumChanged])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAMaximumChanged()
	{
		((EventHandler)base.Events[UIAMaximumChanged])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIASelectionChanged()
	{
		((EventHandler)base.Events[UIASelectionChanged])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAChecked()
	{
		((EventHandler)base.Events[UIAChecked])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAShowCheckBoxChanged()
	{
		((EventHandler)base.Events[UIAShowCheckBoxChanged])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAShowUpDownChanged()
	{
		((EventHandler)base.Events[UIAShowUpDownChanged])?.Invoke(this, EventArgs.Empty);
	}
}
