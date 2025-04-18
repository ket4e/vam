using Mono.Cecil.Metadata;

namespace Mono.Cecil;

internal sealed class TypeSpecTable : MetadataTable<uint>
{
	public override void Write(TableHeapBuffer buffer)
	{
		for (int i = 0; i < length; i++)
		{
			buffer.WriteBlob(rows[i]);
		}
	}
}
