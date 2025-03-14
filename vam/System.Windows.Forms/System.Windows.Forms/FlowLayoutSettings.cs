using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[DefaultProperty("FlowDirection")]
public class FlowLayoutSettings : LayoutSettings
{
	private FlowDirection flow_direction;

	private bool wrap_contents;

	private LayoutEngine layout_engine;

	private Dictionary<object, bool> flow_breaks;

	private Control owner;

	[DefaultValue(FlowDirection.LeftToRight)]
	public FlowDirection FlowDirection
	{
		get
		{
			return flow_direction;
		}
		set
		{
			if (flow_direction != value)
			{
				flow_direction = value;
				if (owner != null)
				{
					owner.PerformLayout(owner, "FlowDirection");
				}
			}
		}
	}

	public override LayoutEngine LayoutEngine
	{
		get
		{
			if (layout_engine == null)
			{
				layout_engine = new FlowLayout();
			}
			return layout_engine;
		}
	}

	[DefaultValue(true)]
	public bool WrapContents
	{
		get
		{
			return wrap_contents;
		}
		set
		{
			if (wrap_contents != value)
			{
				wrap_contents = value;
				if (owner != null)
				{
					owner.PerformLayout(owner, "WrapContents");
				}
			}
		}
	}

	internal FlowLayoutSettings()
		: this(null)
	{
	}

	internal FlowLayoutSettings(Control owner)
	{
		flow_breaks = new Dictionary<object, bool>();
		wrap_contents = true;
		flow_direction = FlowDirection.LeftToRight;
		this.owner = owner;
	}

	public bool GetFlowBreak(object child)
	{
		if (flow_breaks.TryGetValue(child, out var value))
		{
			return value;
		}
		return false;
	}

	public void SetFlowBreak(object child, bool value)
	{
		flow_breaks[child] = value;
		if (owner != null)
		{
			owner.PerformLayout((Control)child, "FlowBreak");
		}
	}
}
