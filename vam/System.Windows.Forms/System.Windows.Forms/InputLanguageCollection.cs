using System.Collections;

namespace System.Windows.Forms;

public class InputLanguageCollection : ReadOnlyCollectionBase
{
	public InputLanguage this[int index]
	{
		get
		{
			if (index >= base.InnerList.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return base.InnerList[index] as InputLanguage;
		}
	}

	internal InputLanguageCollection(InputLanguage[] data)
	{
		base.InnerList.AddRange(data);
	}

	public bool Contains(InputLanguage value)
	{
		for (int i = 0; i < base.InnerList.Count; i++)
		{
			if (this[i].Culture == value.Culture && this[i].LayoutName == value.LayoutName)
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(InputLanguage[] array, int index)
	{
		if (base.InnerList.Count > 0)
		{
			base.InnerList.CopyTo(array, index);
		}
	}

	public int IndexOf(InputLanguage value)
	{
		for (int i = 0; i < base.InnerList.Count; i++)
		{
			if (this[i].Culture == value.Culture && this[i].LayoutName == value.LayoutName)
			{
				return i;
			}
		}
		return -1;
	}
}
