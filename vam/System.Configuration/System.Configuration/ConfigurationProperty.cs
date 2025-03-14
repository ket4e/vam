using System.ComponentModel;

namespace System.Configuration;

public sealed class ConfigurationProperty
{
	internal static readonly object NoDefaultValue = new object();

	private string name;

	private Type type;

	private object default_value;

	private TypeConverter converter;

	private ConfigurationValidatorBase validation;

	private ConfigurationPropertyOptions flags;

	private string description;

	private ConfigurationCollectionAttribute collectionAttribute;

	public TypeConverter Converter => converter;

	public object DefaultValue => default_value;

	public bool IsKey => (flags & ConfigurationPropertyOptions.IsKey) != 0;

	public bool IsRequired => (flags & ConfigurationPropertyOptions.IsRequired) != 0;

	public bool IsDefaultCollection => (flags & ConfigurationPropertyOptions.IsDefaultCollection) != 0;

	public string Name => name;

	public string Description => description;

	public Type Type => type;

	public ConfigurationValidatorBase Validator => validation;

	internal bool IsElement => typeof(ConfigurationElement).IsAssignableFrom(type);

	internal ConfigurationCollectionAttribute CollectionAttribute
	{
		get
		{
			return collectionAttribute;
		}
		set
		{
			collectionAttribute = value;
		}
	}

	public ConfigurationProperty(string name, Type type)
		: this(name, type, NoDefaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
	{
	}

	public ConfigurationProperty(string name, Type type, object default_value)
		: this(name, type, default_value, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
	{
	}

	public ConfigurationProperty(string name, Type type, object default_value, ConfigurationPropertyOptions flags)
		: this(name, type, default_value, TypeDescriptor.GetConverter(type), new DefaultValidator(), flags, null)
	{
	}

	public ConfigurationProperty(string name, Type type, object default_value, TypeConverter converter, ConfigurationValidatorBase validation, ConfigurationPropertyOptions flags)
		: this(name, type, default_value, converter, validation, flags, null)
	{
	}

	public ConfigurationProperty(string name, Type type, object default_value, TypeConverter converter, ConfigurationValidatorBase validation, ConfigurationPropertyOptions flags, string description)
	{
		this.name = name;
		this.converter = ((converter == null) ? TypeDescriptor.GetConverter(type) : converter);
		if (default_value != null)
		{
			if (default_value == NoDefaultValue)
			{
				default_value = Type.GetTypeCode(type) switch
				{
					TypeCode.Object => null, 
					TypeCode.String => string.Empty, 
					_ => Activator.CreateInstance(type), 
				};
			}
			else if (!type.IsAssignableFrom(default_value.GetType()))
			{
				if (!this.converter.CanConvertFrom(default_value.GetType()))
				{
					throw new ConfigurationErrorsException($"The default value for property '{name}' has a different type than the one of the property itself: expected {type} but was {default_value.GetType()}");
				}
				default_value = this.converter.ConvertFrom(default_value);
			}
		}
		this.default_value = default_value;
		this.flags = flags;
		this.type = type;
		this.validation = ((validation == null) ? new DefaultValidator() : validation);
		this.description = description;
	}

	internal object ConvertFromString(string value)
	{
		if (converter != null)
		{
			return converter.ConvertFromInvariantString(value);
		}
		throw new NotImplementedException();
	}

	internal string ConvertToString(object value)
	{
		if (converter != null)
		{
			return converter.ConvertToInvariantString(value);
		}
		throw new NotImplementedException();
	}

	internal void Validate(object value)
	{
		if (validation != null)
		{
			validation.Validate(value);
		}
	}
}
