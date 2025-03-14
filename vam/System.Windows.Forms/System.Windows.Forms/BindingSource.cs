using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;

namespace System.Windows.Forms;

[ComplexBindingProperties("DataSource", "DataMember")]
[Designer("System.Windows.Forms.Design.BindingSourceDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("DataSource")]
[DefaultEvent("CurrentChanged")]
public class BindingSource : Component, IDisposable, ICollection, IEnumerable, IList, IComponent, IBindingList, IBindingListView, ISupportInitializeNotification, ISupportInitialize, ICancelAddNew, ITypedList, ICurrencyManagerProvider
{
	private bool is_initialized = true;

	private IList list;

	private CurrencyManager currency_manager;

	private Dictionary<string, CurrencyManager> related_currency_managers = new Dictionary<string, CurrencyManager>();

	internal Type item_type;

	private bool item_has_default_ctor;

	private bool list_is_ibinding;

	private object datasource;

	private string datamember;

	private bool raise_list_changed_events;

	private bool allow_new_set;

	private bool allow_new;

	private bool add_pending;

	private int pending_add_index;

	private string filter;

	private string sort;

	private static object AddingNewEvent;

	private static object BindingCompleteEvent;

	private static object CurrentChangedEvent;

	private static object CurrentItemChangedEvent;

	private static object DataErrorEvent;

	private static object DataMemberChangedEvent;

	private static object DataSourceChangedEvent;

	private static object ListChangedEvent;

	private static object PositionChangedEvent;

	private static object InitializedEvent;

	bool ISupportInitializeNotification.IsInitialized => is_initialized;

	[Browsable(false)]
	public virtual bool AllowEdit
	{
		get
		{
			if (list == null)
			{
				return false;
			}
			if (list.IsReadOnly)
			{
				return false;
			}
			if (list is IBindingList)
			{
				return ((IBindingList)list).AllowEdit;
			}
			return true;
		}
	}

	public virtual bool AllowNew
	{
		get
		{
			if (allow_new_set)
			{
				return allow_new;
			}
			if (list is IBindingList)
			{
				return ((IBindingList)list).AllowNew;
			}
			if (list.IsFixedSize || list.IsReadOnly || !item_has_default_ctor)
			{
				return false;
			}
			return true;
		}
		set
		{
			if (value != allow_new || !allow_new_set)
			{
				if (value && (list.IsReadOnly || list.IsFixedSize))
				{
					throw new InvalidOperationException();
				}
				allow_new_set = true;
				allow_new = value;
				if (raise_list_changed_events)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
				}
			}
		}
	}

	private bool IsAddingNewHandled => (object)base.Events[AddingNew] != null;

	[Browsable(false)]
	public virtual bool AllowRemove
	{
		get
		{
			if (list == null)
			{
				return false;
			}
			if (list.IsFixedSize || list.IsReadOnly)
			{
				return false;
			}
			if (list is IBindingList)
			{
				return ((IBindingList)list).AllowRemove;
			}
			return true;
		}
	}

	[Browsable(false)]
	public virtual int Count => list.Count;

	[Browsable(false)]
	public virtual CurrencyManager CurrencyManager => currency_manager;

	[Browsable(false)]
	public object Current
	{
		get
		{
			if (currency_manager.Count > 0)
			{
				return currency_manager.Current;
			}
			return null;
		}
	}

	[Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue("")]
	public string DataMember
	{
		get
		{
			return datamember;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (datamember != value)
			{
				datamember = value;
				ResetList();
				OnDataMemberChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(null)]
	[AttributeProvider(typeof(IListSource))]
	[RefreshProperties(RefreshProperties.Repaint)]
	public object DataSource
	{
		get
		{
			return datasource;
		}
		set
		{
			if (datasource != value)
			{
				if (datasource == null)
				{
					datamember = string.Empty;
				}
				DisconnectDataSourceEvents(datasource);
				datasource = value;
				ConnectDataSourceEvents(datasource);
				ResetList();
				OnDataSourceChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(null)]
	public virtual string Filter
	{
		get
		{
			return filter;
		}
		set
		{
			if (SupportsFiltering)
			{
				((IBindingListView)list).Filter = value;
			}
			filter = value;
		}
	}

	[Browsable(false)]
	public bool IsBindingSuspended => currency_manager.IsBindingSuspended;

	[Browsable(false)]
	public virtual bool IsFixedSize => list.IsFixedSize;

	[Browsable(false)]
	public virtual bool IsReadOnly => list.IsReadOnly;

	[Browsable(false)]
	public virtual bool IsSorted => list is IBindingList && ((IBindingList)list).IsSorted;

	[Browsable(false)]
	public virtual bool IsSynchronized => list.IsSynchronized;

	[Browsable(false)]
	public virtual object this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	[Browsable(false)]
	public IList List => list;

	[Browsable(false)]
	[DefaultValue(-1)]
	public int Position
	{
		get
		{
			return currency_manager.Position;
		}
		set
		{
			if (value >= Count)
			{
				value = Count - 1;
			}
			if (value < 0)
			{
				value = 0;
			}
			currency_manager.Position = value;
		}
	}

	[DefaultValue(true)]
	[Browsable(false)]
	public bool RaiseListChangedEvents
	{
		get
		{
			return raise_list_changed_events;
		}
		set
		{
			raise_list_changed_events = value;
		}
	}

	[DefaultValue(null)]
	public string Sort
	{
		get
		{
			return sort;
		}
		set
		{
			if (value == null || value.Length == 0)
			{
				if (list_is_ibinding && SupportsSorting)
				{
					RemoveSort();
				}
				sort = value;
				return;
			}
			if (!list_is_ibinding || !SupportsSorting)
			{
				throw new ArgumentException("value");
			}
			ProcessSortString(value);
			sort = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public virtual ListSortDescriptionCollection SortDescriptions
	{
		get
		{
			if (list is IBindingListView)
			{
				return ((IBindingListView)list).SortDescriptions;
			}
			return null;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual ListSortDirection SortDirection
	{
		get
		{
			if (list is IBindingList)
			{
				return ((IBindingList)list).SortDirection;
			}
			return ListSortDirection.Ascending;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public virtual PropertyDescriptor SortProperty
	{
		get
		{
			if (list is IBindingList)
			{
				return ((IBindingList)list).SortProperty;
			}
			return null;
		}
	}

	[Browsable(false)]
	public virtual bool SupportsAdvancedSorting => list is IBindingListView && ((IBindingListView)list).SupportsAdvancedSorting;

	[Browsable(false)]
	public virtual bool SupportsChangeNotification => true;

	[Browsable(false)]
	public virtual bool SupportsFiltering => list is IBindingListView && ((IBindingListView)list).SupportsFiltering;

	[Browsable(false)]
	public virtual bool SupportsSearching => list is IBindingList && ((IBindingList)list).SupportsSearching;

	[Browsable(false)]
	public virtual bool SupportsSorting => list is IBindingList && ((IBindingList)list).SupportsSorting;

	[Browsable(false)]
	public virtual object SyncRoot => list.SyncRoot;

	public event AddingNewEventHandler AddingNew
	{
		add
		{
			base.Events.AddHandler(AddingNewEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AddingNewEvent, value);
		}
	}

	public event BindingCompleteEventHandler BindingComplete
	{
		add
		{
			base.Events.AddHandler(BindingCompleteEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BindingCompleteEvent, value);
		}
	}

	public event EventHandler CurrentChanged
	{
		add
		{
			base.Events.AddHandler(CurrentChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CurrentChangedEvent, value);
		}
	}

	public event EventHandler CurrentItemChanged
	{
		add
		{
			base.Events.AddHandler(CurrentItemChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CurrentItemChangedEvent, value);
		}
	}

	public event BindingManagerDataErrorEventHandler DataError
	{
		add
		{
			base.Events.AddHandler(DataErrorEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DataErrorEvent, value);
		}
	}

	public event EventHandler DataMemberChanged
	{
		add
		{
			base.Events.AddHandler(DataMemberChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DataMemberChangedEvent, value);
		}
	}

	public event EventHandler DataSourceChanged
	{
		add
		{
			base.Events.AddHandler(DataSourceChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DataSourceChangedEvent, value);
		}
	}

	public event ListChangedEventHandler ListChanged
	{
		add
		{
			base.Events.AddHandler(ListChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ListChangedEvent, value);
		}
	}

	public event EventHandler PositionChanged
	{
		add
		{
			base.Events.AddHandler(PositionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PositionChangedEvent, value);
		}
	}

	event EventHandler ISupportInitializeNotification.Initialized
	{
		add
		{
			base.Events.AddHandler(InitializedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(InitializedEvent, value);
		}
	}

	public BindingSource(IContainer container)
		: this()
	{
		container.Add(this);
	}

	public BindingSource(object dataSource, string dataMember)
	{
		datasource = dataSource;
		datamember = dataMember;
		raise_list_changed_events = true;
		ResetList();
		ConnectCurrencyManager();
	}

	public BindingSource()
		: this(null, string.Empty)
	{
	}

	static BindingSource()
	{
		AddingNew = new object();
		BindingComplete = new object();
		CurrentChanged = new object();
		CurrentItemChanged = new object();
		DataError = new object();
		DataMemberChanged = new object();
		DataSourceChanged = new object();
		ListChanged = new object();
		PositionChanged = new object();
		InitializedEvent = new object();
	}

	void ICancelAddNew.CancelNew(int position)
	{
		if (add_pending && position == pending_add_index)
		{
			add_pending = false;
			list.RemoveAt(position);
			if (raise_list_changed_events && !list_is_ibinding)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
			}
		}
	}

	void ICancelAddNew.EndNew(int position)
	{
		if (add_pending && position == pending_add_index)
		{
			add_pending = false;
		}
	}

	void ISupportInitialize.BeginInit()
	{
		is_initialized = false;
	}

	void ISupportInitialize.EndInit()
	{
		if (datasource != null && datasource is ISupportInitializeNotification)
		{
			ISupportInitializeNotification supportInitializeNotification = (ISupportInitializeNotification)datasource;
			if (!supportInitializeNotification.IsInitialized)
			{
				supportInitializeNotification.Initialized += DataSourceEndInitHandler;
				return;
			}
		}
		is_initialized = true;
		ResetList();
		((EventHandler)base.Events[InitializedEvent])?.Invoke(this, EventArgs.Empty);
	}

	void IBindingList.AddIndex(PropertyDescriptor property)
	{
		if (!(list is IBindingList))
		{
			throw new NotSupportedException();
		}
		((IBindingList)list).AddIndex(property);
	}

	void IBindingList.RemoveIndex(PropertyDescriptor prop)
	{
		if (!(list is IBindingList))
		{
			throw new NotSupportedException();
		}
		((IBindingList)list).RemoveIndex(prop);
	}

	private IList GetListFromEnumerable(IEnumerable enumerable)
	{
		IList list = null;
		IEnumerator enumerator = enumerable.GetEnumerator();
		if (enumerable is string)
		{
			list = new BindingList<char>();
		}
		else
		{
			object obj = null;
			if (enumerator.MoveNext())
			{
				obj = enumerator.Current;
			}
			if (obj == null)
			{
				return null;
			}
			Type type = typeof(BindingList<>).MakeGenericType(obj.GetType());
			list = (IList)Activator.CreateInstance(type);
		}
		enumerator.Reset();
		while (enumerator.MoveNext())
		{
			list.Add(enumerator.Current);
		}
		return list;
	}

	private void ConnectCurrencyManager()
	{
		currency_manager = new CurrencyManager(this);
		currency_manager.PositionChanged += delegate(object o, EventArgs args)
		{
			OnPositionChanged(args);
		};
		currency_manager.CurrentChanged += delegate(object o, EventArgs args)
		{
			OnCurrentChanged(args);
		};
		currency_manager.BindingComplete += delegate(object o, BindingCompleteEventArgs args)
		{
			OnBindingComplete(args);
		};
		currency_manager.DataError += delegate(object o, BindingManagerDataErrorEventArgs args)
		{
			OnDataError(args);
		};
		currency_manager.CurrentChanged += delegate(object o, EventArgs args)
		{
			OnCurrentChanged(args);
		};
		currency_manager.CurrentItemChanged += delegate(object o, EventArgs args)
		{
			OnCurrentItemChanged(args);
		};
	}

	private void ResetList()
	{
		if (!is_initialized)
		{
			return;
		}
		object obj = ListBindingHelper.GetList(datasource, datamember);
		IList list;
		if (datasource == null)
		{
			list = new BindingList<object>();
		}
		else if (obj == null)
		{
			Type propertyType = ListBindingHelper.GetListItemProperties(datasource)[datamember].PropertyType;
			Type type = typeof(BindingList<>).MakeGenericType(propertyType);
			list = (IList)Activator.CreateInstance(type);
		}
		else if (obj is IList)
		{
			list = (IList)obj;
		}
		else if (obj is IEnumerable)
		{
			IList listFromEnumerable = GetListFromEnumerable((IEnumerable)obj);
			IList obj2;
			if (listFromEnumerable == null)
			{
				IList list2 = this.list;
				obj2 = list2;
			}
			else
			{
				obj2 = listFromEnumerable;
			}
			list = obj2;
		}
		else if (obj is Type)
		{
			Type type2 = typeof(BindingList<>).MakeGenericType((Type)obj);
			list = (IList)Activator.CreateInstance(type2);
		}
		else
		{
			Type type3 = typeof(BindingList<>).MakeGenericType(obj.GetType());
			list = (IList)Activator.CreateInstance(type3);
			list.Add(obj);
		}
		SetList(list);
	}

	private void SetList(IList l)
	{
		if (list is IBindingList)
		{
			((IBindingList)list).ListChanged -= IBindingListChangedHandler;
		}
		list = l;
		item_type = ListBindingHelper.GetListItemType(list);
		item_has_default_ctor = item_type.GetConstructor(Type.EmptyTypes) != null;
		list_is_ibinding = list is IBindingList;
		if (list_is_ibinding)
		{
			((IBindingList)list).ListChanged += IBindingListChangedHandler;
			if (list is IBindingListView)
			{
				((IBindingListView)list).Filter = filter;
			}
		}
		ResetBindings(metadataChanged: true);
	}

	private void ConnectDataSourceEvents(object dataSource)
	{
		if (dataSource != null && dataSource is ICurrencyManagerProvider currencyManagerProvider && currencyManagerProvider.CurrencyManager != null)
		{
			currencyManagerProvider.CurrencyManager.CurrentItemChanged += OnParentCurrencyManagerChanged;
			currencyManagerProvider.CurrencyManager.MetaDataChanged += OnParentCurrencyManagerChanged;
		}
	}

	private void OnParentCurrencyManagerChanged(object sender, EventArgs args)
	{
		ResetList();
	}

	private void DisconnectDataSourceEvents(object dataSource)
	{
		if (dataSource != null && dataSource is ICurrencyManagerProvider currencyManagerProvider && currencyManagerProvider.CurrencyManager != null)
		{
			currencyManagerProvider.CurrencyManager.CurrentItemChanged -= OnParentCurrencyManagerChanged;
			currencyManagerProvider.CurrencyManager.MetaDataChanged -= OnParentCurrencyManagerChanged;
		}
	}

	private void IBindingListChangedHandler(object o, ListChangedEventArgs args)
	{
		if (raise_list_changed_events)
		{
			OnListChanged(args);
		}
	}

	private void ProcessSortString(string sort)
	{
		sort = Regex.Replace(sort, "( )+", " ");
		string[] array = sort.Split(',');
		PropertyDescriptorCollection itemProperties = GetItemProperties(null);
		if (array.Length == 1)
		{
			ListSortDescription listSortDescription = GetListSortDescription(itemProperties, array[0]);
			ApplySort(listSortDescription.PropertyDescriptor, listSortDescription.SortDirection);
			return;
		}
		if (!SupportsAdvancedSorting)
		{
			throw new ArgumentException("value");
		}
		ListSortDescription[] array2 = new ListSortDescription[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = GetListSortDescription(itemProperties, array[i]);
		}
		ApplySort(new ListSortDescriptionCollection(array2));
	}

	private ListSortDescription GetListSortDescription(PropertyDescriptorCollection prop_descs, string property)
	{
		property = property.Trim();
		string[] array = property.Split(new char[1] { ' ' }, 2);
		string s = array[0];
		PropertyDescriptor propertyDescriptor = prop_descs[s];
		if (propertyDescriptor == null)
		{
			throw new ArgumentException("value");
		}
		ListSortDirection sortDirection = ListSortDirection.Ascending;
		if (array.Length > 1)
		{
			string strA = array[1];
			if (string.Compare(strA, "ASC", ignoreCase: true) == 0)
			{
				sortDirection = ListSortDirection.Ascending;
			}
			else
			{
				if (string.Compare(strA, "DESC", ignoreCase: true) != 0)
				{
					throw new ArgumentException("value");
				}
				sortDirection = ListSortDirection.Descending;
			}
		}
		return new ListSortDescription(propertyDescriptor, sortDirection);
	}

	public virtual int Add(object value)
	{
		if (datasource == null && this.list.Count == 0 && value != null)
		{
			Type type = typeof(BindingList<>).MakeGenericType(value.GetType());
			IList list = (IList)Activator.CreateInstance(type);
			SetList(list);
		}
		if (value != null && !item_type.IsAssignableFrom(value.GetType()))
		{
			throw new InvalidOperationException("Objects added to the list must all be of the same type.");
		}
		if (this.list.IsReadOnly)
		{
			throw new NotSupportedException("Collection is read-only.");
		}
		if (this.list.IsFixedSize)
		{
			throw new NotSupportedException("Collection has a fixed size.");
		}
		int num = this.list.Add(value);
		if (raise_list_changed_events && !list_is_ibinding)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, num));
		}
		return num;
	}

	public virtual object AddNew()
	{
		if (!AllowEdit)
		{
			throw new InvalidOperationException("Item cannot be added to a read-only or fixed-size list.");
		}
		if (!AllowNew)
		{
			throw new InvalidOperationException("AddNew is set to false.");
		}
		EndEdit();
		AddingNewEventArgs addingNewEventArgs = new AddingNewEventArgs();
		OnAddingNew(addingNewEventArgs);
		object obj = addingNewEventArgs.NewObject;
		if (obj != null)
		{
			if (!item_type.IsAssignableFrom(obj.GetType()))
			{
				throw new InvalidOperationException("Objects added to the list must all be of the same type.");
			}
		}
		else
		{
			if (list is IBindingList)
			{
				object obj2 = ((IBindingList)list).AddNew();
				add_pending = true;
				pending_add_index = list.IndexOf(obj2);
				return obj2;
			}
			if (!item_has_default_ctor)
			{
				throw new InvalidOperationException("AddNew cannot be called on '" + item_type.Name + ", since it does not have a public default ctor. Set AllowNew to true , handling AddingNew and creating the appropriate object.");
			}
			obj = Activator.CreateInstance(item_type);
		}
		int newIndex = list.Add(obj);
		if (raise_list_changed_events && !list_is_ibinding)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, newIndex));
		}
		add_pending = true;
		pending_add_index = newIndex;
		return obj;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ApplySort(PropertyDescriptor property, ListSortDirection sort)
	{
		if (!list_is_ibinding)
		{
			throw new NotSupportedException("This operation requires an IBindingList.");
		}
		IBindingList bindingList = (IBindingList)list;
		bindingList.ApplySort(property, sort);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ApplySort(ListSortDescriptionCollection sorts)
	{
		if (!(list is IBindingListView))
		{
			throw new NotSupportedException("This operation requires an IBindingListView.");
		}
		IBindingListView bindingListView = (IBindingListView)list;
		bindingListView.ApplySort(sorts);
	}

	public void CancelEdit()
	{
		currency_manager.CancelCurrentEdit();
	}

	public virtual void Clear()
	{
		if (list.IsReadOnly)
		{
			throw new NotSupportedException("Collection is read-only.");
		}
		list.Clear();
		if (raise_list_changed_events && !list_is_ibinding)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
	}

	public virtual bool Contains(object value)
	{
		return list.Contains(value);
	}

	public virtual void CopyTo(Array arr, int index)
	{
		list.CopyTo(arr, index);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	public void EndEdit()
	{
		currency_manager.EndCurrentEdit();
	}

	public int Find(string propertyName, object key)
	{
		PropertyDescriptor propertyDescriptor = GetItemProperties(null).Find(propertyName, ignoreCase: true);
		if (propertyDescriptor == null)
		{
			throw new ArgumentException("propertyName");
		}
		return Find(propertyDescriptor, key);
	}

	public virtual int Find(PropertyDescriptor prop, object key)
	{
		if (!list_is_ibinding)
		{
			throw new NotSupportedException();
		}
		return ((IBindingList)list).Find(prop, key);
	}

	public virtual IEnumerator GetEnumerator()
	{
		return List.GetEnumerator();
	}

	public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
	{
		return ListBindingHelper.GetListItemProperties(list, listAccessors);
	}

	public virtual string GetListName(PropertyDescriptor[] listAccessors)
	{
		return ListBindingHelper.GetListName(list, listAccessors);
	}

	public virtual CurrencyManager GetRelatedCurrencyManager(string dataMember)
	{
		if (dataMember == null || dataMember.Length == 0)
		{
			return currency_manager;
		}
		if (related_currency_managers.ContainsKey(dataMember))
		{
			return related_currency_managers[dataMember];
		}
		if (dataMember.IndexOf('.') != -1)
		{
			return null;
		}
		BindingSource bindingSource = new BindingSource(this, dataMember);
		related_currency_managers[dataMember] = bindingSource.CurrencyManager;
		return bindingSource.CurrencyManager;
	}

	public virtual int IndexOf(object value)
	{
		return list.IndexOf(value);
	}

	public virtual void Insert(int index, object value)
	{
		if (index < 0 || index > list.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (list.IsReadOnly || list.IsFixedSize)
		{
			throw new NotSupportedException();
		}
		if (!item_type.IsAssignableFrom(value.GetType()))
		{
			throw new ArgumentException("value");
		}
		list.Insert(index, value);
		if (raise_list_changed_events && !list_is_ibinding)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}
	}

	public void MoveFirst()
	{
		Position = 0;
	}

	public void MoveLast()
	{
		Position = Count - 1;
	}

	public void MoveNext()
	{
		Position++;
	}

	public void MovePrevious()
	{
		Position--;
	}

	protected virtual void OnAddingNew(AddingNewEventArgs e)
	{
		((AddingNewEventHandler)base.Events[AddingNew])?.Invoke(this, e);
	}

	protected virtual void OnBindingComplete(BindingCompleteEventArgs e)
	{
		((BindingCompleteEventHandler)base.Events[BindingComplete])?.Invoke(this, e);
	}

	protected virtual void OnCurrentChanged(EventArgs e)
	{
		((EventHandler)base.Events[CurrentChanged])?.Invoke(this, e);
	}

	protected virtual void OnCurrentItemChanged(EventArgs e)
	{
		((EventHandler)base.Events[CurrentItemChanged])?.Invoke(this, e);
	}

	protected virtual void OnDataError(BindingManagerDataErrorEventArgs e)
	{
		((BindingManagerDataErrorEventHandler)base.Events[DataError])?.Invoke(this, e);
	}

	protected virtual void OnDataMemberChanged(EventArgs e)
	{
		((EventHandler)base.Events[DataMemberChanged])?.Invoke(this, e);
	}

	protected virtual void OnDataSourceChanged(EventArgs e)
	{
		((EventHandler)base.Events[DataSourceChanged])?.Invoke(this, e);
	}

	protected virtual void OnListChanged(ListChangedEventArgs e)
	{
		((ListChangedEventHandler)base.Events[ListChanged])?.Invoke(this, e);
	}

	protected virtual void OnPositionChanged(EventArgs e)
	{
		((EventHandler)base.Events[PositionChanged])?.Invoke(this, e);
	}

	public virtual void Remove(object value)
	{
		if (list.IsReadOnly)
		{
			throw new NotSupportedException("Collection is read-only.");
		}
		if (list.IsFixedSize)
		{
			throw new NotSupportedException("Collection has a fixed size.");
		}
		int num = ((!list_is_ibinding) ? list.IndexOf(value) : (-1));
		list.Remove(value);
		if (num != -1 && raise_list_changed_events)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, num));
		}
	}

	public virtual void RemoveAt(int index)
	{
		if (index < 0 || index > list.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (list.IsReadOnly || list.IsFixedSize)
		{
			throw new InvalidOperationException();
		}
		list.RemoveAt(index);
		if (raise_list_changed_events && !list_is_ibinding)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}
	}

	public void RemoveCurrent()
	{
		if (Position < 0)
		{
			throw new InvalidOperationException("Cannot remove item because there is no current item.");
		}
		if (!AllowRemove)
		{
			throw new InvalidOperationException("Cannot remove item because list does not allow removal of items.");
		}
		RemoveAt(Position);
	}

	public virtual void RemoveFilter()
	{
		Filter = null;
	}

	public virtual void RemoveSort()
	{
		if (list_is_ibinding)
		{
			sort = null;
			((IBindingList)list).RemoveSort();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual void ResetAllowNew()
	{
		allow_new_set = false;
	}

	public void ResetBindings(bool metadataChanged)
	{
		if (metadataChanged)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, null));
		}
		OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
	}

	public void ResetCurrentItem()
	{
		OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, Position, -1));
	}

	public void ResetItem(int itemIndex)
	{
		OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, itemIndex, -1));
	}

	public void ResumeBinding()
	{
		currency_manager.ResumeBinding();
	}

	public void SuspendBinding()
	{
		currency_manager.SuspendBinding();
	}

	private void DataSourceEndInitHandler(object o, EventArgs args)
	{
		((ISupportInitializeNotification)datasource).Initialized -= DataSourceEndInitHandler;
		((ISupportInitialize)this).EndInit();
	}
}
