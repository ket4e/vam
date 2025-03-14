using System.IO;

namespace Octodiff.Core;

internal class SignatureWriter : ISignatureWriter
{
	private readonly BinaryWriter signatureStream;

	public SignatureWriter(Stream signatureStream)
	{
		this.signatureStream = new BinaryWriter(signatureStream);
	}

	public void WriteMetadata(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm, byte[] hash)
	{
		signatureStream.Write(BinaryFormat.SignatureHeader);
		signatureStream.Write((byte)1);
		signatureStream.Write(hashAlgorithm.Name);
		signatureStream.Write(rollingChecksumAlgorithm.Name);
		signatureStream.Write(BinaryFormat.EndOfMetadata);
	}

	public void WriteChunk(ChunkSignature signature)
	{
		signatureStream.Write(signature.Length);
		signatureStream.Write(signature.RollingChecksum);
		signatureStream.Write(signature.Hash);
	}
}
