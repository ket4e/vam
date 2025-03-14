using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("Load")]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner))]
[Designer("System.Windows.Forms.Design.ControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DesignerCategory("UserControl")]
public class UserControl : ContainerControl
{
	private static object LoadEvent;

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

	[Browsable(true)]
	[Localizable(true)]
	[DefaultValue(AutoSizeMode.GrowOnly)]
	public AutoSizeMode AutoSizeMode
	{
		get
		{
			return GetAutoSizeMode();
		}
		set
		{
			if (GetAutoSizeMode() != value)
			{
				SetAutoSizeMode(value);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public override AutoValidate AutoValidate
	{
		get
		{
			return base.AutoValidate;
		}
		set
		{
			base.AutoValidate = value;
		}
	}

	protected override Size DefaultSize => new Size(150, 150);

	[Bindable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = base.CreateParams;
			createParams.Style |= 65536;
			createParams.ExStyle |= 65536;
			return createParams;
		}
	}

	[DefaultValue(BorderStyle.None)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
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

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new event EventHandler AutoValidateChanged
	{
		add
		{
			base.AutoValidateChanged += value;
		}
		remove
		{
			base.AutoValidateChanged -= value;
		}
	}

	public event EventHandler Load
	{
		add
		{
			base.Events.AddHandler(LoadEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LoadEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	public UserControl()
	{
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
	}

	static UserControl()
	{
		Load = new object();
	}

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public override bool ValidateChildren()
	{
		return base.ValidateChildren();
	}

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public override bool ValidateChildren(ValidationConstraints validationConstraints)
	{
		return base.ValidateChildren(validationConstraints);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		OnLoad(EventArgs.Empty);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnLoad(EventArgs e)
	{
		((EventHandler)base.Events[Load])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void WndProc(ref Message m)
	{
		Msg msg = (Msg)m.Msg;
		if (msg == Msg.WM_SETFOCUS)
		{
			if (ActiveControl == null)
			{
				SelectNextControl(null, forward: true, tabStopOnly: true, nested: true, wrap: false);
			}
			base.WndProc(ref m);
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		Size empty = Size.Empty;
		foreach (Control control3 in base.Controls)
		{
			if (control3.is_visible)
			{
				if (control3.Dock == DockStyle.Left || control3.Dock == DockStyle.Right)
				{
					empty.Width += control3.PreferredSize.Width;
				}
				else if (control3.Dock == DockStyle.Top || control3.Dock == DockStyle.Bottom)
				{
					empty.Height += control3.PreferredSize.Height;
				}
			}
		}
		foreach (Control control4 in base.Controls)
		{
			if (control4.is_visible && control4.Dock == DockStyle.None && (control4.Anchor & AnchorStyles.Bottom) != AnchorStyles.Bottom && (control4.Anchor & AnchorStyles.Right) != AnchorStyles.Right)
			{
				empty.Width = Math.Max(empty.Width, control4.Bounds.Right + control4.Margin.Right);
				empty.Height = Math.Max(empty.Height, control4.Bounds.Bottom + control4.Margin.Bottom);
			}
		}
		return empty;
	}
}
