using IKVM.Reflection.Emit;
using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class MethodImplTable : SortedTable<MethodImplTable.Record>
{
	internal struct Record : IRecord
	{
		internal int Class;

		internal int MethodBody;

		internal int MethodDeclaration;

		int IRecord.SortKey => Class;

		int IRecord.FilterKey => Class;
	}

	internal const int Index = 25;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].Class = mr.ReadTypeDef();
			records[i].MethodBody = mr.ReadMethodDefOrRef();
			records[i].MethodDeclaration = mr.ReadMethodDefOrRef();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.WriteTypeDef(records[i].Class);
			mw.WriteMethodDefOrRef(records[i].MethodBody);
			mw.WriteMethodDefOrRef(records[i].MethodDeclaration);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.WriteTypeDef().WriteMethodDefOrRef().WriteMethodDefOrRef()
			.Value;
	}

	internal void Fixup(ModuleBuilder moduleBuilder)
	{
		for (int i = 0; i < rowCount; i++)
		{
			moduleBuilder.FixupPseudoToken(ref records[i].MethodBody);
			moduleBuilder.FixupPseudoToken(ref records[i].MethodDeclaration);
		}
		Sort();
	}
}
