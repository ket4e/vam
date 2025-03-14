using Mono.Cecil.Metadata;

namespace Mono.Cecil;

internal sealed class MemberRefTable : MetadataTable<Row<uint, uint, uint>>
{
	public override void Write(TableHeapBuffer buffer)
	{
		for (int i = 0; i < length; i++)
		{
			buffer.WriteCodedRID(rows[i].Col1, CodedIndex.MemberRefParent);
			buffer.WriteString(rows[i].Col2);
			buffer.WriteBlob(rows[i].Col3);
		}
	}
}
