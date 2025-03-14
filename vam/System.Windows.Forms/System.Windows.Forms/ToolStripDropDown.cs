using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.ToolStripDropDownDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class ToolStripDropDown : ToolStrip
{
	[ComVisible(true)]
	public class ToolStripDropDownAccessibleObject : ToolStripAccessibleObject
	{
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public override AccessibleRole Role => AccessibleRole.MenuPopup;

		public ToolStripDropDownAccessibleObject(ToolStripDropDown owner)
			: base(owner)
		{
		}
	}

	private bool allow_transparency;

	private bool auto_close;

	private bool can_overflow;

	private bool drop_shadow_enabled = true;

	private double opacity = 1.0;

	private ToolStripItem owner_item;

	private static object ClosedEvent;

	private static object ClosingEvent;

	private static object OpenedEvent;

	private static object OpeningEvent;

	private static object ScrollEvent;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool AllowItemReorder
	{
		get
		{
			return base.AllowItemReorder;
		}
		set
		{
			base.AllowItemReorder = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool AllowTransparency
	{
		get
		{
			return allow_transparency;
		}
		set
		{
			if (value == allow_transparency || (XplatUI.SupportsTransparency() & TransparencySupport.Set) == 0)
			{
				return;
			}
			allow_transparency = value;
			if (base.IsHandleCreated)
			{
				if (value)
				{
					XplatUI.SetWindowTransparency(Handle, Opacity, Color.Empty);
				}
				else
				{
					UpdateStyles();
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[DefaultValue(true)]
	public bool AutoClose
	{
		get
		{
			return auto_close;
		}
		set
		{
			auto_close = value;
		}
	}

	[DefaultValue(true)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DefaultValue(false)]
	public new bool CanOverflow
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new ContextMenu ContextMenu
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public override ToolStripDropDownDirection DefaultDropDownDirection
	{
		get
		{
			return base.DefaultDropDownDirection;
		}
		set
		{
			base.DefaultDropDownDirection = value;
		}
	}

	[Browsable(false)]
	[DefaultValue(DockStyle.None)]
	[EditorBrowsable(EditorBrowsableState.Always)]
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

	public bool DropShadowEnabled
	{
		get
		{
			return drop_shadow_enabled;
		}
		set
		{
			if (drop_shadow_enabled != value)
			{
				drop_shadow_enabled = value;
				UpdateStyles();
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
			base.Font = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new ToolStripGripDisplayStyle GripDisplayStyle => ToolStripGripDisplayStyle.Vertical;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Padding GripMargin
	{
		get
		{
			return Padding.Empty;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Rectangle GripRectangle => Rectangle.Empty;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DefaultValue(ToolStripGripStyle.Hidden)]
	public new ToolStripGripStyle GripStyle
	{
		get
		{
			return base.GripStyle;
		}
		set
		{
			base.GripStyle = value;
		}
	}

	[Browsable(false)]
	public bool IsAutoGenerated => this is ToolStripOverflow;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

	[TypeConverter(typeof(OpacityConverter))]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[DefaultValue(1.0)]
	public double Opacity
	{
		get
		{
			return opacity;
		}
		set
		{
			if (opacity != value)
			{
				opacity = value;
				allow_transparency = true;
				if (base.IsHandleCreated)
				{
					UpdateStyles();
					XplatUI.SetWindowTransparency(Handle, opacity, Color.Empty);
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new ToolStripOverflowButton OverflowButton => base.OverflowButton;

	[DefaultValue(null)]
	[Browsable(false)]
	public ToolStripItem OwnerItem
	{
		get
		{
			return owner_item;
		}
		set
		{
			owner_item = value;
			if (owner_item != null)
			{
				if (owner_item.Owner != null && owner_item.Owner.RenderMode != ToolStripRenderMode.ManagerRenderMode)
				{
					base.Renderer = owner_item.Owner.Renderer;
				}
				Font = owner_item.Font;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
	public new Region Region
	{
		get
		{
			return base.Region;
		}
		set
		{
			base.Region = value;
		}
	}

	[AmbientValue(RightToLeft.Inherit)]
	[Localizable(true)]
	public override RightToLeft RightToLeft
	{
		get
		{
			return base.RightToLeft;
		}
		set
		{
			base.RightToLeft = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool Stretch
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new int TabIndex
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[Browsable(false)]
	[DefaultValue(ToolStripTextDirection.Horizontal)]
	public override ToolStripTextDirection TextDirection
	{
		get
		{
			return base.TextDirection;
		}
		set
		{
			base.TextDirection = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool TopLevel
	{
		get
		{
			return GetTopLevel();
		}
		set
		{
			SetTopLevel(value);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[DefaultValue(false)]
	[Localizable(true)]
	public new bool Visible
	{
		get
		{
			return base.Visible;
		}
		set
		{
			base.Visible = value;
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = base.CreateParams;
			createParams.Style = -2113929216;
			createParams.ClassStyle |= 131072;
			createParams.ExStyle |= 136;
			if (Opacity < 1.0 && allow_transparency)
			{
				createParams.ExStyle |= 524288;
			}
			if (TopMost)
			{
				createParams.ExStyle |= 8;
			}
			return createParams;
		}
	}

	protected override DockStyle DefaultDock => DockStyle.None;

	protected override Padding DefaultPadding => new Padding(1, 2, 1, 2);

	protected override bool DefaultShowItemToolTips => true;

	protected internal override Size MaxItemSize => new Size(Screen.PrimaryScreen.Bounds.Width - 2, Screen.PrimaryScreen.Bounds.Height - 34);

	protected virtual bool TopMost => true;

	internal override bool ActivateOnShow => false;

	internal ToolStripItem TopLevelOwnerItem
	{
		get
		{
			ToolStripItem ownerItem = OwnerItem;
			ToolStrip toolStrip = null;
			while (ownerItem != null)
			{
				toolStrip = ownerItem.Owner;
				if (toolStrip != null && toolStrip is ToolStripDropDown)
				{
					ownerItem = (toolStrip as ToolStripDropDown).OwnerItem;
					continue;
				}
				return ownerItem;
			}
			return null;
		}
	}

	[Browsable(false)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[Browsable(false)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	[Browsable(false)]
	public new event EventHandler BindingContextChanged
	{
		add
		{
			base.BindingContextChanged += value;
		}
		remove
		{
			base.BindingContextChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new event UICuesEventHandler ChangeUICues
	{
		add
		{
			base.ChangeUICues += value;
		}
		remove
		{
			base.ChangeUICues -= value;
		}
	}

	public event ToolStripDropDownClosedEventHandler Closed
	{
		add
		{
			base.Events.AddHandler(ClosedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ClosedEvent, value);
		}
	}

	public event ToolStripDropDownClosingEventHandler Closing
	{
		add
		{
			base.Events.AddHandler(ClosingEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ClosingEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ContextMenuChanged
	{
		add
		{
			base.ContextMenuChanged += value;
		}
		remove
		{
			base.ContextMenuChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
	public new event EventHandler ContextMenuStripChanged
	{
		add
		{
			base.ContextMenuStripChanged += value;
		}
		remove
		{
			base.ContextMenuStripChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
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
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new event EventHandler Enter
	{
		add
		{
			base.Enter += value;
		}
		remove
		{
			base.Enter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
	public new event EventHandler FontChanged
	{
		add
		{
			base.FontChanged += value;
		}
		remove
		{
			base.FontChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event GiveFeedbackEventHandler GiveFeedback
	{
		add
		{
			base.GiveFeedback += value;
		}
		remove
		{
			base.GiveFeedback -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
	public new event HelpEventHandler HelpRequested
	{
		add
		{
			base.HelpRequested += value;
		}
		remove
		{
			base.HelpRequested -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Always)]
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

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
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

	[EditorBrowsable(EditorBrowsableState.Always)]
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

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(false)]
	public new event EventHandler Leave
	{
		add
		{
			base.Leave += value;
		}
		remove
		{
			base.Leave -= value;
		}
	}

	public event EventHandler Opened
	{
		add
		{
			base.Events.AddHandler(OpenedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(OpenedEvent, value);
		}
	}

	public event CancelEventHandler Opening
	{
		add
		{
			base.Events.AddHandler(OpeningEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(OpeningEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new event EventHandler RegionChanged
	{
		add
		{
			base.RegionChanged += value;
		}
		remove
		{
			base.RegionChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event ScrollEventHandler Scroll
	{
		add
		{
			base.Events.AddHandler(ScrollEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ScrollEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new event EventHandler StyleChanged
	{
		add
		{
			base.StyleChanged += value;
		}
		remove
		{
			base.StyleChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler Validated
	{
		add
		{
			base.Validated += value;
		}
		remove
		{
			base.Validated -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event CancelEventHandler Validating
	{
		add
		{
			base.Validating += value;
		}
		remove
		{
			base.Validating -= value;
		}
	}

	public ToolStripDropDown()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		auto_close = true;
		is_visible = false;
		DefaultDropDownDirection = ToolStripDropDownDirection.Right;
		GripStyle = ToolStripGripStyle.Hidden;
		is_toplevel = true;
	}

	static ToolStripDropDown()
	{
		Closed = new object();
		Closing = new object();
		Opened = new object();
		Opening = new object();
		Scroll = new object();
	}

	public void Close()
	{
		Close(ToolStripDropDownCloseReason.CloseCalled);
	}

	public void Close(ToolStripDropDownCloseReason reason)
	{
		if (!Visible)
		{
			return;
		}
		ToolStripDropDownClosingEventArgs toolStripDropDownClosingEventArgs = new ToolStripDropDownClosingEventArgs(reason);
		OnClosing(toolStripDropDownClosingEventArgs);
		if (toolStripDropDownClosingEventArgs.Cancel || (!auto_close && reason != ToolStripDropDownCloseReason.CloseCalled))
		{
			return;
		}
		ToolStripManager.AppClicked -= ToolStripMenuTracker_AppClicked;
		ToolStripManager.AppFocusChange -= ToolStripMenuTracker_AppFocusChange;
		Hide();
		if (owner_item != null)
		{
			owner_item.Invalidate();
		}
		foreach (ToolStripItem item in Items)
		{
			item.Dismiss(reason);
		}
		OnClosed(new ToolStripDropDownClosedEventArgs(reason));
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void Show()
	{
		Show(Location, DefaultDropDownDirection);
	}

	public void Show(Point screenLocation)
	{
		Show(screenLocation, DefaultDropDownDirection);
	}

	public void Show(Control control, Point position)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		XplatUI.SetOwner(Handle, control.Handle);
		Show(control.PointToScreen(position), DefaultDropDownDirection);
	}

	public void Show(int x, int y)
	{
		Show(new Point(x, y), DefaultDropDownDirection);
	}

	public void Show(Point position, ToolStripDropDownDirection direction)
	{
		PerformLayout();
		Point point = position;
		Point point2 = new Point(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
		if (this is ContextMenuStrip)
		{
			switch (direction)
			{
			case ToolStripDropDownDirection.AboveLeft:
				if (point.X - base.Width < 0)
				{
					direction = ToolStripDropDownDirection.AboveRight;
				}
				break;
			case ToolStripDropDownDirection.BelowLeft:
				if (point.X - base.Width < 0)
				{
					direction = ToolStripDropDownDirection.BelowRight;
				}
				break;
			case ToolStripDropDownDirection.Left:
				if (point.X - base.Width < 0)
				{
					direction = ToolStripDropDownDirection.Right;
				}
				break;
			case ToolStripDropDownDirection.AboveRight:
				if (point.X + base.Width > point2.X)
				{
					direction = ToolStripDropDownDirection.AboveLeft;
				}
				break;
			case ToolStripDropDownDirection.BelowRight:
			case ToolStripDropDownDirection.Default:
				if (point.X + base.Width > point2.X)
				{
					direction = ToolStripDropDownDirection.BelowLeft;
				}
				break;
			case ToolStripDropDownDirection.Right:
				if (point.X + base.Width > point2.X)
				{
					direction = ToolStripDropDownDirection.Left;
				}
				break;
			}
			switch (direction)
			{
			case ToolStripDropDownDirection.AboveLeft:
				if (point.Y - base.Height < 0)
				{
					direction = ToolStripDropDownDirection.BelowLeft;
				}
				break;
			case ToolStripDropDownDirection.AboveRight:
				if (point.Y - base.Height < 0)
				{
					direction = ToolStripDropDownDirection.BelowRight;
				}
				break;
			case ToolStripDropDownDirection.BelowLeft:
				if (point.Y + base.Height > point2.Y)
				{
					direction = ToolStripDropDownDirection.AboveLeft;
				}
				break;
			case ToolStripDropDownDirection.BelowRight:
			case ToolStripDropDownDirection.Default:
				if (point.Y + base.Height > point2.Y)
				{
					direction = ToolStripDropDownDirection.AboveRight;
				}
				break;
			case ToolStripDropDownDirection.Left:
				if (point.Y + base.Height > point2.Y)
				{
					direction = ToolStripDropDownDirection.AboveLeft;
				}
				break;
			case ToolStripDropDownDirection.Right:
				if (point.Y + base.Height > point2.Y)
				{
					direction = ToolStripDropDownDirection.AboveRight;
				}
				break;
			}
		}
		switch (direction)
		{
		case ToolStripDropDownDirection.AboveLeft:
			point.Y -= base.Height;
			point.X -= base.Width;
			break;
		case ToolStripDropDownDirection.AboveRight:
			point.Y -= base.Height;
			break;
		case ToolStripDropDownDirection.BelowLeft:
			point.X -= base.Width;
			break;
		case ToolStripDropDownDirection.Left:
			point.X -= base.Width;
			break;
		}
		if (point.X + base.Width > point2.X)
		{
			point.X = point2.X - base.Width;
		}
		if (point.X < 0)
		{
			point.X = 0;
		}
		if (Location != point)
		{
			Location = point;
		}
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		OnOpening(cancelEventArgs);
		if (!cancelEventArgs.Cancel)
		{
			ToolStripManager.AppClicked += ToolStripMenuTracker_AppClicked;
			ToolStripManager.AppFocusChange += ToolStripMenuTracker_AppFocusChange;
			base.Show();
			ToolStripManager.SetActiveToolStrip(this, ToolStripManager.ActivatedByKeyboard);
			OnOpened(EventArgs.Empty);
		}
	}

	public void Show(Control control, int x, int y)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		Show(control, new Point(x, y));
	}

	public void Show(Control control, Point position, ToolStripDropDownDirection direction)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		XplatUI.SetOwner(Handle, control.Handle);
		Show(control.PointToScreen(position), direction);
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripDropDownAccessibleObject(this);
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
	}

	protected override LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle style)
	{
		return base.CreateLayoutSettings(style);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected virtual void OnClosed(ToolStripDropDownClosedEventArgs e)
	{
		((ToolStripDropDownClosedEventHandler)base.Events[Closed])?.Invoke(this, e);
	}

	protected virtual void OnClosing(ToolStripDropDownClosingEventArgs e)
	{
		((ToolStripDropDownClosingEventHandler)base.Events[Closing])?.Invoke(this, e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (Application.MWFThread.Current.Context != null && Application.MWFThread.Current.Context.MainForm != null)
		{
			XplatUI.SetOwner(Handle, Application.MWFThread.Current.Context.MainForm.Handle);
		}
	}

	protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
	{
		base.OnItemClicked(e);
	}

	protected override void OnLayout(LayoutEventArgs e)
	{
		int num = 0;
		foreach (ToolStripItem item in Items)
		{
			if (item.Available)
			{
				item.SetPlacement(ToolStripItemPlacement.Main);
				num = Math.Max(num, item.GetPreferredSize(Size.Empty).Width + item.Margin.Horizontal);
			}
		}
		num += base.Padding.Horizontal;
		int left = base.Padding.Left;
		int num2 = base.Padding.Top;
		foreach (ToolStripItem item2 in Items)
		{
			if (item2.Available)
			{
				num2 += item2.Margin.Top;
				int num3 = 0;
				Size preferredSize = item2.GetPreferredSize(Size.Empty);
				num3 = ((preferredSize.Height > 22) ? preferredSize.Height : ((!(item2 is ToolStripSeparator)) ? 22 : 7));
				item2.SetBounds(new Rectangle(left, num2, preferredSize.Width, num3));
				num2 += num3 + item2.Margin.Bottom;
			}
		}
		base.Size = new Size(num, num2 + base.Padding.Bottom);
		SetDisplayedItems();
		OnLayoutCompleted(EventArgs.Empty);
		Invalidate();
	}

	protected override void OnMouseUp(MouseEventArgs mea)
	{
		base.OnMouseUp(mea);
	}

	protected virtual void OnOpened(EventArgs e)
	{
		((EventHandler)base.Events[Opened])?.Invoke(this, e);
	}

	protected virtual void OnOpening(CancelEventArgs e)
	{
		((CancelEventHandler)base.Events[Opening])?.Invoke(this, e);
	}

	protected override void OnParentChanged(EventArgs e)
	{
		base.OnParentChanged(e);
		if (base.Parent is ToolStrip)
		{
			base.Renderer = (base.Parent as ToolStrip).Renderer;
		}
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
		if (owner_item != null && owner_item is ToolStripDropDownItem)
		{
			ToolStripDropDownItem toolStripDropDownItem = (ToolStripDropDownItem)owner_item;
			if (Visible)
			{
				toolStripDropDownItem.OnDropDownOpened(EventArgs.Empty);
			}
			else
			{
				toolStripDropDownItem.OnDropDownClosed(EventArgs.Empty);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override bool ProcessDialogChar(char charCode)
	{
		return base.ProcessDialogChar(charCode);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		if (keyData == (Keys.Tab | Keys.Control) || keyData == (Keys.Tab | Keys.Shift | Keys.Control))
		{
			return true;
		}
		return base.ProcessDialogKey(keyData);
	}

	protected override bool ProcessMnemonic(char charCode)
	{
		return base.ProcessMnemonic(charCode);
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		base.ScaleControl(factor, specified);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void ScaleCore(float dx, float dy)
	{
		base.ScaleCore(dx, dy);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, height, specified);
	}

	protected override void SetVisibleCore(bool visible)
	{
		base.SetVisibleCore(visible);
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 33)
		{
			m.Result = (IntPtr)3;
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	internal override void Dismiss(ToolStripDropDownCloseReason reason)
	{
		Close(reason);
		base.Dismiss(reason);
	}

	internal override ToolStrip GetTopLevelToolStrip()
	{
		if (OwnerItem == null)
		{
			return this;
		}
		return OwnerItem.GetTopLevelToolStrip();
	}

	internal override bool ProcessArrowKey(Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Tab:
		case Keys.Down:
			SelectNextToolStripItem(GetCurrentlySelectedItem(), forward: true);
			return true;
		case Keys.Up:
		case Keys.Tab | Keys.Shift:
			SelectNextToolStripItem(GetCurrentlySelectedItem(), forward: false);
			return true;
		case Keys.Right:
			GetTopLevelToolStrip().SelectNextToolStripItem(TopLevelOwnerItem, forward: true);
			return true;
		case Keys.Escape:
		case Keys.Left:
		{
			Dismiss(ToolStripDropDownCloseReason.Keyboard);
			if (OwnerItem == null)
			{
				return true;
			}
			ToolStrip toolStrip = OwnerItem.Parent;
			ToolStripManager.SetActiveToolStrip(toolStrip, keyboard: true);
			if (toolStrip is MenuStrip && keyData == Keys.Left)
			{
				toolStrip.SelectNextToolStripItem(TopLevelOwnerItem, forward: false);
				TopLevelOwnerItem.Invalidate();
			}
			else if (toolStrip is MenuStrip && keyData == Keys.Escape)
			{
				(toolStrip as MenuStrip).MenuDroppedDown = false;
				TopLevelOwnerItem.Select();
			}
			return true;
		}
		default:
			return false;
		}
	}

	internal override ToolStripItem SelectNextToolStripItem(ToolStripItem start, bool forward)
	{
		ToolStripItem nextItem = GetNextItem(start, (!forward) ? ArrowDirection.Up : ArrowDirection.Down);
		if (nextItem != null)
		{
			ChangeSelection(nextItem);
		}
		return nextItem;
	}

	private void ToolStripMenuTracker_AppFocusChange(object sender, EventArgs e)
	{
		GetTopLevelToolStrip().Dismiss(ToolStripDropDownCloseReason.AppFocusChange);
	}

	private void ToolStripMenuTracker_AppClicked(object sender, EventArgs e)
	{
		GetTopLevelToolStrip().Dismiss(ToolStripDropDownCloseReason.AppClicked);
	}
}
