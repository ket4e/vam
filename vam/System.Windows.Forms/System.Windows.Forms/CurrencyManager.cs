using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public class CurrencyManager : BindingManagerBase
{
	protected int listposition;

	protected Type finalType;

	private IList list;

	private bool binding_suspended;

	private object data_source;

	private bool editing;

	public IList List => list;

	public override object Current
	{
		get
		{
			if (listposition == -1 || listposition >= list.Count)
			{
				throw new IndexOutOfRangeException("list position");
			}
			return list[listposition];
		}
	}

	public override int Count => list.Count;

	public override int Position
	{
		get
		{
			return listposition;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value >= list.Count)
			{
				value = list.Count - 1;
			}
			if (listposition != value)
			{
				if (listposition != -1)
				{
					EndCurrentEdit();
				}
				listposition = value;
				OnCurrentChanged(EventArgs.Empty);
				OnPositionChanged(EventArgs.Empty);
			}
		}
	}

	internal override bool IsSuspended
	{
		get
		{
			if (Count == 0)
			{
				return true;
			}
			return binding_suspended;
		}
	}

	internal bool AllowNew
	{
		get
		{
			if (list is IBindingList)
			{
				return ((IBindingList)list).AllowNew;
			}
			if (list.IsReadOnly)
			{
				return false;
			}
			return false;
		}
	}

	internal bool AllowRemove
	{
		get
		{
			if (list.IsReadOnly)
			{
				return false;
			}
			if (list is IBindingList)
			{
				return ((IBindingList)list).AllowRemove;
			}
			return false;
		}
	}

	internal bool AllowEdit
	{
		get
		{
			if (list is IBindingList)
			{
				return ((IBindingList)list).AllowEdit;
			}
			return false;
		}
	}

	internal object this[int index] => list[index];

	public event ListChangedEventHandler ListChanged;

	public event ItemChangedEventHandler ItemChanged;

	public event EventHandler MetaDataChanged;

	internal CurrencyManager()
	{
	}

	internal CurrencyManager(object data_source)
	{
		SetDataSource(data_source);
	}

	internal void SetDataSource(object data_source)
	{
		if (this.data_source is IBindingList)
		{
			((IBindingList)this.data_source).ListChanged -= ListChangedHandler;
		}
		if (data_source is IListSource)
		{
			data_source = ((IListSource)data_source).GetList();
		}
		this.data_source = data_source;
		if (data_source != null)
		{
			finalType = data_source.GetType();
		}
		listposition = -1;
		if (this.data_source is IBindingList)
		{
			((IBindingList)this.data_source).ListChanged += ListChangedHandler;
		}
		list = (IList)data_source;
		ListChangedHandler(null, new ListChangedEventArgs(ListChangedType.Reset, -1));
	}

	public override PropertyDescriptorCollection GetItemProperties()
	{
		return ListBindingHelper.GetListItemProperties(list);
	}

	public override void RemoveAt(int index)
	{
		list.RemoveAt(index);
	}

	public override void SuspendBinding()
	{
		binding_suspended = true;
	}

	public override void ResumeBinding()
	{
		binding_suspended = false;
	}

	public override void AddNew()
	{
		if (!(list is IBindingList bindingList))
		{
			throw new NotSupportedException();
		}
		bindingList.AddNew();
		bool flag = Position != list.Count - 1;
		ChangeRecordState(list.Count - 1, flag, flag, firePositionChanged: true, pullData: true);
	}

	private void BeginEdit()
	{
		if (Current is IEditableObject editableObject)
		{
			try
			{
				editableObject.BeginEdit();
				editing = true;
			}
			catch
			{
			}
		}
	}

	public override void CancelCurrentEdit()
	{
		if (listposition != -1)
		{
			if (Current is IEditableObject editableObject)
			{
				editing = false;
				editableObject.CancelEdit();
				OnItemChanged(new ItemChangedEventArgs(Position));
			}
			if (list is ICancelAddNew)
			{
				((ICancelAddNew)list).CancelNew(listposition);
			}
		}
	}

	public override void EndCurrentEdit()
	{
		if (listposition != -1)
		{
			if (Current is IEditableObject editableObject)
			{
				editing = false;
				editableObject.EndEdit();
			}
			if (list is ICancelAddNew)
			{
				((ICancelAddNew)list).EndNew(listposition);
			}
		}
	}

	public void Refresh()
	{
		ListChangedHandler(null, new ListChangedEventArgs(ListChangedType.Reset, -1));
	}

	protected void CheckEmpty()
	{
		if (list == null || list.Count < 1)
		{
			throw new Exception("List is empty.");
		}
	}

	protected internal override void OnCurrentChanged(EventArgs e)
	{
		if (onCurrentChangedHandler != null)
		{
			onCurrentChangedHandler(this, e);
		}
		if (onCurrentItemChangedHandler != null)
		{
			onCurrentItemChangedHandler(this, e);
		}
	}

	protected override void OnCurrentItemChanged(EventArgs e)
	{
		if (onCurrentItemChangedHandler != null)
		{
			onCurrentItemChangedHandler(this, e);
		}
	}

	protected virtual void OnItemChanged(ItemChangedEventArgs e)
	{
		if (this.ItemChanged != null)
		{
			this.ItemChanged(this, e);
		}
		transfering_data = true;
		PushData();
		transfering_data = false;
	}

	private void OnListChanged(ListChangedEventArgs args)
	{
		if (this.ListChanged != null)
		{
			this.ListChanged(this, args);
		}
	}

	protected virtual void OnPositionChanged(EventArgs e)
	{
		if (onPositionChangedHandler != null)
		{
			onPositionChangedHandler(this, e);
		}
	}

	protected internal override string GetListName(ArrayList listAccessors)
	{
		if (list is ITypedList)
		{
			PropertyDescriptor[] array = null;
			if (listAccessors != null)
			{
				array = new PropertyDescriptor[listAccessors.Count];
				listAccessors.CopyTo(array, 0);
			}
			return ((ITypedList)list).GetListName(array);
		}
		if (finalType != null)
		{
			return finalType.Name;
		}
		return string.Empty;
	}

	protected override void UpdateIsBinding()
	{
		UpdateItem();
		foreach (Binding binding in base.Bindings)
		{
			binding.UpdateIsBinding();
		}
		ChangeRecordState(listposition, validating: false, endCurrentEdit: false, firePositionChanged: true, pullData: false);
		OnItemChanged(new ItemChangedEventArgs(-1));
	}

	private void ChangeRecordState(int newPosition, bool validating, bool endCurrentEdit, bool firePositionChanged, bool pullData)
	{
		if (endCurrentEdit)
		{
			EndCurrentEdit();
		}
		int num = listposition;
		listposition = newPosition;
		if (listposition >= list.Count)
		{
			listposition = list.Count - 1;
		}
		if (num != -1 && listposition != -1)
		{
			OnCurrentChanged(EventArgs.Empty);
		}
		if (firePositionChanged)
		{
			OnPositionChanged(EventArgs.Empty);
		}
	}

	private void UpdateItem()
	{
		if (!transfering_data && listposition == -1 && list.Count > 0)
		{
			listposition = 0;
			BeginEdit();
		}
	}

	private PropertyDescriptorCollection GetBrowsableProperties(Type t)
	{
		return TypeDescriptor.GetProperties(t, new Attribute[1]
		{
			new BrowsableAttribute(browsable: true)
		});
	}

	protected void OnMetaDataChanged(EventArgs e)
	{
		if (this.MetaDataChanged != null)
		{
			this.MetaDataChanged(this, e);
		}
	}

	private void ListChangedHandler(object sender, ListChangedEventArgs e)
	{
		switch (e.ListChangedType)
		{
		case ListChangedType.PropertyDescriptorAdded:
		case ListChangedType.PropertyDescriptorDeleted:
		case ListChangedType.PropertyDescriptorChanged:
			OnMetaDataChanged(EventArgs.Empty);
			OnListChanged(e);
			break;
		case ListChangedType.ItemDeleted:
			if (list.Count == 0)
			{
				listposition = -1;
				UpdateIsBinding();
				OnPositionChanged(EventArgs.Empty);
				OnCurrentChanged(EventArgs.Empty);
			}
			else if (e.NewIndex <= listposition)
			{
				ChangeRecordState(e.NewIndex, validating: false, endCurrentEdit: false, e.NewIndex != listposition, pullData: false);
			}
			OnItemChanged(new ItemChangedEventArgs(-1));
			OnListChanged(e);
			break;
		case ListChangedType.ItemAdded:
			if (list.Count == 1)
			{
				ChangeRecordState(e.NewIndex, validating: false, endCurrentEdit: false, firePositionChanged: true, pullData: false);
				OnItemChanged(new ItemChangedEventArgs(-1));
				OnListChanged(e);
			}
			else if (e.NewIndex <= listposition)
			{
				ChangeRecordState(listposition + 1, validating: false, endCurrentEdit: false, firePositionChanged: false, pullData: false);
				OnItemChanged(new ItemChangedEventArgs(-1));
				OnListChanged(e);
				OnPositionChanged(EventArgs.Empty);
			}
			else
			{
				OnItemChanged(new ItemChangedEventArgs(-1));
				OnListChanged(e);
			}
			break;
		case ListChangedType.ItemChanged:
			if (editing)
			{
				if (e.NewIndex == listposition)
				{
					OnCurrentItemChanged(EventArgs.Empty);
				}
				OnItemChanged(new ItemChangedEventArgs(e.NewIndex));
			}
			OnListChanged(e);
			break;
		case ListChangedType.Reset:
			PushData();
			UpdateIsBinding();
			OnListChanged(e);
			break;
		default:
			OnListChanged(e);
			break;
		}
	}
}
