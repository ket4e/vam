using IKVM.Reflection.Emit;
using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class ImplMapTable : SortedTable<ImplMapTable.Record>
{
	internal struct Record : IRecord
	{
		internal short MappingFlags;

		internal int MemberForwarded;

		internal int ImportName;

		internal int ImportScope;

		int IRecord.SortKey => MemberForwarded;

		int IRecord.FilterKey => MemberForwarded;
	}

	internal const int Index = 28;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].MappingFlags = mr.ReadInt16();
			records[i].MemberForwarded = mr.ReadMemberForwarded();
			records[i].ImportName = mr.ReadStringIndex();
			records[i].ImportScope = mr.ReadModuleRef();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.Write(records[i].MappingFlags);
			mw.WriteMemberForwarded(records[i].MemberForwarded);
			mw.WriteStringIndex(records[i].ImportName);
			mw.WriteModuleRef(records[i].ImportScope);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.AddFixed(2).WriteMemberForwarded().WriteStringIndex()
			.WriteModuleRef()
			.Value;
	}

	internal void Fixup(ModuleBuilder moduleBuilder)
	{
		for (int i = 0; i < rowCount; i++)
		{
			moduleBuilder.FixupPseudoToken(ref records[i].MemberForwarded);
		}
		Sort();
	}
}
