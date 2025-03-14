using System.Drawing;

namespace System.Windows.Forms;

internal class FormWindowManager : InternalWindowManager
{
	private bool pending_activation;

	internal override Rectangle MaximizedBounds
	{
		get
		{
			Rectangle maximizedBounds = base.MaximizedBounds;
			int num = ThemeEngine.Current.ManagedWindowBorderWidth(this);
			maximizedBounds.Inflate(num, num);
			return maximizedBounds;
		}
	}

	public FormWindowManager(Form form)
		: base(form)
	{
		form.MouseCaptureChanged += HandleCaptureChanged;
	}

	private void HandleCaptureChanged(object sender, EventArgs e)
	{
		if (pending_activation && !form.Capture)
		{
			form.BringToFront();
			pending_activation = false;
		}
	}

	public override void PointToClient(ref int x, ref int y)
	{
		XplatUI.ScreenToClient(base.Form.Parent.Handle, ref x, ref y);
	}

	protected override bool HandleNCLButtonDown(ref Message m)
	{
		pending_activation = true;
		return base.HandleNCLButtonDown(ref m);
	}

	protected override void HandleTitleBarDoubleClick(int x, int y)
	{
		if (IconRectangleContains(x, y))
		{
			form.Close();
		}
		else if (form.WindowState == FormWindowState.Maximized)
		{
			form.WindowState = FormWindowState.Normal;
		}
		else
		{
			form.WindowState = FormWindowState.Maximized;
		}
		base.HandleTitleBarDoubleClick(x, y);
	}
}
