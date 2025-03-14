using System.Collections;

namespace System.Windows.Forms;

internal class FileFilter
{
	private ArrayList filterArrayList = new ArrayList();

	private string filter;

	public ArrayList FilterArrayList
	{
		get
		{
			return filterArrayList;
		}
		set
		{
			filterArrayList = value;
		}
	}

	public string Filter
	{
		get
		{
			return filter;
		}
		set
		{
			filter = value;
			SplitFilter();
		}
	}

	public FileFilter()
	{
	}

	public FileFilter(string filter)
	{
		this.filter = filter;
		SplitFilter();
	}

	public static bool CheckFilter(string val)
	{
		if (val.Length == 0)
		{
			return true;
		}
		string[] array = val.Split('|');
		if (array.Length % 2 != 0)
		{
			return false;
		}
		return true;
	}

	private void SplitFilter()
	{
		filterArrayList.Clear();
		if (filter.Length != 0)
		{
			string[] array = filter.Split('|');
			for (int i = 0; i < array.Length; i += 2)
			{
				FilterStruct filterStruct = new FilterStruct(array[i], array[i + 1]);
				filterArrayList.Add(filterStruct);
			}
		}
	}
}
