using System.Drawing;

namespace System.Windows.Forms.Layout;

internal class DefaultLayout : LayoutEngine
{
	private void LayoutDockedChildren(Control parent, Control[] controls)
	{
		Rectangle displayRectangle = parent.DisplayRectangle;
		MdiClient mdiClient = null;
		for (int num = controls.Length - 1; num >= 0; num--)
		{
			Control control = controls[num];
			Size size = control.Size;
			if (control.AutoSize)
			{
				size = GetPreferredControlSize(control);
			}
			if (control.VisibleInternal && control.ControlLayoutType != 0)
			{
				if (control is MdiClient)
				{
					mdiClient = (MdiClient)control;
				}
				else
				{
					switch (control.Dock)
					{
					case DockStyle.Left:
						control.SetBoundsInternal(displayRectangle.Left, displayRectangle.Y, size.Width, displayRectangle.Height, BoundsSpecified.None);
						displayRectangle.X += control.Width;
						displayRectangle.Width -= control.Width;
						break;
					case DockStyle.Top:
						control.SetBoundsInternal(displayRectangle.Left, displayRectangle.Y, displayRectangle.Width, size.Height, BoundsSpecified.None);
						displayRectangle.Y += control.Height;
						displayRectangle.Height -= control.Height;
						break;
					case DockStyle.Right:
						control.SetBoundsInternal(displayRectangle.Right - size.Width, displayRectangle.Y, size.Width, displayRectangle.Height, BoundsSpecified.None);
						displayRectangle.Width -= control.Width;
						break;
					case DockStyle.Bottom:
						control.SetBoundsInternal(displayRectangle.Left, displayRectangle.Bottom - size.Height, displayRectangle.Width, size.Height, BoundsSpecified.None);
						displayRectangle.Height -= control.Height;
						break;
					case DockStyle.Fill:
						control.SetBoundsInternal(displayRectangle.Left, displayRectangle.Top, displayRectangle.Width, displayRectangle.Height, BoundsSpecified.None);
						break;
					}
				}
			}
		}
		mdiClient?.SetBoundsInternal(displayRectangle.Left, displayRectangle.Top, displayRectangle.Width, displayRectangle.Height, BoundsSpecified.None);
	}

	private void LayoutAnchoredChildren(Control parent, Control[] controls)
	{
		Rectangle clientRectangle = parent.ClientRectangle;
		foreach (Control control in controls)
		{
			if (!control.VisibleInternal || control.ControlLayoutType == Control.LayoutType.Dock)
			{
				continue;
			}
			AnchorStyles anchor = control.Anchor;
			int num = control.Left;
			int num2 = control.Top;
			int num3 = control.Width;
			int num4 = control.Height;
			if ((anchor & AnchorStyles.Right) != 0)
			{
				if ((anchor & AnchorStyles.Left) != 0)
				{
					num3 = clientRectangle.Width - control.dist_right - num;
				}
				else
				{
					num = clientRectangle.Width - control.dist_right - num3;
				}
			}
			else if ((anchor & AnchorStyles.Left) == 0)
			{
				num += (clientRectangle.Width - (num + num3 + control.dist_right)) / 2;
				control.dist_right = clientRectangle.Width - (num + num3);
			}
			if ((anchor & AnchorStyles.Bottom) != 0)
			{
				if ((anchor & AnchorStyles.Top) != 0)
				{
					num4 = clientRectangle.Height - control.dist_bottom - num2;
				}
				else
				{
					num2 = clientRectangle.Height - control.dist_bottom - num4;
				}
			}
			else if ((anchor & AnchorStyles.Top) == 0)
			{
				num2 += (clientRectangle.Height - (num2 + num4 + control.dist_bottom)) / 2;
				control.dist_bottom = clientRectangle.Height - (num2 + num4);
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			control.SetBoundsInternal(num, num2, num3, num4, BoundsSpecified.None);
		}
	}

	private void LayoutAutoSizedChildren(Control parent, Control[] controls)
	{
		foreach (Control control in controls)
		{
			if (control.VisibleInternal && control.ControlLayoutType != Control.LayoutType.Dock && control.AutoSize)
			{
				AnchorStyles anchor = control.Anchor;
				int left = control.Left;
				int top = control.Top;
				Size preferredControlSize = GetPreferredControlSize(control);
				if ((anchor & AnchorStyles.Left) != 0 || (anchor & AnchorStyles.Right) == 0)
				{
					control.dist_right += control.Width - preferredControlSize.Width;
				}
				if ((anchor & AnchorStyles.Top) != 0 || (anchor & AnchorStyles.Bottom) == 0)
				{
					control.dist_bottom += control.Height - preferredControlSize.Height;
				}
				control.SetBoundsInternal(left, top, preferredControlSize.Width, preferredControlSize.Height, BoundsSpecified.None);
			}
		}
	}

	private void LayoutAutoSizeContainer(Control container)
	{
		if (!container.VisibleInternal || container.ControlLayoutType == Control.LayoutType.Dock || !container.AutoSize)
		{
			return;
		}
		int left = container.Left;
		int top = container.Top;
		Size preferredSize = container.PreferredSize;
		int width;
		int height;
		if (container.GetAutoSizeMode() == AutoSizeMode.GrowAndShrink)
		{
			width = preferredSize.Width;
			height = preferredSize.Height;
		}
		else
		{
			width = container.ExplicitBounds.Width;
			height = container.ExplicitBounds.Height;
			if (preferredSize.Width > width)
			{
				width = preferredSize.Width;
			}
			if (preferredSize.Height > height)
			{
				height = preferredSize.Height;
			}
		}
		if (width < container.MinimumSize.Width)
		{
			width = container.MinimumSize.Width;
		}
		if (height < container.MinimumSize.Height)
		{
			height = container.MinimumSize.Height;
		}
		if (container.MaximumSize.Width != 0 && width > container.MaximumSize.Width)
		{
			width = container.MaximumSize.Width;
		}
		if (container.MaximumSize.Height != 0 && height > container.MaximumSize.Height)
		{
			height = container.MaximumSize.Height;
		}
		container.SetBoundsInternal(left, top, width, height, BoundsSpecified.None);
	}

	public override bool Layout(object container, LayoutEventArgs args)
	{
		Control control = container as Control;
		Control[] allControls = control.Controls.GetAllControls();
		LayoutDockedChildren(control, allControls);
		LayoutAnchoredChildren(control, allControls);
		LayoutAutoSizedChildren(control, allControls);
		if (control is Form)
		{
			LayoutAutoSizeContainer(control);
		}
		return false;
	}

	private Size GetPreferredControlSize(Control child)
	{
		Size preferredSize = child.PreferredSize;
		int width;
		int height;
		if (child.GetAutoSizeMode() == AutoSizeMode.GrowAndShrink || (child.Dock != 0 && !(child is Button)))
		{
			width = preferredSize.Width;
			height = preferredSize.Height;
		}
		else
		{
			width = child.ExplicitBounds.Width;
			height = child.ExplicitBounds.Height;
			if (preferredSize.Width > width)
			{
				width = preferredSize.Width;
			}
			if (preferredSize.Height > height)
			{
				height = preferredSize.Height;
			}
		}
		if (width < child.MinimumSize.Width)
		{
			width = child.MinimumSize.Width;
		}
		if (height < child.MinimumSize.Height)
		{
			height = child.MinimumSize.Height;
		}
		if (child.MaximumSize.Width != 0 && width > child.MaximumSize.Width)
		{
			width = child.MaximumSize.Width;
		}
		if (child.MaximumSize.Height != 0 && height > child.MaximumSize.Height)
		{
			height = child.MaximumSize.Height;
		}
		return new Size(width, height);
	}
}
