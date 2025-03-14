using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public class PropertyManager : BindingManagerBase
{
	internal string property_name;

	private PropertyDescriptor prop_desc;

	private object data_source;

	private EventDescriptor changed_event;

	private EventHandler property_value_changed_handler;

	public override object Current => (prop_desc != null) ? prop_desc.GetValue(data_source) : data_source;

	public override int Position
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public override int Count => 1;

	internal override bool IsSuspended => data_source == null;

	public PropertyManager()
	{
	}

	internal PropertyManager(object data_source)
	{
		SetDataSource(data_source);
	}

	internal PropertyManager(object data_source, string property_name)
	{
		this.property_name = property_name;
		SetDataSource(data_source);
	}

	internal void SetDataSource(object new_data_source)
	{
		if (changed_event != null)
		{
			changed_event.RemoveEventHandler(data_source, property_value_changed_handler);
		}
		data_source = new_data_source;
		if (property_name == null)
		{
			return;
		}
		prop_desc = TypeDescriptor.GetProperties(data_source).Find(property_name, ignoreCase: true);
		if (prop_desc != null)
		{
			changed_event = TypeDescriptor.GetEvents(data_source).Find(property_name + "Changed", ignoreCase: false);
			if (changed_event != null)
			{
				property_value_changed_handler = PropertyValueChanged;
				changed_event.AddEventHandler(data_source, property_value_changed_handler);
			}
		}
	}

	private void PropertyValueChanged(object sender, EventArgs args)
	{
		OnCurrentChanged(args);
	}

	public override void AddNew()
	{
		throw new NotSupportedException("AddNew is not supported for property to property binding");
	}

	public override void CancelCurrentEdit()
	{
		if (data_source is IEditableObject editableObject)
		{
			editableObject.CancelEdit();
			PushData();
		}
	}

	public override void EndCurrentEdit()
	{
		PullData();
		if (data_source is IEditableObject editableObject)
		{
			editableObject.EndEdit();
		}
	}

	internal override PropertyDescriptorCollection GetItemPropertiesInternal()
	{
		return TypeDescriptor.GetProperties(data_source);
	}

	public override void RemoveAt(int index)
	{
		throw new NotSupportedException("RemoveAt is not supported for property to property binding");
	}

	public override void ResumeBinding()
	{
	}

	public override void SuspendBinding()
	{
	}

	protected internal override string GetListName(ArrayList listAccessors)
	{
		return string.Empty;
	}

	[System.MonoTODO("Stub, does nothing")]
	protected override void UpdateIsBinding()
	{
	}

	protected internal override void OnCurrentChanged(EventArgs ea)
	{
		PushData();
		if (onCurrentChangedHandler != null)
		{
			onCurrentChangedHandler(this, ea);
		}
	}

	protected override void OnCurrentItemChanged(EventArgs ea)
	{
		throw new NotImplementedException();
	}
}
