using System;
using System.IO;
using IKVM.Reflection.Metadata;

namespace IKVM.Reflection.Reader;

internal sealed class MetadataReader : MetadataRW
{
	private readonly Stream stream;

	private const int bufferLength = 2048;

	private readonly byte[] buffer = new byte[2048];

	private int pos = 2048;

	internal MetadataReader(ModuleReader module, Stream stream, byte heapSizes)
		: base(module, (heapSizes & 1) != 0, (heapSizes & 2) != 0, (heapSizes & 4) != 0)
	{
		this.stream = stream;
	}

	private void FillBuffer(int needed)
	{
		int i = 2048 - pos;
		if (i != 0)
		{
			Buffer.BlockCopy(buffer, pos, buffer, 0, i);
		}
		pos = 0;
		int num;
		for (; i < needed; i += num)
		{
			num = stream.Read(buffer, i, 2048 - i);
			if (num == 0)
			{
				throw new BadImageFormatException();
			}
		}
		if (i != 2048)
		{
			Buffer.BlockCopy(buffer, 0, buffer, 2048 - i, i);
			pos = 2048 - i;
		}
	}

	internal ushort ReadUInt16()
	{
		return (ushort)ReadInt16();
	}

	internal short ReadInt16()
	{
		if (pos > 2046)
		{
			FillBuffer(2);
		}
		byte num = buffer[pos++];
		byte b = buffer[pos++];
		return (short)(num | (b << 8));
	}

	internal int ReadInt32()
	{
		if (pos > 2044)
		{
			FillBuffer(4);
		}
		byte num = buffer[pos++];
		byte b = buffer[pos++];
		byte b2 = buffer[pos++];
		byte b3 = buffer[pos++];
		return num | (b << 8) | (b2 << 16) | (b3 << 24);
	}

	private int ReadIndex(bool big)
	{
		if (big)
		{
			return ReadInt32();
		}
		return ReadUInt16();
	}

	internal int ReadStringIndex()
	{
		return ReadIndex(bigStrings);
	}

	internal int ReadGuidIndex()
	{
		return ReadIndex(bigGuids);
	}

	internal int ReadBlobIndex()
	{
		return ReadIndex(bigBlobs);
	}

	internal int ReadResolutionScope()
	{
		int num = ReadIndex(bigResolutionScope);
		return (num & 3) switch
		{
			0 => (0 << 24) + (num >> 2), 
			1 => (26 << 24) + (num >> 2), 
			2 => (35 << 24) + (num >> 2), 
			3 => (1 << 24) + (num >> 2), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadTypeDefOrRef()
	{
		int num = ReadIndex(bigTypeDefOrRef);
		return (num & 3) switch
		{
			0 => (2 << 24) + (num >> 2), 
			1 => (1 << 24) + (num >> 2), 
			2 => (27 << 24) + (num >> 2), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadMemberRefParent()
	{
		int num = ReadIndex(bigMemberRefParent);
		return (num & 7) switch
		{
			0 => (2 << 24) + (num >> 3), 
			1 => (1 << 24) + (num >> 3), 
			2 => (26 << 24) + (num >> 3), 
			3 => (6 << 24) + (num >> 3), 
			4 => (27 << 24) + (num >> 3), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadHasCustomAttribute()
	{
		int num = ReadIndex(bigHasCustomAttribute);
		return (num & 0x1F) switch
		{
			0 => (6 << 24) + (num >> 5), 
			1 => (4 << 24) + (num >> 5), 
			2 => (1 << 24) + (num >> 5), 
			3 => (2 << 24) + (num >> 5), 
			4 => (8 << 24) + (num >> 5), 
			5 => (9 << 24) + (num >> 5), 
			6 => (10 << 24) + (num >> 5), 
			7 => (0 << 24) + (num >> 5), 
			8 => throw new BadImageFormatException(), 
			9 => (23 << 24) + (num >> 5), 
			10 => (20 << 24) + (num >> 5), 
			11 => (17 << 24) + (num >> 5), 
			12 => (26 << 24) + (num >> 5), 
			13 => (27 << 24) + (num >> 5), 
			14 => (32 << 24) + (num >> 5), 
			15 => (35 << 24) + (num >> 5), 
			16 => (38 << 24) + (num >> 5), 
			17 => (39 << 24) + (num >> 5), 
			18 => (40 << 24) + (num >> 5), 
			19 => (42 << 24) + (num >> 5), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadCustomAttributeType()
	{
		int num = ReadIndex(bigCustomAttributeType);
		return (num & 7) switch
		{
			2 => (6 << 24) + (num >> 3), 
			3 => (10 << 24) + (num >> 3), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadMethodDefOrRef()
	{
		int num = ReadIndex(bigMethodDefOrRef);
		return (num & 1) switch
		{
			0 => (6 << 24) + (num >> 1), 
			1 => (10 << 24) + (num >> 1), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadHasConstant()
	{
		int num = ReadIndex(bigHasConstant);
		return (num & 3) switch
		{
			0 => (4 << 24) + (num >> 2), 
			1 => (8 << 24) + (num >> 2), 
			2 => (23 << 24) + (num >> 2), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadHasSemantics()
	{
		int num = ReadIndex(bigHasSemantics);
		return (num & 1) switch
		{
			0 => (20 << 24) + (num >> 1), 
			1 => (23 << 24) + (num >> 1), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadHasFieldMarshal()
	{
		int num = ReadIndex(bigHasFieldMarshal);
		return (num & 1) switch
		{
			0 => (4 << 24) + (num >> 1), 
			1 => (8 << 24) + (num >> 1), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadHasDeclSecurity()
	{
		int num = ReadIndex(bigHasDeclSecurity);
		return (num & 3) switch
		{
			0 => (2 << 24) + (num >> 2), 
			1 => (6 << 24) + (num >> 2), 
			2 => (32 << 24) + (num >> 2), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadTypeOrMethodDef()
	{
		int num = ReadIndex(bigTypeOrMethodDef);
		return (num & 1) switch
		{
			0 => (2 << 24) + (num >> 1), 
			1 => (6 << 24) + (num >> 1), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadMemberForwarded()
	{
		int num = ReadIndex(bigMemberForwarded);
		return (num & 1) switch
		{
			0 => (4 << 24) + (num >> 1), 
			1 => (6 << 24) + (num >> 1), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadImplementation()
	{
		int num = ReadIndex(bigImplementation);
		return (num & 3) switch
		{
			0 => (38 << 24) + (num >> 2), 
			1 => (35 << 24) + (num >> 2), 
			2 => (39 << 24) + (num >> 2), 
			_ => throw new BadImageFormatException(), 
		};
	}

	internal int ReadField()
	{
		return ReadIndex(bigField);
	}

	internal int ReadMethodDef()
	{
		return ReadIndex(bigMethodDef);
	}

	internal int ReadParam()
	{
		return ReadIndex(bigParam);
	}

	internal int ReadProperty()
	{
		return ReadIndex(bigProperty);
	}

	internal int ReadEvent()
	{
		return ReadIndex(bigEvent);
	}

	internal int ReadTypeDef()
	{
		return ReadIndex(bigTypeDef) | 0x2000000;
	}

	internal int ReadGenericParam()
	{
		return ReadIndex(bigGenericParam) | 0x2A000000;
	}

	internal int ReadModuleRef()
	{
		return ReadIndex(bigModuleRef) | 0x1A000000;
	}
}
