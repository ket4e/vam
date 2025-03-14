using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class ButtonRenderer
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

	private ButtonRenderer()
	{
	}

	public static void DrawButton(Graphics g, Rectangle bounds, PushButtonState state)
	{
		DrawButton(g, bounds, string.Empty, null, TextFormatFlags.Left, null, Rectangle.Empty, focused: false, state);
	}

	public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state)
	{
		DrawButton(g, bounds, string.Empty, null, TextFormatFlags.Left, null, Rectangle.Empty, focused, state);
	}

	public static void DrawButton(Graphics g, Rectangle bounds, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
	{
		DrawButton(g, bounds, string.Empty, null, TextFormatFlags.Left, image, imageBounds, focused, state);
	}

	public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, bool focused, PushButtonState state)
	{
		DrawButton(g, bounds, buttonText, font, TextFormatFlags.HorizontalCenter, null, Rectangle.Empty, focused, state);
	}

	public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, TextFormatFlags flags, bool focused, PushButtonState state)
	{
		DrawButton(g, bounds, buttonText, font, flags, null, Rectangle.Empty, focused, state);
	}

	public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
	{
		DrawButton(g, bounds, buttonText, font, TextFormatFlags.HorizontalCenter, image, imageBounds, focused, state);
	}

	public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
	{
		if (Application.RenderWithVisualStyles || always_use_visual_styles)
		{
			VisualStyleRenderer pushButtonRenderer = GetPushButtonRenderer(state);
			pushButtonRenderer.DrawBackground(g, bounds);
			if (image != null)
			{
				pushButtonRenderer.DrawImage(g, imageBounds, image);
			}
		}
		else
		{
			if (state == PushButtonState.Pressed)
			{
				ControlPaint.DrawButton(g, bounds, ButtonState.Pushed);
			}
			else
			{
				ControlPaint.DrawButton(g, bounds, ButtonState.Normal);
			}
			if (image != null)
			{
				g.DrawImage(image, imageBounds);
			}
		}
		Rectangle rectangle = bounds;
		rectangle.Inflate(-3, -3);
		if (focused)
		{
			ControlPaint.DrawFocusRectangle(g, rectangle);
		}
		if (buttonText != string.Empty)
		{
			if (state == PushButtonState.Disabled)
			{
				TextRenderer.DrawText(g, buttonText, font, rectangle, SystemColors.GrayText, flags);
			}
			else
			{
				TextRenderer.DrawText(g, buttonText, font, rectangle, SystemColors.ControlText, flags);
			}
		}
	}

	public static bool IsBackgroundPartiallyTransparent(PushButtonState state)
	{
		if (!VisualStyleRenderer.IsSupported)
		{
			return false;
		}
		VisualStyleRenderer pushButtonRenderer = GetPushButtonRenderer(state);
		return pushButtonRenderer.IsBackgroundPartiallyTransparent();
	}

	public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
	{
		if (VisualStyleRenderer.IsSupported)
		{
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Default);
			visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
		}
	}

	internal static VisualStyleRenderer GetPushButtonRenderer(PushButtonState state)
	{
		return state switch
		{
			PushButtonState.Normal => new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal), 
			PushButtonState.Hot => new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Hot), 
			PushButtonState.Pressed => new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Pressed), 
			PushButtonState.Disabled => new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Disabled), 
			_ => new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Default), 
		};
	}
}
