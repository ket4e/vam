using System.Collections;
using System.Globalization;

namespace System.ComponentModel.Design;

public abstract class DesignerOptionService : IDesignerOptionService
{
	[TypeConverter(typeof(TypeConverter))]
	[Editor("", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[System.MonoTODO("implement own TypeConverter")]
	public sealed class DesignerOptionCollection : IList, ICollection, IEnumerable
	{
		public sealed class WrappedPropertyDescriptor : PropertyDescriptor
		{
			private PropertyDescriptor _property;

			private object _component;

			public override AttributeCollection Attributes => _property.Attributes;

			public override bool IsReadOnly => _property.IsReadOnly;

			public override Type ComponentType => _property.ComponentType;

			public override Type PropertyType => _property.PropertyType;

			public WrappedPropertyDescriptor(PropertyDescriptor property, object component)
				: base(property.Name, new Attribute[0])
			{
				_property = property;
				_component = component;
			}

			public override object GetValue(object ignored)
			{
				return _property.GetValue(_component);
			}

			public override void SetValue(object ignored, object value)
			{
				_property.SetValue(_component, value);
			}

			public override bool CanResetValue(object ignored)
			{
				return _property.CanResetValue(_component);
			}

			public override void ResetValue(object ignored)
			{
				_property.ResetValue(_component);
			}

			public override bool ShouldSerializeValue(object ignored)
			{
				return _property.ShouldSerializeValue(_component);
			}
		}

		private string _name;

		private object _propertiesProvider;

		private DesignerOptionCollection _parent;

		private ArrayList _children;

		private DesignerOptionService _optionService;

		bool IList.IsFixedSize => true;

		bool IList.IsReadOnly => true;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		public DesignerOptionCollection this[int index] => (DesignerOptionCollection)_children[index];

		public DesignerOptionCollection this[string index]
		{
			get
			{
				foreach (DesignerOptionCollection child in _children)
				{
					if (string.Compare(child.Name, index, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
					{
						return child;
					}
				}
				return null;
			}
		}

		public string Name => _name;

		public int Count
		{
			get
			{
				if (_children != null)
				{
					return _children.Count;
				}
				return 0;
			}
		}

		public DesignerOptionCollection Parent => _parent;

		public PropertyDescriptorCollection Properties
		{
			get
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(_propertiesProvider);
				ArrayList arrayList = new ArrayList(properties.Count);
				foreach (PropertyDescriptor item in properties)
				{
					arrayList.Add(new WrappedPropertyDescriptor(item, _propertiesProvider));
				}
				PropertyDescriptor[] properties2 = (PropertyDescriptor[])arrayList.ToArray(typeof(PropertyDescriptor));
				return new PropertyDescriptorCollection(properties2);
			}
		}

		internal DesignerOptionCollection(DesignerOptionCollection parent, string name, object propertiesProvider, DesignerOptionService service)
		{
			_name = name;
			_propertiesProvider = propertiesProvider;
			_parent = parent;
			if (parent != null)
			{
				if (parent._children == null)
				{
					parent._children = new ArrayList();
				}
				parent._children.Add(this);
			}
			_children = new ArrayList();
			_optionService = service;
			service.PopulateOptionCollection(this);
		}

		bool IList.Contains(object item)
		{
			return _children.Contains(item);
		}

		int IList.IndexOf(object item)
		{
			return _children.IndexOf(item);
		}

		int IList.Add(object item)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object item)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void IList.Insert(int index, object item)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		public bool ShowDialog()
		{
			return _optionService.ShowDialog(this, _propertiesProvider);
		}

		public IEnumerator GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		public int IndexOf(DesignerOptionCollection item)
		{
			return _children.IndexOf(item);
		}

		public void CopyTo(Array array, int index)
		{
			_children.CopyTo(array, index);
		}
	}

	private DesignerOptionCollection _options;

	public DesignerOptionCollection Options
	{
		get
		{
			if (_options == null)
			{
				_options = new DesignerOptionCollection(null, string.Empty, null, this);
			}
			return _options;
		}
	}

	protected internal DesignerOptionService()
	{
	}

	object IDesignerOptionService.GetOptionValue(string pageName, string valueName)
	{
		if (pageName == null)
		{
			throw new ArgumentNullException("pageName");
		}
		if (valueName == null)
		{
			throw new ArgumentNullException("valueName");
		}
		return GetOptionProperty(pageName, valueName)?.GetValue(null);
	}

	void IDesignerOptionService.SetOptionValue(string pageName, string valueName, object value)
	{
		if (pageName == null)
		{
			throw new ArgumentNullException("pageName");
		}
		if (valueName == null)
		{
			throw new ArgumentNullException("valueName");
		}
		GetOptionProperty(pageName, valueName)?.SetValue(null, value);
	}

	protected DesignerOptionCollection CreateOptionCollection(DesignerOptionCollection parent, string name, object value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (parent == null)
		{
			throw new ArgumentNullException("parent");
		}
		if (name == string.Empty)
		{
			throw new ArgumentException("name.Length == 0");
		}
		return new DesignerOptionCollection(parent, name, value, this);
	}

	protected virtual bool ShowDialog(DesignerOptionCollection options, object optionObject)
	{
		return false;
	}

	protected virtual void PopulateOptionCollection(DesignerOptionCollection options)
	{
	}

	private PropertyDescriptor GetOptionProperty(string pageName, string valueName)
	{
		string[] array = pageName.Split('\\');
		DesignerOptionCollection designerOptionCollection = Options;
		string[] array2 = array;
		foreach (string index in array2)
		{
			designerOptionCollection = designerOptionCollection[index];
			if (designerOptionCollection == null)
			{
				return null;
			}
		}
		return designerOptionCollection.Properties[valueName];
	}
}
