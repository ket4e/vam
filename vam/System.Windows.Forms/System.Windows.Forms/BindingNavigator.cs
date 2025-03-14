using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.BindingNavigatorDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultProperty("BindingSource")]
[DefaultEvent("RefreshItems")]
[ComVisible(true)]
public class BindingNavigator : ToolStrip, ISupportInitialize
{
	private ToolStripItem addNewItem;

	private BindingSource bindingSource;

	private ToolStripItem countItem;

	private string countItemFormat = Locale.GetText("of {0}");

	private ToolStripItem deleteItem;

	private bool initFlag;

	private ToolStripItem moveFirstItem;

	private ToolStripItem moveLastItem;

	private ToolStripItem moveNextItem;

	private ToolStripItem movePreviousItem;

	private ToolStripItem positionItem;

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem AddNewItem
	{
		get
		{
			return addNewItem;
		}
		set
		{
			ReplaceItem(ref addNewItem, value, OnAddNew);
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	[DefaultValue(null)]
	public BindingSource BindingSource
	{
		get
		{
			return bindingSource;
		}
		set
		{
			AttachNewSource(value);
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem CountItem
	{
		get
		{
			return countItem;
		}
		set
		{
			countItem = value;
			OnRefreshItems();
		}
	}

	public string CountItemFormat
	{
		get
		{
			return countItemFormat;
		}
		set
		{
			countItemFormat = value;
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem DeleteItem
	{
		get
		{
			return deleteItem;
		}
		set
		{
			ReplaceItem(ref deleteItem, value, OnDelete);
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem MoveFirstItem
	{
		get
		{
			return moveFirstItem;
		}
		set
		{
			ReplaceItem(ref moveFirstItem, value, OnMoveFirst);
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem MoveLastItem
	{
		get
		{
			return moveLastItem;
		}
		set
		{
			ReplaceItem(ref moveLastItem, value, OnMoveLast);
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem MoveNextItem
	{
		get
		{
			return moveNextItem;
		}
		set
		{
			ReplaceItem(ref moveNextItem, value, OnMoveNext);
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem MovePreviousItem
	{
		get
		{
			return movePreviousItem;
		}
		set
		{
			ReplaceItem(ref movePreviousItem, value, OnMovePrevious);
			OnRefreshItems();
		}
	}

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripItem PositionItem
	{
		get
		{
			return positionItem;
		}
		set
		{
			positionItem = value;
			OnRefreshItems();
		}
	}

	public event EventHandler RefreshItems;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public BindingNavigator()
		: this(addStandardItems: false)
	{
	}

	public BindingNavigator(BindingSource bindingSource)
	{
		AttachNewSource(bindingSource);
		AddStandardItems();
	}

	public BindingNavigator(bool addStandardItems)
	{
		bindingSource = null;
		if (addStandardItems)
		{
			AddStandardItems();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public BindingNavigator(IContainer container)
	{
		bindingSource = null;
		container.Add(this);
	}

	private void ReplaceItem(ref ToolStripItem existingItem, ToolStripItem newItem, EventHandler clickHandler)
	{
		if (existingItem != null)
		{
			existingItem.Click -= clickHandler;
		}
		if (newItem != null)
		{
			newItem.Click += clickHandler;
		}
		existingItem = newItem;
	}

	public virtual void AddStandardItems()
	{
		BeginInit();
		MoveFirstItem = new ToolStripButton();
		moveFirstItem.Image = ResourceImageLoader.Get("nav_first.png");
		moveFirstItem.ToolTipText = Locale.GetText("Move first");
		Items.Add(moveFirstItem);
		MovePreviousItem = new ToolStripButton();
		movePreviousItem.Image = ResourceImageLoader.Get("nav_previous.png");
		movePreviousItem.ToolTipText = Locale.GetText("Move previous");
		Items.Add(movePreviousItem);
		Items.Add(new ToolStripSeparator());
		PositionItem = new ToolStripTextBox();
		positionItem.Width = 50;
		positionItem.Text = ((bindingSource != null) ? 1 : 0).ToString();
		positionItem.Width = 50;
		positionItem.ToolTipText = Locale.GetText("Current position");
		Items.Add(positionItem);
		CountItem = new ToolStripLabel();
		countItem.ToolTipText = Locale.GetText("Total number of items");
		countItem.Text = Locale.GetText(countItemFormat, (bindingSource != null) ? bindingSource.Count : 0);
		Items.Add(countItem);
		Items.Add(new ToolStripSeparator());
		MoveNextItem = new ToolStripButton();
		moveNextItem.Image = ResourceImageLoader.Get("nav_next.png");
		moveNextItem.ToolTipText = Locale.GetText("Move next");
		Items.Add(moveNextItem);
		MoveLastItem = new ToolStripButton();
		moveLastItem.Image = ResourceImageLoader.Get("nav_end.png");
		moveLastItem.ToolTipText = Locale.GetText("Move last");
		Items.Add(moveLastItem);
		Items.Add(new ToolStripSeparator());
		AddNewItem = new ToolStripButton();
		addNewItem.Image = ResourceImageLoader.Get("nav_plus.png");
		addNewItem.ToolTipText = Locale.GetText("Add new");
		Items.Add(addNewItem);
		DeleteItem = new ToolStripButton();
		deleteItem.Image = ResourceImageLoader.Get("nav_delete.png");
		deleteItem.ToolTipText = Locale.GetText("Delete");
		Items.Add(deleteItem);
		EndInit();
	}

	public void BeginInit()
	{
		initFlag = true;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	public void EndInit()
	{
		initFlag = false;
		OnRefreshItems();
	}

	protected virtual void OnRefreshItems()
	{
		if (!initFlag)
		{
			if (this.RefreshItems != null)
			{
				this.RefreshItems(this, EventArgs.Empty);
			}
			RefreshItemsCore();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void RefreshItemsCore()
	{
		try
		{
			bool flag = bindingSource != null;
			initFlag = true;
			if (addNewItem != null)
			{
				addNewItem.Enabled = flag && bindingSource.AllowNew;
			}
			if (moveFirstItem != null)
			{
				moveFirstItem.Enabled = flag && bindingSource.Position > 0;
			}
			if (moveLastItem != null)
			{
				moveLastItem.Enabled = flag && bindingSource.Position < bindingSource.Count - 1;
			}
			if (moveNextItem != null)
			{
				moveNextItem.Enabled = flag && bindingSource.Position < bindingSource.Count - 1;
			}
			if (movePreviousItem != null)
			{
				movePreviousItem.Enabled = flag && bindingSource.Position > 0;
			}
			if (deleteItem != null)
			{
				deleteItem.Enabled = flag && bindingSource.Count != 0 && bindingSource.AllowRemove;
			}
			if (countItem != null)
			{
				countItem.Text = string.Format(countItemFormat, flag ? bindingSource.Count : 0);
				countItem.Enabled = flag && bindingSource.Count > 0;
			}
			if (positionItem != null)
			{
				positionItem.Text = $"{(flag ? (bindingSource.Position + 1) : 0)}";
				positionItem.Enabled = flag && bindingSource.Count > 0;
			}
		}
		finally
		{
			initFlag = false;
		}
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	public bool Validate()
	{
		throw new NotImplementedException();
	}

	private void AttachNewSource(BindingSource source)
	{
		if (bindingSource != null)
		{
			bindingSource.ListChanged -= OnListChanged;
			bindingSource.PositionChanged -= OnPositionChanged;
			bindingSource.AddingNew -= OnAddingNew;
		}
		bindingSource = source;
		if (bindingSource != null)
		{
			bindingSource.ListChanged += OnListChanged;
			bindingSource.PositionChanged += OnPositionChanged;
			bindingSource.AddingNew += OnAddingNew;
		}
	}

	private void OnAddNew(object sender, EventArgs e)
	{
		if (bindingSource != null)
		{
			bindingSource.AddNew();
		}
		OnRefreshItems();
	}

	private void OnAddingNew(object sender, AddingNewEventArgs e)
	{
		OnRefreshItems();
	}

	private void OnDelete(object sender, EventArgs e)
	{
		if (bindingSource != null)
		{
			bindingSource.RemoveCurrent();
		}
		OnRefreshItems();
	}

	private void OnListChanged(object sender, ListChangedEventArgs e)
	{
		OnRefreshItems();
	}

	private void OnMoveFirst(object sender, EventArgs e)
	{
		if (bindingSource != null)
		{
			bindingSource.MoveFirst();
		}
		OnRefreshItems();
	}

	private void OnMoveLast(object sender, EventArgs e)
	{
		if (bindingSource != null)
		{
			bindingSource.MoveLast();
		}
		OnRefreshItems();
	}

	private void OnMoveNext(object sender, EventArgs e)
	{
		if (bindingSource != null)
		{
			bindingSource.MoveNext();
		}
		OnRefreshItems();
	}

	private void OnMovePrevious(object sender, EventArgs e)
	{
		if (bindingSource != null)
		{
			bindingSource.MovePrevious();
		}
		OnRefreshItems();
	}

	private void OnPositionChanged(object sender, EventArgs e)
	{
		OnRefreshItems();
	}
}
