using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultProperty("Text")]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.GroupBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("Enter")]
public class GroupBox : Control
{
	private class GroupBoxAccessibleObject : ControlAccessibleObject
	{
		public GroupBoxAccessibleObject(Control owner)
			: base(owner)
		{
		}
	}

	private FlatStyle flat_style;

	private Rectangle display_rectangle = default(Rectangle);

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public override bool AllowDrop
	{
		get
		{
			return base.AllowDrop;
		}
		set
		{
			base.AllowDrop = value;
		}
	}

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
	public AutoSizeMode AutoSizeMode
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

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => ThemeEngine.Current.GroupBoxDefaultSize;

	public override Rectangle DisplayRectangle
	{
		get
		{
			display_rectangle.X = base.Padding.Left;
			display_rectangle.Y = Font.Height + base.Padding.Top;
			display_rectangle.Width = base.Width - base.Padding.Horizontal;
			display_rectangle.Height = base.Height - Font.Height - base.Padding.Vertical;
			return display_rectangle;
		}
	}

	[DefaultValue(FlatStyle.Standard)]
	public FlatStyle FlatStyle
	{
		get
		{
			return flat_style;
		}
		set
		{
			if (!Enum.IsDefined(typeof(FlatStyle), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for FlatStyle");
			}
			if (flat_style != value)
			{
				flat_style = value;
				Refresh();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			base.TabStop = value;
		}
	}

	[Localizable(true)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			if (!(base.Text == value))
			{
				base.Text = value;
				Refresh();
			}
		}
	}

	[DefaultValue(false)]
	public bool UseCompatibleTextRendering
	{
		get
		{
			return use_compatible_text_rendering;
		}
		set
		{
			if (use_compatible_text_rendering != value)
			{
				use_compatible_text_rendering = value;
				if (base.Parent != null)
				{
					base.Parent.PerformLayout(this, "UseCompatibleTextRendering");
				}
				Invalidate();
			}
		}
	}

	protected override Padding DefaultPadding => new Padding(3);

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
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event EventHandler Click
	{
		add
		{
			base.Click += value;
		}
		remove
		{
			base.Click -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event EventHandler DoubleClick
	{
		add
		{
			base.DoubleClick += value;
		}
		remove
		{
			base.DoubleClick -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
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
	[EditorBrowsable(EditorBrowsableState.Advanced)]
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

	[EditorBrowsable(EditorBrowsableState.Advanced)]
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

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public new event MouseEventHandler MouseClick
	{
		add
		{
			base.MouseClick += value;
		}
		remove
		{
			base.MouseClick -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event MouseEventHandler MouseDoubleClick
	{
		add
		{
			base.MouseDoubleClick += value;
		}
		remove
		{
			base.MouseDoubleClick -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event MouseEventHandler MouseDown
	{
		add
		{
			base.MouseDown += value;
		}
		remove
		{
			base.MouseDown -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event EventHandler MouseEnter
	{
		add
		{
			base.MouseEnter += value;
		}
		remove
		{
			base.MouseEnter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public new event EventHandler MouseLeave
	{
		add
		{
			base.MouseLeave += value;
		}
		remove
		{
			base.MouseLeave -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event MouseEventHandler MouseMove
	{
		add
		{
			base.MouseMove += value;
		}
		remove
		{
			base.MouseMove -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event MouseEventHandler MouseUp
	{
		add
		{
			base.MouseUp += value;
		}
		remove
		{
			base.MouseUp -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event EventHandler TabStopChanged
	{
		add
		{
			base.TabStopChanged += value;
		}
		remove
		{
			base.TabStopChanged -= value;
		}
	}

	public GroupBox()
	{
		TabStop = false;
		flat_style = FlatStyle.Standard;
		SetStyle(ControlStyles.ContainerControl | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, value: true);
		SetStyle(ControlStyles.Selectable, value: false);
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new GroupBoxAccessibleObject(this);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		Refresh();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		ThemeEngine.Current.DrawGroupBox(e.Graphics, base.ClientRectangle, this);
		base.OnPaint(e);
	}

	protected override bool ProcessMnemonic(char charCode)
	{
		if (Control.IsMnemonic(charCode, Text))
		{
			if (base.Parent != null)
			{
				base.Parent.SelectNextControl(this, forward: true, tabStopOnly: false, nested: true, wrap: false);
			}
			return true;
		}
		return base.ProcessMnemonic(charCode);
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		base.ScaleControl(factor, specified);
	}

	public override string ToString()
	{
		return GetType().FullName + ", Text: " + Text;
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		Size result = new Size(base.Padding.Left, base.Padding.Top);
		foreach (Control control in base.Controls)
		{
			if (control.Dock == DockStyle.Fill)
			{
				if (control.Bounds.Right > result.Width)
				{
					result.Width = control.Bounds.Right;
				}
			}
			else if (control.Dock != DockStyle.Top && control.Dock != DockStyle.Bottom && control.Bounds.Right + control.Margin.Right > result.Width)
			{
				result.Width = control.Bounds.Right + control.Margin.Right;
			}
			if (control.Dock == DockStyle.Fill)
			{
				if (control.Bounds.Bottom > result.Height)
				{
					result.Height = control.Bounds.Bottom;
				}
			}
			else if (control.Dock != DockStyle.Left && control.Dock != DockStyle.Right && control.Bounds.Bottom + control.Margin.Bottom > result.Height)
			{
				result.Height = control.Bounds.Bottom + control.Margin.Bottom;
			}
		}
		result.Width += base.Padding.Right;
		result.Height += base.Padding.Bottom;
		result.Height += Font.Height;
		return result;
	}
}
