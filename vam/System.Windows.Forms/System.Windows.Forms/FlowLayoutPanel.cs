using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.FlowLayoutPanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ProvideProperty("FlowBreak", typeof(Control))]
[ComVisible(true)]
[DefaultProperty("FlowDirection")]
[Docking(DockingBehavior.Ask)]
public class FlowLayoutPanel : Panel, IExtenderProvider
{
	private FlowLayoutSettings settings;

	[DefaultValue(FlowDirection.LeftToRight)]
	[Localizable(true)]
	public FlowDirection FlowDirection
	{
		get
		{
			return LayoutSettings.FlowDirection;
		}
		set
		{
			LayoutSettings.FlowDirection = value;
		}
	}

	[DefaultValue(true)]
	[Localizable(true)]
	public bool WrapContents
	{
		get
		{
			return LayoutSettings.WrapContents;
		}
		set
		{
			LayoutSettings.WrapContents = value;
		}
	}

	public override LayoutEngine LayoutEngine => LayoutSettings.LayoutEngine;

	internal FlowLayoutSettings LayoutSettings
	{
		get
		{
			if (settings == null)
			{
				settings = new FlowLayoutSettings(this);
			}
			return settings;
		}
	}

	bool IExtenderProvider.CanExtend(object obj)
	{
		if (obj is Control && (obj as Control).Parent == this)
		{
			return true;
		}
		return false;
	}

	[DisplayName("FlowBreak")]
	[DefaultValue(false)]
	public bool GetFlowBreak(Control control)
	{
		return LayoutSettings.GetFlowBreak(control);
	}

	[DisplayName("FlowBreak")]
	public void SetFlowBreak(Control control, bool value)
	{
		LayoutSettings.SetFlowBreak(control, value);
	}

	internal override void CalculateCanvasSize(bool canOverride)
	{
		if (canOverride)
		{
			canvas_size = base.ClientSize;
		}
		else
		{
			base.CalculateCanvasSize(canOverride);
		}
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		int num = 0;
		int num2 = 0;
		bool flag = FlowDirection == FlowDirection.LeftToRight || FlowDirection == FlowDirection.RightToLeft;
		if (!WrapContents || (flag && proposedSize.Width == 0) || (!flag && proposedSize.Height == 0))
		{
			foreach (Control control3 in base.Controls)
			{
				Size size = ((!control3.AutoSize) ? control3.Size : control3.PreferredSize);
				Padding padding = control3.Margin;
				if (flag)
				{
					num += size.Width + padding.Horizontal;
					num2 = Math.Max(num2, size.Height + padding.Vertical);
				}
				else
				{
					num2 += size.Height + padding.Vertical;
					num = Math.Max(num, size.Width + padding.Horizontal);
				}
			}
		}
		else
		{
			int num3 = 0;
			int num4 = 0;
			foreach (Control control4 in base.Controls)
			{
				Size size2 = ((!control4.AutoSize) ? control4.ExplicitBounds.Size : control4.PreferredSize);
				Padding padding2 = control4.Margin;
				if (flag)
				{
					int num5 = size2.Width + padding2.Horizontal;
					if (num3 != 0 && num3 + num5 >= proposedSize.Width)
					{
						num = Math.Max(num, num3);
						num3 = 0;
						num2 += num4;
						num4 = 0;
					}
					num3 += num5;
					num4 = Math.Max(num4, size2.Height + padding2.Vertical);
				}
				else
				{
					int num5 = size2.Height + padding2.Vertical;
					if (num3 != 0 && num3 + num5 >= proposedSize.Height)
					{
						num2 = Math.Max(num2, num3);
						num3 = 0;
						num += num4;
						num4 = 0;
					}
					num3 += num5;
					num4 = Math.Max(num4, size2.Width + padding2.Horizontal);
				}
			}
			if (flag)
			{
				num = Math.Max(num, num3);
				num2 += num4;
			}
			else
			{
				num2 = Math.Max(num2, num3);
				num += num4;
			}
		}
		return new Size(num, num2);
	}
}
