using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Allow)]
[DefaultEvent("Popup")]
[ProvideProperty("ToolTip", typeof(Control))]
public class ToolTip : Component, IExtenderProvider
{
	internal class ToolTipWindow : Control
	{
		private Control associated_control;

		internal Icon icon;

		internal string title = string.Empty;

		internal Rectangle icon_rect;

		internal Rectangle title_rect;

		internal Rectangle text_rect;

		private static object DrawEvent;

		private static object PopupEvent;

		private static object UnPopupEvent;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.Style = int.MinValue;
				createParams.Style |= 67108864;
				createParams.ExStyle = 136;
				return createParams;
			}
		}

		internal override bool ActivateOnShow => false;

		public event DrawToolTipEventHandler Draw
		{
			add
			{
				base.Events.AddHandler(DrawEvent, value);
			}
			remove
			{
				base.Events.RemoveHandler(DrawEvent, value);
			}
		}

		public event PopupEventHandler Popup
		{
			add
			{
				base.Events.AddHandler(PopupEvent, value);
			}
			remove
			{
				base.Events.RemoveHandler(PopupEvent, value);
			}
		}

		internal event PopupEventHandler UnPopup
		{
			add
			{
				base.Events.AddHandler(UnPopupEvent, value);
			}
			remove
			{
				base.Events.RemoveHandler(UnPopupEvent, value);
			}
		}

		internal ToolTipWindow()
		{
			base.Visible = false;
			base.Size = new Size(100, 20);
			ForeColor = ThemeEngine.Current.ColorInfoText;
			BackColor = ThemeEngine.Current.ColorInfo;
			base.VisibleChanged += ToolTipWindow_VisibleChanged;
			base.VisibleChanged += OnUIAToolTip_VisibleChanged;
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			if (ThemeEngine.Current.ToolTipTransparentBackground)
			{
				SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
				BackColor = Color.Transparent;
			}
			else
			{
				SetStyle(ControlStyles.Opaque, value: true);
			}
		}

		static ToolTipWindow()
		{
			Draw = new object();
			Popup = new object();
			UnPopup = new object();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			XplatUI.SetTopmost(window.Handle, Enabled: true);
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);
			OnDraw(new DrawToolTipEventArgs(pevent.Graphics, associated_control, associated_control, base.ClientRectangle, Text, BackColor, ForeColor, Font));
		}

		protected override void OnTextChanged(EventArgs args)
		{
			Invalidate();
			base.OnTextChanged(args);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 7 && m.WParam != IntPtr.Zero)
			{
				XplatUI.SetFocus(m.WParam);
			}
			base.WndProc(ref m);
		}

		internal virtual void OnDraw(DrawToolTipEventArgs e)
		{
			DrawToolTipEventHandler drawToolTipEventHandler = (DrawToolTipEventHandler)base.Events[Draw];
			if (drawToolTipEventHandler != null)
			{
				drawToolTipEventHandler(this, e);
			}
			else
			{
				ThemeEngine.Current.DrawToolTip(e.Graphics, e.Bounds, this);
			}
		}

		internal virtual void OnPopup(PopupEventArgs e)
		{
			PopupEventHandler popupEventHandler = (PopupEventHandler)base.Events[Popup];
			if (popupEventHandler != null)
			{
				popupEventHandler(this, e);
			}
			else
			{
				e.ToolTipSize = ThemeEngine.Current.ToolTipSize(this, Text);
			}
		}

		private void ToolTipWindow_VisibleChanged(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			if (control.is_visible)
			{
				XplatUI.SetTopmost(control.window.Handle, Enabled: true);
			}
			else
			{
				XplatUI.SetTopmost(control.window.Handle, Enabled: false);
			}
		}

		private void OnUIAToolTip_VisibleChanged(object sender, EventArgs e)
		{
			if (!base.Visible)
			{
				OnUnPopup(new PopupEventArgs(associated_control, associated_control, isBalloon: false, Size.Empty));
			}
		}

		private void OnUnPopup(PopupEventArgs e)
		{
			((PopupEventHandler)base.Events[UnPopup])?.Invoke(this, e);
		}

		public void PresentModal(Control control, string text)
		{
			if (!base.IsDisposed)
			{
				XplatUI.GetDisplaySize(out var _);
				associated_control = control;
				Text = text;
				PopupEventArgs popupEventArgs = new PopupEventArgs(control, control, isBalloon: false, Size.Empty);
				OnPopup(popupEventArgs);
				if (!popupEventArgs.Cancel)
				{
					base.Size = popupEventArgs.ToolTipSize;
					base.Visible = true;
				}
			}
		}

		public void Present(Control control, string text)
		{
			if (base.IsDisposed)
			{
				return;
			}
			XplatUI.GetDisplaySize(out var size);
			associated_control = control;
			Text = text;
			PopupEventArgs popupEventArgs = new PopupEventArgs(control, control, isBalloon: false, Size.Empty);
			OnPopup(popupEventArgs);
			if (!popupEventArgs.Cancel)
			{
				Size toolTipSize = popupEventArgs.ToolTipSize;
				base.Width = toolTipSize.Width;
				base.Height = toolTipSize.Height;
				XplatUI.GetCursorInfo(control.Cursor.Handle, out var _, out var height, out var _, out var hotspot_y);
				Point mousePosition = Control.MousePosition;
				mousePosition.Y += height - hotspot_y;
				if (mousePosition.X + base.Width > size.Width)
				{
					mousePosition.X = size.Width - base.Width;
				}
				if (mousePosition.Y + base.Height > size.Height)
				{
					mousePosition.Y = Control.MousePosition.Y - base.Height - hotspot_y;
				}
				base.Location = mousePosition;
				base.Visible = true;
			}
		}
	}

	private enum TipState
	{
		Initial,
		Show,
		Down
	}

	internal bool is_active;

	internal int automatic_delay;

	internal int autopop_delay;

	internal int initial_delay;

	internal int re_show_delay;

	internal bool show_always;

	internal Color back_color;

	internal Color fore_color;

	internal ToolTipWindow tooltip_window;

	internal Hashtable tooltip_strings;

	internal ArrayList controls;

	internal Control active_control;

	internal Control last_control;

	internal Timer timer;

	private Form hooked_form;

	private bool isBalloon;

	private bool owner_draw;

	private bool stripAmpersands;

	private ToolTipIcon tool_tip_icon;

	private bool useAnimation;

	private bool useFading;

	private object tag;

	private static object UnPopupEvent;

	private TipState state;

	private static object PopupEvent;

	private static object DrawEvent;

	internal Rectangle UIAToolTipRectangle => tooltip_window.Bounds;

	[DefaultValue(true)]
	public bool Active
	{
		get
		{
			return is_active;
		}
		set
		{
			if (is_active != value)
			{
				is_active = value;
				if (tooltip_window.Visible)
				{
					tooltip_window.Visible = false;
					active_control = null;
				}
			}
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[DefaultValue(500)]
	public int AutomaticDelay
	{
		get
		{
			return automatic_delay;
		}
		set
		{
			if (automatic_delay != value)
			{
				automatic_delay = value;
				autopop_delay = automatic_delay * 10;
				initial_delay = automatic_delay;
				re_show_delay = automatic_delay / 5;
			}
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	public int AutoPopDelay
	{
		get
		{
			return autopop_delay;
		}
		set
		{
			if (autopop_delay != value)
			{
				autopop_delay = value;
			}
		}
	}

	[DefaultValue("Color [Info]")]
	public Color BackColor
	{
		get
		{
			return back_color;
		}
		set
		{
			back_color = value;
			tooltip_window.BackColor = value;
		}
	}

	[DefaultValue("Color [InfoText]")]
	public Color ForeColor
	{
		get
		{
			return fore_color;
		}
		set
		{
			fore_color = value;
			tooltip_window.ForeColor = value;
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	public int InitialDelay
	{
		get
		{
			return initial_delay;
		}
		set
		{
			if (initial_delay != value)
			{
				initial_delay = value;
			}
		}
	}

	[DefaultValue(false)]
	public bool OwnerDraw
	{
		get
		{
			return owner_draw;
		}
		set
		{
			owner_draw = value;
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	public int ReshowDelay
	{
		get
		{
			return re_show_delay;
		}
		set
		{
			if (re_show_delay != value)
			{
				re_show_delay = value;
			}
		}
	}

	[DefaultValue(false)]
	public bool ShowAlways
	{
		get
		{
			return show_always;
		}
		set
		{
			if (show_always != value)
			{
				show_always = value;
			}
		}
	}

	[DefaultValue(false)]
	public bool IsBalloon
	{
		get
		{
			return isBalloon;
		}
		set
		{
			isBalloon = value;
		}
	}

	[DefaultValue(false)]
	[Browsable(true)]
	public bool StripAmpersands
	{
		get
		{
			return stripAmpersands;
		}
		set
		{
			stripAmpersands = value;
		}
	}

	[TypeConverter(typeof(StringConverter))]
	[Localizable(false)]
	[Bindable(true)]
	[DefaultValue(null)]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	[DefaultValue(ToolTipIcon.None)]
	public ToolTipIcon ToolTipIcon
	{
		get
		{
			return tool_tip_icon;
		}
		set
		{
			switch (value)
			{
			case ToolTipIcon.None:
				tooltip_window.icon = null;
				break;
			case ToolTipIcon.Error:
				tooltip_window.icon = SystemIcons.Error;
				break;
			case ToolTipIcon.Warning:
				tooltip_window.icon = SystemIcons.Warning;
				break;
			case ToolTipIcon.Info:
				tooltip_window.icon = SystemIcons.Information;
				break;
			}
			tool_tip_icon = value;
		}
	}

	[DefaultValue("")]
	public string ToolTipTitle
	{
		get
		{
			return tooltip_window.title;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			tooltip_window.title = value;
		}
	}

	[Browsable(true)]
	[DefaultValue(true)]
	public bool UseAnimation
	{
		get
		{
			return useAnimation;
		}
		set
		{
			useAnimation = value;
		}
	}

	[Browsable(true)]
	[DefaultValue(true)]
	public bool UseFading
	{
		get
		{
			return useFading;
		}
		set
		{
			useFading = value;
		}
	}

	protected virtual CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = new CreateParams();
			createParams.Style = 2;
			return createParams;
		}
	}

	internal bool Visible => tooltip_window.Visible;

	internal event PopupEventHandler UnPopup
	{
		add
		{
			base.Events.AddHandler(UnPopupEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UnPopupEvent, value);
		}
	}

	internal static event PopupEventHandler UIAUnPopup;

	internal static event ControlEventHandler UIAToolTipHookUp;

	internal static event ControlEventHandler UIAToolTipUnhookUp;

	public event PopupEventHandler Popup
	{
		add
		{
			base.Events.AddHandler(PopupEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PopupEvent, value);
		}
	}

	public event DrawToolTipEventHandler Draw
	{
		add
		{
			base.Events.AddHandler(DrawEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DrawEvent, value);
		}
	}

	public ToolTip()
	{
		is_active = true;
		automatic_delay = 500;
		autopop_delay = 5000;
		initial_delay = 500;
		re_show_delay = 100;
		show_always = false;
		back_color = SystemColors.Info;
		fore_color = SystemColors.InfoText;
		isBalloon = false;
		stripAmpersands = false;
		useAnimation = true;
		useFading = true;
		tooltip_strings = new Hashtable(5);
		controls = new ArrayList(5);
		tooltip_window = new ToolTipWindow();
		tooltip_window.MouseLeave += control_MouseLeave;
		tooltip_window.Draw += tooltip_window_Draw;
		tooltip_window.Popup += tooltip_window_Popup;
		tooltip_window.UnPopup += delegate(object sender, PopupEventArgs args)
		{
			OnUnPopup(args);
		};
		UnPopup += OnUIAUnPopup;
		timer = new Timer();
		timer.Enabled = false;
		timer.Tick += timer_Tick;
	}

	public ToolTip(IContainer cont)
		: this()
	{
		cont.Add(this);
	}

	static ToolTip()
	{
		UnPopup = new object();
		Popup = new object();
		Draw = new object();
	}

	internal static void OnUIAUnPopup(object sender, PopupEventArgs args)
	{
		if (ToolTip.UIAUnPopup != null)
		{
			ToolTip.UIAUnPopup(sender, args);
		}
	}

	internal static void OnUIAToolTipHookUp(object sender, ControlEventArgs args)
	{
		if (ToolTip.UIAToolTipHookUp != null)
		{
			ToolTip.UIAToolTipHookUp(sender, args);
		}
	}

	internal static void OnUIAToolTipUnhookUp(object sender, ControlEventArgs args)
	{
		if (ToolTip.UIAToolTipUnhookUp != null)
		{
			ToolTip.UIAToolTipUnhookUp(sender, args);
		}
	}

	~ToolTip()
	{
	}

	public bool CanExtend(object target)
	{
		return false;
	}

	[Localizable(true)]
	[DefaultValue("")]
	[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string GetToolTip(Control control)
	{
		string text = (string)tooltip_strings[control];
		if (text == null)
		{
			return string.Empty;
		}
		return text;
	}

	public void RemoveAll()
	{
		tooltip_strings.Clear();
		foreach (Control control in controls)
		{
			OnUIAToolTipUnhookUp(this, new ControlEventArgs(control));
		}
		controls.Clear();
	}

	public void SetToolTip(Control control, string caption)
	{
		OnUIAToolTipHookUp(this, new ControlEventArgs(control));
		tooltip_strings[control] = caption;
		if (!controls.Contains(control))
		{
			control.MouseEnter += control_MouseEnter;
			control.MouseMove += control_MouseMove;
			control.MouseLeave += control_MouseLeave;
			control.MouseDown += control_MouseDown;
			controls.Add(control);
		}
		if (active_control == control && caption != null && state == TipState.Show)
		{
			Size size = ThemeEngine.Current.ToolTipSize(tooltip_window, caption);
			tooltip_window.Width = size.Width;
			tooltip_window.Height = size.Height;
			tooltip_window.Text = caption;
			timer.Stop();
			timer.Start();
		}
		else if (control.IsHandleCreated && MouseInControl(control, fuzzy: false))
		{
			ShowTooltip(control);
		}
	}

	public override string ToString()
	{
		return base.ToString() + " InitialDelay: " + initial_delay + ", ShowAlways: " + show_always;
	}

	public void Show(string text, IWin32Window window)
	{
		Show(text, window, 0);
	}

	public void Show(string text, IWin32Window window, int duration)
	{
		if (window == null)
		{
			throw new ArgumentNullException("window");
		}
		if (duration < 0)
		{
			throw new ArgumentOutOfRangeException("duration", "duration cannot be less than zero");
		}
		if (Active)
		{
			timer.Stop();
			Control control = (Control)window;
			XplatUI.SetOwner(tooltip_window.Handle, control.TopLevelControl.Handle);
			if (control.ClientRectangle.Contains(control.PointToClient(Control.MousePosition)))
			{
				tooltip_window.Location = Control.MousePosition;
				tooltip_strings[control] = text;
				HookupControlEvents(control);
			}
			else
			{
				tooltip_window.Location = control.PointToScreen(new Point(control.Width / 2, control.Height / 2));
			}
			HookupFormEvents((Form)control.TopLevelControl);
			tooltip_window.PresentModal((Control)window, text);
			state = TipState.Show;
			if (duration > 0)
			{
				timer.Interval = duration;
				timer.Start();
			}
		}
	}

	public void Show(string text, IWin32Window window, Point point)
	{
		Show(text, window, point, 0);
	}

	public void Show(string text, IWin32Window window, int x, int y)
	{
		Show(text, window, new Point(x, y), 0);
	}

	public void Show(string text, IWin32Window window, Point point, int duration)
	{
		if (window == null)
		{
			throw new ArgumentNullException("window");
		}
		if (duration < 0)
		{
			throw new ArgumentOutOfRangeException("duration", "duration cannot be less than zero");
		}
		if (Active)
		{
			timer.Stop();
			Control control = (Control)window;
			Point location = control.PointToScreen(Point.Empty);
			location.X += point.X;
			location.Y += point.Y;
			XplatUI.SetOwner(tooltip_window.Handle, control.TopLevelControl.Handle);
			HookupFormEvents((Form)control.TopLevelControl);
			tooltip_window.Location = location;
			tooltip_window.PresentModal((Control)window, text);
			state = TipState.Show;
			if (duration > 0)
			{
				timer.Interval = duration;
				timer.Start();
			}
		}
	}

	public void Show(string text, IWin32Window window, int x, int y, int duration)
	{
		Show(text, window, new Point(x, y), duration);
	}

	public void Hide(IWin32Window win)
	{
		timer.Stop();
		state = TipState.Initial;
		UnhookFormEvents();
		tooltip_window.Visible = false;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (!disposing)
		{
			return;
		}
		timer.Stop();
		timer.Dispose();
		tooltip_window.Dispose();
		tooltip_strings.Clear();
		foreach (Control control in controls)
		{
			OnUIAToolTipUnhookUp(this, new ControlEventArgs(control));
		}
		controls.Clear();
	}

	protected void StopTimer()
	{
		timer.Stop();
	}

	private void HookupFormEvents(Form form)
	{
		hooked_form = form;
		form.Deactivate += Form_Deactivate;
		form.Closed += Form_Closed;
		form.Resize += Form_Resize;
	}

	private void HookupControlEvents(Control control)
	{
		if (!controls.Contains(control))
		{
			control.MouseEnter += control_MouseEnter;
			control.MouseMove += control_MouseMove;
			control.MouseLeave += control_MouseLeave;
			control.MouseDown += control_MouseDown;
			controls.Add(control);
		}
	}

	private void UnhookControlEvents(Control control)
	{
		control.MouseEnter -= control_MouseEnter;
		control.MouseMove -= control_MouseMove;
		control.MouseLeave -= control_MouseLeave;
		control.MouseDown -= control_MouseDown;
	}

	private void UnhookFormEvents()
	{
		if (hooked_form != null)
		{
			hooked_form.Deactivate -= Form_Deactivate;
			hooked_form.Closed -= Form_Closed;
			hooked_form.Resize -= Form_Resize;
			hooked_form = null;
		}
	}

	private void Form_Resize(object sender, EventArgs e)
	{
		Form form = (Form)sender;
		if (form.WindowState == FormWindowState.Minimized)
		{
			tooltip_window.Visible = false;
		}
	}

	private void Form_Closed(object sender, EventArgs e)
	{
		tooltip_window.Visible = false;
	}

	private void Form_Deactivate(object sender, EventArgs e)
	{
		tooltip_window.Visible = false;
	}

	internal void Present(Control control, string text)
	{
		tooltip_window.Present(control, text);
	}

	private void control_MouseEnter(object sender, EventArgs e)
	{
		ShowTooltip(sender as Control);
	}

	private void ShowTooltip(Control control)
	{
		last_control = control;
		tooltip_window.Visible = false;
		timer.Stop();
		state = TipState.Initial;
		if (!is_active || (!show_always && control.FindForm() != Form.ActiveForm))
		{
			return;
		}
		string text = (string)tooltip_strings[control];
		if (text != null && text.Length > 0)
		{
			if (active_control == null)
			{
				timer.Interval = Math.Max(initial_delay, 1);
			}
			else
			{
				timer.Interval = Math.Max(re_show_delay, 1);
			}
			active_control = control;
			timer.Start();
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		timer.Stop();
		switch (state)
		{
		case TipState.Initial:
			if (active_control != null)
			{
				tooltip_window.Present(active_control, (string)tooltip_strings[active_control]);
				state = TipState.Show;
				timer.Interval = autopop_delay;
				timer.Start();
			}
			break;
		case TipState.Show:
			tooltip_window.Visible = false;
			state = TipState.Down;
			break;
		default:
			throw new Exception("Timer shouldn't be running in state: " + state);
		}
	}

	private void tooltip_window_Popup(object sender, PopupEventArgs e)
	{
		e.ToolTipSize = ThemeEngine.Current.ToolTipSize(tooltip_window, tooltip_window.Text);
		OnPopup(e);
	}

	private void tooltip_window_Draw(object sender, DrawToolTipEventArgs e)
	{
		if (OwnerDraw)
		{
			OnDraw(e);
		}
		else
		{
			ThemeEngine.Current.DrawToolTip(e.Graphics, e.Bounds, tooltip_window);
		}
	}

	private bool MouseInControl(Control control, bool fuzzy)
	{
		if (control == null)
		{
			return false;
		}
		Point mousePosition = Control.MousePosition;
		Point point = new Point(control.Bounds.X, control.Bounds.Y);
		if (control.Parent != null)
		{
			point = control.Parent.PointToScreen(point);
		}
		Size clientSize = control.ClientSize;
		Rectangle rectangle = new Rectangle(point, clientSize);
		if (fuzzy)
		{
			rectangle.Inflate(2, 2);
		}
		return rectangle.Contains(mousePosition);
	}

	private void control_MouseLeave(object sender, EventArgs e)
	{
		timer.Stop();
		active_control = null;
		tooltip_window.Visible = false;
		if (last_control == sender)
		{
			last_control = null;
		}
	}

	private void control_MouseDown(object sender, MouseEventArgs e)
	{
		timer.Stop();
		active_control = null;
		tooltip_window.Visible = false;
		if (last_control == sender)
		{
			last_control = null;
		}
	}

	private void control_MouseMove(object sender, MouseEventArgs e)
	{
		if (state != TipState.Down)
		{
			timer.Stop();
			timer.Start();
		}
	}

	internal void OnDraw(DrawToolTipEventArgs e)
	{
		((DrawToolTipEventHandler)base.Events[Draw])?.Invoke(this, e);
	}

	internal void OnPopup(PopupEventArgs e)
	{
		((PopupEventHandler)base.Events[Popup])?.Invoke(this, e);
	}

	internal void OnUnPopup(PopupEventArgs e)
	{
		((PopupEventHandler)base.Events[UnPopup])?.Invoke(this, e);
	}
}
