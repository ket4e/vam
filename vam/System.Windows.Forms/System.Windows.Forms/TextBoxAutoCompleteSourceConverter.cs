using System.ComponentModel;

namespace System.Windows.Forms;

internal class TextBoxAutoCompleteSourceConverter : EnumConverter
{
	public TextBoxAutoCompleteSourceConverter(Type type)
		: base(type)
	{
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		StandardValuesCollection standardValues = base.GetStandardValues(context);
		AutoCompleteSource[] array = new AutoCompleteSource[standardValues.Count];
		standardValues.CopyTo(array, 0);
		AutoCompleteSource[] values = Array.FindAll(array, (AutoCompleteSource value) => value != AutoCompleteSource.ListItems);
		return new StandardValuesCollection(values);
	}
}
