using System.Windows.Forms.Theming.Default;
using System.Windows.Forms.Theming.VisualStyles;

namespace System.Windows.Forms.Theming;

internal class ThemeElementsVisualStyles : ThemeElementsDefault
{
	public override System.Windows.Forms.Theming.Default.CheckBoxPainter CheckBoxPainter
	{
		get
		{
			if (checkBoxPainter == null)
			{
				checkBoxPainter = new System.Windows.Forms.Theming.VisualStyles.CheckBoxPainter();
			}
			return checkBoxPainter;
		}
	}

	public override System.Windows.Forms.Theming.Default.RadioButtonPainter RadioButtonPainter
	{
		get
		{
			if (radioButtonPainter == null)
			{
				radioButtonPainter = new System.Windows.Forms.Theming.VisualStyles.RadioButtonPainter();
			}
			return radioButtonPainter;
		}
	}

	public override System.Windows.Forms.Theming.Default.ToolStripPainter ToolStripPainter
	{
		get
		{
			if (toolStripPainter == null)
			{
				toolStripPainter = new System.Windows.Forms.Theming.VisualStyles.ToolStripPainter();
			}
			return toolStripPainter;
		}
	}

	public override System.Windows.Forms.Theming.Default.TabControlPainter TabControlPainter
	{
		get
		{
			if (tabControlPainter == null)
			{
				tabControlPainter = new System.Windows.Forms.Theming.VisualStyles.TabControlPainter();
			}
			return tabControlPainter;
		}
	}
}
