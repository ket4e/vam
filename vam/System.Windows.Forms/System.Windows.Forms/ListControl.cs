using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public abstract class ListControl : Control
{
	private object data_source;

	private BindingMemberInfo value_member;

	private string display_member;

	private CurrencyManager data_manager;

	private BindingContext last_binding_context;

	private IFormatProvider format_info;

	private string format_string = string.Empty;

	private bool formatting_enabled;

	private static object DataSourceChangedEvent;

	private static object DisplayMemberChangedEvent;

	private static object FormatEvent;

	private static object FormatInfoChangedEvent;

	private static object FormatStringChangedEvent;

	private static object FormattingEnabledChangedEvent;

	private static object SelectedValueChangedEvent;

	private static object ValueMemberChangedEvent;

	[Browsable(false)]
	[DefaultValue(null)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IFormatProvider FormatInfo
	{
		get
		{
			return format_info;
		}
		set
		{
			if (format_info != value)
			{
				format_info = value;
				RefreshItems();
				OnFormatInfoChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue("")]
	[MergableProperty(false)]
	[Editor("System.Windows.Forms.Design.FormatStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public string FormatString
	{
		get
		{
			return format_string;
		}
		set
		{
			if (format_string != value)
			{
				format_string = value;
				RefreshItems();
				OnFormatStringChanged(EventArgs.Empty);
			}
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
				RefreshItems();
				OnFormattingEnabledChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(null)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[AttributeProvider(typeof(IListSource))]
	[MWFCategory("Data")]
	public object DataSource
	{
		get
		{
			return data_source;
		}
		set
		{
			if (data_source != value)
			{
				if (value == null)
				{
					display_member = string.Empty;
				}
				else if (!(value is IList) && !(value is IListSource))
				{
					throw new Exception("Complex DataBinding accepts as a data source either an IList or an IListSource");
				}
				data_source = value;
				ConnectToDataSource();
				OnDataSourceChanged(EventArgs.Empty);
			}
		}
	}

	[MWFCategory("Data")]
	[DefaultValue("")]
	[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string DisplayMember
	{
		get
		{
			return display_member;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (!(display_member == value))
			{
				display_member = value;
				ConnectToDataSource();
				OnDisplayMemberChanged(EventArgs.Empty);
			}
		}
	}

	public abstract int SelectedIndex { get; set; }

	[DefaultValue(null)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Bindable(BindableSupport.Yes)]
	[Browsable(false)]
	public object SelectedValue
	{
		get
		{
			if (data_manager == null || SelectedIndex == -1)
			{
				return null;
			}
			object item = data_manager[SelectedIndex];
			return FilterItemOnProperty(item, ValueMember);
		}
		set
		{
			if (data_manager == null)
			{
				return;
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			PropertyDescriptorCollection itemProperties = data_manager.GetItemProperties();
			PropertyDescriptor propertyDescriptor = itemProperties.Find(ValueMember, ignoreCase: true);
			for (int i = 0; i < data_manager.Count; i++)
			{
				if (value.Equals(propertyDescriptor.GetValue(data_manager[i])))
				{
					SelectedIndex = i;
					return;
				}
			}
			SelectedIndex = -1;
		}
	}

	[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[MWFCategory("Data")]
	[DefaultValue("")]
	public string ValueMember
	{
		get
		{
			return value_member.BindingMember;
		}
		set
		{
			BindingMemberInfo bindingMemberInfo = new BindingMemberInfo(value);
			if (!value_member.Equals(bindingMemberInfo))
			{
				value_member = bindingMemberInfo;
				if (display_member == string.Empty)
				{
					DisplayMember = value_member.BindingMember;
				}
				ConnectToDataSource();
				OnValueMemberChanged(EventArgs.Empty);
			}
		}
	}

	protected virtual bool AllowSelection => true;

	internal override bool ScaleChildrenInternal => false;

	protected CurrencyManager DataManager => data_manager;

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

	public event EventHandler DisplayMemberChanged
	{
		add
		{
			base.Events.AddHandler(DisplayMemberChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DisplayMemberChangedEvent, value);
		}
	}

	public event ListControlConvertEventHandler Format
	{
		add
		{
			base.Events.AddHandler(FormatEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FormatEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public event EventHandler FormatInfoChanged
	{
		add
		{
			base.Events.AddHandler(FormatInfoChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FormatInfoChangedEvent, value);
		}
	}

	public event EventHandler FormatStringChanged
	{
		add
		{
			base.Events.AddHandler(FormatStringChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FormatStringChangedEvent, value);
		}
	}

	public event EventHandler FormattingEnabledChanged
	{
		add
		{
			base.Events.AddHandler(FormattingEnabledChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FormattingEnabledChangedEvent, value);
		}
	}

	public event EventHandler SelectedValueChanged
	{
		add
		{
			base.Events.AddHandler(SelectedValueChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectedValueChangedEvent, value);
		}
	}

	public event EventHandler ValueMemberChanged
	{
		add
		{
			base.Events.AddHandler(ValueMemberChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ValueMemberChangedEvent, value);
		}
	}

	protected ListControl()
	{
		value_member = new BindingMemberInfo(string.Empty);
		display_member = string.Empty;
		SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, value: false);
	}

	static ListControl()
	{
		DataSourceChanged = new object();
		DisplayMemberChanged = new object();
		Format = new object();
		FormatInfoChanged = new object();
		FormatStringChanged = new object();
		FormattingEnabledChanged = new object();
		SelectedValueChanged = new object();
		ValueMemberChanged = new object();
	}

	protected object FilterItemOnProperty(object item)
	{
		return FilterItemOnProperty(item, string.Empty);
	}

	protected object FilterItemOnProperty(object item, string field)
	{
		if (item == null)
		{
			return null;
		}
		if (field == null || field == string.Empty)
		{
			return item;
		}
		PropertyDescriptor propertyDescriptor = null;
		if (data_manager != null)
		{
			PropertyDescriptorCollection itemProperties = data_manager.GetItemProperties();
			propertyDescriptor = itemProperties.Find(field, ignoreCase: true);
		}
		else
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(item);
			propertyDescriptor = properties.Find(field, ignoreCase: true);
		}
		if (propertyDescriptor == null)
		{
			return item;
		}
		return propertyDescriptor.GetValue(item);
	}

	public string GetItemText(object item)
	{
		object obj = FilterItemOnProperty(item, DisplayMember);
		if (obj == null)
		{
			obj = item;
		}
		string text = obj.ToString();
		if (FormattingEnabled)
		{
			ListControlConvertEventArgs listControlConvertEventArgs = new ListControlConvertEventArgs(text, typeof(string), item);
			OnFormat(listControlConvertEventArgs);
			if (listControlConvertEventArgs.Value.ToString() != text)
			{
				return listControlConvertEventArgs.Value.ToString();
			}
			if (obj is IFormattable)
			{
				return ((IFormattable)obj).ToString((!string.IsNullOrEmpty(FormatString)) ? FormatString : null, FormatInfo);
			}
		}
		return text;
	}

	protected override bool IsInputKey(Keys keyData)
	{
		switch (keyData)
		{
		case Keys.ShiftKey:
		case Keys.ControlKey:
		case Keys.Space:
		case Keys.PageUp:
		case Keys.PageDown:
		case Keys.End:
		case Keys.Home:
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
			return true;
		default:
			return false;
		}
	}

	protected override void OnBindingContextChanged(EventArgs e)
	{
		base.OnBindingContextChanged(e);
		if (last_binding_context == BindingContext)
		{
			return;
		}
		last_binding_context = BindingContext;
		ConnectToDataSource();
		if (DataManager != null)
		{
			SetItemsCore(DataManager.List);
			if (AllowSelection)
			{
				SelectedIndex = DataManager.Position;
			}
		}
	}

	protected virtual void OnDataSourceChanged(EventArgs e)
	{
		((EventHandler)base.Events[DataSourceChanged])?.Invoke(this, e);
	}

	protected virtual void OnDisplayMemberChanged(EventArgs e)
	{
		((EventHandler)base.Events[DisplayMemberChanged])?.Invoke(this, e);
	}

	protected virtual void OnFormat(ListControlConvertEventArgs e)
	{
		((ListControlConvertEventHandler)base.Events[Format])?.Invoke(this, e);
	}

	protected virtual void OnFormatInfoChanged(EventArgs e)
	{
		((EventHandler)base.Events[FormatInfoChanged])?.Invoke(this, e);
	}

	protected virtual void OnFormatStringChanged(EventArgs e)
	{
		((EventHandler)base.Events[FormatStringChanged])?.Invoke(this, e);
	}

	protected virtual void OnFormattingEnabledChanged(EventArgs e)
	{
		((EventHandler)base.Events[FormattingEnabledChanged])?.Invoke(this, e);
	}

	protected virtual void OnSelectedIndexChanged(EventArgs e)
	{
		if (data_manager != null && data_manager.Position != SelectedIndex)
		{
			data_manager.Position = SelectedIndex;
		}
	}

	protected virtual void OnSelectedValueChanged(EventArgs e)
	{
		((EventHandler)base.Events[SelectedValueChanged])?.Invoke(this, e);
	}

	protected virtual void OnValueMemberChanged(EventArgs e)
	{
		((EventHandler)base.Events[ValueMemberChanged])?.Invoke(this, e);
	}

	protected abstract void RefreshItem(int index);

	protected virtual void RefreshItems()
	{
	}

	protected virtual void SetItemCore(int index, object value)
	{
	}

	protected abstract void SetItemsCore(IList items);

	internal void BindDataItems()
	{
		object itemsCore;
		if (data_manager != null)
		{
			IList list = data_manager.List;
			itemsCore = list;
		}
		else
		{
			itemsCore = new object[0];
		}
		SetItemsCore((IList)itemsCore);
	}

	private void ConnectToDataSource()
	{
		if (BindingContext == null)
		{
			return;
		}
		CurrencyManager currencyManager = null;
		if (data_source != null)
		{
			currencyManager = (CurrencyManager)BindingContext[data_source];
		}
		if (currencyManager != data_manager)
		{
			if (data_manager != null)
			{
				data_manager.PositionChanged -= OnPositionChanged;
				data_manager.ItemChanged -= OnItemChanged;
			}
			if (currencyManager != null)
			{
				currencyManager.PositionChanged += OnPositionChanged;
				currencyManager.ItemChanged += OnItemChanged;
			}
			data_manager = currencyManager;
		}
	}

	private void OnItemChanged(object sender, ItemChangedEventArgs e)
	{
		if (e.Index == -1)
		{
			SetItemsCore(data_manager.List);
		}
		else
		{
			RefreshItem(e.Index);
		}
		if (AllowSelection && SelectedIndex == -1 && data_manager.Count == 1)
		{
			SelectedIndex = data_manager.Position;
		}
	}

	private void OnPositionChanged(object sender, EventArgs e)
	{
		if (AllowSelection && data_manager.Count > 1)
		{
			SelectedIndex = data_manager.Position;
		}
	}
}
