using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DesignTimeVisible(false)]
[DefaultProperty("Text")]
[DefaultEvent("Click")]
[Designer("System.Windows.Forms.Design.ToolStripItemDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ToolboxItem(false)]
public abstract class ToolStripItem : Component, IDisposable, IComponent, IDropTarget
{
	[ComVisible(true)]
	public class ToolStripItemAccessibleObject : AccessibleObject
	{
		internal ToolStripItem owner_item;

		public override Rectangle Bounds => (!owner_item.Visible) ? Rectangle.Empty : owner_item.Bounds;

		public override string DefaultAction => base.DefaultAction;

		public override string Description => base.Description;

		public override string Help => base.Help;

		public override string KeyboardShortcut => base.KeyboardShortcut;

		public override string Name
		{
			get
			{
				if (name == string.Empty)
				{
					return owner_item.Text;
				}
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public override AccessibleObject Parent => base.Parent;

		public override AccessibleRole Role => base.Role;

		public override AccessibleStates State => base.State;

		public ToolStripItemAccessibleObject(ToolStripItem ownerItem)
		{
			if (ownerItem == null)
			{
				throw new ArgumentNullException("ownerItem");
			}
			owner_item = ownerItem;
			default_action = string.Empty;
			keyboard_shortcut = string.Empty;
			name = string.Empty;
			value = string.Empty;
		}

		public void AddState(AccessibleStates state)
		{
			base.state = state;
		}

		public override void DoDefaultAction()
		{
			base.DoDefaultAction();
		}

		public override int GetHelpTopic(out string fileName)
		{
			return base.GetHelpTopic(out fileName);
		}

		public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
		{
			return base.Navigate(navigationDirection);
		}

		public override string ToString()
		{
			return $"ToolStripItemAccessibleObject: Owner = {owner_item.ToString()}";
		}
	}

	private AccessibleObject accessibility_object;

	private string accessible_default_action_description;

	private bool allow_drop;

	private ToolStripItemAlignment alignment;

	private AnchorStyles anchor;

	private bool available;

	private bool auto_size;

	private bool auto_tool_tip;

	private Color back_color;

	private Image background_image;

	private ImageLayout background_image_layout;

	private Rectangle bounds;

	private bool can_select;

	private ToolStripItemDisplayStyle display_style;

	private DockStyle dock;

	private bool double_click_enabled;

	private bool enabled;

	private Size explicit_size;

	private Font font;

	private Color fore_color;

	private Image image;

	private ContentAlignment image_align;

	private int image_index;

	private string image_key;

	private ToolStripItemImageScaling image_scaling;

	private Color image_transparent_color;

	private bool is_disposed;

	internal bool is_pressed;

	private bool is_selected;

	private Padding margin;

	private MergeAction merge_action;

	private int merge_index;

	private string name;

	private ToolStripItemOverflow overflow;

	private ToolStrip owner;

	internal ToolStripItem owner_item;

	private Padding padding;

	private ToolStripItemPlacement placement;

	private RightToLeft right_to_left;

	private bool right_to_left_auto_mirror_image;

	private object tag;

	private string text;

	private ContentAlignment text_align;

	private ToolStripTextDirection text_direction;

	private TextImageRelation text_image_relation;

	private string tool_tip_text;

	private bool visible;

	private EventHandler frame_handler;

	private ToolStrip parent;

	private Size text_size;

	private static object AvailableChangedEvent;

	private static object BackColorChangedEvent;

	private static object ClickEvent;

	private static object DisplayStyleChangedEvent;

	private static object DoubleClickEvent;

	private static object DragDropEvent;

	private static object DragEnterEvent;

	private static object DragLeaveEvent;

	private static object DragOverEvent;

	private static object EnabledChangedEvent;

	private static object ForeColorChangedEvent;

	private static object GiveFeedbackEvent;

	private static object LocationChangedEvent;

	private static object MouseDownEvent;

	private static object MouseEnterEvent;

	private static object MouseHoverEvent;

	private static object MouseLeaveEvent;

	private static object MouseMoveEvent;

	private static object MouseUpEvent;

	private static object OwnerChangedEvent;

	private static object PaintEvent;

	private static object QueryAccessibilityHelpEvent;

	private static object QueryContinueDragEvent;

	private static object RightToLeftChangedEvent;

	private static object TextChangedEvent;

	private static object VisibleChangedEvent;

	private static object UIASelectionChangedEvent;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public AccessibleObject AccessibilityObject
	{
		get
		{
			if (accessibility_object == null)
			{
				accessibility_object = CreateAccessibilityInstance();
			}
			return accessibility_object;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string AccessibleDefaultActionDescription
	{
		get
		{
			if (accessibility_object == null)
			{
				return null;
			}
			return accessible_default_action_description;
		}
		set
		{
			accessible_default_action_description = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(null)]
	public string AccessibleDescription
	{
		get
		{
			if (accessibility_object == null)
			{
				return null;
			}
			return AccessibilityObject.Description;
		}
		set
		{
			AccessibilityObject.description = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(null)]
	public string AccessibleName
	{
		get
		{
			if (accessibility_object == null)
			{
				return null;
			}
			return AccessibilityObject.Name;
		}
		set
		{
			AccessibilityObject.Name = value;
		}
	}

	[DefaultValue(AccessibleRole.Default)]
	public AccessibleRole AccessibleRole
	{
		get
		{
			if (accessibility_object == null)
			{
				return AccessibleRole.Default;
			}
			return AccessibilityObject.Role;
		}
		set
		{
			AccessibilityObject.role = value;
		}
	}

	[DefaultValue(ToolStripItemAlignment.Left)]
	public ToolStripItemAlignment Alignment
	{
		get
		{
			return alignment;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ToolStripItemAlignment), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripItemAlignment");
			}
			if (alignment != value)
			{
				alignment = value;
				CalculateAutoSize();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO("Stub, does nothing")]
	[Browsable(false)]
	[DefaultValue(false)]
	public virtual bool AllowDrop
	{
		get
		{
			return allow_drop;
		}
		set
		{
			allow_drop = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
	public AnchorStyles Anchor
	{
		get
		{
			return anchor;
		}
		set
		{
			anchor = value;
		}
	}

	[DefaultValue(true)]
	[Localizable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[RefreshProperties(RefreshProperties.All)]
	public bool AutoSize
	{
		get
		{
			return auto_size;
		}
		set
		{
			auto_size = value;
			CalculateAutoSize();
		}
	}

	[DefaultValue(false)]
	public bool AutoToolTip
	{
		get
		{
			return auto_tool_tip;
		}
		set
		{
			auto_tool_tip = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool Available
	{
		get
		{
			return available;
		}
		set
		{
			if (available != value)
			{
				available = value;
				visible = value;
				if (parent != null)
				{
					parent.PerformLayout();
				}
				OnAvailableChanged(EventArgs.Empty);
				OnVisibleChanged(EventArgs.Empty);
			}
		}
	}

	public virtual Color BackColor
	{
		get
		{
			if (back_color != Color.Empty)
			{
				return back_color;
			}
			if (Parent != null)
			{
				return parent.BackColor;
			}
			return Control.DefaultBackColor;
		}
		set
		{
			if (back_color != value)
			{
				back_color = value;
				OnBackColorChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(null)]
	public virtual Image BackgroundImage
	{
		get
		{
			return background_image;
		}
		set
		{
			if (background_image != value)
			{
				background_image = value;
				Invalidate();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(ImageLayout.Tile)]
	public virtual ImageLayout BackgroundImageLayout
	{
		get
		{
			return background_image_layout;
		}
		set
		{
			if (background_image_layout != value)
			{
				background_image_layout = value;
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	public virtual Rectangle Bounds => bounds;

	[Browsable(false)]
	public virtual bool CanSelect => can_select;

	[Browsable(false)]
	public Rectangle ContentRectangle
	{
		get
		{
			if (this is ToolStripLabel || this is ToolStripStatusLabel)
			{
				return new Rectangle(0, 0, bounds.Width, bounds.Height);
			}
			if (this is ToolStripDropDownButton && (this as ToolStripDropDownButton).ShowDropDownArrow)
			{
				return new Rectangle(2, 2, bounds.Width - 13, bounds.Height - 4);
			}
			return new Rectangle(2, 2, bounds.Width - 4, bounds.Height - 4);
		}
	}

	public virtual ToolStripItemDisplayStyle DisplayStyle
	{
		get
		{
			return display_style;
		}
		set
		{
			if (display_style != value)
			{
				display_style = value;
				CalculateAutoSize();
				OnDisplayStyleChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	public bool IsDisposed => is_disposed;

	[Browsable(false)]
	[DefaultValue(DockStyle.None)]
	public DockStyle Dock
	{
		get
		{
			return dock;
		}
		set
		{
			if (dock != value)
			{
				if (!Enum.IsDefined(typeof(DockStyle), value))
				{
					throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for DockStyle");
				}
				dock = value;
				CalculateAutoSize();
			}
		}
	}

	[DefaultValue(false)]
	public bool DoubleClickEnabled
	{
		get
		{
			return double_click_enabled;
		}
		set
		{
			double_click_enabled = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(true)]
	public virtual bool Enabled
	{
		get
		{
			if (Parent != null && !Parent.Enabled)
			{
				return false;
			}
			if (Owner != null && !Owner.Enabled)
			{
				return false;
			}
			return enabled;
		}
		set
		{
			if (enabled != value)
			{
				enabled = value;
				OnEnabledChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	[Localizable(true)]
	public virtual Font Font
	{
		get
		{
			if (font != null)
			{
				return font;
			}
			if (Parent != null)
			{
				return Parent.Font;
			}
			return DefaultFont;
		}
		set
		{
			if (font != value)
			{
				font = value;
				CalculateAutoSize();
				OnFontChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	public virtual Color ForeColor
	{
		get
		{
			if (fore_color != Color.Empty)
			{
				return fore_color;
			}
			if (Parent != null)
			{
				return parent.ForeColor;
			}
			return Control.DefaultForeColor;
		}
		set
		{
			if (fore_color != value)
			{
				fore_color = value;
				OnForeColorChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int Height
	{
		get
		{
			return Size.Height;
		}
		set
		{
			Size = new Size(Size.Width, value);
			explicit_size.Height = value;
			if (Visible)
			{
				CalculateAutoSize();
				OnBoundsChanged();
				Invalidate();
			}
		}
	}

	[Localizable(true)]
	public virtual Image Image
	{
		get
		{
			if (image != null)
			{
				return image;
			}
			if (image_index >= 0 && owner != null && owner.ImageList != null && owner.ImageList.Images.Count > image_index)
			{
				return owner.ImageList.Images[image_index];
			}
			if (!string.IsNullOrEmpty(image_key) && owner != null && owner.ImageList != null && owner.ImageList.Images.Count > image_index)
			{
				return owner.ImageList.Images[image_key];
			}
			return null;
		}
		set
		{
			if (image != value)
			{
				StopAnimation();
				image = value;
				image_index = -1;
				image_key = string.Empty;
				CalculateAutoSize();
				Invalidate();
				BeginAnimation();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(ContentAlignment.MiddleCenter)]
	public ContentAlignment ImageAlign
	{
		get
		{
			return image_align;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ContentAlignment), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ContentAlignment");
			}
			if (image_align != value)
			{
				image_align = value;
				CalculateAutoSize();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[Localizable(true)]
	[Browsable(false)]
	[RelatedImageList("Owner.ImageList")]
	[TypeConverter(typeof(NoneExcludedImageIndexConverter))]
	[Editor("System.Windows.Forms.Design.ToolStripImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public int ImageIndex
	{
		get
		{
			return image_index;
		}
		set
		{
			if (image_index != value)
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("ImageIndex cannot be less than -1");
				}
				image_index = value;
				image = null;
				image_key = string.Empty;
				CalculateAutoSize();
				Invalidate();
			}
		}
	}

	[Editor("System.Windows.Forms.Design.ToolStripImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Browsable(false)]
	[RelatedImageList("Owner.ImageList")]
	[TypeConverter(typeof(ImageKeyConverter))]
	[Localizable(true)]
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
				CalculateAutoSize();
				Invalidate();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(ToolStripItemImageScaling.SizeToFit)]
	public ToolStripItemImageScaling ImageScaling
	{
		get
		{
			return image_scaling;
		}
		set
		{
			if (image_scaling != value)
			{
				image_scaling = value;
				CalculateAutoSize();
			}
		}
	}

	[Localizable(true)]
	public Color ImageTransparentColor
	{
		get
		{
			return image_transparent_color;
		}
		set
		{
			image_transparent_color = value;
		}
	}

	[Browsable(false)]
	public bool IsOnDropDown
	{
		get
		{
			if (parent != null && parent is ToolStripDropDown)
			{
				return true;
			}
			return false;
		}
	}

	[Browsable(false)]
	public bool IsOnOverflow => placement == ToolStripItemPlacement.Overflow;

	public Padding Margin
	{
		get
		{
			return margin;
		}
		set
		{
			margin = value;
			CalculateAutoSize();
		}
	}

	[DefaultValue(MergeAction.Append)]
	public MergeAction MergeAction
	{
		get
		{
			return merge_action;
		}
		set
		{
			if (!Enum.IsDefined(typeof(MergeAction), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for MergeAction");
			}
			merge_action = value;
		}
	}

	[DefaultValue(-1)]
	public int MergeIndex
	{
		get
		{
			return merge_index;
		}
		set
		{
			merge_index = value;
		}
	}

	[DefaultValue(null)]
	[Browsable(false)]
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	[DefaultValue(ToolStripItemOverflow.AsNeeded)]
	public ToolStripItemOverflow Overflow
	{
		get
		{
			return overflow;
		}
		set
		{
			if (overflow != value)
			{
				if (!Enum.IsDefined(typeof(ToolStripItemOverflow), value))
				{
					throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripItemOverflow");
				}
				overflow = value;
				if (owner != null)
				{
					owner.PerformLayout();
				}
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public ToolStrip Owner
	{
		get
		{
			return owner;
		}
		set
		{
			if (owner != value)
			{
				if (owner != null)
				{
					owner.Items.Remove(this);
				}
				if (value != null)
				{
					value.Items.Add(this);
				}
				else
				{
					owner = null;
				}
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public ToolStripItem OwnerItem => owner_item;

	public virtual Padding Padding
	{
		get
		{
			return padding;
		}
		set
		{
			padding = value;
			CalculateAutoSize();
			Invalidate();
		}
	}

	[Browsable(false)]
	public ToolStripItemPlacement Placement => placement;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual bool Pressed => is_pressed;

	[System.MonoTODO("RTL not implemented")]
	[Localizable(true)]
	public virtual RightToLeft RightToLeft
	{
		get
		{
			return right_to_left;
		}
		set
		{
			if (right_to_left != value)
			{
				right_to_left = value;
				OnRightToLeftChanged(EventArgs.Empty);
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(false)]
	public bool RightToLeftAutoMirrorImage
	{
		get
		{
			return right_to_left_auto_mirror_image;
		}
		set
		{
			if (right_to_left_auto_mirror_image != value)
			{
				right_to_left_auto_mirror_image = value;
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual bool Selected => is_selected;

	[Localizable(true)]
	public virtual Size Size
	{
		get
		{
			if (!AutoSize && explicit_size != Size.Empty)
			{
				return explicit_size;
			}
			return bounds.Size;
		}
		set
		{
			bounds.Size = value;
			explicit_size = value;
			if (Visible)
			{
				CalculateAutoSize();
				OnBoundsChanged();
			}
		}
	}

	[Localizable(false)]
	[Bindable(true)]
	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	[DefaultValue("")]
	[Localizable(true)]
	public virtual string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (text != value)
			{
				text = value;
				Invalidate();
				CalculateAutoSize();
				Invalidate();
				OnTextChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(ContentAlignment.MiddleCenter)]
	[Localizable(true)]
	public virtual ContentAlignment TextAlign
	{
		get
		{
			return text_align;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ContentAlignment), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ContentAlignment");
			}
			if (text_align != value)
			{
				text_align = value;
				CalculateAutoSize();
			}
		}
	}

	public virtual ToolStripTextDirection TextDirection
	{
		get
		{
			if (text_direction == ToolStripTextDirection.Inherit)
			{
				if (Parent != null)
				{
					return Parent.TextDirection;
				}
				return ToolStripTextDirection.Horizontal;
			}
			return text_direction;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ToolStripTextDirection), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripTextDirection");
			}
			if (text_direction != value)
			{
				text_direction = value;
				CalculateAutoSize();
				Invalidate();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(TextImageRelation.ImageBeforeText)]
	public TextImageRelation TextImageRelation
	{
		get
		{
			return text_image_relation;
		}
		set
		{
			text_image_relation = value;
			CalculateAutoSize();
			Invalidate();
		}
	}

	[Localizable(true)]
	[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string ToolTipText
	{
		get
		{
			return tool_tip_text;
		}
		set
		{
			tool_tip_text = value;
		}
	}

	[Localizable(true)]
	public bool Visible
	{
		get
		{
			if (parent == null)
			{
				return false;
			}
			return visible && parent.Visible;
		}
		set
		{
			if (visible != value)
			{
				available = value;
				SetVisibleCore(value);
				if (Owner != null)
				{
					Owner.PerformLayout();
				}
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
	public int Width
	{
		get
		{
			return Size.Width;
		}
		set
		{
			Size = new Size(value, Size.Height);
			explicit_size.Width = value;
			if (Visible)
			{
				CalculateAutoSize();
				OnBoundsChanged();
				Invalidate();
			}
		}
	}

	protected virtual bool DefaultAutoToolTip => false;

	protected virtual ToolStripItemDisplayStyle DefaultDisplayStyle => ToolStripItemDisplayStyle.ImageAndText;

	protected internal virtual Padding DefaultMargin => new Padding(0, 1, 0, 2);

	protected virtual Padding DefaultPadding => default(Padding);

	protected virtual Size DefaultSize => new Size(23, 23);

	protected internal virtual bool DismissWhenClicked => true;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	protected internal ToolStrip Parent
	{
		get
		{
			return parent;
		}
		set
		{
			if (parent != value)
			{
				ToolStrip oldParent = parent;
				parent = value;
				OnParentChanged(oldParent, parent);
			}
		}
	}

	protected internal virtual bool ShowKeyboardCues => false;

	private static Font DefaultFont => new Font("Tahoma", 8.25f);

	internal virtual ToolStripTextDirection DefaultTextDirection => ToolStripTextDirection.Inherit;

	internal bool ShowMargin
	{
		get
		{
			if (!IsOnDropDown)
			{
				return true;
			}
			if (!(Owner is ToolStripDropDownMenu))
			{
				return false;
			}
			ToolStripDropDownMenu toolStripDropDownMenu = (ToolStripDropDownMenu)Owner;
			return toolStripDropDownMenu.ShowCheckMargin || toolStripDropDownMenu.ShowImageMargin;
		}
	}

	internal bool UseImageMargin
	{
		get
		{
			if (!IsOnDropDown)
			{
				return true;
			}
			if (!(Owner is ToolStripDropDownMenu))
			{
				return false;
			}
			ToolStripDropDownMenu toolStripDropDownMenu = (ToolStripDropDownMenu)Owner;
			return toolStripDropDownMenu.ShowImageMargin || toolStripDropDownMenu.ShowCheckMargin;
		}
	}

	internal virtual bool InternalVisible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
			Invalidate();
		}
	}

	internal ToolStrip InternalOwner
	{
		set
		{
			if (owner != value)
			{
				owner = value;
				CalculateAutoSize();
				OnOwnerChanged(EventArgs.Empty);
			}
		}
	}

	internal Point Location
	{
		get
		{
			return bounds.Location;
		}
		set
		{
			if (bounds.Location != value)
			{
				bounds.Location = value;
				OnLocationChanged(EventArgs.Empty);
			}
		}
	}

	internal int Top
	{
		get
		{
			return bounds.Y;
		}
		set
		{
			if (bounds.Y != value)
			{
				bounds.Y = value;
				OnLocationChanged(EventArgs.Empty);
			}
		}
	}

	internal int Left
	{
		get
		{
			return bounds.X;
		}
		set
		{
			if (bounds.X != value)
			{
				bounds.X = value;
				OnLocationChanged(EventArgs.Empty);
			}
		}
	}

	internal int Right => bounds.Right;

	internal int Bottom => bounds.Bottom;

	[Browsable(false)]
	public event EventHandler AvailableChanged
	{
		add
		{
			base.Events.AddHandler(AvailableChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AvailableChangedEvent, value);
		}
	}

	public event EventHandler BackColorChanged
	{
		add
		{
			base.Events.AddHandler(BackColorChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BackColorChangedEvent, value);
		}
	}

	public event EventHandler Click
	{
		add
		{
			base.Events.AddHandler(ClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ClickEvent, value);
		}
	}

	public event EventHandler DisplayStyleChanged
	{
		add
		{
			base.Events.AddHandler(DisplayStyleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DisplayStyleChangedEvent, value);
		}
	}

	public event EventHandler DoubleClick
	{
		add
		{
			base.Events.AddHandler(DoubleClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DoubleClickEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[System.MonoTODO("Event never raised")]
	public event DragEventHandler DragDrop
	{
		add
		{
			base.Events.AddHandler(DragDropEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DragDropEvent, value);
		}
	}

	[Browsable(false)]
	[System.MonoTODO("Event never raised")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public event DragEventHandler DragEnter
	{
		add
		{
			base.Events.AddHandler(DragEnterEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DragEnterEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO("Event never raised")]
	[Browsable(false)]
	public event EventHandler DragLeave
	{
		add
		{
			base.Events.AddHandler(DragLeaveEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DragLeaveEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO("Event never raised")]
	[Browsable(false)]
	public event DragEventHandler DragOver
	{
		add
		{
			base.Events.AddHandler(DragOverEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DragOverEvent, value);
		}
	}

	public event EventHandler EnabledChanged
	{
		add
		{
			base.Events.AddHandler(EnabledChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(EnabledChangedEvent, value);
		}
	}

	public event EventHandler ForeColorChanged
	{
		add
		{
			base.Events.AddHandler(ForeColorChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ForeColorChangedEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO("Event never raised")]
	[Browsable(false)]
	public event GiveFeedbackEventHandler GiveFeedback
	{
		add
		{
			base.Events.AddHandler(GiveFeedbackEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(GiveFeedbackEvent, value);
		}
	}

	public event EventHandler LocationChanged
	{
		add
		{
			base.Events.AddHandler(LocationChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LocationChangedEvent, value);
		}
	}

	public event MouseEventHandler MouseDown
	{
		add
		{
			base.Events.AddHandler(MouseDownEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseDownEvent, value);
		}
	}

	public event EventHandler MouseEnter
	{
		add
		{
			base.Events.AddHandler(MouseEnterEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseEnterEvent, value);
		}
	}

	public event EventHandler MouseHover
	{
		add
		{
			base.Events.AddHandler(MouseHoverEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseHoverEvent, value);
		}
	}

	public event EventHandler MouseLeave
	{
		add
		{
			base.Events.AddHandler(MouseLeaveEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseLeaveEvent, value);
		}
	}

	public event MouseEventHandler MouseMove
	{
		add
		{
			base.Events.AddHandler(MouseMoveEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseMoveEvent, value);
		}
	}

	public event MouseEventHandler MouseUp
	{
		add
		{
			base.Events.AddHandler(MouseUpEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseUpEvent, value);
		}
	}

	public event EventHandler OwnerChanged
	{
		add
		{
			base.Events.AddHandler(OwnerChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(OwnerChangedEvent, value);
		}
	}

	public event PaintEventHandler Paint
	{
		add
		{
			base.Events.AddHandler(PaintEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PaintEvent, value);
		}
	}

	[System.MonoTODO("Event never raised")]
	public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
	{
		add
		{
			base.Events.AddHandler(QueryAccessibilityHelpEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(QueryAccessibilityHelpEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO("Event never raised")]
	[Browsable(false)]
	public event QueryContinueDragEventHandler QueryContinueDrag
	{
		add
		{
			base.Events.AddHandler(QueryContinueDragEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(QueryContinueDragEvent, value);
		}
	}

	public event EventHandler RightToLeftChanged
	{
		add
		{
			base.Events.AddHandler(RightToLeftChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RightToLeftChangedEvent, value);
		}
	}

	public event EventHandler TextChanged
	{
		add
		{
			base.Events.AddHandler(TextChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(TextChangedEvent, value);
		}
	}

	public event EventHandler VisibleChanged
	{
		add
		{
			base.Events.AddHandler(VisibleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(VisibleChangedEvent, value);
		}
	}

	internal event EventHandler UIASelectionChanged
	{
		add
		{
			base.Events.AddHandler(UIASelectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIASelectionChangedEvent, value);
		}
	}

	protected ToolStripItem()
		: this(string.Empty, null, null, string.Empty)
	{
	}

	protected ToolStripItem(string text, Image image, EventHandler onClick)
		: this(text, image, onClick, string.Empty)
	{
	}

	protected ToolStripItem(string text, Image image, EventHandler onClick, string name)
	{
		alignment = ToolStripItemAlignment.Left;
		anchor = AnchorStyles.Top | AnchorStyles.Left;
		auto_size = true;
		auto_tool_tip = DefaultAutoToolTip;
		available = true;
		back_color = Color.Empty;
		background_image_layout = ImageLayout.Tile;
		can_select = true;
		display_style = DefaultDisplayStyle;
		dock = DockStyle.None;
		enabled = true;
		fore_color = Color.Empty;
		this.image = image;
		image_align = ContentAlignment.MiddleCenter;
		image_index = -1;
		image_key = string.Empty;
		image_scaling = ToolStripItemImageScaling.SizeToFit;
		image_transparent_color = Color.Empty;
		margin = DefaultMargin;
		merge_action = MergeAction.Append;
		merge_index = -1;
		this.name = name;
		overflow = ToolStripItemOverflow.AsNeeded;
		padding = DefaultPadding;
		placement = ToolStripItemPlacement.None;
		right_to_left = RightToLeft.Inherit;
		bounds.Size = DefaultSize;
		this.text = text;
		text_align = ContentAlignment.MiddleCenter;
		text_direction = DefaultTextDirection;
		text_image_relation = TextImageRelation.ImageBeforeText;
		visible = true;
		Click += onClick;
		OnLayout(new LayoutEventArgs(null, string.Empty));
	}

	static ToolStripItem()
	{
		AvailableChanged = new object();
		BackColorChanged = new object();
		Click = new object();
		DisplayStyleChanged = new object();
		DoubleClick = new object();
		DragDrop = new object();
		DragEnter = new object();
		DragLeave = new object();
		DragOver = new object();
		EnabledChanged = new object();
		ForeColorChanged = new object();
		GiveFeedback = new object();
		LocationChanged = new object();
		MouseDown = new object();
		MouseEnter = new object();
		MouseHover = new object();
		MouseLeave = new object();
		MouseMove = new object();
		MouseUp = new object();
		OwnerChanged = new object();
		Paint = new object();
		QueryAccessibilityHelp = new object();
		QueryContinueDrag = new object();
		RightToLeftChanged = new object();
		TextChanged = new object();
		VisibleChanged = new object();
		UIASelectionChanged = new object();
	}

	void IDropTarget.OnDragDrop(DragEventArgs dragEvent)
	{
		OnDragDrop(dragEvent);
	}

	void IDropTarget.OnDragEnter(DragEventArgs dragEvent)
	{
		OnDragEnter(dragEvent);
	}

	void IDropTarget.OnDragLeave(EventArgs e)
	{
		OnDragLeave(e);
	}

	void IDropTarget.OnDragOver(DragEventArgs dragEvent)
	{
		OnDragOver(dragEvent);
	}

	[System.MonoTODO("Stub, does nothing")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
	{
		return allowedEffects;
	}

	public ToolStrip GetCurrentParent()
	{
		return parent;
	}

	public virtual Size GetPreferredSize(Size constrainingSize)
	{
		return CalculatePreferredSize(constrainingSize);
	}

	public void Invalidate()
	{
		if (parent != null)
		{
			parent.Invalidate(bounds);
		}
	}

	public void Invalidate(Rectangle r)
	{
		if (parent != null)
		{
			parent.Invalidate(r);
		}
	}

	public void PerformClick()
	{
		OnClick(EventArgs.Empty);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetBackColor()
	{
		BackColor = Color.Empty;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetDisplayStyle()
	{
		display_style = DefaultDisplayStyle;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetFont()
	{
		font = null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetForeColor()
	{
		ForeColor = Color.Empty;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetImage()
	{
		image = null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ResetMargin()
	{
		margin = DefaultMargin;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ResetPadding()
	{
		padding = DefaultPadding;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetRightToLeft()
	{
		right_to_left = RightToLeft.Inherit;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetTextDirection()
	{
		TextDirection = DefaultTextDirection;
	}

	public void Select()
	{
		if (is_selected || !CanSelect)
		{
			return;
		}
		is_selected = true;
		if (Parent != null)
		{
			if (Visible && Parent.Focused && this is ToolStripControlHost)
			{
				(this as ToolStripControlHost).Focus();
			}
			Invalidate();
			Parent.NotifySelectedChanged(this);
		}
		OnUIASelectionChanged();
	}

	public override string ToString()
	{
		return text;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripItemAccessibleObject(this);
	}

	protected override void Dispose(bool disposing)
	{
		if (!is_disposed && disposing)
		{
			is_disposed = true;
		}
		if (image != null)
		{
			StopAnimation();
			image = null;
		}
		if (owner != null)
		{
			owner.Items.Remove(this);
		}
		base.Dispose(disposing);
	}

	protected internal virtual bool IsInputChar(char charCode)
	{
		return false;
	}

	protected internal virtual bool IsInputKey(Keys keyData)
	{
		return false;
	}

	protected virtual void OnAvailableChanged(EventArgs e)
	{
		((EventHandler)base.Events[AvailableChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnBackColorChanged(EventArgs e)
	{
		((EventHandler)base.Events[BackColorChanged])?.Invoke(this, e);
	}

	protected virtual void OnBoundsChanged()
	{
		OnLayout(new LayoutEventArgs(null, string.Empty));
	}

	protected virtual void OnClick(EventArgs e)
	{
		((EventHandler)base.Events[Click])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnDisplayStyleChanged(EventArgs e)
	{
		((EventHandler)base.Events[DisplayStyleChanged])?.Invoke(this, e);
	}

	protected virtual void OnDoubleClick(EventArgs e)
	{
		((EventHandler)base.Events[DoubleClick])?.Invoke(this, e);
		if (!double_click_enabled)
		{
			OnClick(e);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnDragDrop(DragEventArgs dragEvent)
	{
		((DragEventHandler)base.Events[DragDrop])?.Invoke(this, dragEvent);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnDragEnter(DragEventArgs dragEvent)
	{
		((DragEventHandler)base.Events[DragEnter])?.Invoke(this, dragEvent);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnDragLeave(EventArgs e)
	{
		((EventHandler)base.Events[DragLeave])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnDragOver(DragEventArgs dragEvent)
	{
		((DragEventHandler)base.Events[DragOver])?.Invoke(this, dragEvent);
	}

	protected virtual void OnEnabledChanged(EventArgs e)
	{
		((EventHandler)base.Events[EnabledChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnFontChanged(EventArgs e)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnForeColorChanged(EventArgs e)
	{
		((EventHandler)base.Events[ForeColorChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEvent)
	{
		((GiveFeedbackEventHandler)base.Events[GiveFeedback])?.Invoke(this, giveFeedbackEvent);
	}

	protected virtual void OnLayout(LayoutEventArgs e)
	{
	}

	protected virtual void OnLocationChanged(EventArgs e)
	{
		((EventHandler)base.Events[LocationChanged])?.Invoke(this, e);
	}

	protected virtual void OnMouseDown(MouseEventArgs e)
	{
		if (Enabled)
		{
			is_pressed = true;
			Invalidate();
			((MouseEventHandler)base.Events[MouseDown])?.Invoke(this, e);
		}
	}

	protected virtual void OnMouseEnter(EventArgs e)
	{
		Select();
		((EventHandler)base.Events[MouseEnter])?.Invoke(this, e);
	}

	protected virtual void OnMouseHover(EventArgs e)
	{
		if (Enabled)
		{
			((EventHandler)base.Events[MouseHover])?.Invoke(this, e);
		}
	}

	protected virtual void OnMouseLeave(EventArgs e)
	{
		if (CanSelect)
		{
			is_selected = false;
			is_pressed = false;
			Invalidate();
			OnUIASelectionChanged();
		}
		((EventHandler)base.Events[MouseLeave])?.Invoke(this, e);
	}

	protected virtual void OnMouseMove(MouseEventArgs mea)
	{
		if (Enabled)
		{
			((MouseEventHandler)base.Events[MouseMove])?.Invoke(this, mea);
		}
	}

	protected virtual void OnMouseUp(MouseEventArgs e)
	{
		if (!Enabled)
		{
			return;
		}
		is_pressed = false;
		Invalidate();
		if (IsOnDropDown && (!(this is ToolStripDropDownItem) || !(this as ToolStripDropDownItem).HasDropDownItems || !(this as ToolStripDropDownItem).DropDown.Visible))
		{
			if ((Parent as ToolStripDropDown).OwnerItem != null)
			{
				((Parent as ToolStripDropDown).OwnerItem as ToolStripDropDownItem).HideDropDown();
			}
			else
			{
				(Parent as ToolStripDropDown).Hide();
			}
		}
		((MouseEventHandler)base.Events[MouseUp])?.Invoke(this, e);
	}

	protected virtual void OnOwnerChanged(EventArgs e)
	{
		((EventHandler)base.Events[OwnerChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected internal virtual void OnOwnerFontChanged(EventArgs e)
	{
		CalculateAutoSize();
		OnFontChanged(EventArgs.Empty);
	}

	protected virtual void OnPaint(PaintEventArgs e)
	{
		if (parent != null)
		{
			parent.Renderer.DrawItemBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
		}
		((PaintEventHandler)base.Events[Paint])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnParentBackColorChanged(EventArgs e)
	{
	}

	protected virtual void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
	{
		text_size = TextRenderer.MeasureText((Text != null) ? text : string.Empty, Font, Size.Empty, TextFormatFlags.HidePrefix);
		oldParent?.PerformLayout();
		newParent?.PerformLayout();
	}

	protected internal virtual void OnParentEnabledChanged(EventArgs e)
	{
		OnEnabledChanged(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnParentForeColorChanged(EventArgs e)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected internal virtual void OnParentRightToLeftChanged(EventArgs e)
	{
		OnRightToLeftChanged(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEvent)
	{
		((QueryContinueDragEventHandler)base.Events[QueryContinueDrag])?.Invoke(this, queryContinueDragEvent);
	}

	protected virtual void OnRightToLeftChanged(EventArgs e)
	{
		((EventHandler)base.Events[RightToLeftChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnTextChanged(EventArgs e)
	{
		((EventHandler)base.Events[TextChanged])?.Invoke(this, e);
	}

	protected virtual void OnVisibleChanged(EventArgs e)
	{
		((EventHandler)base.Events[VisibleChanged])?.Invoke(this, e);
	}

	protected internal virtual bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		return false;
	}

	protected internal virtual bool ProcessDialogKey(Keys keyData)
	{
		if (Selected && keyData == Keys.Return)
		{
			FireEvent(EventArgs.Empty, ToolStripItemEventType.Click);
			return true;
		}
		return false;
	}

	protected internal virtual bool ProcessMnemonic(char charCode)
	{
		ToolStripManager.SetActiveToolStrip(Parent, keyboard: true);
		PerformClick();
		return true;
	}

	protected internal virtual void SetBounds(Rectangle bounds)
	{
		if (this.bounds != bounds)
		{
			this.bounds = bounds;
			OnBoundsChanged();
		}
	}

	protected virtual void SetVisibleCore(bool visible)
	{
		this.visible = visible;
		OnVisibleChanged(EventArgs.Empty);
		if (this.visible)
		{
			BeginAnimation();
		}
		else
		{
			StopAnimation();
		}
		Invalidate();
	}

	internal Rectangle AlignInRectangle(Rectangle outer, Size inner, ContentAlignment align)
	{
		int x = 0;
		int y = 0;
		switch (align)
		{
		case ContentAlignment.TopLeft:
		case ContentAlignment.MiddleLeft:
		case ContentAlignment.BottomLeft:
			x = outer.X;
			break;
		case ContentAlignment.TopCenter:
		case ContentAlignment.MiddleCenter:
		case ContentAlignment.BottomCenter:
			x = Math.Max(outer.X + (outer.Width - inner.Width) / 2, outer.Left);
			break;
		case ContentAlignment.TopRight:
		case ContentAlignment.MiddleRight:
		case ContentAlignment.BottomRight:
			x = outer.Right - inner.Width;
			break;
		}
		switch (align)
		{
		case ContentAlignment.TopLeft:
		case ContentAlignment.TopCenter:
		case ContentAlignment.TopRight:
			y = outer.Y;
			break;
		case ContentAlignment.MiddleLeft:
		case ContentAlignment.MiddleCenter:
		case ContentAlignment.MiddleRight:
			y = outer.Y + (outer.Height - inner.Height) / 2;
			break;
		case ContentAlignment.BottomLeft:
		case ContentAlignment.BottomCenter:
		case ContentAlignment.BottomRight:
			y = outer.Bottom - inner.Height;
			break;
		}
		return new Rectangle(x, y, Math.Min(inner.Width, outer.Width), Math.Min(inner.Height, outer.Height));
	}

	internal void CalculateAutoSize()
	{
		text_size = TextRenderer.MeasureText((Text != null) ? text : string.Empty, Font, Size.Empty, TextFormatFlags.HidePrefix);
		ToolStripTextDirection textDirection = TextDirection;
		if (textDirection == ToolStripTextDirection.Vertical270 || textDirection == ToolStripTextDirection.Vertical90)
		{
			text_size = new Size(text_size.Height, text_size.Width);
		}
		if (!auto_size || this is ToolStripControlHost)
		{
			return;
		}
		Size size = CalculatePreferredSize(Size.Empty);
		if (size != Size)
		{
			bounds.Width = size.Width;
			if (parent != null)
			{
				parent.PerformLayout();
			}
		}
	}

	internal virtual Size CalculatePreferredSize(Size constrainingSize)
	{
		if (!auto_size)
		{
			return explicit_size;
		}
		Size result = DefaultSize;
		switch (display_style)
		{
		case ToolStripItemDisplayStyle.Text:
		{
			int width = text_size.Width + padding.Horizontal;
			int height = text_size.Height + padding.Vertical;
			result = new Size(width, height);
			break;
		}
		case ToolStripItemDisplayStyle.Image:
			if (GetImageSize() == Size.Empty)
			{
				result = DefaultSize;
				break;
			}
			switch (image_scaling)
			{
			case ToolStripItemImageScaling.None:
				result = GetImageSize();
				break;
			case ToolStripItemImageScaling.SizeToFit:
				result = ((parent != null) ? parent.ImageScalingSize : GetImageSize());
				break;
			}
			break;
		case ToolStripItemDisplayStyle.ImageAndText:
		{
			int num = text_size.Width + padding.Horizontal;
			int num2 = text_size.Height + padding.Vertical;
			if (GetImageSize() != Size.Empty)
			{
				Size size = GetImageSize();
				if (image_scaling == ToolStripItemImageScaling.SizeToFit && parent != null)
				{
					size = parent.ImageScalingSize;
				}
				switch (text_image_relation)
				{
				case TextImageRelation.Overlay:
					num = Math.Max(num, size.Width);
					num2 = Math.Max(num2, size.Height);
					break;
				case TextImageRelation.ImageAboveText:
				case TextImageRelation.TextAboveImage:
					num = Math.Max(num, size.Width);
					num2 += size.Height;
					break;
				case TextImageRelation.ImageBeforeText:
				case TextImageRelation.TextBeforeImage:
					num2 = Math.Max(num2, size.Height);
					num += size.Width;
					break;
				}
			}
			result = new Size(num, num2);
			break;
		}
		}
		if (!(this is ToolStripLabel))
		{
			result.Height += 4;
			result.Width += 4;
		}
		return result;
	}

	internal void CalculateTextAndImageRectangles(out Rectangle text_rect, out Rectangle image_rect)
	{
		CalculateTextAndImageRectangles(ContentRectangle, out text_rect, out image_rect);
	}

	internal void CalculateTextAndImageRectangles(Rectangle contentRectangle, out Rectangle text_rect, out Rectangle image_rect)
	{
		text_rect = Rectangle.Empty;
		image_rect = Rectangle.Empty;
		switch (display_style)
		{
		case ToolStripItemDisplayStyle.None:
			break;
		case ToolStripItemDisplayStyle.Text:
			if (text != string.Empty)
			{
				text_rect = AlignInRectangle(contentRectangle, text_size, text_align);
			}
			break;
		case ToolStripItemDisplayStyle.Image:
			if (Image != null && UseImageMargin)
			{
				image_rect = AlignInRectangle(contentRectangle, GetImageSize(), image_align);
			}
			break;
		case ToolStripItemDisplayStyle.ImageAndText:
			if (text != string.Empty && (Image == null || !UseImageMargin))
			{
				text_rect = AlignInRectangle(contentRectangle, text_size, text_align);
			}
			else
			{
				if (text == string.Empty && (Image == null || !UseImageMargin))
				{
					break;
				}
				if (text == string.Empty && Image != null)
				{
					image_rect = AlignInRectangle(contentRectangle, GetImageSize(), image_align);
					break;
				}
				switch (text_image_relation)
				{
				case TextImageRelation.Overlay:
					text_rect = AlignInRectangle(contentRectangle, text_size, text_align);
					image_rect = AlignInRectangle(contentRectangle, GetImageSize(), image_align);
					break;
				case TextImageRelation.ImageAboveText:
				{
					Rectangle outer = new Rectangle(contentRectangle.Left, contentRectangle.Bottom - (text_size.Height - 4), contentRectangle.Width, text_size.Height - 4);
					Rectangle outer2 = new Rectangle(contentRectangle.Left, contentRectangle.Top, contentRectangle.Width, contentRectangle.Height - outer.Height);
					text_rect = AlignInRectangle(outer, text_size, text_align);
					image_rect = AlignInRectangle(outer2, GetImageSize(), image_align);
					break;
				}
				case TextImageRelation.TextAboveImage:
				{
					Rectangle outer = new Rectangle(contentRectangle.Left, contentRectangle.Top, contentRectangle.Width, text_size.Height - 4);
					Rectangle outer2 = new Rectangle(contentRectangle.Left, outer.Bottom, contentRectangle.Width, contentRectangle.Height - outer.Height);
					text_rect = AlignInRectangle(outer, text_size, text_align);
					image_rect = AlignInRectangle(outer2, GetImageSize(), image_align);
					break;
				}
				case TextImageRelation.ImageBeforeText:
					LayoutTextBeforeOrAfterImage(contentRectangle, textFirst: false, text_size, GetImageSize(), text_align, image_align, out text_rect, out image_rect);
					break;
				case TextImageRelation.TextBeforeImage:
					LayoutTextBeforeOrAfterImage(contentRectangle, textFirst: true, text_size, GetImageSize(), text_align, image_align, out text_rect, out image_rect);
					break;
				case (TextImageRelation)3:
				case (TextImageRelation)5:
				case (TextImageRelation)6:
				case (TextImageRelation)7:
					break;
				}
			}
			break;
		}
	}

	internal virtual void Dismiss(ToolStripDropDownCloseReason reason)
	{
		if (is_selected)
		{
			is_selected = false;
			Invalidate();
			OnUIASelectionChanged();
		}
	}

	internal virtual ToolStrip GetTopLevelToolStrip()
	{
		if (Parent != null)
		{
			return Parent.GetTopLevelToolStrip();
		}
		return null;
	}

	private void LayoutTextBeforeOrAfterImage(Rectangle totalArea, bool textFirst, Size textSize, Size imageSize, ContentAlignment textAlign, ContentAlignment imageAlign, out Rectangle textRect, out Rectangle imageRect)
	{
		int num = 0;
		int num2 = textSize.Width + num + imageSize.Width;
		int num3 = totalArea.Width - num2;
		int num4 = 0;
		HorizontalAlignment horizontalAlignment = GetHorizontalAlignment(textAlign);
		HorizontalAlignment horizontalAlignment2 = GetHorizontalAlignment(imageAlign);
		num4 = ((horizontalAlignment2 != 0) ? ((horizontalAlignment2 == HorizontalAlignment.Right && horizontalAlignment == HorizontalAlignment.Right) ? num3 : ((horizontalAlignment2 != HorizontalAlignment.Center || (horizontalAlignment != 0 && horizontalAlignment != HorizontalAlignment.Center)) ? (num4 + 2 * (num3 / 3)) : (num4 + num3 / 3))) : 0);
		Rectangle rectangle;
		Rectangle rectangle2;
		if (textFirst)
		{
			rectangle = new Rectangle(totalArea.Left + num4, AlignInRectangle(totalArea, textSize, textAlign).Top, textSize.Width, textSize.Height);
			rectangle2 = new Rectangle(rectangle.Right + num, AlignInRectangle(totalArea, imageSize, imageAlign).Top, imageSize.Width, imageSize.Height);
		}
		else
		{
			rectangle2 = new Rectangle(totalArea.Left + num4, AlignInRectangle(totalArea, imageSize, imageAlign).Top, imageSize.Width, imageSize.Height);
			rectangle = new Rectangle(rectangle2.Right + num, AlignInRectangle(totalArea, textSize, textAlign).Top, textSize.Width, textSize.Height);
		}
		textRect = rectangle;
		imageRect = rectangle2;
	}

	private HorizontalAlignment GetHorizontalAlignment(ContentAlignment align)
	{
		switch (align)
		{
		case ContentAlignment.TopLeft:
		case ContentAlignment.MiddleLeft:
		case ContentAlignment.BottomLeft:
			return HorizontalAlignment.Left;
		case ContentAlignment.TopCenter:
		case ContentAlignment.MiddleCenter:
		case ContentAlignment.BottomCenter:
			return HorizontalAlignment.Center;
		case ContentAlignment.TopRight:
		case ContentAlignment.MiddleRight:
		case ContentAlignment.BottomRight:
			return HorizontalAlignment.Right;
		default:
			return HorizontalAlignment.Left;
		}
	}

	internal Size GetImageSize()
	{
		if (image_scaling == ToolStripItemImageScaling.None)
		{
			if (image != null)
			{
				return image.Size;
			}
			if ((image_index >= 0 || !string.IsNullOrEmpty(image_key)) && owner != null && owner.ImageList != null)
			{
				return owner.ImageList.ImageSize;
			}
		}
		else
		{
			if (Parent == null)
			{
				return Size.Empty;
			}
			if (image != null)
			{
				return Parent.ImageScalingSize;
			}
			if ((image_index >= 0 || !string.IsNullOrEmpty(image_key)) && owner != null && owner.ImageList != null)
			{
				return Parent.ImageScalingSize;
			}
		}
		return Size.Empty;
	}

	internal string GetToolTip()
	{
		if (auto_tool_tip && string.IsNullOrEmpty(tool_tip_text))
		{
			return Text;
		}
		return tool_tip_text;
	}

	internal void FireEvent(EventArgs e, ToolStripItemEventType met)
	{
		if (Enabled || met == ToolStripItemEventType.Paint)
		{
			switch (met)
			{
			case ToolStripItemEventType.MouseUp:
				HandleClick(e);
				OnMouseUp((MouseEventArgs)e);
				break;
			case ToolStripItemEventType.MouseDown:
				OnMouseDown((MouseEventArgs)e);
				break;
			case ToolStripItemEventType.MouseEnter:
				OnMouseEnter(e);
				break;
			case ToolStripItemEventType.MouseHover:
				OnMouseHover(e);
				break;
			case ToolStripItemEventType.MouseLeave:
				OnMouseLeave(e);
				break;
			case ToolStripItemEventType.MouseMove:
				OnMouseMove((MouseEventArgs)e);
				break;
			case ToolStripItemEventType.Paint:
				OnPaint((PaintEventArgs)e);
				break;
			case ToolStripItemEventType.Click:
				HandleClick(e);
				break;
			}
		}
	}

	internal virtual void HandleClick(EventArgs e)
	{
		Parent.HandleItemClick(this);
		OnClick(e);
	}

	internal virtual void SetPlacement(ToolStripItemPlacement placement)
	{
		this.placement = placement;
	}

	private void BeginAnimation()
	{
		if (image != null && ImageAnimator.CanAnimate(image))
		{
			frame_handler = OnAnimateImage;
			ImageAnimator.Animate(image, frame_handler);
		}
	}

	private void OnAnimateImage(object sender, EventArgs e)
	{
		if (Parent != null && Parent.IsHandleCreated)
		{
			Parent.BeginInvoke(new EventHandler(UpdateAnimatedImage), this, e);
		}
	}

	private void StopAnimation()
	{
		if (frame_handler != null)
		{
			ImageAnimator.StopAnimate(image, frame_handler);
			frame_handler = null;
		}
	}

	private void UpdateAnimatedImage(object sender, EventArgs e)
	{
		if (Parent != null && Parent.IsHandleCreated)
		{
			ImageAnimator.UpdateFrames(image);
			Invalidate();
		}
	}

	internal void OnUIASelectionChanged()
	{
		((EventHandler)base.Events[UIASelectionChanged])?.Invoke(this, EventArgs.Empty);
	}
}
