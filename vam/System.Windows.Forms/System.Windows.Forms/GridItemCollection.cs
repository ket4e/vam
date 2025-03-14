using System.Collections;

namespace System.Windows.Forms;

public class GridItemCollection : ICollection, IEnumerable
{
	internal class GridItemEnumerator : IEnumerator
	{
		private int nIndex;

		private GridItemCollection collection;

		object IEnumerator.Current => collection[nIndex];

		public GridItemEnumerator(GridItemCollection coll)
		{
			collection = coll;
			nIndex = -1;
		}

		public bool MoveNext()
		{
			nIndex++;
			return nIndex < collection.Count;
		}

		public void Reset()
		{
			nIndex = -1;
		}
	}

	private SortedList list;

	public static GridItemCollection Empty = new GridItemCollection();

	bool ICollection.IsSynchronized => list.IsSynchronized;

	object ICollection.SyncRoot => list.SyncRoot;

	public int Count => list.Count;

	public GridItem this[int index]
	{
		get
		{
			if (index >= list.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return (GridItem)list.GetByIndex(index);
		}
	}

	public GridItem this[string label] => (GridItem)list[label];

	internal GridItemCollection()
	{
		list = new SortedList();
	}

	void ICollection.CopyTo(Array dest, int index)
	{
		list.CopyTo(dest, index);
	}

	internal void Add(GridItem grid_item)
	{
		string text = grid_item.Label;
		while (list.ContainsKey(text))
		{
			text += "_";
		}
		list.Add(text, grid_item);
	}

	internal void AddRange(GridItemCollection items)
	{
		foreach (GridItem item in items)
		{
			Add(item);
		}
	}

	internal int IndexOf(GridItem grid_item)
	{
		return list.IndexOfValue(grid_item);
	}

	public IEnumerator GetEnumerator()
	{
		return new GridItemEnumerator(this);
	}

	internal void Clear()
	{
		list.Clear();
	}
}
