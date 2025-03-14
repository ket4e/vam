using System;
using IKVM.Reflection.Emit;
using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class CustomAttributeTable : SortedTable<CustomAttributeTable.Record>
{
	internal struct Record : IRecord
	{
		internal int Parent;

		internal int Type;

		internal int Value;

		int IRecord.SortKey => EncodeHasCustomAttribute(Parent);

		int IRecord.FilterKey => Parent;
	}

	internal const int Index = 12;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].Parent = mr.ReadHasCustomAttribute();
			records[i].Type = mr.ReadCustomAttributeType();
			records[i].Value = mr.ReadBlobIndex();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.WriteHasCustomAttribute(records[i].Parent);
			mw.WriteCustomAttributeType(records[i].Type);
			mw.WriteBlobIndex(records[i].Value);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.WriteHasCustomAttribute().WriteCustomAttributeType().WriteBlobIndex()
			.Value;
	}

	internal void Fixup(ModuleBuilder moduleBuilder)
	{
		int[] indexFixup = moduleBuilder.GenericParam.GetIndexFixup();
		for (int i = 0; i < rowCount; i++)
		{
			moduleBuilder.FixupPseudoToken(ref records[i].Type);
			moduleBuilder.FixupPseudoToken(ref records[i].Parent);
			if (records[i].Parent >> 24 == 42)
			{
				records[i].Parent = (42 << 24) + indexFixup[(records[i].Parent & 0xFFFFFF) - 1] + 1;
			}
		}
		Sort();
	}

	internal static int EncodeHasCustomAttribute(int token)
	{
		return (token >> 24) switch
		{
			6 => ((token & 0xFFFFFF) << 5) | 0, 
			4 => ((token & 0xFFFFFF) << 5) | 1, 
			1 => ((token & 0xFFFFFF) << 5) | 2, 
			2 => ((token & 0xFFFFFF) << 5) | 3, 
			8 => ((token & 0xFFFFFF) << 5) | 4, 
			9 => ((token & 0xFFFFFF) << 5) | 5, 
			10 => ((token & 0xFFFFFF) << 5) | 6, 
			0 => ((token & 0xFFFFFF) << 5) | 7, 
			23 => ((token & 0xFFFFFF) << 5) | 9, 
			20 => ((token & 0xFFFFFF) << 5) | 0xA, 
			17 => ((token & 0xFFFFFF) << 5) | 0xB, 
			26 => ((token & 0xFFFFFF) << 5) | 0xC, 
			27 => ((token & 0xFFFFFF) << 5) | 0xD, 
			32 => ((token & 0xFFFFFF) << 5) | 0xE, 
			35 => ((token & 0xFFFFFF) << 5) | 0xF, 
			38 => ((token & 0xFFFFFF) << 5) | 0x10, 
			39 => ((token & 0xFFFFFF) << 5) | 0x11, 
			40 => ((token & 0xFFFFFF) << 5) | 0x12, 
			42 => ((token & 0xFFFFFF) << 5) | 0x13, 
			_ => throw new InvalidOperationException(), 
		};
	}
}
