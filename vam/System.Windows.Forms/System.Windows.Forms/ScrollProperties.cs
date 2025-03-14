using System.ComponentModel;

namespace System.Windows.Forms;

public abstract class ScrollProperties
{
	private ScrollableControl parentControl;

	internal ScrollBar scroll_bar;

	[DefaultValue(true)]
	public bool Enabled
	{
		get
		{
			return scroll_bar.Enabled;
		}
		set
		{
			scroll_bar.Enabled = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(10)]
	public int LargeChange
	{
		get
		{
			return scroll_bar.LargeChange;
		}
		set
		{
			scroll_bar.LargeChange = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(100)]
	public int Maximum
	{
		get
		{
			return scroll_bar.Maximum;
		}
		set
		{
			scroll_bar.Maximum = value;
		}
	}

	[DefaultValue(0)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public int Minimum
	{
		get
		{
			return scroll_bar.Minimum;
		}
		set
		{
			scroll_bar.Minimum = value;
		}
	}

	[DefaultValue(1)]
	public int SmallChange
	{
		get
		{
			return scroll_bar.SmallChange;
		}
		set
		{
			scroll_bar.SmallChange = value;
		}
	}

	[Bindable(true)]
	[DefaultValue(0)]
	public int Value
	{
		get
		{
			return scroll_bar.Value;
		}
		set
		{
			scroll_bar.Value = value;
		}
	}

	[DefaultValue(false)]
	public bool Visible
	{
		get
		{
			return scroll_bar.Visible;
		}
		set
		{
			scroll_bar.Visible = value;
		}
	}

	protected ScrollableControl ParentControl => parentControl;

	protected ScrollProperties(ScrollableControl container)
	{
		parentControl = container;
	}
}
