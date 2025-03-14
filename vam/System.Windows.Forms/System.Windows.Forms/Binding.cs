using System.ComponentModel;

namespace System.Windows.Forms;

[TypeConverter(typeof(ListBindingConverter))]
public class Binding
{
	private string property_name;

	private object data_source;

	private string data_member;

	private bool is_binding;

	private bool checked_isnull;

	private BindingMemberInfo binding_member_info;

	private IBindableComponent control;

	private BindingManagerBase manager;

	private PropertyDescriptor control_property;

	private PropertyDescriptor is_null_desc;

	private object data;

	private Type data_type;

	private DataSourceUpdateMode datasource_update_mode;

	private ControlUpdateMode control_update_mode;

	private object datasource_null_value = Convert.DBNull;

	private object null_value;

	private IFormatProvider format_info;

	private string format_string;

	private bool formatting_enabled;

	[DefaultValue(null)]
	public IBindableComponent BindableComponent => control;

	public BindingManagerBase BindingManagerBase => manager;

	public BindingMemberInfo BindingMemberInfo => binding_member_info;

	[DefaultValue(null)]
	public Control Control => control as Control;

	[DefaultValue(ControlUpdateMode.OnPropertyChanged)]
	public ControlUpdateMode ControlUpdateMode
	{
		get
		{
			return control_update_mode;
		}
		set
		{
			control_update_mode = value;
		}
	}

	public object DataSource => data_source;

	[DefaultValue(DataSourceUpdateMode.OnValidation)]
	public DataSourceUpdateMode DataSourceUpdateMode
	{
		get
		{
			return datasource_update_mode;
		}
		set
		{
			datasource_update_mode = value;
		}
	}

	public object DataSourceNullValue
	{
		get
		{
			return datasource_null_value;
		}
		set
		{
			datasource_null_value = value;
		}
	}

	[DefaultValue(false)]
	public bool FormattingEnabled
	{
		get
		{
			return formatting_enabled;
		}
		set
		{
			if (formatting_enabled != value)
			{
				formatting_enabled = value;
				PushData();
			}
		}
	}

	[DefaultValue(null)]
	public IFormatProvider FormatInfo
	{
		get
		{
			return format_info;
		}
		set
		{
			if (value != format_info)
			{
				format_info = value;
				if (formatting_enabled)
				{
					PushData();
				}
			}
		}
	}

	public string FormatString
	{
		get
		{
			return format_string;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (!(value == format_string))
			{
				format_string = value;
				if (formatting_enabled)
				{
					PushData();
				}
			}
		}
	}

	public bool IsBinding
	{
		get
		{
			if (manager == null || manager.IsSuspended)
			{
				return false;
			}
			return is_binding;
		}
	}

	public object NullValue
	{
		get
		{
			return null_value;
		}
		set
		{
			if (value != null_value)
			{
				null_value = value;
				if (formatting_enabled)
				{
					PushData();
				}
			}
		}
	}

	[DefaultValue("")]
	public string PropertyName => property_name;

	internal string DataMember => data_member;

	public event ConvertEventHandler Format;

	public event ConvertEventHandler Parse;

	public event BindingCompleteEventHandler BindingComplete;

	public Binding(string propertyName, object dataSource, string dataMember)
		: this(propertyName, dataSource, dataMember, formattingEnabled: false, DataSourceUpdateMode.OnValidation, null, string.Empty, null)
	{
	}

	public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled)
		: this(propertyName, dataSource, dataMember, formattingEnabled, DataSourceUpdateMode.OnValidation, null, string.Empty, null)
	{
	}

	public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode)
		: this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, null, string.Empty, null)
	{
	}

	public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue)
		: this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, string.Empty, null)
	{
	}

	public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString)
		: this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, formatString, null)
	{
	}

	public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString, IFormatProvider formatInfo)
	{
		property_name = propertyName;
		data_source = dataSource;
		data_member = dataMember;
		binding_member_info = new BindingMemberInfo(dataMember);
		datasource_update_mode = dataSourceUpdateMode;
		null_value = nullValue;
		format_string = formatString;
		format_info = formatInfo;
	}

	public void ReadValue()
	{
		PushData(force: true);
	}

	public void WriteValue()
	{
		PullData(force: true);
	}

	protected virtual void OnBindingComplete(BindingCompleteEventArgs e)
	{
		if (this.BindingComplete != null)
		{
			this.BindingComplete(this, e);
		}
	}

	protected virtual void OnFormat(ConvertEventArgs cevent)
	{
		if (this.Format != null)
		{
			this.Format(this, cevent);
		}
	}

	protected virtual void OnParse(ConvertEventArgs cevent)
	{
		if (this.Parse != null)
		{
			this.Parse(this, cevent);
		}
	}

	internal void SetControl(IBindableComponent control)
	{
		if (control == this.control)
		{
			return;
		}
		control_property = TypeDescriptor.GetProperties(control).Find(property_name, ignoreCase: true);
		if (control_property == null)
		{
			throw new ArgumentException("Cannot bind to property '" + property_name + "' on target control.");
		}
		if (control_property.IsReadOnly)
		{
			throw new ArgumentException("Cannot bind to property '" + property_name + "' because it is read only.");
		}
		data_type = control_property.PropertyType;
		if (control is Control control2)
		{
			control2.Validating += ControlValidatingHandler;
			if (!control2.IsHandleCreated)
			{
				control2.HandleCreated += ControlCreatedHandler;
			}
		}
		GetPropertyChangedEvent(control, property_name)?.AddEventHandler(control, new EventHandler(ControlPropertyChangedHandler));
		this.control = control;
		UpdateIsBinding();
	}

	internal void Check()
	{
		if (control == null || control.BindingContext == null)
		{
			return;
		}
		if (manager == null)
		{
			manager = control.BindingContext[data_source, binding_member_info.BindingPath];
			if (manager.Position > -1 && binding_member_info.BindingField != string.Empty && TypeDescriptor.GetProperties(manager.Current).Find(binding_member_info.BindingField, ignoreCase: true) == null)
			{
				throw new ArgumentException("Cannot bind to property '" + binding_member_info.BindingField + "' on DataSource.", "dataMember");
			}
			manager.AddBinding(this);
			manager.PositionChanged += PositionChangedHandler;
			if (manager is PropertyManager)
			{
				GetPropertyChangedEvent(manager.Current, binding_member_info.BindingField)?.AddEventHandler(manager.Current, new EventHandler(SourcePropertyChangedHandler));
			}
		}
		if (manager.Position != -1)
		{
			if (!checked_isnull)
			{
				is_null_desc = TypeDescriptor.GetProperties(manager.Current).Find(property_name + "IsNull", ignoreCase: false);
				checked_isnull = true;
			}
			PushData();
		}
	}

	internal bool PullData()
	{
		return PullData(force: false);
	}

	private bool PullData(bool force)
	{
		if (!IsBinding || manager.Current == null)
		{
			return true;
		}
		if (!force && datasource_update_mode == DataSourceUpdateMode.Never)
		{
			return true;
		}
		data = control_property.GetValue(control);
		if (data == null)
		{
			data = datasource_null_value;
		}
		try
		{
			SetPropertyValue(data);
		}
		catch (Exception ex)
		{
			if (formatting_enabled)
			{
				FireBindingComplete(BindingCompleteContext.DataSourceUpdate, ex, ex.Message);
				return false;
			}
			throw ex;
		}
		if (formatting_enabled)
		{
			FireBindingComplete(BindingCompleteContext.DataSourceUpdate, null, null);
		}
		return true;
	}

	internal void PushData()
	{
		PushData(force: false);
	}

	private void PushData(bool force)
	{
		if (manager == null || manager.IsSuspended || manager.Count == 0 || manager.Position == -1 || (!force && control_update_mode == ControlUpdateMode.Never))
		{
			return;
		}
		if (is_null_desc != null && (bool)is_null_desc.GetValue(manager.Current))
		{
			data = Convert.DBNull;
			return;
		}
		PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(manager.Current).Find(binding_member_info.BindingField, ignoreCase: true);
		if (propertyDescriptor == null)
		{
			data = manager.Current;
		}
		else
		{
			data = propertyDescriptor.GetValue(manager.Current);
		}
		if ((data == null || data == DBNull.Value) && null_value != null)
		{
			data = null_value;
		}
		try
		{
			data = FormatData(data);
			SetControlValue(data);
		}
		catch (Exception ex)
		{
			if (formatting_enabled)
			{
				FireBindingComplete(BindingCompleteContext.ControlUpdate, ex, ex.Message);
				return;
			}
			throw ex;
		}
		if (formatting_enabled)
		{
			FireBindingComplete(BindingCompleteContext.ControlUpdate, null, null);
		}
	}

	internal void UpdateIsBinding()
	{
		is_binding = false;
		if (control != null && (!(control is Control) || ((Control)control).IsHandleCreated))
		{
			is_binding = true;
			PushData();
		}
	}

	private void SetControlValue(object data)
	{
		control_property.SetValue(control, data);
	}

	private void SetPropertyValue(object data)
	{
		PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(manager.Current).Find(binding_member_info.BindingField, ignoreCase: true);
		if (!propertyDescriptor.IsReadOnly)
		{
			data = ParseData(data, propertyDescriptor.PropertyType);
			propertyDescriptor.SetValue(manager.Current, data);
		}
	}

	private void ControlValidatingHandler(object sender, CancelEventArgs e)
	{
		if (datasource_update_mode == DataSourceUpdateMode.OnValidation)
		{
			bool flag = true;
			try
			{
				flag = PullData();
			}
			catch
			{
				flag = false;
			}
			e.Cancel = !flag;
		}
	}

	private void ControlCreatedHandler(object o, EventArgs args)
	{
		UpdateIsBinding();
	}

	private void PositionChangedHandler(object sender, EventArgs e)
	{
		Check();
		PushData();
	}

	private EventDescriptor GetPropertyChangedEvent(object o, string property_name)
	{
		if (o == null || property_name == null || property_name.Length == 0)
		{
			return null;
		}
		string text = property_name + "Changed";
		Type typeFromHandle = typeof(EventHandler);
		EventDescriptor result = null;
		foreach (EventDescriptor @event in TypeDescriptor.GetEvents(o))
		{
			if (@event.Name == text && @event.EventType == typeFromHandle)
			{
				result = @event;
				break;
			}
		}
		return result;
	}

	private void SourcePropertyChangedHandler(object o, EventArgs args)
	{
		PushData();
	}

	private void ControlPropertyChangedHandler(object o, EventArgs args)
	{
		if (datasource_update_mode == DataSourceUpdateMode.OnPropertyChanged)
		{
			PullData();
		}
	}

	private object ParseData(object data, Type data_type)
	{
		ConvertEventArgs convertEventArgs = new ConvertEventArgs(data, data_type);
		OnParse(convertEventArgs);
		if (data_type.IsInstanceOfType(convertEventArgs.Value))
		{
			return convertEventArgs.Value;
		}
		if (convertEventArgs.Value == Convert.DBNull)
		{
			return convertEventArgs.Value;
		}
		if (convertEventArgs.Value == null)
		{
			bool flag = data_type.IsGenericType && !data_type.ContainsGenericParameters && data_type.GetGenericTypeDefinition() == typeof(Nullable<>);
			return (!data_type.IsValueType || flag) ? null : Convert.DBNull;
		}
		return ConvertData(convertEventArgs.Value, data_type);
	}

	private object FormatData(object data)
	{
		ConvertEventArgs convertEventArgs = new ConvertEventArgs(data, data_type);
		OnFormat(convertEventArgs);
		if (data_type.IsInstanceOfType(convertEventArgs.Value))
		{
			return convertEventArgs.Value;
		}
		if (formatting_enabled)
		{
			if ((convertEventArgs.Value == null || convertEventArgs.Value == Convert.DBNull) && null_value != null)
			{
				return null_value;
			}
			if (convertEventArgs.Value is IFormattable && data_type == typeof(string))
			{
				IFormattable formattable = (IFormattable)convertEventArgs.Value;
				return formattable.ToString(format_string, format_info);
			}
		}
		if (convertEventArgs.Value == null && data_type == typeof(object))
		{
			return Convert.DBNull;
		}
		return ConvertData(data, data_type);
	}

	private object ConvertData(object data, Type data_type)
	{
		if (data == null)
		{
			return null;
		}
		TypeConverter converter = TypeDescriptor.GetConverter(data.GetType());
		if (converter != null && converter.CanConvertTo(data_type))
		{
			return converter.ConvertTo(data, data_type);
		}
		converter = TypeDescriptor.GetConverter(data_type);
		if (converter != null && converter.CanConvertFrom(data.GetType()))
		{
			return converter.ConvertFrom(data);
		}
		if (data is IConvertible)
		{
			object obj = Convert.ChangeType(data, data_type);
			if (data_type.IsInstanceOfType(obj))
			{
				return obj;
			}
		}
		return null;
	}

	private void FireBindingComplete(BindingCompleteContext context, Exception exc, string error_message)
	{
		BindingCompleteEventArgs bindingCompleteEventArgs = new BindingCompleteEventArgs(this, (exc != null) ? BindingCompleteState.Exception : BindingCompleteState.Success, context);
		if (exc != null)
		{
			bindingCompleteEventArgs.SetException(exc);
			bindingCompleteEventArgs.SetErrorText(error_message);
		}
		OnBindingComplete(bindingCompleteEventArgs);
	}
}
