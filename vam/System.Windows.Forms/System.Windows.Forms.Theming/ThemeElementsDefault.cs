using System.Windows.Forms.Theming.Default;

namespace System.Windows.Forms.Theming;

internal class ThemeElementsDefault
{
	protected TabControlPainter tabControlPainter;

	protected ButtonPainter buttonPainter;

	protected LabelPainter labelPainter;

	protected LinkLabelPainter linklabelPainter;

	protected ToolStripPainter toolStripPainter;

	protected CheckBoxPainter checkBoxPainter;

	protected RadioButtonPainter radioButtonPainter;

	public virtual TabControlPainter TabControlPainter
	{
		get
		{
			if (tabControlPainter == null)
			{
				tabControlPainter = new TabControlPainter();
			}
			return tabControlPainter;
		}
	}

	public virtual ButtonPainter ButtonPainter
	{
		get
		{
			if (buttonPainter == null)
			{
				buttonPainter = new ButtonPainter();
			}
			return buttonPainter;
		}
	}

	public virtual LabelPainter LabelPainter
	{
		get
		{
			if (labelPainter == null)
			{
				labelPainter = new LabelPainter();
			}
			return labelPainter;
		}
	}

	public virtual LinkLabelPainter LinkLabelPainter
	{
		get
		{
			if (linklabelPainter == null)
			{
				linklabelPainter = new LinkLabelPainter();
			}
			return linklabelPainter;
		}
	}

	public virtual ToolStripPainter ToolStripPainter
	{
		get
		{
			if (toolStripPainter == null)
			{
				toolStripPainter = new ToolStripPainter();
			}
			return toolStripPainter;
		}
	}

	public virtual CheckBoxPainter CheckBoxPainter
	{
		get
		{
			if (checkBoxPainter == null)
			{
				checkBoxPainter = new CheckBoxPainter();
			}
			return checkBoxPainter;
		}
	}

	public virtual RadioButtonPainter RadioButtonPainter
	{
		get
		{
			if (radioButtonPainter == null)
			{
				radioButtonPainter = new RadioButtonPainter();
			}
			return radioButtonPainter;
		}
	}
}
