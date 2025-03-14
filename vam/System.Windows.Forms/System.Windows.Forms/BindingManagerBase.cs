using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public abstract class BindingManagerBase
{
	private BindingsCollection bindings;

	internal bool transfering_data;

	protected EventHandler onCurrentChangedHandler;

	protected EventHandler onPositionChangedHandler;

	internal EventHandler onCurrentItemChangedHandler;

	public BindingsCollection Bindings
	{
		get
		{
			if (bindings == null)
			{
				bindings = new BindingsCollection();
			}
			return bindings;
		}
	}

	public abstract int Count { get; }

	public abstract object Current { get; }

	public bool IsBindingSuspended => IsSuspended;

	public abstract int Position { get; set; }

	internal virtual bool IsSuspended => false;

	public event EventHandler CurrentChanged
	{
		add
		{
			onCurrentChangedHandler = (EventHandler)Delegate.Combine(onCurrentChangedHandler, value);
		}
		remove
		{
			onCurrentChangedHandler = (EventHandler)Delegate.Remove(onCurrentChangedHandler, value);
		}
	}

	public event EventHandler PositionChanged
	{
		add
		{
			onPositionChangedHandler = (EventHandler)Delegate.Combine(onPositionChangedHandler, value);
		}
		remove
		{
			onPositionChangedHandler = (EventHandler)Delegate.Remove(onPositionChangedHandler, value);
		}
	}

	public event EventHandler CurrentItemChanged
	{
		add
		{
			onCurrentItemChangedHandler = (EventHandler)Delegate.Combine(onCurrentItemChangedHandler, value);
		}
		remove
		{
			onCurrentItemChangedHandler = (EventHandler)Delegate.Remove(onCurrentItemChangedHandler, value);
		}
	}

	public event BindingCompleteEventHandler BindingComplete;

	public event BindingManagerDataErrorEventHandler DataError;

	public BindingManagerBase()
	{
	}

	public abstract void AddNew();

	public abstract void CancelCurrentEdit();

	public abstract void EndCurrentEdit();

	public virtual PropertyDescriptorCollection GetItemProperties()
	{
		return GetItemPropertiesInternal();
	}

	internal virtual PropertyDescriptorCollection GetItemPropertiesInternal()
	{
		throw new NotImplementedException();
	}

	public abstract void RemoveAt(int index);

	public abstract void ResumeBinding();

	public abstract void SuspendBinding();

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	protected internal virtual PropertyDescriptorCollection GetItemProperties(ArrayList dataSources, ArrayList listAccessors)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	protected virtual PropertyDescriptorCollection GetItemProperties(Type listType, int offset, ArrayList dataSources, ArrayList listAccessors)
	{
		throw new NotImplementedException();
	}

	protected internal abstract string GetListName(ArrayList listAccessors);

	protected internal abstract void OnCurrentChanged(EventArgs e);

	protected void PullData()
	{
		try
		{
			if (!transfering_data)
			{
				transfering_data = true;
				UpdateIsBinding();
			}
			foreach (Binding binding in Bindings)
			{
				binding.PullData();
			}
		}
		finally
		{
			transfering_data = false;
		}
	}

	protected void PushData()
	{
		try
		{
			if (!transfering_data)
			{
				transfering_data = true;
				UpdateIsBinding();
			}
			foreach (Binding binding in Bindings)
			{
				binding.PushData();
			}
		}
		finally
		{
			transfering_data = false;
		}
	}

	protected void OnBindingComplete(BindingCompleteEventArgs args)
	{
		if (this.BindingComplete != null)
		{
			this.BindingComplete(this, args);
		}
	}

	protected abstract void OnCurrentItemChanged(EventArgs e);

	protected void OnDataError(Exception e)
	{
		if (this.DataError != null)
		{
			this.DataError(this, new BindingManagerDataErrorEventArgs(e));
		}
	}

	protected abstract void UpdateIsBinding();

	internal void AddBinding(Binding binding)
	{
		if (!Bindings.Contains(binding))
		{
			Bindings.Add(binding);
		}
	}
}
