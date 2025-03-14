using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

public class ColumnHeaderConverter : ExpandableObjectConverter
{
	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor) && value is ColumnHeader)
		{
			ColumnHeader columnHeader = (ColumnHeader)value;
			if (columnHeader.ImageIndex != -1)
			{
				Type[] types = new Type[1] { typeof(int) };
				ConstructorInfo constructor = typeof(ColumnHeader).GetConstructor(types);
				if (constructor != null)
				{
					object[] arguments = new object[1] { columnHeader.ImageIndex };
					return new InstanceDescriptor(constructor, arguments, isComplete: false);
				}
			}
			else if (string.IsNullOrEmpty(columnHeader.ImageKey))
			{
				Type[] types = new Type[1] { typeof(string) };
				ConstructorInfo constructor = typeof(ColumnHeader).GetConstructor(types);
				if (constructor != null)
				{
					object[] arguments2 = new object[1] { columnHeader.ImageKey };
					return new InstanceDescriptor(constructor, arguments2, isComplete: false);
				}
			}
			else
			{
				Type[] types = Type.EmptyTypes;
				ConstructorInfo constructor = typeof(ColumnHeader).GetConstructor(types);
				if (constructor != null)
				{
					object[] arguments3 = new object[0];
					return new InstanceDescriptor(constructor, arguments3, isComplete: false);
				}
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}
}
