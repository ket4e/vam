using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[DesignerSerializer("System.Windows.Forms.Design.ToolStripCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("ItemClicked")]
[DefaultProperty("Items")]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.ToolStripDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public class ToolStrip : ScrollableControl, IDisposable, IComponent, IToolStripData
{
	[ComVisible(true)]
	public class ToolStripAccessibleObject : ControlAccessibleObject
	{
		public override AccessibleRole Role => AccessibleRole.ToolBar;

		public ToolStripAccessibleObject(ToolStrip owner)
			: base(owner)
		{
		}

		public override AccessibleObject GetChild(int index)
		{
			return base.GetChild(index);
		}

		public override int GetChildCount()
		{
			return (owner as ToolStrip).Items.Count;
		}

		public override AccessibleObject HitTest(int x, int y)
		{
			return base.HitTest(x, y);
		}
	}

	private bool allow_item_reorder;

	private bool allow_merge;

	private Color back_color;

	private bool can_overflow;

	private ToolStrip currently_merged_with;

	private ToolStripDropDownDirection default_drop_down_direction;

	internal ToolStripItemCollection displayed_items;

	private Color fore_color;

	private Padding grip_margin;

	private ToolStripGripStyle grip_style;

	private List<ToolStripItem> hidden_merged_items;

	private ImageList image_list;

	private Size image_scaling_size;

	private bool is_currently_merged;

	private ToolStripItemCollection items;

	private bool keyboard_active;

	private LayoutEngine layout_engine;

	private LayoutSettings layout_settings;

	private ToolStripLayoutStyle layout_style;

	private Orientation orientation;

	private ToolStripOverflowButton overflow_button;

	private List<ToolStripItem> pre_merge_items;

	private ToolStripRenderer renderer;

	private ToolStripRenderMode render_mode;

	private ToolStripTextDirection text_direction;

	private Timer tooltip_timer;

	private ToolTip tooltip_window;

	private bool show_item_tool_tips;

	private bool stretch;

	private ToolStripItem mouse_currently_over;

	internal bool menu_selected;

	private ToolStripItem tooltip_currently_showing;

	private static object BeginDragEvent;

	private static object EndDragEvent;

	private static object ItemAddedEvent;

	private static object ItemClickedEvent;

	private static object ItemRemovedEvent;

	private static object LayoutCompletedEvent;

	private static object LayoutStyleChangedEvent;

	private static object PaintGripEvent;

	private static object RendererChangedEvent;

	[System.MonoTODO("Stub, does nothing")]
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

	[DefaultValue(false)]
	[System.MonoTODO("Stub, does nothing")]
	public bool AllowItemReorder
	{
		get
		{
			return allow_item_reorder;
		}
		set
		{
			allow_item_reorder = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowMerge
	{
		get
		{
			return allow_merge;
		}
		set
		{
			allow_merge = value;
		}
	}

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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override bool AutoScroll
	{
		get
		{
			return base.AutoScroll;
		}
		set
		{
			base.AutoScroll = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Size AutoScrollMargin
	{
		get
		{
			return base.AutoScrollMargin;
		}
		set
		{
			base.AutoScrollMargin = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Size AutoScrollMinSize
	{
		get
		{
			return base.AutoScrollMinSize;
		}
		set
		{
			base.AutoScrollMinSize = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Point AutoScrollPosition
	{
		get
		{
			return base.AutoScrollPosition;
		}
		set
		{
			base.AutoScrollPosition = value;
		}
	}

	[DefaultValue(true)]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
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

	public new Color BackColor
	{
		get
		{
			return back_color;
		}
		set
		{
			back_color = value;
		}
	}

	public override BindingContext BindingContext
	{
		get
		{
			return base.BindingContext;
		}
		set
		{
			base.BindingContext = value;
		}
	}

	[DefaultValue(true)]
	public bool CanOverflow
	{
		get
		{
			return can_overflow;
		}
		set
		{
			can_overflow = value;
		}
	}

	[Browsable(false)]
	[DefaultValue(false)]
	public new bool CausesValidation
	{
		get
		{
			return base.CausesValidation;
		}
		set
		{
			base.CausesValidation = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ControlCollection Controls => base.Controls;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Cursor Cursor
	{
		get
		{
			return base.Cursor;
		}
		set
		{
			base.Cursor = value;
		}
	}

	[Browsable(false)]
	public virtual ToolStripDropDownDirection DefaultDropDownDirection
	{
		get
		{
			return default_drop_down_direction;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ToolStripDropDownDirection), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripDropDownDirection");
			}
			default_drop_down_direction = value;
		}
	}

	public override Rectangle DisplayRectangle
	{
		get
		{
			if (orientation == Orientation.Horizontal)
			{
				if (grip_style == ToolStripGripStyle.Hidden || layout_style == ToolStripLayoutStyle.Flow || layout_style == ToolStripLayoutStyle.Table)
				{
					return new Rectangle(base.Padding.Left, base.Padding.Top, base.Width - base.Padding.Horizontal, base.Height - base.Padding.Vertical);
				}
				return new Rectangle(GripRectangle.Right + GripMargin.Right, base.Padding.Top, base.Width - base.Padding.Horizontal - GripRectangle.Right - GripMargin.Right, base.Height - base.Padding.Vertical);
			}
			if (grip_style == ToolStripGripStyle.Hidden || layout_style == ToolStripLayoutStyle.Flow || layout_style == ToolStripLayoutStyle.Table)
			{
				return new Rectangle(base.Padding.Left, base.Padding.Top, base.Width - base.Padding.Horizontal, base.Height - base.Padding.Vertical);
			}
			return new Rectangle(base.Padding.Left, GripRectangle.Bottom + GripMargin.Bottom + base.Padding.Top, base.Width - base.Padding.Horizontal, base.Height - base.Padding.Vertical - GripRectangle.Bottom - GripMargin.Bottom);
		}
	}

	[DefaultValue(DockStyle.Top)]
	public override DockStyle Dock
	{
		get
		{
			return base.Dock;
		}
		set
		{
			if (base.Dock != value)
			{
				base.Dock = value;
				switch (value)
				{
				case DockStyle.None:
				case DockStyle.Top:
				case DockStyle.Bottom:
					LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
					break;
				case DockStyle.Left:
				case DockStyle.Right:
					LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
					break;
				}
			}
		}
	}

	public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			if (base.Font == value)
			{
				return;
			}
			base.Font = value;
			foreach (ToolStripItem item in Items)
			{
				item.OnOwnerFontChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	public new Color ForeColor
	{
		get
		{
			return fore_color;
		}
		set
		{
			if (fore_color != value)
			{
				fore_color = value;
				OnForeColorChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	public ToolStripGripDisplayStyle GripDisplayStyle => (orientation != Orientation.Vertical) ? ToolStripGripDisplayStyle.Vertical : ToolStripGripDisplayStyle.Horizontal;

	public Padding GripMargin
	{
		get
		{
			return grip_margin;
		}
		set
		{
			if (grip_margin != value)
			{
				grip_margin = value;
				PerformLayout();
			}
		}
	}

	[Browsable(false)]
	public Rectangle GripRectangle
	{
		get
		{
			if (grip_style == ToolStripGripStyle.Hidden)
			{
				return Rectangle.Empty;
			}
			if (orientation == Orientation.Horizontal)
			{
				return new Rectangle(grip_margin.Left + base.Padding.Left, base.Padding.Top, 3, base.Height);
			}
			return new Rectangle(base.Padding.Left, grip_margin.Top + base.Padding.Top, base.Width, 3);
		}
	}

	[DefaultValue(ToolStripGripStyle.Visible)]
	public ToolStripGripStyle GripStyle
	{
		get
		{
			return grip_style;
		}
		set
		{
			if (grip_style != value)
			{
				if (!Enum.IsDefined(typeof(ToolStripGripStyle), value))
				{
					throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripGripStyle");
				}
				grip_style = value;
				PerformLayout(this, "GripStyle");
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new bool HasChildren => base.HasChildren;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new HScrollProperties HorizontalScroll => base.HorizontalScroll;

	[DefaultValue(null)]
	[Browsable(false)]
	public ImageList ImageList
	{
		get
		{
			return image_list;
		}
		set
		{
			image_list = value;
		}
	}

	[DefaultValue("{Width=16, Height=16}")]
	public Size ImageScalingSize
	{
		get
		{
			return image_scaling_size;
		}
		set
		{
			image_scaling_size = value;
		}
	}

	[System.MonoTODO("Always returns false, dragging not implemented yet.")]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public bool IsCurrentlyDragging => false;

	[Browsable(false)]
	public bool IsDropDown
	{
		get
		{
			if (this is ToolStripDropDown)
			{
				return true;
			}
			return false;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MergableProperty(false)]
	public virtual ToolStripItemCollection Items => items;

	public override LayoutEngine LayoutEngine
	{
		get
		{
			if (layout_engine == null)
			{
				layout_engine = new ToolStripSplitStackLayout();
			}
			return layout_engine;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[DefaultValue(null)]
	public LayoutSettings LayoutSettings
	{
		get
		{
			return layout_settings;
		}
		set
		{
			if (layout_settings != value)
			{
				layout_settings = value;
				PerformLayout(this, "LayoutSettings");
			}
		}
	}

	[AmbientValue(ToolStripLayoutStyle.StackWithOverflow)]
	public ToolStripLayoutStyle LayoutStyle
	{
		get
		{
			return layout_style;
		}
		set
		{
			if (layout_style == value)
			{
				return;
			}
			if (!Enum.IsDefined(typeof(ToolStripLayoutStyle), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripLayoutStyle");
			}
			layout_style = value;
			if (layout_style == ToolStripLayoutStyle.Flow)
			{
				layout_engine = new FlowLayout();
			}
			else
			{
				layout_engine = new ToolStripSplitStackLayout();
			}
			if (layout_style == ToolStripLayoutStyle.StackWithOverflow)
			{
				if (Dock == DockStyle.Left || Dock == DockStyle.Right)
				{
					layout_style = ToolStripLayoutStyle.VerticalStackWithOverflow;
				}
				else
				{
					layout_style = ToolStripLayoutStyle.HorizontalStackWithOverflow;
				}
			}
			if (layout_style == ToolStripLayoutStyle.HorizontalStackWithOverflow)
			{
				orientation = Orientation.Horizontal;
			}
			else if (layout_style == ToolStripLayoutStyle.VerticalStackWithOverflow)
			{
				orientation = Orientation.Vertical;
			}
			layout_settings = CreateLayoutSettings(value);
			PerformLayout(this, "LayoutStyle");
			OnLayoutStyleChanged(EventArgs.Empty);
		}
	}

	[Browsable(false)]
	public Orientation Orientation => orientation;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public ToolStripOverflowButton OverflowButton => overflow_button;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public ToolStripRenderer Renderer
	{
		get
		{
			if (render_mode == ToolStripRenderMode.ManagerRenderMode)
			{
				return ToolStripManager.Renderer;
			}
			return renderer;
		}
		set
		{
			if (renderer != value)
			{
				renderer = value;
				render_mode = ToolStripRenderMode.Custom;
				PerformLayout(this, "Renderer");
				OnRendererChanged(EventArgs.Empty);
			}
		}
	}

	public ToolStripRenderMode RenderMode
	{
		get
		{
			return render_mode;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ToolStripRenderMode), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripRenderMode");
			}
			if (value == ToolStripRenderMode.Custom && renderer == null)
			{
				throw new NotSupportedException("Must set Renderer property before setting RenderMode to Custom");
			}
			switch (value)
			{
			case ToolStripRenderMode.Professional:
				Renderer = new ToolStripProfessionalRenderer();
				break;
			case ToolStripRenderMode.System:
				Renderer = new ToolStripSystemRenderer();
				break;
			}
			render_mode = value;
		}
	}

	[DefaultValue(true)]
	public bool ShowItemToolTips
	{
		get
		{
			return show_item_tool_tips;
		}
		set
		{
			show_item_tool_tips = value;
		}
	}

	[DefaultValue(false)]
	public bool Stretch
	{
		get
		{
			return stretch;
		}
		set
		{
			stretch = value;
		}
	}

	[DefaultValue(false)]
	[DispId(-516)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			base.TabStop = value;
			SetStyle(ControlStyles.Selectable, value);
		}
	}

	[DefaultValue(ToolStripTextDirection.Horizontal)]
	public virtual ToolStripTextDirection TextDirection
	{
		get
		{
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
				PerformLayout(this, "TextDirection");
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new VScrollProperties VerticalScroll => base.VerticalScroll;

	protected virtual DockStyle DefaultDock => DockStyle.Top;

	protected virtual Padding DefaultGripMargin => new Padding(2);

	protected override Padding DefaultMargin => Padding.Empty;

	protected override Padding DefaultPadding => new Padding(0, 0, 1, 0);

	protected virtual bool DefaultShowItemToolTips => true;

	protected override Size DefaultSize => new Size(100, 25);

	protected internal virtual ToolStripItemCollection DisplayedItems => displayed_items;

	protected internal virtual Size MaxItemSize => new Size(base.Width - ((GripStyle == ToolStripGripStyle.Hidden) ? 1 : 8), base.Height);

	internal virtual bool KeyboardActive
	{
		get
		{
			return keyboard_active;
		}
		set
		{
			if (keyboard_active != value)
			{
				keyboard_active = value;
				if (value)
				{
					Application.KeyboardCapture = this;
				}
				else if (Application.KeyboardCapture == this)
				{
					Application.KeyboardCapture = null;
					ToolStripManager.ActivatedByKeyboard = false;
				}
				Invalidate();
			}
		}
	}

	private Timer ToolTipTimer
	{
		get
		{
			if (tooltip_timer == null)
			{
				tooltip_timer = new Timer();
				tooltip_timer.Enabled = false;
				tooltip_timer.Interval = 500;
				tooltip_timer.Tick += ToolTipTimer_Tick;
			}
			return tooltip_timer;
		}
	}

	private ToolTip ToolTipWindow
	{
		get
		{
			if (tooltip_window == null)
			{
				tooltip_window = new ToolTip();
			}
			return tooltip_window;
		}
	}

	internal ToolStrip CurrentlyMergedWith
	{
		get
		{
			return currently_merged_with;
		}
		set
		{
			currently_merged_with = value;
		}
	}

	internal List<ToolStripItem> HiddenMergedItems
	{
		get
		{
			if (hidden_merged_items == null)
			{
				hidden_merged_items = new List<ToolStripItem>();
			}
			return hidden_merged_items;
		}
	}

	internal bool IsCurrentlyMerged
	{
		get
		{
			return is_currently_merged;
		}
		set
		{
			is_currently_merged = value;
			if (value || !(this is MenuStrip))
			{
				return;
			}
			foreach (ToolStripMenuItem item in Items)
			{
				item.DropDown.IsCurrentlyMerged = value;
			}
		}
	}

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

	[System.MonoTODO("Event never raised")]
	public event EventHandler BeginDrag
	{
		add
		{
			base.Events.AddHandler(BeginDragEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BeginDragEvent, value);
		}
	}

	[Browsable(false)]
	public new event EventHandler CausesValidationChanged
	{
		add
		{
			base.CausesValidationChanged += value;
		}
		remove
		{
			base.CausesValidationChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event ControlEventHandler ControlAdded
	{
		add
		{
			base.ControlAdded += value;
		}
		remove
		{
			base.ControlAdded -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event ControlEventHandler ControlRemoved
	{
		add
		{
			base.ControlRemoved += value;
		}
		remove
		{
			base.ControlRemoved -= value;
		}
	}

	[Browsable(false)]
	public new event EventHandler CursorChanged
	{
		add
		{
			base.CursorChanged += value;
		}
		remove
		{
			base.CursorChanged -= value;
		}
	}

	[System.MonoTODO("Event never raised")]
	public event EventHandler EndDrag
	{
		add
		{
			base.Events.AddHandler(EndDragEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(EndDragEvent, value);
		}
	}

	[Browsable(false)]
	public new event EventHandler ForeColorChanged
	{
		add
		{
			base.ForeColorChanged += value;
		}
		remove
		{
			base.ForeColorChanged -= value;
		}
	}

	public event ToolStripItemEventHandler ItemAdded
	{
		add
		{
			base.Events.AddHandler(ItemAddedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemAddedEvent, value);
		}
	}

	public event ToolStripItemClickedEventHandler ItemClicked
	{
		add
		{
			base.Events.AddHandler(ItemClickedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemClickedEvent, value);
		}
	}

	public event ToolStripItemEventHandler ItemRemoved
	{
		add
		{
			base.Events.AddHandler(ItemRemovedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemRemovedEvent, value);
		}
	}

	public event EventHandler LayoutCompleted
	{
		add
		{
			base.Events.AddHandler(LayoutCompletedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LayoutCompletedEvent, value);
		}
	}

	public event EventHandler LayoutStyleChanged
	{
		add
		{
			base.Events.AddHandler(LayoutStyleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LayoutStyleChangedEvent, value);
		}
	}

	public event PaintEventHandler PaintGrip
	{
		add
		{
			base.Events.AddHandler(PaintGripEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PaintGripEvent, value);
		}
	}

	public event EventHandler RendererChanged
	{
		add
		{
			base.Events.AddHandler(RendererChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RendererChangedEvent, value);
		}
	}

	public ToolStrip()
		: this((ToolStripItem[])null)
	{
	}

	public ToolStrip(params ToolStripItem[] items)
	{
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
		SetStyle(ControlStyles.Selectable, value: false);
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
		SuspendLayout();
		this.items = new ToolStripItemCollection(this, items, internalcreated: true);
		allow_merge = true;
		base.AutoSize = true;
		SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
		back_color = Control.DefaultBackColor;
		can_overflow = true;
		base.CausesValidation = false;
		default_drop_down_direction = ToolStripDropDownDirection.BelowRight;
		displayed_items = new ToolStripItemCollection(this, null, internalcreated: true);
		Dock = DefaultDock;
		base.Font = new Font("Tahoma", 8.25f);
		fore_color = Control.DefaultForeColor;
		grip_margin = DefaultGripMargin;
		grip_style = ToolStripGripStyle.Visible;
		image_scaling_size = new Size(16, 16);
		layout_style = ToolStripLayoutStyle.HorizontalStackWithOverflow;
		orientation = Orientation.Horizontal;
		if (!(this is ToolStripDropDown))
		{
			overflow_button = new ToolStripOverflowButton(this);
		}
		renderer = null;
		render_mode = ToolStripRenderMode.ManagerRenderMode;
		show_item_tool_tips = DefaultShowItemToolTips;
		base.TabStop = false;
		text_direction = ToolStripTextDirection.Horizontal;
		ResumeLayout();
		ToolStripManager.AddToolStrip(this);
	}

	static ToolStrip()
	{
		BeginDrag = new object();
		EndDrag = new object();
		ItemAdded = new object();
		ItemClicked = new object();
		ItemRemoved = new object();
		LayoutCompleted = new object();
		LayoutStyleChanged = new object();
		PaintGrip = new object();
		RendererChanged = new object();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Control GetChildAtPoint(Point point)
	{
		return base.GetChildAtPoint(point);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
	{
		return base.GetChildAtPoint(pt, skipValue);
	}

	public ToolStripItem GetItemAt(Point point)
	{
		foreach (ToolStripItem displayed_item in displayed_items)
		{
			if (displayed_item.Visible && displayed_item.Bounds.Contains(point))
			{
				return displayed_item;
			}
		}
		return null;
	}

	public ToolStripItem GetItemAt(int x, int y)
	{
		return GetItemAt(new Point(x, y));
	}

	public virtual ToolStripItem GetNextItem(ToolStripItem start, ArrowDirection direction)
	{
		if (!Enum.IsDefined(typeof(ArrowDirection), direction))
		{
			throw new InvalidEnumArgumentException($"Enum argument value '{direction}' is not valid for ArrowDirection");
		}
		ToolStripItem toolStripItem = null;
		switch (direction)
		{
		case ArrowDirection.Right:
		{
			int num = int.MaxValue;
			if (start != null)
			{
				foreach (ToolStripItem displayedItem in DisplayedItems)
				{
					if (displayedItem.Left >= start.Right && displayedItem.Left < num && displayedItem.Visible && displayedItem.CanSelect)
					{
						toolStripItem = displayedItem;
						num = displayedItem.Left;
					}
				}
			}
			if (toolStripItem != null)
			{
				break;
			}
			foreach (ToolStripItem displayedItem2 in DisplayedItems)
			{
				if (displayedItem2.Left < num && displayedItem2.Visible && displayedItem2.CanSelect)
				{
					toolStripItem = displayedItem2;
					num = displayedItem2.Left;
				}
			}
			break;
		}
		case ArrowDirection.Up:
		{
			int num = int.MinValue;
			if (start != null)
			{
				foreach (ToolStripItem displayedItem3 in DisplayedItems)
				{
					if (displayedItem3.Bottom <= start.Top && displayedItem3.Top > num && displayedItem3.Visible && displayedItem3.CanSelect)
					{
						toolStripItem = displayedItem3;
						num = displayedItem3.Top;
					}
				}
			}
			if (toolStripItem != null)
			{
				break;
			}
			foreach (ToolStripItem displayedItem4 in DisplayedItems)
			{
				if (displayedItem4.Top > num && displayedItem4.Visible && displayedItem4.CanSelect)
				{
					toolStripItem = displayedItem4;
					num = displayedItem4.Top;
				}
			}
			break;
		}
		case ArrowDirection.Left:
		{
			int num = int.MinValue;
			if (start != null)
			{
				foreach (ToolStripItem displayedItem5 in DisplayedItems)
				{
					if (displayedItem5.Right <= start.Left && displayedItem5.Left > num && displayedItem5.Visible && displayedItem5.CanSelect)
					{
						toolStripItem = displayedItem5;
						num = displayedItem5.Left;
					}
				}
			}
			if (toolStripItem != null)
			{
				break;
			}
			foreach (ToolStripItem displayedItem6 in DisplayedItems)
			{
				if (displayedItem6.Left > num && displayedItem6.Visible && displayedItem6.CanSelect)
				{
					toolStripItem = displayedItem6;
					num = displayedItem6.Left;
				}
			}
			break;
		}
		case ArrowDirection.Down:
		{
			int num = int.MaxValue;
			if (start != null)
			{
				foreach (ToolStripItem displayedItem7 in DisplayedItems)
				{
					if (displayedItem7.Top >= start.Bottom && displayedItem7.Bottom < num && displayedItem7.Visible && displayedItem7.CanSelect)
					{
						toolStripItem = displayedItem7;
						num = displayedItem7.Top;
					}
				}
			}
			if (toolStripItem != null)
			{
				break;
			}
			foreach (ToolStripItem displayedItem8 in DisplayedItems)
			{
				if (displayedItem8.Top < num && displayedItem8.Visible && displayedItem8.CanSelect)
				{
					toolStripItem = displayedItem8;
					num = displayedItem8.Top;
				}
			}
			break;
		}
		}
		return toolStripItem;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ResetMinimumSize()
	{
		MinimumSize = new Size(-1, -1);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void SetAutoScrollMargin(int x, int y)
	{
		base.SetAutoScrollMargin(x, y);
	}

	public override string ToString()
	{
		return $"{base.ToString()}, Name: {base.Name}, Items: {items.Count.ToString()}";
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripAccessibleObject(this);
	}

	protected override ControlCollection CreateControlsInstance()
	{
		return base.CreateControlsInstance();
	}

	protected internal virtual ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick)
	{
		if (text == "-")
		{
			return new ToolStripSeparator();
		}
		if (this is ToolStripDropDown)
		{
			return new ToolStripMenuItem(text, image, onClick);
		}
		return new ToolStripButton(text, image, onClick);
	}

	protected virtual LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle layoutStyle)
	{
		return layoutStyle switch
		{
			ToolStripLayoutStyle.Flow => new FlowLayoutSettings(this), 
			_ => null, 
		};
	}

	protected override void Dispose(bool disposing)
	{
		if (!base.IsDisposed)
		{
			for (int num = Items.Count - 1; num >= 0; num--)
			{
				Items[num].Dispose();
			}
			if (overflow_button != null && overflow_button.drop_down != null)
			{
				overflow_button.drop_down.Dispose();
			}
			ToolStripManager.RemoveToolStrip(this);
			base.Dispose(disposing);
		}
	}

	[System.MonoTODO("Stub, never called")]
	protected virtual void OnBeginDrag(EventArgs e)
	{
		((EventHandler)base.Events[BeginDrag])?.Invoke(this, e);
	}

	protected override void OnDockChanged(EventArgs e)
	{
		base.OnDockChanged(e);
	}

	[System.MonoTODO("Stub, never called")]
	protected virtual void OnEndDrag(EventArgs e)
	{
		((EventHandler)base.Events[EndDrag])?.Invoke(this, e);
	}

	protected override bool IsInputChar(char charCode)
	{
		return base.IsInputChar(charCode);
	}

	protected override bool IsInputKey(Keys keyData)
	{
		return base.IsInputKey(keyData);
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
		foreach (ToolStripItem item in Items)
		{
			item.OnParentEnabledChanged(EventArgs.Empty);
		}
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void OnInvalidated(InvalidateEventArgs e)
	{
		base.OnInvalidated(e);
	}

	protected internal virtual void OnItemAdded(ToolStripItemEventArgs e)
	{
		if (e.Item.InternalVisible)
		{
			e.Item.Available = true;
		}
		e.Item.SetPlacement(ToolStripItemPlacement.Main);
		if (base.Created)
		{
			PerformLayout();
		}
		((ToolStripItemEventHandler)base.Events[ItemAdded])?.Invoke(this, e);
	}

	protected virtual void OnItemClicked(ToolStripItemClickedEventArgs e)
	{
		if (KeyboardActive)
		{
			ToolStripManager.SetActiveToolStrip(null, keyboard: false);
		}
		((ToolStripItemClickedEventHandler)base.Events[ItemClicked])?.Invoke(this, e);
	}

	protected internal virtual void OnItemRemoved(ToolStripItemEventArgs e)
	{
		((ToolStripItemEventHandler)base.Events[ItemRemoved])?.Invoke(this, e);
	}

	protected override void OnLayout(LayoutEventArgs e)
	{
		base.OnLayout(e);
		SetDisplayedItems();
		OnLayoutCompleted(EventArgs.Empty);
		Invalidate();
	}

	protected virtual void OnLayoutCompleted(EventArgs e)
	{
		((EventHandler)base.Events[LayoutCompleted])?.Invoke(this, e);
	}

	protected virtual void OnLayoutStyleChanged(EventArgs e)
	{
		((EventHandler)base.Events[LayoutStyleChanged])?.Invoke(this, e);
	}

	protected override void OnLeave(EventArgs e)
	{
		base.OnLeave(e);
	}

	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
	}

	protected override void OnMouseCaptureChanged(EventArgs e)
	{
		base.OnMouseCaptureChanged(e);
	}

	protected override void OnMouseDown(MouseEventArgs mea)
	{
		if (mouse_currently_over != null)
		{
			ToolStripItem currentlyFocusedItem = GetCurrentlyFocusedItem();
			if (currentlyFocusedItem != null && currentlyFocusedItem != mouse_currently_over)
			{
				FocusInternal(skip_check: true);
			}
			if (this is MenuStrip && !menu_selected)
			{
				(this as MenuStrip).FireMenuActivate();
				menu_selected = true;
			}
			mouse_currently_over.FireEvent(mea, ToolStripItemEventType.MouseDown);
			if (this is MenuStrip && mouse_currently_over is ToolStripMenuItem && !(mouse_currently_over as ToolStripMenuItem).HasDropDownItems)
			{
				return;
			}
		}
		else
		{
			HideMenus(release: true, ToolStripDropDownCloseReason.AppClicked);
		}
		if (this is MenuStrip)
		{
			base.Capture = false;
		}
		base.OnMouseDown(mea);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		if (mouse_currently_over != null)
		{
			MouseLeftItem(mouse_currently_over);
			mouse_currently_over.FireEvent(e, ToolStripItemEventType.MouseLeave);
			mouse_currently_over = null;
		}
		base.OnMouseLeave(e);
	}

	protected override void OnMouseMove(MouseEventArgs mea)
	{
		ToolStripItem toolStripItem = ((overflow_button == null || !overflow_button.Visible || !overflow_button.Bounds.Contains(mea.Location)) ? GetItemAt(mea.X, mea.Y) : overflow_button);
		if (toolStripItem != null)
		{
			if (toolStripItem == mouse_currently_over)
			{
				toolStripItem.FireEvent(mea, ToolStripItemEventType.MouseMove);
			}
			else
			{
				if (mouse_currently_over != null)
				{
					MouseLeftItem(toolStripItem);
					mouse_currently_over.FireEvent(mea, ToolStripItemEventType.MouseLeave);
				}
				mouse_currently_over = toolStripItem;
				toolStripItem.FireEvent(mea, ToolStripItemEventType.MouseEnter);
				MouseEnteredItem(toolStripItem);
				toolStripItem.FireEvent(mea, ToolStripItemEventType.MouseMove);
				if (menu_selected && mouse_currently_over.Enabled && mouse_currently_over is ToolStripDropDownItem && (mouse_currently_over as ToolStripDropDownItem).HasDropDownItems)
				{
					(mouse_currently_over as ToolStripDropDownItem).ShowDropDown();
				}
			}
		}
		else if (mouse_currently_over != null)
		{
			MouseLeftItem(toolStripItem);
			mouse_currently_over.FireEvent(mea, ToolStripItemEventType.MouseLeave);
			mouse_currently_over = null;
		}
		base.OnMouseMove(mea);
	}

	protected override void OnMouseUp(MouseEventArgs mea)
	{
		if (mouse_currently_over != null && !(mouse_currently_over is ToolStripControlHost) && mouse_currently_over.Enabled)
		{
			OnItemClicked(new ToolStripItemClickedEventArgs(mouse_currently_over));
			if (mouse_currently_over != null)
			{
				mouse_currently_over.FireEvent(mea, ToolStripItemEventType.MouseUp);
			}
			if (mouse_currently_over == null)
			{
				return;
			}
		}
		base.OnMouseUp(mea);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		OnPaintGrip(e);
		for (int i = 0; i < displayed_items.Count; i++)
		{
			ToolStripItem toolStripItem = displayed_items[i];
			if (toolStripItem.Visible)
			{
				e.Graphics.TranslateTransform(toolStripItem.Bounds.Left, toolStripItem.Bounds.Top);
				toolStripItem.FireEvent(e, ToolStripItemEventType.Paint);
				e.Graphics.ResetTransform();
			}
		}
		if (overflow_button != null && overflow_button.Visible)
		{
			e.Graphics.TranslateTransform(overflow_button.Bounds.Left, overflow_button.Bounds.Top);
			overflow_button.FireEvent(e, ToolStripItemEventType.Paint);
			e.Graphics.ResetTransform();
		}
		ToolStripRenderEventArgs toolStripRenderEventArgs = new ToolStripRenderEventArgs(affectedBounds: new Rectangle(Point.Empty, base.Size), g: e.Graphics, toolStrip: this, backColor: Color.Empty);
		toolStripRenderEventArgs.InternalConnectedArea = CalculateConnectedArea();
		Renderer.DrawToolStripBorder(toolStripRenderEventArgs);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
		ToolStripRenderEventArgs e2 = new ToolStripRenderEventArgs(affectedBounds: new Rectangle(Point.Empty, base.Size), g: e.Graphics, toolStrip: this, backColor: SystemColors.Control);
		Renderer.DrawToolStripBackground(e2);
	}

	protected internal virtual void OnPaintGrip(PaintEventArgs e)
	{
		if (layout_style == ToolStripLayoutStyle.Flow || layout_style == ToolStripLayoutStyle.Table)
		{
			return;
		}
		((PaintEventHandler)base.Events[PaintGrip])?.Invoke(this, e);
		if (!(this is MenuStrip))
		{
			if (orientation == Orientation.Horizontal)
			{
				e.Graphics.TranslateTransform(2f, 0f);
			}
			else
			{
				e.Graphics.TranslateTransform(0f, 2f);
			}
		}
		Renderer.DrawGrip(new ToolStripGripRenderEventArgs(e.Graphics, this, GripRectangle, GripDisplayStyle, grip_style));
		e.Graphics.ResetTransform();
	}

	protected virtual void OnRendererChanged(EventArgs e)
	{
		((EventHandler)base.Events[RendererChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnRightToLeftChanged(EventArgs e)
	{
		base.OnRightToLeftChanged(e);
		foreach (ToolStripItem item in Items)
		{
			item.OnParentRightToLeftChanged(e);
		}
	}

	protected override void OnScroll(ScrollEventArgs se)
	{
		base.OnScroll(se);
	}

	protected override void OnTabStopChanged(EventArgs e)
	{
		base.OnTabStopChanged(e);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
	}

	protected override bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		return base.ProcessCmdKey(ref m, keyData);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		if (!KeyboardActive)
		{
			return false;
		}
		foreach (ToolStripItem item in Items)
		{
			if (item.ProcessDialogKey(keyData))
			{
				return true;
			}
		}
		if (ProcessArrowKey(keyData))
		{
			return true;
		}
		ToolStrip toolStrip = null;
		switch (keyData)
		{
		case Keys.Escape:
			Dismiss(ToolStripDropDownCloseReason.Keyboard);
			return true;
		case Keys.Tab | Keys.Control:
			toolStrip = ToolStripManager.GetNextToolStrip(this, forward: true);
			if (toolStrip != null)
			{
				foreach (ToolStripItem item2 in Items)
				{
					item2.Dismiss(ToolStripDropDownCloseReason.Keyboard);
				}
				ToolStripManager.SetActiveToolStrip(toolStrip, keyboard: true);
				toolStrip.SelectNextToolStripItem(null, forward: true);
			}
			return true;
		case Keys.Tab | Keys.Shift | Keys.Control:
			toolStrip = ToolStripManager.GetNextToolStrip(this, forward: false);
			if (toolStrip != null)
			{
				foreach (ToolStripItem item3 in Items)
				{
					item3.Dismiss(ToolStripDropDownCloseReason.Keyboard);
				}
				ToolStripManager.SetActiveToolStrip(toolStrip, keyboard: true);
				toolStrip.SelectNextToolStripItem(null, forward: true);
			}
			return true;
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
			if (GetCurrentlySelectedItem() is ToolStripControlHost)
			{
				return false;
			}
			break;
		}
		return base.ProcessDialogKey(keyData);
	}

	protected override bool ProcessMnemonic(char charCode)
	{
		foreach (ToolStripItem item in Items)
		{
			if (item.Enabled && item.Visible && !string.IsNullOrEmpty(item.Text) && Control.IsMnemonic(charCode, item.Text))
			{
				return item.ProcessMnemonic(charCode);
			}
		}
		string value = char.ToUpper(charCode).ToString();
		if ((Control.ModifierKeys & Keys.Alt) != 0 || this is ToolStripDropDownMenu)
		{
			foreach (ToolStripItem item2 in Items)
			{
				if (item2.Enabled && item2.Visible && !string.IsNullOrEmpty(item2.Text) && item2.Text.ToUpper().StartsWith(value) && !(item2 is ToolStripControlHost))
				{
					return item2.ProcessMnemonic(charCode);
				}
			}
		}
		return base.ProcessMnemonic(charCode);
	}

	[System.MonoTODO("Stub, does nothing")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void RestoreFocus()
	{
	}

	protected override void Select(bool directed, bool forward)
	{
		foreach (ToolStripItem displayedItem in DisplayedItems)
		{
			if (displayedItem.CanSelect)
			{
				displayedItem.Select();
				break;
			}
		}
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, height, specified);
	}

	protected virtual void SetDisplayedItems()
	{
		displayed_items.Clear();
		foreach (ToolStripItem item in items)
		{
			if (item.Placement == ToolStripItemPlacement.Main && item.Available)
			{
				displayed_items.AddNoOwnerOrLayout(item);
				item.Parent = this;
			}
			else if (item.Placement == ToolStripItemPlacement.Overflow)
			{
				item.Parent = OverflowButton.DropDown;
			}
		}
		if (OverflowButton != null)
		{
			OverflowButton.DropDown.SetDisplayedItems();
		}
	}

	protected internal void SetItemLocation(ToolStripItem item, Point location)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (item.Owner != this)
		{
			throw new NotSupportedException("The item is not owned by this ToolStrip");
		}
		item.SetBounds(new Rectangle(location, item.Size));
	}

	protected internal static void SetItemParent(ToolStripItem item, ToolStrip parent)
	{
		if (item.Owner != null)
		{
			item.Owner.Items.RemoveNoOwnerOrLayout(item);
			if (item.Owner is ToolStripOverflow)
			{
				(item.Owner as ToolStripOverflow).ParentToolStrip.Items.RemoveNoOwnerOrLayout(item);
			}
		}
		parent.Items.AddNoOwnerOrLayout(item);
		item.Parent = parent;
	}

	protected override void SetVisibleCore(bool visible)
	{
		base.SetVisibleCore(visible);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	internal virtual Rectangle CalculateConnectedArea()
	{
		return Rectangle.Empty;
	}

	internal void ChangeSelection(ToolStripItem nextItem)
	{
		if (Application.KeyboardCapture != this)
		{
			ToolStripManager.SetActiveToolStrip(this, ToolStripManager.ActivatedByKeyboard);
		}
		foreach (ToolStripItem item in Items)
		{
			if (item != nextItem)
			{
				item.Dismiss(ToolStripDropDownCloseReason.Keyboard);
			}
		}
		ToolStripItem currentlySelectedItem = GetCurrentlySelectedItem();
		if (currentlySelectedItem != null && !(currentlySelectedItem is ToolStripControlHost))
		{
			FocusInternal(skip_check: true);
		}
		if (nextItem is ToolStripControlHost)
		{
			(nextItem as ToolStripControlHost).Focus();
		}
		nextItem.Select();
		if (nextItem.Parent is MenuStrip && (nextItem.Parent as MenuStrip).MenuDroppedDown)
		{
			(nextItem as ToolStripMenuItem).HandleAutoExpansion();
		}
	}

	internal virtual void Dismiss()
	{
		Dismiss(ToolStripDropDownCloseReason.AppClicked);
	}

	internal virtual void Dismiss(ToolStripDropDownCloseReason reason)
	{
		KeyboardActive = false;
		menu_selected = false;
		foreach (ToolStripItem item in Items)
		{
			item.Dismiss(reason);
		}
		Invalidate();
	}

	internal ToolStripItem GetCurrentlySelectedItem()
	{
		foreach (ToolStripItem displayedItem in DisplayedItems)
		{
			if (displayedItem.Selected)
			{
				return displayedItem;
			}
		}
		return null;
	}

	internal ToolStripItem GetCurrentlyFocusedItem()
	{
		foreach (ToolStripItem displayedItem in DisplayedItems)
		{
			if (displayedItem is ToolStripControlHost && (displayedItem as ToolStripControlHost).Control.Focused)
			{
				return displayedItem;
			}
		}
		return null;
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		return GetToolStripPreferredSize(proposedSize);
	}

	internal virtual Size GetToolStripPreferredSize(Size proposedSize)
	{
		Size empty = Size.Empty;
		if (LayoutStyle == ToolStripLayoutStyle.Flow)
		{
			Point empty2 = Point.Empty;
			int num = 0;
			foreach (ToolStripItem item in items)
			{
				if (DisplayRectangle.Width - empty2.X < item.Width + item.Margin.Horizontal)
				{
					empty2.Y += num;
					num = 0;
					empty2.X = DisplayRectangle.Left;
				}
				empty2.Offset(item.Margin.Left, 0);
				num = Math.Max(num, item.Height + item.Margin.Vertical);
				empty2.X += item.Width + item.Margin.Right;
			}
			empty2.Y += num;
			return new Size(empty2.X, empty2.Y);
		}
		if (orientation == Orientation.Vertical)
		{
			foreach (ToolStripItem item2 in items)
			{
				if (item2.Available)
				{
					Size preferredSize = item2.GetPreferredSize(Size.Empty);
					empty.Height += preferredSize.Height + item2.Margin.Top + item2.Margin.Bottom;
					if (empty.Width < base.Padding.Horizontal + preferredSize.Width + item2.Margin.Horizontal)
					{
						empty.Width = base.Padding.Horizontal + preferredSize.Width + item2.Margin.Horizontal;
					}
				}
			}
			empty.Height += GripRectangle.Height + GripMargin.Vertical + base.Padding.Vertical + 4;
			if (empty.Width == 0)
			{
				empty.Width = base.ExplicitBounds.Width;
			}
			return empty;
		}
		foreach (ToolStripItem item3 in items)
		{
			if (item3.Available)
			{
				Size preferredSize2 = item3.GetPreferredSize(Size.Empty);
				empty.Width += preferredSize2.Width + item3.Margin.Left + item3.Margin.Right;
				if (empty.Height < base.Padding.Vertical + preferredSize2.Height + item3.Margin.Vertical)
				{
					empty.Height = base.Padding.Vertical + preferredSize2.Height + item3.Margin.Vertical;
				}
			}
		}
		empty.Width += GripRectangle.Width + GripMargin.Horizontal + base.Padding.Horizontal + 4;
		if (empty.Height == 0)
		{
			empty.Height = base.ExplicitBounds.Height;
		}
		if (this is StatusStrip)
		{
			empty.Height = Math.Max(empty.Height, 22);
		}
		return empty;
	}

	internal virtual ToolStrip GetTopLevelToolStrip()
	{
		return this;
	}

	internal virtual void HandleItemClick(ToolStripItem dismissingItem)
	{
		GetTopLevelToolStrip().Dismiss(ToolStripDropDownCloseReason.ItemClicked);
	}

	internal void HideMenus(bool release, ToolStripDropDownCloseReason reason)
	{
		if (this is MenuStrip && release && menu_selected)
		{
			(this as MenuStrip).FireMenuDeactivate();
		}
		if (release)
		{
			menu_selected = false;
		}
		NotifySelectedChanged(null);
	}

	internal void NotifySelectedChanged(ToolStripItem tsi)
	{
		foreach (ToolStripItem displayedItem in DisplayedItems)
		{
			if (tsi != displayedItem && displayedItem is ToolStripDropDownItem)
			{
				(displayedItem as ToolStripDropDownItem).HideDropDown(ToolStripDropDownCloseReason.Keyboard);
			}
		}
		if (OverflowButton != null)
		{
			ToolStripItemCollection displayedItems = OverflowButton.DropDown.DisplayedItems;
			foreach (ToolStripItem item in displayedItems)
			{
				if (tsi != item && item is ToolStripDropDownItem)
				{
					(item as ToolStripDropDownItem).HideDropDown(ToolStripDropDownCloseReason.Keyboard);
				}
			}
			OverflowButton.HideDropDown();
		}
		foreach (ToolStripItem item2 in Items)
		{
			if (tsi != item2)
			{
				item2.Dismiss(ToolStripDropDownCloseReason.Keyboard);
			}
		}
	}

	internal virtual bool OnMenuKey()
	{
		return false;
	}

	internal virtual bool ProcessArrowKey(Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Right:
		{
			ToolStripItem currentlySelectedItem = GetCurrentlySelectedItem();
			if (currentlySelectedItem is ToolStripControlHost)
			{
				return false;
			}
			currentlySelectedItem = SelectNextToolStripItem(currentlySelectedItem, forward: true);
			if (currentlySelectedItem is ToolStripControlHost)
			{
				(currentlySelectedItem as ToolStripControlHost).Focus();
			}
			return true;
		}
		case Keys.Tab:
		{
			ToolStripItem currentlySelectedItem = GetCurrentlySelectedItem();
			currentlySelectedItem = SelectNextToolStripItem(currentlySelectedItem, forward: true);
			if (currentlySelectedItem is ToolStripControlHost)
			{
				(currentlySelectedItem as ToolStripControlHost).Focus();
			}
			return true;
		}
		case Keys.Left:
		{
			ToolStripItem currentlySelectedItem = GetCurrentlySelectedItem();
			if (currentlySelectedItem is ToolStripControlHost)
			{
				return false;
			}
			currentlySelectedItem = SelectNextToolStripItem(currentlySelectedItem, forward: false);
			if (currentlySelectedItem is ToolStripControlHost)
			{
				(currentlySelectedItem as ToolStripControlHost).Focus();
			}
			return true;
		}
		case Keys.Tab | Keys.Shift:
		{
			ToolStripItem currentlySelectedItem = GetCurrentlySelectedItem();
			currentlySelectedItem = SelectNextToolStripItem(currentlySelectedItem, forward: false);
			if (currentlySelectedItem is ToolStripControlHost)
			{
				(currentlySelectedItem as ToolStripControlHost).Focus();
			}
			return true;
		}
		default:
			return false;
		}
	}

	internal virtual ToolStripItem SelectNextToolStripItem(ToolStripItem start, bool forward)
	{
		ToolStripItem nextItem = GetNextItem(start, forward ? ArrowDirection.Right : ArrowDirection.Left);
		if (nextItem == null)
		{
			return nextItem;
		}
		ChangeSelection(nextItem);
		if (nextItem is ToolStripControlHost)
		{
			(nextItem as ToolStripControlHost).Focus();
		}
		return nextItem;
	}

	private void MouseEnteredItem(ToolStripItem item)
	{
		if (show_item_tool_tips && !(item is ToolStripTextBox))
		{
			tooltip_currently_showing = item;
			ToolTipTimer.Start();
		}
	}

	private void MouseLeftItem(ToolStripItem item)
	{
		ToolTipTimer.Stop();
		ToolTipWindow.Hide(this);
		tooltip_currently_showing = null;
	}

	private void ToolTipTimer_Tick(object o, EventArgs args)
	{
		string toolTip = tooltip_currently_showing.GetToolTip();
		if (!string.IsNullOrEmpty(toolTip))
		{
			ToolTipWindow.Present(this, toolTip);
		}
		tooltip_currently_showing.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseHover);
		ToolTipTimer.Stop();
	}

	internal void BeginMerge()
	{
		if (IsCurrentlyMerged)
		{
			return;
		}
		IsCurrentlyMerged = true;
		if (pre_merge_items != null)
		{
			return;
		}
		pre_merge_items = new List<ToolStripItem>();
		foreach (ToolStripItem item in Items)
		{
			pre_merge_items.Add(item);
		}
	}

	internal void RevertMergeItem(ToolStripItem item)
	{
		int num = 0;
		if (item.Parent != null && item.Parent != this)
		{
			if (item.Parent is ToolStripOverflow)
			{
				(item.Parent as ToolStripOverflow).ParentToolStrip.Items.RemoveNoOwnerOrLayout(item);
			}
			else
			{
				item.Parent.Items.RemoveNoOwnerOrLayout(item);
			}
			item.Parent = item.Owner;
		}
		num = item.Owner.pre_merge_items.IndexOf(item);
		for (int i = num; i < pre_merge_items.Count; i++)
		{
			if (Items.Contains(pre_merge_items[i]))
			{
				item.Owner.Items.InsertNoOwnerOrLayout(Items.IndexOf(pre_merge_items[i]), item);
				return;
			}
		}
		item.Owner.Items.AddNoOwnerOrLayout(item);
	}
}
