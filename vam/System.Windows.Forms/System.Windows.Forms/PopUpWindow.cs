using System.Drawing;

namespace System.Windows.Forms;

internal class PopUpWindow : Control
{
	private Menu menu;

	private Control form;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = base.CreateParams;
			createParams.Caption = "Menu PopUp";
			createParams.Style = int.MinValue;
			createParams.ExStyle |= 136;
			return createParams;
		}
	}

	internal override bool ActivateOnShow => false;

	public PopUpWindow(Control form, Menu menu)
	{
		this.menu = menu;
		this.form = form;
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.Opaque | ControlStyles.ResizeRedraw, value: true);
		is_visible = false;
	}

	public void ShowWindow()
	{
		XplatUI.SetCursor(form.Handle, Cursors.Default.handle);
		RefreshItems();
		Show();
	}

	internal override void OnPaintInternal(PaintEventArgs args)
	{
		ThemeEngine.Current.DrawPopupMenu(args.Graphics, menu, args.ClipRectangle, base.ClientRectangle);
	}

	public void HideWindow()
	{
		XplatUI.SetCursor(form.Handle, form.Cursor.handle);
		MenuTracker.HideSubPopups(menu, null);
		Hide();
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
		RefreshItems();
	}

	internal void RefreshItems()
	{
		Point location = new Point(base.Location.X, base.Location.Y);
		ThemeEngine.Current.CalcPopupMenuSize(base.DeviceContext, menu);
		if (location.X + menu.Rect.Width > SystemInformation.VirtualScreen.Width)
		{
			if (location.X - menu.Rect.Width > 0 && !(menu.parent_menu is MainMenu))
			{
				location.X -= menu.Rect.Width;
			}
			else
			{
				location.X = SystemInformation.VirtualScreen.Width - menu.Rect.Width;
			}
			if (location.X < 0)
			{
				location.X = 0;
			}
		}
		if (location.Y + menu.Rect.Height > SystemInformation.VirtualScreen.Height)
		{
			if (location.Y - menu.Rect.Height > 0)
			{
				location.Y -= menu.Rect.Height;
			}
			else
			{
				location.Y = SystemInformation.VirtualScreen.Height - menu.Rect.Height;
			}
			if (location.Y < 0)
			{
				location.Y = 0;
			}
		}
		base.Location = location;
		base.Width = menu.Rect.Width;
		base.Height = menu.Rect.Height;
	}
}
