using System.Collections;

namespace System.Windows.Forms;

public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection
{
	public new ColumnStyle this[int index]
	{
		get
		{
			return (ColumnStyle)base[index];
		}
		set
		{
			base[index] = value;
		}
	}

	internal TableLayoutColumnStyleCollection(TableLayoutPanel panel)
		: base(panel)
	{
	}

	public int Add(ColumnStyle columnStyle)
	{
		return Add((TableLayoutStyle)columnStyle);
	}

	public bool Contains(ColumnStyle columnStyle)
	{
		return ((IList)this).Contains((object)columnStyle);
	}

	public int IndexOf(ColumnStyle columnStyle)
	{
		return ((IList)this).IndexOf((object)columnStyle);
	}

	public void Insert(int index, ColumnStyle columnStyle)
	{
		((IList)this).Insert(index, (object)columnStyle);
	}

	public void Remove(ColumnStyle columnStyle)
	{
		((IList)this).Remove((object)columnStyle);
	}
}
