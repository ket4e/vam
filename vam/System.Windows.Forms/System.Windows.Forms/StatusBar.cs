using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultEvent("PanelClick")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.StatusBarDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("Text")]
public class StatusBar : Control
{
	[ListBindable(false)]
	public class StatusBarPanelCollection : ICollection, IEnumerable, IList
	{
		private StatusBar owner;

		private ArrayList panels = new ArrayList();

		private int last_index_by_key;

		private static object UIACollectionChangedEvent;

		bool ICollection.IsSynchronized => panels.IsSynchronized;

		object ICollection.SyncRoot => panels.SyncRoot;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is StatusBarPanel))
				{
					throw new ArgumentException("Value must be of type StatusBarPanel.", "value");
				}
				this[index] = (StatusBarPanel)value;
			}
		}

		bool IList.IsFixedSize => false;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public int Count => panels.Count;

		public bool IsReadOnly => false;

		public virtual StatusBarPanel this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (StatusBarPanel)panels[index];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("index");
				}
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, index));
				value.SetParent(owner);
				panels[index] = value;
				OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, index));
			}
		}

		public virtual StatusBarPanel this[string key]
		{
			get
			{
				int num = IndexOfKey(key);
				if (num >= 0 && num < Count)
				{
					return (StatusBarPanel)panels[num];
				}
				return null;
			}
		}

		internal event CollectionChangeEventHandler UIACollectionChanged
		{
			add
			{
				owner.Events.AddHandler(UIACollectionChangedEvent, value);
			}
			remove
			{
				owner.Events.RemoveHandler(UIACollectionChangedEvent, value);
			}
		}

		public StatusBarPanelCollection(StatusBar owner)
		{
			this.owner = owner;
		}

		static StatusBarPanelCollection()
		{
			UIACollectionChanged = new object();
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			panels.CopyTo(dest, index);
		}

		int IList.Add(object value)
		{
			if (!(value is StatusBarPanel))
			{
				throw new ArgumentException("Value must be of type StatusBarPanel.", "value");
			}
			return AddInternal((StatusBarPanel)value, refresh: true);
		}

		bool IList.Contains(object panel)
		{
			return panels.Contains(panel);
		}

		int IList.IndexOf(object panel)
		{
			return panels.IndexOf(panel);
		}

		void IList.Insert(int index, object value)
		{
			if (!(value is StatusBarPanel))
			{
				throw new ArgumentException("Value must be of type StatusBarPanel.", "value");
			}
			Insert(index, (StatusBarPanel)value);
		}

		void IList.Remove(object value)
		{
			StatusBarPanel value2 = value as StatusBarPanel;
			Remove(value2);
		}

		internal void OnUIACollectionChanged(CollectionChangeEventArgs e)
		{
			((CollectionChangeEventHandler)owner.Events[UIACollectionChanged])?.Invoke(owner, e);
		}

		private int AddInternal(StatusBarPanel p, bool refresh)
		{
			if (p == null)
			{
				throw new ArgumentNullException("value");
			}
			p.SetParent(owner);
			int num = panels.Add(p);
			if (refresh)
			{
				owner.CalcPanelSizes();
				owner.Refresh();
			}
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, num));
			return num;
		}

		public virtual int Add(StatusBarPanel value)
		{
			return AddInternal(value, refresh: true);
		}

		public virtual StatusBarPanel Add(string text)
		{
			StatusBarPanel statusBarPanel = new StatusBarPanel();
			statusBarPanel.Text = text;
			Add(statusBarPanel);
			return statusBarPanel;
		}

		public virtual void AddRange(StatusBarPanel[] panels)
		{
			if (panels == null)
			{
				throw new ArgumentNullException("panels");
			}
			if (panels.Length != 0)
			{
				for (int i = 0; i < panels.Length; i++)
				{
					AddInternal(panels[i], refresh: false);
				}
				owner.Refresh();
			}
		}

		public virtual void Clear()
		{
			panels.Clear();
			owner.Refresh();
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, -1));
		}

		public bool Contains(StatusBarPanel panel)
		{
			return panels.Contains(panel);
		}

		public virtual bool ContainsKey(string key)
		{
			int num = IndexOfKey(key);
			return num >= 0 && num < Count;
		}

		public IEnumerator GetEnumerator()
		{
			return panels.GetEnumerator();
		}

		public int IndexOf(StatusBarPanel panel)
		{
			return panels.IndexOf(panel);
		}

		public virtual int IndexOfKey(string key)
		{
			if (key == null || key == string.Empty)
			{
				return -1;
			}
			if (last_index_by_key >= 0 && last_index_by_key < Count && string.Compare(((StatusBarPanel)panels[last_index_by_key]).Name, key, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return last_index_by_key;
			}
			for (int i = 0; i < Count; i++)
			{
				if (panels[i] is StatusBarPanel statusBarPanel && string.Compare(statusBarPanel.Name, key, StringComparison.OrdinalIgnoreCase) == 0)
				{
					last_index_by_key = i;
					return i;
				}
			}
			return -1;
		}

		public virtual void Insert(int index, StatusBarPanel value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (index > Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			value.SetParent(owner);
			panels.Insert(index, value);
			owner.Refresh();
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, index));
		}

		public virtual void Remove(StatusBarPanel value)
		{
			int num = IndexOf(value);
			panels.Remove(value);
			if (num >= 0)
			{
				OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, num));
			}
		}

		public virtual void RemoveAt(int index)
		{
			panels.RemoveAt(index);
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, index));
		}

		public virtual void RemoveByKey(string key)
		{
			int num = IndexOfKey(key);
			if (num >= 0 && num < Count)
			{
				RemoveAt(num);
			}
		}
	}

	private StatusBarPanelCollection panels;

	private bool show_panels;

	private bool sizing_grip = true;

	private Timer tooltip_timer;

	private ToolTip tooltip_window;

	private StatusBarPanel tooltip_currently_showing;

	private static object DrawItemEvent;

	private static object PanelClickEvent;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Localizable(true)]
	[DefaultValue(DockStyle.Bottom)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool DoubleBuffered
	{
		get
		{
			return base.DoubleBuffered;
		}
		set
		{
			base.DoubleBuffered = value;
		}
	}

	[Localizable(true)]
	public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			if (value != Font)
			{
				base.Font = value;
				UpdateStatusBar();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[MergableProperty(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Localizable(true)]
	public StatusBarPanelCollection Panels
	{
		get
		{
			if (panels == null)
			{
				panels = new StatusBarPanelCollection(this);
			}
			return panels;
		}
	}

	[DefaultValue(false)]
	public bool ShowPanels
	{
		get
		{
			return show_panels;
		}
		set
		{
			if (show_panels != value)
			{
				show_panels = value;
				UpdateStatusBar();
			}
		}
	}

	[DefaultValue(true)]
	public bool SizingGrip
	{
		get
		{
			return sizing_grip;
		}
		set
		{
			if (sizing_grip != value)
			{
				sizing_grip = value;
				UpdateStatusBar();
			}
		}
	}

	[DefaultValue(false)]
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
			if (!(value == Text))
			{
				base.Text = value;
				UpdateStatusBar();
			}
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override ImeMode DefaultImeMode => ImeMode.Disable;

	protected override Size DefaultSize => ThemeEngine.Current.StatusBarDefaultSize;

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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BackColorChanged
	{
		add
		{
			base.BackColorChanged += value;
		}
		remove
		{
			base.BackColorChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event PaintEventHandler Paint
	{
		add
		{
			base.Paint += value;
		}
		remove
		{
			base.Paint -= value;
		}
	}

	public event StatusBarDrawItemEventHandler DrawItem
	{
		add
		{
			base.Events.AddHandler(DrawItemEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DrawItemEvent, value);
		}
	}

	public event StatusBarPanelClickEventHandler PanelClick
	{
		add
		{
			base.Events.AddHandler(PanelClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PanelClickEvent, value);
		}
	}

	public StatusBar()
	{
		Dock = DockStyle.Bottom;
		TabStop = false;
		SetStyle(ControlStyles.UserPaint | ControlStyles.Selectable, value: false);
		base.MouseMove += StatusBar_MouseMove;
		base.MouseLeave += StatusBar_MouseLeave;
	}

	static StatusBar()
	{
		DrawItem = new object();
		PanelClick = new object();
	}

	public override string ToString()
	{
		return base.ToString() + ", Panels.Count: " + Panels.Count + ((Panels.Count <= 0) ? string.Empty : (", Panels[0]: " + Panels[0]));
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
	{
		((StatusBarDrawItemEventHandler)base.Events[DrawItem])?.Invoke(this, sbdievent);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		CalcPanelSizes();
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		base.OnLayout(levent);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (panels == null)
		{
			return;
		}
		float num = 0f;
		float num2 = ThemeEngine.Current.StatusBarHorzGapWidth;
		for (int i = 0; i < panels.Count; i++)
		{
			float num3 = (float)panels[i].Width + num + ((i != panels.Count - 1) ? (num2 / 2f) : num2);
			if ((float)e.X >= num && (float)e.X <= num3)
			{
				OnPanelClick(new StatusBarPanelClickEventArgs(panels[i], e.Button, e.Clicks, e.X, e.Y));
				break;
			}
			num = num3;
		}
		base.OnMouseDown(e);
	}

	protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e)
	{
		((StatusBarPanelClickEventHandler)base.Events[PanelClick])?.Invoke(this, e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		if (base.Width > 0 && base.Height > 0)
		{
			UpdateStatusBar();
		}
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	internal void OnDrawItemInternal(StatusBarDrawItemEventArgs e)
	{
		OnDrawItem(e);
	}

	internal void UpdatePanel(StatusBarPanel panel)
	{
		if (panel.AutoSize == StatusBarPanelAutoSize.Contents)
		{
			UpdateStatusBar();
		}
		else
		{
			UpdateStatusBar();
		}
	}

	internal void UpdatePanelContents(StatusBarPanel panel)
	{
		if (panel.AutoSize == StatusBarPanelAutoSize.Contents)
		{
			UpdateStatusBar();
			Invalidate();
		}
		else
		{
			Invalidate(new Rectangle(panel.X + 2, 2, panel.Width - 4, bounds.Height - 4));
		}
	}

	private void UpdateStatusBar()
	{
		CalcPanelSizes();
		Refresh();
	}

	internal override void OnPaintInternal(PaintEventArgs pevent)
	{
		Draw(pevent.Graphics, pevent.ClipRectangle);
	}

	private void CalcPanelSizes()
	{
		if (panels == null || !show_panels || base.Width == 0 || base.Height == 0)
		{
			return;
		}
		int num = 2;
		int statusBarHorzGapWidth = ThemeEngine.Current.StatusBarHorzGapWidth;
		int num2 = 0;
		ArrayList arrayList = null;
		num2 = num;
		for (int i = 0; i < panels.Count; i++)
		{
			StatusBarPanel statusBarPanel = panels[i];
			if (statusBarPanel.AutoSize == StatusBarPanelAutoSize.None)
			{
				num2 += statusBarPanel.Width;
				num2 += statusBarHorzGapWidth;
			}
			else if (statusBarPanel.AutoSize == StatusBarPanelAutoSize.Contents)
			{
				int num3 = (int)(TextRenderer.MeasureString(statusBarPanel.Text, Font).Width + 0.5f);
				if (statusBarPanel.Icon != null)
				{
					num3 += 21;
				}
				statusBarPanel.SetWidth(num3 + 8);
				num2 += statusBarPanel.Width;
				num2 += statusBarHorzGapWidth;
			}
			else if (statusBarPanel.AutoSize == StatusBarPanelAutoSize.Spring)
			{
				if (arrayList == null)
				{
					arrayList = new ArrayList();
				}
				arrayList.Add(statusBarPanel);
				num2 += statusBarHorzGapWidth;
			}
		}
		if (arrayList != null)
		{
			int count = arrayList.Count;
			int num4 = base.Width - num2 - (SizingGrip ? ThemeEngine.Current.StatusBarSizeGripWidth : 0);
			for (int j = 0; j < count; j++)
			{
				StatusBarPanel statusBarPanel2 = (StatusBarPanel)arrayList[j];
				int num5 = num4 / count;
				statusBarPanel2.SetWidth((num5 < statusBarPanel2.MinWidth) ? statusBarPanel2.MinWidth : num5);
			}
		}
		num2 = num;
		for (int k = 0; k < panels.Count; k++)
		{
			StatusBarPanel statusBarPanel3 = panels[k];
			statusBarPanel3.X = num2;
			num2 += statusBarPanel3.Width + statusBarHorzGapWidth;
		}
	}

	private void Draw(Graphics dc, Rectangle clip)
	{
		ThemeEngine.Current.DrawStatusBar(dc, clip, this);
	}

	private void StatusBar_MouseMove(object sender, MouseEventArgs e)
	{
		if (show_panels)
		{
			StatusBarPanel panelAtPoint = GetPanelAtPoint(e.Location);
			if (panelAtPoint != tooltip_currently_showing)
			{
				MouseLeftPanel(tooltip_currently_showing);
			}
			if (panelAtPoint != null && tooltip_currently_showing == null)
			{
				MouseEnteredPanel(panelAtPoint);
			}
		}
	}

	private void StatusBar_MouseLeave(object sender, EventArgs e)
	{
		if (tooltip_currently_showing != null)
		{
			MouseLeftPanel(tooltip_currently_showing);
		}
	}

	private StatusBarPanel GetPanelAtPoint(Point point)
	{
		foreach (StatusBarPanel panel in Panels)
		{
			if (point.X >= panel.X && point.X <= panel.X + panel.Width)
			{
				return panel;
			}
		}
		return null;
	}

	private void MouseEnteredPanel(StatusBarPanel item)
	{
		tooltip_currently_showing = item;
		ToolTipTimer.Start();
	}

	private void MouseLeftPanel(StatusBarPanel item)
	{
		ToolTipTimer.Stop();
		ToolTipWindow.Hide(this);
		tooltip_currently_showing = null;
	}

	private void ToolTipTimer_Tick(object o, EventArgs args)
	{
		string toolTipText = tooltip_currently_showing.ToolTipText;
		if (toolTipText != null && toolTipText.Length > 0)
		{
			ToolTipWindow.Present(this, toolTipText);
		}
		ToolTipTimer.Stop();
	}
}
