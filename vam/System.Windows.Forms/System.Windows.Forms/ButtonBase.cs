using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Designer("System.Windows.Forms.Design.ButtonBaseDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ComVisible(true)]
public abstract class ButtonBase : Control
{
	[ComVisible(true)]
	public class ButtonBaseAccessibleObject : ControlAccessibleObject
	{
		private new Control owner;

		public override AccessibleStates State => base.State;

		public ButtonBaseAccessibleObject(Control owner)
			: base(owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
			default_action = "Press";
			role = AccessibleRole.PushButton;
		}

		public override void DoDefaultAction()
		{
			((ButtonBase)owner).OnClick(EventArgs.Empty);
		}
	}

	private FlatStyle flat_style;

	private int image_index;

	internal Image image;

	internal ImageList image_list;

	private ContentAlignment image_alignment;

	internal ContentAlignment text_alignment;

	private bool is_default;

	internal bool is_pressed;

	internal StringFormat text_format;

	internal bool paint_as_acceptbutton;

	private bool auto_ellipsis;

	private FlatButtonAppearance flat_button_appearance;

	private string image_key;

	private TextImageRelation text_image_relation;

	private TextFormatFlags text_format_flags;

	private bool use_mnemonic;

	private bool use_visual_style_back_color;

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[MWFCategory("Behavior")]
	[DefaultValue(false)]
	public bool AutoEllipsis
	{
		get
		{
			return auto_ellipsis;
		}
		set
		{
			if (auto_ellipsis != value)
			{
				auto_ellipsis = value;
				if (auto_ellipsis)
				{
					text_format_flags |= TextFormatFlags.EndEllipsis;
					text_format_flags &= ~TextFormatFlags.WordBreak;
				}
				else
				{
					text_format_flags &= ~TextFormatFlags.EndEllipsis;
					text_format_flags |= TextFormatFlags.WordBreak;
				}
				if (base.Parent != null)
				{
					base.Parent.PerformLayout(this, "AutoEllipsis");
				}
				Invalidate();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[MWFCategory("Layout")]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MWFCategory("Appearance")]
	[Browsable(true)]
	public FlatButtonAppearance FlatAppearance => flat_button_appearance;

	[MWFCategory("Appearance")]
	[Localizable(true)]
	[DefaultValue(FlatStyle.Standard)]
	[MWFDescription("Determines look of button")]
	public FlatStyle FlatStyle
	{
		get
		{
			return flat_style;
		}
		set
		{
			if (flat_style != value)
			{
				flat_style = value;
				if (base.Parent != null)
				{
					base.Parent.PerformLayout(this, "FlatStyle");
				}
				Invalidate();
			}
		}
	}

	[Localizable(true)]
	[MWFDescription("Sets image to be displayed on button face")]
	[MWFCategory("Appearance")]
	public Image Image
	{
		get
		{
			if (image != null)
			{
				return image;
			}
			if (image_index >= 0 && image_list != null)
			{
				return image_list.Images[image_index];
			}
			if (!string.IsNullOrEmpty(image_key) && image_list != null)
			{
				return image_list.Images[image_key];
			}
			return null;
		}
		set
		{
			if (image != value)
			{
				image = value;
				image_index = -1;
				image_key = string.Empty;
				image_list = null;
				if (AutoSize && base.Parent != null)
				{
					base.Parent.PerformLayout(this, "Image");
				}
				Invalidate();
			}
		}
	}

	[MWFCategory("Appearance")]
	[Localizable(true)]
	[DefaultValue(ContentAlignment.MiddleCenter)]
	[MWFDescription("Sets the alignment of the image to be displayed on button face")]
	public ContentAlignment ImageAlign
	{
		get
		{
			return image_alignment;
		}
		set
		{
			if (image_alignment != value)
			{
				image_alignment = value;
				Invalidate();
			}
		}
	}

	[TypeConverter(typeof(ImageIndexConverter))]
	[DefaultValue(-1)]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[MWFCategory("Appearance")]
	[MWFDescription("Index of image to display, if ImageList is used for button face images")]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public int ImageIndex
	{
		get
		{
			if (image_list == null)
			{
				return -1;
			}
			return image_index;
		}
		set
		{
			if (image_index != value)
			{
				image_index = value;
				image = null;
				image_key = string.Empty;
				Invalidate();
			}
		}
	}

	[DefaultValue("")]
	[Localizable(true)]
	[MWFCategory("Appearance")]
	[TypeConverter(typeof(ImageKeyConverter))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public string ImageKey
	{
		get
		{
			return image_key;
		}
		set
		{
			if (image_key != value)
			{
				image = null;
				image_index = -1;
				image_key = value;
				Invalidate();
			}
		}
	}

	[MWFCategory("Appearance")]
	[RefreshProperties(RefreshProperties.Repaint)]
	[MWFDescription("ImageList used for ImageIndex")]
	[DefaultValue(null)]
	public ImageList ImageList
	{
		get
		{
			return image_list;
		}
		set
		{
			if (image_list != value)
			{
				image_list = value;
				if (value != null && image != null)
				{
					image = null;
				}
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ImeMode ImeMode
	{
		get
		{
			return base.ImeMode;
		}
		set
		{
			base.ImeMode = value;
		}
	}

	[SettingsBindable(true)]
	[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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

	[MWFCategory("Appearance")]
	[DefaultValue(ContentAlignment.MiddleCenter)]
	[Localizable(true)]
	[MWFDescription("Alignment for button text")]
	public virtual ContentAlignment TextAlign
	{
		get
		{
			return text_alignment;
		}
		set
		{
			if (text_alignment != value)
			{
				text_alignment = value;
				text_format_flags &= ~TextFormatFlags.Bottom;
				text_format_flags &= (TextFormatFlags)(-1);
				text_format_flags &= (TextFormatFlags)(-1);
				text_format_flags &= ~TextFormatFlags.Right;
				text_format_flags &= ~TextFormatFlags.HorizontalCenter;
				text_format_flags &= ~TextFormatFlags.VerticalCenter;
				switch (text_alignment)
				{
				case ContentAlignment.TopLeft:
					text_format.Alignment = StringAlignment.Near;
					text_format.LineAlignment = StringAlignment.Near;
					break;
				case ContentAlignment.TopCenter:
					text_format.Alignment = StringAlignment.Center;
					text_format.LineAlignment = StringAlignment.Near;
					text_format_flags |= TextFormatFlags.HorizontalCenter;
					break;
				case ContentAlignment.TopRight:
					text_format.Alignment = StringAlignment.Far;
					text_format.LineAlignment = StringAlignment.Near;
					text_format_flags |= TextFormatFlags.Right;
					break;
				case ContentAlignment.MiddleLeft:
					text_format.Alignment = StringAlignment.Near;
					text_format.LineAlignment = StringAlignment.Center;
					text_format_flags |= TextFormatFlags.VerticalCenter;
					break;
				case ContentAlignment.MiddleCenter:
					text_format.Alignment = StringAlignment.Center;
					text_format.LineAlignment = StringAlignment.Center;
					text_format_flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
					break;
				case ContentAlignment.MiddleRight:
					text_format.Alignment = StringAlignment.Far;
					text_format.LineAlignment = StringAlignment.Center;
					text_format_flags |= TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
					break;
				case ContentAlignment.BottomLeft:
					text_format.Alignment = StringAlignment.Near;
					text_format.LineAlignment = StringAlignment.Far;
					text_format_flags |= TextFormatFlags.Bottom;
					break;
				case ContentAlignment.BottomCenter:
					text_format.Alignment = StringAlignment.Center;
					text_format.LineAlignment = StringAlignment.Far;
					text_format_flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
					break;
				case ContentAlignment.BottomRight:
					text_format.Alignment = StringAlignment.Far;
					text_format.LineAlignment = StringAlignment.Far;
					text_format_flags |= TextFormatFlags.Right | TextFormatFlags.Bottom;
					break;
				}
				Invalidate();
			}
		}
	}

	[MWFCategory("Appearance")]
	[DefaultValue(TextImageRelation.Overlay)]
	[Localizable(true)]
	public TextImageRelation TextImageRelation
	{
		get
		{
			return text_image_relation;
		}
		set
		{
			if (!Enum.IsDefined(typeof(TextImageRelation), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for TextImageRelation");
			}
			if (text_image_relation != value)
			{
				text_image_relation = value;
				if (AutoSize && base.Parent != null)
				{
					base.Parent.PerformLayout(this, "TextImageRelation");
				}
				Invalidate();
			}
		}
	}

	[DefaultValue(false)]
	[MWFCategory("Behavior")]
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

	[MWFCategory("Appearance")]
	[DefaultValue(true)]
	public bool UseMnemonic
	{
		get
		{
			return use_mnemonic;
		}
		set
		{
			if (use_mnemonic != value)
			{
				use_mnemonic = value;
				if (use_mnemonic)
				{
					text_format_flags &= ~TextFormatFlags.NoPrefix;
				}
				else
				{
					text_format_flags |= TextFormatFlags.NoPrefix;
				}
				Invalidate();
			}
		}
	}

	[MWFCategory("Appearance")]
	public bool UseVisualStyleBackColor
	{
		get
		{
			return use_visual_style_back_color;
		}
		set
		{
			if (use_visual_style_back_color != value)
			{
				use_visual_style_back_color = value;
				Invalidate();
			}
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override ImeMode DefaultImeMode => ImeMode.Disable;

	protected override Size DefaultSize => ThemeEngine.Current.ButtonBaseDefaultSize;

	protected internal bool IsDefault
	{
		get
		{
			return is_default;
		}
		set
		{
			if (is_default != value)
			{
				is_default = value;
				Invalidate();
			}
		}
	}

	internal ButtonState ButtonState
	{
		get
		{
			ButtonState buttonState = ButtonState.Normal;
			if (base.Enabled)
			{
				if (is_entered)
				{
					if (flat_style == FlatStyle.Flat)
					{
						buttonState |= ButtonState.Flat;
					}
				}
				else if (flat_style == FlatStyle.Flat || flat_style == FlatStyle.Popup)
				{
					buttonState |= ButtonState.Flat;
				}
				if (is_entered && is_pressed)
				{
					buttonState |= ButtonState.Pushed;
				}
			}
			else
			{
				buttonState |= ButtonState.Inactive;
				if (flat_style == FlatStyle.Flat || flat_style == FlatStyle.Popup)
				{
					buttonState |= ButtonState.Flat;
				}
			}
			return buttonState;
		}
	}

	internal bool Pressed => is_pressed;

	internal TextFormatFlags TextFormatFlags => text_format_flags;

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
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
	public new event EventHandler ImeModeChanged
	{
		add
		{
			base.ImeModeChanged += value;
		}
		remove
		{
			base.ImeModeChanged -= value;
		}
	}

	protected ButtonBase()
	{
		flat_style = FlatStyle.Standard;
		flat_button_appearance = new FlatButtonAppearance(this);
		image_key = string.Empty;
		text_image_relation = TextImageRelation.Overlay;
		use_mnemonic = true;
		use_visual_style_back_color = true;
		image_index = -1;
		image = null;
		image_list = null;
		image_alignment = ContentAlignment.MiddleCenter;
		ImeMode = ImeMode.Disable;
		text_alignment = ContentAlignment.MiddleCenter;
		is_default = false;
		is_pressed = false;
		text_format = new StringFormat();
		text_format.Alignment = StringAlignment.Center;
		text_format.LineAlignment = StringAlignment.Center;
		text_format.HotkeyPrefix = HotkeyPrefix.Show;
		text_format.FormatFlags |= StringFormatFlags.LineLimit;
		text_format_flags = TextFormatFlags.HorizontalCenter;
		text_format_flags |= TextFormatFlags.VerticalCenter;
		text_format_flags |= TextFormatFlags.TextBoxControl;
		SetStyle(ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserMouse | ControlStyles.SupportsTransparentBackColor | ControlStyles.CacheText | ControlStyles.OptimizedDoubleBuffer, value: true);
		SetStyle(ControlStyles.StandardClick, value: false);
	}

	internal bool ShouldSerializeImage()
	{
		return Image != null;
	}

	public override Size GetPreferredSize(Size proposedSize)
	{
		return base.GetPreferredSize(proposedSize);
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ButtonBaseAccessibleObject(this);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
	}

	protected override void OnGotFocus(EventArgs e)
	{
		Invalidate();
		base.OnGotFocus(e);
	}

	protected override void OnKeyDown(KeyEventArgs kevent)
	{
		if (kevent.KeyData == Keys.Space)
		{
			is_pressed = true;
			Invalidate();
			kevent.Handled = true;
		}
		base.OnKeyDown(kevent);
	}

	protected override void OnKeyUp(KeyEventArgs kevent)
	{
		if (kevent.KeyData == Keys.Space)
		{
			is_pressed = false;
			Invalidate();
			OnClick(EventArgs.Empty);
			kevent.Handled = true;
		}
		base.OnKeyUp(kevent);
	}

	protected override void OnLostFocus(EventArgs e)
	{
		Invalidate();
		base.OnLostFocus(e);
	}

	protected override void OnMouseDown(MouseEventArgs mevent)
	{
		if ((mevent.Button & MouseButtons.Left) != 0)
		{
			is_pressed = true;
			Invalidate();
		}
		base.OnMouseDown(mevent);
	}

	protected override void OnMouseEnter(EventArgs eventargs)
	{
		is_entered = true;
		Invalidate();
		base.OnMouseEnter(eventargs);
	}

	protected override void OnMouseLeave(EventArgs eventargs)
	{
		is_entered = false;
		Invalidate();
		base.OnMouseLeave(eventargs);
	}

	protected override void OnMouseMove(MouseEventArgs mevent)
	{
		bool flag = false;
		bool flag2 = false;
		if (base.ClientRectangle.Contains(mevent.Location))
		{
			flag = true;
		}
		if ((mevent.Button & MouseButtons.Left) != 0 && base.Capture && flag != is_pressed)
		{
			is_pressed = flag;
			flag2 = true;
		}
		if (is_entered != flag)
		{
			is_entered = flag;
			flag2 = true;
		}
		if (flag2)
		{
			Invalidate();
		}
		base.OnMouseMove(mevent);
	}

	protected override void OnMouseUp(MouseEventArgs mevent)
	{
		if (base.Capture && (mevent.Button & MouseButtons.Left) != 0)
		{
			base.Capture = false;
			if (is_pressed)
			{
				is_pressed = false;
				Invalidate();
			}
			else if (flat_style == FlatStyle.Flat || flat_style == FlatStyle.Popup)
			{
				Invalidate();
			}
			if (base.ClientRectangle.Contains(mevent.Location) && !base.ValidationFailed)
			{
				OnClick(EventArgs.Empty);
				OnMouseClick(mevent);
			}
		}
		base.OnMouseUp(mevent);
	}

	protected override void OnPaint(PaintEventArgs pevent)
	{
		Draw(pevent);
		base.OnPaint(pevent);
	}

	protected override void OnParentChanged(EventArgs e)
	{
		base.OnParentChanged(e);
	}

	protected override void OnTextChanged(EventArgs e)
	{
		Invalidate();
		base.OnTextChanged(e);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		if (!base.Visible)
		{
			is_pressed = false;
			is_entered = false;
		}
		base.OnVisibleChanged(e);
	}

	protected void ResetFlagsandPaint()
	{
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_LBUTTONDBLCLK:
			HaveDoubleClick();
			break;
		case Msg.WM_MBUTTONDBLCLK:
			HaveDoubleClick();
			break;
		case Msg.WM_RBUTTONDBLCLK:
			HaveDoubleClick();
			break;
		}
		base.WndProc(ref m);
	}

	internal virtual void Draw(PaintEventArgs pevent)
	{
		ThemeEngine.Current.DrawButtonBase(pevent.Graphics, pevent.ClipRectangle, this);
	}

	internal virtual void HaveDoubleClick()
	{
	}

	internal override void OnPaintBackgroundInternal(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
	}
}
