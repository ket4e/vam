using System.Collections;

namespace System.Windows.Forms;

public class FormCollection : ReadOnlyCollectionBase
{
	public virtual Form this[int index] => (Form)base.InnerList[index];

	public virtual Form this[string name]
	{
		get
		{
			foreach (Form inner in base.InnerList)
			{
				if (inner.Name == name)
				{
					return inner;
				}
			}
			return null;
		}
	}

	internal void Add(Form form)
	{
		if (!base.InnerList.Contains(form))
		{
			base.InnerList.Add(form);
		}
	}

	internal void Remove(Form form)
	{
		base.InnerList.Remove(form);
	}
}
