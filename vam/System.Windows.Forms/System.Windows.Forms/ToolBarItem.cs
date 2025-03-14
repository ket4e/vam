using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;

namespace System.Windows.Forms;

internal class ToolBarItem : Component
{
	private ToolBar toolbar;

	private ToolBarButton button;

	private Rectangle bounds;

	private Rectangle image_rect;

	private Rectangle text_rect;

	private bool dd_pressed;

	private bool inside;

	private bool hilight;

	private bool pressed;

	public ToolBarButton Button => button;

	public Rectangle Rectangle
	{
		get
		{
			if (!button.Visible || toolbar == null)
			{
				return Rectangle.Empty;
			}
			if (button.Style == ToolBarButtonStyle.DropDownButton && toolbar.DropDownArrows)
			{
				Rectangle result = bounds;
				result.Width += ThemeEngine.Current.ToolBarDropDownWidth;
				return result;
			}
			return bounds;
		}
		set
		{
			bounds = value;
		}
	}

	public Point Location
	{
		get
		{
			return bounds.Location;
		}
		set
		{
			bounds.Location = value;
		}
	}

	public Rectangle ImageRectangle
	{
		get
		{
			Rectangle result = image_rect;
			result.X += bounds.X;
			result.Y += bounds.Y;
			return result;
		}
	}

	public Rectangle TextRectangle
	{
		get
		{
			Rectangle result = text_rect;
			result.X += bounds.X;
			result.Y += bounds.Y;
			return result;
		}
	}

	private Size TextSize
	{
		get
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.HotkeyPrefix = HotkeyPrefix.Hide;
			SizeF sizeF = TextRenderer.MeasureString(button.Text, toolbar.Font, SizeF.Empty, stringFormat);
			if (sizeF == SizeF.Empty)
			{
				return Size.Empty;
			}
			return new Size((int)Math.Ceiling(sizeF.Width) + 6, (int)Math.Ceiling(sizeF.Height));
		}
	}

	public bool Pressed
	{
		get
		{
			return pressed && inside;
		}
		set
		{
			pressed = value;
		}
	}

	public bool DDPressed
	{
		get
		{
			return dd_pressed;
		}
		set
		{
			dd_pressed = value;
		}
	}

	public bool Inside
	{
		get
		{
			return inside;
		}
		set
		{
			inside = value;
		}
	}

	public bool Hilight
	{
		get
		{
			return hilight;
		}
		set
		{
			if (hilight != value)
			{
				hilight = value;
				Invalidate();
			}
		}
	}

	public ToolBarItem(ToolBarButton button)
	{
		toolbar = button.Parent;
		this.button = button;
	}

	public Size CalculateSize()
	{
		Theme current = ThemeEngine.Current;
		int height = toolbar.ButtonSize.Height + 2 * current.ToolBarGripWidth;
		if (button.Style == ToolBarButtonStyle.Separator)
		{
			return new Size(current.ToolBarSeparatorWidth, height);
		}
		Size result = ((!TextSize.IsEmpty || button.Image != null) ? TextSize : toolbar.default_size);
		Size size = ((!(toolbar.ImageSize == Size.Empty)) ? toolbar.ImageSize : new Size(16, 16));
		int num = size.Width + 2 * current.ToolBarImageGripWidth;
		int num2 = size.Height + 2 * current.ToolBarImageGripWidth;
		if (toolbar.TextAlign == ToolBarTextAlign.Right)
		{
			result.Width = num + result.Width;
			result.Height = ((result.Height <= num2) ? num2 : result.Height);
		}
		else
		{
			result.Height = num2 + result.Height;
			result.Width = ((result.Width <= num) ? num : result.Width);
		}
		result.Width += current.ToolBarGripWidth;
		result.Height += current.ToolBarGripWidth;
		return result;
	}

	public bool Layout(bool vertical, int calculated_size)
	{
		if (toolbar == null || !button.Visible)
		{
			return false;
		}
		Size buttonSize = toolbar.ButtonSize;
		Size size = buttonSize;
		if (!toolbar.SizeSpecified || button.Style == ToolBarButtonStyle.Separator)
		{
			size = CalculateSize();
			if (size.Width == 0 || size.Height == 0)
			{
				size = buttonSize;
			}
			if (vertical)
			{
				size.Width = calculated_size;
			}
			else
			{
				size.Height = calculated_size;
			}
		}
		return Layout(size);
	}

	public bool Layout(Size size)
	{
		if (toolbar == null || !button.Visible)
		{
			return false;
		}
		bounds.Size = size;
		Size size2 = ((!(toolbar.ImageSize == Size.Empty)) ? toolbar.ImageSize : new Size(16, 16));
		int toolBarImageGripWidth = ThemeEngine.Current.ToolBarImageGripWidth;
		Rectangle rectangle;
		Rectangle rectangle2;
		if (toolbar.TextAlign == ToolBarTextAlign.Underneath)
		{
			rectangle = new Rectangle((bounds.Size.Width - size2.Width) / 2 - toolBarImageGripWidth, 0, size2.Width + 2 + toolBarImageGripWidth, size2.Height + 2 * toolBarImageGripWidth);
			rectangle2 = new Rectangle(0, rectangle.Height, bounds.Size.Width, bounds.Size.Height - rectangle.Height - 2 * toolBarImageGripWidth);
		}
		else
		{
			rectangle = new Rectangle(0, 0, size2.Width + 2 * toolBarImageGripWidth, size2.Height + 2 * toolBarImageGripWidth);
			rectangle2 = new Rectangle(rectangle.Width, 0, bounds.Size.Width - rectangle.Width, bounds.Size.Height - 2 * toolBarImageGripWidth);
		}
		bool result = false;
		if (rectangle != image_rect || rectangle2 != text_rect)
		{
			result = true;
		}
		image_rect = rectangle;
		text_rect = rectangle2;
		return result;
	}

	public void Invalidate()
	{
		if (toolbar != null)
		{
			toolbar.Invalidate(Rectangle);
		}
	}
}
