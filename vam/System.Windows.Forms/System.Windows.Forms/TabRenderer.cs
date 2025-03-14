using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class TabRenderer
{
	public static bool IsSupported => VisualStyleInformation.IsEnabledByUser && (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled);

	private TabRenderer()
	{
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, TabItemState state)
	{
		DrawTabItem(g, bounds, string.Empty, null, TextFormatFlags.Left, null, Rectangle.Empty, focused: false, state);
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, bool focused, TabItemState state)
	{
		DrawTabItem(g, bounds, string.Empty, null, TextFormatFlags.Left, null, Rectangle.Empty, focused, state);
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, TabItemState state)
	{
		DrawTabItem(g, bounds, tabItemText, font, TextFormatFlags.HorizontalCenter, null, Rectangle.Empty, focused: false, state);
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
	{
		DrawTabItem(g, bounds, string.Empty, null, TextFormatFlags.HorizontalCenter, image, imageRectangle, focused, state);
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, bool focused, TabItemState state)
	{
		DrawTabItem(g, bounds, tabItemText, font, TextFormatFlags.HorizontalCenter, null, Rectangle.Empty, focused, state);
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, TextFormatFlags flags, bool focused, TabItemState state)
	{
		DrawTabItem(g, bounds, tabItemText, font, flags, null, Rectangle.Empty, focused, state);
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
	{
		DrawTabItem(g, bounds, tabItemText, font, TextFormatFlags.HorizontalCenter, image, imageRectangle, focused, state);
	}

	public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, TextFormatFlags flags, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = state switch
		{
			TabItemState.Disabled => new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Disabled), 
			TabItemState.Hot => new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Hot), 
			TabItemState.Selected => new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Normal), 
		};
		visualStyleRenderer.DrawBackground(g, bounds);
		if (image != null)
		{
			visualStyleRenderer.DrawImage(g, imageRectangle, image);
		}
		bounds.Offset(3, 3);
		bounds.Height -= 6;
		bounds.Width -= 6;
		if (tabItemText != string.Empty)
		{
			TextRenderer.DrawText(g, tabItemText, font, bounds, SystemColors.ControlText, flags);
		}
		if (focused)
		{
			ControlPaint.DrawFocusRectangle(g, bounds);
		}
	}

	public static void DrawTabPage(Graphics g, Rectangle bounds)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Tab.Pane.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}
}
