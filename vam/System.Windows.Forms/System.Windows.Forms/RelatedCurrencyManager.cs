using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms;

[DefaultMember("Item")]
internal class RelatedCurrencyManager : CurrencyManager
{
	private BindingManagerBase parent;

	private PropertyDescriptor prop_desc;

	public RelatedCurrencyManager(BindingManagerBase parent, PropertyDescriptor prop_desc)
		: base(prop_desc.GetValue(parent.Current))
	{
		this.parent = parent;
		this.prop_desc = prop_desc;
		parent.PositionChanged += parent_PositionChanged;
	}

	private void parent_PositionChanged(object sender, EventArgs args)
	{
		SetDataSource(prop_desc.GetValue(parent.Current));
	}
}
