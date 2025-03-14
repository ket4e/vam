using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultEvent("SplitterMoved")]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.SplitContainerDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[Docking(DockingBehavior.AutoDock)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class SplitContainer : ContainerControl
{
	internal class SplitContainerTypedControlCollection : ControlCollection
	{
		public SplitContainerTypedControlCollection(Control owner)
			: base(owner)
		{
		}
	}

	private FixedPanel fixed_panel;

	private Orientation orientation;

	private int splitter_increment;

	private Rectangle splitter_rectangle;

	private Rectangle splitter_rectangle_moving;

	private Rectangle splitter_rectangle_before_move;

	private bool splitter_fixed;

	private bool splitter_dragging;

	private int splitter_prev_move;

	private Cursor restore_cursor;

	private double fixed_none_ratio;

	private SplitterPanel panel1;

	private bool panel1_collapsed;

	private int panel1_min_size;

	private SplitterPanel panel2;

	private bool panel2_collapsed;

	private int panel2_min_size;

	private static object SplitterMovedEvent;

	private static object SplitterMovingEvent;

	private static object UIACanResizeChangedEvent;

	[Browsable(false)]
	[Localizable(true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DefaultValue(false)]
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

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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
	[DefaultValue("{X=0,Y=0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Point AutoScrollOffset
	{
		get
		{
			return base.AutoScrollOffset;
		}
		set
		{
			base.AutoScrollOffset = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			base.BackgroundImage = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	[Browsable(false)]
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

	[DispId(-504)]
	[DefaultValue(BorderStyle.None)]
	public BorderStyle BorderStyle
	{
		get
		{
			return panel1.BorderStyle;
		}
		set
		{
			if (!Enum.IsDefined(typeof(BorderStyle), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for BorderStyle");
			}
			panel1.BorderStyle = value;
			panel2.BorderStyle = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new ControlCollection Controls => base.Controls;

	public new DockStyle Dock
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

	[DefaultValue(FixedPanel.None)]
	public FixedPanel FixedPanel
	{
		get
		{
			return fixed_panel;
		}
		set
		{
			if (!Enum.IsDefined(typeof(FixedPanel), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for FixedPanel");
			}
			fixed_panel = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(false)]
	public bool IsSplitterFixed
	{
		get
		{
			return splitter_fixed;
		}
		set
		{
			splitter_fixed = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(Orientation.Vertical)]
	public Orientation Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			if (!Enum.IsDefined(typeof(Orientation), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for Orientation");
			}
			if (orientation != value)
			{
				if (value == Orientation.Vertical)
				{
					splitter_rectangle.Width = splitter_rectangle.Height;
					splitter_rectangle.X = splitter_rectangle.Y;
				}
				else
				{
					splitter_rectangle.Height = splitter_rectangle.Width;
					splitter_rectangle.Y = splitter_rectangle.X;
				}
				orientation = value;
				UpdateSplitter();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
		}
	}

	[Localizable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public SplitterPanel Panel1 => panel1;

	[DefaultValue(false)]
	public bool Panel1Collapsed
	{
		get
		{
			return panel1_collapsed;
		}
		set
		{
			if (panel1_collapsed != value)
			{
				panel1_collapsed = value;
				panel1.Visible = !value;
				OnUIACanResizeChanged(EventArgs.Empty);
				PerformLayout();
			}
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[Localizable(true)]
	[DefaultValue(25)]
	public int Panel1MinSize
	{
		get
		{
			return panel1_min_size;
		}
		set
		{
			panel1_min_size = value;
		}
	}

	[Localizable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public SplitterPanel Panel2 => panel2;

	[DefaultValue(false)]
	public bool Panel2Collapsed
	{
		get
		{
			return panel2_collapsed;
		}
		set
		{
			if (panel2_collapsed != value)
			{
				panel2_collapsed = value;
				panel2.Visible = !value;
				OnUIACanResizeChanged(EventArgs.Empty);
				PerformLayout();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(25)]
	[RefreshProperties(RefreshProperties.All)]
	public int Panel2MinSize
	{
		get
		{
			return panel2_min_size;
		}
		set
		{
			panel2_min_size = value;
		}
	}

	[Localizable(true)]
	[SettingsBindable(true)]
	[DefaultValue(50)]
	public int SplitterDistance
	{
		get
		{
			if (orientation == Orientation.Vertical)
			{
				return splitter_rectangle.X;
			}
			return splitter_rectangle.Y;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (value < panel1_min_size)
			{
				value = panel1_min_size;
			}
			bool flag = true;
			if (orientation == Orientation.Vertical)
			{
				if (base.Width - (SplitterWidth + value) < panel2_min_size)
				{
					value = base.Width - (SplitterWidth + panel2_min_size);
				}
				if (splitter_rectangle.X != value)
				{
					splitter_rectangle.X = value;
					flag = true;
				}
			}
			else
			{
				if (base.Height - (SplitterWidth + value) < panel2_min_size)
				{
					value = base.Height - (SplitterWidth + panel2_min_size);
				}
				if (splitter_rectangle.Y != value)
				{
					splitter_rectangle.Y = value;
					flag = true;
				}
			}
			if (flag)
			{
				UpdateSplitter();
				OnSplitterMoved(new SplitterEventArgs(base.Left, base.Top, splitter_rectangle.X, splitter_rectangle.Y));
			}
		}
	}

	[DefaultValue(1)]
	[System.MonoTODO("Stub, never called")]
	[Localizable(true)]
	public int SplitterIncrement
	{
		get
		{
			return splitter_increment;
		}
		set
		{
			splitter_increment = value;
		}
	}

	[Browsable(false)]
	public Rectangle SplitterRectangle => splitter_rectangle;

	[Localizable(true)]
	[DefaultValue(4)]
	public int SplitterWidth
	{
		get
		{
			if (orientation == Orientation.Vertical)
			{
				return splitter_rectangle.Width;
			}
			return splitter_rectangle.Height;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (orientation == Orientation.Vertical)
			{
				splitter_rectangle.Width = value;
			}
			else
			{
				splitter_rectangle.Height = value;
			}
			UpdateSplitter();
		}
	}

	[DispId(-516)]
	[DefaultValue(true)]
	[System.MonoTODO("Stub, never called")]
	public new bool TabStop
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
	[Bindable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	protected override Size DefaultSize => new Size(150, 100);

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

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
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
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler PaddingChanged
	{
		add
		{
			base.PaddingChanged += value;
		}
		remove
		{
			base.PaddingChanged -= value;
		}
	}

	public event SplitterEventHandler SplitterMoved
	{
		add
		{
			base.Events.AddHandler(SplitterMovedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SplitterMovedEvent, value);
		}
	}

	public event SplitterCancelEventHandler SplitterMoving
	{
		add
		{
			base.Events.AddHandler(SplitterMovingEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SplitterMovingEvent, value);
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

	internal event EventHandler UIACanResizeChanged
	{
		add
		{
			base.Events.AddHandler(UIACanResizeChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACanResizeChangedEvent, value);
		}
	}

	public SplitContainer()
	{
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
		fixed_panel = FixedPanel.None;
		orientation = Orientation.Vertical;
		splitter_rectangle = new Rectangle(50, 0, 4, base.Height);
		splitter_increment = 1;
		splitter_prev_move = -1;
		restore_cursor = null;
		splitter_fixed = false;
		panel1_collapsed = false;
		panel2_collapsed = false;
		panel1_min_size = 25;
		panel2_min_size = 25;
		panel1 = new SplitterPanel(this);
		panel2 = new SplitterPanel(this);
		panel1.Size = new Size(50, 50);
		UpdateSplitter();
		Controls.Add(panel2);
		Controls.Add(panel1);
	}

	static SplitContainer()
	{
		SplitterMoved = new object();
		SplitterMoving = new object();
		UIACanResizeChanged = new object();
	}

	internal void OnUIACanResizeChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIACanResizeChanged])?.Invoke(this, e);
	}

	public void OnSplitterMoved(SplitterEventArgs e)
	{
		((SplitterEventHandler)base.Events[SplitterMoved])?.Invoke(this, e);
	}

	public void OnSplitterMoving(SplitterCancelEventArgs e)
	{
		((SplitterCancelEventHandler)base.Events[SplitterMoving])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override ControlCollection CreateControlsInstance()
	{
		return new SplitContainerTypedControlCollection(this);
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
	}

	protected override void OnLayout(LayoutEventArgs e)
	{
		UpdateLayout();
		base.OnLayout(e);
	}

	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
	}

	protected override void OnMouseCaptureChanged(EventArgs e)
	{
		base.OnMouseCaptureChanged(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if (!splitter_fixed && SplitterHitTest(e.Location))
		{
			splitter_dragging = true;
			SplitterBeginMove(e.Location);
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		SplitterRestoreCursor();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		if (splitter_dragging)
		{
			SplitterMove(e.Location);
		}
		if (!splitter_fixed && SplitterHitTest(e.Location))
		{
			SplitterSetCursor(orientation);
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		if (splitter_dragging)
		{
			SplitterEndMove(e.Location, cancel: false);
			SplitterRestoreCursor();
			splitter_dragging = false;
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnRightToLeftChanged(EventArgs e)
	{
		base.OnRightToLeftChanged(e);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		return base.ProcessDialogKey(keyData);
	}

	protected override bool ProcessTabKey(bool forward)
	{
		return base.ProcessTabKey(forward);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		base.ScaleControl(factor, specified);
	}

	protected override void Select(bool directed, bool forward)
	{
		base.Select(directed, forward);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, height, specified);
	}

	protected override void WndProc(ref Message msg)
	{
		base.WndProc(ref msg);
	}

	private bool SplitterHitTest(Point location)
	{
		if (location.X >= splitter_rectangle.X && location.X <= splitter_rectangle.X + splitter_rectangle.Width && location.Y >= splitter_rectangle.Y && location.Y <= splitter_rectangle.Y + splitter_rectangle.Height)
		{
			return true;
		}
		return false;
	}

	private void SplitterBeginMove(Point location)
	{
		splitter_prev_move = ((orientation != Orientation.Vertical) ? location.Y : location.X);
		splitter_rectangle_moving = splitter_rectangle;
		splitter_rectangle_before_move = splitter_rectangle;
	}

	private void SplitterMove(Point location)
	{
		int num = ((orientation != Orientation.Vertical) ? location.Y : location.X);
		int num2 = num - splitter_prev_move;
		Rectangle rect = splitter_rectangle_moving;
		bool flag = false;
		if (orientation == Orientation.Vertical)
		{
			int num3 = panel1_min_size;
			int num4 = panel2.Location.X + (panel2.Width - panel2_min_size) - splitter_rectangle_moving.Width;
			if (splitter_rectangle_moving.X + num2 > num3 && splitter_rectangle_moving.X + num2 < num4)
			{
				splitter_rectangle_moving.X += num2;
				flag = true;
			}
			else if (splitter_rectangle_moving.X + num2 <= num3 && splitter_rectangle_moving.X != num3)
			{
				splitter_rectangle_moving.X = num3;
				flag = true;
			}
			else if (splitter_rectangle_moving.X + num2 >= num4 && splitter_rectangle_moving.X != num4)
			{
				splitter_rectangle_moving.X = num4;
				flag = true;
			}
		}
		else if (orientation == Orientation.Horizontal)
		{
			int num5 = panel1_min_size;
			int num6 = panel2.Location.Y + (panel2.Height - panel2_min_size) - splitter_rectangle_moving.Height;
			if (splitter_rectangle_moving.Y + num2 > num5 && splitter_rectangle_moving.Y + num2 < num6)
			{
				splitter_rectangle_moving.Y += num2;
				flag = true;
			}
			else if (splitter_rectangle_moving.Y + num2 <= num5 && splitter_rectangle_moving.Y != num5)
			{
				splitter_rectangle_moving.Y = num5;
				flag = true;
			}
			else if (splitter_rectangle_moving.Y + num2 >= num6 && splitter_rectangle_moving.Y != num6)
			{
				splitter_rectangle_moving.Y = num6;
				flag = true;
			}
		}
		if (flag)
		{
			splitter_prev_move = num;
			OnSplitterMoving(new SplitterCancelEventArgs(location.X, location.Y, splitter_rectangle.X, splitter_rectangle.Y));
			XplatUI.DrawReversibleRectangle(Handle, rect, 1);
			XplatUI.DrawReversibleRectangle(Handle, splitter_rectangle_moving, 1);
		}
	}

	private void SplitterEndMove(Point location, bool cancel)
	{
		if (!cancel && splitter_rectangle_before_move != splitter_rectangle_moving)
		{
			splitter_rectangle = splitter_rectangle_moving;
			UpdateSplitter();
		}
		SplitterEventArgs e = new SplitterEventArgs(location.X, location.Y, splitter_rectangle.X, splitter_rectangle.Y);
		OnSplitterMoved(e);
	}

	private void SplitterSetCursor(Orientation orientation)
	{
		if (restore_cursor == null)
		{
			restore_cursor = Cursor;
		}
		Cursor = ((orientation != Orientation.Vertical) ? Cursors.HSplit : Cursors.VSplit);
	}

	private void SplitterRestoreCursor()
	{
		if (restore_cursor != null)
		{
			Cursor = restore_cursor;
			restore_cursor = null;
		}
	}

	private void UpdateSplitter()
	{
		SuspendLayout();
		panel1.SuspendLayout();
		panel2.SuspendLayout();
		if (panel1_collapsed)
		{
			panel2.Size = base.Size;
			panel2.Location = new Point(0, 0);
		}
		else if (panel2_collapsed)
		{
			panel1.Size = base.Size;
			panel1.Location = new Point(0, 0);
		}
		else
		{
			panel1.Location = new Point(0, 0);
			if (orientation == Orientation.Vertical)
			{
				splitter_rectangle.Y = 0;
				SplitterPanel splitterPanel = panel1;
				int height = base.Height;
				panel2.InternalHeight = height;
				splitterPanel.InternalHeight = height;
				panel1.InternalWidth = Math.Max(SplitterDistance, panel1_min_size);
				panel2.Location = new Point(SplitterWidth + SplitterDistance, 0);
				panel2.InternalWidth = Math.Max(base.Width - (SplitterWidth + SplitterDistance), panel2_min_size);
				fixed_none_ratio = (double)base.Width / (double)SplitterDistance;
			}
			else if (orientation == Orientation.Horizontal)
			{
				splitter_rectangle.X = 0;
				SplitterPanel splitterPanel2 = panel1;
				int height = base.Width;
				panel2.InternalWidth = height;
				splitterPanel2.InternalWidth = height;
				panel1.InternalHeight = Math.Max(SplitterDistance, panel1_min_size);
				panel2.Location = new Point(0, SplitterWidth + SplitterDistance);
				panel2.InternalHeight = Math.Max(base.Height - (SplitterWidth + SplitterDistance), panel2_min_size);
				fixed_none_ratio = (double)base.Height / (double)SplitterDistance;
			}
		}
		panel1.ResumeLayout();
		panel2.ResumeLayout();
		ResumeLayout();
	}

	private void UpdateLayout()
	{
		panel1.SuspendLayout();
		panel2.SuspendLayout();
		if (panel1_collapsed)
		{
			panel2.Size = base.Size;
			panel2.Location = new Point(0, 0);
		}
		else if (panel2_collapsed)
		{
			panel1.Size = base.Size;
			panel1.Location = new Point(0, 0);
		}
		else
		{
			panel1.Location = new Point(0, 0);
			if (orientation == Orientation.Vertical)
			{
				panel1.Location = new Point(0, 0);
				SplitterPanel splitterPanel = panel1;
				int height = base.Height;
				panel2.InternalHeight = height;
				splitterPanel.InternalHeight = height;
				splitter_rectangle.Height = base.Height;
				if (fixed_panel == FixedPanel.None)
				{
					splitter_rectangle.X = Math.Max((int)Math.Floor((double)base.Width / fixed_none_ratio), panel1_min_size);
					panel1.InternalWidth = SplitterDistance;
					panel2.InternalWidth = base.Width - (SplitterWidth + SplitterDistance);
					panel2.Location = new Point(SplitterWidth + SplitterDistance, 0);
				}
				else if (fixed_panel == FixedPanel.Panel1)
				{
					panel1.InternalWidth = SplitterDistance;
					panel2.InternalWidth = Math.Max(base.Width - (SplitterWidth + SplitterDistance), panel2_min_size);
					panel2.Location = new Point(SplitterWidth + SplitterDistance, 0);
				}
				else if (fixed_panel == FixedPanel.Panel2)
				{
					splitter_rectangle.X = Math.Max(base.Width - (SplitterWidth + panel2.Width), panel1_min_size);
					panel1.InternalWidth = SplitterDistance;
					panel2.Location = new Point(SplitterWidth + SplitterDistance, 0);
				}
			}
			else if (orientation == Orientation.Horizontal)
			{
				panel1.Location = new Point(0, 0);
				SplitterPanel splitterPanel2 = panel1;
				int height = base.Width;
				panel2.InternalWidth = height;
				splitterPanel2.InternalWidth = height;
				splitter_rectangle.Width = base.Width;
				if (fixed_panel == FixedPanel.None)
				{
					splitter_rectangle.Y = Math.Max((int)Math.Floor((double)base.Height / fixed_none_ratio), panel1_min_size);
					panel1.InternalHeight = SplitterDistance;
					panel2.InternalHeight = base.Height - (SplitterWidth + SplitterDistance);
					panel2.Location = new Point(0, SplitterWidth + SplitterDistance);
				}
				else if (fixed_panel == FixedPanel.Panel1)
				{
					panel1.InternalHeight = SplitterDistance;
					panel2.InternalHeight = Math.Max(base.Height - (SplitterWidth + SplitterDistance), panel2_min_size);
					panel2.Location = new Point(0, SplitterWidth + SplitterDistance);
				}
				else if (fixed_panel == FixedPanel.Panel2)
				{
					splitter_rectangle.Y = Math.Max(base.Height - (SplitterWidth + panel2.Height), panel1_min_size);
					panel1.InternalHeight = SplitterDistance;
					panel2.Location = new Point(0, SplitterWidth + SplitterDistance);
				}
			}
		}
		panel1.ResumeLayout();
		panel2.ResumeLayout();
	}
}
