using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Windows.Forms;

public abstract class ToolStripRenderer
{
	private static ColorMatrix grayscale_matrix = new ColorMatrix(new float[5][]
	{
		new float[5] { 0.22f, 0.22f, 0.22f, 0f, 0f },
		new float[5] { 0.27f, 0.27f, 0.27f, 0f, 0f },
		new float[5] { 0.04f, 0.04f, 0.04f, 0f, 0f },
		new float[5] { 0.365f, 0.365f, 0.365f, 0.7f, 0f },
		new float[5] { 0f, 0f, 0f, 0f, 1f }
	});

	private EventHandlerList events;

	private static object RenderArrowEvent;

	private static object RenderButtonBackgroundEvent;

	private static object RenderDropDownButtonBackgroundEvent;

	private static object RenderGripEvent;

	private static object RenderImageMarginEvent;

	private static object RenderItemBackgroundEvent;

	private static object RenderItemCheckEvent;

	private static object RenderItemImageEvent;

	private static object RenderItemTextEvent;

	private static object RenderLabelBackgroundEvent;

	private static object RenderMenuItemBackgroundEvent;

	private static object RenderOverflowButtonBackgroundEvent;

	private static object RenderSeparatorEvent;

	private static object RenderSplitButtonBackgroundEvent;

	private static object RenderStatusStripSizingGripEvent;

	private static object RenderToolStripBackgroundEvent;

	private static object RenderToolStripBorderEvent;

	private static object RenderToolStripContentPanelBackgroundEvent;

	private static object RenderToolStripPanelBackgroundEvent;

	private static object RenderToolStripStatusLabelBackgroundEvent;

	private EventHandlerList Events
	{
		get
		{
			if (events == null)
			{
				events = new EventHandlerList();
			}
			return events;
		}
	}

	public event ToolStripArrowRenderEventHandler RenderArrow
	{
		add
		{
			Events.AddHandler(RenderArrowEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderArrowEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderButtonBackground
	{
		add
		{
			Events.AddHandler(RenderButtonBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderButtonBackgroundEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderDropDownButtonBackground
	{
		add
		{
			Events.AddHandler(RenderDropDownButtonBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderDropDownButtonBackgroundEvent, value);
		}
	}

	public event ToolStripGripRenderEventHandler RenderGrip
	{
		add
		{
			Events.AddHandler(RenderGripEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderGripEvent, value);
		}
	}

	public event ToolStripRenderEventHandler RenderImageMargin
	{
		add
		{
			Events.AddHandler(RenderImageMarginEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderImageMarginEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderItemBackground
	{
		add
		{
			Events.AddHandler(RenderItemBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderItemBackgroundEvent, value);
		}
	}

	public event ToolStripItemImageRenderEventHandler RenderItemCheck
	{
		add
		{
			Events.AddHandler(RenderItemCheckEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderItemCheckEvent, value);
		}
	}

	public event ToolStripItemImageRenderEventHandler RenderItemImage
	{
		add
		{
			Events.AddHandler(RenderItemImageEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderItemImageEvent, value);
		}
	}

	public event ToolStripItemTextRenderEventHandler RenderItemText
	{
		add
		{
			Events.AddHandler(RenderItemTextEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderItemTextEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderLabelBackground
	{
		add
		{
			Events.AddHandler(RenderLabelBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderLabelBackgroundEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderMenuItemBackground
	{
		add
		{
			Events.AddHandler(RenderMenuItemBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderMenuItemBackgroundEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderOverflowButtonBackground
	{
		add
		{
			Events.AddHandler(RenderOverflowButtonBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderOverflowButtonBackgroundEvent, value);
		}
	}

	public event ToolStripSeparatorRenderEventHandler RenderSeparator
	{
		add
		{
			Events.AddHandler(RenderSeparatorEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderSeparatorEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderSplitButtonBackground
	{
		add
		{
			Events.AddHandler(RenderSplitButtonBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderSplitButtonBackgroundEvent, value);
		}
	}

	public event ToolStripRenderEventHandler RenderStatusStripSizingGrip
	{
		add
		{
			Events.AddHandler(RenderStatusStripSizingGripEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderStatusStripSizingGripEvent, value);
		}
	}

	public event ToolStripRenderEventHandler RenderToolStripBackground
	{
		add
		{
			Events.AddHandler(RenderToolStripBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderToolStripBackgroundEvent, value);
		}
	}

	public event ToolStripRenderEventHandler RenderToolStripBorder
	{
		add
		{
			Events.AddHandler(RenderToolStripBorderEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderToolStripBorderEvent, value);
		}
	}

	public event ToolStripContentPanelRenderEventHandler RenderToolStripContentPanelBackground
	{
		add
		{
			Events.AddHandler(RenderToolStripContentPanelBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderToolStripContentPanelBackgroundEvent, value);
		}
	}

	public event ToolStripPanelRenderEventHandler RenderToolStripPanelBackground
	{
		add
		{
			Events.AddHandler(RenderToolStripPanelBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderToolStripPanelBackgroundEvent, value);
		}
	}

	public event ToolStripItemRenderEventHandler RenderToolStripStatusLabelBackground
	{
		add
		{
			Events.AddHandler(RenderToolStripStatusLabelBackgroundEvent, value);
		}
		remove
		{
			Events.RemoveHandler(RenderToolStripStatusLabelBackgroundEvent, value);
		}
	}

	static ToolStripRenderer()
	{
		RenderArrow = new object();
		RenderButtonBackground = new object();
		RenderDropDownButtonBackground = new object();
		RenderGrip = new object();
		RenderImageMargin = new object();
		RenderItemBackground = new object();
		RenderItemCheck = new object();
		RenderItemImage = new object();
		RenderItemText = new object();
		RenderLabelBackground = new object();
		RenderMenuItemBackground = new object();
		RenderOverflowButtonBackground = new object();
		RenderSeparator = new object();
		RenderSplitButtonBackground = new object();
		RenderStatusStripSizingGrip = new object();
		RenderToolStripBackground = new object();
		RenderToolStripBorder = new object();
		RenderToolStripContentPanelBackground = new object();
		RenderToolStripPanelBackground = new object();
		RenderToolStripStatusLabelBackground = new object();
	}

	public static Image CreateDisabledImage(Image normalImage)
	{
		if (normalImage == null)
		{
			return null;
		}
		ImageAttributes imageAttributes = new ImageAttributes();
		imageAttributes.SetColorMatrix(grayscale_matrix);
		Bitmap bitmap = new Bitmap(normalImage.Width, normalImage.Height);
		Graphics.FromImage(bitmap).DrawImage(normalImage, new Rectangle(0, 0, normalImage.Width, normalImage.Height), 0, 0, normalImage.Width, normalImage.Height, GraphicsUnit.Pixel, imageAttributes);
		return bitmap;
	}

	public void DrawArrow(ToolStripArrowRenderEventArgs e)
	{
		OnRenderArrow(e);
	}

	public void DrawButtonBackground(ToolStripItemRenderEventArgs e)
	{
		OnRenderButtonBackground(e);
	}

	public void DrawDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		OnRenderDropDownButtonBackground(e);
	}

	public void DrawGrip(ToolStripGripRenderEventArgs e)
	{
		OnRenderGrip(e);
	}

	public void DrawImageMargin(ToolStripRenderEventArgs e)
	{
		OnRenderImageMargin(e);
	}

	public void DrawItemBackground(ToolStripItemRenderEventArgs e)
	{
		OnRenderItemBackground(e);
	}

	public void DrawItemCheck(ToolStripItemImageRenderEventArgs e)
	{
		OnRenderItemCheck(e);
	}

	public void DrawItemImage(ToolStripItemImageRenderEventArgs e)
	{
		OnRenderItemImage(e);
	}

	public void DrawItemText(ToolStripItemTextRenderEventArgs e)
	{
		OnRenderItemText(e);
	}

	public void DrawLabelBackground(ToolStripItemRenderEventArgs e)
	{
		OnRenderLabelBackground(e);
	}

	public void DrawMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		OnRenderMenuItemBackground(e);
	}

	public void DrawOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		OnRenderOverflowButtonBackground(e);
	}

	public void DrawSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		OnRenderSeparator(e);
	}

	public void DrawSplitButton(ToolStripItemRenderEventArgs e)
	{
		OnRenderSplitButtonBackground(e);
	}

	public void DrawStatusStripSizingGrip(ToolStripRenderEventArgs e)
	{
		OnRenderStatusStripSizingGrip(e);
	}

	public void DrawToolStripBackground(ToolStripRenderEventArgs e)
	{
		OnRenderToolStripBackground(e);
	}

	public void DrawToolStripBorder(ToolStripRenderEventArgs e)
	{
		OnRenderToolStripBorder(e);
	}

	public void DrawToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
	{
		OnRenderToolStripContentPanelBackground(e);
	}

	public void DrawToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
	{
		OnRenderToolStripPanelBackground(e);
	}

	public void DrawToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
	{
		OnRenderToolStripStatusLabelBackground(e);
	}

	protected internal virtual void Initialize(ToolStrip toolStrip)
	{
	}

	protected internal virtual void InitializeContentPanel(ToolStripContentPanel contentPanel)
	{
	}

	protected internal virtual void InitializeItem(ToolStripItem item)
	{
	}

	protected internal virtual void InitializePanel(ToolStripPanel toolStripPanel)
	{
	}

	protected virtual void OnRenderArrow(ToolStripArrowRenderEventArgs e)
	{
		switch (e.Direction)
		{
		case ArrowDirection.Down:
		{
			using (Pen p2 = new Pen(e.ArrowColor))
			{
				int x2 = e.ArrowRectangle.Left + e.ArrowRectangle.Width / 2 - 3;
				int y2 = e.ArrowRectangle.Top + e.ArrowRectangle.Height / 2 - 2;
				DrawDownArrow(e.Graphics, p2, x2, y2);
			}
			break;
		}
		case ArrowDirection.Right:
		{
			using (Pen p = new Pen(e.ArrowColor))
			{
				int x = e.ArrowRectangle.Left + e.ArrowRectangle.Width / 2 - 3;
				int y = e.ArrowRectangle.Top + e.ArrowRectangle.Height / 2 - 4;
				DrawRightArrow(e.Graphics, p, x, y);
			}
			break;
		}
		}
		((ToolStripArrowRenderEventHandler)Events[RenderArrow])?.Invoke(this, e);
	}

	protected virtual void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		((ToolStripItemRenderEventHandler)Events[RenderButtonBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		((ToolStripItemRenderEventHandler)Events[RenderDropDownButtonBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderGrip(ToolStripGripRenderEventArgs e)
	{
		((ToolStripGripRenderEventHandler)Events[RenderGrip])?.Invoke(this, e);
	}

	protected virtual void OnRenderImageMargin(ToolStripRenderEventArgs e)
	{
		((ToolStripRenderEventHandler)Events[RenderImageMargin])?.Invoke(this, e);
	}

	protected virtual void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
	{
		if (e.Item.BackgroundImage != null)
		{
			Rectangle rectangle = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(e.Item.BackColor), rectangle);
			DrawBackground(e.Graphics, rectangle, e.Item.BackgroundImage, e.Item.BackgroundImageLayout);
		}
		((ToolStripItemRenderEventHandler)Events[RenderItemBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
	{
		((ToolStripItemImageRenderEventHandler)Events[RenderItemCheck])?.Invoke(this, e);
	}

	protected virtual void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
	{
		bool flag = false;
		Image image = e.Image;
		if (e.Item.RightToLeft == RightToLeft.Yes && e.Item.RightToLeftAutoMirrorImage)
		{
			image = CreateMirrorImage(image);
			flag = true;
		}
		if (e.Item.ImageTransparentColor != Color.Empty)
		{
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetColorKey(e.Item.ImageTransparentColor, e.Item.ImageTransparentColor);
			e.Graphics.DrawImage(image, e.ImageRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			imageAttributes.Dispose();
		}
		else
		{
			e.Graphics.DrawImage(image, e.ImageRectangle);
		}
		if (flag)
		{
			image.Dispose();
		}
		((ToolStripItemImageRenderEventHandler)Events[RenderItemImage])?.Invoke(this, e);
	}

	protected virtual void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		if (e.TextDirection == ToolStripTextDirection.Vertical90)
		{
			GraphicsState gstate = e.Graphics.Save();
			PointF pointF = new PointF(e.Graphics.Transform.OffsetX, e.Graphics.Transform.OffsetY);
			e.Graphics.ResetTransform();
			e.Graphics.RotateTransform(90f);
			RectangleF layoutRectangle = new RectangleF((e.Item.Height - e.TextRectangle.Height) / 2, ((float)e.TextRectangle.Width + pointF.X) * -1f - 18f, e.TextRectangle.Height, e.TextRectangle.Width);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			e.Graphics.DrawString(e.Text, e.TextFont, ThemeEngine.Current.ResPool.GetSolidBrush(e.TextColor), layoutRectangle, stringFormat);
			e.Graphics.Restore(gstate);
		}
		else if (e.TextDirection == ToolStripTextDirection.Vertical270)
		{
			GraphicsState gstate2 = e.Graphics.Save();
			PointF pointF2 = new PointF(e.Graphics.Transform.OffsetX, e.Graphics.Transform.OffsetY);
			e.Graphics.ResetTransform();
			e.Graphics.RotateTransform(270f);
			RectangleF layoutRectangle2 = new RectangleF(-e.TextRectangle.Height - (e.Item.Height - e.TextRectangle.Height) / 2, (float)e.TextRectangle.Width + pointF2.X + 4f, e.TextRectangle.Height, e.TextRectangle.Width);
			StringFormat stringFormat2 = new StringFormat();
			stringFormat2.Alignment = StringAlignment.Center;
			e.Graphics.DrawString(e.Text, e.TextFont, ThemeEngine.Current.ResPool.GetSolidBrush(e.TextColor), layoutRectangle2, stringFormat2);
			e.Graphics.Restore(gstate2);
		}
		else
		{
			TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, e.TextRectangle, e.TextColor, e.TextFormat);
		}
		((ToolStripItemTextRenderEventHandler)Events[RenderItemText])?.Invoke(this, e);
	}

	protected virtual void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
	{
		((ToolStripItemRenderEventHandler)Events[RenderLabelBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		((ToolStripItemRenderEventHandler)Events[RenderMenuItemBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		((ToolStripItemRenderEventHandler)Events[RenderOverflowButtonBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		((ToolStripSeparatorRenderEventHandler)Events[RenderSeparator])?.Invoke(this, e);
	}

	protected virtual void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
	{
		((ToolStripItemRenderEventHandler)Events[RenderSplitButtonBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
	{
		StatusStrip statusStrip = (StatusStrip)e.ToolStrip;
		if (statusStrip.SizingGrip)
		{
			DrawSizingGrip(e.Graphics, statusStrip.SizeGripBounds);
		}
		((ToolStripRenderEventHandler)Events[RenderStatusStripSizingGrip])?.Invoke(this, e);
	}

	protected virtual void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		((ToolStripRenderEventHandler)Events[RenderToolStripBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
	{
		((ToolStripRenderEventHandler)Events[RenderToolStripBorder])?.Invoke(this, e);
	}

	protected virtual void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
	{
		((ToolStripContentPanelRenderEventHandler)Events[RenderToolStripContentPanelBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
	{
		((ToolStripPanelRenderEventHandler)Events[RenderToolStripPanelBackground])?.Invoke(this, e);
	}

	protected virtual void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
	{
		((ToolStripItemRenderEventHandler)Events[RenderToolStripStatusLabelBackground])?.Invoke(this, e);
	}

	internal static Image CreateMirrorImage(Image normalImage)
	{
		if (normalImage == null)
		{
			return null;
		}
		Bitmap bitmap = new Bitmap(normalImage);
		bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
		return bitmap;
	}

	private void DrawBackground(Graphics g, Rectangle bounds, Image image, ImageLayout layout)
	{
		if ((layout == ImageLayout.Center || layout == ImageLayout.Tile) && image.Size.Width >= bounds.Size.Width && image.Size.Height >= bounds.Size.Height)
		{
			layout = ImageLayout.None;
		}
		switch (layout)
		{
		case ImageLayout.None:
			g.DrawImageUnscaledAndClipped(image, bounds);
			break;
		case ImageLayout.Tile:
		{
			int i = 0;
			for (int j = 0; j < bounds.Height; j += image.Height)
			{
				for (; i < bounds.Width; i += image.Width)
				{
					g.DrawImageUnscaledAndClipped(image, bounds);
				}
				i = 0;
			}
			break;
		}
		case ImageLayout.Center:
		{
			Rectangle rect3 = new Rectangle((bounds.Size.Width - image.Size.Width) / 2, (bounds.Size.Height - image.Size.Height) / 2, image.Width, image.Height);
			g.DrawImageUnscaledAndClipped(image, rect3);
			break;
		}
		case ImageLayout.Stretch:
			g.DrawImage(image, bounds);
			break;
		case ImageLayout.Zoom:
			if ((float)image.Height / (float)image.Width < (float)bounds.Height / (float)bounds.Width)
			{
				Rectangle rect = new Rectangle(0, 0, bounds.Width, (int)((float)bounds.Width * ((float)image.Height / (float)image.Width)));
				rect.Y = (bounds.Height - rect.Height) / 2;
				g.DrawImage(image, rect);
			}
			else
			{
				Rectangle rect2 = new Rectangle(0, 0, (int)((float)bounds.Height * ((float)image.Width / (float)image.Height)), bounds.Height);
				rect2.X = (bounds.Width - rect2.Width) / 2;
				g.DrawImage(image, rect2);
			}
			break;
		}
	}

	internal static void DrawRightArrow(Graphics g, Pen p, int x, int y)
	{
		g.DrawLine(p, x, y, x, y + 6);
		g.DrawLine(p, x + 1, y + 1, x + 1, y + 5);
		g.DrawLine(p, x + 2, y + 2, x + 2, y + 4);
		g.DrawLine(p, x + 2, y + 3, x + 3, y + 3);
	}

	internal static void DrawDownArrow(Graphics g, Pen p, int x, int y)
	{
		g.DrawLine(p, x + 1, y, x + 5, y);
		g.DrawLine(p, x + 2, y + 1, x + 4, y + 1);
		g.DrawLine(p, x + 3, y + 1, x + 3, y + 2);
	}

	private void DrawSizingGrip(Graphics g, Rectangle rect)
	{
		DrawGripBox(g, rect.Right - 5, rect.Bottom - 5);
		DrawGripBox(g, rect.Right - 9, rect.Bottom - 5);
		DrawGripBox(g, rect.Right - 5, rect.Bottom - 9);
		DrawGripBox(g, rect.Right - 13, rect.Bottom - 5);
		DrawGripBox(g, rect.Right - 5, rect.Bottom - 13);
		DrawGripBox(g, rect.Right - 9, rect.Bottom - 9);
	}

	private void DrawGripBox(Graphics g, int x, int y)
	{
		g.DrawRectangle(Pens.White, x + 1, y + 1, 1, 1);
		g.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(Color.FromArgb(172, 168, 153)), x, y, 1, 1);
	}
}
