using System;

namespace Octodiff.Core;

public class ChunkSignature
{
	public long StartOffset;

	public short Length;

	public byte[] Hash;

	public uint RollingChecksum;

	public override string ToString()
	{
		return string.Format("{0,6}:{1,6} |{2,20}| {3}", StartOffset, Length, RollingChecksum, BitConverter.ToString(Hash).ToLowerInvariant().Replace("-", ""));
	}
}
