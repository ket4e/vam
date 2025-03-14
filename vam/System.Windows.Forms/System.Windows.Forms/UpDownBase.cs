using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

[ComVisible(true)]
[Designer("System.Windows.Forms.Design.UpDownBaseDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public abstract class UpDownBase : ContainerControl
{
	internal sealed class UpDownSpinner : Control
	{
		private const int InitialRepeatDelay = 50;

		private UpDownBase owner;

		private Timer tmrRepeat;

		private Rectangle top_button_rect;

		private Rectangle bottom_button_rect;

		private int mouse_pressed;

		private int mouse_x;

		private int mouse_y;

		private int repeat_delay;

		private int repeat_counter;

		private bool top_button_entered;

		private bool bottom_button_entered;

		public UpDownSpinner(UpDownBase owner)
		{
			this.owner = owner;
			mouse_pressed = 0;
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.Opaque, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.FixedHeight, value: true);
			SetStyle(ControlStyles.Selectable, value: false);
			tmrRepeat = new Timer();
			tmrRepeat.Enabled = false;
			tmrRepeat.Interval = 10;
			tmrRepeat.Tick += tmrRepeat_Tick;
			compute_rects();
		}

		private void compute_rects()
		{
			int num = base.ClientSize.Height / 2;
			int height = base.ClientSize.Height - num;
			top_button_rect = new Rectangle(0, 0, base.ClientSize.Width, num);
			bottom_button_rect = new Rectangle(0, num, base.ClientSize.Width, height);
		}

		private void redraw(Graphics graphics)
		{
			PushButtonState state = PushButtonState.Normal;
			PushButtonState state2 = PushButtonState.Normal;
			if (owner.Enabled)
			{
				if (mouse_pressed != 0)
				{
					if (mouse_pressed == 1 && top_button_rect.Contains(mouse_x, mouse_y))
					{
						state = PushButtonState.Pressed;
					}
					if (mouse_pressed == 2 && bottom_button_rect.Contains(mouse_x, mouse_y))
					{
						state2 = PushButtonState.Pressed;
					}
				}
				else
				{
					if (top_button_entered)
					{
						state = PushButtonState.Hot;
					}
					if (bottom_button_entered)
					{
						state2 = PushButtonState.Hot;
					}
				}
			}
			else
			{
				state = PushButtonState.Disabled;
				state2 = PushButtonState.Disabled;
			}
			ThemeEngine.Current.UpDownBaseDrawButton(graphics, top_button_rect, top: true, state);
			ThemeEngine.Current.UpDownBaseDrawButton(graphics, bottom_button_rect, top: false, state2);
		}

		private void tmrRepeat_Tick(object sender, EventArgs e)
		{
			if (repeat_delay > 1)
			{
				repeat_counter++;
				if (repeat_counter < repeat_delay)
				{
					return;
				}
				repeat_counter = 0;
				repeat_delay = repeat_delay * 3 / 4;
			}
			if (mouse_pressed == 0)
			{
				tmrRepeat.Enabled = false;
			}
			if (mouse_pressed == 1 && top_button_rect.Contains(mouse_x, mouse_y))
			{
				owner.UpButton();
			}
			if (mouse_pressed == 2 && bottom_button_rect.Contains(mouse_x, mouse_y))
			{
				owner.DownButton();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (top_button_rect.Contains(e.X, e.Y))
				{
					mouse_pressed = 1;
					owner.UpButton();
				}
				else if (bottom_button_rect.Contains(e.X, e.Y))
				{
					mouse_pressed = 2;
					owner.DownButton();
				}
				mouse_x = e.X;
				mouse_y = e.Y;
				base.Capture = true;
				tmrRepeat.Enabled = true;
				repeat_counter = 0;
				repeat_delay = 50;
				Refresh();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			ButtonState buttonState = ButtonState.Normal;
			if (mouse_pressed == 1 && top_button_rect.Contains(mouse_x, mouse_y))
			{
				buttonState = ButtonState.Pushed;
			}
			if (mouse_pressed == 2 && bottom_button_rect.Contains(mouse_x, mouse_y))
			{
				buttonState = ButtonState.Pushed;
			}
			mouse_x = e.X;
			mouse_y = e.Y;
			ButtonState buttonState2 = ButtonState.Normal;
			if (mouse_pressed == 1 && top_button_rect.Contains(mouse_x, mouse_y))
			{
				buttonState2 = ButtonState.Pushed;
			}
			if (mouse_pressed == 2 && bottom_button_rect.Contains(mouse_x, mouse_y))
			{
				buttonState2 = ButtonState.Pushed;
			}
			bool flag = top_button_rect.Contains(e.Location);
			bool flag2 = bottom_button_rect.Contains(e.Location);
			if (buttonState != buttonState2)
			{
				if (buttonState2 == ButtonState.Pushed)
				{
					tmrRepeat.Enabled = true;
					repeat_counter = 0;
					repeat_delay = 50;
					if (mouse_pressed == 1)
					{
						owner.UpButton();
					}
					if (mouse_pressed == 2)
					{
						owner.DownButton();
					}
				}
				else
				{
					tmrRepeat.Enabled = false;
				}
				top_button_entered = flag;
				bottom_button_entered = flag2;
				Refresh();
			}
			else if (ThemeEngine.Current.UpDownBaseHasHotButtonStyle)
			{
				Region region = new Region();
				bool flag3 = false;
				region.MakeEmpty();
				if (top_button_entered != flag)
				{
					top_button_entered = flag;
					region.Union(top_button_rect);
					flag3 = true;
				}
				if (bottom_button_entered != flag2)
				{
					bottom_button_entered = flag2;
					region.Union(bottom_button_rect);
					flag3 = true;
				}
				if (flag3)
				{
					Invalidate(region);
				}
				region.Dispose();
			}
			else
			{
				top_button_entered = flag;
				bottom_button_entered = flag2;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouse_pressed = 0;
			base.Capture = false;
			Refresh();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				owner.UpButton();
			}
			else if (e.Delta < 0)
			{
				owner.DownButton();
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (top_button_entered)
			{
				top_button_entered = false;
				if (ThemeEngine.Current.UpDownBaseHasHotButtonStyle)
				{
					Invalidate(top_button_rect);
				}
			}
			if (bottom_button_entered)
			{
				bottom_button_entered = false;
				if (ThemeEngine.Current.UpDownBaseHasHotButtonStyle)
				{
					Invalidate(bottom_button_rect);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			redraw(e.Graphics);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			compute_rects();
		}
	}

	internal class UpDownTextBox : TextBox
	{
		private UpDownBase owner;

		public UpDownTextBox(UpDownBase owner)
		{
			this.owner = owner;
			SetStyle(ControlStyles.FixedWidth, value: false);
			SetStyle(ControlStyles.Selectable, value: false);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.ShowSelection = true;
			owner.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.ShowSelection = false;
			owner.OnLostFocus(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			owner.OnMouseDown(e);
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			owner.OnMouseUp(e);
			base.OnMouseUp(e);
		}
	}

	internal UpDownTextBox txtView;

	private UpDownSpinner spnSpinner;

	private bool _InterceptArrowKeys = true;

	private LeftRightAlignment _UpDownAlign;

	private bool changing_text;

	private bool user_edit;

	private static object UIAUpButtonClickEvent;

	private static object UIADownButtonClickEvent;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override bool AutoScroll
	{
		get
		{
			return base.AutoScroll;
		}
		set
		{
			base.AutoScroll = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Size AutoScrollMargin
	{
		get
		{
			return base.AutoScrollMargin;
		}
		set
		{
			base.AutoScrollMargin = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Size AutoScrollMinSize
	{
		get
		{
			return base.AutoScrollMinSize;
		}
		set
		{
			base.AutoScrollMinSize = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
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

	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
			txtView.BackColor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			base.BackgroundImage = value;
			txtView.BackgroundImage = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

	[DefaultValue(BorderStyle.Fixed3D)]
	[DispId(-504)]
	public BorderStyle BorderStyle
	{
		get
		{
			return base.InternalBorderStyle;
		}
		set
		{
			base.InternalBorderStyle = value;
		}
	}

	public override ContextMenu ContextMenu
	{
		get
		{
			return base.ContextMenu;
		}
		set
		{
			base.ContextMenu = value;
			txtView.ContextMenu = value;
			spnSpinner.ContextMenu = value;
		}
	}

	public override ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return base.ContextMenuStrip;
		}
		set
		{
			base.ContextMenuStrip = value;
			txtView.ContextMenuStrip = value;
			spnSpinner.ContextMenuStrip = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new DockPaddingEdges DockPadding => base.DockPadding;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override bool Focused => txtView.Focused;

	public override Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			base.ForeColor = value;
			txtView.ForeColor = value;
		}
	}

	[DefaultValue(true)]
	public bool InterceptArrowKeys
	{
		get
		{
			return _InterceptArrowKeys;
		}
		set
		{
			_InterceptArrowKeys = value;
		}
	}

	public override Size MaximumSize
	{
		get
		{
			return base.MaximumSize;
		}
		set
		{
			base.MaximumSize = new Size(value.Width, 0);
		}
	}

	public override Size MinimumSize
	{
		get
		{
			return base.MinimumSize;
		}
		set
		{
			base.MinimumSize = new Size(value.Width, 0);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int PreferredHeight
	{
		get
		{
			int height = Font.Height;
			switch (border_style)
			{
			case BorderStyle.FixedSingle:
			case BorderStyle.Fixed3D:
				height += 3;
				return height + 4;
			default:
				return height;
			}
		}
	}

	[DefaultValue(false)]
	public bool ReadOnly
	{
		get
		{
			return txtView.ReadOnly;
		}
		set
		{
			txtView.ReadOnly = value;
		}
	}

	[Localizable(true)]
	public override string Text
	{
		get
		{
			if (txtView != null)
			{
				return txtView.Text;
			}
			return string.Empty;
		}
		set
		{
			txtView.Text = value;
			if (UserEdit)
			{
				ValidateEditText();
			}
			txtView.SelectionLength = 0;
		}
	}

	[Localizable(true)]
	[DefaultValue(HorizontalAlignment.Left)]
	public HorizontalAlignment TextAlign
	{
		get
		{
			return txtView.TextAlign;
		}
		set
		{
			txtView.TextAlign = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(LeftRightAlignment.Right)]
	public LeftRightAlignment UpDownAlign
	{
		get
		{
			return _UpDownAlign;
		}
		set
		{
			if (_UpDownAlign != value)
			{
				_UpDownAlign = value;
				if (value == LeftRightAlignment.Left)
				{
					spnSpinner.Dock = DockStyle.Left;
				}
				else
				{
					spnSpinner.Dock = DockStyle.Right;
				}
			}
		}
	}

	protected bool ChangingText
	{
		get
		{
			return changing_text;
		}
		set
		{
			changing_text = value;
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => new Size(120, PreferredHeight);

	protected bool UserEdit
	{
		get
		{
			return user_edit;
		}
		set
		{
			user_edit = value;
		}
	}

	internal event EventHandler UIAUpButtonClick
	{
		add
		{
			base.Events.AddHandler(UIAUpButtonClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAUpButtonClickEvent, value);
		}
	}

	internal event EventHandler UIADownButtonClick
	{
		add
		{
			base.Events.AddHandler(UIADownButtonClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIADownButtonClickEvent, value);
		}
	}

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MouseEnter
	{
		add
		{
			base.MouseEnter += value;
		}
		remove
		{
			base.MouseEnter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MouseHover
	{
		add
		{
			base.MouseHover += value;
		}
		remove
		{
			base.MouseHover -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler MouseLeave
	{
		add
		{
			base.MouseLeave += value;
		}
		remove
		{
			base.MouseLeave -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	public UpDownBase()
	{
		_UpDownAlign = LeftRightAlignment.Right;
		base.InternalBorderStyle = BorderStyle.Fixed3D;
		spnSpinner = new UpDownSpinner(this);
		txtView = new UpDownTextBox(this);
		txtView.ModifiedChanged += OnChanged;
		txtView.AcceptsReturn = true;
		txtView.AutoSize = false;
		txtView.BorderStyle = BorderStyle.None;
		txtView.Location = new Point(17, 17);
		txtView.TabIndex = base.TabIndex;
		spnSpinner.Width = 16;
		spnSpinner.Dock = DockStyle.Right;
		txtView.Dock = DockStyle.Fill;
		SuspendLayout();
		base.Controls.Add(txtView);
		base.Controls.Add(spnSpinner);
		ResumeLayout();
		base.Height = PreferredHeight;
		base.BackColor = txtView.BackColor;
		base.TabIndexChanged += TabIndexChangedHandler;
		txtView.KeyDown += OnTextBoxKeyDown;
		txtView.KeyPress += OnTextBoxKeyPress;
		txtView.Resize += OnTextBoxResize;
		txtView.TextChanged += OnTextBoxTextChanged;
		auto_select_child = false;
		SetStyle(ControlStyles.FixedHeight, value: true);
		SetStyle(ControlStyles.Selectable, value: true);
		SetStyle(ControlStyles.Opaque | ControlStyles.ResizeRedraw, value: true);
		SetStyle(ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, value: false);
	}

	static UpDownBase()
	{
		UIAUpButtonClick = new object();
		UIADownButtonClick = new object();
	}

	internal void OnUIAUpButtonClick(EventArgs e)
	{
		((EventHandler)base.Events[UIAUpButtonClick])?.Invoke(this, e);
	}

	internal void OnUIADownButtonClick(EventArgs e)
	{
		((EventHandler)base.Events[UIADownButtonClick])?.Invoke(this, e);
	}

	private void TabIndexChangedHandler(object sender, EventArgs e)
	{
		txtView.TabIndex = base.TabIndex;
	}

	internal override void OnPaintInternal(PaintEventArgs e)
	{
		e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), base.ClientRectangle);
	}

	public abstract void DownButton();

	public void Select(int start, int length)
	{
		txtView.Select(start, length);
	}

	public abstract void UpButton();

	protected virtual void OnChanged(object source, EventArgs e)
	{
	}

	protected override void OnFontChanged(EventArgs e)
	{
		txtView.Font = Font;
		base.Height = PreferredHeight;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void OnLayout(LayoutEventArgs e)
	{
		base.OnLayout(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
	}

	protected override void OnMouseUp(MouseEventArgs mevent)
	{
		base.OnMouseUp(mevent);
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if (e.Delta > 0)
		{
			UpButton();
		}
		else if (e.Delta < 0)
		{
			DownButton();
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
	}

	protected virtual void OnTextBoxKeyDown(object source, KeyEventArgs e)
	{
		if (_InterceptArrowKeys && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
		{
			e.Handled = true;
			if (e.KeyCode == Keys.Up)
			{
				UpButton();
			}
			if (e.KeyCode == Keys.Down)
			{
				DownButton();
			}
		}
		OnKeyDown(e);
	}

	protected virtual void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\r')
		{
			e.Handled = true;
			ValidateEditText();
		}
		OnKeyPress(e);
	}

	protected virtual void OnTextBoxLostFocus(object source, EventArgs e)
	{
		if (UserEdit)
		{
			ValidateEditText();
		}
	}

	protected virtual void OnTextBoxResize(object source, EventArgs e)
	{
		base.Height = PreferredHeight;
	}

	protected virtual void OnTextBoxTextChanged(object source, EventArgs e)
	{
		if (changing_text)
		{
			ChangingText = false;
		}
		else
		{
			UserEdit = true;
		}
		OnTextChanged(e);
	}

	internal override void SetBoundsCoreInternal(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCoreInternal(x, y, width, Math.Min(width, PreferredHeight), specified);
	}

	protected abstract void UpdateEditText();

	protected virtual void ValidateEditText()
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_KEYDOWN:
		case Msg.WM_KEYUP:
		case Msg.WM_CHAR:
			XplatUI.SendMessage(txtView.Handle, (Msg)m.Msg, m.WParam, m.LParam);
			break;
		case Msg.WM_SETFOCUS:
			ActiveControl = txtView;
			break;
		case Msg.WM_KILLFOCUS:
			ActiveControl = null;
			break;
		default:
			base.WndProc(ref m);
			break;
		}
	}
}
