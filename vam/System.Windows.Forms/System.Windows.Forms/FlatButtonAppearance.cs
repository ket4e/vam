using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[TypeConverter(typeof(FlatButtonAppearanceConverter))]
public class FlatButtonAppearance
{
	private Color borderColor = Color.Empty;

	private int borderSize = 1;

	private Color checkedBackColor = Color.Empty;

	private Color mouseDownBackColor = Color.Empty;

	private Color mouseOverBackColor = Color.Empty;

	private ButtonBase owner;

	[Browsable(true)]
	[NotifyParentProperty(true)]
	[DefaultValue(typeof(Color), "")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public Color BorderColor
	{
		get
		{
			return borderColor;
		}
		set
		{
			if (!(borderColor == value))
			{
				if (value == Color.Transparent)
				{
					throw new NotSupportedException("Cannot have a Transparent border.");
				}
				borderColor = value;
				if (owner != null)
				{
					owner.Invalidate();
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[DefaultValue(1)]
	[NotifyParentProperty(true)]
	[Browsable(true)]
	public int BorderSize
	{
		get
		{
			return borderSize;
		}
		set
		{
			if (borderSize != value)
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", $"'{value}' is not a valid value for 'BorderSize'. 'BorderSize' must be greater or equal than {0}.");
				}
				borderSize = value;
				if (owner != null)
				{
					owner.Invalidate();
				}
			}
		}
	}

	[DefaultValue(typeof(Color), "")]
	[Browsable(true)]
	[NotifyParentProperty(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public Color CheckedBackColor
	{
		get
		{
			return checkedBackColor;
		}
		set
		{
			if (!(checkedBackColor == value))
			{
				checkedBackColor = value;
				if (owner != null)
				{
					owner.Invalidate();
				}
			}
		}
	}

	[DefaultValue(typeof(Color), "")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[NotifyParentProperty(true)]
	[Browsable(true)]
	public Color MouseDownBackColor
	{
		get
		{
			return mouseDownBackColor;
		}
		set
		{
			if (!(mouseDownBackColor == value))
			{
				mouseDownBackColor = value;
				if (owner != null)
				{
					owner.Invalidate();
				}
			}
		}
	}

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[DefaultValue(typeof(Color), "")]
	[NotifyParentProperty(true)]
	public Color MouseOverBackColor
	{
		get
		{
			return mouseOverBackColor;
		}
		set
		{
			if (!(mouseOverBackColor == value))
			{
				mouseOverBackColor = value;
				if (owner != null)
				{
					owner.Invalidate();
				}
			}
		}
	}

	internal FlatButtonAppearance(ButtonBase owner)
	{
		this.owner = owner;
	}
}
