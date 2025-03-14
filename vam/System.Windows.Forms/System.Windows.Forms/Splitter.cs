using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("SplitterMoved")]
[Designer("System.Windows.Forms.Design.SplitterDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("Dock")]
[ComVisible(true)]
public class Splitter : Control
{
	private static Cursor splitter_ns;

	private static Cursor splitter_we;

	private new BorderStyle border_style;

	private int min_extra;

	private int min_size;

	private int max_size;

	private int splitter_size;

	private bool horizontal;

	private Control affected;

	private int split_requested;

	private int splitter_prev_move;

	private Rectangle splitter_rectangle_moving;

	private int moving_offset;

	private static object SplitterMovedEvent;

	private static object SplitterMovingEvent;

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DefaultValue(AnchorStyles.None)]
	public override AnchorStyles Anchor
	{
		get
		{
			return AnchorStyles.None;
		}
		set
		{
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[MWFDescription("Sets the border style for the splitter")]
	[DispId(-504)]
	[DefaultValue(BorderStyle.None)]
	[MWFCategory("Appearance")]
	public BorderStyle BorderStyle
	{
		get
		{
			return border_style;
		}
		set
		{
			border_style = value;
			switch (value)
			{
			case BorderStyle.FixedSingle:
				splitter_size = 4;
				break;
			case BorderStyle.Fixed3D:
				value = BorderStyle.None;
				splitter_size = 3;
				break;
			case BorderStyle.None:
				splitter_size = 3;
				break;
			default:
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for BorderStyle");
			}
			base.InternalBorderStyle = value;
		}
	}

	[DefaultValue(DockStyle.Left)]
	[Localizable(true)]
	public override DockStyle Dock
	{
		get
		{
			return base.Dock;
		}
		set
		{
			if (!Enum.IsDefined(typeof(DockStyle), value) || value == DockStyle.None || value == DockStyle.Fill)
			{
				throw new ArgumentException("Splitter must be docked left, top, bottom or right");
			}
			if (value == DockStyle.Top || value == DockStyle.Bottom)
			{
				horizontal = true;
				Cursor = splitter_ns;
			}
			else
			{
				horizontal = false;
				Cursor = splitter_we;
			}
			base.Dock = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			base.ForeColor = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[MWFCategory("Behaviour")]
	[DefaultValue(25)]
	[Localizable(true)]
	[MWFDescription("Sets minimum size of undocked window")]
	public int MinExtra
	{
		get
		{
			return min_extra;
		}
		set
		{
			min_extra = value;
		}
	}

	[DefaultValue(25)]
	[Localizable(true)]
	[MWFDescription("Sets minimum size of the resized control")]
	[MWFCategory("Behaviour")]
	public int MinSize
	{
		get
		{
			return min_size;
		}
		set
		{
			min_size = value;
		}
	}

	internal int MaxSize
	{
		get
		{
			if (base.Parent == null)
			{
				return 0;
			}
			if (affected == null)
			{
				affected = AffectedControl;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (Control control in base.Parent.Controls)
			{
				if (control == affected)
				{
					continue;
				}
				switch (control.Dock)
				{
				case DockStyle.Left:
				case DockStyle.Right:
					num += control.Width;
					if (control.Location.X < base.Location.X)
					{
						num3 += control.Width;
					}
					break;
				case DockStyle.Top:
				case DockStyle.Bottom:
					num2 += control.Height;
					if (control.Location.Y < base.Location.Y)
					{
						num4 += control.Height;
					}
					break;
				}
			}
			if (horizontal)
			{
				moving_offset = num4;
				return base.Parent.ClientSize.Height - num2 - MinExtra;
			}
			moving_offset = num3;
			return base.Parent.ClientSize.Width - num - MinExtra;
		}
	}

	[MWFCategory("Layout")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MWFDescription("Current splitter position")]
	public int SplitPosition
	{
		get
		{
			affected = AffectedControl;
			if (affected == null)
			{
				return -1;
			}
			if (base.Capture)
			{
				return CalculateSplitPosition();
			}
			if (horizontal)
			{
				return affected.Height;
			}
			return affected.Width;
		}
		set
		{
			if (value > MaxSize)
			{
				value = MaxSize;
			}
			if (value < MinSize)
			{
				value = MinSize;
			}
			affected = AffectedControl;
			if (affected == null)
			{
				split_requested = value;
				return;
			}
			if (horizontal)
			{
				affected.Height = value;
			}
			else
			{
				affected.Width = value;
			}
			OnSplitterMoved(new SplitterEventArgs(base.Left, base.Top, value, value));
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[Bindable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Cursor DefaultCursor => base.DefaultCursor;

	protected override ImeMode DefaultImeMode => ImeMode.Disable;

	protected override Size DefaultSize => new Size(3, 3);

	private Control AffectedControl
	{
		get
		{
			if (base.Parent == null)
			{
				return null;
			}
			for (int i = base.Parent.Controls.GetChildIndex(this) + 1; i < base.Parent.Controls.Count; i++)
			{
				switch (Dock)
				{
				case DockStyle.Top:
					if (base.Top == base.Parent.Controls[i].Bottom)
					{
						return base.Parent.Controls[i];
					}
					break;
				case DockStyle.Bottom:
					if (base.Bottom == base.Parent.Controls[i].Top)
					{
						return base.Parent.Controls[i];
					}
					break;
				case DockStyle.Left:
					if (base.Left == base.Parent.Controls[i].Right)
					{
						return base.Parent.Controls[i];
					}
					break;
				case DockStyle.Right:
					if (base.Right == base.Parent.Controls[i].Left)
					{
						return base.Parent.Controls[i];
					}
					break;
				}
			}
			return null;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
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
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	public event SplitterEventHandler SplitterMoving
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

	public Splitter()
	{
		min_extra = 25;
		min_size = 25;
		split_requested = -1;
		splitter_size = 3;
		horizontal = false;
		SetStyle(ControlStyles.Selectable, value: false);
		Anchor = AnchorStyles.None;
		Dock = DockStyle.Left;
		base.Layout += LayoutSplitter;
		base.ParentChanged += ReparentSplitter;
		Cursor = splitter_we;
	}

	static Splitter()
	{
		SplitterMoved = new object();
		SplitterMoving = new object();
		splitter_ns = Cursors.HSplit;
		splitter_we = Cursors.VSplit;
	}

	public override string ToString()
	{
		return base.ToString() + $", MinExtra: {min_extra}, MinSize: {min_size}";
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (base.Capture && e.KeyCode == Keys.Escape)
		{
			base.Capture = false;
			SplitterEndMove(Point.Empty, cancel: true);
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if (affected == null)
		{
			affected = AffectedControl;
		}
		max_size = MaxSize;
		if (affected != null && e.Button == MouseButtons.Left)
		{
			base.Capture = true;
			SplitterBeginMove(base.Parent.PointToClient(PointToScreen(new Point(e.X, e.Y))));
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		if (base.Capture && e.Button == MouseButtons.Left && affected != null)
		{
			SplitterMove(base.Parent.PointToClient(PointToScreen(new Point(e.X, e.Y))));
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (!base.Capture || e.Button != MouseButtons.Left || affected == null)
		{
			base.OnMouseUp(e);
			return;
		}
		base.OnMouseUp(e);
		base.Capture = false;
		SplitterEndMove(base.Parent.PointToClient(PointToScreen(new Point(e.X, e.Y))), cancel: false);
	}

	private void SplitterBeginMove(Point location)
	{
		splitter_rectangle_moving = new Rectangle(Bounds.X, Bounds.Y, base.Width, base.Height);
		splitter_prev_move = ((!horizontal) ? location.X : location.Y);
	}

	private void SplitterMove(Point location)
	{
		int num = ((!horizontal) ? location.X : location.Y);
		int num2 = num - splitter_prev_move;
		Rectangle rect = splitter_rectangle_moving;
		bool flag = false;
		int num3 = MinSize + moving_offset;
		int num4 = max_size + moving_offset;
		if (horizontal)
		{
			if (splitter_rectangle_moving.Y + num2 > num3 && splitter_rectangle_moving.Y + num2 < num4)
			{
				splitter_rectangle_moving.Y += num2;
				flag = true;
			}
			else if (splitter_rectangle_moving.Y + num2 <= num3 && splitter_rectangle_moving.Y != num3)
			{
				splitter_rectangle_moving.Y = num3;
				flag = true;
			}
			else if (splitter_rectangle_moving.Y + num2 >= num4 && splitter_rectangle_moving.Y != num4)
			{
				splitter_rectangle_moving.Y = num4;
				flag = true;
			}
		}
		else if (splitter_rectangle_moving.X + num2 > num3 && splitter_rectangle_moving.X + num2 < num4)
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
		if (flag)
		{
			splitter_prev_move = num;
			OnSplitterMoving(new SplitterEventArgs(location.X, location.Y, splitter_rectangle_moving.X, splitter_rectangle_moving.Y));
			XplatUI.DrawReversibleRectangle(base.Parent.Handle, rect, 1);
			XplatUI.DrawReversibleRectangle(base.Parent.Handle, splitter_rectangle_moving, 1);
		}
	}

	private void SplitterEndMove(Point location, bool cancel)
	{
		if (!cancel)
		{
			if (horizontal)
			{
				affected.Height = CalculateSplitPosition();
			}
			else
			{
				affected.Width = CalculateSplitPosition();
			}
		}
		base.Parent.Refresh();
		SplitterEventArgs sevent = new SplitterEventArgs(location.X, location.Y, splitter_rectangle_moving.X, splitter_rectangle_moving.Y);
		OnSplitterMoved(sevent);
	}

	protected virtual void OnSplitterMoved(SplitterEventArgs sevent)
	{
		((SplitterEventHandler)base.Events[SplitterMoved])?.Invoke(this, sevent);
	}

	protected virtual void OnSplitterMoving(SplitterEventArgs sevent)
	{
		((SplitterEventHandler)base.Events[SplitterMoving])?.Invoke(this, sevent);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		if (horizontal)
		{
			splitter_size = height;
			if (splitter_size < 1)
			{
				splitter_size = 3;
			}
			base.SetBoundsCore(x, y, width, splitter_size, specified);
		}
		else
		{
			splitter_size = width;
			if (splitter_size < 1)
			{
				splitter_size = 3;
			}
			base.SetBoundsCore(x, y, splitter_size, height, specified);
		}
	}

	private int CalculateSplitPosition()
	{
		if (horizontal)
		{
			if (Dock == DockStyle.Top)
			{
				return splitter_rectangle_moving.Y - affected.Top;
			}
			return affected.Bottom - splitter_rectangle_moving.Y - splitter_size;
		}
		if (Dock == DockStyle.Left)
		{
			return splitter_rectangle_moving.X - affected.Left;
		}
		return affected.Right - splitter_rectangle_moving.X - splitter_size;
	}

	internal override void OnPaintInternal(PaintEventArgs e)
	{
		e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), e.ClipRectangle);
	}

	private void LayoutSplitter(object sender, LayoutEventArgs e)
	{
		affected = AffectedControl;
		if (split_requested != -1)
		{
			SplitPosition = split_requested;
			split_requested = -1;
		}
	}

	private void ReparentSplitter(object sender, EventArgs e)
	{
		affected = null;
	}
}
