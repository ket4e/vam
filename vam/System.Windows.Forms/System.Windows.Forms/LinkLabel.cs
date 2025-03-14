using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Theming;

namespace System.Windows.Forms;

[DefaultEvent("LinkClicked")]
[ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class LinkLabel : Label, IButtonControl
{
	internal class Piece
	{
		public string text;

		public int start;

		public int length;

		public Link link;

		public Region region;

		public Piece(int start, int length, string text, Link link)
		{
			this.start = start;
			this.length = length;
			this.text = text;
			this.link = link;
		}
	}

	[TypeConverter(typeof(LinkConverter))]
	public class Link
	{
		private bool enabled;

		internal int length;

		private object linkData;

		private int start;

		private bool visited;

		private LinkLabel owner;

		private bool hovered;

		internal ArrayList pieces;

		private bool focused;

		private bool active;

		private string description;

		private string name;

		private object tag;

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		[DefaultValue("")]
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

		[Bindable(true)]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue(null)]
		[Localizable(false)]
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

		[DefaultValue(true)]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled != value)
				{
					Invalidate();
				}
				enabled = value;
			}
		}

		public int Length
		{
			get
			{
				if (length == -1)
				{
					return owner.Text.Length;
				}
				return length;
			}
			set
			{
				if (length != value)
				{
					length = value;
					owner.CreateLinkPieces();
				}
			}
		}

		[DefaultValue(null)]
		public object LinkData
		{
			get
			{
				return linkData;
			}
			set
			{
				linkData = value;
			}
		}

		public int Start
		{
			get
			{
				return start;
			}
			set
			{
				if (start != value)
				{
					start = value;
					owner.sorted_links = null;
					owner.CreateLinkPieces();
				}
			}
		}

		[DefaultValue(false)]
		public bool Visited
		{
			get
			{
				return visited;
			}
			set
			{
				if (visited != value)
				{
					Invalidate();
				}
				visited = value;
			}
		}

		internal bool Hovered
		{
			get
			{
				return hovered;
			}
			set
			{
				if (hovered != value)
				{
					Invalidate();
				}
				hovered = value;
			}
		}

		internal bool Focused
		{
			get
			{
				return focused;
			}
			set
			{
				if (focused != value)
				{
					Invalidate();
				}
				focused = value;
			}
		}

		internal bool Active
		{
			get
			{
				return active;
			}
			set
			{
				if (active != value)
				{
					Invalidate();
				}
				active = value;
			}
		}

		internal LinkLabel Owner
		{
			set
			{
				owner = value;
			}
		}

		internal Link(LinkLabel owner)
		{
			focused = false;
			enabled = true;
			visited = false;
			length = (start = 0);
			linkData = null;
			this.owner = owner;
			pieces = new ArrayList();
			name = string.Empty;
		}

		public Link()
		{
			enabled = true;
			name = string.Empty;
			pieces = new ArrayList();
		}

		public Link(int start, int length)
			: this()
		{
			this.start = start;
			this.length = length;
		}

		public Link(int start, int length, object linkData)
			: this(start, length)
		{
			this.linkData = linkData;
		}

		private void Invalidate()
		{
			for (int i = 0; i < pieces.Count; i++)
			{
				owner.Invalidate(((Piece)pieces[i]).region);
			}
		}

		internal bool Contains(int x, int y)
		{
			foreach (Piece piece in pieces)
			{
				if (piece.region.IsVisible(new Point(x, y)))
				{
					return true;
				}
			}
			return false;
		}
	}

	private class LinkComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			Link link = (Link)x;
			Link link2 = (Link)y;
			return link.Start - link2.Start;
		}
	}

	public class LinkCollection : ICollection, IEnumerable, IList
	{
		private LinkLabel owner;

		private bool links_added;

		bool IList.IsFixedSize => false;

		object IList.this[int index]
		{
			get
			{
				return owner.links[index];
			}
			set
			{
				owner.links[index] = value;
			}
		}

		object ICollection.SyncRoot => this;

		bool ICollection.IsSynchronized => false;

		[Browsable(false)]
		public int Count => owner.links.Count;

		public bool IsReadOnly => false;

		public virtual Link this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				return (Link)owner.links[index];
			}
			set
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				owner.links[index] = value;
			}
		}

		public virtual Link this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty(key))
				{
					return null;
				}
				foreach (Link link in owner.links)
				{
					if (string.Compare(link.Name, key, ignoreCase: true) == 0)
					{
						return link;
					}
				}
				return null;
			}
		}

		internal bool IsDefault => Count == 1 && this[0].Start == 0 && this[0].length == -1;

		public bool LinksAdded => links_added;

		public LinkCollection(LinkLabel owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			owner.links.CopyTo(dest, index);
		}

		int IList.Add(object value)
		{
			int result = owner.links.Add(value);
			owner.sorted_links = null;
			owner.CheckLinks();
			owner.CreateLinkPieces();
			return result;
		}

		bool IList.Contains(object link)
		{
			return Contains((Link)link);
		}

		int IList.IndexOf(object link)
		{
			return owner.links.IndexOf(link);
		}

		void IList.Insert(int index, object value)
		{
			owner.links.Insert(index, value);
			owner.sorted_links = null;
			owner.CheckLinks();
			owner.CreateLinkPieces();
		}

		void IList.Remove(object value)
		{
			Remove((Link)value);
		}

		public int Add(Link value)
		{
			value.Owner = owner;
			if (IsDefault)
			{
				owner.links.Clear();
			}
			int result = owner.links.Add(value);
			links_added = true;
			owner.sorted_links = null;
			owner.CheckLinks();
			owner.CreateLinkPieces();
			return result;
		}

		public Link Add(int start, int length)
		{
			return Add(start, length, null);
		}

		public Link Add(int start, int length, object linkData)
		{
			Link link = new Link(owner);
			link.Length = length;
			link.Start = start;
			link.LinkData = linkData;
			int index = Add(link);
			return (Link)owner.links[index];
		}

		public virtual void Clear()
		{
			owner.links.Clear();
			owner.sorted_links = null;
			owner.CreateLinkPieces();
		}

		public bool Contains(Link link)
		{
			return owner.links.Contains(link);
		}

		public virtual bool ContainsKey(string key)
		{
			return this[key] != null;
		}

		public IEnumerator GetEnumerator()
		{
			return owner.links.GetEnumerator();
		}

		public int IndexOf(Link link)
		{
			return owner.links.IndexOf(link);
		}

		public virtual int IndexOfKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return -1;
			}
			return IndexOf(this[key]);
		}

		public void Remove(Link value)
		{
			owner.links.Remove(value);
			owner.sorted_links = null;
			owner.CreateLinkPieces();
		}

		public virtual void RemoveByKey(string key)
		{
			Remove(this[key]);
		}

		public void RemoveAt(int index)
		{
			if (index >= Count)
			{
				throw new ArgumentOutOfRangeException("Invalid value for array index");
			}
			owner.links.Remove(owner.links[index]);
			owner.sorted_links = null;
			owner.CreateLinkPieces();
		}
	}

	private Color active_link_color;

	private Color disabled_link_color;

	private Color link_color;

	private Color visited_color;

	private LinkArea link_area;

	private LinkBehavior link_behavior;

	private LinkCollection link_collection;

	private ArrayList links = new ArrayList();

	internal Link[] sorted_links;

	private bool link_visited;

	internal Piece[] pieces;

	private Cursor override_cursor;

	private DialogResult dialog_result;

	private Link active_link;

	private Link hovered_link;

	private int focused_index;

	private static object LinkClickedEvent;

	DialogResult IButtonControl.DialogResult
	{
		get
		{
			return dialog_result;
		}
		set
		{
			dialog_result = value;
		}
	}

	public Color ActiveLinkColor
	{
		get
		{
			return active_link_color;
		}
		set
		{
			if (!(active_link_color == value))
			{
				active_link_color = value;
				Invalidate();
			}
		}
	}

	public Color DisabledLinkColor
	{
		get
		{
			return disabled_link_color;
		}
		set
		{
			if (!(disabled_link_color == value))
			{
				disabled_link_color = value;
				Invalidate();
			}
		}
	}

	public Color LinkColor
	{
		get
		{
			return link_color;
		}
		set
		{
			if (!(link_color == value))
			{
				link_color = value;
				Invalidate();
			}
		}
	}

	public Color VisitedLinkColor
	{
		get
		{
			return visited_color;
		}
		set
		{
			if (!(visited_color == value))
			{
				visited_color = value;
				Invalidate();
			}
		}
	}

	[Editor("System.Windows.Forms.Design.LinkAreaEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public LinkArea LinkArea
	{
		get
		{
			return link_area;
		}
		set
		{
			if (value.Start < 0 || value.Length < -1)
			{
				throw new ArgumentException();
			}
			Links.Clear();
			if (!value.IsEmpty)
			{
				Links.Add(value.Start, value.Length);
				link_area = value;
				Invalidate();
			}
		}
	}

	[DefaultValue(LinkBehavior.SystemDefault)]
	public LinkBehavior LinkBehavior
	{
		get
		{
			return link_behavior;
		}
		set
		{
			if (link_behavior != value)
			{
				link_behavior = value;
				Invalidate();
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public LinkCollection Links
	{
		get
		{
			if (link_collection == null)
			{
				link_collection = new LinkCollection(this);
			}
			return link_collection;
		}
	}

	[DefaultValue(false)]
	public bool LinkVisited
	{
		get
		{
			return link_visited;
		}
		set
		{
			if (link_visited != value)
			{
				link_visited = value;
				Invalidate();
			}
		}
	}

	protected Cursor OverrideCursor
	{
		get
		{
			if (override_cursor == null)
			{
				override_cursor = Cursors.Hand;
			}
			return override_cursor;
		}
		set
		{
			override_cursor = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			if (!(base.Text == value))
			{
				base.Text = value;
				CreateLinkPieces();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new FlatStyle FlatStyle
	{
		get
		{
			return base.FlatStyle;
		}
		set
		{
			if (base.FlatStyle != value)
			{
				base.FlatStyle = value;
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			if (!(base.Padding == value))
			{
				base.Padding = value;
				CreateLinkPieces();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	public new bool UseCompatibleTextRendering
	{
		get
		{
			return use_compatible_text_rendering;
		}
		set
		{
			use_compatible_text_rendering = value;
		}
	}

	public event LinkLabelLinkClickedEventHandler LinkClicked
	{
		add
		{
			base.Events.AddHandler(LinkClickedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LinkClickedEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
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

	public LinkLabel()
	{
		LinkArea = new LinkArea(0, -1);
		link_behavior = LinkBehavior.SystemDefault;
		link_visited = false;
		pieces = null;
		focused_index = -1;
		string_format.FormatFlags |= StringFormatFlags.NoClip;
		ActiveLinkColor = Color.Red;
		DisabledLinkColor = ThemeEngine.Current.ColorGrayText;
		LinkColor = Color.FromArgb(255, 0, 0, 255);
		VisitedLinkColor = Color.FromArgb(255, 128, 0, 128);
		SetStyle(ControlStyles.Selectable, value: false);
		SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		CreateLinkPieces();
	}

	static LinkLabel()
	{
		LinkClicked = new object();
	}

	void IButtonControl.NotifyDefault(bool value)
	{
	}

	void IButtonControl.PerformClick()
	{
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return base.CreateAccessibilityInstance();
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
		CreateLinkPieces();
	}

	protected override void OnAutoSizeChanged(EventArgs e)
	{
		base.OnAutoSizeChanged(e);
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
		Invalidate();
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		CreateLinkPieces();
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		if (focused_index == -1)
		{
			if ((Control.ModifierKeys & Keys.Shift) == 0)
			{
				for (int i = 0; i < sorted_links.Length; i++)
				{
					if (sorted_links[i].Enabled)
					{
						focused_index = i;
						break;
					}
				}
			}
			else
			{
				if (focused_index == -1)
				{
					focused_index = sorted_links.Length;
				}
				for (int num = focused_index - 1; num >= 0; num--)
				{
					if (sorted_links[num].Enabled)
					{
						sorted_links[num].Focused = true;
						focused_index = num;
						return;
					}
				}
			}
		}
		if (focused_index != -1)
		{
			sorted_links[focused_index].Focused = true;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && focused_index != -1)
		{
			OnLinkClicked(new LinkLabelLinkClickedEventArgs(sorted_links[focused_index]));
		}
		base.OnKeyDown(e);
	}

	protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
	{
		((LinkLabelLinkClickedEventHandler)base.Events[LinkClicked])?.Invoke(this, e);
	}

	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		if (focused_index != -1)
		{
			sorted_links[focused_index].Focused = false;
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (!base.Enabled)
		{
			return;
		}
		base.OnMouseDown(e);
		for (int i = 0; i < sorted_links.Length; i++)
		{
			if (sorted_links[i].Contains(e.X, e.Y) && sorted_links[i].Enabled)
			{
				sorted_links[i].Active = true;
				if (focused_index != -1)
				{
					sorted_links[focused_index].Focused = false;
				}
				active_link = sorted_links[i];
				focused_index = i;
				sorted_links[focused_index].Focused = true;
				break;
			}
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		if (base.Enabled)
		{
			base.OnMouseLeave(e);
			UpdateHover(null);
		}
	}

	protected override void OnPaddingChanged(EventArgs e)
	{
		base.OnPaddingChanged(e);
	}

	private void UpdateHover(Link link)
	{
		if (link != hovered_link)
		{
			if (hovered_link != null)
			{
				hovered_link.Hovered = false;
			}
			hovered_link = link;
			if (hovered_link != null)
			{
				hovered_link.Hovered = true;
			}
			Cursor = ((hovered_link == null) ? Cursors.Default : OverrideCursor);
			Invalidate();
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		UpdateHover(PointInLink(e.X, e.Y));
		base.OnMouseMove(e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (!base.Enabled)
		{
			return;
		}
		base.OnMouseUp(e);
		if (active_link != null)
		{
			Link link = ((PointInLink(e.X, e.Y) != active_link) ? null : active_link);
			active_link.Active = false;
			active_link = null;
			if (link != null)
			{
				OnLinkClicked(new LinkLabelLinkClickedEventArgs(link, e.Button));
			}
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		InvokePaintBackground(this, e);
		ThemeElements.LinkLabelPainter.Draw(e.Graphics, e.ClipRectangle, this);
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
	}

	protected override void OnTextAlignChanged(EventArgs e)
	{
		CreateLinkPieces();
		base.OnTextAlignChanged(e);
	}

	protected override void OnTextChanged(EventArgs e)
	{
		CreateLinkPieces();
		base.OnTextChanged(e);
	}

	protected Link PointInLink(int x, int y)
	{
		for (int i = 0; i < sorted_links.Length; i++)
		{
			if (sorted_links[i].Contains(x, y))
			{
				return sorted_links[i];
			}
		}
		return null;
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		if ((keyData & Keys.KeyCode) == Keys.Tab)
		{
			Select(directed: true, (keyData & Keys.Shift) == 0);
			return true;
		}
		return base.ProcessDialogKey(keyData);
	}

	protected override void Select(bool directed, bool forward)
	{
		if (!directed)
		{
			return;
		}
		if (focused_index != -1)
		{
			sorted_links[focused_index].Focused = false;
			focused_index = -1;
		}
		if (forward)
		{
			for (int i = focused_index + 1; i < sorted_links.Length; i++)
			{
				if (sorted_links[i].Enabled)
				{
					sorted_links[i].Focused = true;
					focused_index = i;
					base.Select(directed, forward);
					return;
				}
			}
		}
		else
		{
			if (focused_index == -1)
			{
				focused_index = sorted_links.Length;
			}
			for (int num = focused_index - 1; num >= 0; num--)
			{
				if (sorted_links[num].Enabled)
				{
					sorted_links[num].Focused = true;
					focused_index = num;
					base.Select(directed, forward);
					return;
				}
			}
		}
		focused_index = -1;
		if (base.Parent != null)
		{
			base.Parent.SelectNextControl(this, forward, tabStopOnly: false, nested: true, wrap: true);
		}
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, height, specified);
		CreateLinkPieces();
	}

	protected override void WndProc(ref Message msg)
	{
		base.WndProc(ref msg);
	}

	private ArrayList CreatePiecesFromText(int start, int len, Link link)
	{
		ArrayList arrayList = new ArrayList();
		if (start + len > Text.Length)
		{
			len = Text.Length - start;
		}
		if (len < 0)
		{
			return arrayList;
		}
		string text = Text.Substring(start, len);
		int num = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == '\n')
			{
				if (i != 0)
				{
					Piece value = new Piece(start + num, i + 1 - num, text.Substring(num, i + 1 - num), link);
					arrayList.Add(value);
				}
				num = i + 1;
			}
		}
		if (num < text.Length)
		{
			Piece value2 = new Piece(start + num, text.Length - num, text.Substring(num, text.Length - num), link);
			arrayList.Add(value2);
		}
		return arrayList;
	}

	private void CreateLinkPieces()
	{
		if (Text.Length == 0)
		{
			SetStyle(ControlStyles.Selectable, value: false);
			base.TabStop = false;
			link_area.Start = 0;
			link_area.Length = 0;
			return;
		}
		if (Links.Count == 1 && Links[0].Start == 0 && Links[0].Length == -1)
		{
			Links[0].Length = Text.Length;
		}
		SortLinks();
		if (Links.Count > 0)
		{
			link_area.Start = Links[0].Start;
			link_area.Length = Links[0].Length;
		}
		else
		{
			link_area.Start = 0;
			link_area.Length = 0;
		}
		base.TabStop = LinkArea.Length > 0;
		SetStyle(ControlStyles.Selectable, base.TabStop);
		if (!base.IsHandleCreated)
		{
			return;
		}
		ArrayList arrayList = new ArrayList();
		int num = 0;
		for (int i = 0; i < sorted_links.Length; i++)
		{
			int start = sorted_links[i].Start;
			if (start > num)
			{
				ArrayList c = CreatePiecesFromText(num, start - num, null);
				arrayList.AddRange(c);
			}
			ArrayList c2 = CreatePiecesFromText(start, sorted_links[i].Length, sorted_links[i]);
			arrayList.AddRange(c2);
			sorted_links[i].pieces.AddRange(c2);
			num = sorted_links[i].Start + sorted_links[i].Length;
		}
		if (num < Text.Length)
		{
			ArrayList c3 = CreatePiecesFromText(num, Text.Length - num, null);
			arrayList.AddRange(c3);
		}
		pieces = new Piece[arrayList.Count];
		arrayList.CopyTo(pieces, 0);
		CharacterRange[] array = new CharacterRange[pieces.Length];
		for (int j = 0; j < pieces.Length; j++)
		{
			ref CharacterRange reference = ref array[j];
			reference = new CharacterRange(pieces[j].start, pieces[j].length);
		}
		string_format.SetMeasurableCharacterRanges(array);
		Region[] array2 = TextRenderer.MeasureCharacterRanges(Text, ThemeEngine.Current.GetLinkFont(this), base.PaddingClientRectangle, string_format);
		for (int k = 0; k < pieces.Length; k++)
		{
			pieces[k].region = array2[k];
			pieces[k].region.Translate(Padding.Left, Padding.Top);
		}
		Invalidate();
	}

	private void SortLinks()
	{
		if (sorted_links == null)
		{
			sorted_links = new Link[Links.Count];
			((ICollection)Links).CopyTo((Array)sorted_links, 0);
			Array.Sort(sorted_links, new LinkComparer());
		}
	}

	private void CheckLinks()
	{
		SortLinks();
		int num = 0;
		for (int i = 0; i < sorted_links.Length; i++)
		{
			if (sorted_links[i].Start < num)
			{
				throw new InvalidOperationException("Overlapping link regions.");
			}
			num = sorted_links[i].Start + sorted_links[i].Length;
		}
	}
}
