using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.ComponentModel;

[Serializable]
public class BindingList<T> : Collection<T>, IList, ICollection, IEnumerable, IBindingList, ICancelAddNew, IRaiseItemChangedEvents
{
	private bool allow_edit = true;

	private bool allow_remove = true;

	private bool allow_new;

	private bool allow_new_set;

	private bool raise_list_changed_events = true;

	private bool type_has_default_ctor;

	private bool type_raises_item_changed_events;

	private bool add_pending;

	private int pending_add_index;

	bool IBindingList.IsSorted => IsSortedCore;

	ListSortDirection IBindingList.SortDirection => SortDirectionCore;

	PropertyDescriptor IBindingList.SortProperty => SortPropertyCore;

	bool IBindingList.AllowEdit => AllowEdit;

	bool IBindingList.AllowNew => AllowNew;

	bool IBindingList.AllowRemove => AllowRemove;

	bool IBindingList.SupportsChangeNotification => SupportsChangeNotificationCore;

	bool IBindingList.SupportsSearching => SupportsSearchingCore;

	bool IBindingList.SupportsSorting => SupportsSortingCore;

	bool IRaiseItemChangedEvents.RaisesItemChangedEvents => type_raises_item_changed_events;

	public bool AllowEdit
	{
		get
		{
			return allow_edit;
		}
		set
		{
			if (allow_edit != value)
			{
				allow_edit = value;
				if (raise_list_changed_events)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
				}
			}
		}
	}

	public bool AllowNew
	{
		get
		{
			if (allow_new_set)
			{
				return allow_new;
			}
			if (type_has_default_ctor)
			{
				return true;
			}
			if (this.AddingNew != null)
			{
				return true;
			}
			return false;
		}
		set
		{
			if (AllowNew != value)
			{
				allow_new_set = true;
				allow_new = value;
				if (raise_list_changed_events)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
				}
			}
		}
	}

	public bool AllowRemove
	{
		get
		{
			return allow_remove;
		}
		set
		{
			if (allow_remove != value)
			{
				allow_remove = value;
				if (raise_list_changed_events)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
				}
			}
		}
	}

	protected virtual bool IsSortedCore => false;

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

	protected virtual ListSortDirection SortDirectionCore => ListSortDirection.Ascending;

	protected virtual PropertyDescriptor SortPropertyCore => null;

	protected virtual bool SupportsChangeNotificationCore => true;

	protected virtual bool SupportsSearchingCore => false;

	protected virtual bool SupportsSortingCore => false;

	public event AddingNewEventHandler AddingNew;

	public event ListChangedEventHandler ListChanged;

	public BindingList(IList<T> list)
		: base(list)
	{
		CheckType();
	}

	public BindingList()
	{
		CheckType();
	}

	void IBindingList.AddIndex(PropertyDescriptor index)
	{
	}

	object IBindingList.AddNew()
	{
		return AddNew();
	}

	void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
	{
		ApplySortCore(property, direction);
	}

	int IBindingList.Find(PropertyDescriptor property, object key)
	{
		return FindCore(property, key);
	}

	void IBindingList.RemoveIndex(PropertyDescriptor property)
	{
	}

	void IBindingList.RemoveSort()
	{
		RemoveSortCore();
	}

	private void CheckType()
	{
		ConstructorInfo constructor = typeof(T).GetConstructor(Type.EmptyTypes);
		type_has_default_ctor = constructor != null;
		type_raises_item_changed_events = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T));
	}

	public T AddNew()
	{
		return (T)AddNewCore();
	}

	protected virtual object AddNewCore()
	{
		if (!AllowNew)
		{
			throw new InvalidOperationException();
		}
		AddingNewEventArgs addingNewEventArgs = new AddingNewEventArgs();
		OnAddingNew(addingNewEventArgs);
		T val = (T)addingNewEventArgs.NewObject;
		if (val == null)
		{
			if (!type_has_default_ctor)
			{
				throw new InvalidOperationException();
			}
			val = (T)Activator.CreateInstance(typeof(T));
		}
		Add(val);
		pending_add_index = IndexOf(val);
		add_pending = true;
		return val;
	}

	protected virtual void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
	{
		throw new NotSupportedException();
	}

	public virtual void CancelNew(int itemIndex)
	{
		if (add_pending && itemIndex == pending_add_index)
		{
			add_pending = false;
			base.RemoveItem(itemIndex);
			if (raise_list_changed_events)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, itemIndex));
			}
		}
	}

	protected override void ClearItems()
	{
		EndNew(pending_add_index);
		base.ClearItems();
		OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
	}

	public virtual void EndNew(int itemIndex)
	{
		if (add_pending && itemIndex == pending_add_index)
		{
			add_pending = false;
		}
	}

	protected virtual int FindCore(PropertyDescriptor prop, object key)
	{
		throw new NotSupportedException();
	}

	protected override void InsertItem(int index, T item)
	{
		EndNew(pending_add_index);
		base.InsertItem(index, item);
		if (raise_list_changed_events)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}
	}

	protected virtual void OnAddingNew(AddingNewEventArgs e)
	{
		if (this.AddingNew != null)
		{
			this.AddingNew(this, e);
		}
	}

	protected virtual void OnListChanged(ListChangedEventArgs e)
	{
		if (this.ListChanged != null)
		{
			this.ListChanged(this, e);
		}
	}

	protected override void RemoveItem(int index)
	{
		if (!AllowRemove)
		{
			throw new NotSupportedException();
		}
		EndNew(pending_add_index);
		base.RemoveItem(index);
		if (raise_list_changed_events)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}
	}

	protected virtual void RemoveSortCore()
	{
		throw new NotSupportedException();
	}

	public void ResetBindings()
	{
		OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
	}

	public void ResetItem(int position)
	{
		OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, position));
	}

	protected override void SetItem(int index, T item)
	{
		base.SetItem(index, item);
		OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
	}
}
