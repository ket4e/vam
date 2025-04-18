using System.Collections.Generic;

namespace Mono.Cecil;

internal abstract class SortedTable<TRow> : MetadataTable<TRow>, IComparer<TRow> where TRow : struct
{
	public sealed override void Sort()
	{
		MergeSort<TRow>.Sort(rows, 0, length, this);
	}

	protected static int Compare(uint x, uint y)
	{
		if (x != y)
		{
			if (x <= y)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	public abstract int Compare(TRow x, TRow y);
}
