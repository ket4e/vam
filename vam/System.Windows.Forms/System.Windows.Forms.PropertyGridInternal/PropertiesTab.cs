using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

public class PropertiesTab : PropertyTab
{
	public override string HelpKeyword => "vs.properties";

	public override string TabName => "Properties";

	public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
	{
		return GetProperties(null, component, attributes);
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
	{
		if (component == null)
		{
			return new PropertyDescriptorCollection(null);
		}
		if (attributes == null)
		{
			attributes = new Attribute[1] { BrowsableAttribute.Yes };
		}
		PropertyDescriptorCollection propertyDescriptorCollection = null;
		TypeConverter converter = TypeDescriptor.GetConverter(component);
		if (converter != null && converter.GetPropertiesSupported())
		{
			propertyDescriptorCollection = converter.GetProperties(context, component, attributes);
		}
		if (propertyDescriptorCollection == null)
		{
			propertyDescriptorCollection = TypeDescriptor.GetProperties(component, attributes);
		}
		return propertyDescriptorCollection;
	}

	public override PropertyDescriptor GetDefaultProperty(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		return TypeDescriptor.GetDefaultProperty(obj);
	}
}
