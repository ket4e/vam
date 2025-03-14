using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class GroupBoxRenderer
{
	private static bool always_use_visual_styles;

	public static bool RenderMatchingApplicationState
	{
		get
		{
			return !always_use_visual_styles;
		}
		set
		{
			always_use_visual_styles = !value;
		}
	}

	private GroupBoxRenderer()
	{
	}

	public static void DrawGroupBox(Graphics g, Rectangle bounds, GroupBoxState state)
	{
		DrawGroupBox(g, bounds, string.Empty, null, Color.Empty, TextFormatFlags.Left, state);
	}

	public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, GroupBoxState state)
	{
		DrawGroupBox(g, bounds, groupBoxText, font, Color.Empty, TextFormatFlags.Left, state);
	}

	public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, GroupBoxState state)
	{
		DrawGroupBox(g, bounds, groupBoxText, font, textColor, TextFormatFlags.Left, state);
	}

	public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, TextFormatFlags flags, GroupBoxState state)
	{
		DrawGroupBox(g, bounds, groupBoxText, font, Color.Empty, flags, state);
	}

	public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, TextFormatFlags flags, GroupBoxState state)
	{
		Size size = TextRenderer.MeasureText(groupBoxText, font);
		if (Application.RenderWithVisualStyles || always_use_visual_styles)
		{
			VisualStyleRenderer visualStyleRenderer;
			Rectangle bounds2;
			if (state == GroupBoxState.Normal || state != GroupBoxState.Disabled)
			{
				visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal);
				bounds2 = new Rectangle(bounds.Left, bounds.Top + size.Height / 2 - 1, bounds.Width, bounds.Height - size.Height / 2 + 1);
			}
			else
			{
				visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Disabled);
				bounds2 = new Rectangle(bounds.Left, bounds.Top + size.Height / 2 - 2, bounds.Width, bounds.Height - size.Height / 2 + 2);
			}
			if (groupBoxText == string.Empty)
			{
				visualStyleRenderer.DrawBackground(g, bounds);
			}
			else
			{
				visualStyleRenderer.DrawBackgroundExcludingArea(g, bounds2, new Rectangle(bounds.Left + 9, bounds.Top, size.Width - 3, size.Height));
			}
			if (textColor == Color.Empty)
			{
				textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
			}
			if (groupBoxText != string.Empty)
			{
				TextRenderer.DrawText(g, groupBoxText, font, new Point(bounds.Left + 8, bounds.Top), textColor, flags);
			}
			return;
		}
		Rectangle rectangle = new Rectangle(bounds.Left, bounds.Top + size.Height / 2, bounds.Width, bounds.Height - size.Height / 2);
		Region clip = g.Clip;
		g.SetClip(new Rectangle(bounds.Left + 9, bounds.Top, size.Width - 3, size.Height), CombineMode.Exclude);
		ControlPaint.DrawBorder3D(g, rectangle, Border3DStyle.Etched);
		g.Clip = clip;
		if (groupBoxText != string.Empty)
		{
			if (textColor == Color.Empty)
			{
				textColor = ((state != GroupBoxState.Normal) ? SystemColors.GrayText : SystemColors.ControlText);
			}
			TextRenderer.DrawText(g, groupBoxText, font, new Point(bounds.Left + 8, bounds.Top), textColor, flags);
		}
	}

	public static bool IsBackgroundPartiallyTransparent(GroupBoxState state)
	{
		if (!VisualStyleRenderer.IsSupported)
		{
			return false;
		}
		VisualStyleRenderer visualStyleRenderer = ((state != GroupBoxState.Normal && state == GroupBoxState.Disabled) ? new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Disabled) : new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal));
		return visualStyleRenderer.IsBackgroundPartiallyTransparent();
	}

	public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
	{
		if (VisualStyleRenderer.IsSupported)
		{
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal);
			visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
		}
	}
}
