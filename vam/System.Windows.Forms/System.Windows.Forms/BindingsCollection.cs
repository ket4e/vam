using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

[DefaultEvent("CollectionChanged")]
public class BindingsCollection : BaseCollection
{
	public override int Count => base.Count;

	public Binding this[int index] => (Binding)base.List[index];

	protected override ArrayList List => base.List;

	public event CollectionChangeEventHandler CollectionChanged;

	public event CollectionChangeEventHandler CollectionChanging;

	internal BindingsCollection()
	{
	}

	protected internal void Add(Binding binding)
	{
		AddCore(binding);
	}

	protected virtual void AddCore(Binding dataBinding)
	{
		CollectionChangeEventArgs collectionChangeEventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Add, dataBinding);
		OnCollectionChanging(collectionChangeEventArgs);
		base.List.Add(dataBinding);
		OnCollectionChanged(collectionChangeEventArgs);
	}

	protected internal void Clear()
	{
		ClearCore();
	}

	protected virtual void ClearCore()
	{
		CollectionChangeEventArgs collectionChangeEventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
		OnCollectionChanging(collectionChangeEventArgs);
		base.List.Clear();
		OnCollectionChanged(collectionChangeEventArgs);
	}

	protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
	{
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, ccevent);
		}
	}

	protected virtual void OnCollectionChanging(CollectionChangeEventArgs e)
	{
		if (this.CollectionChanging != null)
		{
			this.CollectionChanging(this, e);
		}
	}

	protected internal void Remove(Binding binding)
	{
		RemoveCore(binding);
	}

	protected internal void RemoveAt(int index)
	{
		base.List.RemoveAt(index);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, base.List));
	}

	protected virtual void RemoveCore(Binding dataBinding)
	{
		CollectionChangeEventArgs collectionChangeEventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataBinding);
		OnCollectionChanging(collectionChangeEventArgs);
		base.List.Remove(dataBinding);
		OnCollectionChanged(collectionChangeEventArgs);
	}

	protected internal bool ShouldSerializeMyAll()
	{
		if (Count > 0)
		{
			return true;
		}
		return false;
	}

	internal bool Contains(Binding binding)
	{
		return List.Contains(binding);
	}
}
