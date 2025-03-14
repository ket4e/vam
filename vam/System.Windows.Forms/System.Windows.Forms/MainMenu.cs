using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxItemFilter("System.Windows.Forms.MainMenu", ToolboxItemFilterType.Allow)]
public class MainMenu : Menu
{
	private RightToLeft right_to_left = RightToLeft.Inherit;

	private Form form;

	private static object CollapseEvent;

	private static object PaintEvent;

	[Localizable(true)]
	[AmbientValue(RightToLeft.Inherit)]
	public virtual RightToLeft RightToLeft
	{
		get
		{
			return right_to_left;
		}
		set
		{
			right_to_left = value;
		}
	}

	public event EventHandler Collapse
	{
		add
		{
			base.Events.AddHandler(CollapseEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CollapseEvent, value);
		}
	}

	internal event PaintEventHandler Paint
	{
		add
		{
			base.Events.AddHandler(PaintEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PaintEvent, value);
		}
	}

	public MainMenu()
		: base(null)
	{
	}

	public MainMenu(MenuItem[] items)
		: base(items)
	{
	}

	public MainMenu(IContainer container)
		: this()
	{
		container.Add(this);
	}

	static MainMenu()
	{
		Collapse = new object();
		Paint = new object();
	}

	public virtual MainMenu CloneMenu()
	{
		MainMenu mainMenu = new MainMenu();
		mainMenu.CloneMenu(this);
		return mainMenu;
	}

	protected override IntPtr CreateMenuHandle()
	{
		return IntPtr.Zero;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	public Form GetForm()
	{
		return form;
	}

	public override string ToString()
	{
		return base.ToString() + ", GetForm: " + form;
	}

	protected internal virtual void OnCollapse(EventArgs e)
	{
		((EventHandler)base.Events[Collapse])?.Invoke(this, e);
	}

	internal void Draw()
	{
		Message msg = Message.Create(Wnd.window.Handle, 15, IntPtr.Zero, IntPtr.Zero);
		PaintEventArgs pe = XplatUI.PaintEventStart(ref msg, Wnd.window.Handle, client: false);
		Draw(pe, base.Rect);
	}

	internal void Draw(Rectangle rect)
	{
		if (Wnd.IsHandleCreated)
		{
			Point menuOrigin = XplatUI.GetMenuOrigin(Wnd.window.Handle);
			Message msg = Message.Create(Wnd.window.Handle, 15, IntPtr.Zero, IntPtr.Zero);
			PaintEventArgs paintEventArgs = XplatUI.PaintEventStart(ref msg, Wnd.window.Handle, client: false);
			paintEventArgs.Graphics.SetClip(new Rectangle(rect.X + menuOrigin.X, rect.Y + menuOrigin.Y, rect.Width, rect.Height));
			Draw(paintEventArgs, base.Rect);
			XplatUI.PaintEventEnd(ref msg, Wnd.window.Handle, client: false);
		}
	}

	internal void Draw(PaintEventArgs pe)
	{
		Draw(pe, base.Rect);
	}

	internal void Draw(PaintEventArgs pe, Rectangle rect)
	{
		if (Wnd.IsHandleCreated)
		{
			base.X = rect.X;
			base.Y = rect.Y;
			base.Height = base.Rect.Height;
			ThemeEngine.Current.DrawMenuBar(pe.Graphics, this, rect);
			((PaintEventHandler)base.Events[Paint])?.Invoke(this, pe);
		}
	}

	internal override void InvalidateItem(MenuItem item)
	{
		Draw(item.bounds);
	}

	internal void SetForm(Form form)
	{
		this.form = form;
		Wnd = form;
		if (tracker == null)
		{
			tracker = new MenuTracker(this);
			tracker.GrabControl = form;
		}
	}

	internal override void OnMenuChanged(EventArgs e)
	{
		base.OnMenuChanged(EventArgs.Empty);
		if (form != null)
		{
			Rectangle clip = base.Rect;
			base.Height = 0;
			if (Wnd.IsHandleCreated)
			{
				Message msg = Message.Create(Wnd.window.Handle, 15, IntPtr.Zero, IntPtr.Zero);
				PaintEventArgs paintEventArgs = XplatUI.PaintEventStart(ref msg, Wnd.window.Handle, client: false);
				paintEventArgs.Graphics.SetClip(clip);
				Draw(paintEventArgs, clip);
			}
		}
	}

	internal void OnMouseDown(object window, MouseEventArgs args)
	{
		tracker.OnMouseDown(args);
	}

	internal void OnMouseMove(object window, MouseEventArgs e)
	{
		MouseEventArgs args = new MouseEventArgs(e.Button, e.Clicks, Control.MousePosition.X, Control.MousePosition.Y, e.Delta);
		tracker.OnMotion(args);
	}
}
