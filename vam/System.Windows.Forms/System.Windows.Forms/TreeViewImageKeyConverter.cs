using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class TreeViewImageKeyConverter : ImageKeyConverter
{
	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
