using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;
using Microsoft.Win32;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.PropertyGridDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class PropertyGrid : ContainerControl, IComPropertyBrowser
{
	public class PropertyTabCollection : ICollection, IEnumerable
	{
		private ArrayList property_tabs;

		private ArrayList property_tabs_scopes;

		private PropertyGrid property_grid;

		bool ICollection.IsSynchronized => property_tabs.IsSynchronized;

		object ICollection.SyncRoot => property_tabs.SyncRoot;

		public PropertyTab this[int index] => (PropertyTab)property_tabs[index];

		public int Count => property_tabs.Count;

		internal PropertyTab this[Type tabType]
		{
			get
			{
				foreach (PropertyTab property_tab in property_tabs)
				{
					if (tabType == property_tab.GetType())
					{
						return property_tab;
					}
				}
				return null;
			}
		}

		internal PropertyTabCollection(PropertyGrid propertyGrid)
		{
			property_grid = propertyGrid;
			property_tabs = new ArrayList();
			property_tabs_scopes = new ArrayList();
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			property_tabs.CopyTo(dest, index);
		}

		public IEnumerator GetEnumerator()
		{
			return property_tabs.GetEnumerator();
		}

		public void AddTabType(Type propertyTabType)
		{
			AddTabType(propertyTabType, PropertyTabScope.Global);
		}

		public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
		{
			if (propertyTabType == null)
			{
				throw new ArgumentNullException("propertyTabType");
			}
			if (!Contains(propertyTabType))
			{
				PropertyTab propertyTab = property_grid.CreatePropertyTab(propertyTabType);
				if (propertyTab != null)
				{
					property_tabs.Add(propertyTab);
					property_tabs_scopes.Add(tabScope);
				}
				property_grid.RefreshToolbar(this);
			}
		}

		internal PropertyTabScope GetTabScope(PropertyTab tab)
		{
			if (tab == null)
			{
				throw new ArgumentNullException("tab");
			}
			int num = property_tabs.IndexOf(tab);
			if (num != -1)
			{
				return (PropertyTabScope)(int)property_tabs_scopes[num];
			}
			return PropertyTabScope.Global;
		}

		internal void InsertTab(int index, PropertyTab propertyTab, PropertyTabScope tabScope)
		{
			if (propertyTab == null)
			{
				throw new ArgumentNullException("propertyTab");
			}
			if (!Contains(propertyTab.GetType()))
			{
				property_tabs.Insert(index, propertyTab);
				property_tabs_scopes.Insert(index, tabScope);
			}
		}

		internal bool Contains(Type propertyType)
		{
			if (propertyType == null)
			{
				throw new ArgumentNullException("propertyType");
			}
			foreach (PropertyTab property_tab in property_tabs)
			{
				if (property_tab.GetType() == propertyType)
				{
					return true;
				}
			}
			return false;
		}

		public void Clear(PropertyTabScope tabScope)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < property_tabs_scopes.Count; i++)
			{
				if ((int)property_tabs_scopes[i] == (int)tabScope)
				{
					arrayList.Add(i);
				}
			}
			foreach (int item in arrayList)
			{
				property_tabs.RemoveAt(item);
				property_tabs_scopes.RemoveAt(item);
			}
			property_grid.RefreshToolbar(this);
		}

		public void RemoveTabType(Type propertyTabType)
		{
			if (propertyTabType == null)
			{
				throw new ArgumentNullException("propertyTabType");
			}
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < property_tabs.Count; i++)
			{
				if (property_tabs[i].GetType() == propertyTabType)
				{
					arrayList.Add(i);
				}
			}
			foreach (int item in arrayList)
			{
				property_tabs.RemoveAt(item);
				property_tabs_scopes.RemoveAt(item);
			}
			property_grid.RefreshToolbar(this);
		}
	}

	internal class BorderHelperControl : Control
	{
		public BorderHelperControl()
		{
			BackColor = ThemeEngine.Current.ColorWindow;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(SystemPens.ControlDark, 0, 0, base.Width - 1, base.Height - 1);
			base.OnPaint(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if (base.Controls.Count == 1)
			{
				Control control = base.Controls[0];
				if (control.Location.X != 1 || control.Location.Y != 1)
				{
					control.Location = new Point(1, 1);
				}
				control.Width = base.ClientRectangle.Width - 2;
				control.Height = base.ClientRectangle.Height - 2;
			}
			base.OnSizeChanged(e);
		}
	}

	private class PropertyToolBarSeparator : ToolStripSeparator
	{
	}

	private class PropertyToolBarButton : ToolStripButton
	{
		private PropertyTab property_tab;

		public PropertyTab PropertyTab => property_tab;

		public bool Pushed
		{
			get
			{
				return base.Checked;
			}
			set
			{
				base.Checked = value;
			}
		}

		public ToolBarButtonStyle Style
		{
			get
			{
				return ToolBarButtonStyle.PushButton;
			}
			set
			{
			}
		}

		public PropertyToolBarButton()
		{
		}

		public PropertyToolBarButton(PropertyTab propertyTab)
		{
			if (propertyTab == null)
			{
				throw new ArgumentNullException("propertyTab");
			}
			property_tab = propertyTab;
		}
	}

	internal class PropertyToolBar : ToolStrip
	{
		private ToolBarAppearance appearance;

		public bool ShowToolTips
		{
			get
			{
				return base.ShowItemToolTips;
			}
			set
			{
				base.ShowItemToolTips = value;
			}
		}

		public ToolBarAppearance Appearance
		{
			get
			{
				return appearance;
			}
			set
			{
				if (value != Appearance)
				{
					switch (value)
					{
					case ToolBarAppearance.Flat:
						base.Renderer = new ToolStripSystemRenderer();
						appearance = ToolBarAppearance.Flat;
						break;
					case ToolBarAppearance.Normal:
					{
						ProfessionalColorTable professionalColorTable = new ProfessionalColorTable();
						professionalColorTable.UseSystemColors = true;
						base.Renderer = new ToolStripProfessionalRenderer(professionalColorTable);
						appearance = ToolBarAppearance.Normal;
						break;
					}
					}
				}
			}
		}

		public PropertyToolBar()
		{
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			base.GripStyle = ToolStripGripStyle.Hidden;
			appearance = ToolBarAppearance.Normal;
		}
	}

	[System.MonoInternalNote("not sure what this class does, but it's listed as a type converter for a property in this class, and this causes problems if it's not present")]
	private class SelectedObjectConverter : TypeConverter
	{
	}

	private const string UNCATEGORIZED_CATEGORY_LABEL = "Misc";

	private AttributeCollection browsable_attributes;

	private bool can_show_commands;

	private Color commands_back_color;

	private Color commands_fore_color;

	private bool commands_visible;

	private bool commands_visible_if_available;

	private Point context_menu_default_location;

	private bool large_buttons;

	private Color line_color;

	private PropertySort property_sort;

	private PropertyTabCollection property_tabs;

	private GridEntry selected_grid_item;

	private GridEntry root_grid_item;

	private object[] selected_objects;

	private PropertyTab properties_tab;

	private PropertyTab selected_tab;

	private ImageList toolbar_imagelist;

	private Image categorized_image;

	private Image alphabetical_image;

	private Image propertypages_image;

	private PropertyToolBarButton categorized_toolbarbutton;

	private PropertyToolBarButton alphabetic_toolbarbutton;

	private PropertyToolBarButton propertypages_toolbarbutton;

	private PropertyToolBarSeparator separator_toolbarbutton;

	private bool events_tab_visible;

	private PropertyToolBar toolbar;

	private PropertyGridView property_grid_view;

	private Splitter splitter;

	private Panel help_panel;

	private Label help_title_label;

	private Label help_description_label;

	private MenuItem reset_menuitem;

	private MenuItem description_menuitem;

	private Color category_fore_color;

	private Color commands_active_link_color;

	private Color commands_disabled_link_color;

	private Color commands_link_color;

	private static object PropertySortChangedEvent;

	private static object PropertyTabChangedEvent;

	private static object PropertyValueChangedEvent;

	private static object SelectedGridItemChangedEvent;

	private static object SelectedObjectsChangedEvent;

	private static object ComComponentNameChangedEvent;

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	bool IComPropertyBrowser.InPropertySet
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AttributeCollection BrowsableAttributes
	{
		get
		{
			if (browsable_attributes == null)
			{
				browsable_attributes = new AttributeCollection(BrowsableAttribute.Yes);
			}
			return browsable_attributes;
		}
		set
		{
			if (browsable_attributes != value)
			{
				if (browsable_attributes == null || browsable_attributes.Count == 0)
				{
					browsable_attributes = null;
				}
				else
				{
					browsable_attributes = value;
				}
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
			toolbar.BackColor = value;
			Refresh();
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
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual bool CanShowCommands => can_show_commands;

	[DefaultValue(typeof(Color), "ControlText")]
	public Color CategoryForeColor
	{
		get
		{
			return category_fore_color;
		}
		set
		{
			if (category_fore_color != value)
			{
				category_fore_color = value;
				Invalidate();
			}
		}
	}

	public Color CommandsBackColor
	{
		get
		{
			return commands_back_color;
		}
		set
		{
			if (!(commands_back_color == value))
			{
				commands_back_color = value;
			}
		}
	}

	public Color CommandsForeColor
	{
		get
		{
			return commands_fore_color;
		}
		set
		{
			if (!(commands_fore_color == value))
			{
				commands_fore_color = value;
			}
		}
	}

	public Color CommandsActiveLinkColor
	{
		get
		{
			return commands_active_link_color;
		}
		set
		{
			commands_active_link_color = value;
		}
	}

	public Color CommandsDisabledLinkColor
	{
		get
		{
			return commands_disabled_link_color;
		}
		set
		{
			commands_disabled_link_color = value;
		}
	}

	public Color CommandsLinkColor
	{
		get
		{
			return commands_link_color;
		}
		set
		{
			commands_link_color = value;
		}
	}

	[System.MonoTODO("Commands are not implemented yet.")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public virtual bool CommandsVisible => commands_visible;

	[DefaultValue(true)]
	public virtual bool CommandsVisibleIfAvailable
	{
		get
		{
			return commands_visible_if_available;
		}
		set
		{
			if (commands_visible_if_available != value)
			{
				commands_visible_if_available = value;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public Point ContextMenuDefaultLocation => context_menu_default_location;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ControlCollection Controls => base.Controls;

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

	[DefaultValue("Color [Control]")]
	public Color HelpBackColor
	{
		get
		{
			return help_panel.BackColor;
		}
		set
		{
			help_panel.BackColor = value;
		}
	}

	[DefaultValue("Color [ControlText]")]
	public Color HelpForeColor
	{
		get
		{
			return help_panel.ForeColor;
		}
		set
		{
			help_panel.ForeColor = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(true)]
	public virtual bool HelpVisible
	{
		get
		{
			return help_panel.Visible;
		}
		set
		{
			splitter.Visible = value;
			help_panel.Visible = value;
		}
	}

	[DefaultValue(false)]
	public bool LargeButtons
	{
		get
		{
			return large_buttons;
		}
		set
		{
			if (large_buttons != value)
			{
				large_buttons = value;
			}
		}
	}

	[DefaultValue("Color [InactiveBorder]")]
	public Color LineColor
	{
		get
		{
			return line_color;
		}
		set
		{
			if (!(line_color == value))
			{
				line_color = value;
			}
		}
	}

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

	[DefaultValue(PropertySort.CategorizedAlphabetical)]
	public PropertySort PropertySort
	{
		get
		{
			return property_sort;
		}
		set
		{
			if (!Enum.IsDefined(typeof(PropertySort), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(PropertySort));
			}
			if (property_sort == value)
			{
				return;
			}
			bool flag = (property_sort & PropertySort.Categorized) == 0 || (value & PropertySort.Categorized) == 0;
			property_sort = value;
			if (flag)
			{
				UpdateSortLayout(root_grid_item);
				if (selected_grid_item != null)
				{
					if (selected_grid_item.GridItemType == GridItemType.Category && (value == PropertySort.Alphabetical || value == PropertySort.NoSort))
					{
						SelectItemCore(null, null);
					}
					else
					{
						SelectItemCore(null, selected_grid_item);
					}
				}
				property_grid_view.UpdateView();
				((EventHandler)base.Events[PropertySortChanged])?.Invoke(this, EventArgs.Empty);
			}
			UpdatePropertySortButtonsState();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public PropertyTabCollection PropertyTabs => property_tabs;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public GridItem SelectedGridItem
	{
		get
		{
			return selected_grid_item;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentException("GridItem specified to PropertyGrid.SelectedGridItem must be a valid GridItem.");
			}
			if (value != selected_grid_item)
			{
				GridEntry gridEntry = selected_grid_item;
				SelectItemCore(gridEntry, (GridEntry)value);
				OnSelectedGridItemChanged(new SelectedGridItemChangedEventArgs(gridEntry, value));
			}
		}
	}

	internal GridItem RootGridItem => root_grid_item;

	[TypeConverter("System.Windows.Forms.PropertyGrid+SelectedObjectConverter, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	[DefaultValue(null)]
	public object SelectedObject
	{
		get
		{
			if (selected_objects.Length > 0)
			{
				return selected_objects[0];
			}
			return null;
		}
		set
		{
			if (selected_objects == null || selected_objects.Length != 1 || selected_objects[0] != value)
			{
				if (value == null)
				{
					SelectedObjects = new object[0];
					return;
				}
				SelectedObjects = new object[1] { value };
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public object[] SelectedObjects
	{
		get
		{
			return selected_objects;
		}
		set
		{
			root_grid_item = null;
			SelectItemCore(null, null);
			if (value != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] == null)
					{
						throw new ArgumentException($"Item {i} in the objs array is null.");
					}
				}
				selected_objects = value;
			}
			else
			{
				selected_objects = new object[0];
			}
			ShowEventsButton(value: false);
			PopulateGrid(selected_objects);
			RefreshTabs(PropertyTabScope.Component);
			if (root_grid_item != null)
			{
				SelectItemCore(null, GetDefaultPropertyItem(root_grid_item, selected_tab));
			}
			property_grid_view.UpdateView();
			OnSelectedObjectsChanged(EventArgs.Empty);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public PropertyTab SelectedTab => selected_tab;

	public override ISite Site
	{
		get
		{
			return base.Site;
		}
		set
		{
			base.Site = value;
		}
	}

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

	[DefaultValue(true)]
	public virtual bool ToolbarVisible
	{
		get
		{
			return toolbar.Visible;
		}
		set
		{
			if (toolbar.Visible != value)
			{
				toolbar.Visible = value;
			}
		}
	}

	protected ToolStripRenderer ToolStripRenderer
	{
		get
		{
			if (toolbar != null)
			{
				return toolbar.Renderer;
			}
			return null;
		}
		set
		{
			if (toolbar != null)
			{
				toolbar.Renderer = value;
			}
		}
	}

	[DefaultValue("Color [Window]")]
	public Color ViewBackColor
	{
		get
		{
			return property_grid_view.BackColor;
		}
		set
		{
			if (!(property_grid_view.BackColor == value))
			{
				property_grid_view.BackColor = value;
			}
		}
	}

	[DefaultValue("Color [WindowText]")]
	public Color ViewForeColor
	{
		get
		{
			return property_grid_view.ForeColor;
		}
		set
		{
			if (!(property_grid_view.ForeColor == value))
			{
				property_grid_view.ForeColor = value;
			}
		}
	}

	[DefaultValue(false)]
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

	protected override Size DefaultSize => base.DefaultSize;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual Type DefaultTabType => typeof(PropertiesTab);

	protected bool DrawFlatToolbar
	{
		get
		{
			return toolbar.Appearance == ToolBarAppearance.Flat;
		}
		set
		{
			if (value)
			{
				toolbar.Appearance = ToolBarAppearance.Flat;
			}
			else
			{
				toolbar.Appearance = ToolBarAppearance.Normal;
			}
		}
	}

	protected internal override bool ShowFocusCues => base.ShowFocusCues;

	public event EventHandler PropertySortChanged
	{
		add
		{
			base.Events.AddHandler(PropertySortChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PropertySortChangedEvent, value);
		}
	}

	public event PropertyTabChangedEventHandler PropertyTabChanged
	{
		add
		{
			base.Events.AddHandler(PropertyTabChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PropertyTabChangedEvent, value);
		}
	}

	public event PropertyValueChangedEventHandler PropertyValueChanged
	{
		add
		{
			base.Events.AddHandler(PropertyValueChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PropertyValueChangedEvent, value);
		}
	}

	public event SelectedGridItemChangedEventHandler SelectedGridItemChanged
	{
		add
		{
			base.Events.AddHandler(SelectedGridItemChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectedGridItemChangedEvent, value);
		}
	}

	public event EventHandler SelectedObjectsChanged
	{
		add
		{
			base.Events.AddHandler(SelectedObjectsChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectedObjectsChangedEvent, value);
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
	[EditorBrowsable(EditorBrowsableState.Advanced)]
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

	[EditorBrowsable(EditorBrowsableState.Advanced)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
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

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public new event MouseEventHandler MouseDown
	{
		add
		{
			base.MouseDown += value;
		}
		remove
		{
			base.MouseDown -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event EventHandler MouseEnter
	{
		add
		{
			base.MouseEnter += value;
		}
		remove
		{
			base.MouseEnter -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event EventHandler MouseLeave
	{
		add
		{
			base.MouseLeave += value;
		}
		remove
		{
			base.MouseLeave -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event MouseEventHandler MouseMove
	{
		add
		{
			base.MouseMove += value;
		}
		remove
		{
			base.MouseMove -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new event MouseEventHandler MouseUp
	{
		add
		{
			base.MouseUp += value;
		}
		remove
		{
			base.MouseUp -= value;
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

	event ComponentRenameEventHandler IComPropertyBrowser.ComComponentNameChanged
	{
		add
		{
			base.Events.AddHandler(ComComponentNameChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ComComponentNameChangedEvent, value);
		}
	}

	public PropertyGrid()
	{
		selected_objects = new object[0];
		property_tabs = new PropertyTabCollection(this);
		line_color = SystemColors.ScrollBar;
		category_fore_color = line_color;
		commands_visible = false;
		commands_visible_if_available = false;
		property_sort = PropertySort.CategorizedAlphabetical;
		property_grid_view = new PropertyGridView(this);
		splitter = new Splitter();
		splitter.Dock = DockStyle.Bottom;
		help_panel = new Panel();
		help_panel.Dock = DockStyle.Bottom;
		help_panel.Height = 50;
		help_panel.BackColor = SystemColors.Control;
		help_title_label = new Label();
		help_title_label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		help_title_label.Name = "help_title_label";
		help_title_label.Font = new Font(Font, FontStyle.Bold);
		help_title_label.Location = new Point(2, 2);
		help_title_label.Height = 17;
		help_title_label.Width = help_panel.Width - 4;
		help_description_label = new Label();
		help_description_label.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		help_description_label.AutoEllipsis = true;
		help_description_label.AutoSize = false;
		help_description_label.Font = Font;
		help_description_label.Location = new Point(2, help_title_label.Top + help_title_label.Height);
		help_description_label.Width = help_panel.Width - 4;
		help_description_label.Height = help_panel.Height - help_description_label.Top - 2;
		help_panel.Controls.Add(help_description_label);
		help_panel.Controls.Add(help_title_label);
		help_panel.Paint += help_panel_Paint;
		toolbar = new PropertyToolBar();
		toolbar.Dock = DockStyle.Top;
		categorized_toolbarbutton = new PropertyToolBarButton();
		categorized_toolbarbutton.Pushed = true;
		alphabetic_toolbarbutton = new PropertyToolBarButton();
		propertypages_toolbarbutton = new PropertyToolBarButton();
		separator_toolbarbutton = new PropertyToolBarSeparator();
		ContextMenu contextMenu = new ContextMenu();
		context_menu_default_location = Point.Empty;
		categorized_image = new Bitmap(typeof(PropertyGrid), "propertygrid-categorized.png");
		alphabetical_image = new Bitmap(typeof(PropertyGrid), "propertygrid-alphabetical.png");
		propertypages_image = new Bitmap(typeof(PropertyGrid), "propertygrid-propertypages.png");
		toolbar_imagelist = new ImageList();
		toolbar_imagelist.ColorDepth = ColorDepth.Depth32Bit;
		toolbar_imagelist.ImageSize = new Size(16, 16);
		toolbar_imagelist.TransparentColor = Color.Transparent;
		toolbar.Appearance = ToolBarAppearance.Flat;
		toolbar.AutoSize = false;
		toolbar.ImageList = toolbar_imagelist;
		toolbar.Location = new Point(0, 0);
		toolbar.ShowToolTips = true;
		toolbar.Size = new Size(256, 27);
		toolbar.TabIndex = 0;
		toolbar.Items.AddRange(new ToolStripItem[4]
		{
			categorized_toolbarbutton,
			alphabetic_toolbarbutton,
			new PropertyToolBarSeparator(),
			propertypages_toolbarbutton
		});
		categorized_toolbarbutton.Click += toolbarbutton_clicked;
		alphabetic_toolbarbutton.Click += toolbarbutton_clicked;
		propertypages_toolbarbutton.Click += toolbarbutton_clicked;
		categorized_toolbarbutton.Style = ToolBarButtonStyle.ToggleButton;
		categorized_toolbarbutton.ToolTipText = Locale.GetText("Categorized");
		alphabetic_toolbarbutton.Style = ToolBarButtonStyle.ToggleButton;
		alphabetic_toolbarbutton.ToolTipText = Locale.GetText("Alphabetic");
		propertypages_toolbarbutton.Enabled = false;
		propertypages_toolbarbutton.Style = ToolBarButtonStyle.ToggleButton;
		propertypages_toolbarbutton.ToolTipText = "Property Pages";
		properties_tab = CreatePropertyTab(DefaultTabType);
		selected_tab = properties_tab;
		RefreshToolbar(property_tabs);
		reset_menuitem = contextMenu.MenuItems.Add("Reset");
		reset_menuitem.Click += OnResetPropertyClick;
		contextMenu.MenuItems.Add("-");
		description_menuitem = contextMenu.MenuItems.Add("Description");
		description_menuitem.Click += OnDescriptionClick;
		description_menuitem.Checked = HelpVisible;
		ContextMenu = contextMenu;
		toolbar.ContextMenu = contextMenu;
		BorderHelperControl value = new BorderHelperControl
		{
			Dock = DockStyle.Fill,
			Controls = { (Control)property_grid_view }
		};
		Controls.Add(value);
		Controls.Add(toolbar);
		Controls.Add(splitter);
		Controls.Add(help_panel);
		base.Name = "PropertyGrid";
		base.Size = new Size(256, 400);
	}

	static PropertyGrid()
	{
		PropertySortChanged = new object();
		PropertyTabChanged = new object();
		PropertyValueChanged = new object();
		SelectedGridItemChanged = new object();
		SelectedObjectsChanged = new object();
		ComComponentNameChangedEvent = new object();
	}

	[System.MonoTODO("Stub, does nothing")]
	void IComPropertyBrowser.DropDownDone()
	{
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	bool IComPropertyBrowser.EnsurePendingChangesCommitted()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Stub, does nothing")]
	void IComPropertyBrowser.HandleF4()
	{
	}

	[System.MonoTODO("Stub, does nothing")]
	void IComPropertyBrowser.LoadState(RegistryKey optRoot)
	{
	}

	[System.MonoTODO("Stub, does nothing")]
	void IComPropertyBrowser.SaveState(RegistryKey optRoot)
	{
	}

	private void UpdateHelp(GridItem item)
	{
		if (item == null)
		{
			help_title_label.Text = string.Empty;
			help_description_label.Text = string.Empty;
			return;
		}
		help_title_label.Text = item.Label;
		if (item.PropertyDescriptor != null)
		{
			help_description_label.Text = item.PropertyDescriptor.Description;
		}
	}

	private void SelectItemCore(GridEntry oldItem, GridEntry item)
	{
		UpdateHelp(item);
		selected_grid_item = item;
		property_grid_view.SelectItem(oldItem, item);
	}

	internal void OnPropertyValueChangedInternal(GridItem item, object property_value)
	{
		property_grid_view.UpdateView();
		OnPropertyValueChanged(new PropertyValueChangedEventArgs(item, property_value));
	}

	internal void OnExpandItem(GridEntry item)
	{
		property_grid_view.ExpandItem(item);
	}

	internal void OnCollapseItem(GridEntry item)
	{
		property_grid_view.CollapseItem(item);
	}

	internal DialogResult ShowError(string text)
	{
		return ShowError(text, MessageBoxButtons.OK);
	}

	internal DialogResult ShowError(string text, MessageBoxButtons buttons)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		return MessageBox.Show(this, text, "Properties Window", buttons, MessageBoxIcon.Exclamation);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	public void CollapseAllGridItems()
	{
		GridEntry gridEntry = FindCategoryItem(selected_grid_item);
		if (gridEntry != null)
		{
			SelectedGridItem = gridEntry;
		}
		CollapseItemRecursive(root_grid_item);
		property_grid_view.UpdateView();
	}

	private void CollapseItemRecursive(GridItem item)
	{
		if (item == null)
		{
			return;
		}
		foreach (GridItem gridItem in item.GridItems)
		{
			CollapseItemRecursive(gridItem);
			if (gridItem.Expandable)
			{
				gridItem.Expanded = false;
			}
		}
	}

	private GridEntry FindCategoryItem(GridEntry entry)
	{
		if (entry == null || (property_sort != PropertySort.Categorized && property_sort != PropertySort.CategorizedAlphabetical))
		{
			return null;
		}
		if (entry.GridItemType == GridItemType.Category)
		{
			return entry;
		}
		GridEntry gridEntry = null;
		GridItem gridItem = entry;
		while (gridEntry == null)
		{
			if (gridItem.Parent != null && gridItem.Parent.GridItemType == GridItemType.Category)
			{
				gridEntry = (GridEntry)gridItem.Parent;
			}
			gridItem = gridItem.Parent;
			if (gridItem == null)
			{
				break;
			}
		}
		return gridEntry;
	}

	public void ExpandAllGridItems()
	{
		ExpandItemRecursive(root_grid_item);
		property_grid_view.UpdateView();
	}

	private void ExpandItemRecursive(GridItem item)
	{
		if (item == null)
		{
			return;
		}
		foreach (GridItem gridItem in item.GridItems)
		{
			ExpandItemRecursive(gridItem);
			if (gridItem.Expandable)
			{
				gridItem.Expanded = true;
			}
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		SelectedObjects = SelectedObjects;
	}

	private void toolbar_Clicked(PropertyToolBarButton button)
	{
		if (button != null)
		{
			if (button == alphabetic_toolbarbutton)
			{
				PropertySort = PropertySort.Alphabetical;
				alphabetic_toolbarbutton.Pushed = true;
				categorized_toolbarbutton.Pushed = false;
			}
			else if (button == categorized_toolbarbutton)
			{
				PropertySort = PropertySort.CategorizedAlphabetical;
				categorized_toolbarbutton.Pushed = true;
				alphabetic_toolbarbutton.Pushed = false;
			}
			else if (button.Enabled)
			{
				SelectPropertyTab(button.PropertyTab);
			}
		}
	}

	private void toolbarbutton_clicked(object o, EventArgs args)
	{
		toolbar_Clicked(o as PropertyToolBarButton);
	}

	private void SelectPropertyTab(PropertyTab propertyTab)
	{
		if (propertyTab == null || selected_tab == propertyTab)
		{
			return;
		}
		foreach (object item in toolbar.Items)
		{
			if (item is PropertyToolBarButton propertyToolBarButton && propertyToolBarButton.PropertyTab != null)
			{
				if (propertyToolBarButton.PropertyTab == selected_tab)
				{
					propertyToolBarButton.Pushed = false;
				}
				else if (propertyToolBarButton.PropertyTab == propertyTab)
				{
					propertyToolBarButton.Pushed = true;
				}
			}
		}
		selected_tab = propertyTab;
		PopulateGrid(selected_objects);
		SelectItemCore(null, GetDefaultPropertyItem(root_grid_item, selected_tab));
		property_grid_view.UpdateView();
	}

	private void UpdatePropertySortButtonsState()
	{
		if (property_sort == PropertySort.NoSort)
		{
			alphabetic_toolbarbutton.Pushed = false;
			categorized_toolbarbutton.Pushed = false;
		}
		else if (property_sort == PropertySort.Alphabetical)
		{
			alphabetic_toolbarbutton.Pushed = true;
			categorized_toolbarbutton.Pushed = false;
		}
		else if (property_sort == PropertySort.Categorized || property_sort == PropertySort.CategorizedAlphabetical)
		{
			alphabetic_toolbarbutton.Pushed = false;
			categorized_toolbarbutton.Pushed = true;
		}
	}

	protected void ShowEventsButton(bool value)
	{
		if (value && property_tabs.Contains(typeof(EventsTab)))
		{
			events_tab_visible = true;
		}
		else
		{
			events_tab_visible = false;
		}
		RefreshTabs(PropertyTabScope.Component);
	}

	public void RefreshTabs(PropertyTabScope tabScope)
	{
		property_tabs.Clear(tabScope);
		if (selected_objects != null)
		{
			Type[] tabTypes = null;
			PropertyTabScope[] tabScopes = null;
			if (events_tab_visible && property_tabs.Contains(typeof(EventsTab)))
			{
				property_tabs.InsertTab(0, properties_tab, PropertyTabScope.Component);
			}
			GetMergedPropertyTabs(selected_objects, out tabTypes, out tabScopes);
			if (tabTypes != null && tabScopes != null && tabTypes.Length > 0)
			{
				bool flag = false;
				for (int i = 0; i < tabTypes.Length; i++)
				{
					property_tabs.AddTabType(tabTypes[i], tabScopes[i]);
					if (tabTypes[i] == selected_tab.GetType())
					{
						flag = true;
					}
				}
				if (!flag)
				{
					SelectPropertyTab(properties_tab);
				}
			}
		}
		else
		{
			SelectPropertyTab(properties_tab);
		}
		RefreshToolbar(property_tabs);
	}

	private void RefreshToolbar(PropertyTabCollection tabs)
	{
		EnsurePropertiesTab();
		toolbar.SuspendLayout();
		toolbar.Items.Clear();
		toolbar_imagelist.Images.Clear();
		int num = 0;
		toolbar.Items.Add(categorized_toolbarbutton);
		toolbar_imagelist.Images.Add(categorized_image);
		categorized_toolbarbutton.ImageIndex = num;
		num++;
		toolbar.Items.Add(alphabetic_toolbarbutton);
		toolbar_imagelist.Images.Add(alphabetical_image);
		alphabetic_toolbarbutton.ImageIndex = num;
		num++;
		toolbar.Items.Add(separator_toolbarbutton);
		if (tabs != null && tabs.Count > 0)
		{
			foreach (PropertyTab tab in tabs)
			{
				PropertyToolBarButton propertyToolBarButton = new PropertyToolBarButton(tab);
				toolbar.Items.Add(propertyToolBarButton);
				if (tab.Bitmap != null)
				{
					tab.Bitmap.MakeTransparent();
					toolbar_imagelist.Images.Add(tab.Bitmap);
					propertyToolBarButton.ImageIndex = num;
					num++;
				}
				if (tab == selected_tab)
				{
					propertyToolBarButton.Pushed = true;
				}
			}
			toolbar.Items.Add(new PropertyToolBarSeparator());
		}
		toolbar.Items.Add(propertypages_toolbarbutton);
		toolbar_imagelist.Images.Add(propertypages_image);
		propertypages_toolbarbutton.ImageIndex = num;
		toolbar.ResumeLayout();
	}

	private void EnsurePropertiesTab()
	{
		if (property_tabs != null && property_tabs.Count > 0 && !property_tabs.Contains(DefaultTabType))
		{
			property_tabs.InsertTab(0, properties_tab, PropertyTabScope.Component);
		}
	}

	private void GetMergedPropertyTabs(object[] objects, out Type[] tabTypes, out PropertyTabScope[] tabScopes)
	{
		tabTypes = null;
		tabScopes = null;
		if (objects == null || objects.Length == 0)
		{
			return;
		}
		ArrayList arrayList = null;
		ArrayList arrayList2 = new ArrayList();
		for (int i = 0; i < objects.Length; i++)
		{
			if (objects[i] == null)
			{
				continue;
			}
			PropertyTabAttribute propertyTabAttribute = (PropertyTabAttribute)TypeDescriptor.GetAttributes(objects[i])[typeof(PropertyTabAttribute)];
			if (propertyTabAttribute == null || propertyTabAttribute.TabClasses == null || propertyTabAttribute.TabClasses.Length == 0)
			{
				return;
			}
			ArrayList arrayList3 = new ArrayList();
			arrayList2.Clear();
			object obj;
			if (i == 0)
			{
				IList tabClasses = propertyTabAttribute.TabClasses;
				obj = tabClasses;
			}
			else
			{
				obj = arrayList;
			}
			IList list = (IList)obj;
			for (int j = 0; j < list.Count; j++)
			{
				if ((Type)arrayList[j] == propertyTabAttribute.TabClasses[j])
				{
					arrayList3.Add(propertyTabAttribute.TabClasses[j]);
					arrayList2.Add(propertyTabAttribute.TabScopes[j]);
				}
			}
			arrayList = arrayList3;
		}
		tabTypes = new Type[arrayList.Count];
		arrayList.CopyTo(tabTypes);
		tabScopes = new PropertyTabScope[tabTypes.Length];
		arrayList2.CopyTo(tabScopes);
	}

	public void ResetSelectedProperty()
	{
		if (selected_grid_item != null)
		{
			selected_grid_item.ResetValue();
		}
	}

	protected virtual PropertyTab CreatePropertyTab(Type tabType)
	{
		if (!typeof(PropertyTab).IsAssignableFrom(tabType))
		{
			return null;
		}
		PropertyTab propertyTab = null;
		ConstructorInfo constructor = tabType.GetConstructor(new Type[1] { typeof(IServiceProvider) });
		if (constructor != null)
		{
			return (PropertyTab)constructor.Invoke(new object[1] { Site });
		}
		return (PropertyTab)Activator.CreateInstance(tabType);
	}

	[System.MonoTODO("Never called")]
	protected void OnComComponentNameChanged(ComponentRenameEventArgs e)
	{
		((ComponentRenameEventHandler)base.Events[ComComponentNameChangedEvent])?.Invoke(this, e);
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void OnMouseDown(MouseEventArgs me)
	{
		base.OnMouseDown(me);
	}

	protected override void OnMouseMove(MouseEventArgs me)
	{
		base.OnMouseMove(me);
	}

	protected override void OnMouseUp(MouseEventArgs me)
	{
		base.OnMouseUp(me);
	}

	protected void OnNotifyPropertyValueUIItemsChanged(object sender, EventArgs e)
	{
		property_grid_view.UpdateView();
	}

	protected override void OnPaint(PaintEventArgs pevent)
	{
		pevent.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), pevent.ClipRectangle);
		base.OnPaint(pevent);
	}

	protected virtual void OnPropertySortChanged(EventArgs e)
	{
		((EventHandler)base.Events[PropertySortChanged])?.Invoke(this, e);
	}

	protected virtual void OnPropertyTabChanged(PropertyTabChangedEventArgs e)
	{
		((PropertyTabChangedEventHandler)base.Events[PropertyTabChanged])?.Invoke(this, e);
	}

	protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
	{
		((PropertyValueChangedEventHandler)base.Events[PropertyValueChanged])?.Invoke(this, e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
	}

	protected virtual void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
	{
		((SelectedGridItemChangedEventHandler)base.Events[SelectedGridItemChanged])?.Invoke(this, e);
	}

	protected virtual void OnSelectedObjectsChanged(EventArgs e)
	{
		((EventHandler)base.Events[SelectedObjectsChanged])?.Invoke(this, e);
	}

	protected override void OnSystemColorsChanged(EventArgs e)
	{
		base.OnSystemColorsChanged(e);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		return base.ProcessDialogKey(keyData);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void ScaleCore(float dx, float dy)
	{
		base.ScaleCore(dx, dy);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	private GridItem FindFirstPropertyItem(GridItem root)
	{
		if (root.GridItemType == GridItemType.Property)
		{
			return root;
		}
		foreach (GridItem gridItem2 in root.GridItems)
		{
			GridItem gridItem = FindFirstPropertyItem(gridItem2);
			if (gridItem != null)
			{
				return gridItem;
			}
		}
		return null;
	}

	private GridEntry GetDefaultPropertyItem(GridEntry rootItem, PropertyTab propertyTab)
	{
		if (rootItem == null || rootItem.GridItems.Count == 0 || propertyTab == null)
		{
			return null;
		}
		object[] values = rootItem.Values;
		if (values == null || values.Length == 0 || values[0] == null)
		{
			return null;
		}
		GridItem gridItem = null;
		if (values.Length > 1)
		{
			gridItem = rootItem.GridItems[0];
		}
		else
		{
			PropertyDescriptor defaultProperty = propertyTab.GetDefaultProperty(values[0]);
			if (defaultProperty != null)
			{
				gridItem = FindItem(defaultProperty.Name, rootItem);
			}
			if (gridItem == null)
			{
				gridItem = FindFirstPropertyItem(rootItem);
			}
		}
		return gridItem as GridEntry;
	}

	private GridEntry FindItem(string name, GridEntry rootItem)
	{
		if (rootItem == null || name == null)
		{
			return null;
		}
		if (property_sort == PropertySort.Alphabetical || property_sort == PropertySort.NoSort)
		{
			foreach (GridItem gridItem4 in rootItem.GridItems)
			{
				if (gridItem4.Label == name)
				{
					return (GridEntry)gridItem4;
				}
			}
		}
		else if (property_sort == PropertySort.Categorized || property_sort == PropertySort.CategorizedAlphabetical)
		{
			foreach (GridItem gridItem5 in rootItem.GridItems)
			{
				foreach (GridItem gridItem6 in gridItem5.GridItems)
				{
					if (gridItem6.Label == name)
					{
						return (GridEntry)gridItem6;
					}
				}
			}
		}
		return null;
	}

	private void OnResetPropertyClick(object sender, EventArgs e)
	{
		ResetSelectedProperty();
	}

	private void OnDescriptionClick(object sender, EventArgs e)
	{
		HelpVisible = !HelpVisible;
		description_menuitem.Checked = HelpVisible;
	}

	private void PopulateGrid(object[] objects)
	{
		if (objects.Length > 0)
		{
			root_grid_item = new RootGridEntry(this, objects);
			root_grid_item.Expanded = true;
			UpdateSortLayout(root_grid_item);
		}
		else
		{
			root_grid_item = null;
		}
	}

	private void UpdateSortLayout(GridEntry rootItem)
	{
		if (rootItem == null)
		{
			return;
		}
		GridItemCollection gridItemCollection = new GridItemCollection();
		if (property_sort == PropertySort.Alphabetical || property_sort == PropertySort.NoSort)
		{
			alphabetic_toolbarbutton.Pushed = true;
			categorized_toolbarbutton.Pushed = false;
			foreach (GridItem gridItem5 in rootItem.GridItems)
			{
				if (gridItem5.GridItemType == GridItemType.Category)
				{
					foreach (GridItem gridItem6 in gridItem5.GridItems)
					{
						gridItemCollection.Add(gridItem6);
						((GridEntry)gridItem6).SetParent(rootItem);
					}
				}
				else
				{
					gridItemCollection.Add(gridItem5);
				}
			}
		}
		else if (property_sort == PropertySort.Categorized || property_sort == PropertySort.CategorizedAlphabetical)
		{
			alphabetic_toolbarbutton.Pushed = false;
			categorized_toolbarbutton.Pushed = true;
			GridItemCollection gridItemCollection2 = new GridItemCollection();
			foreach (GridItem gridItem7 in rootItem.GridItems)
			{
				if (gridItem7.GridItemType == GridItemType.Category)
				{
					gridItemCollection2.Add(gridItem7);
					continue;
				}
				string text = gridItem7.PropertyDescriptor.Category;
				if (text == null)
				{
					text = "Misc";
				}
				GridItem gridItem4 = rootItem.GridItems[text];
				if (gridItem4 == null)
				{
					gridItem4 = gridItemCollection2[text];
				}
				if (gridItem4 == null)
				{
					gridItem4 = new CategoryGridEntry(this, text, rootItem);
					gridItem4.Expanded = true;
					gridItemCollection2.Add(gridItem4);
				}
				gridItem4.GridItems.Add(gridItem7);
				((GridEntry)gridItem7).SetParent(gridItem4);
			}
			gridItemCollection.AddRange(gridItemCollection2);
		}
		rootItem.GridItems.Clear();
		rootItem.GridItems.AddRange(gridItemCollection);
	}

	private void help_panel_Paint(object sender, PaintEventArgs e)
	{
		e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(help_panel.BackColor), help_panel.ClientRectangle);
		e.Graphics.DrawRectangle(SystemPens.ControlDark, 0, 0, help_panel.Width - 1, help_panel.Height - 1);
	}
}
