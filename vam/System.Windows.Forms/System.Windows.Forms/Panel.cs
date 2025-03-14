using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.PanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("BorderStyle")]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Docking(DockingBehavior.Ask)]
[DefaultEvent("Paint")]
public class Panel : ScrollableControl
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public override bool AutoSize
	{
		get
		{
			return base.AutoSize;
		}
		set
		{
			base.AutoSize = value;
		}
	}

	[DefaultValue(AutoSizeMode.GrowOnly)]
	[Browsable(true)]
	[Localizable(true)]
	public virtual AutoSizeMode AutoSizeMode
	{
		get
		{
			return GetAutoSizeMode();
		}
		set
		{
			SetAutoSizeMode(value);
		}
	}

	[DispId(-504)]
	[DefaultValue(BorderStyle.None)]
	public BorderStyle BorderStyle
	{
		get
		{
			return base.InternalBorderStyle;
		}
		set
		{
			base.InternalBorderStyle = value;
		}
	}

	[DefaultValue(false)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			if (value != TabStop)
			{
				base.TabStop = value;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[Bindable(false)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			if (!(value == Text))
			{
				base.Text = value;
				Refresh();
			}
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => ThemeEngine.Current.PanelDefaultSize;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public new event EventHandler AutoSizeChanged
	{
		add
		{
			base.AutoSizeChanged += value;
		}
		remove
		{
			base.AutoSizeChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyEventHandler KeyDown
	{
		add
		{
			base.KeyDown += value;
		}
		remove
		{
			base.KeyDown -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyPressEventHandler KeyPress
	{
		add
		{
			base.KeyPress += value;
		}
		remove
		{
			base.KeyPress -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event KeyEventHandler KeyUp
	{
		add
		{
			base.KeyUp += value;
		}
		remove
		{
			base.KeyUp -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler TextChanged
	{
		add
		{
			base.TextChanged += value;
		}
		remove
		{
			base.TextChanged -= value;
		}
	}

	public Panel()
	{
		base.TabStop = false;
		SetStyle(ControlStyles.Selectable, value: false);
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
	}

	public override string ToString()
	{
		return base.ToString() + ", BorderStyle: " + BorderStyle;
	}

	protected override void OnResize(EventArgs eventargs)
	{
		base.OnResize(eventargs);
		Invalidate(invalidateChildren: true);
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		Size empty = Size.Empty;
		foreach (Control control in base.Controls)
		{
			if (control.Dock == DockStyle.Fill)
			{
				if (control.Bounds.Right > empty.Width)
				{
					empty.Width = control.Bounds.Right;
				}
			}
			else if (control.Dock != DockStyle.Top && control.Dock != DockStyle.Bottom && (control.Anchor & AnchorStyles.Right) == 0 && control.Bounds.Right + control.Margin.Right > empty.Width)
			{
				empty.Width = control.Bounds.Right + control.Margin.Right;
			}
			if (control.Dock == DockStyle.Fill)
			{
				if (control.Bounds.Bottom > empty.Height)
				{
					empty.Height = control.Bounds.Bottom;
				}
			}
			else if (control.Dock != DockStyle.Left && control.Dock != DockStyle.Right && (control.Anchor & AnchorStyles.Bottom) == 0 && control.Bounds.Bottom + control.Margin.Bottom > empty.Height)
			{
				empty.Height = control.Bounds.Bottom + control.Margin.Bottom;
			}
		}
		return empty;
	}
}
