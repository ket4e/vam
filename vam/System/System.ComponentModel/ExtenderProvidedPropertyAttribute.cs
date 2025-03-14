namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class ExtenderProvidedPropertyAttribute : Attribute
{
	private PropertyDescriptor extender;

	private IExtenderProvider extenderProvider;

	private Type receiver;

	public PropertyDescriptor ExtenderProperty => extender;

	public IExtenderProvider Provider => extenderProvider;

	public Type ReceiverType => receiver;

	internal static ExtenderProvidedPropertyAttribute CreateAttribute(PropertyDescriptor extenderProperty, IExtenderProvider provider, Type receiverType)
	{
		ExtenderProvidedPropertyAttribute extenderProvidedPropertyAttribute = new ExtenderProvidedPropertyAttribute();
		extenderProvidedPropertyAttribute.extender = extenderProperty;
		extenderProvidedPropertyAttribute.receiver = receiverType;
		extenderProvidedPropertyAttribute.extenderProvider = provider;
		return extenderProvidedPropertyAttribute;
	}

	public override bool IsDefaultAttribute()
	{
		return extender == null && extenderProvider == null && receiver == null;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ExtenderProvidedPropertyAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((ExtenderProvidedPropertyAttribute)obj).ExtenderProperty.Equals(extender) && ((ExtenderProvidedPropertyAttribute)obj).Provider.Equals(extenderProvider) && ((ExtenderProvidedPropertyAttribute)obj).ReceiverType.Equals(receiver);
	}

	public override int GetHashCode()
	{
		return extender.GetHashCode() ^ extenderProvider.GetHashCode() ^ receiver.GetHashCode();
	}
}
