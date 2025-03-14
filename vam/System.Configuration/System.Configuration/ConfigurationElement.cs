using System.Collections;
using System.Xml;

namespace System.Configuration;

public abstract class ConfigurationElement
{
	private string rawXml;

	private bool modified;

	private ElementMap map;

	private ConfigurationPropertyCollection keyProps;

	private ConfigurationElementCollection defaultCollection;

	private bool readOnly;

	private ElementInformation elementInfo;

	private ConfigurationElementProperty elementProperty;

	private Configuration _configuration;

	private ConfigurationLockCollection lockAllAttributesExcept;

	private ConfigurationLockCollection lockAllElementsExcept;

	private ConfigurationLockCollection lockAttributes;

	private ConfigurationLockCollection lockElements;

	private bool lockItem;

	internal Configuration Configuration
	{
		get
		{
			return _configuration;
		}
		set
		{
			_configuration = value;
		}
	}

	public ElementInformation ElementInformation
	{
		get
		{
			if (elementInfo == null)
			{
				elementInfo = new ElementInformation(this, null);
			}
			return elementInfo;
		}
	}

	internal string RawXml
	{
		get
		{
			return rawXml;
		}
		set
		{
			if (rawXml == null || value != null)
			{
				rawXml = value;
			}
		}
	}

	protected internal virtual ConfigurationElementProperty ElementProperty
	{
		get
		{
			if (elementProperty == null)
			{
				elementProperty = new ConfigurationElementProperty(ElementInformation.Validator);
			}
			return elementProperty;
		}
	}

	[System.MonoTODO]
	protected ContextInformation EvaluationContext
	{
		get
		{
			if (Configuration != null)
			{
				return Configuration.EvaluationContext;
			}
			throw new NotImplementedException();
		}
	}

	public ConfigurationLockCollection LockAllAttributesExcept
	{
		get
		{
			if (lockAllAttributesExcept == null)
			{
				lockAllAttributesExcept = new ConfigurationLockCollection(this, ConfigurationLockType.Attribute | ConfigurationLockType.Exclude);
			}
			return lockAllAttributesExcept;
		}
	}

	public ConfigurationLockCollection LockAllElementsExcept
	{
		get
		{
			if (lockAllElementsExcept == null)
			{
				lockAllElementsExcept = new ConfigurationLockCollection(this, ConfigurationLockType.Element | ConfigurationLockType.Exclude);
			}
			return lockAllElementsExcept;
		}
	}

	public ConfigurationLockCollection LockAttributes
	{
		get
		{
			if (lockAttributes == null)
			{
				lockAttributes = new ConfigurationLockCollection(this, ConfigurationLockType.Attribute);
			}
			return lockAttributes;
		}
	}

	public ConfigurationLockCollection LockElements
	{
		get
		{
			if (lockElements == null)
			{
				lockElements = new ConfigurationLockCollection(this, ConfigurationLockType.Element);
			}
			return lockElements;
		}
	}

	public bool LockItem
	{
		get
		{
			return lockItem;
		}
		set
		{
			lockItem = value;
		}
	}

	protected internal object this[ConfigurationProperty property]
	{
		get
		{
			return this[property.Name];
		}
		set
		{
			this[property.Name] = value;
		}
	}

	protected internal object this[string property_name]
	{
		get
		{
			PropertyInformation propertyInformation = ElementInformation.Properties[property_name];
			if (propertyInformation == null)
			{
				throw new InvalidOperationException("Property '" + property_name + "' not found in configuration element");
			}
			return propertyInformation.Value;
		}
		set
		{
			PropertyInformation propertyInformation = ElementInformation.Properties[property_name];
			if (propertyInformation == null)
			{
				throw new InvalidOperationException("Property '" + property_name + "' not found in configuration element");
			}
			SetPropertyValue(propertyInformation.Property, value, ignoreLocks: false);
			propertyInformation.Value = value;
			modified = true;
		}
	}

	protected internal virtual ConfigurationPropertyCollection Properties
	{
		get
		{
			if (map == null)
			{
				map = ElementMap.GetMap(GetType());
			}
			return map.Properties;
		}
	}

	internal virtual void InitFromProperty(PropertyInformation propertyInfo)
	{
		elementInfo = new ElementInformation(this, propertyInfo);
		Init();
	}

	protected internal virtual void Init()
	{
	}

	[System.MonoTODO]
	protected virtual void ListErrors(IList list)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void SetPropertyValue(ConfigurationProperty prop, object value, bool ignoreLocks)
	{
		try
		{
			prop.Validate(value);
		}
		catch (Exception ex)
		{
			throw new ConfigurationErrorsException($"The value for the property '{prop.Name}' is not valid. The error is: {ex.Message}", ex);
		}
	}

	internal ConfigurationPropertyCollection GetKeyProperties()
	{
		if (keyProps != null)
		{
			return keyProps;
		}
		ConfigurationPropertyCollection configurationPropertyCollection = new ConfigurationPropertyCollection();
		foreach (ConfigurationProperty property in Properties)
		{
			if (property.IsKey)
			{
				configurationPropertyCollection.Add(property);
			}
		}
		return keyProps = configurationPropertyCollection;
	}

	internal ConfigurationElementCollection GetDefaultCollection()
	{
		if (defaultCollection != null)
		{
			return defaultCollection;
		}
		ConfigurationProperty configurationProperty = null;
		foreach (ConfigurationProperty property in Properties)
		{
			if (property.IsDefaultCollection)
			{
				configurationProperty = property;
				break;
			}
		}
		if (configurationProperty != null)
		{
			defaultCollection = this[configurationProperty] as ConfigurationElementCollection;
		}
		return defaultCollection;
	}

	public override bool Equals(object compareTo)
	{
		if (!(compareTo is ConfigurationElement configurationElement))
		{
			return false;
		}
		if (GetType() != configurationElement.GetType())
		{
			return false;
		}
		foreach (ConfigurationProperty property in Properties)
		{
			if (!object.Equals(this[property], configurationElement[property]))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = 0;
		foreach (ConfigurationProperty property in Properties)
		{
			object obj = this[property];
			if (obj != null)
			{
				num += obj.GetHashCode();
			}
		}
		return num;
	}

	internal virtual bool HasValues()
	{
		foreach (PropertyInformation property in ElementInformation.Properties)
		{
			if (property.ValueOrigin != 0)
			{
				return true;
			}
		}
		return false;
	}

	internal virtual bool HasLocalModifications()
	{
		foreach (PropertyInformation property in ElementInformation.Properties)
		{
			if (property.ValueOrigin == PropertyValueOrigin.SetHere && property.IsModified)
			{
				return true;
			}
		}
		return false;
	}

	protected internal virtual void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
	{
		Hashtable hashtable = new Hashtable();
		reader.MoveToContent();
		while (reader.MoveToNextAttribute())
		{
			PropertyInformation propertyInformation = ElementInformation.Properties[reader.LocalName];
			if (propertyInformation == null || (serializeCollectionKey && !propertyInformation.IsKey))
			{
				if (reader.LocalName == "lockAllAttributesExcept")
				{
					LockAllAttributesExcept.SetFromList(reader.Value);
				}
				else if (reader.LocalName == "lockAllElementsExcept")
				{
					LockAllElementsExcept.SetFromList(reader.Value);
				}
				else if (reader.LocalName == "lockAttributes")
				{
					LockAttributes.SetFromList(reader.Value);
				}
				else if (reader.LocalName == "lockElements")
				{
					LockElements.SetFromList(reader.Value);
				}
				else if (reader.LocalName == "lockItem")
				{
					LockItem = reader.Value.ToLowerInvariant() == "true";
				}
				else if (!(reader.LocalName == "xmlns") && (!(this is ConfigurationSection) || !(reader.LocalName == "configSource")) && !OnDeserializeUnrecognizedAttribute(reader.LocalName, reader.Value))
				{
					throw new ConfigurationErrorsException("Unrecognized attribute '" + reader.LocalName + "'.", reader);
				}
				continue;
			}
			if (hashtable.ContainsKey(propertyInformation))
			{
				throw new ConfigurationErrorsException("The attribute '" + propertyInformation.Name + "' may only appear once in this element.", reader);
			}
			string text = null;
			try
			{
				text = reader.Value;
				ValidateValue(propertyInformation.Property, text);
				propertyInformation.SetStringValue(text);
			}
			catch (ConfigurationErrorsException)
			{
				throw;
			}
			catch (ConfigurationException)
			{
				throw;
			}
			catch (Exception ex3)
			{
				string message = $"The value for the property '{propertyInformation.Name}' is not valid. The error is: {ex3.Message}";
				throw new ConfigurationErrorsException(message, reader);
			}
			hashtable[propertyInformation] = propertyInformation.Name;
		}
		reader.MoveToElement();
		if (reader.IsEmptyElement)
		{
			reader.Skip();
		}
		else
		{
			int depth = reader.Depth;
			reader.ReadStartElement();
			reader.MoveToContent();
			do
			{
				if (reader.NodeType != XmlNodeType.Element)
				{
					reader.Skip();
					continue;
				}
				PropertyInformation propertyInformation2 = ElementInformation.Properties[reader.LocalName];
				if (propertyInformation2 == null || (serializeCollectionKey && !propertyInformation2.IsKey))
				{
					if (OnDeserializeUnrecognizedElement(reader.LocalName, reader))
					{
						continue;
					}
					if (propertyInformation2 == null)
					{
						ConfigurationElementCollection configurationElementCollection = GetDefaultCollection();
						if (configurationElementCollection != null && configurationElementCollection.OnDeserializeUnrecognizedElement(reader.LocalName, reader))
						{
							continue;
						}
					}
					throw new ConfigurationErrorsException("Unrecognized element '" + reader.LocalName + "'.", reader);
				}
				if (!propertyInformation2.IsElement)
				{
					throw new ConfigurationException("Property '" + propertyInformation2.Name + "' is not a ConfigurationElement.");
				}
				if (hashtable.Contains(propertyInformation2))
				{
					throw new ConfigurationErrorsException("The element <" + propertyInformation2.Name + "> may only appear once in this section.", reader);
				}
				ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation2.Value;
				configurationElement.DeserializeElement(reader, serializeCollectionKey);
				hashtable[propertyInformation2] = propertyInformation2.Name;
				if (depth == reader.Depth)
				{
					reader.Read();
				}
			}
			while (depth < reader.Depth);
		}
		modified = false;
		foreach (PropertyInformation property in ElementInformation.Properties)
		{
			if (string.IsNullOrEmpty(property.Name) || !property.IsRequired || hashtable.ContainsKey(property))
			{
				continue;
			}
			PropertyInformation propertyInformation4 = ElementInformation.Properties[property.Name];
			if (propertyInformation4 == null)
			{
				object obj = OnRequiredPropertyNotFound(property.Name);
				if (!object.Equals(obj, property.DefaultValue))
				{
					property.Value = obj;
					property.IsModified = false;
				}
			}
		}
		PostDeserialize();
	}

	protected virtual bool OnDeserializeUnrecognizedAttribute(string name, string value)
	{
		return false;
	}

	protected virtual bool OnDeserializeUnrecognizedElement(string element, XmlReader reader)
	{
		return false;
	}

	protected virtual object OnRequiredPropertyNotFound(string name)
	{
		throw new ConfigurationErrorsException("Required attribute '" + name + "' not found.");
	}

	protected virtual void PreSerialize(XmlWriter writer)
	{
	}

	protected virtual void PostDeserialize()
	{
	}

	protected internal virtual void InitializeDefault()
	{
	}

	protected internal virtual bool IsModified()
	{
		return modified;
	}

	protected internal virtual void SetReadOnly()
	{
		readOnly = true;
	}

	public virtual bool IsReadOnly()
	{
		return readOnly;
	}

	protected internal virtual void Reset(ConfigurationElement parentElement)
	{
		if (parentElement != null)
		{
			ElementInformation.Reset(parentElement.ElementInformation);
		}
		else
		{
			InitializeDefault();
		}
	}

	protected internal virtual void ResetModified()
	{
		modified = false;
		foreach (PropertyInformation property in ElementInformation.Properties)
		{
			property.IsModified = false;
		}
	}

	protected internal virtual bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
	{
		PreSerialize(writer);
		if (serializeCollectionKey)
		{
			ConfigurationPropertyCollection keyProperties = GetKeyProperties();
			foreach (ConfigurationProperty item in keyProperties)
			{
				writer.WriteAttributeString(item.Name, item.ConvertToString(this[item.Name]));
			}
			return keyProperties.Count > 0;
		}
		bool flag = false;
		foreach (PropertyInformation property in ElementInformation.Properties)
		{
			if (!property.IsElement && property.ValueOrigin != 0 && !object.Equals(property.Value, property.DefaultValue))
			{
				writer.WriteAttributeString(property.Name, property.GetStringValue());
				flag = true;
			}
		}
		foreach (PropertyInformation property2 in ElementInformation.Properties)
		{
			if (property2.IsElement)
			{
				ConfigurationElement configurationElement = (ConfigurationElement)property2.Value;
				if (configurationElement != null)
				{
					flag = configurationElement.SerializeToXmlElement(writer, property2.Name) || flag;
				}
			}
		}
		return flag;
	}

	protected internal virtual bool SerializeToXmlElement(XmlWriter writer, string elementName)
	{
		if (!HasValues())
		{
			return false;
		}
		if (elementName != null && elementName != string.Empty)
		{
			writer.WriteStartElement(elementName);
		}
		bool result = SerializeElement(writer, serializeCollectionKey: false);
		if (elementName != null && elementName != string.Empty)
		{
			writer.WriteEndElement();
		}
		return result;
	}

	protected internal virtual void Unmerge(ConfigurationElement source, ConfigurationElement parent, ConfigurationSaveMode updateMode)
	{
		if (parent != null && source.GetType() != parent.GetType())
		{
			throw new ConfigurationException("Can't unmerge two elements of different type");
		}
		foreach (PropertyInformation property in source.ElementInformation.Properties)
		{
			if (property.ValueOrigin == PropertyValueOrigin.Default)
			{
				continue;
			}
			PropertyInformation propertyInformation2 = ElementInformation.Properties[property.Name];
			object value = property.Value;
			if (parent == null || !parent.HasValue(property.Name))
			{
				propertyInformation2.Value = value;
			}
			else
			{
				if (value == null)
				{
					continue;
				}
				object obj = parent[property.Name];
				if (property.IsElement)
				{
					if (obj != null)
					{
						ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation2.Value;
						configurationElement.Unmerge((ConfigurationElement)value, (ConfigurationElement)obj, updateMode);
					}
					else
					{
						propertyInformation2.Value = value;
					}
				}
				else if (!object.Equals(value, obj) || updateMode == ConfigurationSaveMode.Full || (updateMode == ConfigurationSaveMode.Modified && property.ValueOrigin == PropertyValueOrigin.SetHere))
				{
					propertyInformation2.Value = value;
				}
			}
		}
	}

	internal bool HasValue(string propName)
	{
		PropertyInformation propertyInformation = ElementInformation.Properties[propName];
		return propertyInformation != null && propertyInformation.ValueOrigin != PropertyValueOrigin.Default;
	}

	internal bool IsReadFromConfig(string propName)
	{
		PropertyInformation propertyInformation = ElementInformation.Properties[propName];
		return propertyInformation != null && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere;
	}

	private void ValidateValue(ConfigurationProperty p, string value)
	{
		ConfigurationValidatorBase validator;
		if (p != null && (validator = p.Validator) != null)
		{
			if (!validator.CanValidate(p.Type))
			{
				throw new ConfigurationException($"Validator does not support type {p.Type}");
			}
			validator.Validate(p.ConvertFromString(value));
		}
	}
}
