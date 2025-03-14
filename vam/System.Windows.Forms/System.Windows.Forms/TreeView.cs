using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultProperty("Nodes")]
[DefaultEvent("AfterSelect")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Docking(DockingBehavior.Ask)]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.TreeViewDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public class TreeView : Control
{
	private string path_separator = "\\";

	private int item_height = -1;

	private bool sorted;

	internal TreeNode root_node;

	internal bool nodes_added;

	private TreeNodeCollection nodes;

	private TreeViewAction selection_action;

	internal TreeNode selected_node;

	private TreeNode pre_selected_node;

	private TreeNode focused_node;

	internal TreeNode highlighted_node;

	private Rectangle mouse_rect;

	private bool select_mmove;

	private ImageList image_list;

	private int image_index = -1;

	private int selected_image_index = -1;

	private string image_key;

	private bool is_hovering;

	private TreeNode mouse_click_node;

	private bool right_to_left_layout;

	private string selected_image_key;

	private bool show_node_tool_tips;

	private ImageList state_image_list;

	private TreeNode tooltip_currently_showing;

	private ToolTip tooltip_window;

	private bool full_row_select;

	private bool hot_tracking;

	private int indent = 19;

	private NodeLabelEditEventArgs edit_args;

	private LabelEditTextBox edit_text_box;

	internal TreeNode edit_node;

	private bool checkboxes;

	private bool label_edit;

	private bool scrollable = true;

	private bool show_lines = true;

	private bool show_root_lines = true;

	private bool show_plus_minus = true;

	private bool hide_selection = true;

	private int max_visible_order = -1;

	internal VScrollBar vbar;

	internal HScrollBar hbar;

	private bool vbar_bounds_set;

	private bool hbar_bounds_set;

	internal int skipped_nodes;

	internal int hbar_offset;

	private int update_stack;

	private bool update_needed;

	private Pen dash;

	private Color line_color;

	private StringFormat string_format;

	private int drag_begin_x = -1;

	private int drag_begin_y = -1;

	private long handle_count = 1L;

	private TreeViewDrawMode draw_mode;

	private IComparer tree_view_node_sorter;

	private static object ItemDragEvent;

	private static object AfterCheckEvent;

	private static object AfterCollapseEvent;

	private static object AfterExpandEvent;

	private static object AfterLabelEditEvent;

	private static object AfterSelectEvent;

	private static object BeforeCheckEvent;

	private static object BeforeCollapseEvent;

	private static object BeforeExpandEvent;

	private static object BeforeLabelEditEvent;

	private static object BeforeSelectEvent;

	private static object DrawNodeEvent;

	private static object NodeMouseClickEvent;

	private static object NodeMouseDoubleClickEvent;

	private static object NodeMouseHoverEvent;

	private static object RightToLeftLayoutChangedEvent;

	private static object UIACheckBoxesChangedEvent;

	private static object UIALabelEditChangedEvent;

	private static object UIANodeTextChangedEvent;

	private static object UIACollectionChangedEvent;

	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
			CreateDashPen();
			Invalidate();
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

	[DispId(-504)]
	[DefaultValue(BorderStyle.Fixed3D)]
	public BorderStyle BorderStyle
	{
		get
		{
			return base.InternalBorderStyle;
		}
		set
		{
			base.InternalBorderStyle = value;
		}
	}

	[DefaultValue(false)]
	public bool CheckBoxes
	{
		get
		{
			return checkboxes;
		}
		set
		{
			if (value != checkboxes)
			{
				checkboxes = value;
				if (!checkboxes)
				{
					root_node.CollapseAllUncheck();
				}
				Invalidate();
				OnUIACheckBoxesChanged(EventArgs.Empty);
			}
		}
	}

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

	[DefaultValue(false)]
	public bool FullRowSelect
	{
		get
		{
			return full_row_select;
		}
		set
		{
			if (value != full_row_select)
			{
				full_row_select = value;
				Invalidate();
			}
		}
	}

	[DefaultValue(true)]
	public bool HideSelection
	{
		get
		{
			return hide_selection;
		}
		set
		{
			if (hide_selection != value)
			{
				hide_selection = value;
				Invalidate();
			}
		}
	}

	[DefaultValue(false)]
	public bool HotTracking
	{
		get
		{
			return hot_tracking;
		}
		set
		{
			hot_tracking = value;
		}
	}

	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Localizable(true)]
	[RelatedImageList("ImageList")]
	[RefreshProperties(RefreshProperties.Repaint)]
	[TypeConverter(typeof(NoneExcludedImageIndexConverter))]
	[DefaultValue(-1)]
	public int ImageIndex
	{
		get
		{
			return image_index;
		}
		set
		{
			if (value < -1)
			{
				throw new ArgumentException("'" + value + "' is not a valid value for 'value'. 'value' must be greater than or equal to 0.");
			}
			if (image_index != value)
			{
				image_index = value;
				Invalidate();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(null)]
	public ImageList ImageList
	{
		get
		{
			return image_list;
		}
		set
		{
			image_list = value;
			Invalidate();
		}
	}

	[Localizable(true)]
	public int Indent
	{
		get
		{
			return indent;
		}
		set
		{
			if (indent != value)
			{
				if (value > 32000)
				{
					throw new ArgumentException("'" + value + "' is not a valid value for 'Indent'. 'Indent' must be less than or equal to 32000");
				}
				if (value < 0)
				{
					throw new ArgumentException("'" + value + "' is not a valid value for 'Indent'. 'Indent' must be greater than or equal to 0.");
				}
				indent = value;
				Invalidate();
			}
		}
	}

	public int ItemHeight
	{
		get
		{
			if (item_height == -1)
			{
				return base.FontHeight + 3;
			}
			return item_height;
		}
		set
		{
			if (value != item_height)
			{
				item_height = value;
				Invalidate();
			}
		}
	}

	internal int ActualItemHeight
	{
		get
		{
			int num = ItemHeight;
			if (ImageList != null && ImageList.ImageSize.Height > num)
			{
				num = ImageList.ImageSize.Height;
			}
			return num;
		}
	}

	[DefaultValue(false)]
	public bool LabelEdit
	{
		get
		{
			return label_edit;
		}
		set
		{
			label_edit = value;
			OnUIALabelEditChanged(EventArgs.Empty);
		}
	}

	[Localizable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MergableProperty(false)]
	public TreeNodeCollection Nodes => nodes;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[DefaultValue("\\")]
	public string PathSeparator
	{
		get
		{
			return path_separator;
		}
		set
		{
			path_separator = value;
		}
	}

	[DefaultValue(false)]
	[Localizable(true)]
	public virtual bool RightToLeftLayout
	{
		get
		{
			return right_to_left_layout;
		}
		set
		{
			if (right_to_left_layout != value)
			{
				right_to_left_layout = value;
				OnRightToLeftLayoutChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(true)]
	public bool Scrollable
	{
		get
		{
			return scrollable;
		}
		set
		{
			if (scrollable != value)
			{
				scrollable = value;
				UpdateScrollBars(force: false);
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(-1)]
	[RelatedImageList("ImageList")]
	[TypeConverter(typeof(NoneExcludedImageIndexConverter))]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public int SelectedImageIndex
	{
		get
		{
			return selected_image_index;
		}
		set
		{
			if (value < -1)
			{
				throw new ArgumentException("'" + value + "' is not a valid value for 'value'. 'value' must be greater than or equal to 0.");
			}
			UpdateNode(SelectedNode);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TreeNode SelectedNode
	{
		get
		{
			if (!base.IsHandleCreated)
			{
				return pre_selected_node;
			}
			return selected_node;
		}
		set
		{
			if (!base.IsHandleCreated)
			{
				pre_selected_node = value;
				return;
			}
			if (selected_node == value)
			{
				selection_action = TreeViewAction.Unknown;
				return;
			}
			if (value != null)
			{
				TreeViewCancelEventArgs treeViewCancelEventArgs = new TreeViewCancelEventArgs(value, cancel: false, selection_action);
				OnBeforeSelect(treeViewCancelEventArgs);
				if (treeViewCancelEventArgs.Cancel)
				{
					return;
				}
			}
			Rectangle rectangle = Rectangle.Empty;
			if (selected_node != null)
			{
				rectangle = Bloat(selected_node.Bounds);
			}
			if (focused_node != null)
			{
				rectangle = Rectangle.Union(rectangle, Bloat(focused_node.Bounds));
			}
			if (value != null)
			{
				rectangle = Rectangle.Union(rectangle, Bloat(value.Bounds));
			}
			highlighted_node = value;
			selected_node = value;
			focused_node = value;
			if (full_row_select || draw_mode != 0)
			{
				rectangle.X = 0;
				rectangle.Width = ViewportRectangle.Width;
			}
			if (rectangle != Rectangle.Empty)
			{
				Invalidate(rectangle);
			}
			if (selected_node != null)
			{
				selected_node.EnsureVisible();
			}
			if (value != null)
			{
				OnAfterSelect(new TreeViewEventArgs(value, TreeViewAction.Unknown));
			}
			selection_action = TreeViewAction.Unknown;
		}
	}

	[DefaultValue(true)]
	public bool ShowLines
	{
		get
		{
			return show_lines;
		}
		set
		{
			if (show_lines != value)
			{
				show_lines = value;
				Invalidate();
			}
		}
	}

	[DefaultValue(false)]
	public bool ShowNodeToolTips
	{
		get
		{
			return show_node_tool_tips;
		}
		set
		{
			show_node_tool_tips = value;
		}
	}

	[DefaultValue(true)]
	public bool ShowPlusMinus
	{
		get
		{
			return show_plus_minus;
		}
		set
		{
			if (show_plus_minus != value)
			{
				show_plus_minus = value;
				Invalidate();
			}
		}
	}

	[DefaultValue(true)]
	public bool ShowRootLines
	{
		get
		{
			return show_root_lines;
		}
		set
		{
			if (show_root_lines != value)
			{
				show_root_lines = value;
				Invalidate();
			}
		}
	}

	[DefaultValue(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public bool Sorted
	{
		get
		{
			return sorted;
		}
		set
		{
			if (sorted != value)
			{
				sorted = value;
				if (sorted && tree_view_node_sorter == null)
				{
					Sort(null);
				}
			}
		}
	}

	[DefaultValue(null)]
	public ImageList StateImageList
	{
		get
		{
			return state_image_list;
		}
		set
		{
			state_image_list = value;
			Invalidate();
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Bindable(false)]
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

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TreeNode TopNode
	{
		get
		{
			if (root_node.FirstNode == null)
			{
				return null;
			}
			OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(root_node.FirstNode);
			openTreeNodeEnumerator.MoveNext();
			for (int i = 0; i < skipped_nodes; i++)
			{
				openTreeNodeEnumerator.MoveNext();
			}
			return openTreeNodeEnumerator.CurrentNode;
		}
		set
		{
			SetTop(value);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IComparer TreeViewNodeSorter
	{
		get
		{
			return tree_view_node_sorter;
		}
		set
		{
			tree_view_node_sorter = value;
			if (tree_view_node_sorter != null)
			{
				Sort();
				sorted = true;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int VisibleCount => ViewportRectangle.Height / ActualItemHeight;

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

	[DefaultValue("Color [Black]")]
	public Color LineColor
	{
		get
		{
			if (line_color == Color.Empty)
			{
				Color color = ControlPaint.Dark(BackColor);
				if (color == BackColor)
				{
					color = ControlPaint.Light(BackColor);
				}
				return color;
			}
			return line_color;
		}
		set
		{
			line_color = value;
			if (show_lines)
			{
				CreateDashPen();
				Invalidate();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[Localizable(true)]
	[DefaultValue("")]
	[RelatedImageList("ImageList")]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[TypeConverter(typeof(ImageKeyConverter))]
	public string ImageKey
	{
		get
		{
			return image_key;
		}
		set
		{
			if (!(image_key == value))
			{
				image_index = -1;
				image_key = value;
				Invalidate();
			}
		}
	}

	[DefaultValue("")]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[TypeConverter(typeof(ImageKeyConverter))]
	[RelatedImageList("ImageList")]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public string SelectedImageKey
	{
		get
		{
			return selected_image_key;
		}
		set
		{
			if (!(selected_image_key == value))
			{
				selected_image_index = -1;
				selected_image_key = value;
				UpdateNode(SelectedNode);
			}
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

	[DefaultValue(TreeViewDrawMode.Normal)]
	public TreeViewDrawMode DrawMode
	{
		get
		{
			return draw_mode;
		}
		set
		{
			draw_mode = value;
		}
	}

	internal ScrollBar UIAHScrollBar => hbar;

	internal ScrollBar UIAVScrollBar => vbar;

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => new Size(121, 97);

	internal override bool ScaleChildrenInternal => false;

	internal Rectangle ViewportRectangle
	{
		get
		{
			Rectangle clientRectangle = base.ClientRectangle;
			if (vbar != null && vbar.Visible)
			{
				clientRectangle.Width -= vbar.Width;
			}
			if (hbar != null && hbar.Visible)
			{
				clientRectangle.Height -= hbar.Height;
			}
			return clientRectangle;
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

	public event ItemDragEventHandler ItemDrag
	{
		add
		{
			base.Events.AddHandler(ItemDragEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemDragEvent, value);
		}
	}

	public event TreeViewEventHandler AfterCheck
	{
		add
		{
			base.Events.AddHandler(AfterCheckEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AfterCheckEvent, value);
		}
	}

	public event TreeViewEventHandler AfterCollapse
	{
		add
		{
			base.Events.AddHandler(AfterCollapseEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AfterCollapseEvent, value);
		}
	}

	public event TreeViewEventHandler AfterExpand
	{
		add
		{
			base.Events.AddHandler(AfterExpandEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AfterExpandEvent, value);
		}
	}

	public event NodeLabelEditEventHandler AfterLabelEdit
	{
		add
		{
			base.Events.AddHandler(AfterLabelEditEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AfterLabelEditEvent, value);
		}
	}

	public event TreeViewEventHandler AfterSelect
	{
		add
		{
			base.Events.AddHandler(AfterSelectEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AfterSelectEvent, value);
		}
	}

	public event TreeViewCancelEventHandler BeforeCheck
	{
		add
		{
			base.Events.AddHandler(BeforeCheckEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BeforeCheckEvent, value);
		}
	}

	public event TreeViewCancelEventHandler BeforeCollapse
	{
		add
		{
			base.Events.AddHandler(BeforeCollapseEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BeforeCollapseEvent, value);
		}
	}

	public event TreeViewCancelEventHandler BeforeExpand
	{
		add
		{
			base.Events.AddHandler(BeforeExpandEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BeforeExpandEvent, value);
		}
	}

	public event NodeLabelEditEventHandler BeforeLabelEdit
	{
		add
		{
			base.Events.AddHandler(BeforeLabelEditEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BeforeLabelEditEvent, value);
		}
	}

	public event TreeViewCancelEventHandler BeforeSelect
	{
		add
		{
			base.Events.AddHandler(BeforeSelectEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BeforeSelectEvent, value);
		}
	}

	public event DrawTreeNodeEventHandler DrawNode
	{
		add
		{
			base.Events.AddHandler(DrawNodeEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DrawNodeEvent, value);
		}
	}

	public event TreeNodeMouseClickEventHandler NodeMouseClick
	{
		add
		{
			base.Events.AddHandler(NodeMouseClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(NodeMouseClickEvent, value);
		}
	}

	public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick
	{
		add
		{
			base.Events.AddHandler(NodeMouseDoubleClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(NodeMouseDoubleClickEvent, value);
		}
	}

	public event TreeNodeMouseHoverEventHandler NodeMouseHover
	{
		add
		{
			base.Events.AddHandler(NodeMouseHoverEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(NodeMouseHoverEvent, value);
		}
	}

	public event EventHandler RightToLeftLayoutChanged
	{
		add
		{
			base.Events.AddHandler(RightToLeftLayoutChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RightToLeftLayoutChangedEvent, value);
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	internal event EventHandler UIACheckBoxesChanged
	{
		add
		{
			base.Events.AddHandler(UIACheckBoxesChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACheckBoxesChangedEvent, value);
		}
	}

	internal event EventHandler UIALabelEditChanged
	{
		add
		{
			base.Events.AddHandler(UIALabelEditChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIALabelEditChangedEvent, value);
		}
	}

	internal event TreeViewEventHandler UIANodeTextChanged
	{
		add
		{
			base.Events.AddHandler(UIANodeTextChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIANodeTextChangedEvent, value);
		}
	}

	internal event CollectionChangeEventHandler UIACollectionChanged
	{
		add
		{
			base.Events.AddHandler(UIACollectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACollectionChangedEvent, value);
		}
	}

	public TreeView()
	{
		vbar = new ImplicitVScrollBar();
		hbar = new ImplicitHScrollBar();
		base.InternalBorderStyle = BorderStyle.Fixed3D;
		background_color = ThemeEngine.Current.ColorWindow;
		foreground_color = ThemeEngine.Current.ColorWindowText;
		draw_mode = TreeViewDrawMode.Normal;
		root_node = new TreeNode(this);
		root_node.Text = "ROOT NODE";
		nodes = new TreeNodeCollection(root_node);
		root_node.SetNodes(nodes);
		base.MouseDown += MouseDownHandler;
		base.MouseUp += MouseUpHandler;
		base.MouseMove += MouseMoveHandler;
		base.SizeChanged += SizeChangedHandler;
		base.FontChanged += FontChangedHandler;
		base.LostFocus += LostFocusHandler;
		base.GotFocus += GotFocusHandler;
		base.MouseWheel += MouseWheelHandler;
		base.VisibleChanged += VisibleChangedHandler;
		SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, value: false);
		string_format = new StringFormat();
		string_format.LineAlignment = StringAlignment.Center;
		string_format.Alignment = StringAlignment.Center;
		vbar.Visible = false;
		hbar.Visible = false;
		vbar.ValueChanged += VScrollBarValueChanged;
		hbar.ValueChanged += HScrollBarValueChanged;
		SuspendLayout();
		base.Controls.AddImplicit(vbar);
		base.Controls.AddImplicit(hbar);
		ResumeLayout();
	}

	static TreeView()
	{
		ItemDrag = new object();
		AfterCheck = new object();
		AfterCollapse = new object();
		AfterExpand = new object();
		AfterLabelEdit = new object();
		AfterSelect = new object();
		BeforeCheck = new object();
		BeforeCollapse = new object();
		BeforeExpand = new object();
		BeforeLabelEdit = new object();
		BeforeSelect = new object();
		DrawNode = new object();
		NodeMouseClick = new object();
		NodeMouseDoubleClick = new object();
		NodeMouseHover = new object();
		RightToLeftLayoutChanged = new object();
		UIACheckBoxesChanged = new object();
		UIALabelEditChanged = new object();
		UIANodeTextChanged = new object();
		UIACollectionChanged = new object();
	}

	private Rectangle Bloat(Rectangle rect)
	{
		rect.Y--;
		rect.X--;
		rect.Height += 2;
		rect.Width += 2;
		return rect;
	}

	public void BeginUpdate()
	{
		update_stack++;
	}

	public void EndUpdate()
	{
		if (update_stack > 1)
		{
			update_stack--;
			return;
		}
		update_stack = 0;
		if (update_needed)
		{
			RecalculateVisibleOrder(root_node);
			UpdateScrollBars(force: false);
			Invalidate(ViewportRectangle);
			update_needed = false;
		}
	}

	public void Sort()
	{
		object sorter;
		if (Nodes.Count >= 2)
		{
			IComparer comparer = tree_view_node_sorter;
			sorter = comparer;
		}
		else
		{
			sorter = null;
		}
		Sort((IComparer)sorter);
	}

	private void Sort(IComparer sorter)
	{
		sorted = true;
		Nodes.Sort(sorter);
		RecalculateVisibleOrder(root_node);
		UpdateScrollBars(force: false);
		Invalidate();
	}

	public void ExpandAll()
	{
		BeginUpdate();
		root_node.ExpandAll();
		EndUpdate();
		if (!base.IsHandleCreated)
		{
			return;
		}
		bool flag = false;
		foreach (TreeNode node in Nodes)
		{
			if (node.Nodes.Count > 0)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		if (base.IsHandleCreated && vbar.VisibleInternal)
		{
			vbar.Value = vbar.Maximum - VisibleCount + 1;
			return;
		}
		RecalculateVisibleOrder(root_node);
		UpdateScrollBars(force: true);
		if (vbar.VisibleInternal)
		{
			SetTop(Nodes[Nodes.Count - 1]);
			SelectedNode = Nodes[Nodes.Count - 1];
		}
	}

	public void CollapseAll()
	{
		BeginUpdate();
		root_node.CollapseAll();
		EndUpdate();
		if (vbar.VisibleInternal)
		{
			vbar.Value = vbar.Maximum - VisibleCount + 1;
		}
	}

	public TreeNode GetNodeAt(Point pt)
	{
		return GetNodeAt(pt.Y);
	}

	public TreeNode GetNodeAt(int x, int y)
	{
		return GetNodeAt(y);
	}

	private TreeNode GetNodeAtUseX(int x, int y)
	{
		TreeNode nodeAt = GetNodeAt(y);
		if (nodeAt == null || (!IsTextArea(nodeAt, x) && !full_row_select))
		{
			return null;
		}
		return nodeAt;
	}

	public int GetNodeCount(bool includeSubTrees)
	{
		return root_node.GetNodeCount(includeSubTrees);
	}

	public TreeViewHitTestInfo HitTest(Point pt)
	{
		return HitTest(pt.X, pt.Y);
	}

	public TreeViewHitTestInfo HitTest(int x, int y)
	{
		TreeNode nodeAt = GetNodeAt(y);
		if (nodeAt == null)
		{
			return new TreeViewHitTestInfo(null, TreeViewHitTestLocations.None);
		}
		if (IsTextArea(nodeAt, x))
		{
			return new TreeViewHitTestInfo(nodeAt, TreeViewHitTestLocations.Label);
		}
		if (IsPlusMinusArea(nodeAt, x))
		{
			return new TreeViewHitTestInfo(nodeAt, TreeViewHitTestLocations.PlusMinus);
		}
		if ((checkboxes || nodeAt.StateImage != null) && IsCheckboxArea(nodeAt, x))
		{
			return new TreeViewHitTestInfo(nodeAt, TreeViewHitTestLocations.StateImage);
		}
		if (x > nodeAt.Bounds.Right)
		{
			return new TreeViewHitTestInfo(nodeAt, TreeViewHitTestLocations.RightOfLabel);
		}
		if (IsImage(nodeAt, x))
		{
			return new TreeViewHitTestInfo(nodeAt, TreeViewHitTestLocations.Image);
		}
		return new TreeViewHitTestInfo(null, TreeViewHitTestLocations.Indent);
	}

	public override string ToString()
	{
		int count = Nodes.Count;
		if (count <= 0)
		{
			return base.ToString() + ", Nodes.Count: 0";
		}
		return base.ToString() + ", Nodes.Count: " + count + ", Nodes[0]: " + Nodes[0];
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
		RecalculateVisibleOrder(root_node);
		UpdateScrollBars(force: false);
		if (pre_selected_node != null)
		{
			SelectedNode = pre_selected_node;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			image_list = null;
		}
		base.Dispose(disposing);
	}

	protected OwnerDrawPropertyBag GetItemRenderStyles(TreeNode node, int state)
	{
		return node.prop_bag;
	}

	protected override bool IsInputKey(Keys keyData)
	{
		if (base.IsHandleCreated && (keyData & Keys.Alt) == 0)
		{
			switch (keyData & Keys.KeyCode)
			{
			case Keys.Left:
			case Keys.Up:
			case Keys.Right:
			case Keys.Down:
				return true;
			case Keys.Return:
			case Keys.Escape:
			case Keys.PageUp:
			case Keys.PageDown:
			case Keys.End:
			case Keys.Home:
				if (edit_node != null)
				{
					return true;
				}
				break;
			}
		}
		return base.IsInputKey(keyData);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.KeyData & Keys.KeyCode)
		{
		case Keys.Add:
			if (selected_node != null && selected_node.IsExpanded)
			{
				selected_node.Expand();
			}
			break;
		case Keys.Subtract:
			if (selected_node != null && selected_node.IsExpanded)
			{
				selected_node.Collapse();
			}
			break;
		case Keys.Left:
		{
			if (selected_node == null)
			{
				break;
			}
			if (selected_node.IsExpanded && selected_node.Nodes.Count > 0)
			{
				selected_node.Collapse();
				break;
			}
			TreeNode treeNode = selected_node.Parent;
			if (treeNode != null)
			{
				selection_action = TreeViewAction.ByKeyboard;
				SelectedNode = treeNode;
			}
			break;
		}
		case Keys.Right:
		{
			if (selected_node == null)
			{
				break;
			}
			if (!selected_node.IsExpanded)
			{
				selected_node.Expand();
				break;
			}
			TreeNode firstNode = selected_node.FirstNode;
			if (firstNode != null)
			{
				SelectedNode = firstNode;
			}
			break;
		}
		case Keys.Up:
			if (selected_node != null)
			{
				OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(selected_node);
				if (openTreeNodeEnumerator.MovePrevious() && openTreeNodeEnumerator.MovePrevious())
				{
					selection_action = TreeViewAction.ByKeyboard;
					SelectedNode = openTreeNodeEnumerator.CurrentNode;
				}
			}
			break;
		case Keys.Down:
			if (selected_node != null)
			{
				OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(selected_node);
				if (openTreeNodeEnumerator.MoveNext() && openTreeNodeEnumerator.MoveNext())
				{
					selection_action = TreeViewAction.ByKeyboard;
					SelectedNode = openTreeNodeEnumerator.CurrentNode;
				}
			}
			break;
		case Keys.Home:
			if (root_node.Nodes.Count > 0)
			{
				OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(root_node.Nodes[0]);
				if (openTreeNodeEnumerator.MoveNext())
				{
					selection_action = TreeViewAction.ByKeyboard;
					SelectedNode = openTreeNodeEnumerator.CurrentNode;
				}
			}
			break;
		case Keys.End:
			if (root_node.Nodes.Count > 0)
			{
				OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(root_node.Nodes[0]);
				while (openTreeNodeEnumerator.MoveNext())
				{
				}
				selection_action = TreeViewAction.ByKeyboard;
				SelectedNode = openTreeNodeEnumerator.CurrentNode;
			}
			break;
		case Keys.PageDown:
		{
			if (selected_node == null)
			{
				break;
			}
			OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(selected_node);
			int visibleCount2 = VisibleCount;
			for (int j = 0; j < visibleCount2; j++)
			{
				if (!openTreeNodeEnumerator.MoveNext())
				{
					break;
				}
			}
			selection_action = TreeViewAction.ByKeyboard;
			SelectedNode = openTreeNodeEnumerator.CurrentNode;
			break;
		}
		case Keys.PageUp:
		{
			if (selected_node == null)
			{
				break;
			}
			OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(selected_node);
			int visibleCount = VisibleCount;
			for (int i = 0; i < visibleCount; i++)
			{
				if (!openTreeNodeEnumerator.MovePrevious())
				{
					break;
				}
			}
			selection_action = TreeViewAction.ByKeyboard;
			SelectedNode = openTreeNodeEnumerator.CurrentNode;
			break;
		}
		case Keys.Multiply:
			if (selected_node != null)
			{
				selected_node.ExpandAll();
			}
			break;
		}
		base.OnKeyDown(e);
		if (!e.Handled && checkboxes && selected_node != null && (e.KeyData & Keys.KeyCode) == Keys.Space)
		{
			selected_node.check_reason = TreeViewAction.ByKeyboard;
			selected_node.Checked = !selected_node.Checked;
			e.Handled = true;
		}
	}

	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		base.OnKeyPress(e);
		if (e.KeyChar == ' ')
		{
			e.Handled = true;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
		if ((e.KeyData & Keys.KeyCode) == Keys.Space)
		{
			e.Handled = true;
		}
	}

	protected override void OnMouseHover(EventArgs e)
	{
		base.OnMouseHover(e);
		is_hovering = true;
		TreeNode nodeAt = GetNodeAt(PointToClient(Control.MousePosition));
		if (nodeAt != null)
		{
			MouseEnteredItem(nodeAt);
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		is_hovering = false;
		if (tooltip_currently_showing != null)
		{
			MouseLeftItem(tooltip_currently_showing);
		}
	}

	protected virtual void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
	{
		((TreeNodeMouseClickEventHandler)base.Events[NodeMouseClick])?.Invoke(this, e);
	}

	protected virtual void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
	{
		((TreeNodeMouseClickEventHandler)base.Events[NodeMouseDoubleClick])?.Invoke(this, e);
	}

	protected virtual void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e)
	{
		((TreeNodeMouseHoverEventHandler)base.Events[NodeMouseHover])?.Invoke(this, e);
	}

	protected virtual void OnItemDrag(ItemDragEventArgs e)
	{
		((ItemDragEventHandler)base.Events[ItemDrag])?.Invoke(this, e);
	}

	protected virtual void OnDrawNode(DrawTreeNodeEventArgs e)
	{
		((DrawTreeNodeEventHandler)base.Events[DrawNode])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
	{
		((EventHandler)base.Events[RightToLeftLayoutChanged])?.Invoke(this, e);
	}

	protected internal virtual void OnAfterCheck(TreeViewEventArgs e)
	{
		((TreeViewEventHandler)base.Events[AfterCheck])?.Invoke(this, e);
	}

	protected internal virtual void OnAfterCollapse(TreeViewEventArgs e)
	{
		((TreeViewEventHandler)base.Events[AfterCollapse])?.Invoke(this, e);
	}

	protected internal virtual void OnAfterExpand(TreeViewEventArgs e)
	{
		((TreeViewEventHandler)base.Events[AfterExpand])?.Invoke(this, e);
	}

	protected virtual void OnAfterLabelEdit(NodeLabelEditEventArgs e)
	{
		((NodeLabelEditEventHandler)base.Events[AfterLabelEdit])?.Invoke(this, e);
	}

	protected virtual void OnAfterSelect(TreeViewEventArgs e)
	{
		((TreeViewEventHandler)base.Events[AfterSelect])?.Invoke(this, e);
	}

	protected internal virtual void OnBeforeCheck(TreeViewCancelEventArgs e)
	{
		((TreeViewCancelEventHandler)base.Events[BeforeCheck])?.Invoke(this, e);
	}

	protected internal virtual void OnBeforeCollapse(TreeViewCancelEventArgs e)
	{
		((TreeViewCancelEventHandler)base.Events[BeforeCollapse])?.Invoke(this, e);
	}

	protected internal virtual void OnBeforeExpand(TreeViewCancelEventArgs e)
	{
		((TreeViewCancelEventHandler)base.Events[BeforeExpand])?.Invoke(this, e);
	}

	protected virtual void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
	{
		((NodeLabelEditEventHandler)base.Events[BeforeLabelEdit])?.Invoke(this, e);
	}

	protected virtual void OnBeforeSelect(TreeViewCancelEventArgs e)
	{
		((TreeViewCancelEventHandler)base.Events[BeforeSelect])?.Invoke(this, e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_LBUTTONDBLCLK:
		{
			int num = m.LParam.ToInt32();
			DoubleClickHandler(null, new MouseEventArgs(MouseButtons.Left, 2, num & 0xFFFF, (num >> 16) & 0xFFFF, 0));
			break;
		}
		case Msg.WM_CONTEXTMENU:
			if (WmContextMenu(ref m))
			{
				return;
			}
			break;
		}
		base.WndProc(ref m);
	}

	internal IntPtr CreateNodeHandle()
	{
		return (IntPtr)(handle_count++);
	}

	internal override void HandleClick(int clicks, MouseEventArgs me)
	{
		if (GetNodeAt(me.Location) != null)
		{
			if (clicks > 1 && GetStyle(ControlStyles.StandardDoubleClick))
			{
				OnDoubleClick(me);
				OnMouseDoubleClick(me);
			}
			else
			{
				OnClick(me);
				OnMouseClick(me);
			}
		}
	}

	internal override bool IsInputCharInternal(char charCode)
	{
		return true;
	}

	internal TreeNode NodeFromHandle(IntPtr handle)
	{
		return NodeFromHandleRecursive(root_node, handle);
	}

	private TreeNode NodeFromHandleRecursive(TreeNode node, IntPtr handle)
	{
		if (node.handle == handle)
		{
			return node;
		}
		foreach (TreeNode node2 in node.Nodes)
		{
			TreeNode treeNode = NodeFromHandleRecursive(node2, handle);
			if (treeNode != null)
			{
				return treeNode;
			}
		}
		return null;
	}

	private TreeNode GetNodeAt(int y)
	{
		if (nodes.Count <= 0)
		{
			return null;
		}
		OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(TopNode);
		int num = y / ActualItemHeight;
		for (int i = -1; i < num; i++)
		{
			if (!openTreeNodeEnumerator.MoveNext())
			{
				return null;
			}
		}
		return openTreeNodeEnumerator.CurrentNode;
	}

	private bool IsTextArea(TreeNode node, int x)
	{
		return node != null && node.Bounds.Left <= x && node.Bounds.Right >= x;
	}

	private bool IsSelectableArea(TreeNode node, int x)
	{
		if (node == null)
		{
			return false;
		}
		int num = node.Bounds.Left;
		if (ImageList != null)
		{
			num -= ImageList.ImageSize.Width;
		}
		return num <= x && node.Bounds.Right >= x;
	}

	private bool IsPlusMinusArea(TreeNode node, int x)
	{
		if (node.Nodes.Count == 0 || (node.parent == root_node && !show_root_lines))
		{
			return false;
		}
		int num = node.Bounds.Left + 5;
		if (show_root_lines || node.Parent != null)
		{
			num -= indent;
		}
		if (ImageList != null)
		{
			num -= ImageList.ImageSize.Width + 3;
		}
		if (checkboxes)
		{
			num -= 19;
		}
		else if (node.StateImage != null)
		{
			num -= 19;
		}
		return x > num && x < num + 8;
	}

	private bool IsCheckboxArea(TreeNode node, int x)
	{
		int num = CheckBoxLeft(node);
		return x > num && x < num + 10;
	}

	private bool IsImage(TreeNode node, int x)
	{
		if (ImageList == null)
		{
			return false;
		}
		int left = node.Bounds.Left;
		left -= ImageList.ImageSize.Width + 5;
		if (x >= left && x <= left + ImageList.ImageSize.Width + 5)
		{
			return true;
		}
		return false;
	}

	private int CheckBoxLeft(TreeNode node)
	{
		int num = node.Bounds.Left + 5;
		if (show_root_lines || node.Parent != null)
		{
			num -= indent;
		}
		if (!show_root_lines && node.Parent == null)
		{
			num -= indent;
		}
		if (ImageList != null)
		{
			num -= ImageList.ImageSize.Width + 3;
		}
		return num;
	}

	internal void RecalculateVisibleOrder(TreeNode start)
	{
		if (update_stack <= 0)
		{
			int num;
			if (start == null)
			{
				start = root_node;
				num = 0;
			}
			else
			{
				num = start.visible_order;
			}
			OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(start);
			while (openTreeNodeEnumerator.MoveNext())
			{
				openTreeNodeEnumerator.CurrentNode.visible_order = num;
				num++;
			}
			max_visible_order = num;
		}
	}

	internal void SetTop(TreeNode node)
	{
		int val = 0;
		if (node != null)
		{
			val = Math.Max(0, node.visible_order - 1);
		}
		if (!vbar.is_visible)
		{
			skipped_nodes = val;
		}
		else
		{
			vbar.Value = Math.Min(val, vbar.Maximum - VisibleCount + 1);
		}
	}

	internal void SetBottom(TreeNode node)
	{
		if (vbar.is_visible)
		{
			OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(node);
			int bottom = ViewportRectangle.Bottom;
			int num = 0;
			while (openTreeNodeEnumerator.MovePrevious() && openTreeNodeEnumerator.CurrentNode.Bounds.Bottom > bottom)
			{
				num++;
			}
			int value = vbar.Value + num;
			if (vbar.Value + num < vbar.Maximum)
			{
				vbar.Value = value;
			}
		}
	}

	internal void UpdateBelow(TreeNode node)
	{
		if (update_stack > 0)
		{
			update_needed = true;
			return;
		}
		if (node == root_node)
		{
			Invalidate(ViewportRectangle);
			return;
		}
		int num = Math.Max(node.Bounds.Top - 1, 0);
		Rectangle rc = new Rectangle(0, num, base.Width, base.Height - num);
		Invalidate(rc);
	}

	internal void UpdateNode(TreeNode node)
	{
		if (node != null)
		{
			if (update_stack > 0)
			{
				update_needed = true;
				return;
			}
			if (node == root_node)
			{
				Invalidate();
				return;
			}
			Rectangle rc = new Rectangle(0, node.Bounds.Top - 1, base.Width, node.Bounds.Height + 1);
			Invalidate(rc);
		}
	}

	internal void UpdateNodePlusMinus(TreeNode node)
	{
		if (update_stack > 0)
		{
			update_needed = true;
			return;
		}
		int num = node.Bounds.Left + 5;
		if (show_root_lines || node.Parent != null)
		{
			num -= indent;
		}
		if (ImageList != null)
		{
			num -= ImageList.ImageSize.Width + 3;
		}
		if (checkboxes)
		{
			num -= 19;
		}
		Invalidate(new Rectangle(num, node.Bounds.Top, 8, node.Bounds.Height));
	}

	internal override void OnPaintInternal(PaintEventArgs pe)
	{
		Draw(pe.ClipRectangle, pe.Graphics);
	}

	internal void CreateDashPen()
	{
		dash = new Pen(LineColor, 1f);
		dash.DashStyle = DashStyle.Dot;
	}

	private void Draw(Rectangle clip, Graphics dc)
	{
		dc.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), clip);
		if (dash == null)
		{
			CreateDashPen();
		}
		Rectangle viewportRectangle = ViewportRectangle;
		Rectangle rectangle = clip;
		if (clip.Bottom > viewportRectangle.Bottom)
		{
			clip.Height = viewportRectangle.Bottom - clip.Top;
		}
		OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(TopNode);
		while (openTreeNodeEnumerator.MoveNext())
		{
			TreeNode currentNode = openTreeNodeEnumerator.CurrentNode;
			if (currentNode.GetY() + ActualItemHeight >= clip.Top)
			{
				if (currentNode.GetY() > clip.Bottom)
				{
					break;
				}
				DrawTreeNode(currentNode, dc, clip);
			}
		}
		if (hbar.Visible && vbar.Visible)
		{
			Rectangle rect = new Rectangle(hbar.Right, vbar.Bottom, vbar.Width, hbar.Height);
			if (rectangle.IntersectsWith(rect))
			{
				dc.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ThemeEngine.Current.ColorControl), rect);
			}
		}
	}

	private void DrawNodeState(TreeNode node, Graphics dc, int x, int y)
	{
		if (node.Checked)
		{
			if (StateImageList.Images[1] != null)
			{
				dc.DrawImage(StateImageList.Images[1], new Rectangle(x, y, 16, 16));
			}
		}
		else if (StateImageList.Images[0] != null)
		{
			dc.DrawImage(StateImageList.Images[0], new Rectangle(x, y, 16, 16));
		}
	}

	private void DrawNodeCheckBox(TreeNode node, Graphics dc, int x, int middle)
	{
		Pen sizedPen = ThemeEngine.Current.ResPool.GetSizedPen(Color.Black, 2);
		dc.DrawRectangle(sizedPen, x + 3, middle - 4, 11, 11);
		if (node.Checked)
		{
			Pen pen = ThemeEngine.Current.ResPool.GetPen(Color.Black);
			int num = 5;
			int num2 = 3;
			Rectangle rectangle = new Rectangle(x + 4, middle - 3, num, num);
			for (int i = 0; i < num2; i++)
			{
				dc.DrawLine(pen, rectangle.Left + 1, rectangle.Top + num2 + i, rectangle.Left + 3, rectangle.Top + 5 + i);
				dc.DrawLine(pen, rectangle.Left + 3, rectangle.Top + 5 + i, rectangle.Left + 7, rectangle.Top + 1 + i);
			}
		}
	}

	private void DrawNodeLines(TreeNode node, Graphics dc, Rectangle clip, Pen dash, int x, int y, int middle)
	{
		int num = 9;
		int num2 = 0;
		if (node.nodes.Count > 0 && show_plus_minus)
		{
			num = 13;
		}
		if (checkboxes)
		{
			num2 = 3;
		}
		if (show_root_lines || node.Parent != null)
		{
			dc.DrawLine(dash, x - indent + num, middle, x + num2, middle);
		}
		if (node.PrevNode != null || node.Parent != null)
		{
			num = 9;
			dc.DrawLine(dash, x - indent + num, node.Bounds.Top, x - indent + num, middle - ((show_plus_minus && node.Nodes.Count > 0) ? 4 : 0));
		}
		if (node.NextNode != null)
		{
			num = 9;
			dc.DrawLine(dash, x - indent + num, middle + ((show_plus_minus && node.Nodes.Count > 0) ? 4 : 0), x - indent + num, node.Bounds.Bottom);
		}
		num = 0;
		if (show_plus_minus)
		{
			num = 9;
		}
		for (TreeNode treeNode = node.Parent; treeNode != null; treeNode = treeNode.Parent)
		{
			if (treeNode.NextNode != null)
			{
				int num3 = treeNode.GetLinesX() - indent + num;
				dc.DrawLine(dash, num3, node.Bounds.Top, num3, node.Bounds.Bottom);
			}
		}
	}

	private void DrawNodeImage(TreeNode node, Graphics dc, Rectangle clip, int x, int y)
	{
		if (RectsIntersect(clip, x, y, ImageList.ImageSize.Width, ImageList.ImageSize.Height))
		{
			int image = node.Image;
			if (image > -1 && image < ImageList.Images.Count)
			{
				ImageList.Draw(dc, x, y, ImageList.ImageSize.Width, ImageList.ImageSize.Height, image);
			}
		}
	}

	private void LabelEditFinished(object sender, EventArgs e)
	{
		EndEdit(edit_node);
	}

	internal void BeginEdit(TreeNode node)
	{
		if (edit_node != null)
		{
			EndEdit(edit_node);
		}
		if (edit_text_box == null)
		{
			edit_text_box = new LabelEditTextBox();
			edit_text_box.BorderStyle = BorderStyle.FixedSingle;
			edit_text_box.Visible = false;
			edit_text_box.EditingCancelled += LabelEditCancelled;
			edit_text_box.EditingFinished += LabelEditFinished;
			edit_text_box.TextChanged += LabelTextChanged;
			base.Controls.Add(edit_text_box);
		}
		node.EnsureVisible();
		edit_text_box.Bounds = node.Bounds;
		edit_text_box.Text = node.Text;
		edit_text_box.Visible = true;
		edit_text_box.Focus();
		edit_text_box.SelectAll();
		edit_args = new NodeLabelEditEventArgs(node);
		OnBeforeLabelEdit(edit_args);
		edit_node = node;
		if (edit_args.CancelEdit)
		{
			edit_node = null;
			EndEdit(node);
		}
	}

	private void LabelEditCancelled(object sender, EventArgs e)
	{
		edit_args.SetLabel(null);
		EndEdit(edit_node);
	}

	private void LabelTextChanged(object sender, EventArgs e)
	{
		int width = TextRenderer.MeasureTextInternal(edit_text_box.Text, edit_text_box.Font, useMeasureString: false).Width + 4;
		edit_text_box.Width = width;
		if (edit_args != null)
		{
			edit_args.SetLabel(edit_text_box.Text);
		}
	}

	internal void EndEdit(TreeNode node)
	{
		if (edit_text_box != null && edit_text_box.Visible)
		{
			edit_text_box.Visible = false;
			Focus();
		}
		Application.DoEvents();
		if (edit_node != null && edit_node == node)
		{
			edit_node = null;
			NodeLabelEditEventArgs nodeLabelEditEventArgs = new NodeLabelEditEventArgs(edit_args.Node, edit_args.Label);
			OnAfterLabelEdit(nodeLabelEditEventArgs);
			if (nodeLabelEditEventArgs.CancelEdit)
			{
				return;
			}
			if (nodeLabelEditEventArgs.Label != null)
			{
				nodeLabelEditEventArgs.Node.Text = nodeLabelEditEventArgs.Label;
			}
		}
		edit_node = null;
		UpdateNode(node);
	}

	internal void CancelEdit(TreeNode node)
	{
		edit_args.SetLabel(null);
		if (edit_text_box != null && edit_text_box.Visible)
		{
			edit_text_box.Visible = false;
			Focus();
		}
		edit_node = null;
		UpdateNode(node);
	}

	internal int GetNodeWidth(TreeNode node)
	{
		Font nodeFont = node.NodeFont;
		if (node.NodeFont == null)
		{
			nodeFont = Font;
		}
		return (int)TextRenderer.MeasureString(node.Text, nodeFont, 0, string_format).Width + 3;
	}

	private void DrawSelectionAndFocus(TreeNode node, Graphics dc, Rectangle r)
	{
		if (Focused && focused_node == node && !full_row_select)
		{
			ControlPaint.DrawFocusRectangle(dc, r, ForeColor, BackColor);
		}
		if (draw_mode == TreeViewDrawMode.Normal)
		{
			r.Inflate(-1, -1);
			if (Focused && node == highlighted_node)
			{
				Color color = ((node == selected_node || !(node.BackColor != Color.Empty)) ? ThemeEngine.Current.ColorHighlight : node.BackColor);
				dc.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(color), r);
			}
			else if (!hide_selection && node == highlighted_node)
			{
				dc.FillRectangle(SystemBrushes.Control, r);
			}
			else
			{
				Color color2 = ((node != selected_node) ? node.BackColor : BackColor);
				dc.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(color2), r);
			}
		}
	}

	private void DrawStaticNode(TreeNode node, Graphics dc)
	{
		if (!full_row_select || show_lines)
		{
			DrawSelectionAndFocus(node, dc, node.Bounds);
		}
		Font nodeFont = node.NodeFont;
		if (node.NodeFont == null)
		{
			nodeFont = Font;
		}
		Color color = ((!Focused || node != highlighted_node) ? node.ForeColor : ThemeEngine.Current.ColorHighlightText);
		if (color.IsEmpty)
		{
			color = ForeColor;
		}
		dc.DrawString(node.Text, nodeFont, ThemeEngine.Current.ResPool.GetSolidBrush(color), node.Bounds, string_format);
	}

	private void DrawTreeNode(TreeNode node, Graphics dc, Rectangle clip)
	{
		int count = node.nodes.Count;
		int y = node.GetY();
		int middle = y + ActualItemHeight / 2;
		if (full_row_select && !show_lines)
		{
			Rectangle r = new Rectangle(1, y, ViewportRectangle.Width - 2, ActualItemHeight);
			DrawSelectionAndFocus(node, dc, r);
		}
		if (draw_mode == TreeViewDrawMode.Normal || draw_mode == TreeViewDrawMode.OwnerDrawText)
		{
			if ((show_root_lines || node.Parent != null) && show_plus_minus && count > 0)
			{
				ThemeEngine.Current.TreeViewDrawNodePlusMinus(this, node, dc, node.GetLinesX() - Indent + 5, middle);
			}
			if (checkboxes && state_image_list == null)
			{
				DrawNodeCheckBox(node, dc, CheckBoxLeft(node) - 3, middle);
			}
			if (checkboxes && state_image_list != null)
			{
				DrawNodeState(node, dc, CheckBoxLeft(node) - 3, y);
			}
			if (!checkboxes && node.StateImage != null)
			{
				dc.DrawImage(node.StateImage, new Rectangle(CheckBoxLeft(node) - 3, y, 16, 16));
			}
			if (show_lines)
			{
				DrawNodeLines(node, dc, clip, dash, node.GetLinesX(), y, middle);
			}
			if (ImageList != null)
			{
				DrawNodeImage(node, dc, clip, node.GetImageX(), y);
			}
		}
		if (draw_mode != 0)
		{
			dc.FillRectangle(Brushes.White, node.Bounds);
			TreeNodeStates treeNodeStates = TreeNodeStates.Default;
			if (node.IsSelected)
			{
				treeNodeStates = TreeNodeStates.Selected;
			}
			if (node.Checked)
			{
				treeNodeStates |= TreeNodeStates.Checked;
			}
			if (node == focused_node)
			{
				treeNodeStates |= TreeNodeStates.Focused;
			}
			Rectangle rectangle = node.Bounds;
			if (draw_mode == TreeViewDrawMode.OwnerDrawText)
			{
				rectangle.X += 3;
				rectangle.Y++;
			}
			else
			{
				rectangle.X = 0;
				rectangle.Width = base.Width;
			}
			DrawTreeNodeEventArgs drawTreeNodeEventArgs = new DrawTreeNodeEventArgs(dc, node, rectangle, treeNodeStates);
			OnDrawNode(drawTreeNodeEventArgs);
			if (!drawTreeNodeEventArgs.DrawDefault)
			{
				return;
			}
		}
		if (!node.IsEditing)
		{
			DrawStaticNode(node, dc);
		}
	}

	internal void UpdateScrollBars(bool force)
	{
		if (!force && (base.IsDisposed || update_stack > 0 || !base.IsHandleCreated || !base.Visible))
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		int num2 = -1;
		int actualItemHeight = ActualItemHeight;
		if (scrollable)
		{
			OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(root_node);
			while (openTreeNodeEnumerator.MoveNext())
			{
				int right = openTreeNodeEnumerator.CurrentNode.Bounds.Right;
				if (right > num2)
				{
					num2 = right;
				}
				num += actualItemHeight;
			}
			num -= actualItemHeight;
			num2 += hbar_offset;
			if (num > base.ClientRectangle.Height)
			{
				flag = true;
				if (num2 > base.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth)
				{
					flag2 = true;
				}
			}
			else if (num2 > base.ClientRectangle.Width)
			{
				flag2 = true;
			}
			if (!flag && flag2 && num > base.ClientRectangle.Height - SystemInformation.HorizontalScrollBarHeight)
			{
				flag = true;
			}
		}
		if (flag)
		{
			int num3 = ((!flag2) ? base.ClientRectangle.Height : (base.ClientRectangle.Height - hbar.Height));
			vbar.SetValues(Math.Max(0, max_visible_order - 2), num3 / ActualItemHeight);
			if (!vbar_bounds_set)
			{
				vbar.Bounds = new Rectangle(base.ClientRectangle.Width - vbar.Width, 0, vbar.Width, base.ClientRectangle.Height - (flag2 ? SystemInformation.VerticalScrollBarWidth : 0));
				vbar_bounds_set = true;
				hbar_bounds_set = false;
			}
			vbar.Visible = true;
			if (skipped_nodes > 0)
			{
				int value = Math.Min(skipped_nodes, vbar.Maximum - VisibleCount + 1);
				skipped_nodes = 0;
				vbar.SafeValueSet(value);
				skipped_nodes = value;
			}
		}
		else
		{
			skipped_nodes = 0;
			RecalculateVisibleOrder(root_node);
			vbar.Visible = false;
			vbar.Value = 0;
			vbar_bounds_set = false;
		}
		if (flag2)
		{
			hbar.SetValues(num2 + 1, base.ClientRectangle.Width - (flag ? SystemInformation.VerticalScrollBarWidth : 0));
			if (!hbar_bounds_set)
			{
				hbar.Bounds = new Rectangle(0, base.ClientRectangle.Height - hbar.Height, base.ClientRectangle.Width - (flag ? SystemInformation.VerticalScrollBarWidth : 0), hbar.Height);
				hbar_bounds_set = true;
			}
			hbar.Visible = true;
		}
		else
		{
			hbar_offset = 0;
			hbar.Visible = false;
			hbar_bounds_set = false;
		}
	}

	private void SizeChangedHandler(object sender, EventArgs e)
	{
		if (base.IsHandleCreated)
		{
			if (max_visible_order == -1)
			{
				RecalculateVisibleOrder(root_node);
			}
			UpdateScrollBars(force: false);
		}
		if (vbar.Visible)
		{
			vbar.Bounds = new Rectangle(base.ClientRectangle.Width - vbar.Width, 0, vbar.Width, base.ClientRectangle.Height - (hbar.Visible ? SystemInformation.HorizontalScrollBarHeight : 0));
		}
		if (hbar.Visible)
		{
			hbar.Bounds = new Rectangle(0, base.ClientRectangle.Height - hbar.Height, base.ClientRectangle.Width - (vbar.Visible ? SystemInformation.VerticalScrollBarWidth : 0), hbar.Height);
		}
	}

	private void VScrollBarValueChanged(object sender, EventArgs e)
	{
		EndEdit(edit_node);
		SetVScrollPos(vbar.Value, null);
	}

	private void SetVScrollPos(int pos, TreeNode new_top)
	{
		if (!vbar.VisibleInternal)
		{
			return;
		}
		if (pos < 0)
		{
			pos = 0;
		}
		if (skipped_nodes != pos)
		{
			int num = skipped_nodes - pos;
			skipped_nodes = pos;
			if (base.IsHandleCreated)
			{
				int yAmount = num * ActualItemHeight;
				XplatUI.ScrollWindow(Handle, ViewportRectangle, 0, yAmount, with_children: false);
			}
		}
	}

	private void HScrollBarValueChanged(object sender, EventArgs e)
	{
		EndEdit(edit_node);
		int num = hbar_offset;
		hbar_offset = hbar.Value;
		if (hbar_offset < 0)
		{
			hbar_offset = 0;
		}
		XplatUI.ScrollWindow(Handle, ViewportRectangle, num - hbar_offset, 0, with_children: false);
	}

	internal void ExpandBelow(TreeNode node, int count_to_next)
	{
		if (update_stack > 0)
		{
			update_needed = true;
			return;
		}
		int num = ((node.Bounds.Bottom >= 0) ? node.Bounds.Bottom : 0);
		Rectangle rectangle = new Rectangle(0, num, ViewportRectangle.Width, ViewportRectangle.Height - num);
		int num2 = count_to_next * ActualItemHeight;
		if (num2 > 0)
		{
			XplatUI.ScrollWindow(Handle, rectangle, 0, num2, with_children: false);
		}
		if (show_plus_minus)
		{
			Invalidate(new Rectangle(0, node.GetY(), base.Width, ActualItemHeight));
		}
	}

	internal void CollapseBelow(TreeNode node, int count_to_next)
	{
		if (update_stack > 0)
		{
			update_needed = true;
			return;
		}
		Rectangle rectangle = new Rectangle(0, node.Bounds.Bottom, ViewportRectangle.Width, ViewportRectangle.Height - node.Bounds.Bottom);
		int num = count_to_next * ActualItemHeight;
		if (num > 0)
		{
			XplatUI.ScrollWindow(Handle, rectangle, 0, -num, with_children: false);
		}
		if (show_plus_minus)
		{
			Invalidate(new Rectangle(0, node.GetY(), base.Width, ActualItemHeight));
		}
	}

	private void MouseWheelHandler(object sender, MouseEventArgs e)
	{
		if (vbar != null && vbar.is_visible)
		{
			if (e.Delta < 0)
			{
				vbar.Value = Math.Min(vbar.Value + SystemInformation.MouseWheelScrollLines, vbar.Maximum - VisibleCount + 1);
			}
			else
			{
				vbar.Value = Math.Max(0, vbar.Value - SystemInformation.MouseWheelScrollLines);
			}
		}
	}

	private void VisibleChangedHandler(object sender, EventArgs e)
	{
		if (base.Visible)
		{
			UpdateScrollBars(force: false);
		}
	}

	private void FontChangedHandler(object sender, EventArgs e)
	{
		if (base.IsHandleCreated)
		{
			TreeNode topNode = TopNode;
			InvalidateNodeWidthRecursive(root_node);
			SetTop(topNode);
		}
	}

	private void InvalidateNodeWidthRecursive(TreeNode node)
	{
		node.InvalidateWidth();
		foreach (TreeNode node2 in node.Nodes)
		{
			InvalidateNodeWidthRecursive(node2);
		}
	}

	private void GotFocusHandler(object sender, EventArgs e)
	{
		if (selected_node == null)
		{
			if (pre_selected_node != null)
			{
				SelectedNode = pre_selected_node;
			}
			else
			{
				SelectedNode = TopNode;
			}
		}
		else if (selected_node != null)
		{
			UpdateNode(selected_node);
		}
	}

	private void LostFocusHandler(object sender, EventArgs e)
	{
		UpdateNode(SelectedNode);
	}

	private void MouseDownHandler(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			Focus();
		}
		TreeNode nodeAt = GetNodeAt(e.Y);
		if (nodeAt == null)
		{
			return;
		}
		mouse_click_node = nodeAt;
		if (show_plus_minus && IsPlusMinusArea(nodeAt, e.X) && e.Button == MouseButtons.Left)
		{
			nodeAt.Toggle();
		}
		else if (checkboxes && IsCheckboxArea(nodeAt, e.X) && e.Button == MouseButtons.Left)
		{
			nodeAt.check_reason = TreeViewAction.ByMouse;
			nodeAt.Checked = !nodeAt.Checked;
			UpdateNode(nodeAt);
		}
		else if (IsSelectableArea(nodeAt, e.X) || full_row_select)
		{
			TreeNode treeNode = highlighted_node;
			highlighted_node = nodeAt;
			if (label_edit && e.Clicks == 1 && highlighted_node == treeNode && e.Button == MouseButtons.Left)
			{
				BeginEdit(nodeAt);
			}
			else if (highlighted_node != focused_node)
			{
				Size dragSize = SystemInformation.DragSize;
				mouse_rect.X = e.X - dragSize.Width;
				mouse_rect.Y = e.Y - dragSize.Height;
				mouse_rect.Width = dragSize.Width * 2;
				mouse_rect.Height = dragSize.Height * 2;
				select_mmove = true;
			}
			Invalidate(highlighted_node.Bounds);
			if (treeNode != null)
			{
				Invalidate(Bloat(treeNode.Bounds));
			}
		}
	}

	private void MouseUpHandler(object sender, MouseEventArgs e)
	{
		TreeNode nodeAt = GetNodeAt(e.Y);
		if (nodeAt != null && nodeAt == mouse_click_node)
		{
			if (e.Clicks == 2)
			{
				OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(nodeAt, e.Button, e.Clicks, e.X, e.Y));
			}
			else
			{
				OnNodeMouseClick(new TreeNodeMouseClickEventArgs(nodeAt, e.Button, e.Clicks, e.X, e.Y));
			}
		}
		mouse_click_node = null;
		drag_begin_x = -1;
		drag_begin_y = -1;
		if (!select_mmove)
		{
			return;
		}
		select_mmove = false;
		if (e.Button == MouseButtons.Right && selected_node != null)
		{
			Invalidate(highlighted_node.Bounds);
			highlighted_node = selected_node;
			Invalidate(selected_node.Bounds);
			return;
		}
		TreeViewCancelEventArgs treeViewCancelEventArgs = new TreeViewCancelEventArgs(highlighted_node, cancel: false, TreeViewAction.ByMouse);
		OnBeforeSelect(treeViewCancelEventArgs);
		if (!treeViewCancelEventArgs.Cancel)
		{
			TreeNode treeNode = focused_node;
			TreeNode treeNode2 = highlighted_node;
			selected_node = highlighted_node;
			focused_node = highlighted_node;
			OnAfterSelect(new TreeViewEventArgs(selected_node, TreeViewAction.ByMouse));
			if (treeNode2 != null)
			{
				Rectangle rc = ((treeNode == null) ? Bloat(treeNode2.Bounds) : Rectangle.Union(Bloat(treeNode.Bounds), Bloat(treeNode2.Bounds)));
				rc.X = 0;
				rc.Width = ViewportRectangle.Width;
				Invalidate(rc);
			}
		}
		else
		{
			if (highlighted_node != null)
			{
				Invalidate(highlighted_node.Bounds);
			}
			highlighted_node = focused_node;
			selected_node = focused_node;
			if (selected_node != null)
			{
				Invalidate(selected_node.Bounds);
			}
		}
	}

	private void MouseMoveHandler(object sender, MouseEventArgs e)
	{
		TreeNode nodeAt = GetNodeAt(e.Location);
		if (nodeAt != tooltip_currently_showing)
		{
			MouseLeftItem(tooltip_currently_showing);
		}
		if (nodeAt != null && nodeAt != tooltip_currently_showing)
		{
			MouseEnteredItem(nodeAt);
		}
		if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
		{
			if (drag_begin_x == -1 && drag_begin_y == -1)
			{
				drag_begin_x = e.X;
				drag_begin_y = e.Y;
			}
			else
			{
				double num = Math.Pow(drag_begin_x - e.X, 2.0);
				double num2 = Math.Pow(drag_begin_y - e.Y, 2.0);
				double num3 = Math.Sqrt(num + num2);
				if (num3 > 3.0)
				{
					TreeNode nodeAtUseX = GetNodeAtUseX(e.X, e.Y);
					if (nodeAtUseX != null)
					{
						OnItemDrag(new ItemDragEventArgs(e.Button, nodeAtUseX));
					}
					drag_begin_x = -1;
					drag_begin_y = -1;
				}
			}
		}
		if (select_mmove && !mouse_rect.Contains(e.X, e.Y))
		{
			Invalidate(highlighted_node.Bounds);
			if (selected_node != null)
			{
				Invalidate(selected_node.Bounds);
			}
			if (focused_node != null)
			{
				Invalidate(focused_node.Bounds);
			}
			highlighted_node = selected_node;
			focused_node = selected_node;
			select_mmove = false;
		}
	}

	private void DoubleClickHandler(object sender, MouseEventArgs e)
	{
		GetNodeAtUseX(e.X, e.Y)?.Toggle();
	}

	private bool RectsIntersect(Rectangle r, int left, int top, int width, int height)
	{
		return r.Left <= left + width && r.Right >= left && r.Top <= top + height && r.Bottom >= top;
	}

	private bool WmContextMenu(ref Message m)
	{
		Point point = new Point(Control.LowOrder(m.LParam.ToInt32()), Control.HighOrder(m.LParam.ToInt32()));
		TreeNode treeNode;
		if (point.X == -1 || point.Y == -1)
		{
			treeNode = SelectedNode;
			if (treeNode == null)
			{
				return false;
			}
			point = new Point(treeNode.Bounds.Left, treeNode.Bounds.Top + treeNode.Bounds.Height / 2);
		}
		else
		{
			point = PointToClient(point);
			treeNode = GetNodeAt(point);
			if (treeNode == null)
			{
				return false;
			}
		}
		if (treeNode.ContextMenu != null)
		{
			treeNode.ContextMenu.Show(this, point);
			return true;
		}
		if (treeNode.ContextMenuStrip != null)
		{
			treeNode.ContextMenuStrip.Show(this, point);
			return true;
		}
		return false;
	}

	private void MouseEnteredItem(TreeNode item)
	{
		tooltip_currently_showing = item;
		if (is_hovering)
		{
			if (ShowNodeToolTips && !string.IsNullOrEmpty(tooltip_currently_showing.ToolTipText))
			{
				ToolTipWindow.Present(this, tooltip_currently_showing.ToolTipText);
			}
			OnNodeMouseHover(new TreeNodeMouseHoverEventArgs(tooltip_currently_showing));
		}
	}

	private void MouseLeftItem(TreeNode item)
	{
		ToolTipWindow.Hide(this);
		tooltip_currently_showing = null;
	}

	internal void OnUIACheckBoxesChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIACheckBoxesChanged])?.Invoke(this, e);
	}

	internal void OnUIALabelEditChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIALabelEditChanged])?.Invoke(this, e);
	}

	internal void OnUIANodeTextChanged(TreeViewEventArgs e)
	{
		((TreeViewEventHandler)base.Events[UIANodeTextChanged])?.Invoke(this, e);
	}

	internal void OnUIACollectionChanged(object sender, CollectionChangeEventArgs e)
	{
		CollectionChangeEventHandler collectionChangeEventHandler = (CollectionChangeEventHandler)base.Events[UIACollectionChanged];
		if (collectionChangeEventHandler != null)
		{
			if (sender == root_node)
			{
				sender = this;
			}
			collectionChangeEventHandler(sender, e);
		}
	}
}
