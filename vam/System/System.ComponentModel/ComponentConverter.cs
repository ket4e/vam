namespace System.ComponentModel;

public class ComponentConverter : ReferenceConverter
{
	public ComponentConverter(Type type)
		: base(type)
	{
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		return TypeDescriptor.GetProperties(value, attributes);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
