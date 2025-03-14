using System.Collections.Generic;
using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class NestedClassTable : SortedTable<NestedClassTable.Record>
{
	internal struct Record : IRecord
	{
		internal int NestedClass;

		internal int EnclosingClass;

		int IRecord.SortKey => NestedClass;

		int IRecord.FilterKey => NestedClass;
	}

	internal const int Index = 41;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].NestedClass = mr.ReadTypeDef();
			records[i].EnclosingClass = mr.ReadTypeDef();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.WriteTypeDef(records[i].NestedClass);
			mw.WriteTypeDef(records[i].EnclosingClass);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.WriteTypeDef().WriteTypeDef().Value;
	}

	internal List<int> GetNestedClasses(int enclosingClass)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < rowCount; i++)
		{
			if (records[i].EnclosingClass == enclosingClass)
			{
				list.Add(records[i].NestedClass);
			}
		}
		return list;
	}
}
