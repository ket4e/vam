using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class CheckBoxRenderer
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

	private CheckBoxRenderer()
	{
	}

	public static void DrawCheckBox(Graphics g, Point glyphLocation, CheckBoxState state)
	{
		DrawCheckBox(g, glyphLocation, Rectangle.Empty, string.Empty, null, TextFormatFlags.HorizontalCenter, null, Rectangle.Empty, focused: false, state);
	}

	public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, bool focused, CheckBoxState state)
	{
		DrawCheckBox(g, glyphLocation, textBounds, checkBoxText, font, TextFormatFlags.HorizontalCenter, null, Rectangle.Empty, focused, state);
	}

	public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, TextFormatFlags flags, bool focused, CheckBoxState state)
	{
		DrawCheckBox(g, glyphLocation, textBounds, checkBoxText, font, flags, null, Rectangle.Empty, focused, state);
	}

	public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, Image image, Rectangle imageBounds, bool focused, CheckBoxState state)
	{
		DrawCheckBox(g, glyphLocation, textBounds, checkBoxText, font, TextFormatFlags.HorizontalCenter, image, imageBounds, focused, state);
	}

	public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, CheckBoxState state)
	{
		Rectangle rectangle = new Rectangle(glyphLocation, GetGlyphSize(g, state));
		if (Application.RenderWithVisualStyles || always_use_visual_styles)
		{
			VisualStyleRenderer checkBoxRenderer = GetCheckBoxRenderer(state);
			checkBoxRenderer.DrawBackground(g, rectangle);
			if (image != null)
			{
				checkBoxRenderer.DrawImage(g, imageBounds, image);
			}
			if (focused)
			{
				ControlPaint.DrawFocusRectangle(g, textBounds);
			}
			if (checkBoxText != string.Empty)
			{
				if (state == CheckBoxState.CheckedDisabled || state == CheckBoxState.MixedDisabled || state == CheckBoxState.UncheckedDisabled)
				{
					TextRenderer.DrawText(g, checkBoxText, font, textBounds, SystemColors.GrayText, flags);
				}
				else
				{
					TextRenderer.DrawText(g, checkBoxText, font, textBounds, SystemColors.ControlText, flags);
				}
			}
			return;
		}
		switch (state)
		{
		case CheckBoxState.CheckedDisabled:
		case CheckBoxState.MixedPressed:
		case CheckBoxState.MixedDisabled:
			ControlPaint.DrawCheckBox(g, rectangle, ButtonState.Inactive | ButtonState.Checked);
			break;
		case CheckBoxState.CheckedNormal:
		case CheckBoxState.CheckedHot:
			ControlPaint.DrawCheckBox(g, rectangle, ButtonState.Checked);
			break;
		case CheckBoxState.CheckedPressed:
			ControlPaint.DrawCheckBox(g, rectangle, ButtonState.Pushed | ButtonState.Checked);
			break;
		case CheckBoxState.MixedNormal:
		case CheckBoxState.MixedHot:
			ControlPaint.DrawMixedCheckBox(g, rectangle, ButtonState.Checked);
			break;
		case CheckBoxState.UncheckedPressed:
		case CheckBoxState.UncheckedDisabled:
			ControlPaint.DrawCheckBox(g, rectangle, ButtonState.Inactive);
			break;
		case CheckBoxState.UncheckedNormal:
		case CheckBoxState.UncheckedHot:
			ControlPaint.DrawCheckBox(g, rectangle, ButtonState.Normal);
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
		if (checkBoxText != string.Empty)
		{
			TextRenderer.DrawText(g, checkBoxText, font, textBounds, SystemColors.ControlText, flags);
		}
	}

	public static bool IsBackgroundPartiallyTransparent(CheckBoxState state)
	{
		if (!VisualStyleRenderer.IsSupported)
		{
			return false;
		}
		VisualStyleRenderer checkBoxRenderer = GetCheckBoxRenderer(state);
		return checkBoxRenderer.IsBackgroundPartiallyTransparent();
	}

	public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
	{
		if (VisualStyleRenderer.IsSupported)
		{
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedNormal);
			visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
		}
	}

	public static Size GetGlyphSize(Graphics g, CheckBoxState state)
	{
		if (!VisualStyleRenderer.IsSupported)
		{
			return new Size(13, 13);
		}
		VisualStyleRenderer checkBoxRenderer = GetCheckBoxRenderer(state);
		return checkBoxRenderer.GetPartSize(g, ThemeSizeType.Draw);
	}

	private static VisualStyleRenderer GetCheckBoxRenderer(CheckBoxState state)
	{
		return state switch
		{
			CheckBoxState.CheckedDisabled => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.CheckedDisabled), 
			CheckBoxState.CheckedHot => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.CheckedHot), 
			CheckBoxState.CheckedNormal => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.CheckedNormal), 
			CheckBoxState.CheckedPressed => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.CheckedPressed), 
			CheckBoxState.MixedDisabled => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.MixedDisabled), 
			CheckBoxState.MixedHot => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.MixedHot), 
			CheckBoxState.MixedNormal => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.MixedNormal), 
			CheckBoxState.MixedPressed => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.MixedPressed), 
			CheckBoxState.UncheckedDisabled => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedDisabled), 
			CheckBoxState.UncheckedHot => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedHot), 
			CheckBoxState.UncheckedPressed => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedPressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedNormal), 
		};
	}
}
