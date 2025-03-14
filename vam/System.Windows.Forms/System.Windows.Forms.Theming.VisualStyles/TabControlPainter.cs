using System.Drawing;
using System.Windows.Forms.Theming.Default;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Theming.VisualStyles;

internal class TabControlPainter : System.Windows.Forms.Theming.Default.TabControlPainter
{
	private static bool ShouldPaint(TabControl tabControl)
	{
		return ThemeVisualStyles.RenderClientAreas && tabControl.Alignment == TabAlignment.Top && tabControl.DrawMode == TabDrawMode.Normal;
	}

	protected override void DrawBackground(Graphics dc, Rectangle area, TabControl tab)
	{
		if (!ShouldPaint(tab))
		{
			base.DrawBackground(dc, area, tab);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.Tab.Pane.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.DrawBackground(dc, area, tab);
			return;
		}
		Rectangle tabPanelRect = GetTabPanelRect(tab);
		if (tabPanelRect.IntersectsWith(area))
		{
			new VisualStyleRenderer(normal).DrawBackground(dc, tabPanelRect, area);
		}
	}

	protected override int DrawTab(Graphics dc, TabPage page, TabControl tab, Rectangle bounds, bool is_selected)
	{
		if (!ShouldPaint(tab))
		{
			return base.DrawTab(dc, page, tab, bounds, is_selected);
		}
		VisualStyleElement visualStyleElement = GetVisualStyleElement(tab, page, is_selected);
		if (!VisualStyleRenderer.IsElementDefined(visualStyleElement))
		{
			return base.DrawTab(dc, page, tab, bounds, is_selected);
		}
		new VisualStyleRenderer(visualStyleElement).DrawBackground(dc, bounds);
		bounds.Inflate(-(FocusRectSpacing.X + BorderThickness.X), -(FocusRectSpacing.Y + BorderThickness.Y));
		Rectangle rectangle = bounds;
		if (tab.ImageList != null && page.ImageIndex >= 0 && page.ImageIndex < tab.ImageList.Images.Count)
		{
			int y = bounds.Y + (bounds.Height - tab.ImageList.ImageSize.Height) / 2;
			tab.ImageList.Draw(dc, new Point(bounds.X, y), page.ImageIndex);
			int num = tab.ImageList.ImageSize.Width + 2;
			rectangle.X += num;
			rectangle.Width -= num;
		}
		if (page.Text != null)
		{
			dc.DrawString(page.Text, page.Font, SystemBrushes.ControlText, rectangle, DefaultFormatting);
		}
		if (tab.Focused && is_selected && tab.ShowFocusCues)
		{
			ControlPaint.DrawFocusRectangle(dc, bounds);
		}
		return 0;
	}

	private static VisualStyleElement GetVisualStyleElement(TabControl tabControl, TabPage tabPage, bool selected)
	{
		bool flag = tabPage.Row == tabControl.RowCount;
		int num = tabControl.TabPages.IndexOf(tabPage);
		bool flag2 = true;
		for (int i = tabControl.SliderPos; i < num; i++)
		{
			if (tabControl.TabPages[i].Row == tabPage.Row)
			{
				flag2 = false;
				break;
			}
		}
		bool flag3 = true;
		for (int i = num; i < tabControl.TabCount; i++)
		{
			if (tabControl.TabPages[i].Row == tabPage.Row)
			{
				flag3 = false;
				break;
			}
		}
		if (!tabPage.Enabled)
		{
			if (flag)
			{
				if (flag2)
				{
					if (flag3)
					{
						return VisualStyleElement.Tab.TopTabItem.Disabled;
					}
					return VisualStyleElement.Tab.TopTabItemLeftEdge.Disabled;
				}
				if (flag3)
				{
					return VisualStyleElement.Tab.TopTabItemRightEdge.Disabled;
				}
				return VisualStyleElement.Tab.TopTabItem.Disabled;
			}
			if (flag2)
			{
				if (flag3)
				{
					return VisualStyleElement.Tab.TabItem.Disabled;
				}
				return VisualStyleElement.Tab.TabItemLeftEdge.Disabled;
			}
			if (flag3)
			{
				return VisualStyleElement.Tab.TabItemRightEdge.Disabled;
			}
			return VisualStyleElement.Tab.TabItem.Disabled;
		}
		if (selected)
		{
			if (flag)
			{
				if (flag2)
				{
					if (flag3)
					{
						return VisualStyleElement.Tab.TopTabItem.Pressed;
					}
					return VisualStyleElement.Tab.TopTabItemLeftEdge.Pressed;
				}
				if (flag3)
				{
					return VisualStyleElement.Tab.TopTabItemRightEdge.Pressed;
				}
				return VisualStyleElement.Tab.TopTabItem.Pressed;
			}
			if (flag2)
			{
				if (flag3)
				{
					return VisualStyleElement.Tab.TabItem.Pressed;
				}
				return VisualStyleElement.Tab.TabItemLeftEdge.Pressed;
			}
			if (flag3)
			{
				return VisualStyleElement.Tab.TabItemRightEdge.Pressed;
			}
			return VisualStyleElement.Tab.TabItem.Pressed;
		}
		if (tabControl.EnteredTabPage == tabPage)
		{
			if (flag)
			{
				if (flag2)
				{
					if (flag3)
					{
						return VisualStyleElement.Tab.TopTabItem.Hot;
					}
					return VisualStyleElement.Tab.TopTabItemLeftEdge.Hot;
				}
				if (flag3)
				{
					return VisualStyleElement.Tab.TopTabItemRightEdge.Hot;
				}
				return VisualStyleElement.Tab.TopTabItem.Hot;
			}
			if (flag2)
			{
				if (flag3)
				{
					return VisualStyleElement.Tab.TabItem.Hot;
				}
				return VisualStyleElement.Tab.TabItemLeftEdge.Hot;
			}
			if (flag3)
			{
				return VisualStyleElement.Tab.TabItemRightEdge.Hot;
			}
			return VisualStyleElement.Tab.TabItem.Hot;
		}
		if (flag)
		{
			if (flag2)
			{
				if (flag3)
				{
					return VisualStyleElement.Tab.TopTabItemBothEdges.Normal;
				}
				return VisualStyleElement.Tab.TopTabItemLeftEdge.Normal;
			}
			if (flag3)
			{
				return VisualStyleElement.Tab.TopTabItemRightEdge.Normal;
			}
			return VisualStyleElement.Tab.TopTabItem.Normal;
		}
		if (flag2)
		{
			if (flag3)
			{
				return VisualStyleElement.Tab.TabItemBothEdges.Normal;
			}
			return VisualStyleElement.Tab.TabItemLeftEdge.Normal;
		}
		if (flag3)
		{
			return VisualStyleElement.Tab.TabItemRightEdge.Normal;
		}
		return VisualStyleElement.Tab.TabItem.Normal;
	}

	public override bool HasHotElementStyles(TabControl tabControl)
	{
		if (!ShouldPaint(tabControl))
		{
			return base.HasHotElementStyles(tabControl);
		}
		return true;
	}

	protected override void DrawScrollButton(Graphics dc, Rectangle bounds, Rectangle clippingArea, ScrollButton button, PushButtonState state)
	{
		if (!ThemeVisualStyles.RenderClientAreas)
		{
			base.DrawScrollButton(dc, bounds, clippingArea, button, state);
			return;
		}
		VisualStyleElement element = ((button == ScrollButton.Left) ? (state switch
		{
			PushButtonState.Hot => VisualStyleElement.Spin.DownHorizontal.Hot, 
			PushButtonState.Pressed => VisualStyleElement.Spin.DownHorizontal.Pressed, 
			_ => VisualStyleElement.Spin.DownHorizontal.Normal, 
		}) : (state switch
		{
			PushButtonState.Hot => VisualStyleElement.Spin.UpHorizontal.Hot, 
			PushButtonState.Pressed => VisualStyleElement.Spin.UpHorizontal.Pressed, 
			_ => VisualStyleElement.Spin.UpHorizontal.Normal, 
		}));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			element = ((button == ScrollButton.Left) ? (state switch
			{
				PushButtonState.Hot => VisualStyleElement.ScrollBar.ArrowButton.LeftHot, 
				PushButtonState.Pressed => VisualStyleElement.ScrollBar.ArrowButton.LeftPressed, 
				_ => VisualStyleElement.ScrollBar.ArrowButton.LeftNormal, 
			}) : (state switch
			{
				PushButtonState.Hot => VisualStyleElement.ScrollBar.ArrowButton.RightHot, 
				PushButtonState.Pressed => VisualStyleElement.ScrollBar.ArrowButton.RightPressed, 
				_ => VisualStyleElement.ScrollBar.ArrowButton.RightNormal, 
			}));
			if (!VisualStyleRenderer.IsElementDefined(element))
			{
				base.DrawScrollButton(dc, bounds, clippingArea, button, state);
				return;
			}
		}
		new VisualStyleRenderer(element).DrawBackground(dc, bounds, clippingArea);
	}
}
