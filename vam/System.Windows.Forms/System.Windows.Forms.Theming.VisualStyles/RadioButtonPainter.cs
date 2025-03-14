using System.Drawing;
using System.Windows.Forms.Theming.Default;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Theming.VisualStyles;

internal class RadioButtonPainter : System.Windows.Forms.Theming.Default.RadioButtonPainter
{
	public override void DrawNormalRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		DrawRadioButton(g, bounds, (!isChecked) ? RadioButtonState.UncheckedNormal : RadioButtonState.CheckedNormal);
	}

	public override void DrawHotRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		DrawRadioButton(g, bounds, (!isChecked) ? RadioButtonState.UncheckedHot : RadioButtonState.CheckedHot);
	}

	public override void DrawPressedRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		DrawRadioButton(g, bounds, (!isChecked) ? RadioButtonState.UncheckedPressed : RadioButtonState.CheckedPressed);
	}

	public override void DrawDisabledRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		DrawRadioButton(g, bounds, (!isChecked) ? RadioButtonState.UncheckedDisabled : RadioButtonState.CheckedDisabled);
	}

	private static void DrawRadioButton(Graphics g, Rectangle bounds, RadioButtonState state)
	{
		RadioButtonRenderer.DrawRadioButton(g, bounds.Location, state);
	}
}
