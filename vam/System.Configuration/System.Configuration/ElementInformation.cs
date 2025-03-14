using System.Collections;

namespace System.Configuration;

public sealed class ElementInformation
{
	private readonly PropertyInformation propertyInfo;

	private readonly ConfigurationElement owner;

	private readonly PropertyInformationCollection properties;

	[System.MonoTODO]
	public ICollection Errors
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool IsCollection => owner is ConfigurationElementCollection;

	public bool IsLocked => propertyInfo != null && propertyInfo.IsLocked;

	[System.MonoTODO]
	public bool IsPresent => propertyInfo != null;

	public int LineNumber => (propertyInfo != null) ? propertyInfo.LineNumber : 0;

	public string Source => (propertyInfo == null) ? null : propertyInfo.Source;

	public Type Type => (propertyInfo == null) ? owner.GetType() : propertyInfo.Type;

	public ConfigurationValidatorBase Validator => (propertyInfo == null) ? new DefaultValidator() : propertyInfo.Validator;

	public PropertyInformationCollection Properties => properties;

	internal ElementInformation(ConfigurationElement owner, PropertyInformation propertyInfo)
	{
		this.propertyInfo = propertyInfo;
		this.owner = owner;
		properties = new PropertyInformationCollection();
		foreach (ConfigurationProperty property in owner.Properties)
		{
			properties.Add(new PropertyInformation(owner, property));
		}
	}

	internal void Reset(ElementInformation parentInfo)
	{
		foreach (PropertyInformation property in Properties)
		{
			PropertyInformation parentProperty = parentInfo.Properties[property.Name];
			property.Reset(parentProperty);
		}
	}
}
