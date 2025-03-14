using System.Collections.Specialized;

namespace System.Windows.Forms;

internal struct FilterStruct
{
	public string filterName;

	public StringCollection filters;

	public FilterStruct(string filterName, string filter)
	{
		this.filterName = filterName;
		filters = new StringCollection();
		SplitFilters(filter);
	}

	private void SplitFilters(string filter)
	{
		string[] array = filter.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			filters.Add(text.Trim());
		}
	}
}
