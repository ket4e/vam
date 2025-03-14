using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Configuration;

internal class ElementMap
{
	private static readonly Hashtable elementMaps = Hashtable.Synchronized(new Hashtable());

	private readonly ConfigurationPropertyCollection properties;

	private readonly ConfigurationCollectionAttribute collectionAttribute;

	public ConfigurationCollectionAttribute CollectionAttribute => collectionAttribute;

	public bool HasProperties => properties.Count > 0;

	public ConfigurationPropertyCollection Properties => properties;

	public ElementMap(Type t)
	{
		properties = new ConfigurationPropertyCollection();
		collectionAttribute = Attribute.GetCustomAttribute(t, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute;
		PropertyInfo[] array = t.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyInfo[] array2 = array;
		foreach (PropertyInfo propertyInfo in array2)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationPropertyAttribute)) is ConfigurationPropertyAttribute configurationPropertyAttribute)
			{
				string name = ((configurationPropertyAttribute.Name == null) ? propertyInfo.Name : configurationPropertyAttribute.Name);
				ConfigurationValidatorBase validation = ((!(Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationValidatorAttribute)) is ConfigurationValidatorAttribute configurationValidatorAttribute)) ? null : configurationValidatorAttribute.ValidatorInstance);
				TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TypeConverterAttribute));
				TypeConverter converter = ((typeConverterAttribute == null) ? null : ((TypeConverter)Activator.CreateInstance(Type.GetType(typeConverterAttribute.ConverterTypeName), nonPublic: true)));
				ConfigurationProperty property = new ConfigurationProperty(name, propertyInfo.PropertyType, configurationPropertyAttribute.DefaultValue, converter, validation, configurationPropertyAttribute.Options)
				{
					CollectionAttribute = (Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute)
				};
				properties.Add(property);
			}
		}
	}

	public static ElementMap GetMap(Type t)
	{
		if (elementMaps[t] is ElementMap result)
		{
			return result;
		}
		ElementMap elementMap = new ElementMap(t);
		elementMaps[t] = elementMap;
		return elementMap;
	}
}
