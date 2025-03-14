using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms;

[DefaultEvent("CollectionChanged")]
[Editor("System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
[TypeConverter("System.Windows.Forms.Design.ControlBindingsConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public class ControlBindingsCollection : BindingsCollection
{
	private Control control;

	private IBindableComponent bindable_component;

	private DataSourceUpdateMode default_datasource_update_mode;

	public Control Control => control;

	public Binding this[string propertyName]
	{
		get
		{
			foreach (Binding item in base.List)
			{
				if (item.PropertyName == propertyName)
				{
					return item;
				}
			}
			return null;
		}
	}

	public IBindableComponent BindableComponent => bindable_component;

	public DataSourceUpdateMode DefaultDataSourceUpdateMode
	{
		get
		{
			return default_datasource_update_mode;
		}
		set
		{
			default_datasource_update_mode = value;
		}
	}

	internal ControlBindingsCollection(Control control)
	{
		this.control = control;
		bindable_component = control;
		default_datasource_update_mode = DataSourceUpdateMode.OnValidation;
	}

	public ControlBindingsCollection(IBindableComponent control)
	{
		bindable_component = control;
		control = control as Control;
		default_datasource_update_mode = DataSourceUpdateMode.OnValidation;
	}

	public new void Add(Binding binding)
	{
		AddCore(binding);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, binding));
	}

	public Binding Add(string propertyName, object dataSource, string dataMember)
	{
		if (dataSource == null)
		{
			throw new ArgumentNullException("dataSource");
		}
		Binding binding = new Binding(propertyName, dataSource, dataMember);
		binding.DataSourceUpdateMode = default_datasource_update_mode;
		Add(binding);
		return binding;
	}

	public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled)
	{
		return Add(propertyName, dataSource, dataMember, formattingEnabled, default_datasource_update_mode, null, string.Empty, null);
	}

	public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode)
	{
		return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, null, string.Empty, null);
	}

	public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue)
	{
		return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, string.Empty, null);
	}

	public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString)
	{
		return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, formatString, null);
	}

	public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString, IFormatProvider formatInfo)
	{
		if (dataSource == null)
		{
			throw new ArgumentNullException("dataSource");
		}
		Binding binding = new Binding(propertyName, dataSource, dataMember);
		binding.FormattingEnabled = formattingEnabled;
		binding.DataSourceUpdateMode = updateMode;
		binding.NullValue = nullValue;
		binding.FormatString = formatString;
		binding.FormatInfo = formatInfo;
		Add(binding);
		return binding;
	}

	public new void Clear()
	{
		base.Clear();
	}

	public new void Remove(Binding binding)
	{
		if (binding == null)
		{
			throw new NullReferenceException("The binding is null");
		}
		base.Remove(binding);
	}

	public new void RemoveAt(int index)
	{
		if (index < 0 || index >= base.List.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		base.RemoveAt(index);
	}

	protected override void AddCore(Binding dataBinding)
	{
		if (dataBinding == null)
		{
			throw new ArgumentNullException("dataBinding");
		}
		if (dataBinding.Control != null && dataBinding.BindableComponent != bindable_component)
		{
			throw new ArgumentException("dataBinding belongs to another BindingsCollection");
		}
		for (int i = 0; i < Count; i++)
		{
			Binding binding = base[i];
			if (binding != null && binding.PropertyName.Length != 0 && dataBinding.PropertyName.Length != 0 && string.Compare(binding.PropertyName, dataBinding.PropertyName, ignoreCase: true) == 0)
			{
				throw new ArgumentException("The binding is already in the collection");
			}
		}
		dataBinding.SetControl(bindable_component);
		dataBinding.Check();
		base.AddCore(dataBinding);
	}

	protected override void ClearCore()
	{
		base.ClearCore();
	}

	protected override void RemoveCore(Binding dataBinding)
	{
		if (dataBinding == null)
		{
			throw new ArgumentNullException("dataBinding");
		}
		base.RemoveCore(dataBinding);
	}
}
