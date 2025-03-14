using System.Windows.Forms.Theming;

namespace System.Windows.Forms;

public class ToolStripSystemRenderer : ToolStripRenderer
{
	protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderButtonBackground(e);
		base.OnRenderButtonBackground(e);
	}

	protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderDropDownButtonBackground(e);
		base.OnRenderDropDownButtonBackground(e);
	}

	protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderGrip(e);
		base.OnRenderGrip(e);
	}

	protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
	{
		base.OnRenderImageMargin(e);
	}

	protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
	{
		base.OnRenderItemBackground(e);
	}

	protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
	{
		base.OnRenderLabelBackground(e);
	}

	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderMenuItemBackground(e);
		base.OnRenderMenuItemBackground(e);
	}

	protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderOverflowButtonBackground(e);
		base.OnRenderOverflowButtonBackground(e);
	}

	protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderSeparator(e);
		base.OnRenderSeparator(e);
	}

	protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderSplitButtonBackground(e);
		base.OnRenderSplitButtonBackground(e);
	}

	protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderToolStripBackground(e);
		base.OnRenderToolStripBackground(e);
	}

	protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
	{
		ThemeElements.CurrentTheme.ToolStripPainter.OnRenderToolStripBorder(e);
		base.OnRenderToolStripBorder(e);
	}

	protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
	{
		base.OnRenderToolStripStatusLabelBackground(e);
	}
}
