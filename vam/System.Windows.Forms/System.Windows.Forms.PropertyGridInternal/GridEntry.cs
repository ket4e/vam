using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

internal class GridEntry : GridItem, ITypeDescriptorContext, IServiceProvider
{
	private PropertyGrid property_grid;

	private bool expanded;

	private GridItemCollection grid_items;

	private GridItem parent;

	private PropertyDescriptor[] property_descriptors;

	private int top;

	private Rectangle plus_minus_bounds;

	private GridItemCollection child_griditems_cache;

	IContainer ITypeDescriptorContext.Container
	{
		get
		{
			if (PropertyOwner == null)
			{
				return null;
			}
			if (property_grid.SelectedObject is IComponent component && component.Site != null)
			{
				return component.Site.Container;
			}
			return null;
		}
	}

	object ITypeDescriptorContext.Instance
	{
		get
		{
			if (ParentEntry != null && ParentEntry.PropertyOwner != null)
			{
				return ParentEntry.PropertyOwner;
			}
			return PropertyOwner;
		}
	}

	PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
	{
		get
		{
			if (ParentEntry != null && ParentEntry.PropertyDescriptor != null)
			{
				return ParentEntry.PropertyDescriptor;
			}
			return PropertyDescriptor;
		}
	}

	public override bool Expandable
	{
		get
		{
			TypeConverter converter = GetConverter();
			if (converter == null || !converter.GetPropertiesSupported(this))
			{
				return false;
			}
			if (GetChildGridItemsCached().Count > 0)
			{
				return true;
			}
			return false;
		}
	}

	public override bool Expanded
	{
		get
		{
			return expanded;
		}
		set
		{
			if (expanded != value)
			{
				expanded = value;
				PopulateChildGridItems();
				if (value)
				{
					property_grid.OnExpandItem(this);
				}
				else
				{
					property_grid.OnCollapseItem(this);
				}
			}
		}
	}

	public override GridItemCollection GridItems
	{
		get
		{
			PopulateChildGridItems();
			return grid_items;
		}
	}

	public override GridItemType GridItemType => GridItemType.Property;

	public override string Label
	{
		get
		{
			PropertyDescriptor propertyDescriptor = PropertyDescriptor;
			if (propertyDescriptor != null)
			{
				string text = propertyDescriptor.DisplayName;
				if (propertyDescriptor.Attributes[typeof(ParenthesizePropertyNameAttribute)] is ParenthesizePropertyNameAttribute parenthesizePropertyNameAttribute && parenthesizePropertyNameAttribute.NeedParenthesis)
				{
					text = "(" + text + ")";
				}
				return text;
			}
			return string.Empty;
		}
	}

	public override GridItem Parent => parent;

	public GridEntry ParentEntry
	{
		get
		{
			if (parent != null && parent.GridItemType == GridItemType.Category)
			{
				return parent.Parent as GridEntry;
			}
			return parent as GridEntry;
		}
	}

	public override PropertyDescriptor PropertyDescriptor => (property_descriptors == null) ? null : property_descriptors[0];

	public PropertyDescriptor[] PropertyDescriptors => property_descriptors;

	public object PropertyOwner
	{
		get
		{
			object[] propertyOwners = PropertyOwners;
			if (propertyOwners != null)
			{
				return propertyOwners[0];
			}
			return null;
		}
	}

	public object[] PropertyOwners
	{
		get
		{
			if (ParentEntry == null)
			{
				return null;
			}
			object[] values = ParentEntry.Values;
			PropertyDescriptor[] propertyDescriptors = PropertyDescriptors;
			object obj = null;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] is ICustomTypeDescriptor)
				{
					obj = ((ICustomTypeDescriptor)values[i]).GetPropertyOwner(propertyDescriptors[i]);
					if (obj != null)
					{
						values[i] = obj;
					}
				}
			}
			return values;
		}
	}

	public bool HasMergedValue
	{
		get
		{
			if (!IsMerged)
			{
				return false;
			}
			object[] values = Values;
			for (int i = 0; i + 1 < values.Length; i++)
			{
				if (!object.Equals(values[i], values[i + 1]))
				{
					return false;
				}
			}
			return true;
		}
	}

	public virtual bool IsMerged => PropertyDescriptors != null && PropertyDescriptors.Length > 1;

	public virtual object[] Values
	{
		get
		{
			if (PropertyDescriptor == null || PropertyOwners == null)
			{
				return null;
			}
			if (IsMerged)
			{
				object[] propertyOwners = PropertyOwners;
				PropertyDescriptor[] propertyDescriptors = PropertyDescriptors;
				object[] array = new object[propertyOwners.Length];
				for (int i = 0; i < propertyOwners.Length; i++)
				{
					array[i] = propertyDescriptors[i].GetValue(propertyOwners[i]);
				}
				return array;
			}
			return new object[1] { Value };
		}
	}

	public override object Value
	{
		get
		{
			if (PropertyDescriptor == null || PropertyOwner == null)
			{
				return null;
			}
			return PropertyDescriptor.GetValue(PropertyOwner);
		}
	}

	public string ValueText
	{
		get
		{
			string text = null;
			try
			{
				text = ConvertToString(Value);
				if (text == null)
				{
					text = string.Empty;
				}
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}
	}

	internal int Top
	{
		get
		{
			return top;
		}
		set
		{
			if (top != value)
			{
				top = value;
			}
		}
	}

	internal Rectangle PlusMinusBounds
	{
		get
		{
			return plus_minus_bounds;
		}
		set
		{
			plus_minus_bounds = value;
		}
	}

	public ICollection AcceptedValues
	{
		get
		{
			TypeConverter converter = GetConverter();
			if (PropertyDescriptor != null && converter != null && converter.GetStandardValuesSupported(this))
			{
				ArrayList arrayList = new ArrayList();
				string text = null;
				ICollection standardValues = converter.GetStandardValues(this);
				if (standardValues != null)
				{
					foreach (object item in standardValues)
					{
						text = ConvertToString(item);
						if (text != null)
						{
							arrayList.Add(text);
						}
					}
				}
				return (arrayList.Count <= 0) ? null : arrayList;
			}
			return null;
		}
	}

	public bool HasCustomEditor => EditorStyle != UITypeEditorEditStyle.None;

	public UITypeEditorEditStyle EditorStyle
	{
		get
		{
			UITypeEditor editor = GetEditor();
			if (editor != null)
			{
				try
				{
					return editor.GetEditStyle(this);
				}
				catch
				{
				}
			}
			return UITypeEditorEditStyle.None;
		}
	}

	public bool EditorResizeable
	{
		get
		{
			if (EditorStyle == UITypeEditorEditStyle.DropDown)
			{
				UITypeEditor editor = GetEditor();
				if (editor != null && editor.IsDropDownResizable)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool HasDefaultValue
	{
		get
		{
			if (PropertyDescriptor != null)
			{
				return !PropertyDescriptor.ShouldSerializeValue(PropertyOwner);
			}
			return false;
		}
	}

	public virtual bool IsResetable => !IsReadOnly && PropertyDescriptor.CanResetValue(PropertyOwner);

	public virtual bool IsEditable
	{
		get
		{
			TypeConverter converter = GetConverter();
			if (PropertyDescriptor == null)
			{
				return false;
			}
			if (PropertyDescriptor.PropertyType.IsArray)
			{
				return false;
			}
			if (PropertyDescriptor.IsReadOnly && !ShouldCreateParentInstance)
			{
				return false;
			}
			if (converter == null || !converter.CanConvertFrom(this, typeof(string)))
			{
				return false;
			}
			if (converter.GetStandardValuesSupported(this) && converter.GetStandardValuesExclusive(this))
			{
				return false;
			}
			return true;
		}
	}

	public virtual bool IsReadOnly
	{
		get
		{
			TypeConverter converter = GetConverter();
			if (PropertyDescriptor == null || PropertyOwner == null)
			{
				return true;
			}
			if (PropertyDescriptor.IsReadOnly && (EditorStyle != UITypeEditorEditStyle.Modal || PropertyDescriptor.PropertyType.IsValueType) && !ShouldCreateParentInstance)
			{
				return true;
			}
			if (PropertyDescriptor.IsReadOnly && TypeDescriptor.GetAttributes(PropertyDescriptor.PropertyType)[typeof(ImmutableObjectAttribute)].Equals(ImmutableObjectAttribute.Yes))
			{
				return true;
			}
			if (ShouldCreateParentInstance && ParentEntry.IsReadOnly)
			{
				return true;
			}
			if (!HasCustomEditor && converter == null)
			{
				return true;
			}
			if (converter != null && !converter.GetStandardValuesSupported(this) && !converter.CanConvertFrom(this, typeof(string)) && !HasCustomEditor)
			{
				return true;
			}
			if (PropertyDescriptor.PropertyType.IsArray && !HasCustomEditor)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsPassword
	{
		get
		{
			if (PropertyDescriptor != null)
			{
				return PropertyDescriptor.Attributes.Contains(PasswordPropertyTextAttribute.Yes);
			}
			return false;
		}
	}

	public virtual bool ShouldCreateParentInstance
	{
		get
		{
			if (ParentEntry != null && ParentEntry.PropertyDescriptor != null)
			{
				TypeConverter converter = ParentEntry.GetConverter();
				if (converter != null && converter.GetCreateInstanceSupported(this))
				{
					return true;
				}
			}
			return false;
		}
	}

	public virtual bool PaintValueSupported
	{
		get
		{
			UITypeEditor editor = GetEditor();
			if (editor != null)
			{
				try
				{
					return editor.GetPaintValueSupported();
				}
				catch
				{
				}
			}
			return false;
		}
	}

	protected GridEntry(PropertyGrid propertyGrid, GridEntry parent)
	{
		if (propertyGrid == null)
		{
			throw new ArgumentNullException("propertyGrid");
		}
		property_grid = propertyGrid;
		plus_minus_bounds = new Rectangle(0, 0, 0, 0);
		top = -1;
		grid_items = new GridItemCollection();
		expanded = false;
		this.parent = parent;
		child_griditems_cache = null;
	}

	public GridEntry(PropertyGrid propertyGrid, PropertyDescriptor[] properties, GridEntry parent)
		: this(propertyGrid, parent)
	{
		if (properties == null || properties.Length == 0)
		{
			throw new ArgumentNullException("prop_desc");
		}
		property_descriptors = properties;
	}

	void ITypeDescriptorContext.OnComponentChanged()
	{
	}

	bool ITypeDescriptorContext.OnComponentChanging()
	{
		return false;
	}

	object IServiceProvider.GetService(Type serviceType)
	{
		if (property_grid.SelectedObject is IComponent component && component.Site != null)
		{
			return component.Site.GetService(serviceType);
		}
		return null;
	}

	public override bool Select()
	{
		property_grid.SelectedGridItem = this;
		return true;
	}

	public void SetParent(GridItem parent)
	{
		this.parent = parent;
	}

	private string ConvertToString(object value)
	{
		if (value is string)
		{
			return (string)value;
		}
		if (PropertyDescriptor != null && PropertyDescriptor.Converter != null && PropertyDescriptor.Converter.CanConvertTo(this, typeof(string)))
		{
			try
			{
				return PropertyDescriptor.Converter.ConvertToString(this, value);
			}
			catch
			{
				return null;
			}
		}
		return null;
	}

	public bool EditValue(IWindowsFormsEditorService service)
	{
		if (service == null)
		{
			throw new ArgumentNullException("service");
		}
		IServiceContainer serviceContainer = ((IServiceProvider)this).GetService(typeof(IServiceContainer)) as IServiceContainer;
		ServiceContainer serviceContainer2 = null;
		serviceContainer2 = ((serviceContainer == null) ? new ServiceContainer() : new ServiceContainer(serviceContainer));
		serviceContainer2.AddService(typeof(IWindowsFormsEditorService), service);
		UITypeEditor editor = GetEditor();
		if (editor != null)
		{
			try
			{
				object value = editor.EditValue(this, serviceContainer2, Value);
				string error = null;
				return SetValue(value, out error);
			}
			catch
			{
			}
		}
		return false;
	}

	private UITypeEditor GetEditor()
	{
		if (PropertyDescriptor != null)
		{
			try
			{
				if (PropertyDescriptor != null)
				{
					return (UITypeEditor)PropertyDescriptor.GetEditor(typeof(UITypeEditor));
				}
			}
			catch
			{
			}
		}
		return null;
	}

	private TypeConverter GetConverter()
	{
		if (PropertyDescriptor != null)
		{
			return PropertyDescriptor.Converter;
		}
		return null;
	}

	public bool ToggleValue()
	{
		if (IsReadOnly || (IsMerged && !HasMergedValue))
		{
			return false;
		}
		bool flag = false;
		string error = null;
		object value = Value;
		if (PropertyDescriptor.PropertyType == typeof(bool))
		{
			flag = SetValue(!(bool)value, out error);
		}
		else
		{
			TypeConverter converter = GetConverter();
			if (converter != null && converter.GetStandardValuesSupported(this))
			{
				TypeConverter.StandardValuesCollection standardValues = converter.GetStandardValues(this);
				if (standardValues != null)
				{
					for (int i = 0; i < standardValues.Count; i++)
					{
						if (value != null && value.Equals(standardValues[i]))
						{
							flag = ((i >= standardValues.Count - 1) ? SetValue(standardValues[0], out error) : SetValue(standardValues[i + 1], out error));
							break;
						}
					}
				}
			}
		}
		if (!flag && error != null)
		{
			property_grid.ShowError(error);
		}
		return flag;
	}

	public bool SetValue(object value, out string error)
	{
		error = null;
		if (IsReadOnly)
		{
			return false;
		}
		if (SetValueCore(value, out error))
		{
			InvalidateChildGridItemsCache();
			property_grid.OnPropertyValueChangedInternal(this, Value);
			return true;
		}
		return false;
	}

	protected virtual bool SetValueCore(object value, out string error)
	{
		error = null;
		TypeConverter converter = GetConverter();
		Type type = value?.GetType();
		if (type != null && PropertyDescriptor.PropertyType != null && !PropertyDescriptor.PropertyType.IsAssignableFrom(type))
		{
			bool flag = false;
			try
			{
				if (converter != null && converter.CanConvertFrom(this, type))
				{
					value = converter.ConvertFrom(this, CultureInfo.CurrentCulture, value);
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception ex)
			{
				error = ex.Message;
				flag = true;
			}
			if (flag)
			{
				string text = ConvertToString(value);
				string text2 = null;
				text2 = ((text == null) ? ("Property value of '" + PropertyDescriptor.Name + "' is not convertible to type '" + PropertyDescriptor.PropertyType.Name + "'") : ("Property value '" + text + "' of '" + PropertyDescriptor.Name + "' is not convertible to type '" + PropertyDescriptor.PropertyType.Name + "'"));
				error = text2 + Environment.NewLine + Environment.NewLine + error;
				return false;
			}
		}
		bool result = false;
		bool flag2 = false;
		object[] propertyOwners = PropertyOwners;
		PropertyDescriptor[] propertyDescriptors = PropertyDescriptors;
		for (int i = 0; i < propertyOwners.Length; i++)
		{
			object value2 = propertyDescriptors[i].GetValue(propertyOwners[i]);
			flag2 = false;
			if (!object.Equals(value2, value))
			{
				if (ShouldCreateParentInstance)
				{
					Hashtable hashtable = new Hashtable();
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(propertyOwners[i]);
					foreach (PropertyDescriptor item in properties)
					{
						if (item.Name == propertyDescriptors[i].Name)
						{
							hashtable[item.Name] = value;
						}
						else
						{
							hashtable[item.Name] = item.GetValue(propertyOwners[i]);
						}
					}
					object obj = ParentEntry.PropertyDescriptor.Converter.CreateInstance(this, hashtable);
					if (obj != null)
					{
						flag2 = ParentEntry.SetValueCore(obj, out error);
					}
				}
				else
				{
					try
					{
						propertyDescriptors[i].SetValue(propertyOwners[i], value);
					}
					catch
					{
						return false;
					}
					flag2 = ((!IsValueType(ParentEntry)) ? object.Equals(propertyDescriptors[i].GetValue(propertyOwners[i]), value) : ParentEntry.SetValueCore(propertyOwners[i], out error));
				}
			}
			if (flag2)
			{
				result = true;
			}
		}
		return result;
	}

	private bool IsValueType(GridEntry item)
	{
		if (item != null && item.PropertyDescriptor != null && (item.PropertyDescriptor.PropertyType.IsValueType || item.PropertyDescriptor.PropertyType.IsPrimitive))
		{
			return true;
		}
		return false;
	}

	public bool ResetValue()
	{
		if (IsResetable)
		{
			object[] propertyOwners = PropertyOwners;
			PropertyDescriptor[] propertyDescriptors = PropertyDescriptors;
			for (int i = 0; i < propertyOwners.Length; i++)
			{
				propertyDescriptors[i].ResetValue(propertyOwners[i]);
				if (IsValueType(ParentEntry))
				{
					string error = null;
					if (!ParentEntry.SetValueCore(propertyOwners[i], out error) && error != null)
					{
						property_grid.ShowError(error);
					}
				}
			}
			property_grid.OnPropertyValueChangedInternal(this, Value);
			return true;
		}
		return false;
	}

	public virtual void PaintValue(Graphics gfx, Rectangle rect)
	{
		UITypeEditor editor = GetEditor();
		if (editor != null)
		{
			try
			{
				editor.PaintValue(Value, gfx, rect);
			}
			catch
			{
			}
		}
	}

	protected void PopulateChildGridItems()
	{
		grid_items = GetChildGridItemsCached();
	}

	private void InvalidateChildGridItemsCache()
	{
		if (child_griditems_cache != null)
		{
			child_griditems_cache = null;
			PopulateChildGridItems();
		}
	}

	private GridItemCollection GetChildGridItemsCached()
	{
		if (child_griditems_cache == null)
		{
			child_griditems_cache = GetChildGridItems();
		}
		return child_griditems_cache;
	}

	private GridItemCollection GetChildGridItems()
	{
		object[] values = Values;
		string[] mergedPropertyNames = GetMergedPropertyNames(values);
		GridItemCollection gridItemCollection = new GridItemCollection();
		string[] array = mergedPropertyNames;
		foreach (string propertyName in array)
		{
			PropertyDescriptor[] array2 = new PropertyDescriptor[values.Length];
			for (int j = 0; j < values.Length; j++)
			{
				array2[j] = GetPropertyDescriptor(values[j], propertyName);
			}
			gridItemCollection.Add(new GridEntry(property_grid, array2, this));
		}
		return gridItemCollection;
	}

	private bool IsPropertyMergeable(PropertyDescriptor property)
	{
		if (property == null)
		{
			return false;
		}
		if (property.Attributes[typeof(MergablePropertyAttribute)] is MergablePropertyAttribute mergablePropertyAttribute && !mergablePropertyAttribute.AllowMerge)
		{
			return false;
		}
		return true;
	}

	private string[] GetMergedPropertyNames(object[] objects)
	{
		if (objects == null || objects.Length == 0)
		{
			return new string[0];
		}
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < objects.Length; i++)
		{
			if (objects[i] == null)
			{
				continue;
			}
			PropertyDescriptorCollection properties = GetProperties(objects[i], property_grid.BrowsableAttributes);
			ArrayList arrayList2 = new ArrayList();
			object obj;
			if (i == 0)
			{
				ICollection collection = properties;
				obj = collection;
			}
			else
			{
				obj = arrayList;
			}
			foreach (PropertyDescriptor item in (IEnumerable)obj)
			{
				PropertyDescriptor propertyDescriptor2 = ((i != 0) ? properties[item.Name] : item);
				if ((objects.Length <= 1 || IsPropertyMergeable(propertyDescriptor2)) && propertyDescriptor2.PropertyType == item.PropertyType)
				{
					arrayList2.Add(propertyDescriptor2);
				}
			}
			arrayList = arrayList2;
		}
		string[] array = new string[arrayList.Count];
		for (int j = 0; j < arrayList.Count; j++)
		{
			array[j] = ((PropertyDescriptor)arrayList[j]).Name;
		}
		return array;
	}

	private PropertyDescriptor GetPropertyDescriptor(object propertyOwner, string propertyName)
	{
		if (propertyOwner == null || propertyName == null)
		{
			return null;
		}
		return GetProperties(propertyOwner, property_grid.BrowsableAttributes)?[propertyName];
	}

	private PropertyDescriptorCollection GetProperties(object propertyOwner, AttributeCollection attributes)
	{
		if (propertyOwner == null || property_grid.SelectedTab == null)
		{
			return new PropertyDescriptorCollection(null);
		}
		Attribute[] array = new Attribute[attributes.Count];
		attributes.CopyTo(array, 0);
		return property_grid.SelectedTab.GetProperties(this, propertyOwner, array);
	}
}
