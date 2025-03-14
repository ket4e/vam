using System.Drawing;
using System.Windows.Forms.Theming.Default;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Theming.VisualStyles;

internal class CheckBoxPainter : System.Windows.Forms.Theming.Default.CheckBoxPainter
{
	public override void DrawNormalCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawCheckBox(g, bounds, state switch
		{
			CheckState.Checked => CheckBoxState.CheckedNormal, 
			CheckState.Indeterminate => CheckBoxState.MixedNormal, 
			_ => CheckBoxState.UncheckedNormal, 
		});
	}

	public override void DrawHotCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawCheckBox(g, bounds, state switch
		{
			CheckState.Checked => CheckBoxState.CheckedHot, 
			CheckState.Indeterminate => CheckBoxState.MixedHot, 
			_ => CheckBoxState.UncheckedHot, 
		});
	}

	public override void DrawPressedCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawCheckBox(g, bounds, state switch
		{
			CheckState.Checked => CheckBoxState.CheckedPressed, 
			CheckState.Indeterminate => CheckBoxState.MixedPressed, 
			_ => CheckBoxState.UncheckedPressed, 
		});
	}

	public override void DrawDisabledCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawCheckBox(g, bounds, state switch
		{
			CheckState.Checked => CheckBoxState.CheckedDisabled, 
			CheckState.Indeterminate => CheckBoxState.MixedDisabled, 
			_ => CheckBoxState.UncheckedDisabled, 
		});
	}

	private static void DrawCheckBox(Graphics g, Rectangle bounds, CheckBoxState state)
	{
		CheckBoxRenderer.DrawCheckBox(g, bounds.Location, state);
	}
}
