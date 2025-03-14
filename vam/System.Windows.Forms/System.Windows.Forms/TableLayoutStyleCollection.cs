using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms;

[Editor("System.Windows.Forms.Design.StyleCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public abstract class TableLayoutStyleCollection : ICollection, IEnumerable, IList
{
	private ArrayList al = new ArrayList();

	private TableLayoutPanel table;

	bool IList.IsFixedSize => al.IsFixedSize;

	bool IList.IsReadOnly => al.IsReadOnly;

	object IList.this[int index]
	{
		get
		{
			return al[index];
		}
		set
		{
			if (((TableLayoutStyle)value).Owner != null)
			{
				throw new ArgumentException("Style is already owned");
			}
			((TableLayoutStyle)value).Owner = table;
			al[index] = value;
			table.PerformLayout();
		}
	}

	object ICollection.SyncRoot => al.SyncRoot;

	bool ICollection.IsSynchronized => al.IsSynchronized;

	public int Count => al.Count;

	public TableLayoutStyle this[int index]
	{
		get
		{
			return (TableLayoutStyle)((IList)this)[index];
		}
		set
		{
			((IList)this)[index] = value;
		}
	}

	internal TableLayoutStyleCollection(TableLayoutPanel table)
	{
		this.table = table;
	}

	int IList.Add(object style)
	{
		TableLayoutStyle tableLayoutStyle = (TableLayoutStyle)style;
		if (tableLayoutStyle.Owner != null)
		{
			throw new ArgumentException("Style is already owned");
		}
		tableLayoutStyle.Owner = table;
		int result = al.Add(tableLayoutStyle);
		if (table != null)
		{
			table.PerformLayout();
		}
		return result;
	}

	bool IList.Contains(object style)
	{
		return al.Contains((TableLayoutStyle)style);
	}

	int IList.IndexOf(object style)
	{
		return al.IndexOf((TableLayoutStyle)style);
	}

	void IList.Insert(int index, object style)
	{
		if (((TableLayoutStyle)style).Owner != null)
		{
			throw new ArgumentException("Style is already owned");
		}
		((TableLayoutStyle)style).Owner = table;
		al.Insert(index, (TableLayoutStyle)style);
		table.PerformLayout();
	}

	void IList.Remove(object style)
	{
		((TableLayoutStyle)style).Owner = null;
		al.Remove((TableLayoutStyle)style);
		table.PerformLayout();
	}

	void ICollection.CopyTo(Array array, int startIndex)
	{
		al.CopyTo(array, startIndex);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return al.GetEnumerator();
	}

	public int Add(TableLayoutStyle style)
	{
		return ((IList)this).Add((object)style);
	}

	public void Clear()
	{
		foreach (TableLayoutStyle item in al)
		{
			item.Owner = null;
		}
		al.Clear();
		table.PerformLayout();
	}

	public void RemoveAt(int index)
	{
		((TableLayoutStyle)al[index]).Owner = null;
		al.RemoveAt(index);
		table.PerformLayout();
	}
}
