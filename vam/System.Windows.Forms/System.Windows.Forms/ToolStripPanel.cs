using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.ToolStripPanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ComVisible(true)]
[ToolboxBitmap("")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class ToolStripPanel : ContainerControl, IDisposable, IComponent, IBindableComponent, IDropTarget
{
	[ListBindable(false)]
	[ComVisible(false)]
	public class ToolStripPanelRowCollection : ArrangedElementCollection, ICollection, IEnumerable, IList
	{
		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			[System.MonoTODO("Stub, does nothing")]
			set
			{
			}
		}

		bool IList.IsFixedSize => base.IsFixedSize;

		bool IList.IsReadOnly => IsReadOnly;

		public new virtual ToolStripPanelRow this[int index] => (ToolStripPanelRow)base[index];

		public ToolStripPanelRowCollection(ToolStripPanel owner)
		{
		}

		public ToolStripPanelRowCollection(ToolStripPanel owner, ToolStripPanelRow[] value)
			: this(owner)
		{
			if (value != null)
			{
				foreach (ToolStripPanelRow value2 in value)
				{
					Add(value2);
				}
			}
		}

		int IList.Add(object value)
		{
			return Add(value as ToolStripPanelRow);
		}

		void IList.Clear()
		{
			Clear();
		}

		bool IList.Contains(object value)
		{
			return Contains(value as ToolStripPanelRow);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf(value as ToolStripPanelRow);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, value as ToolStripPanelRow);
		}

		void IList.Remove(object value)
		{
			Remove(value as ToolStripPanelRow);
		}

		void IList.RemoveAt(int index)
		{
			InternalRemoveAt(index);
		}

		public int Add(ToolStripPanelRow value)
		{
			return Add((object)value);
		}

		public void AddRange(ToolStripPanelRowCollection value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			foreach (ToolStripPanelRow item in value)
			{
				Add(item);
			}
		}

		public void AddRange(ToolStripPanelRow[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			foreach (ToolStripPanelRow value2 in value)
			{
				Add(value2);
			}
		}

		public new virtual void Clear()
		{
			base.Clear();
		}

		public bool Contains(ToolStripPanelRow value)
		{
			return Contains((object)value);
		}

		public void CopyTo(ToolStripPanelRow[] array, int index)
		{
			CopyTo((Array)array, index);
		}

		public int IndexOf(ToolStripPanelRow value)
		{
			return IndexOf((object)value);
		}

		public void Insert(int index, ToolStripPanelRow value)
		{
			Insert(index, (object)value);
		}

		public void Remove(ToolStripPanelRow value)
		{
			Remove((object)value);
		}

		public void RemoveAt(int index)
		{
			InternalRemoveAt(index);
		}
	}

	private class ToolStripPanelControlCollection : ControlCollection
	{
		public ToolStripPanelControlCollection(Control owner)
			: base(owner)
		{
		}
	}

	private class TabIndexComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			if (!(x is Control) || !(y is Control))
			{
				throw new ArgumentException();
			}
			return (x as Control).TabIndex - (y as Control).TabIndex;
		}
	}

	private bool done_first_layout;

	private LayoutEngine layout_engine;

	private bool locked;

	private Orientation orientation;

	private ToolStripRenderer renderer;

	private ToolStripRenderMode render_mode;

	private Padding row_margin;

	private ToolStripPanelRowCollection rows;

	private static object RendererChangedEvent;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[DefaultValue(true)]
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

	public override DockStyle Dock
	{
		get
		{
			return base.Dock;
		}
		set
		{
			base.Dock = value;
			switch (value)
			{
			case DockStyle.None:
			case DockStyle.Top:
			case DockStyle.Bottom:
				orientation = Orientation.Horizontal;
				break;
			case DockStyle.Left:
			case DockStyle.Right:
				orientation = Orientation.Vertical;
				break;
			}
		}
	}

	public override LayoutEngine LayoutEngine
	{
		get
		{
			if (layout_engine == null)
			{
				layout_engine = new FlowLayout();
			}
			return layout_engine;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DefaultValue(false)]
	[Browsable(false)]
	public bool Locked
	{
		get
		{
			return locked;
		}
		set
		{
			locked = value;
		}
	}

	public Orientation Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			orientation = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
			if (value == ToolStripRenderMode.Professional || value == ToolStripRenderMode.System)
			{
				Renderer = new ToolStripProfessionalRenderer();
			}
			render_mode = value;
		}
	}

	public Padding RowMargin
	{
		get
		{
			return row_margin;
		}
		set
		{
			row_margin = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ToolStripPanelRow[] Rows
	{
		get
		{
			ToolStripPanelRow[] array = new ToolStripPanelRow[rows.Count];
			rows.CopyTo(array, 0);
			return array;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	protected override Padding DefaultMargin => new Padding(0);

	protected override Padding DefaultPadding => new Padding(0);

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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	public ToolStripPanel()
	{
		base.AutoSize = true;
		locked = false;
		renderer = null;
		render_mode = ToolStripRenderMode.ManagerRenderMode;
		row_margin = new Padding(3, 0, 0, 0);
		rows = new ToolStripPanelRowCollection(this);
	}

	static ToolStripPanel()
	{
		RendererChanged = new object();
	}

	public void BeginInit()
	{
	}

	public void EndInit()
	{
	}

	[System.MonoTODO("Not implemented")]
	public void Join(ToolStrip toolStripToDrag)
	{
		if (!Contains(toolStripToDrag))
		{
			base.Controls.Add(toolStripToDrag);
		}
	}

	[System.MonoTODO("Not implemented")]
	public void Join(ToolStrip toolStripToDrag, int row)
	{
		Join(toolStripToDrag);
	}

	[System.MonoTODO("Not implemented")]
	public void Join(ToolStrip toolStripToDrag, Point location)
	{
		Join(toolStripToDrag);
	}

	[System.MonoTODO("Not implemented")]
	public void Join(ToolStrip toolStripToDrag, int x, int y)
	{
		Join(toolStripToDrag);
	}

	public ToolStripPanelRow PointToRow(Point clientLocation)
	{
		foreach (ToolStripPanelRow row in rows)
		{
			if (row.Bounds.Contains(clientLocation))
			{
				return row;
			}
		}
		return null;
	}

	protected override ControlCollection CreateControlsInstance()
	{
		return new ToolStripPanelControlCollection(this);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected override void OnControlAdded(ControlEventArgs e)
	{
		if (Dock == DockStyle.Left || Dock == DockStyle.Right)
		{
			(e.Control as ToolStrip).LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
		}
		else
		{
			(e.Control as ToolStrip).LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
		}
		if (done_first_layout && e.Control is ToolStrip)
		{
			AddControlToRows(e.Control);
		}
		base.OnControlAdded(e);
	}

	protected override void OnControlRemoved(ControlEventArgs e)
	{
		base.OnControlRemoved(e);
		foreach (ToolStripPanelRow row in rows)
		{
			if (row.controls.Contains(e.Control))
			{
				row.OnControlRemoved(e.Control, 0);
			}
		}
	}

	protected override void OnDockChanged(EventArgs e)
	{
		base.OnDockChanged(e);
	}

	protected override void OnLayout(LayoutEventArgs e)
	{
		if (!base.Created)
		{
			return;
		}
		if (!done_first_layout)
		{
			ArrayList arrayList = new ArrayList(base.Controls);
			arrayList.Sort(new TabIndexComparer());
			foreach (ToolStrip item in arrayList)
			{
				AddControlToRows(item);
			}
			done_first_layout = true;
		}
		Point location = DisplayRectangle.Location;
		if (Dock == DockStyle.Left || Dock == DockStyle.Right)
		{
			foreach (ToolStripPanelRow row in rows)
			{
				row.SetBounds(new Rectangle(location, new Size(row.Bounds.Width, base.Height)));
				location.X += row.Bounds.Width;
			}
			if (rows.Count > 0)
			{
				int right = rows[rows.Count - 1].Bounds.Right;
				if (right != base.Width)
				{
					SetBounds(bounds.X, bounds.Y, right, bounds.Bottom);
				}
			}
		}
		else
		{
			foreach (ToolStripPanelRow row2 in rows)
			{
				row2.SetBounds(new Rectangle(location, new Size(base.Width, row2.Bounds.Height)));
				location.Y += row2.Bounds.Height;
			}
			if (rows.Count > 0)
			{
				int bottom = rows[rows.Count - 1].Bounds.Bottom;
				if (bottom != base.Height)
				{
					SetBounds(bounds.X, bounds.Y, bounds.Width, bottom);
				}
			}
		}
		Invalidate();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
		Renderer.DrawToolStripPanelBackground(new ToolStripPanelRenderEventArgs(e.Graphics, this));
	}

	protected override void OnParentChanged(EventArgs e)
	{
		base.OnParentChanged(e);
	}

	protected virtual void OnRendererChanged(EventArgs e)
	{
		((EventHandler)base.Events[RendererChanged])?.Invoke(this, e);
	}

	protected override void OnRightToLeftChanged(EventArgs e)
	{
		base.OnRightToLeftChanged(e);
	}

	private void AddControlToRows(Control control)
	{
		if (rows.Count > 0 && rows[rows.Count - 1].CanMove((ToolStrip)control))
		{
			rows[rows.Count - 1].OnControlAdded(control, 0);
			return;
		}
		ToolStripPanelRow toolStripPanelRow = new ToolStripPanelRow(this);
		if (Dock == DockStyle.Left || Dock == DockStyle.Right)
		{
			toolStripPanelRow.SetBounds(new Rectangle(0, 0, 25, base.Height));
		}
		else
		{
			toolStripPanelRow.SetBounds(new Rectangle(0, 0, base.Width, 25));
		}
		rows.Add(toolStripPanelRow);
		toolStripPanelRow.OnControlAdded(control, 0);
	}
}
