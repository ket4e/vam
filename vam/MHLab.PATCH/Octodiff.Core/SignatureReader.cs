using System.IO;
using MHLab.PATCH.Utilities;
using Octodiff.Diagnostics;

namespace Octodiff.Core;

public class SignatureReader : ISignatureReader
{
	private readonly IProgressReporter reporter;

	private readonly BinaryReader reader;

	public SignatureReader(string signatureFileName, IProgressReporter reporter)
	{
		this.reporter = reporter;
		reader = new BinaryReader(new FileStream(signatureFileName, FileMode.Open, FileAccess.Read));
	}

	public Signature ReadSignature()
	{
		Progress();
		byte[] strB = reader.ReadBytes(BinaryFormat.SignatureHeader.Length);
		if (!BinaryComparer.CompareArray(BinaryFormat.SignatureHeader, strB))
		{
			throw new CorruptFileFormatException("The signature file appears to be corrupt.");
		}
		if (reader.ReadByte() != 1)
		{
			throw new CorruptFileFormatException("The signature file uses a newer file format than this program can handle.");
		}
		string algorithm = reader.ReadString();
		string algorithm2 = reader.ReadString();
		if (!BinaryComparer.CompareArray(strB: reader.ReadBytes(BinaryFormat.EndOfMetadata.Length), strA: BinaryFormat.EndOfMetadata))
		{
			throw new CorruptFileFormatException("The signature file appears to be corrupt.");
		}
		Progress();
		IHashAlgorithm hashAlgorithm = SupportedAlgorithms.Hashing.Create(algorithm);
		Signature signature = new Signature(hashAlgorithm, SupportedAlgorithms.Checksum.Create(algorithm2));
		int hashLength = hashAlgorithm.HashLength;
		long num = 0L;
		long length = reader.BaseStream.Length;
		long num2 = length - reader.BaseStream.Position;
		int num3 = 6 + hashLength;
		if (num2 % num3 != 0L)
		{
			throw new CorruptFileFormatException("The signature file appears to be corrupt; at least one chunk has data missing.");
		}
		while (reader.BaseStream.Position < length - 1)
		{
			short num4 = reader.ReadInt16();
			uint rollingChecksum = reader.ReadUInt32();
			byte[] hash = reader.ReadBytes(hashLength);
			signature.Chunks.Add(new ChunkSignature
			{
				StartOffset = num,
				Length = num4,
				RollingChecksum = rollingChecksum,
				Hash = hash
			});
			num += num4;
			Progress();
		}
		return signature;
	}

	private void Progress()
	{
		reporter.ReportProgress("Reading signature", reader.BaseStream.Position, reader.BaseStream.Length);
	}
}
