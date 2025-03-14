namespace Octodiff.Core;

public struct DataRange
{
	public long StartOffset;

	public long Length;

	public DataRange(long startOffset, long length)
	{
		StartOffset = startOffset;
		Length = length;
	}
}
