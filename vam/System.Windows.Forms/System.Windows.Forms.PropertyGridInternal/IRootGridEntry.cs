using System.ComponentModel;

namespace System.Windows.Forms.PropertyGridInternal;

public interface IRootGridEntry
{
	AttributeCollection BrowsableAttributes { get; set; }

	void ShowCategories(bool showCategories);

	void ResetBrowsableAttributes();
}
