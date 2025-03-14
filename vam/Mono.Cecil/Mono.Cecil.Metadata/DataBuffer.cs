using Mono.Cecil.PE;

namespace Mono.Cecil.Metadata;

internal sealed class DataBuffer : ByteBuffer
{
	public DataBuffer()
		: base(0)
	{
	}

	public uint AddData(byte[] data)
	{
		int result = position;
		WriteBytes(data);
		return (uint)result;
	}
}
