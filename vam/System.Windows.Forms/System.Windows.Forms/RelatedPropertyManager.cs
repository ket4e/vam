using System.ComponentModel;

namespace System.Windows.Forms;

internal class RelatedPropertyManager : PropertyManager
{
	private BindingManagerBase parent;

	public RelatedPropertyManager(BindingManagerBase parent, string property_name)
	{
		this.parent = parent;
		base.property_name = property_name;
		if (parent.Position != -1)
		{
			SetDataSource(parent.Current);
		}
		parent.PositionChanged += parent_PositionChanged;
	}

	private void parent_PositionChanged(object sender, EventArgs args)
	{
		if (parent.Position == -1)
		{
			SetDataSource(null);
		}
		else
		{
			SetDataSource(parent.Current);
		}
		OnCurrentChanged(EventArgs.Empty);
	}

	public override PropertyDescriptorCollection GetItemProperties()
	{
		PropertyDescriptor propertyDescriptor = parent.GetItemProperties().Find(property_name, ignoreCase: true);
		return TypeDescriptor.GetProperties(propertyDescriptor.GetValue(parent.Current));
	}
}
