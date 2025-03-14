using IKVM.Reflection.Emit;
using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class GenericParamConstraintTable : SortedTable<GenericParamConstraintTable.Record>
{
	internal struct Record : IRecord
	{
		internal int Owner;

		internal int Constraint;

		int IRecord.SortKey => Owner;

		int IRecord.FilterKey => Owner;
	}

	internal const int Index = 44;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].Owner = mr.ReadGenericParam();
			records[i].Constraint = mr.ReadTypeDefOrRef();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.WriteGenericParam(records[i].Owner);
			mw.WriteTypeDefOrRef(records[i].Constraint);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.WriteGenericParam().WriteTypeDefOrRef().Value;
	}

	internal void Fixup(ModuleBuilder moduleBuilder)
	{
		int[] indexFixup = moduleBuilder.GenericParam.GetIndexFixup();
		for (int i = 0; i < rowCount; i++)
		{
			records[i].Owner = indexFixup[records[i].Owner - 1] + 1;
		}
		Sort();
	}
}
