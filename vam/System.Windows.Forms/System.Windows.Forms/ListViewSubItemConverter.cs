using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

internal class ListViewSubItemConverter : ExpandableObjectConverter
{
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor) && value is ListViewItem.ListViewSubItem)
		{
			ListViewItem.ListViewSubItem listViewSubItem = (ListViewItem.ListViewSubItem)value;
			Type[] types = new Type[5]
			{
				typeof(ListViewItem),
				typeof(string),
				typeof(Color),
				typeof(Color),
				typeof(Font)
			};
			ConstructorInfo constructor = typeof(ListViewItem.ListViewSubItem).GetConstructor(types);
			if (constructor != null)
			{
				object[] arguments = new object[4] { listViewSubItem.Text, listViewSubItem.ForeColor, listViewSubItem.BackColor, listViewSubItem.Font };
				return new InstanceDescriptor(constructor, arguments, isComplete: true);
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
