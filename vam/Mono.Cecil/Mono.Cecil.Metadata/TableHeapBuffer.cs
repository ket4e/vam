using System;

namespace Mono.Cecil.Metadata;

internal sealed class TableHeapBuffer : HeapBuffer
{
	private readonly ModuleDefinition module;

	private readonly MetadataBuilder metadata;

	internal readonly TableInformation[] table_infos = new TableInformation[58];

	internal readonly MetadataTable[] tables = new MetadataTable[58];

	private bool large_string;

	private bool large_blob;

	private bool large_guid;

	private readonly int[] coded_index_sizes = new int[14];

	private readonly Func<Table, int> counter;

	internal uint[] string_offsets;

	public override bool IsEmpty => false;

	public TableHeapBuffer(ModuleDefinition module, MetadataBuilder metadata)
		: base(24)
	{
		this.module = module;
		this.metadata = metadata;
		counter = GetTableLength;
	}

	private int GetTableLength(Table table)
	{
		return (int)table_infos[(uint)table].Length;
	}

	public TTable GetTable<TTable>(Table table) where TTable : MetadataTable, new()
	{
		TTable val = (TTable)tables[(uint)table];
		if (val != null)
		{
			return val;
		}
		val = new TTable();
		tables[(uint)table] = val;
		return val;
	}

	public void WriteBySize(uint value, int size)
	{
		if (size == 4)
		{
			WriteUInt32(value);
		}
		else
		{
			WriteUInt16((ushort)value);
		}
	}

	public void WriteBySize(uint value, bool large)
	{
		if (large)
		{
			WriteUInt32(value);
		}
		else
		{
			WriteUInt16((ushort)value);
		}
	}

	public void WriteString(uint @string)
	{
		WriteBySize(string_offsets[@string], large_string);
	}

	public void WriteBlob(uint blob)
	{
		WriteBySize(blob, large_blob);
	}

	public void WriteGuid(uint guid)
	{
		WriteBySize(guid, large_guid);
	}

	public void WriteRID(uint rid, Table table)
	{
		WriteBySize(rid, table_infos[(uint)table].IsLarge);
	}

	private int GetCodedIndexSize(CodedIndex coded_index)
	{
		int num = coded_index_sizes[(int)coded_index];
		if (num != 0)
		{
			return num;
		}
		return coded_index_sizes[(int)coded_index] = coded_index.GetSize(counter);
	}

	public void WriteCodedRID(uint rid, CodedIndex coded_index)
	{
		WriteBySize(rid, GetCodedIndexSize(coded_index));
	}

	public void WriteTableHeap()
	{
		WriteUInt32(0u);
		WriteByte(GetTableHeapVersion());
		WriteByte(0);
		WriteByte(GetHeapSizes());
		WriteByte(10);
		WriteUInt64(GetValid());
		WriteUInt64(55193285546867200uL);
		WriteRowCount();
		WriteTables();
	}

	private void WriteRowCount()
	{
		for (int i = 0; i < tables.Length; i++)
		{
			MetadataTable metadataTable = tables[i];
			if (metadataTable != null && metadataTable.Length != 0)
			{
				WriteUInt32((uint)metadataTable.Length);
			}
		}
	}

	private void WriteTables()
	{
		for (int i = 0; i < tables.Length; i++)
		{
			MetadataTable metadataTable = tables[i];
			if (metadataTable != null && metadataTable.Length != 0)
			{
				metadataTable.Write(this);
			}
		}
	}

	private ulong GetValid()
	{
		ulong num = 0uL;
		for (int i = 0; i < tables.Length; i++)
		{
			MetadataTable metadataTable = tables[i];
			if (metadataTable != null && metadataTable.Length != 0)
			{
				metadataTable.Sort();
				num |= (ulong)(1L << i);
			}
		}
		return num;
	}

	public void ComputeTableInformations()
	{
		if (metadata.metadata_builder != null)
		{
			ComputeTableInformations(metadata.metadata_builder.table_heap);
		}
		ComputeTableInformations(metadata.table_heap);
	}

	private void ComputeTableInformations(TableHeapBuffer table_heap)
	{
		MetadataTable[] array = table_heap.tables;
		for (int i = 0; i < array.Length; i++)
		{
			MetadataTable metadataTable = array[i];
			if (metadataTable != null && metadataTable.Length > 0)
			{
				table_infos[i].Length = (uint)metadataTable.Length;
			}
		}
	}

	private byte GetHeapSizes()
	{
		byte b = 0;
		if (metadata.string_heap.IsLarge)
		{
			large_string = true;
			b = (byte)(b | 1u);
		}
		if (metadata.guid_heap.IsLarge)
		{
			large_guid = true;
			b = (byte)(b | 2u);
		}
		if (metadata.blob_heap.IsLarge)
		{
			large_blob = true;
			b = (byte)(b | 4u);
		}
		return b;
	}

	private byte GetTableHeapVersion()
	{
		TargetRuntime runtime = module.Runtime;
		if (runtime == TargetRuntime.Net_1_0 || runtime == TargetRuntime.Net_1_1)
		{
			return 1;
		}
		return 2;
	}

	public void FixupData(uint data_rva)
	{
		FieldRVATable table = GetTable<FieldRVATable>(Table.FieldRVA);
		if (table.length != 0)
		{
			int num = (GetTable<FieldTable>(Table.Field).IsLarge ? 4 : 2);
			int num2 = position;
			position = table.position;
			for (int i = 0; i < table.length; i++)
			{
				uint num3 = ReadUInt32();
				position -= 4;
				WriteUInt32(num3 + data_rva);
				position += num;
			}
			position = num2;
		}
	}
}
