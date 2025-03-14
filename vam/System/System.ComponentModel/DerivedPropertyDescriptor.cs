using System.Reflection;

namespace System.ComponentModel;

internal class DerivedPropertyDescriptor : PropertyDescriptor
{
	private bool readOnly;

	private Type componentType;

	private Type propertyType;

	private PropertyInfo prop;

	public override Type ComponentType => componentType;

	public override bool IsReadOnly => readOnly;

	public override Type PropertyType => propertyType;

	protected DerivedPropertyDescriptor(string name, Attribute[] attrs)
		: base(name, attrs)
	{
	}

	public DerivedPropertyDescriptor(string name, Attribute[] attrs, int dummy)
		: this(name, attrs)
	{
	}

	public void SetReadOnly(bool value)
	{
		readOnly = value;
	}

	public void SetComponentType(Type type)
	{
		componentType = type;
	}

	public void SetPropertyType(Type type)
	{
		propertyType = type;
	}

	public override object GetValue(object component)
	{
		if (prop == null)
		{
			prop = componentType.GetProperty(Name);
		}
		return prop.GetValue(component, null);
	}

	public override void SetValue(object component, object value)
	{
		if (prop == null)
		{
			prop = componentType.GetProperty(Name);
		}
		prop.SetValue(component, value, null);
		OnValueChanged(component, new PropertyChangedEventArgs(Name));
	}

	[System.MonoTODO]
	public override void ResetValue(object component)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public override bool CanResetValue(object component)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public override bool ShouldSerializeValue(object component)
	{
		throw new NotImplementedException();
	}
}
