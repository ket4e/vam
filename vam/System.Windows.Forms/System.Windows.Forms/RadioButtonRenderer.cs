using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class RadioButtonRenderer
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

	private RadioButtonRenderer()
	{
	}

	public static void DrawRadioButton(Graphics g, Point glyphLocation, RadioButtonState state)
	{
		DrawRadioButton(g, glyphLocation, Rectangle.Empty, string.Empty, null, TextFormatFlags.HorizontalCenter, null, Rectangle.Empty, focused: false, state);
	}

	public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, bool focused, RadioButtonState state)
	{
		DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font, TextFormatFlags.HorizontalCenter, null, Rectangle.Empty, focused, state);
	}

	public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, TextFormatFlags flags, bool focused, RadioButtonState state)
	{
		DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font, flags, null, Rectangle.Empty, focused, state);
	}

	public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, Image image, Rectangle imageBounds, bool focused, RadioButtonState state)
	{
		DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font, TextFormatFlags.HorizontalCenter, image, imageBounds, focused, state);
	}

	public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, RadioButtonState state)
	{
		Rectangle rectangle = new Rectangle(glyphLocation, GetGlyphSize(g, state));
		if (Application.RenderWithVisualStyles || always_use_visual_styles)
		{
			VisualStyleRenderer radioButtonRenderer = GetRadioButtonRenderer(state);
			radioButtonRenderer.DrawBackground(g, rectangle);
			if (image != null)
			{
				radioButtonRenderer.DrawImage(g, imageBounds, image);
			}
			if (focused)
			{
				ControlPaint.DrawFocusRectangle(g, textBounds);
			}
			if (radioButtonText != string.Empty)
			{
				if (state == RadioButtonState.CheckedDisabled || state == RadioButtonState.UncheckedDisabled)
				{
					TextRenderer.DrawText(g, radioButtonText, font, textBounds, SystemColors.GrayText, flags);
				}
				else
				{
					TextRenderer.DrawText(g, radioButtonText, font, textBounds, SystemColors.ControlText, flags);
				}
			}
			return;
		}
		switch (state)
		{
		case RadioButtonState.CheckedDisabled:
			ControlPaint.DrawRadioButton(g, rectangle, ButtonState.Inactive | ButtonState.Checked);
			break;
		case RadioButtonState.CheckedNormal:
		case RadioButtonState.CheckedHot:
			ControlPaint.DrawRadioButton(g, rectangle, ButtonState.Checked);
			break;
		case RadioButtonState.CheckedPressed:
			ControlPaint.DrawRadioButton(g, rectangle, ButtonState.Pushed | ButtonState.Checked);
			break;
		case RadioButtonState.UncheckedPressed:
		case RadioButtonState.UncheckedDisabled:
			ControlPaint.DrawRadioButton(g, rectangle, ButtonState.Inactive);
			break;
		case RadioButtonState.UncheckedNormal:
		case RadioButtonState.UncheckedHot:
			ControlPaint.DrawRadioButton(g, rectangle, ButtonState.Normal);
			break;
		}
		if (image != null)
		{
			g.DrawImage(image, imageBounds);
		}
		if (focused)
		{
			ControlPaint.DrawFocusRectangle(g, textBounds);
		}
		if (radioButtonText != string.Empty)
		{
			TextRenderer.DrawText(g, radioButtonText, font, textBounds, SystemColors.ControlText, flags);
		}
	}

	public static bool IsBackgroundPartiallyTransparent(RadioButtonState state)
	{
		if (!VisualStyleRenderer.IsSupported)
		{
			return false;
		}
		VisualStyleRenderer radioButtonRenderer = GetRadioButtonRenderer(state);
		return radioButtonRenderer.IsBackgroundPartiallyTransparent();
	}

	public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
	{
		if (VisualStyleRenderer.IsSupported)
		{
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.UncheckedNormal);
			visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
		}
	}

	public static Size GetGlyphSize(Graphics g, RadioButtonState state)
	{
		if (!VisualStyleRenderer.IsSupported)
		{
			return new Size(13, 13);
		}
		VisualStyleRenderer radioButtonRenderer = GetRadioButtonRenderer(state);
		return radioButtonRenderer.GetPartSize(g, ThemeSizeType.Draw);
	}

	private static VisualStyleRenderer GetRadioButtonRenderer(RadioButtonState state)
	{
		return state switch
		{
			RadioButtonState.CheckedDisabled => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.CheckedDisabled), 
			RadioButtonState.CheckedHot => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.CheckedHot), 
			RadioButtonState.CheckedNormal => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.CheckedNormal), 
			RadioButtonState.CheckedPressed => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.CheckedPressed), 
			RadioButtonState.UncheckedDisabled => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.UncheckedDisabled), 
			RadioButtonState.UncheckedHot => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.UncheckedHot), 
			RadioButtonState.UncheckedPressed => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.UncheckedPressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.Button.RadioButton.UncheckedNormal), 
		};
	}
}
