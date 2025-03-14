using IKVM.Reflection.Emit;
using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class FieldLayoutTable : SortedTable<FieldLayoutTable.Record>
{
	internal struct Record : IRecord
	{
		internal int Offset;

		internal int Field;

		int IRecord.SortKey => Field;

		int IRecord.FilterKey => Field;
	}

	internal const int Index = 16;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].Offset = mr.ReadInt32();
			records[i].Field = mr.ReadField();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.Write(records[i].Offset);
			mw.WriteField(records[i].Field);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.AddFixed(4).WriteField().Value;
	}

	internal void Fixup(ModuleBuilder moduleBuilder)
	{
		for (int i = 0; i < rowCount; i++)
		{
			records[i].Field = moduleBuilder.ResolvePseudoToken(records[i].Field) & 0xFFFFFF;
		}
		Sort();
	}
}
