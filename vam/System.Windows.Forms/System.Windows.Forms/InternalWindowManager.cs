using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

internal abstract class InternalWindowManager
{
	public enum State
	{
		Idle,
		Moving,
		Sizing
	}

	[Flags]
	public enum FormPos
	{
		None = 0,
		TitleBar = 1,
		Top = 2,
		Left = 4,
		Right = 8,
		Bottom = 0x10,
		TopLeft = 6,
		TopRight = 0xA,
		BottomLeft = 0x14,
		BottomRight = 0x18,
		AnyEdge = 0x1E
	}

	private TitleButtons title_buttons;

	internal Form form;

	internal Point start;

	internal State state;

	protected Point clicked_point;

	private FormPos sizing_edge;

	internal Rectangle virtual_position;

	private Rectangle normal_bounds;

	private Rectangle iconic_bounds;

	public Form Form => form;

	public int IconWidth => TitleBarHeight - 5;

	public TitleButtons TitleButtons => title_buttons;

	internal Rectangle NormalBounds
	{
		get
		{
			return normal_bounds;
		}
		set
		{
			normal_bounds = value;
		}
	}

	internal Size IconicSize => SystemInformation.MinimizedWindowSize;

	internal Rectangle IconicBounds
	{
		get
		{
			if (iconic_bounds == Rectangle.Empty)
			{
				return Rectangle.Empty;
			}
			Rectangle result = iconic_bounds;
			result.Y = Form.Parent.ClientRectangle.Bottom - iconic_bounds.Y;
			return result;
		}
		set
		{
			iconic_bounds = value;
			iconic_bounds.Y = Form.Parent.ClientRectangle.Bottom - iconic_bounds.Y;
		}
	}

	internal virtual Rectangle MaximizedBounds => Form.Parent.ClientRectangle;

	public bool ShowIcon
	{
		get
		{
			if (!Form.ShowIcon)
			{
				return false;
			}
			if (!HasBorders)
			{
				return false;
			}
			if (IsMinimized)
			{
				return true;
			}
			if (IsToolWindow || Form.FormBorderStyle == FormBorderStyle.FixedDialog)
			{
				return false;
			}
			return true;
		}
	}

	public virtual bool IsActive => true;

	public bool IsMaximized => GetWindowState() == FormWindowState.Maximized;

	public bool IsMinimized => GetWindowState() == FormWindowState.Minimized;

	public bool IsSizable
	{
		get
		{
			switch (form.FormBorderStyle)
			{
			case FormBorderStyle.Sizable:
			case FormBorderStyle.SizableToolWindow:
				return form.window_state != FormWindowState.Minimized;
			default:
				return false;
			}
		}
	}

	public bool HasBorders => form.FormBorderStyle != FormBorderStyle.None;

	public bool IsToolWindow
	{
		get
		{
			if (form.FormBorderStyle == FormBorderStyle.SizableToolWindow || form.FormBorderStyle == FormBorderStyle.FixedToolWindow || form.GetCreateParams().IsSet(WindowExStyles.WS_EX_TOOLWINDOW))
			{
				return true;
			}
			return false;
		}
	}

	public int TitleBarHeight => ThemeEngine.Current.ManagedWindowTitleBarHeight(this);

	public int BorderWidth => ThemeEngine.Current.ManagedWindowBorderWidth(this);

	public virtual int MenuHeight => (form.Menu != null) ? ThemeEngine.Current.MenuHeight : 0;

	public InternalWindowManager(Form form)
	{
		this.form = form;
		form.SizeChanged += FormSizeChangedHandler;
		title_buttons = new TitleButtons(form);
		ThemeEngine.Current.ManagedWindowSetButtonLocations(this);
	}

	public virtual void UpdateWindowState(FormWindowState old_window_state, FormWindowState new_window_state, bool force)
	{
		switch (old_window_state)
		{
		case FormWindowState.Normal:
			NormalBounds = form.Bounds;
			break;
		case FormWindowState.Minimized:
			IconicBounds = form.Bounds;
			break;
		}
		switch (new_window_state)
		{
		case FormWindowState.Minimized:
			if (IconicBounds == Rectangle.Empty)
			{
				Size iconicSize = IconicSize;
				Point location = new Point(0, Form.Parent.ClientSize.Height - iconicSize.Height);
				IconicBounds = new Rectangle(location, iconicSize);
			}
			form.Bounds = IconicBounds;
			break;
		case FormWindowState.Maximized:
			form.Bounds = MaximizedBounds;
			break;
		case FormWindowState.Normal:
			form.Bounds = NormalBounds;
			break;
		}
		UpdateWindowDecorations(new_window_state);
		form.ResetCursor();
	}

	public virtual void UpdateWindowDecorations(FormWindowState window_state)
	{
		ThemeEngine.Current.ManagedWindowSetButtonLocations(this);
		if (form.IsHandleCreated)
		{
			XplatUI.RequestNCRecalc(form.Handle);
		}
	}

	public virtual bool WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_MOUSEMOVE:
			return HandleMouseMove(form, ref m);
		case Msg.WM_LBUTTONUP:
			HandleLButtonUp(ref m);
			break;
		case Msg.WM_RBUTTONDOWN:
			return HandleRButtonDown(ref m);
		case Msg.WM_LBUTTONDOWN:
			return HandleLButtonDown(ref m);
		case Msg.WM_LBUTTONDBLCLK:
			return HandleLButtonDblClick(ref m);
		case Msg.WM_PARENTNOTIFY:
			if (Control.LowOrder(m.WParam.ToInt32()) == 513)
			{
				Activate();
			}
			break;
		case Msg.WM_NCHITTEST:
			return HandleNCHitTest(ref m);
		case Msg.WM_NCLBUTTONUP:
			HandleNCLButtonUp(ref m);
			return true;
		case Msg.WM_NCLBUTTONDOWN:
			HandleNCLButtonDown(ref m);
			return true;
		case Msg.WM_NCMOUSEMOVE:
			HandleNCMouseMove(ref m);
			return true;
		case Msg.WM_NCLBUTTONDBLCLK:
			HandleNCLButtonDblClick(ref m);
			break;
		case Msg.WM_NCMOUSELEAVE:
			HandleNCMouseLeave(ref m);
			break;
		case Msg.WM_MOUSELEAVE:
			HandleMouseLeave(ref m);
			break;
		case Msg.WM_NCCALCSIZE:
			return HandleNCCalcSize(ref m);
		case Msg.WM_NCPAINT:
			return HandleNCPaint(ref m);
		}
		return false;
	}

	protected virtual bool HandleNCPaint(ref Message m)
	{
		PaintEventArgs paintEventArgs = XplatUI.PaintEventStart(ref m, form.Handle, client: false);
		if (form.ActiveMenu != null)
		{
			Point menuOrigin = GetMenuOrigin();
			Rectangle a = new Rectangle(menuOrigin.X, menuOrigin.Y, form.ClientSize.Width, 0);
			a = Rectangle.Union(a, paintEventArgs.ClipRectangle);
			paintEventArgs.SetClip(a);
			paintEventArgs.Graphics.SetClip(a);
			form.ActiveMenu.Draw(paintEventArgs, new Rectangle(menuOrigin.X, menuOrigin.Y, form.ClientSize.Width, 0));
		}
		if (HasBorders || (IsMinimized && (!Form.IsMdiChild || !IsMaximized)))
		{
			Rectangle a = new Rectangle(0, 0, form.Width, form.Height);
			ThemeEngine.Current.DrawManagedWindowDecorations(paintEventArgs.Graphics, a, this);
		}
		XplatUI.PaintEventEnd(ref m, form.Handle, client: false);
		return true;
	}

	protected virtual bool HandleNCCalcSize(ref Message m)
	{
		if (m.WParam == (IntPtr)1)
		{
			XplatUIWin32.NCCALCSIZE_PARAMS nCCALCSIZE_PARAMS = (XplatUIWin32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(XplatUIWin32.NCCALCSIZE_PARAMS));
			nCCALCSIZE_PARAMS.rgrc1 = NCCalcSize(nCCALCSIZE_PARAMS.rgrc1);
			Marshal.StructureToPtr(nCCALCSIZE_PARAMS, m.LParam, fDeleteOld: true);
		}
		else
		{
			XplatUIWin32.RECT proposed_window_rect = (XplatUIWin32.RECT)Marshal.PtrToStructure(m.LParam, typeof(XplatUIWin32.RECT));
			proposed_window_rect = NCCalcSize(proposed_window_rect);
			Marshal.StructureToPtr(proposed_window_rect, m.LParam, fDeleteOld: true);
		}
		return true;
	}

	protected virtual XplatUIWin32.RECT NCCalcSize(XplatUIWin32.RECT proposed_window_rect)
	{
		int num = ThemeEngine.Current.ManagedWindowBorderWidth(this);
		if (HasBorders)
		{
			proposed_window_rect.top += TitleBarHeight + num;
			proposed_window_rect.bottom -= num;
			proposed_window_rect.left += num;
			proposed_window_rect.right -= num;
		}
		if (XplatUI.RequiresPositiveClientAreaSize)
		{
			if (proposed_window_rect.right <= proposed_window_rect.left)
			{
				proposed_window_rect.right += proposed_window_rect.left - proposed_window_rect.right + 1;
			}
			if (proposed_window_rect.top >= proposed_window_rect.bottom)
			{
				proposed_window_rect.bottom += proposed_window_rect.top - proposed_window_rect.bottom + 1;
			}
		}
		return proposed_window_rect;
	}

	protected virtual bool HandleNCHitTest(ref Message m)
	{
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCPointToClient(ref x, ref y);
		FormPos formPos = FormPosForCoords(x, y);
		if (formPos == FormPos.TitleBar)
		{
			m.Result = new IntPtr(2);
			return true;
		}
		if (!IsSizable)
		{
			return false;
		}
		switch (formPos)
		{
		case FormPos.Top:
			m.Result = new IntPtr(12);
			break;
		case FormPos.Left:
			m.Result = new IntPtr(10);
			break;
		case FormPos.Right:
			m.Result = new IntPtr(11);
			break;
		case FormPos.Bottom:
			m.Result = new IntPtr(15);
			break;
		case FormPos.TopLeft:
			m.Result = new IntPtr(13);
			break;
		case FormPos.TopRight:
			m.Result = new IntPtr(14);
			break;
		case FormPos.BottomLeft:
			m.Result = new IntPtr(16);
			break;
		case FormPos.BottomRight:
			m.Result = new IntPtr(17);
			break;
		default:
			return false;
		}
		return true;
	}

	public virtual void UpdateBorderStyle(FormBorderStyle border_style)
	{
		if (form.IsHandleCreated)
		{
			XplatUI.SetBorderStyle(form.Handle, border_style);
		}
		if (ShouldRemoveWindowManager(border_style))
		{
			form.RemoveWindowManager();
		}
		else
		{
			ThemeEngine.Current.ManagedWindowSetButtonLocations(this);
		}
	}

	public virtual void SetWindowState(FormWindowState old_state, FormWindowState window_state)
	{
		UpdateWindowState(old_state, window_state, force: false);
	}

	public virtual FormWindowState GetWindowState()
	{
		return form.window_state;
	}

	public virtual void PointToClient(ref int x, ref int y)
	{
		Rectangle workingArea = SystemInformation.WorkingArea;
		if (x > workingArea.Right)
		{
			x = workingArea.Right;
		}
		if (x < workingArea.Left)
		{
			x = workingArea.Left;
		}
		if (y < workingArea.Top)
		{
			y = workingArea.Top;
		}
		if (y > workingArea.Bottom)
		{
			y = workingArea.Bottom;
		}
	}

	public virtual void PointToScreen(ref int x, ref int y)
	{
		XplatUI.ClientToScreen(form.Handle, ref x, ref y);
	}

	protected virtual bool ShouldRemoveWindowManager(FormBorderStyle style)
	{
		return style != FormBorderStyle.FixedToolWindow && style != FormBorderStyle.SizableToolWindow;
	}

	public bool IconRectangleContains(int x, int y)
	{
		if (!ShowIcon)
		{
			return false;
		}
		return ThemeEngine.Current.ManagedWindowGetTitleBarIconArea(this).Contains(x, y);
	}

	protected virtual void Activate()
	{
		form.Invalidate(invalidateChildren: true);
		form.Update();
	}

	private void FormSizeChangedHandler(object sender, EventArgs e)
	{
		if (form.IsHandleCreated)
		{
			ThemeEngine.Current.ManagedWindowSetButtonLocations(this);
			XplatUI.InvalidateNC(form.Handle);
		}
	}

	protected virtual bool HandleRButtonDown(ref Message m)
	{
		Activate();
		return false;
	}

	protected virtual bool HandleLButtonDown(ref Message m)
	{
		Activate();
		return false;
	}

	protected virtual bool HandleLButtonDblClick(ref Message m)
	{
		return false;
	}

	protected virtual bool HandleNCMouseLeave(ref Message m)
	{
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCPointToClient(ref x, ref y);
		FormPos formPos = FormPosForCoords(x, y);
		if (formPos != FormPos.TitleBar)
		{
			HandleTitleBarLeave(x, y);
			return true;
		}
		return true;
	}

	protected virtual bool HandleNCMouseMove(ref Message m)
	{
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCPointToClient(ref x, ref y);
		FormPos formPos = FormPosForCoords(x, y);
		if (formPos == FormPos.TitleBar)
		{
			HandleTitleBarMouseMove(x, y);
			return true;
		}
		if (form.ActiveMenu != null && XplatUI.IsEnabled(form.Handle))
		{
			MouseEventArgs e = new MouseEventArgs(Control.FromParamToMouseButtons(m.WParam.ToInt32()), form.mouse_clicks, x, y, 0);
			form.ActiveMenu.OnMouseMove(form, e);
		}
		return true;
	}

	protected virtual bool HandleNCLButtonDown(ref Message m)
	{
		Activate();
		start = Cursor.Position;
		virtual_position = form.Bounds;
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCPointToClient(ref x, ref y);
		FormPos formPos = FormPosForCoords(x, y);
		if (form.ActiveMenu != null && XplatUI.IsEnabled(form.Handle))
		{
			MouseEventArgs args = new MouseEventArgs(Control.FromParamToMouseButtons(m.WParam.ToInt32()), form.mouse_clicks, x, y - TitleBarHeight, 0);
			form.ActiveMenu.OnMouseDown(form, args);
		}
		if (formPos == FormPos.TitleBar)
		{
			HandleTitleBarDown(x, y);
			return true;
		}
		if (IsSizable)
		{
			if ((formPos & FormPos.AnyEdge) == 0)
			{
				return false;
			}
			virtual_position = form.Bounds;
			state = State.Sizing;
			sizing_edge = formPos;
			form.Capture = true;
			return true;
		}
		return false;
	}

	protected virtual void HandleNCLButtonDblClick(ref Message m)
	{
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCPointToClient(ref x, ref y);
		FormPos formPos = FormPosForCoords(x, y);
		if (formPos == FormPos.TitleBar || formPos == FormPos.Top)
		{
			HandleTitleBarDoubleClick(x, y);
		}
	}

	protected virtual void HandleTitleBarDoubleClick(int x, int y)
	{
	}

	protected virtual void HandleTitleBarLeave(int x, int y)
	{
		title_buttons.MouseLeave(x, y);
	}

	protected virtual void HandleTitleBarMouseMove(int x, int y)
	{
		if (title_buttons.MouseMove(x, y))
		{
			XplatUI.InvalidateNC(form.Handle);
		}
	}

	protected virtual void HandleTitleBarUp(int x, int y)
	{
		title_buttons.MouseUp(x, y);
	}

	protected virtual void HandleTitleBarDown(int x, int y)
	{
		title_buttons.MouseDown(x, y);
		if (!TitleButtons.AnyPushedTitleButtons && !IsMaximized)
		{
			state = State.Moving;
			clicked_point = new Point(x, y);
			if (form.Parent != null)
			{
				form.CaptureWithConfine(form.Parent);
			}
			else
			{
				form.Capture = true;
			}
		}
		XplatUI.InvalidateNC(form.Handle);
	}

	private bool HandleMouseMove(Form form, ref Message m)
	{
		switch (state)
		{
		case State.Moving:
			HandleWindowMove(m);
			return true;
		case State.Sizing:
			HandleSizing(m);
			return true;
		default:
			return false;
		}
	}

	private void HandleMouseLeave(ref Message m)
	{
		form.ResetCursor();
	}

	protected virtual void HandleWindowMove(Message m)
	{
		Point point = MouseMove(Cursor.Position);
		UpdateVP(virtual_position.X + point.X, virtual_position.Y + point.Y, virtual_position.Width, virtual_position.Height);
	}

	private void HandleSizing(Message m)
	{
		Rectangle r = virtual_position;
		int num;
		int num2;
		if (IsToolWindow)
		{
			int borderWidth = BorderWidth;
			num = 2 * (borderWidth + 2) + ThemeEngine.Current.ManagedWindowButtonSize(this).Width;
			num2 = 2 * borderWidth + TitleBarHeight;
		}
		else
		{
			Size minWindowTrackSize = SystemInformation.MinWindowTrackSize;
			num = minWindowTrackSize.Width;
			num2 = minWindowTrackSize.Height;
		}
		int x = Cursor.Position.X;
		int y = Cursor.Position.Y;
		PointToClient(ref x, ref y);
		if ((sizing_edge & FormPos.Top) != 0)
		{
			if (r.Bottom - y < num2)
			{
				y = r.Bottom - num2;
			}
			r.Height = r.Bottom - y;
			r.Y = y;
		}
		else if ((sizing_edge & FormPos.Bottom) != 0)
		{
			int num3 = y - r.Top;
			if (num3 <= num2)
			{
				num3 = num2;
			}
			r.Height = num3;
		}
		if ((sizing_edge & FormPos.Left) != 0)
		{
			if (r.Right - x < num)
			{
				x = r.Right - num;
			}
			r.Width = r.Right - x;
			r.X = x;
		}
		else if ((sizing_edge & FormPos.Right) != 0)
		{
			int num4 = x - form.Left;
			if (num4 <= num)
			{
				num4 = num;
			}
			r.Width = num4;
		}
		UpdateVP(r);
	}

	protected void UpdateVP(Rectangle r)
	{
		UpdateVP(r.X, r.Y, r.Width, r.Height);
	}

	protected void UpdateVP(Point loc, int w, int h)
	{
		UpdateVP(loc.X, loc.Y, w, h);
	}

	protected void UpdateVP(int x, int y, int w, int h)
	{
		virtual_position.X = x;
		virtual_position.Y = y;
		virtual_position.Width = w;
		virtual_position.Height = h;
		DrawVirtualPosition(virtual_position);
	}

	protected virtual void HandleLButtonUp(ref Message m)
	{
		if (state != 0)
		{
			ClearVirtualPosition();
			form.Capture = false;
			if (state == State.Moving && form.Location != virtual_position.Location)
			{
				form.Location = virtual_position.Location;
			}
			else if (state == State.Sizing && form.Bounds != virtual_position)
			{
				form.Bounds = virtual_position;
			}
			state = State.Idle;
			OnWindowFinishedMoving();
		}
	}

	private bool HandleNCLButtonUp(ref Message m)
	{
		if (form.Capture)
		{
			ClearVirtualPosition();
			form.Capture = false;
			state = State.Idle;
			if (form.MdiContainer != null)
			{
				form.MdiContainer.SizeScrollBars();
			}
		}
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCPointToClient(ref x, ref y);
		FormPos formPos = FormPosForCoords(x, y);
		if (formPos == FormPos.TitleBar)
		{
			HandleTitleBarUp(x, y);
			return true;
		}
		return true;
	}

	protected void DrawTitleButton(Graphics dc, TitleButton button, Rectangle clip)
	{
		if (button.Rectangle.IntersectsWith(clip))
		{
			ThemeEngine.Current.ManagedWindowDrawMenuButton(dc, button, clip, this);
		}
	}

	public virtual void DrawMaximizedButtons(object sender, PaintEventArgs pe)
	{
	}

	protected Point MouseMove(Point pos)
	{
		return new Point(pos.X - start.X, pos.Y - start.Y);
	}

	protected virtual void DrawVirtualPosition(Rectangle virtual_position)
	{
		form.Bounds = virtual_position;
		start = Cursor.Position;
	}

	protected virtual void ClearVirtualPosition()
	{
	}

	protected virtual void OnWindowFinishedMoving()
	{
	}

	protected virtual void NCPointToClient(ref int x, ref int y)
	{
		form.PointToClient(ref x, ref y);
		NCClientToNC(ref x, ref y);
	}

	protected virtual void NCClientToNC(ref int x, ref int y)
	{
		y += TitleBarHeight;
		y += BorderWidth;
		y += MenuHeight;
	}

	internal Point GetMenuOrigin()
	{
		return new Point(BorderWidth, BorderWidth + TitleBarHeight);
	}

	protected FormPos FormPosForCoords(int x, int y)
	{
		int borderWidth = BorderWidth;
		if (y < TitleBarHeight + borderWidth)
		{
			if (y > borderWidth && x > borderWidth && x < form.Width - borderWidth)
			{
				return FormPos.TitleBar;
			}
			if (x < borderWidth || (x < 20 && y < borderWidth))
			{
				return FormPos.TopLeft;
			}
			if (x > form.Width - borderWidth || (x > form.Width - 20 && y < borderWidth))
			{
				return FormPos.TopRight;
			}
			if (y < borderWidth)
			{
				return FormPos.Top;
			}
		}
		else if (y > form.Height - 20)
		{
			if (x < borderWidth || (x < 20 && y > form.Height - borderWidth))
			{
				return FormPos.BottomLeft;
			}
			if (x > form.Width - borderWidth * 2 || (x > form.Width - 20 && y > form.Height - borderWidth))
			{
				return FormPos.BottomRight;
			}
			if (y > form.Height - borderWidth * 2)
			{
				return FormPos.Bottom;
			}
		}
		else
		{
			if (x < borderWidth)
			{
				return FormPos.Left;
			}
			if (x > form.Width - borderWidth * 2)
			{
				return FormPos.Right;
			}
		}
		return FormPos.None;
	}
}
