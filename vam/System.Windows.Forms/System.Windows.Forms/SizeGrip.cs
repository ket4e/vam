using System.Drawing;

namespace System.Windows.Forms;

internal class SizeGrip : Control
{
	private Point capture_point;

	private Control captured_control;

	private int window_w;

	private int window_h;

	private bool hide_pending;

	private bool captured;

	private bool is_virtual;

	private bool enabled;

	private bool fill_background;

	private Rectangle last_painted_area;

	public bool FillBackground
	{
		get
		{
			return fill_background;
		}
		set
		{
			fill_background = value;
		}
	}

	public bool Virtual
	{
		get
		{
			return is_virtual;
		}
		set
		{
			if (is_virtual != value)
			{
				is_virtual = value;
				if (is_virtual)
				{
					CapturedControl.MouseMove += HandleMouseMove;
					CapturedControl.MouseUp += HandleMouseUp;
					CapturedControl.MouseDown += HandleMouseDown;
					CapturedControl.EnabledChanged += HandleEnabledChanged;
					CapturedControl.Resize += HandleResize;
				}
				else
				{
					CapturedControl.MouseMove -= HandleMouseMove;
					CapturedControl.MouseUp -= HandleMouseUp;
					CapturedControl.MouseDown -= HandleMouseDown;
					CapturedControl.EnabledChanged -= HandleEnabledChanged;
					CapturedControl.Resize -= HandleResize;
				}
			}
		}
	}

	public Control CapturedControl
	{
		get
		{
			return captured_control;
		}
		set
		{
			captured_control = value;
		}
	}

	public SizeGrip(Control CapturedControl)
	{
		Cursor = Cursors.SizeNWSE;
		enabled = true;
		fill_background = true;
		base.Size = GetDefaultSize();
		this.CapturedControl = CapturedControl;
	}

	internal static Size GetDefaultSize()
	{
		return new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
	}

	internal static Rectangle GetDefaultRectangle(Control Parent)
	{
		Size defaultSize = GetDefaultSize();
		return new Rectangle(Parent.ClientSize.Width - defaultSize.Width, Parent.ClientSize.Height - defaultSize.Height, defaultSize.Width, defaultSize.Height);
	}

	private void HandleResize(object sender, EventArgs e)
	{
		Control control = (Control)sender;
		control.Invalidate(last_painted_area);
	}

	private void HandleEnabledChanged(object sender, EventArgs e)
	{
		Control control = (Control)sender;
		enabled = control.Enabled;
		Cursor cursor = ((!enabled) ? Cursors.Default : Cursors.SizeNWSE);
		if (is_virtual)
		{
			if (CapturedControl != null)
			{
				CapturedControl.Cursor = cursor;
			}
		}
		else
		{
			Cursor = cursor;
		}
		control.Invalidate(GetDefaultRectangle(control));
	}

	internal void HandlePaint(object sender, PaintEventArgs e)
	{
		if (base.Visible)
		{
			Control control = (Control)sender;
			Graphics graphics = e.Graphics;
			Rectangle defaultRectangle = GetDefaultRectangle(control);
			if (!is_virtual || fill_background)
			{
				graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ThemeEngine.Current.ColorControl), defaultRectangle);
			}
			if (enabled)
			{
				ControlPaint.DrawSizeGrip(graphics, BackColor, defaultRectangle);
			}
			last_painted_area = defaultRectangle;
		}
	}

	private void HandleMouseCaptureChanged(object sender, EventArgs e)
	{
		Control control = (Control)sender;
		if (captured && !control.Capture)
		{
			captured = false;
			CapturedControl.Size = new Size(window_w, window_h);
		}
	}

	internal void HandleMouseDown(object sender, MouseEventArgs e)
	{
		if (enabled)
		{
			Control control = (Control)sender;
			if (GetDefaultRectangle(control).Contains(e.X, e.Y))
			{
				control.Capture = true;
				captured = true;
				capture_point = Control.MousePosition;
				window_w = CapturedControl.Width;
				window_h = CapturedControl.Height;
			}
		}
	}

	internal void HandleMouseMove(object sender, MouseEventArgs e)
	{
		Control control = (Control)sender;
		if (GetDefaultRectangle(control).Contains(e.X, e.Y))
		{
			control.Cursor = Cursors.SizeNWSE;
		}
		else
		{
			control.Cursor = Cursors.Default;
		}
		if (captured)
		{
			Point mousePosition = Control.MousePosition;
			int num = mousePosition.X - capture_point.X;
			int num2 = mousePosition.Y - capture_point.Y;
			Control capturedControl = CapturedControl;
			Form form = capturedControl as Form;
			Size size = new Size(window_w + num, window_h + num2);
			Size size2 = form?.MaximumSize ?? Size.Empty;
			Size size3 = form?.MinimumSize ?? Size.Empty;
			if (size.Width > size2.Width && size2.Width > 0)
			{
				size.Width = size2.Width;
			}
			else if (size.Width < size3.Width)
			{
				size.Width = size3.Width;
			}
			if (size.Height > size2.Height && size2.Height > 0)
			{
				size.Height = size2.Height;
			}
			else if (size.Height < size3.Height)
			{
				size.Height = size3.Height;
			}
			if (size != capturedControl.Size)
			{
				capturedControl.Size = size;
			}
		}
	}

	internal void HandleMouseUp(object sender, MouseEventArgs e)
	{
		if (captured)
		{
			Control control = (Control)sender;
			captured = false;
			control.Capture = false;
			control.Invalidate(last_painted_area);
			if (base.Parent is ScrollableControl)
			{
				((ScrollableControl)base.Parent).UpdateSizeGripVisible();
			}
			if (hide_pending)
			{
				Hide();
				hide_pending = false;
			}
		}
	}

	protected override void SetVisibleCore(bool value)
	{
		if (base.Capture)
		{
			if (!value)
			{
				hide_pending = true;
			}
			else
			{
				hide_pending = false;
			}
		}
		else
		{
			base.SetVisibleCore(value);
		}
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		HandlePaint(this, pe);
		base.OnPaint(pe);
	}

	protected override void OnMouseCaptureChanged(EventArgs e)
	{
		base.OnMouseCaptureChanged(e);
		HandleMouseCaptureChanged(this, e);
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
		HandleEnabledChanged(this, e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		HandleMouseDown(this, e);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		HandleMouseMove(this, e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		HandleMouseUp(this, e);
	}
}
