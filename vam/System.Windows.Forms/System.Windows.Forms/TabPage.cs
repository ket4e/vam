using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.TabPageDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ComVisible(true)]
[ToolboxItem(false)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("Click")]
[DesignTimeVisible(false)]
[DefaultProperty("Text")]
public class TabPage : Panel
{
	[ComVisible(false)]
	public class TabPageControlCollection : ControlCollection
	{
		public TabPageControlCollection(TabPage owner)
			: base(owner)
		{
		}

		public override void Add(Control value)
		{
			base.Add(value);
		}
	}

	private int imageIndex = -1;

	private string imageKey;

	private string tooltip_text = string.Empty;

	private Rectangle tab_bounds;

	private int row;

	private bool use_visual_style_back_color;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Localizable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override AutoSizeMode AutoSizeMode
	{
		get
		{
			return base.AutoSizeMode;
		}
		set
		{
			base.AutoSizeMode = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DefaultValue("{Width=0, Height=0}")]
	public override Size MaximumSize
	{
		get
		{
			return base.MaximumSize;
		}
		set
		{
			base.MaximumSize = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Size MinimumSize
	{
		get
		{
			return base.MinimumSize;
		}
		set
		{
			base.MinimumSize = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Size PreferredSize => base.PreferredSize;

	[DefaultValue(false)]
	public bool UseVisualStyleBackColor
	{
		get
		{
			return use_visual_style_back_color;
		}
		set
		{
			use_visual_style_back_color = value;
		}
	}

	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			use_visual_style_back_color = false;
			base.BackColor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override AnchorStyles Anchor
	{
		get
		{
			return base.Anchor;
		}
		set
		{
			base.Anchor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override DockStyle Dock
	{
		get
		{
			return base.Dock;
		}
		set
		{
			base.Dock = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			base.Enabled = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[TypeConverter(typeof(ImageIndexConverter))]
	[Localizable(true)]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DefaultValue(-1)]
	public int ImageIndex
	{
		get
		{
			return imageIndex;
		}
		set
		{
			if (imageIndex != value)
			{
				imageIndex = value;
				UpdateOwner();
			}
		}
	}

	[TypeConverter(typeof(ImageKeyConverter))]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue("")]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string ImageKey
	{
		get
		{
			return imageKey;
		}
		set
		{
			imageKey = value;
			if (base.Parent is TabControl tabControl)
			{
				ImageIndex = tabControl.ImageList.Images.IndexOfKey(imageKey);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new int TabIndex
	{
		get
		{
			return base.TabIndex;
		}
		set
		{
			base.TabIndex = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Localizable(true)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			if (!(value == base.Text))
			{
				base.Text = value;
				UpdateOwner();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue("")]
	public string ToolTipText
	{
		get
		{
			return tooltip_text;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			tooltip_text = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool Visible
	{
		get
		{
			return base.Visible;
		}
		set
		{
		}
	}

	internal Rectangle TabBounds
	{
		get
		{
			return tab_bounds;
		}
		set
		{
			tab_bounds = value;
		}
	}

	internal int Row
	{
		get
		{
			return row;
		}
		set
		{
			row = value;
		}
	}

	private TabControl Owner => base.Parent as TabControl;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Point Location
	{
		get
		{
			return base.Location;
		}
		set
		{
			base.Location = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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
	public new event EventHandler DockChanged
	{
		add
		{
			base.DockChanged += value;
		}
		remove
		{
			base.DockChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler EnabledChanged
	{
		add
		{
			base.EnabledChanged += value;
		}
		remove
		{
			base.EnabledChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler LocationChanged
	{
		add
		{
			base.LocationChanged += value;
		}
		remove
		{
			base.LocationChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler TabIndexChanged
	{
		add
		{
			base.TabIndexChanged += value;
		}
		remove
		{
			base.TabIndexChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler VisibleChanged
	{
		add
		{
			base.VisibleChanged += value;
		}
		remove
		{
			base.VisibleChanged -= value;
		}
	}

	public TabPage()
	{
		Visible = true;
		SetStyle(ControlStyles.CacheText, value: true);
	}

	public TabPage(string text)
	{
		base.Text = text;
	}

	public static TabPage GetTabPageOfComponent(object comp)
	{
		if (!(comp is Control control))
		{
			return null;
		}
		Control control2 = control.Parent;
		while (control2 != null && !(control2 is TabPage))
		{
			control2 = control2.Parent;
		}
		return control2 as TabPage;
	}

	public override string ToString()
	{
		return "TabPage: {" + Text + "}";
	}

	private void UpdateOwner()
	{
		if (Owner != null)
		{
			Owner.Redraw();
		}
	}

	internal void SetVisible(bool value)
	{
		base.Visible = value;
	}

	protected override ControlCollection CreateControlsInstance()
	{
		return new TabPageControlCollection(this);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		if (Owner != null && Owner.IsHandleCreated)
		{
			Rectangle displayRectangle = Owner.DisplayRectangle;
			base.SetBoundsCore(displayRectangle.X, displayRectangle.Y, displayRectangle.Width, displayRectangle.Height, BoundsSpecified.All);
		}
		else
		{
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}

	protected override void OnEnter(EventArgs e)
	{
		base.OnEnter(e);
	}

	protected override void OnLeave(EventArgs e)
	{
		base.OnLeave(e);
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
	}
}
