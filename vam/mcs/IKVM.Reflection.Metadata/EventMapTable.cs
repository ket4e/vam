using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class EventMapTable : SortedTable<EventMapTable.Record>
{
	internal struct Record : IRecord
	{
		internal int Parent;

		internal int EventList;

		int IRecord.SortKey => Parent;

		int IRecord.FilterKey => Parent;
	}

	internal const int Index = 18;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].Parent = mr.ReadTypeDef();
			records[i].EventList = mr.ReadEvent();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.WriteTypeDef(records[i].Parent);
			mw.WriteEvent(records[i].EventList);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.WriteTypeDef().WriteEvent().Value;
	}
}
