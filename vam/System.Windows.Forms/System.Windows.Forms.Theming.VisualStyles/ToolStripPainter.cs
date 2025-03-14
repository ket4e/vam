using System.Drawing;
using System.Windows.Forms.Theming.Default;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Theming.VisualStyles;

internal class ToolStripPainter : System.Windows.Forms.Theming.Default.ToolStripPainter
{
	private static bool IsDisabled(ToolStripItem toolStripItem)
	{
		return !toolStripItem.Enabled;
	}

	private static bool IsPressed(ToolStripItem toolStripItem)
	{
		return toolStripItem.Pressed;
	}

	private static bool IsChecked(ToolStripItem toolStripItem)
	{
		if (!(toolStripItem is ToolStripButton toolStripButton))
		{
			return false;
		}
		return toolStripButton.Checked;
	}

	private static bool IsHot(ToolStripItem toolStripItem)
	{
		return toolStripItem.Selected;
	}

	public override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.OnRenderButtonBackground(e);
			return;
		}
		VisualStyleElement element = (IsDisabled(e.Item) ? VisualStyleElement.ToolBar.Button.Disabled : (IsPressed(e.Item) ? VisualStyleElement.ToolBar.Button.Pressed : (IsChecked(e.Item) ? ((!IsHot(e.Item)) ? VisualStyleElement.ToolBar.Button.Checked : VisualStyleElement.ToolBar.Button.HotChecked) : ((!IsHot(e.Item)) ? VisualStyleElement.ToolBar.Button.Normal : VisualStyleElement.ToolBar.Button.Hot))));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.OnRenderButtonBackground(e);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(e.Graphics, e.Item.Bounds);
		}
	}

	public override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.OnRenderDropDownButtonBackground(e);
			return;
		}
		VisualStyleElement element = (IsDisabled(e.Item) ? VisualStyleElement.ToolBar.DropDownButton.Disabled : (IsPressed(e.Item) ? VisualStyleElement.ToolBar.DropDownButton.Pressed : (IsChecked(e.Item) ? ((!IsHot(e.Item)) ? VisualStyleElement.ToolBar.DropDownButton.Checked : VisualStyleElement.ToolBar.DropDownButton.HotChecked) : ((!IsHot(e.Item)) ? VisualStyleElement.ToolBar.DropDownButton.Normal : VisualStyleElement.ToolBar.DropDownButton.Hot))));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.OnRenderDropDownButtonBackground(e);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(e.Graphics, e.Item.Bounds);
		}
	}

	public override void OnRenderGrip(ToolStripGripRenderEventArgs e)
	{
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.OnRenderGrip(e);
		}
		else if (e.GripStyle != 0)
		{
			VisualStyleElement element = ((e.GripDisplayStyle != ToolStripGripDisplayStyle.Vertical) ? VisualStyleElement.Rebar.GripperVertical.Normal : VisualStyleElement.Rebar.Gripper.Normal);
			if (!VisualStyleRenderer.IsElementDefined(element))
			{
				base.OnRenderGrip(e);
			}
			else
			{
				new VisualStyleRenderer(element).DrawBackground(e.Graphics, (e.GripDisplayStyle != ToolStripGripDisplayStyle.Vertical) ? new Rectangle(0, 2, 20, 5) : new Rectangle(2, 0, 5, 20));
			}
		}
	}

	public override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.OnRenderOverflowButtonBackground(e);
			return;
		}
		VisualStyleElement element = ((e.ToolStrip.Orientation != 0) ? VisualStyleElement.Rebar.ChevronVertical.Normal : VisualStyleElement.Rebar.Chevron.Normal);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.OnRenderOverflowButtonBackground(e);
			return;
		}
		OnRenderButtonBackground(e);
		new VisualStyleRenderer(element).DrawBackground(e.Graphics, e.Item.Bounds);
	}

	public override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.OnRenderSeparator(e);
			return;
		}
		VisualStyleElement element = ((e.ToolStrip.Orientation != 0) ? VisualStyleElement.ToolBar.SeparatorVertical.Normal : VisualStyleElement.ToolBar.SeparatorHorizontal.Normal);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.OnRenderSeparator(e);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(e.Graphics, e.Item.Bounds);
		}
	}

	public override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.OnRenderSplitButtonBackground(e);
			return;
		}
		VisualStyleElement element;
		VisualStyleElement visualStyleElement;
		if (IsDisabled(e.Item))
		{
			element = VisualStyleElement.ToolBar.SplitButton.Disabled;
			visualStyleElement = VisualStyleElement.ToolBar.SplitButtonDropDown.Disabled;
		}
		else if (IsPressed(e.Item))
		{
			element = VisualStyleElement.ToolBar.SplitButton.Pressed;
			visualStyleElement = VisualStyleElement.ToolBar.SplitButtonDropDown.Pressed;
		}
		else if (IsChecked(e.Item))
		{
			if (IsHot(e.Item))
			{
				element = VisualStyleElement.ToolBar.SplitButton.HotChecked;
				visualStyleElement = VisualStyleElement.ToolBar.SplitButtonDropDown.HotChecked;
			}
			else
			{
				element = VisualStyleElement.ToolBar.Button.Checked;
				visualStyleElement = VisualStyleElement.ToolBar.SplitButtonDropDown.Checked;
			}
		}
		else if (IsHot(e.Item))
		{
			element = VisualStyleElement.ToolBar.SplitButton.Hot;
			visualStyleElement = VisualStyleElement.ToolBar.SplitButtonDropDown.Hot;
		}
		else
		{
			element = VisualStyleElement.ToolBar.SplitButton.Normal;
			visualStyleElement = VisualStyleElement.ToolBar.SplitButtonDropDown.Normal;
		}
		if (!VisualStyleRenderer.IsElementDefined(element) || !VisualStyleRenderer.IsElementDefined(visualStyleElement))
		{
			base.OnRenderSplitButtonBackground(e);
			return;
		}
		ToolStripSplitButton toolStripSplitButton = (ToolStripSplitButton)e.Item;
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(element);
		visualStyleRenderer.DrawBackground(e.Graphics, toolStripSplitButton.ButtonBounds);
		visualStyleRenderer.SetParameters(visualStyleElement);
		visualStyleRenderer.DrawBackground(e.Graphics, toolStripSplitButton.DropDownButtonBounds);
	}

	public override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip.BackgroundImage != null)
		{
			return;
		}
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.OnRenderToolStripBackground(e);
			return;
		}
		VisualStyleElement element = ((!(e.ToolStrip is StatusStrip)) ? VisualStyleElement.Rebar.Band.Normal : VisualStyleElement.Status.Bar.Normal);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.OnRenderToolStripBackground(e);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(e.Graphics, e.ToolStrip.Bounds, e.AffectedBounds);
		}
	}
}
